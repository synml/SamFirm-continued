using System;
using System.Net;

namespace SamFirm
{
    internal class KiesRequest : WebRequest
    {
        public new static HttpWebRequest Create(string requestUriString)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUriString);
            request.Headers["Cache-Control"] = "no-cache";
            request.UserAgent = "Kies2.0_FUS";
            request.Headers.Add("Authorization", "FUS nonce=\"\", signature=\"\", nc=\"\", type=\"\", realm=\"\"");
            CookieContainer container = new CookieContainer(1);
            Cookie cookie = new Cookie("JSESSIONID", Web.JSessionID);
            container.Add(new Uri(requestUriString), cookie);
            request.CookieContainer = container;
            return request;
        }
    }
}