using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.Web;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace QSencriptadoCSharp
{
    public class Encryption
    {
        private RSACryptoServiceProvider rsa;
        private RC2CryptoServiceProvider rc2;
        static string saltValue = "s@1tValue";//can be any string
        static string hashAlgorithm = "MD5"; //can be "MD5"  "SHA1"
        static int passwordIterations = 1;
        static string initVector = "@1B2c3D4e5F6g7H8"; //must be 16 bytes
        static int keySize = 192;        //  can be 192 or 128


        private Encryption()
        {
        }

        public static QueryString EncryptQueryString(QueryString queryString)
        {
            QueryString newQueryString = new QueryString();
            string nm = String.Empty;
            string val = String.Empty;
            foreach (string name in queryString)
            {
                nm = name;
                val = System.Web.HttpUtility.UrlEncode(queryString[name]);
                newQueryString.Add(Encryption.Encrypt(nm, DateTime.Today.ToString()), Encryption.Encrypt(val, DateTime.Today.ToString()));
            }
            return newQueryString;
        }


        public static QueryString DecryptQueryString(QueryString queryString)
        {
            QueryString newQueryString = new QueryString();
            string nm;
            string val;
            foreach (string name in queryString)
            {
                nm = Encryption.DecryptString(name, DateTime.Today.ToString());
                val = System.Web.HttpUtility.UrlDecode(Encryption.DecryptString(queryString[name], DateTime.Today.ToString()));
                newQueryString.Add(nm, val);
            }
            return newQueryString;
        }




        private static string Encrypt(string plainText, string passPhrase)
        {
            byte[] initVectorBytes;
            initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes;
            saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes;
            plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password;
            password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes;
            keyBytes = password.GetBytes((keySize / 8));
            RijndaelManaged symmetricKey;
            symmetricKey = new RijndaelManaged();
            symmetricKey.Padding = PaddingMode.Zeros;
            //  It is reasonable to set encryption mode to Cipher Block Chaining
            //  (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor;
            encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream;
            memoryStream = new MemoryStream();
            CryptoStream cryptoStream;
            cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            //  Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            //  Finish encrypting.
            cryptoStream.FlushFinalBlock();


            byte[] cipherTextBytes;
            cipherTextBytes = memoryStream.ToArray();
            
            //  Close both streams.
            cryptoStream.Close();
            memoryStream.Close();

            // Get the data back from the memory stream, and into a string
            StringBuilder ret = new StringBuilder();
            
            byte[] b = cipherTextBytes;
            
            int I;
            for (I = 0; (I <= b.Length - 1); I++)
            {
                // Format as hex
                ret.AppendFormat("{0:X2}", b[I]);
            }
            return ret.ToString();

        }


        private static string DecryptString(string cipherText, string passPhrase)
        {

            //  Convert strings defining encryption key characteristics into byte
            //  arrays. Let us assume that strings only contain ASCII codes.
            //  If strings include Unicode characters, use Unicode, UTF7, or UTF8
            //  encoding.
            if ((cipherText == String.Empty))
            {
                return "";
            }
            else
            {
                byte[] initVectorBytes;
                initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes;
                saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                //  Convert our ciphertext into a byte array.
                // Dim cipherTextBytes As Byte()
                // cipherTextBytes = Convert.FromBase64String(cipherText)
                // Dim cipherTextBytes() As Byte = Encoding.UTF8.GetBytes(cipherText)
                byte[] cipherTextBytes = new byte[Convert.ToInt32(cipherText.Length / 2 )];
                // = Encoding.UTF8.GetBytes(InputString)
                int X;
                for (X = 0; (X <= (cipherTextBytes.Length - 1)); X++)
                {
                    Int32 IJ = Convert.ToInt32(cipherText.Substring((X * 2), 2), 16);
                    System.ComponentModel.ByteConverter BT = new System.ComponentModel.ByteConverter();
                    cipherTextBytes[X] = new byte();
                    cipherTextBytes[X] = ((byte)(BT.ConvertTo(IJ, typeof(byte))));
                }
                //  First, we must create a password, from which the key will be 
                //  derived. This password will be generated from the specified 
                //  passphrase and salt value. The password will be created using
                //  the specified hash algorithm. Password creation can be done in
                //  several iterations.
                PasswordDeriveBytes password;
                password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

                //  Use the password to generate pseudo-random bytes for the encryption
                //  key. Specify the size of the key in bytes (instead of bits).
                byte[] keyBytes;
                keyBytes = password.GetBytes((keySize / 8));
                
                //  Create uninitialized Rijndael encryption object.
                RijndaelManaged symmetricKey;
                symmetricKey = new RijndaelManaged();
                symmetricKey.Padding = PaddingMode.Zeros;
                //  It is reasonable to set encryption mode to Cipher Block Chaining
                //  (CBC). Use default options for other symmetric key parameters.
                symmetricKey.Mode = CipherMode.CBC;
                //  Generate decryptor from the existing key bytes and initialization 
                //  vector. Key size will be defined based on the number of the key 
                //  bytes.
                ICryptoTransform decryptor;
                decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                //  Define memory stream which will be used to hold encrypted data.
                MemoryStream memoryStream;
                memoryStream = new MemoryStream(cipherTextBytes);
                //  Define memory stream which will be used to hold encrypted data.
                CryptoStream cryptoStream;
                cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                
                //  Since at this point we don't know what the size of decrypted data
                //  will be, allocate the buffer long enough to hold ciphertext;
                //  plaintext is never longer than ciphertext.
                byte[] plainTextBytes=new byte[cipherTextBytes.Length];

                int decryptedByteCount;
                decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);


                StringBuilder ret = new StringBuilder();
                byte[] B = memoryStream.ToArray();
                //  Close both streams.
                memoryStream.Close();
                cryptoStream.Close();
              
                //  Convert decrypted data into a string. 
                //  Let us assume that the original plaintext string was UTF8-encoded.
                string plainText;
                plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                //  Return decrypted string.
                return plainText;
            }


        }
    }
}
