using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using DBreeze;
using DBreeze.DataTypes;
using Newtonsoft.Json;
using Adorable.Models;
using Adorable.Database;
using Adorable.Hubs;

namespace Adorable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<SignalrHub,ISignalrHub> hubContext;
        public ChatsController(IConfiguration configuration, IWebHostEnvironment env, IHubContext<SignalrHub,ISignalrHub> _hubContext)
        {
            _configuration = configuration;
            _env = env;
            hubContext = _hubContext;
        }
        [Route("Lastchats")]
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            var lc = new List<LastChat>();
            var lastChats = database.ReadAll("LastChat/"+id);
            lastChats.ForEach(l => lc.Add(JsonConvert.DeserializeObject<LastChat>(l.Value)));
            return new JsonResult(lc);
        }
        [HttpPut("{id}")]
        public JsonResult UpdateLastChat(string id)
        {
            var ids = JsonConvert.DeserializeObject<List<string>>(id);
            var userId = ids[0]; var chatId = ids[1];
            var lastChat = database.Read("LastChat/"+userId,chatId);
            var lc = JsonConvert.DeserializeObject<LastChat>(lastChat.Value);
            lc.NumberOfPendingChats = 0;//use signalr to emit this data to the caller
            var isCreated = database.Create("LastChat/"+userId,chatId,lc);
            if (isCreated)
            {
                return new JsonResult(lc);
            }
            return new JsonResult("false");
        }
        [Route("PostChat")]
        [HttpPost]
        public async Task PostChat(Chat chat)
        {
            if (!string.IsNullOrEmpty(chat.ChatId))
            {
                chat.Id = Guid.NewGuid().ToString();
               var isCreated = database.Create("Chats/"+chat.ChatId,chat.Id,chat);
               if (isCreated)
               {
                    var getLastChat = database.Read("LastChat/"+chat.Receiver,chat.ChatId);
                    int numOfLastChats = 0;
                    if (getLastChat.Key != "")
                    {
                        numOfLastChats = (JsonConvert.DeserializeObject<LastChat>(getLastChat.Value)).NumberOfPendingChats;
                    }
                    else
                    {
                       numOfLastChats = 0; 
                    }
                   LastChat lc = new LastChat();
                   lc.ChatId = chat.ChatId;
                   lc.PartnerId = chat.Sender;
                   lc.DomainId = chat.Receiver;
                   lc.Date = chat.Date;
                   lc.LastMessage = chat.Body;
                   lc.NumberOfPendingChats = numOfLastChats + 1;
                   database.Create("LastChat/"+chat.Receiver,lc.ChatId,lc);
                   var c = JsonConvert.SerializeObject(chat);
                   var l = JsonConvert.SerializeObject(lc);
                   await SendSignalRChat(new List<string>(){c,l});
               }
            }
        }
        [HttpPost]
        public JsonResult Post(ChatManager manager)
        {
           var getAllChatIds = database.ReadAll("ChatIds");
           var receiver = database.Read("Users",manager.Receiver);
           var r = JsonConvert.DeserializeObject<Users>(receiver.Value);
           manager.ReceiverLastname = r.Lastname;
           manager.ReceiverFirstname = r.Firstname;
           manager.ReceiverImagePath = r.ImagePath;
           manager.ReceiverLastSeen = GetLastSeen(manager.Receiver);
           foreach (var Id in getAllChatIds)
           {
               var id = (JsonConvert.DeserializeObject<ChatIds>(Id.Value));
               if ((id.SenderId == manager.Sender && id.ReceiverId == manager.Receiver) || (id.ReceiverId == manager.Sender && id.SenderId == manager.Receiver))
               {
                   var chatList = new List<Chat>();
                   manager.ChatId = id.ChatId;
                   var chats = database.ReadAll("Chats/"+manager.ChatId);
                   chats.ForEach(c => chatList.Add(JsonConvert.DeserializeObject<Chat>(c.Value)));
                   var orderedChats = chatList.OrderBy(d => DateTime.Parse(d.Date)).ToList<Chat>();
                   manager.Chats = orderedChats;
                   return new JsonResult(manager);
               }
           }
           //if Id is not found then chat is new
           var _chatId = Guid.NewGuid().ToString();
           var chatId = new ChatIds();
           chatId.ChatId = _chatId;
           chatId.SenderId = manager.Sender;
           chatId.ReceiverId = manager.Receiver;
           var isCreated = database.Create("ChatIds",chatId.ChatId,chatId);
           if (isCreated)
           {
                manager.ChatId = chatId.ChatId;
                manager.Chats = new List<Chat>();
                return new JsonResult(manager);
           }
           else
           {
               return new JsonResult(manager);
           }
        }
        public async Task SendSignalRChat(List<string> serializedChatAndLastChat)
        {
            var chat = JsonConvert.DeserializeObject<Chat>(serializedChatAndLastChat[0]);
            var lc = JsonConvert.DeserializeObject<LastChat>(serializedChatAndLastChat[1]);
            var online = database.ReadAll("SignalrConnections");
            if (online.Count > 0)
            {
               var con = new List<Connections>();
               online.ForEach(c => con.Add(JsonConvert.DeserializeObject<Connections>(c.Value)));
               var userOnline = con.Where(c => c.UserId == chat.Receiver);
                if (userOnline.Count() > 0)
                {
                    var now =  DateTime.Now;
                    foreach (var user in userOnline)
                    {
                     var time = DateTime.Parse(user.Date);
                     TimeSpan span = now.Subtract(time);
                     if (span.TotalSeconds <= 600)
                     {
                        await hubContext.Clients.Client(user.SignalrId).NewChat(chat);
                        await hubContext.Clients.Client(user.SignalrId).LastChat(lc);
                     }
                    }
                }
            }
            await hubContext.Clients.Client(chat.SenderSignalrId ).NewChat(chat);
        }
        public string GetLastSeen(string receiverId)
        {
           var online = database.ReadAll("SignalrConnections");
            string lastSeen = "";
            if (online.Count > 0)
            {
               var con = new List<Connections>();
               online.ForEach(c => con.Add(JsonConvert.DeserializeObject<Connections>(c.Value)));
               var userOnline = con.Where(c => c.UserId == receiverId);
                if (userOnline.Count() > 0)
                {
                   Dictionary<double,TimeSpan> lt = new Dictionary<double,TimeSpan>();
                   foreach (var u in userOnline)
                   {
                       //var time = DateTime.ParseExact(u.Date.Substring(0,24), "ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    var time2 = DateTime.Parse(u.Date);
                    var now =  DateTime.Now;
                    TimeSpan span = now.Subtract(time2);
                    lt.Add(span.TotalSeconds,span);
                   }
                   var time = lt.Min(m => m.Key);
                   if (time < 60)
                   {
                       return "active now";
                   }
                   else
                   {
                       var span = lt[time];
                       lastSeen = (span.Days > 0 ? span.Days + "days " : "")
                        + (span.Hours > 0 ? span.Hours + " hr(s) " : "")
                        + (span.Minutes > 0 ? span.Minutes + "min(s) " : "")
                        + (span.Seconds > 0 ? span.Seconds + "sec(s) " : "");
                      return "last seen "+lastSeen+"ago";
                   }
               } 
            }
            return "offline";
        }
    }
}