using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using Xap.Infrastructure.Exceptions;
using Xap.Password.Factory.Interfaces;

namespace Xap.Password.CyberArk {
    public class Provider : IXapPasswordProvider {
        IXapPasswordContext IXapPasswordProvider.RetrievePassword(IXapPasswordContext passwordContext) {
            try {
                string requestUrl = string.Format("AppId={0}&Safe={1}&Folder=Root&UserName={2}", passwordContext.VaultAppId, passwordContext.VaultSafe,passwordContext.VaultUserId);

                string jsonData = GET(passwordContext.VaultUrl + requestUrl);
                JObject json = JObject.Parse(jsonData);
                passwordContext.Password = json["Content"].ToString();

                return passwordContext;
            } catch (Exception ex) {
                throw new XapException($"Error retrieving password cyberark for {passwordContext.VaultUserId}",ex);
            }
        }

        string GET(string url) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream()) {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            } catch (WebException ex) {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream()) {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        public Provider() { }
    }
}
