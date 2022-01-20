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
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public JsonResult Get()
        {
            var products = database.ReadAll("Products");
            return new JsonResult(products);
        }
        [HttpGet("{userId}")]
        public JsonResult ReadProducts(string userId)
        {
            var products = database.ReadAll("Products/"+userId);
            return new JsonResult(products);
        }
        [HttpPost]
        public JsonResult Post(Products product)
        {
            if (product.Id == "")
            {
                product.Id = Guid.NewGuid().ToString();
            }
             database.Create("Products",product.Id,product);
            var isCreated = database.Create("Products/"+product.UserId,product.Id,product);
            if (isCreated)
            {
                return new JsonResult(product.Id);
            }
            else 
            {
                return new JsonResult("failed");
            } 
        }
        [HttpPut]
        public JsonResult Put(Products product)
        {
            var isUpdated = database.Create("Products",product.Id,product);
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
            var deserializeId = JsonConvert.DeserializeObject<List<string>>(id);
           string userId = deserializeId[0];
           string productId = deserializeId[1];
           database.Delete("Products",productId);
           var isDeleted = database.Delete("Products/"+userId,productId);
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