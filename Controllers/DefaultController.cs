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
    public class DefaultController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public DefaultController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        [HttpGet]
        public JsonResult Get()
        {
            var users = database.ReadAll("Users");
            return new JsonResult(users);
        }
        [Route("update")]
        [HttpPost]
        public JsonResult Update(Users users)
        {
            var isUpdated = database.Create("Users",users.UserId,users);
            if (isUpdated)
            {
                return new JsonResult("success");
            }
            else{
                return new JsonResult("failed");
            }
        }
        [HttpPost]
        public JsonResult Post(Users users)
        {
            users.UserId = Guid.NewGuid().ToString();
            var isCreated = database.Create("Users",users.UserId,users);
            if (isCreated)
            {
                return new JsonResult(users.UserId);
            }
            else
            {
                return new JsonResult("failed");
            }
        }
        [HttpGet("{userId}")]
        public JsonResult Read(string userId)
        {
            var user = database.Read("Users",userId);
            return new JsonResult(user);
        }
         [HttpDelete("{id}")]
        public JsonResult Delete(string id)
        {
            var isDeleted = database.Delete("Users",id);
           if (isDeleted)
           {
                return new JsonResult("success");
           }
           else
           {
                return new JsonResult("failed");
           }
        }
        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                List<string> fileList = new List<string>();
                var files = Request.Form.Files;
                if (files.Count == 1)
                {
                    var extension = Path.GetExtension(files[0].FileName);
                    var filename = Guid.NewGuid().ToString()+extension;
                    var physicalPath = _env.ContentRootPath + "/Photos/" + filename;
                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    return new JsonResult(filename); 
                }
                else if (files.Count > 1)
                {
                    foreach (var file in files)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        var filename = Guid.NewGuid().ToString()+extension;
                        var physicalPath = _env.ContentRootPath + "/Photos/" + filename;
                        using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                            fileList.Add(filename);
                        }
                    }
                    return new JsonResult(fileList);
                }
                else
                {
                    return new JsonResult("img.jpg");
                }
            }
            catch (System.Exception)
            {
                
               return new JsonResult ("img.jpg");
            }
        }
    }
}