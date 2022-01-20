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
    public class CoverImageController : ControllerBase
    {
        public JsonResult Get()
        {
            var img = database.Read("CoverImage","coverImage");
            var cover = JsonConvert.DeserializeObject<string>(img.Value);
            return new JsonResult(cover);
        }
        [HttpPost("{img}")]
        public void Post(string img)
        {
            var isCreated = database.Create("CoverImage","coverImage",img);
        }
    }
}