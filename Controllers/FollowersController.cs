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
    public class FollowersController : ControllerBase
    {
        [HttpGet]
        public JsonResult Get()
        {
            var allFollowers = database.ReadAll("Followers");//If I ever have need of all the followers
            return new JsonResult(allFollowers);
        }
        [HttpPut("{userId}")]
        public JsonResult GetAllFollowers(string userId)
        {
            var followers = database.ReadAll("Followers/"+userId);
            return new JsonResult(followers);
        }
        [HttpPost]
        public JsonResult Post(Followers follower)
        {
            database.Create("Followers",follower.UserFollowing,follower);
            var isCreated = database.Create("Followers/"+follower.BrandFollowed,follower.UserFollowing,follower.UserFollowing);
            if (isCreated)
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
            var follower = JsonConvert.DeserializeObject<Followers>(id);
            database.Delete("Followers",follower.UserFollowing);
            var isDeleted = database.Delete("Followers/"+follower.BrandFollowed,follower.UserFollowing);
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