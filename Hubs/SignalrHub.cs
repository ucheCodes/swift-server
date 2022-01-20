using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Adorable.Models;
using Adorable.Database;

namespace Adorable.Hubs
{
    public class SignalrHub : Hub<ISignalrHub>
    {
        public async Task UpdateLastChat(string id)
        {
            var ids = JsonConvert.DeserializeObject<List<string>>(id);
            var userId = ids[0]; var chatId = ids[1];
            var lastChat = database.Read("LastChat/"+userId,chatId);
            var lc = JsonConvert.DeserializeObject<LastChat>(lastChat.Value);
            lc.NumberOfPendingChats = 0;
            var isCreated = database.Create("LastChat/"+userId,chatId,lc);
            if (isCreated)
            {
                await Clients.Caller.LastChat(lc);
            }
        }
        public async Task DeleteGroupChat(string groupId, string id)
        {
            var isDeleted = database.Delete("Groups/"+groupId,id);
               if (isDeleted)
               {
                   await Clients.Group(groupId).DeleteGroupChat(groupId,id);
               }
        }
        public async Task DeleteChat(Chat chat)
        {
            database.Delete("LastChat/"+chat.Receiver,chat.Id);
            var isDeleted = database.Delete("Chats/"+chat.ChatId,chat.Id);
            if (isDeleted)
            {
                var con = Helper.GetUserSignalRId(chat.Receiver);
                foreach (var c in con)
                {
                    var time = DateTime.Parse(c.Date);
                    var now =  DateTime.Now;
                    var span = now.Subtract(time).TotalSeconds;
                    if (span < 1800)
                    {
                        await Clients.Client(c.SignalrId).DeleteChat(chat);
                    }
                }
                await Clients.Caller.DeleteChat(chat);
            }
        }
        public async Task AddUserToGroups(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var allJoinedGroup = database.ReadAll("JoinedGroup/"+userId);
                if (allJoinedGroup.Count > 0)
                {
                    foreach (var g in allJoinedGroup)
                    {
                        var group = JsonConvert.DeserializeObject<string>(g.Value);
                        if (!string.IsNullOrEmpty(group)){
                            await Groups.AddToGroupAsync(Context.ConnectionId,group);
                        }
                    }
                }
            }
        }
        public async Task JoinGroup(string userId, string groupId)
        {
            var joinGroup = database.Create("JoinedGroup/"+userId,groupId,groupId);
            if (joinGroup)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId,groupId);
                await Clients.Caller.JoinGroup(groupId);
            }
        }
        public bool LeaveGroup(string userId, string groupId)
        {
            var deleteGroup = database.Delete("JoinedGroup/"+userId,groupId);
            if (deleteGroup)
            {
                 Groups.RemoveFromGroupAsync(Context.ConnectionId,groupId);
            }
            return deleteGroup;
        }
        public async Task DeleteGroup(string groupId)
        {
            var deleteGroup = database.Delete("Groups",groupId);
            if (deleteGroup)
            {
                 await Clients.All.DeleteGroup(groupId);
            }   
        }
        public async Task DeleteProductComment(string productId, string id)
        {
            database.Delete("Comments",id);
            var isDeleted = database.Delete("Comments/"+productId,id);
            if (isDeleted)
            {
                List<string> ids = new List<string>(){productId,id};
                await Clients.All.DeleteProductComment(ids);
            }
        }
        public async Task DeleteCommentResponse(string productId, string id, string commentId)
        {
            database.Delete("Reply",id);
            var isDeleted = database.Delete("Reply/"+productId,id);
            if (isDeleted)
            {
                List<string> ids = new List<string>(){productId,id,commentId};
                await Clients.All.DeleteCommentResponse(ids);
            }
        }
        public async Task SendMessageToGroup(string groupId, Chat chat)
        {//will use this method by invocation if controller doesn't buldge
            chat.Id = Guid.NewGuid().ToString();
            var isCreated = database.Create("Groups/"+chat.ChatId,chat.Id,chat);
            if (true)
            {
                await Clients.Group(groupId).GroupChat(chat);
            }
        }
	 public string GetConnectionId() => Context.ConnectionId;
    }
}