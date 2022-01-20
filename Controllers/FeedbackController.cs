using System;
using System.IO;
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

namespace Adorable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        [HttpGet]
        public JsonResult Get()
        {
            var feedbacks = database.ReadAll("Feedbacks");
            return new JsonResult(feedbacks);
        }
        [HttpPost]
        public JsonResult Post(Feedbacks feedback)
        {
            feedback.Id = Guid.NewGuid().ToString();
            var isCreated = database.Create("Feedbacks",feedback.Id,feedback);
            if (isCreated)
            {
                return new JsonResult(feedback.Id);
            }
            else 
            {
                return new JsonResult("failed");
            } 
        }
        [HttpDelete("{id}")]
        public JsonResult Delete(string id)
        {
            var isDeleted = database.Delete("Feedbacks",id);
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