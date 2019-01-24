using System.Linq;
using System.Net;

namespace SamFirm
{
    public static class WebRequestExtension
    {
        public static WebResponse GetResponseFUS(this WebRequest wr)
        {
            try
            {
                WebResponse response = wr.GetResponse();
                if (response.Headers.AllKeys.Contains<string>("Set-Cookie"))
                {
                    Web.JSessionID = response.Headers["Set-Cookie"].Replace("JSESSIONID=", "").Split(new char[] { ';' })[0];
                }
                if (response.Headers.AllKeys.Contains<string>("NONCE"))
                {
                    Web.Nonce = response.Headers["NONCE"];
                }
                return response;
            }
            catch (WebException exception)
            {
                Logger.WriteLine("Error getting response: " + exception.Message);
                if (exception.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    Web.SetReconnect();
                }
                return exception.Response;
            }
        }
    }
}