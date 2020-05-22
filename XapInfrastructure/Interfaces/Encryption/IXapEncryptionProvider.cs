namespace Xap.Infrastructure.Interfaces.Encryption {
    public interface IXapEncryptionProvider {
        IXapEncryptionProvider EncryptionKey(string key);
        IXapEncryptionProvider SaltPhrase(string saltPhrase);
        IXapEncryptionProvider VectorPhrase(string vectorPhrase);
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}