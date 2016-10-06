using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace MANGO_WS_TEST
{
    public static class Helper
    {
        public static readonly string WMS_URL = "https://wcs.lfuat.net:20170/wms-mango";
        public static readonly string KEY = "MANGO_LFL_API";
        public static readonly string SIGN = "2f07f527-90d2-4353-8cdb-dcb9b417297e";
        public static readonly string VER = "5.0";
        public static readonly string SERVICE_TYPE = "logistic_order_notify";

        public static bool IsXMLValid(string xmlString)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlString);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string GetBase64Encoding(string EncodingText, System.Text.Encoding oEncoding = null)
        {
            if (oEncoding == null)
                oEncoding = System.Text.Encoding.UTF8;

            byte[] arr = oEncoding.GetBytes(EncodingText);
            return System.Convert.ToBase64String(arr);
        }

        public static string GetBase64Decoding(string DecodingText, System.Text.Encoding oEncoding = null)
        {
            if (oEncoding == null)
                oEncoding = System.Text.Encoding.UTF8;

            byte[] arr = System.Convert.FromBase64String(DecodingText);
            return oEncoding.GetString(arr);
        }

        public static string CallWMSOrderNotifyWebService(string xmlRequest)
        {
            // Build request encryption
            string requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string request = "Key=" + Helper.KEY + "&RequestTime=" + requestTime + "&Sign=" + Helper.SIGN +
                "&Version=" + Helper.VER + "&ServiceType=" + Helper.SERVICE_TYPE;
            string md5EncryptionForSign = Helper.CreateMD5(request);

            // Prepare HTTP POST REQUEST 
            var httpRequest = (HttpWebRequest)WebRequest.Create(Helper.WMS_URL);

            // Build parameters
            var postData = "Key=" + Helper.KEY;
            postData += "&RequestTime=" + requestTime;
            postData += "&Sign=" + md5EncryptionForSign;
            postData += "&Version=" + Helper.VER;
            postData += "&ServiceType=" + Helper.SERVICE_TYPE;
            postData += "&Data=" + Helper.GetBase64Encoding(xmlRequest);

            // Call HTTP Post Request
            var data = Encoding.ASCII.GetBytes(postData);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = data.Length;

            using (var stream = httpRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)httpRequest.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }
    }
}
