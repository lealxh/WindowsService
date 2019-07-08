using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Infrastructure
{

    public class SlackClient:ISlackClient 
    {
        private readonly Uri _uri;
        private readonly string _username;
        private readonly string _channel;

        private readonly Encoding _encoding = new UTF8Encoding();
        private readonly ILogService logService;

        public SlackClient(ILogService logService)
    {
        string urlWithAccessToken = ConfigurationManager.AppSettings["SlackUrl"];
        _username= ConfigurationManager.AppSettings["SlackUserName"]; 
        _channel=ConfigurationManager.AppSettings["SlackChannel"]; 

        _uri = new Uri(urlWithAccessToken);
            this.logService = logService;
        }


    public void PostMessage(string text)
    {
            PostMessage(new Payload()
            {
                    Channel = _channel,
                    Username = _username,
                    Text = text
            });
     
    }


    public void PostMessage(Payload payload)
    {
            try
            {
                string payloadJson = JsonConvert.SerializeObject(payload);

                using (WebClient client = new WebClient())
                {
                    NameValueCollection data = new NameValueCollection();
                    data["payload"] = payloadJson;
                    var response = client.UploadValues(_uri, "POST", data);
                    string responseText = _encoding.GetString(response);
                }

            }
            catch (System.Net.WebException ex)
            {
                logService.Log(LogLevel.Error, "Error enviando mensaje de slack :"+ex.Message);
            }
            
            catch (Exception ex)
            {
                 logService.Log(LogLevel.Error, ex.ToString());
            }
      
    }


}

   

}
