using System;
using System.IO;
using System.Linq;
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
    public class ConnectionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public ConnectionController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        [HttpDelete("{id}")]
        public JsonResult Delete(string id)
        {
            var isDeleted = database.Delete("SignalrConnections",id);
            return new JsonResult(isDeleted);
        }
        [HttpGet]
        public JsonResult Get()
        {
            var connectedUsers = database.ReadAll("SignalrConnections");
            return new JsonResult(connectedUsers);
        }
        [HttpPost]
        public JsonResult Post(Connections connection)
        {
           var isCreated = database.Create("SignalrConnections",connection.SignalrId, connection);
           if (isCreated)
           {
                return new JsonResult("connected"); 
           }
           else
           {
               return new JsonResult("connection failed");
           }
        }
                [HttpPut("{id}")]
        public JsonResult Online(string id)
        {
            return new JsonResult("use for some other axios operation");
        }
    }
}