using DBN.EncrypDecryp;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FinanceManagement.Common
{
    public sealed class EncryptUtils
    {
        #region Base64加密解密
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns></returns>
        public static string Base64Encrypt(string input)
        {
            return Base64Encrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符编码</param>
        /// <returns></returns>
        public static string Base64Encrypt(string input, Encoding encode)
        {
            return Convert.ToBase64String(encode.GetBytes(input));
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="input">需要解密的字符串</param>
        /// <returns></returns>
        public static string Base64Decrypt(string input)
        {
            return Base64Decrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="input">需要解密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string Base64Decrypt(string input, Encoding encode)
        {
            return encode.GetString(Convert.FromBase64String(input));
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns></returns>
        public static string MD5Encrypt(string input)
        {
            return MD5Encrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string MD5Encrypt(string input, Encoding encode)
        {
            var md5 = new MD5CryptoServiceProvider();
            var t = md5.ComputeHash(encode.GetBytes(input));
            var sb = new StringBuilder(32);
            foreach (var t1 in t)
                sb.Append(t1.ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }

        /// <summary>
        /// MD5对文件流加密
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string MD5Encrypt(Stream stream)
        {
            var md5Serv = MD5.Create();
            var buffer = md5Serv.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (var var in buffer)
                sb.Append(var.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// MD5加密(返回16位加密串)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string MD5Encrypt16(string input, Encoding encode)
        {
            var md5 = new MD5CryptoServiceProvider();
            var result = BitConverter.ToString(md5.ComputeHash(encode.GetBytes(input)), 4, 8);
            result = result.Replace("-", "");
            return result;
        }
        #endregion

        #region DES加密解密
        public static string Encrypt(string text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.Default.GetBytes(text);
            des.Key = Encoding.ASCII.GetBytes(MD5Encrypt(sKey).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(MD5Encrypt(sKey).Substring(0, 8));
            var ms = new System.IO.MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var ret = new StringBuilder();
            foreach (var b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        public static string Decrypt(string text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            var len = text.Length / 2;
            var inputByteArray = new byte[len];
            int x;
            for (x = 0; x < len; x++)
            {
                var i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.ASCII.GetBytes(MD5Encrypt(sKey).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(MD5Encrypt(sKey).Substring(0, 8));
            var ms = new System.IO.MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }
        #endregion

        #region AES 加密解密

        public static byte[] AESEncrypt(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            var encrypted = new byte[] { };
            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null) return encrypted;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string AESDecrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            // Declare the string used to hold
            // the decrypted text.
            string plaintext;
            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null) return null;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = key;
                aesAlg.IV = iv;
                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {

                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        #endregion

        #region AES加密解密         
       
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文</param>
        /// <param name="aesKey">密钥</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string aesKey)
        {
            string data = string.Empty;
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(aesKey))
            {
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
                Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(aesKey);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = aes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                data = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            return data;
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">密文</param>
        /// <param name="aesKey">密钥</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string aesKey)
        {
            string data = string.Empty;
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(aesKey))
            {
                byte[] toEncryptArray = Convert.FromBase64String(str);
                Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(aesKey);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = aes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                data = Encoding.UTF8.GetString(resultArray);
            }
            return data;
        }
        #endregion

        #region 屠宰企业
        /// <summary>
        /// 判断加密id是否正确
        /// </summary>
        /// <param name="Identifier"></param>
        /// <returns></returns>
        public static bool GetERWMIdentifierCheck(string Identifier)
        {
            EncrypDecryp_MD5 oMD5 = new EncrypDecryp_MD5();
            return Identifier == oMD5.EncryptString("~二维码产品溯源~" + DateTime.Now.ToString("yyyy-MM-dd HH"));
        }
        #endregion 

        #region 屠宰企业        
        /// <summary>
        /// 判断加密id是否正确
        /// </summary>
        /// <param name="Identifier"></param>
        /// <returns></returns>
        public static bool GetTZCIdentifierCheck(string Identifier)
        {
            EncrypDecryp_MD5 oMD5 = new EncrypDecryp_MD5();
            return Identifier == oMD5.EncryptString("~屠宰产品溯源~" + DateTime.Now.ToString("yyyy-MM-dd HH"));
        }
        #endregion

        #region 企联网
        /// <summary>
        /// 判断加密id是否正确
        /// </summary>
        /// <param name="Identifier"></param>
        /// <returns></returns>
        public static bool GetQLWIdentifierCheck(string Identifier)
        {
            EncrypDecryp_MD5 oMD5 = new EncrypDecryp_MD5();
            return Identifier == oMD5.EncryptString("~企联网新OA~" + DateTime.Now.ToString("yyyy-MM-dd HH"));
        }
        #endregion

        #region 生物疫苗
        /// <summary>
        /// 判断加密id是否正确
        /// </summary>
        /// <param name="Identifier"></param>
        /// <returns></returns>
        public static bool GetQRCodelIdentifierCheck(string Identifier)
        {
            EncrypDecryp_MD5 oMD5 = new EncrypDecryp_MD5();
            return Identifier == oMD5.EncryptString("~疫苗产品溯源~" + DateTime.Now.ToString("yyyy-MM-dd HH"));
        }
        #endregion

        #region 商城
        /// <summary>
        /// 获取农信商城对OA提供的接口的密钥
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns></returns>
        public static string GetNXMallIndentifierOA(out string timeStamp)
        {
            var oMd5 = new EncrypDecryp_MD5 { esEncoding = EncrypDecryp_MD5.eEncoding.UTF8 };
            timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string sDateTickMsg = oMd5.EncryptString("~新版农信商城审核接口~" + timeStamp);
            return sDateTickMsg;
        }
        #endregion
    }
    /// <summary>
    /// 银行账号加密
    /// </summary>
    public class EncryptAccount
    {
        HostConfiguration _hostCongfiguration;
        public EncryptAccount(HostConfiguration hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sInputString"></param>
        /// <returns></returns>
        public static string getFieldEncrypt(string sInputString, string spubKeyPath, string sCertificatePass)
        {
            string sPublic = string.Empty;
            string sPrivate = string.Empty;
            //bool bReturn1 = EncrypDecrypBase.GetCertificateInfo(spubKeyPath, sCertificatePass, ref sPublic, ref sPrivate);
            //sPublic = "<RSAKeyValue><Modulus>sgNldN0nwPhYBGO5aPMqdmKeOn1jJWDROAVA/GnfjzLlHU7Pzp5NGNhEAG3aa4fBpne0sZ7OP3NpDISQameQl61RKJmdWN7qNt2Q0Q5jGH5E9+u8joRrIGNdIasc1xXHpYbeC+6viNd7+TWq8466sb1N0s0ItOrjamVSMviYqAk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            //sPrivate = "<RSAKeyValue><Modulus>sgNldN0nwPhYBGO5aPMqdmKeOn1jJWDROAVA/GnfjzLlHU7Pzp5NGNhEAG3aa4fBpne0sZ7OP3NpDISQameQl61RKJmdWN7qNt2Q0Q5jGH5E9+u8joRrIGNdIasc1xXHpYbeC+6viNd7+TWq8466sb1N0s0ItOrjamVSMviYqAk=</Modulus><Exponent>AQAB</Exponent><P>yshTemDxfbI/G1aNVA7CT3fBO8xmas2F1dLGWH0LpaekF9UrlmMBnFeleW9LiGnvl1MBGPyJWBYOaWRdPLG/+Q==</P><Q>4Lr7DYeT44EV/5F3+oIVlw7s0XpXIgQcMV5Abhv8czD9TXJeZMNLztdB9BlU5cyW7o1feZKRlGAQNcKej5VMkQ==</Q><DP>N+Sfncu6xHqtCkwBCHpI9L59dI4SbL3ZdeZy5VESNMbQZAFN0lXXy9AcvCwBFcidUYh/dPOmp7DsqAAR8vjLgQ==</DP><DQ>AS0ZWUXHYV5wlgjV3urFYCgE10fTourwltWOcsUUuimcecZKdi6LfAamYrerORSsCY2V3VYGCwfBrfZZzBiU8Q==</DQ><InverseQ>GXr54xbzMkwjq+YBQiv9Oz1MvuStJshLRhfbqi0vOhfzxOH/rqcYnMFK9ytYaLMbylbj6artAbwxd9XFxhvzug==</InverseQ><D>DuclIyihK4D3CG4LX4+NAGJ+9e1Zo69UcKzS7RbVgCTuBLs8f0MmN/RiFAmeWbaAtjWrF1wwzuPT+XfvKbNQKdjOkak+Et8hVIryC/W8DcuOYxuIUEVsUKV5jUpQhR+MiJNhyPEZLfQCC0Euq3grTH1sUJ334HYqEwgNBeZUUjE=</D></RSAKeyValue>";
            sPublic = "<RSAKeyValue><Modulus>kQwjaIM5tpuNlOnu8nXdKvFlO8tVbHjVVSVGBq1otfFP1rW1WLDqrfskQ6k5IJjZelEPJsjdar+vKlEFk5mVovteqMhUsNyUHXd81ijiVJy02AHyQTsuPgRKcNGAWvQdacotddpvFk/EjDkbSKVRod/GVP9M8TPo4g5RlUWcMXE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            sPrivate = "<RSAKeyValue><Modulus>kQwjaIM5tpuNlOnu8nXdKvFlO8tVbHjVVSVGBq1otfFP1rW1WLDqrfskQ6k5IJjZelEPJsjdar+vKlEFk5mVovteqMhUsNyUHXd81ijiVJy02AHyQTsuPgRKcNGAWvQdacotddpvFk/EjDkbSKVRod/GVP9M8TPo4g5RlUWcMXE=</Modulus><Exponent>AQAB</Exponent><P>y6xcwOjgty6+qkHsVNN3wr6nLylmN8eMccqNcwDtYVuyc2oZH2G2ZxSIKKBWRXQZYr8OWoAzMp28yVKSlqf3Tw==</P><Q>tk/wawe18i092ZbyAUy7ud1gXEw9T0+AzcedfDBnXrl6rYnvhnE2nNFo9wGHs7qPSl7zyDAzrAPUTUaVxX8bPw==</Q><DP>MTx6s7vYTxBC0V/cZOk2I2L5gYItjsBzqDKCHVIVEJsdOZ9lcVuqv6KMA9423NVjKabsLl6dgdf2AmkuvLLgKw==</DP><DQ>E6DnsZ5S6JAWaKbnx1wLmidLiKRstW1J4N3tBsHvXzN5EdYYA9GMn7WsJ2vywFcFtmwisxr9dTe0U92Von3c4Q==</DQ><InverseQ>QvYGM+Bl/TH6nZ+mqlFCW/4RawaqeSYI6X8yQ57Pqk55foPp+Fq5rcbc6fM6cFSikfBrU9GE12R2rExi1tCiyA==</InverseQ><D>J6XDv/uuuOWIK95N/GSPUf+uBkwsS3w0BBQuMgEQkDsn8a61pUqmu2vcm8oT3X8qYy7pD1b0fwtqXAzNk2z146+EXTpAepDplMhEV6d6fQeDAOiCY7EobaI4JJIZu75ZSguPsPQig39Dl+28rJXk8W0RaaT3/TAWUGGGPe8Knqk=</D></RSAKeyValue>";
            EncrypDecryp_RSA EncrypDecryp_RSA1 = new EncrypDecryp_RSA();
            EncrypDecryp_RSA1.bReverse = false;
            EncrypDecryp_RSA1.bOAEP = false;
            EncrypDecryp_RSA1.esEncoding = EncrypDecryp_RSA.eEncoding.UTF8;
            return EncrypDecryp_RSA1.Encrypt(sInputString, 1024, sPublic);
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="sInputString"></param>
        /// <returns></returns>
        public static string getFieldDecrypt(string sInputString, string spubKeyPath, string sCertificatePass)
        {
            string sPublic = string.Empty;
            string sPrivate = string.Empty;
            //bool bReturn1 = EncrypDecrypBase.GetCertificateInfo(spubKeyPath, sCertificatePass, ref sPublic, ref sPrivate);
            //sPublic = "<RSAKeyValue><Modulus>sgNldN0nwPhYBGO5aPMqdmKeOn1jJWDROAVA/GnfjzLlHU7Pzp5NGNhEAG3aa4fBpne0sZ7OP3NpDISQameQl61RKJmdWN7qNt2Q0Q5jGH5E9+u8joRrIGNdIasc1xXHpYbeC+6viNd7+TWq8466sb1N0s0ItOrjamVSMviYqAk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            //sPrivate = "<RSAKeyValue><Modulus>sgNldN0nwPhYBGO5aPMqdmKeOn1jJWDROAVA/GnfjzLlHU7Pzp5NGNhEAG3aa4fBpne0sZ7OP3NpDISQameQl61RKJmdWN7qNt2Q0Q5jGH5E9+u8joRrIGNdIasc1xXHpYbeC+6viNd7+TWq8466sb1N0s0ItOrjamVSMviYqAk=</Modulus><Exponent>AQAB</Exponent><P>yshTemDxfbI/G1aNVA7CT3fBO8xmas2F1dLGWH0LpaekF9UrlmMBnFeleW9LiGnvl1MBGPyJWBYOaWRdPLG/+Q==</P><Q>4Lr7DYeT44EV/5F3+oIVlw7s0XpXIgQcMV5Abhv8czD9TXJeZMNLztdB9BlU5cyW7o1feZKRlGAQNcKej5VMkQ==</Q><DP>N+Sfncu6xHqtCkwBCHpI9L59dI4SbL3ZdeZy5VESNMbQZAFN0lXXy9AcvCwBFcidUYh/dPOmp7DsqAAR8vjLgQ==</DP><DQ>AS0ZWUXHYV5wlgjV3urFYCgE10fTourwltWOcsUUuimcecZKdi6LfAamYrerORSsCY2V3VYGCwfBrfZZzBiU8Q==</DQ><InverseQ>GXr54xbzMkwjq+YBQiv9Oz1MvuStJshLRhfbqi0vOhfzxOH/rqcYnMFK9ytYaLMbylbj6artAbwxd9XFxhvzug==</InverseQ><D>DuclIyihK4D3CG4LX4+NAGJ+9e1Zo69UcKzS7RbVgCTuBLs8f0MmN/RiFAmeWbaAtjWrF1wwzuPT+XfvKbNQKdjOkak+Et8hVIryC/W8DcuOYxuIUEVsUKV5jUpQhR+MiJNhyPEZLfQCC0Euq3grTH1sUJ334HYqEwgNBeZUUjE=</D></RSAKeyValue>";
            sPublic = "<RSAKeyValue><Modulus>kQwjaIM5tpuNlOnu8nXdKvFlO8tVbHjVVSVGBq1otfFP1rW1WLDqrfskQ6k5IJjZelEPJsjdar+vKlEFk5mVovteqMhUsNyUHXd81ijiVJy02AHyQTsuPgRKcNGAWvQdacotddpvFk/EjDkbSKVRod/GVP9M8TPo4g5RlUWcMXE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            sPrivate = "<RSAKeyValue><Modulus>kQwjaIM5tpuNlOnu8nXdKvFlO8tVbHjVVSVGBq1otfFP1rW1WLDqrfskQ6k5IJjZelEPJsjdar+vKlEFk5mVovteqMhUsNyUHXd81ijiVJy02AHyQTsuPgRKcNGAWvQdacotddpvFk/EjDkbSKVRod/GVP9M8TPo4g5RlUWcMXE=</Modulus><Exponent>AQAB</Exponent><P>y6xcwOjgty6+qkHsVNN3wr6nLylmN8eMccqNcwDtYVuyc2oZH2G2ZxSIKKBWRXQZYr8OWoAzMp28yVKSlqf3Tw==</P><Q>tk/wawe18i092ZbyAUy7ud1gXEw9T0+AzcedfDBnXrl6rYnvhnE2nNFo9wGHs7qPSl7zyDAzrAPUTUaVxX8bPw==</Q><DP>MTx6s7vYTxBC0V/cZOk2I2L5gYItjsBzqDKCHVIVEJsdOZ9lcVuqv6KMA9423NVjKabsLl6dgdf2AmkuvLLgKw==</DP><DQ>E6DnsZ5S6JAWaKbnx1wLmidLiKRstW1J4N3tBsHvXzN5EdYYA9GMn7WsJ2vywFcFtmwisxr9dTe0U92Von3c4Q==</DQ><InverseQ>QvYGM+Bl/TH6nZ+mqlFCW/4RawaqeSYI6X8yQ57Pqk55foPp+Fq5rcbc6fM6cFSikfBrU9GE12R2rExi1tCiyA==</InverseQ><D>J6XDv/uuuOWIK95N/GSPUf+uBkwsS3w0BBQuMgEQkDsn8a61pUqmu2vcm8oT3X8qYy7pD1b0fwtqXAzNk2z146+EXTpAepDplMhEV6d6fQeDAOiCY7EobaI4JJIZu75ZSguPsPQig39Dl+28rJXk8W0RaaT3/TAWUGGGPe8Knqk=</D></RSAKeyValue>";
            EncrypDecryp_RSA EncrypDecryp_RSA1 = new EncrypDecryp_RSA();
            EncrypDecryp_RSA1.bReverse = false;
            EncrypDecryp_RSA1.bOAEP = false;
            EncrypDecryp_RSA1.esEncoding = EncrypDecryp_RSA.eEncoding.UTF8;
            return EncrypDecryp_RSA1.Decrypt(sInputString, 1024, sPrivate);
        }
        /// <summary>
        /// 成品加密
        /// var resultAccountNumber = _fmCommonServices.AccountNumberEncrypt(AccountNumber);
        //  if (resultAccountNumber == null || !resultAccountNumber.Item1) return "银行帐号为空！";
        //  row.AccountNumber = resultAccountNumber.Item2;
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <returns></returns>
        public Tuple<bool, string> AccountNumberEncrypt(string AccountNumber)
        {
            var spubKeyPath = _hostCongfiguration.OAToBanking;
            var sCertificatePass = _hostCongfiguration.OAToBankingPass;
            Tuple<bool, string> result = new Tuple<bool, string>(false, "");
            if (string.IsNullOrEmpty(AccountNumber)) return result = new Tuple<bool, string>(false, "账号空!");
            if (string.IsNullOrEmpty(spubKeyPath) || string.IsNullOrEmpty(sCertificatePass))
            {
                return result = new Tuple<bool, string>(false, "密钥空!");
            }
            var accountNumberStr = EncryptAccount.getFieldEncrypt(AccountNumber, spubKeyPath, sCertificatePass);
            result = new Tuple<bool, string>(true, accountNumberStr);
            return result;
        }
        public Tuple<bool, string> AccountNumberDecrypt(string AccountNumber)
        {
            var spubKeyPath = _hostCongfiguration.OAToBanking;
            var sCertificatePass = _hostCongfiguration.OAToBankingPass;
            Tuple<bool, string> result = new Tuple<bool, string>(false, "");
            if (string.IsNullOrEmpty(AccountNumber)) return result = new Tuple<bool, string>(false, "账号空!");
            if (string.IsNullOrEmpty(spubKeyPath) || string.IsNullOrEmpty(sCertificatePass))
            {
                return result = new Tuple<bool, string>(false, "密钥空!");
            }
            if (AccountNumber.Length <= 20)
            {
                return result = new Tuple<bool, string>(false, "请核实银行账号!:"+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            }
            var accountNumberStr = "";
            try
            {
                accountNumberStr = EncryptAccount.getFieldDecrypt(AccountNumber, spubKeyPath, sCertificatePass);
                if (string.IsNullOrEmpty(accountNumberStr)) { accountNumberStr = AccountNumber; }
                return result = new Tuple<bool, string>(true, accountNumberStr);
            }
            catch (Exception ex)
            {
                accountNumberStr = string.Empty;
            }
            return result = new Tuple<bool, string>(false, accountNumberStr);
        }
    }
}
