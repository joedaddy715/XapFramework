using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Extensions;
using Xap.Infrastructure.Interfaces.Web;

namespace Xap.Web.RestRequest {
    public class XapRestRequest : IXapRestRequest {
        #region "Constructors"
        private XapRestRequest(string endPoint) {
            _endPoint = endPoint;
        }

        public static XapRestRequest Create(string endPoint) {
            return new XapRestRequest(endPoint);
        }
        #endregion

        #region "properties"
        private XapCache<string, string> _queryParameters = new XapCache<string, string>();
        private XapCache<string, string> _appendEndpointParameters = new XapCache<string, string>();

        private string _endPoint = string.Empty;
        string IXapRestRequest.EndPoint {
            get => _endPoint;
        }
        #endregion

        #region "public methods"
        IXapRestRequest IXapRestRequest.AppendEndpoint(string name, string value) {
            _appendEndpointParameters.AddItem(name, value);
            return this;
        }

        IXapRestRequest IXapRestRequest.AddQueryParameter(string name, string value) {
            _queryParameters.AddItem(name, value);
            return this;
        }

        string IXapRestRequest.GetResponse() {
            try {
                SetQueryParameters();
                return GET(_endPoint);
            } catch (Exception ex) {
                throw new XapException("Error processing request",ex);
            }
        }
        #endregion

        #region "private methods"
        private void SetQueryParameters() {
            StringBuilder sb = new StringBuilder();
            try {
                sb.Append(_endPoint);

                if (_appendEndpointParameters.Count > 0) {
                    foreach (KeyValuePair<string, string> kvp in _appendEndpointParameters.GetItems()) {
                        sb.Append($"{kvp.Value}/");
                    }

                    sb = sb.RemoveLast(@"/");
                    sb.Append("?");
                }

                foreach (KeyValuePair<string, string> kvp in _queryParameters.GetItems()) {
                    sb.Append($"{kvp.Key}={kvp.Value}&");
                }

                sb = sb.RemoveLast("&");
                _endPoint = sb.ToString();
            } catch (Exception ex) {
                throw new XapException("Error setting query parameters of request",ex);
            }
        }

        private string GET(string url) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try {
                //NetworkCredential credential = new NetworkCredential("userid", "password");
                //request.Credentials = credential;

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
                    throw new XapException(errorText);
                }
                throw;
            }
        }
        #endregion
    }
}
