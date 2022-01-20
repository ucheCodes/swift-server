using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public class CommentController : ControllerBase
    {
        private readonly IHubContext<SignalrHub,ISignalrHub> hubContext;
        public CommentController(IHubContext<SignalrHub,ISignalrHub> _hubContext)
        {
            hubContext = _hubContext;
        }
        [HttpGet]
        public JsonResult Get()
        {
            var comments = database.ReadAll("Comments");
            return new JsonResult(comments);
        }
        [HttpGet("{productId}")]
        public JsonResult ReadComments(string productId)
        {
            var comments = database.ReadAll("Comments/"+productId);
            return new JsonResult(comments);
        }
        [HttpPost]
        public async Task<IActionResult> Post(Comments comment)
        {
            comment.Id = Guid.NewGuid().ToString();
             database.Create("Comments",comment.Id,comment);
            var isCreated = database.Create("Comments/"+comment.ProductId,comment.Id,comment);
            if (isCreated)
            {
                await hubContext.Clients.All.NewProductComment(comment);
                return new JsonResult("success");
            }
            else 
            {
                return new JsonResult("failed");
            } 
        }
        [HttpPut]
        public JsonResult Put(Comments comment)
        {
             database.Create("Comments",comment.Id,comment);
            var isUpdated = database.Create("Comments/"+comment.ProductId,comment.Id,comment);
            if (isUpdated)
            {
                return new JsonResult("success");
            }
            else 
            {
                return new JsonResult("failed");
            }   
        }
         [HttpDelete("{id}")]
        public JsonResult Delete(string id)
        {
            var comment = JsonConvert.DeserializeObject<Comments>(id);
             database.Delete("Comments",comment.Id);
            var isDeleted = database.Delete("Comments/"+comment.ProductId,comment.Id);
           if (isDeleted)
           {
                return new JsonResult("success");
           }
           else
           {
                return new JsonResult("failed");
           }
        }
    }
}