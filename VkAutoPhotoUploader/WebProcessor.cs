using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;

using VkAutoPhotoUploader.Properties;


namespace VkAutoPhotoUploader
{
    static class WebProcessor
    {
        public static T Reguest<T>(string uri)
        {
            var request = WebRequest.Create($"https://api.vk.com/method/{uri}&access_token={Settings.Default.token}") as HttpWebRequest;
            var response = request.GetResponse() as HttpWebResponse;
            var doc = XDocument.Load(response.GetResponseStream());
            if (doc.Root.Name == "error")
                throw new VkResponseExeption((int)doc.Root.Element("error_code"), doc.Root.Element("error_msg").Value);

            Stream str = new MemoryStream();
            doc.Save(str);
            str.Position = 0;

            XmlSerializer xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(str);
        }

        public static byte[] GetPhoto()
        {
            HttpWebRequest request = WebRequest.Create("http://hozpartner.com.ua/sites/default/files/products/93969_0.jpg") as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            return ReadFully(response.GetResponseStream());
        }

        public static void PostForm(string postUrl, byte[] formData)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
                throw new NullReferenceException("request is not a http request");

            request.Method = "POST";
            request.ContentType = contentType;
            //request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            var response = request.GetResponse() as HttpWebResponse;
            var doc = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static string BuildUrl(string url)
        {
            return "" + Settings.Default.token;
        }
    }
}
