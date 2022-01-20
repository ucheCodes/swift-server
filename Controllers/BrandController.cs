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
    public class BrandController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public BrandController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        [HttpGet]
        public JsonResult Get()
        {
            var brands = database.ReadAll("Brands");
            return new JsonResult(brands);
        }
        [Route("exist")]
        [HttpGet("{userId}")]
        public bool Exist(string userId)
        {
            var exists = database.Exists("Brands",userId);
            if (exists)
            {
                return exists;
            }
            else
            {
                return exists;
            }
        }
        [Route("read")]
        [HttpPut]
        public JsonResult Read(Brand dummyBrand)
        {
            var brand = database.Read("Brands",dummyBrand.UserId);
            return new JsonResult(brand);
        }
        [HttpPost]
        public JsonResult Post(Brand brand)
        {
            brand.Id = Guid.NewGuid().ToString();
            var isCreated = database.Create("Brands",brand.UserId,brand);
            if (isCreated)
            {
                return new JsonResult("success");
            }
            else 
            {
                return new JsonResult("failed");
            } 
        }
        [HttpPut]
        public JsonResult Put(Brand brand)
        {
            var isUpdated = database.Create("Brands",brand.UserId,brand);
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
            var isDeleted = database.Delete("Brands",id);
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