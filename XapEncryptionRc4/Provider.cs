using Microsoft.VisualBasic;
using System;
using Xap.Encryption.Factory.Interfaces;
using Xap.Infrastructure.Exceptions;

namespace Xap.Encryption.Rc4 {
    public class Provider : IXapEncryptionProvider {
        #region "Constructors"
        public Provider() {

        }
        #endregion
        #region "private properties"
        private static int[] sbox = new int[257];
        private static int[] key = new int[257];
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
                return EnDeCrypt(_encryptionKey, plainText);
            } catch (Exception ex) {
                throw new XapException("Error encrypting text",ex);
            }
        }

        string IXapEncryptionProvider.Decrypt(string encryptedText) {
            try {
                return EnDeCrypt(_encryptionKey, encryptedText);
            } catch (Exception ex) {
                throw new XapException("Error decrypting text",ex);
            }
        }
        #endregion

        #region "private methods"
        private void RC4Initialize(string strPwd) {
            // Get the length of the password 
            // Instead of Len(), we need to use the Length property 
            // of the string 
            int intLength = strPwd.Length;

            // Set up our for loop. 

            // The first argument is the initializer. Here we declare a 
            // as an integer and set it equal to zero. 

            // The second argument is expression that is used to test 
            // for the loop termination. Since our arrays have 256 
            // elements and are always zero based, we need to loop as long 
            // as a is less than or equal to 255. 

            // The third argument is an iterator used to increment the 
            // value of a by one each time through the loop. Note that 
            // we can use the ++ increment notation instead of a = a + 1 
            for (int a = 0; a <= 255; a++) {
                // we use mid to get a single character from strPwd. 
                //We declare a character variable, ctmp, to hold this value. 

                // Since the return type of String.Substring is a string, 
                // we need to convert it to a char using String.ToCharArray() 
                // and specifying that we want the first value in the array, [0]. 
                key[a] = Strings.Asc(Strings.Mid(strPwd, (a % intLength) + 1, 1));
                sbox[a] = a;
            }

            // Declare an integer x and initialize it to zero. 
            int x = 0;

            for (int b = 0; b <= 255; b++) {
                x = (x + sbox[b] + key[b]) % 256;
                int tempSwap = sbox[b];
                sbox[b] = sbox[x];
                sbox[x] = tempSwap;
            }
        }

        private string EnDeCrypt(string pwd, string plaintext) {
            int i = 0;
            int j = 0;
            string cipher = "";

            // Call our method to initialize the arrays used here. 
            RC4Initialize(pwd);

            for (int a = 1; a <= Strings.Len(plaintext); a++) {
                int itmp = 0;
                i = (i + 1) % 256;
                j = (j + sbox[i]) % 256;
                itmp = sbox[i];
                sbox[i] = sbox[j];
                sbox[j] = itmp;

                int k = sbox[(sbox[i] + sbox[j]) % 256];

                int cipherby = Strings.Asc(Strings.Mid(plaintext, a, 1)) ^ k;
                cipher = cipher + Strings.Chr(cipherby);
            }


            return cipher;
        }
        #endregion
    }
}
