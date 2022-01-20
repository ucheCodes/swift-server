using System;
using Adorable.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace Adorable.Hubs
{
    public interface ISignalrHub
    {
        Task NewChat(Chat chat);
        Task GroupChat(Chat chat);
        Task NewGroup(Groups group);
        Task DeleteChat(Chat chat);
        Task DeleteGroupChat(string groupId, string id);
        Task LastChat(LastChat lc);
        Task JoinGroup(string groupId);
        Task DeleteGroup(string groupId);
        Task NewProductComment(Comments comment);
        Task NewCommentResponse(Reply reply);
        Task DeleteProductComment(List<string> ids);
        Task DeleteCommentResponse(List<string> ids);
        Task NewNotification(Notifications notif);
    }
}