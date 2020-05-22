using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xap.Infrastructure.Interfaces.Encryption;
using Xap.Infrastructure.Logging;

namespace Xap.Encryption.TripleDes {
    public class Provider:IXapEncryptionProvider {
        #region "Constructors"
        public Provider() {

        }
        #endregion
        #region "private properties"
        // define the triple des provider 
        private TripleDESCryptoServiceProvider m_des = new TripleDESCryptoServiceProvider();

        // define the string handler 
        private UTF8Encoding m_utf8 = new UTF8Encoding();
        #endregion

        #region "interface properties"
        private string _encryptionKey = "XapEncryptionKey";
        IXapEncryptionProvider IXapEncryptionProvider.EncryptionKey(string key) {
            _encryptionKey = key;
            return this;
        }

        private string _saltPhrase = string.Empty;
        IXapEncryptionProvider IXapEncryptionProvider.SaltPhrase(string saltPhrase) {
            _saltPhrase = saltPhrase;
            return this;
        }

        private string _vectorPhrase = string.Empty;
        IXapEncryptionProvider IXapEncryptionProvider.VectorPhrase(string vectorPhrase) {
            _vectorPhrase = vectorPhrase;
            return this;
        }
        #endregion

        #region "interface methods"
        string IXapEncryptionProvider.Encrypt(string plainText) {
            try {
                GetKeys();
                byte[] input = m_utf8.GetBytes(plainText);
                byte[] output = Transform(input, m_des.CreateEncryptor(m_des.Key, m_des.IV));
                return Convert.ToBase64String(output);
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error encrypting text");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        string IXapEncryptionProvider.Decrypt(string encryptedText) {
            try {
                GetKeys();
                byte[] input = Convert.FromBase64String(encryptedText);
                byte[] output = Transform(input, m_des.CreateDecryptor(m_des.Key, m_des.IV));
                return m_utf8.GetString(output);
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error decrypting text");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }
        #endregion

        #region "private methods"
        private byte[] Transform(byte[] input, ICryptoTransform CryptoTransform) {
            // create the necessary streams 
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memStream, CryptoTransform, CryptoStreamMode.Write);
            // transform the bytes as requested 
            cryptStream.Write(input, 0, input.Length);
            cryptStream.FlushFinalBlock();
            // Read the memory stream and convert it back into byte array 
            memStream.Position = 0;
            byte[] result = new byte[(System.Int32)memStream.Length - 1 + 1];
            memStream.Read(result, 0, (System.Int32)result.Length);
            // close and release the streams 
            memStream.Close();
            cryptStream.Close();
            // hand back the encrypted buffer 
            return result;
        }

        private void GetKeys() {
            MD5 md5 = new MD5CryptoServiceProvider();
            m_des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(_encryptionKey));
            m_des.IV = new byte[m_des.BlockSize / 8];
        }
        #endregion
    }
}
