using System;
using System.Security.Cryptography;
using System.Text;

namespace BaseFramework
{
    public class RijndaelCryptUtil
    {
        private const string M_KEY = "meevii.game.file.key.tryyourbest";

        public static string Encrypt(string pString, string pKey = "")
        {
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(pString);
            byte[] resultArray = Encrypt(toEncryptArray, pKey);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static byte[] Encrypt(byte[] pByte, string pKey = "")
        {
            if (string.IsNullOrEmpty(pKey))
            {
                pKey = M_KEY;
            }
            byte[] keyArray = Encoding.UTF8.GetBytes(pKey);
            return Encrypt(pByte, keyArray);
        }
        
        public static byte[] Encrypt(byte[] pByte, byte[] pKey)
        {
            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = pKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(pByte, 0, pByte.Length);
            return resultArray;
        }

        public static string Decrypt(string pString, string pKey = "")
        {
            byte[] toEncryptArray = Convert.FromBase64String(pString);
            byte[] resultArray = Decrypt(toEncryptArray, pKey);
            return Encoding.UTF8.GetString(resultArray);
        }

        public static byte[] Decrypt(byte[] pByte, string pKey = "")
        {
            if (string.IsNullOrEmpty(pKey))
            {
                pKey = M_KEY;
            }
            byte[] keyArray = Encoding.UTF8.GetBytes(pKey);

            return Decrypt(pByte, keyArray);
        }

        public static byte[] Decrypt(byte[] pByte, byte[] pKey)
        {
            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = pKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(pByte, 0, pByte.Length);
            return resultArray;
        }
    }
}
