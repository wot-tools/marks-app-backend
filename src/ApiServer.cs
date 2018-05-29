using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarksAppBackend
{
    class ApiServer
    {
        private static async Task RunServerAsync()
        {
//            HttpListener listener = new HttpListener();
//            listener.Prefixes.Add("http://localhost:25432/api/");
//            listener.Start();
//            SemaphoreSlim s = new SemaphoreSlim(MAX_LISTENERS_COUNT);
//            while (true)
//            {
//                await s.WaitAsync();
//#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//                listener.GetContextAsync().ContinueWith(async t =>
//                {
//                    await ProcessContext(await t);
//                    s.Release();
//                });
//#pragma warning restore CS4014
//            }
//        }

//        private static async Task ProcessContext(HttpListenerContext context)
//        {
//            var key = context.Request.Headers.GetValues("API_KEY");

//            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//            context.Response.Close();

//            using (var stream = context.Response.OutputStream)
//            using (var writer = new StreamWriter(stream, Encoding.UTF8))
//                writer.Write(JsonConvert.SerializeObject(liquids));
//            context.Response.ContentType = "application/json";
//            context.Response.Close();
        }

        //single player stats

        //clan stats

        //tank stats
    }
}
