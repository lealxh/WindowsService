using Datatec.Infrastructure;
using Datatec.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly ISlackClient slackClient;

        public NotificationService(ISlackClient slackClient)
        {
            this.slackClient = slackClient;
        }
        public void SendNotification(string message)
        {
            Task.Run(() =>
            {
                 slackClient.PostMessage(new Payload()
                {
                    Text = message,
                    Channel = "datatec",
                    Username = "lealxh"

                });
            });
        }
    }
}
