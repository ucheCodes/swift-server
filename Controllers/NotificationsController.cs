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
    public class NotificationsController : ControllerBase
    {
        private readonly IHubContext<SignalrHub,ISignalrHub> hubContext;
        public NotificationsController(IHubContext<SignalrHub,ISignalrHub> _hubContext)
        {
            hubContext = _hubContext;
        }

        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            List<KeyValuePair<string,string>> notifications = new List<KeyValuePair<string,string>>();
            if (id == "admin")
            {
                notifications = database.ReadAll("Notifications"); 
            }
            else
            {
                notifications = database.ReadAll("Notifications/"+id);
            }
            return new JsonResult(notifications);
        }
        [HttpPost]
        public async Task<IActionResult> PostNotification(Notifications notif)//<IActionResult>
        {
           if (notif.Id == "")
           {
               notif.Id = Guid.NewGuid().ToString();
           }
           database.Create("Notifications",notif.Id,notif);
           var isCreated = database.Create("Notifications/"+notif.ReceiverId,notif.Id,notif);
           if (isCreated)
           {
              var connections = getConnectedUsers(notif.ReceiverId);
              if (connections.Count > 0)
              {
                  foreach (var con in connections)
                  {
                    await hubContext.Clients.Client(con.SignalrId).NewNotification(notif);
                  }
              }
              return new JsonResult(connections);
           }
           return new JsonResult("Something went wrong");
        }
        public List<Connections> getConnectedUsers(string id)
        {
           List<Connections> connected = new List<Connections>();
            var users = database.ReadAll("Users");
            var getAdmins = users.Where(admin => (JsonConvert.DeserializeObject<Users>(admin.Value)).IsAdmin == true || (JsonConvert.DeserializeObject<Users>(admin.Value)).IsSuperAdmin == true);
            var connectedUsers = database.ReadAll("SignalrConnections");
            if (connectedUsers.Count > 0)
            {
                foreach (var conn in connectedUsers)
                {
                    var deserializeConnection = JsonConvert.DeserializeObject<Connections>(conn.Value);
                    var now = DateTime.Now;
                    var time = DateTime.Parse(deserializeConnection.Date);
                    TimeSpan span = now.Subtract(time);
                    foreach (var user in getAdmins)
                    {
                        var userId = JsonConvert.DeserializeObject<Users>(user.Value).UserId;
                        if ((span.TotalSeconds <= 60 && deserializeConnection.UserId == id) || (span.TotalSeconds <= 60 && deserializeConnection.UserId == userId))
                        {
                            if (!connected.Contains(deserializeConnection))
                            {
                                connected.Add(deserializeConnection);
                            }
                        }
                    }
                }
            }
            return connected;
        }
    }
}