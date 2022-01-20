using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Adorable.Models;
using Adorable.Database;
using Adorable.Hubs;
using Newtonsoft.Json;


namespace Adorable.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RandomController : ControllerBase
    {
        private readonly IHubContext<SignalrHub,ISignalrHub> hubContext;
        public RandomController(IHubContext<SignalrHub,ISignalrHub> _hubContext)
        {
            hubContext = _hubContext;
        }
        [HttpGet]
        public JsonResult GetEncryptionKey()
        {
            var getKey = database.Read("Encryption","encryptionKey");
            if (getKey.Value != "")
            {
                return new JsonResult(getKey);
            }
            return new JsonResult("failed");
        }
        [HttpGet("{id}")]
        public JsonResult GetUserActiveGroups(string id)
        {
            var joinedGroups = database.ReadAll("JoinedGroup/"+id);
            return new JsonResult(joinedGroups);
        }
        [HttpPost("{ids}")]
        public bool IsMember(string ids)
        {
            var id = JsonConvert.DeserializeObject<List<string>>(ids);
            var userId = id[0]; var groupId = id[1];
            var IsMember = database.Exists("JoinedGroup/"+userId,groupId);
            return IsMember;
        }
    }
}