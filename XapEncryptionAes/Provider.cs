using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xap.Infrastructure.Interfaces.Encryption;
using Xap.Infrastructure.Logging;

namespace Xap.Encryption.Aes {
    public class Provider : IXapEncryptionProvider {
        #region "Constructors"
        public Provider() {

        }
        #endregion

        #region "private properties"
        private const int Keysize = 256;
        #endregion

        #region "interface properties"
        private string _encryptionKey = "YhK7,bMK]xSCpw2,";
        IXapEncryptionProvider IXapEncryptionProvider.EncryptionKey(string key) {
            _encryptionKey = key;
            return this;
        }

        private string _saltPhrase = "z7D;^b$/a7";
        IXapEncryptionProvider IXapEncryptionProvider.SaltPhrase(string saltPhrase) {
            _saltPhrase = saltPhrase;
            return this;
        }

        private string _vectorPhrase = "pemgail9uzpgzl88";
        IXapEncryptionProvider IXapEncryptionProvider.VectorPhrase(string vectorPhrase) {
            _vectorPhrase = vectorPhrase;
            return this;
        }
        #endregion

        #region "interface methods"
        string IXapEncryptionProvider.Encrypt(string plainText) {
            try {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(_vectorPhrase);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] saltArray = Encoding.ASCII.GetBytes(_saltPhrase);

                Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(_encryptionKey, saltArray);

                byte[] keyBytes = password.GetBytes(Keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                return Convert.ToBase64String(cipherTextBytes);
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error encrypting text");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        string IXapEncryptionProvider.Decrypt(string encryptedText) {
            try {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(_vectorPhrase);
                byte[] plainTextBytes = Convert.FromBase64String(encryptedText);
                byte[] saltArray = Encoding.ASCII.GetBytes(_saltPhrase);

                Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(_encryptionKey, saltArray);

                byte[] keyBytes = password.GetBytes(Keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(cipherTextBytes);
            } catch (Exception ex) {
                XapLogger.Instance.Error("Error decrypting text");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }
        #endregion
    }
}
