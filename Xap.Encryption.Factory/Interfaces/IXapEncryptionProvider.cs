namespace Xap.Encryption.Factory.Interfaces {
    public interface IXapEncryptionProvider   {
        IXapEncryptionProvider EncryptionKey(string key);
        IXapEncryptionProvider SaltPhrase(string saltPhrase);
        IXapEncryptionProvider VectorPhrase(string vectorPhrase);
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}
