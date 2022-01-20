using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Mail;
using Adorable.Database;
using Adorable.Models;
using Newtonsoft.Json;

namespace Adorable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpGet("{search}")]
        public JsonResult Search(string search)
        {
            var users = database.ReadAll("Users");
            List<Users> searchList =  new List<Users>();
            foreach (var u in users)
            {
                var deserialized = JsonConvert.DeserializeObject<Users>(u.Value);
                if (!string.IsNullOrEmpty(deserialized.Firstname) || !string.IsNullOrEmpty(deserialized.Lastname))
                {
                   if (deserialized.Firstname.ToLower().Contains(search) || deserialized.Lastname.ToLower().Contains(search))
                    {
                        searchList.Add(deserialized);
                    }
                }
            }
            return new JsonResult(searchList);
        }
        [HttpPost("{email}")]
        public JsonResult GetEmail(string email)
        {
            // List<int> num = new List<int>(){1,6,2,3,4,5,5,6,5};
            // var n = num.Where(x => x == 5);
            var users = database.ReadAll("Users");
            if (users.Count > 0)
            {
                var user = users.Where(u => JsonConvert.DeserializeObject<Users>(u.Value).Email == email);//.ToList<KeyValuePair<string,Users>>();
                return new JsonResult(user);
            }
            return new JsonResult(email+" was well received, time to process and target email");
        }
        [HttpPost]
        public JsonResult SendEmail(EmailData emailData)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(emailData.FromAddress);
                message.To.Add(new MailAddress(emailData.ToAddress));
                message.Subject = emailData.Title;
                message.IsBodyHtml = true;
                message.Body = emailData.Message;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(emailData.FromAddress, emailData.FromAddressPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return new JsonResult("success");
               // return new JsonResult(emailData);
            }
            catch (System.Exception e)
            {
                return new JsonResult(e.Message.ToString());
               // return new JsonResult(emailData);
            }
        }
    }
}