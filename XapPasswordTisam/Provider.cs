using System;
using System.Text;
using System.Xml;
using Xap.Encryption.Factory;
using Xap.Encryption.Factory.Interfaces;
using Xap.Encryption.Factory.Providers;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Interfaces.Web;
using Xap.Password.Factory.Interfaces;
using Xap.Web.SoapRequest;

namespace Xap.Password.Tisam {
    public class Provider : IXapPasswordProvider {
        private IXapEncryptionProvider encryptor;

        //TODO: look at injecting xapsoaprequest and encryption provider
        IXapPasswordContext IXapPasswordProvider.RetrievePassword(IXapPasswordContext passwordContext) {
            IXapSoapRequest tisamRequest = null;
            try {
                encryptor = EncyptorFactory.Instance.LoadEncryptionProvider(EncryptorProviderType.Rc4);

               
                tisamRequest = XapSoapRequest.Create("TisamVerify", passwordContext.VaultUrl)
                .AddBodyParameter("sKey", passwordContext.VaultKey)
                .AddBodyParameter("sUserID", passwordContext.VaultUserId);

                XmlDocument response = tisamRequest.ExecuteXmlRequest();
                XmlNode retVal = response.SelectSingleNode("/*[local-name()='Envelope']/*[local-name()='Body']/*[local-name()='VerifyResponse']/*[local-name()='VerifyResult'][1]");
                if (retVal == null) {
                    passwordContext.Password = string.Empty;
                } else {
                    byte[] bpwd = Convert.FromBase64String(retVal.InnerText);
                    string epwd = Encoding.UTF8.GetString(bpwd);
                    passwordContext.Password = encryptor.EncryptionKey(passwordContext.VaultKey).Decrypt(epwd);
                }

                return passwordContext;
            } catch (Exception ex) {
                throw new XapException($"Error retrieving password for {passwordContext.VaultUserId}",ex);
            }
        }

        public Provider() { }
    }
}
