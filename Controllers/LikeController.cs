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
    public class LikeController : ControllerBase
    {
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            var likes = database.ReadAll("Likes/"+id);
            return new JsonResult(likes);
        }
        [HttpPost("{id}")]
        public JsonResult Post(string id)
        {
            var users = new List<string>();
            var deserializeIds = JsonConvert.DeserializeObject<List<string>>(id);
            var productId = deserializeIds[0];
            var userId = deserializeIds[1];
            var likedUsers = database.ReadAll("Likes/"+productId);
            foreach (var u in likedUsers)
            {
                var deSerializeLikedUsers = JsonConvert.DeserializeObject<string>(u.Value);
                users.Add(deSerializeLikedUsers);
            }
            if (!users.Contains(userId))
            {
                var isCreated = database.Create("Likes/"+productId,userId,userId);
                if (isCreated)
                {
                    return new JsonResult("success");
                }
            }
             return new JsonResult("you already liked this product!");
        }
    }
}