using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.Utils
{
    public class HttpsWebRequest
    {
        private static bool CheckValidationResult(object sender,
            X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public static string HttpsRequest(HttpWebRequest request)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            
            request.ProtocolVersion = HttpVersion.Version10;
            request.CookieContainer = new CookieContainer();
            request.AllowAutoRedirect = true;


            //var res = request.GetResponse() as HttpWebResponse;
            //var st = res.GetResponseStream();
            //var sr = new StreamReader(st);
            //return sr.ReadToEnd();

            return null;
        }
    }
}
