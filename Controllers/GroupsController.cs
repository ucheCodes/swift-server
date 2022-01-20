using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Adorable.Models;
using Adorable.Database;
using Adorable.Hubs;
using Newtonsoft.Json;


namespace Adorable.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IHubContext<SignalrHub,ISignalrHub> hubContext;
        public GroupsController(IHubContext<SignalrHub,ISignalrHub> _hubContext)
        {
            hubContext = _hubContext;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var groups = database.ReadAll("Groups");
            var groupList = new List<Groups>();
            if (groups.Count > 0)
            {
                groups.ForEach(g => groupList.Add(JsonConvert.DeserializeObject<Groups>(g.Value)));
                return new JsonResult(groupList);
            }
            return new JsonResult(new List<Groups>());
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(Groups group)
        {
           if (group.Id == "")
           {
               group.Id = Guid.NewGuid().ToString();
           }
           var isCreated = database.Create("Groups",group.Id,group);
           if (isCreated)
           {
               var joinGroup = database.Create("JoinedGroup/"+group.AdminId,group.Id,group.Id);
               await hubContext.Clients.All.NewGroup(group);
           }
           return new JsonResult(group.Id);
        }
        [Route("PostChat")]
        [HttpPost]
        public async Task PostGroupChat(Chat chat)
        {
            if (!string.IsNullOrEmpty(chat.ChatId))
            {
                chat.Id = Guid.NewGuid().ToString();
               var isCreated = database.Create("Groups/"+chat.ChatId,chat.Id,chat);
               if (isCreated)
               {
                   //If I need to wire last chat to users then i'll get code from chat
                   await hubContext.Clients.Group(chat.ChatId).GroupChat(chat);
                   //await hubContext.Clients.Client(chat.SenderSignalrId ).GroupChat(chat);
               }
            }
        }
        [HttpPut]
        public JsonResult GetGroupChats()
        {
            var groups = database.ReadAll("Groups");
            List<Chat> groupChats = new List<Chat>();
            Dictionary<string,List<Chat>> allGroupChats = new Dictionary<string,List<Chat>>();
            if (groups.Count > 0)
            {
                foreach (var g in groups)
                {
                    var id = JsonConvert.DeserializeObject<string>(g.Key);
                    var chats = database.ReadAll("Groups/"+id);
                    if (chats.Count > 0)
                    {
                        groupChats = new List<Chat>();
                        foreach (var c in chats)
                        {
                            var chat = JsonConvert.DeserializeObject<Chat>(c.Value);
                            groupChats.Add(chat);
                        }
                        groupChats = groupChats.OrderBy(d => DateTime.Parse(d.Date)).ToList<Chat>();
                        allGroupChats.Add(id,groupChats);
                    }
                }
            }
            return new JsonResult(allGroupChats);
        }
    }
}