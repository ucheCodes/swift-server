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
    public class StoryController : ControllerBase
    {
        [HttpGet]
        public JsonResult Get()
        {
            var story = database.ReadAll("Story");
            return new JsonResult(story);
        }
        [HttpGet("{userId}")]
        public JsonResult ReadStory(string userId)
        {
            var story = database.ReadAll("Story/"+userId);
            return new JsonResult(story);
        }
        [HttpPost]
        public JsonResult Post(Story story)
        {
            if (story.Id == "")
            {
                story.Id = Guid.NewGuid().ToString();
            }
             database.Create("Story",story.Id,story);
            var isCreated = database.Create("Story/"+story.UserId,story.Id,story);
            if (isCreated)
            {
                return new JsonResult(story.Id);
            }
            else 
            {
                return new JsonResult("failed");
            } 
        }
        //[HttpPut]
        // public JsonResult Put(Products product)
        // {
        //     var isUpdated = database.Create("Products",product.Id,product);
        //     if (isUpdated)
        //     {
        //         return new JsonResult("success");
        //     }
        //     else 
        //     {
        //         return new JsonResult("failed");
        //     }   
        // }

         [HttpDelete("{id}")]
        public JsonResult Delete(string id)
        {
            var deserializeId = JsonConvert.DeserializeObject<List<string>>(id);
           string userId = deserializeId[0];
           string storyId = deserializeId[1];
           var isDeleted = database.Delete("Story",storyId);
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