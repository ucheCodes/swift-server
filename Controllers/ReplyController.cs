using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
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
    public class ReplyController : ControllerBase
    {
        private readonly IHubContext<SignalrHub,ISignalrHub> hubContext;
        public ReplyController(IHubContext<SignalrHub,ISignalrHub> _hubContext)
        {
            hubContext = _hubContext;
        }    
        [HttpGet]
        public JsonResult Get()
        {
            var replies = database.ReadAll("Reply");
            return new JsonResult(replies);
        }
        [HttpGet("{productId}")]
        public JsonResult ReadReply(string productId)
        {
            var replies = database.ReadAll("Reply/"+productId);
            return new JsonResult(replies);
        }
        [HttpPost]
        public async Task Post(Reply reply)
        {
            reply.Id = Guid.NewGuid().ToString();
            database.Create("Reply",reply.Id,reply);
            var isCreated = database.Create("Reply/"+reply.ProductId,reply.Id,reply);
            if (isCreated)
            {
                await hubContext.Clients.All.NewCommentResponse(reply);
            }
        }
    }
}