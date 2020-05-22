using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Configuration;
using Xap.Infrastructure.Environment;
using Xap.Infrastructure.Interfaces.Web;
using Xap.Infrastructure.Logging;

namespace Xap.Web.SoapRequest {
    public class XapSoapRequest : IXapSoapRequest {
        private XmlDocument xmlSoapEnvelope = new XmlDocument();
        private HttpWebRequest objWebRequest = null;
        private string soapEnvelope = string.Empty;

        #region "Constructors"
        private XapSoapRequest(string envelopeName, string endPoint) {
            _envelopeName = envelopeName;
            _endPoint = endPoint;
        }

        public static IXapSoapRequest Create(string templateName, string endPoint) {
            return new XapSoapRequest(templateName, endPoint);
        }
        #endregion

        #region "Properties"
        private XapCache<string, string> _headerParameters = new XapCache<string, string>();
        private XapCache<string, string> _bodyParameters = new XapCache<string, string>();

        private string _endPoint = string.Empty;
        string IXapSoapRequest.EndPoint {
            get { return _endPoint; }
        }

        private string _envelopeName = string.Empty;
        string IXapSoapRequest.EnvelopeName {
            get { return _envelopeName; }
        }
        #endregion

        #region "interface methods"
        IXapSoapRequest IXapSoapRequest.AddHeadParameter(string name, string value) {
            _headerParameters.AddItem(name, value);
            return this;
        }

        IXapSoapRequest IXapSoapRequest.AddBodyParameter(string name, string value) {
            _bodyParameters.AddItem(name, value);
            return this;
        }

        XmlDocument IXapSoapRequest.ExecuteXmlRequest() {
            XmlDocument response = new XmlDocument();
            StreamReader reader = null;

            try {
                string _envPath = XapConfig.Instance.GetValue<string>($"{XapEnvironment.Instance.EnvironmentName}.soap", "soapEnvelopes");
                objWebRequest = CreateWebRequest(_endPoint);
                reader = new StreamReader($"{_envPath}{_envelopeName}.xml");
                soapEnvelope = reader.ReadToEnd();
                reader.Close();

                if (_headerParameters.Count > 0) {
                    SetHeaderParameters();
                }

                if (_bodyParameters.Count > 0) {
                    SetBodyParameters();
                }

                xmlSoapEnvelope.LoadXml(soapEnvelope);

                InsertSoapEnvelopeIntoWebRequest();

                response.LoadXml(ReadXmlResponse());
                return response;
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error executing xml request");
                XapLogger.Instance.Write(ex.Message);
                throw;
            } finally {
                reader.Close();
            }
        }

        Stream IXapSoapRequest.ExecuteMtomRequest() {
            StreamReader reader = null;
            try {
                string _envPath = XapConfig.Instance.GetValue<string>($"{XapEnvironment.Instance.EnvironmentName}.soap", "soapEnvelopes");
                objWebRequest = CreateWebRequest(_endPoint);
                reader = new StreamReader($"{_envPath}{_envelopeName}.xml");
                soapEnvelope = reader.ReadToEnd();
                reader.Close();

                if (_headerParameters.Count > 0) {
                    SetHeaderParameters();
                }

                if (_bodyParameters.Count > 0) {
                    SetBodyParameters();
                }

                xmlSoapEnvelope.LoadXml(soapEnvelope);

                InsertSoapEnvelopeIntoWebRequest();
                return ReadMtomResponse();
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error executing mtom request");
                XapLogger.Instance.Write(ex.Message);
                throw;
            } finally {
                reader.Close();
            }
}

        private HttpWebRequest CreateWebRequest(String URL) {
            try {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(URL);

                webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                return webRequest;
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error creating soap request");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        private string ReadXmlResponse() {
            try {
                string result = string.Empty;
                using (WebResponse response = objWebRequest.GetResponse()) {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream())) {
                        result = rd.ReadToEnd();
                    }
                }
                return result;
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error reading soap response");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        private Stream ReadMtomResponse() {
            Stream retVal = null;
            try {
                string result = string.Empty;
                using (WebResponse response = objWebRequest.GetResponse()) {
                    MimeEntity mime = ParseMultipartFormData(response);
                    Multipart multiPart = (Multipart)mime;
                    retVal = (multiPart[0] as MimePart).Content.Stream;
                }
                return retVal;
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error reading soap response");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        private void InsertSoapEnvelopeIntoWebRequest() {
            try {
                using (Stream stream = objWebRequest.GetRequestStream()) {
                    xmlSoapEnvelope.Save(stream);
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error inserting envelope into soap request");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        private void SetHeaderParameters() {
            try {
                foreach (KeyValuePair<string, string> kvp in _headerParameters.GetItems()) {
                    soapEnvelope = soapEnvelope.Replace($"${kvp.Key}$", kvp.Value);
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error setting head parameters of soap request");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        private void SetBodyParameters() {
            try {
                foreach (KeyValuePair<string, string> kvp in _bodyParameters.GetItems()) {
                    soapEnvelope = soapEnvelope.Replace($"${kvp.Key}$", kvp.Value);
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error setting body parameters of soap request");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }


        #endregion

        #region "MimeKit Helpers"
        MimeEntity ParseMultipartFormData(WebResponse response) {
            var contentType = ContentType.Parse(response.ContentType);

            return MimeEntity.Load(contentType, response.GetResponseStream());
        }

        public byte[] FileToByteArray(Stream documentStream) {
            byte[] _binaryData = null;
            FileStream _fileStream = null;
            MemoryStream _memoryStream = null;
            try {
                _memoryStream = new MemoryStream();
                _binaryData = new byte[_memoryStream.Length];
                _binaryData = _memoryStream.ToArray();
                return _binaryData;
            } catch (Exception ex) {
                throw;
            } finally {
                if (_fileStream != null) {
                    _fileStream.Close();
                }
            }
        }
        #endregion
    }
}
