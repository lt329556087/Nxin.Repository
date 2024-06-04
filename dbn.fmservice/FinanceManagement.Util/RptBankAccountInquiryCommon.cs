using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FinanceManagement.Util
{
  public  class RptBankAccountInquiryCommon
    {
        public static string PostRequestWithFullUrlByJson(string fullUrl, string inputJson)
        {
            var responseContent = string.Empty;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(fullUrl.Trim());
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 60000;
                httpWebRequest.ProtocolVersion = HttpVersion.Version10;

                var btBodys = Encoding.UTF8.GetBytes(inputJson);
                httpWebRequest.ContentLength = btBodys.Length;
                httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                var tmpString = streamReader.ReadToEnd();

                streamReader.Close();
                httpWebResponse.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                responseContent = string.IsNullOrEmpty(tmpString) ? string.Empty : tmpString.Replace(@">\n", @">");
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout)
                {
                    return "请求失败！";
                }
            }

            return responseContent;
        }
        


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
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(encode.GetBytes(input));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }
    }
}
