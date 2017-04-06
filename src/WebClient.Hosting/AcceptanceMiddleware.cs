using Microsoft.AspNetCore.Http;
using NetStandard.Contracts;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.Hosting
{
    public class AcceptanceMiddleware
    {
        private readonly RequestDelegate _next;
        public AcceptanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (ApplicationVariables.CacheReady == 0)
            {
                await NonAcceptance(context);
                return;
            }

            await _next.Invoke(context);
        }

        private async Task NonAcceptance(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status423Locked;
            context.Response.ContentType = "text/html";
            // TODO static cache
            #region Content Writer
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("<html><meta charset='utf-8'><body><h1>Uygulama bileşenleri yükleniyor...</h1>");
            strBuilder.AppendLine(@"
                <script type='text/javascript'>
                    document.addEventListener('DOMContentLoaded', function(event) {
                        function check() {
                            var xmlhttp = new XMLHttpRequest();
                            xmlhttp.onreadystatechange = function() {
                                if (xmlhttp.readyState == XMLHttpRequest.DONE)
                                {
                                    if (xmlhttp.status != 423)
                                    {
                                        window.location.href = '/';
                                    }
                                }
                            };
                            xmlhttp.open('GET', '/', true);
                            xmlhttp.send();
                        }

                        setInterval(check, 1500);
                    });
                </script>");
            strBuilder.AppendLine("</body></html>");
            #endregion
            var content = strBuilder.ToString();
            await context.Response.WriteAsync(content);
            context.Request.ContentLength = Encoding.UTF8.GetByteCount(content);
        }
    }
}
