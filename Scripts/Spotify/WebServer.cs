using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Temptica.Overlay.Scripts.Spotify
{
    public class WebServer
    {
        private readonly HttpListener _listener;

        public WebServer(string uri)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(uri);
        }

        public async Task<Authorization> Listen()
        {
            _listener.Start();
            return await OnRequest();
        }

        private async Task<Authorization> OnRequest()
        {
            while (_listener.IsListening)
            {
                var ctx = await _listener.GetContextAsync();
                var req = ctx.Request;
                var resp = ctx.Response;

                await using var writer = new StreamWriter(resp.OutputStream);
                if (req.QueryString.AllKeys.Any("code".Contains))
                {
                    await writer.WriteLineAsync("Authorization started! Check your application!");
                    await writer.FlushAsync();
                    return new Authorization(req.QueryString["code"]);
                }
                else
                {
                    await writer.WriteLineAsync("No code found in query string!");
                    await writer.FlushAsync();
                }
            }
            return null;
        }
        
        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
    
    public class Authorization
    {
        public string Code { get; }

        public Authorization(string code)
        {
            Code = code;
        }
    }
}