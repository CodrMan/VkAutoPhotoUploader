using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using VkAutoPhotoUploader.Models;
using VkAutoPhotoUploader.Properties;


namespace VkAutoPhotoUploader
{
    static class WebProcessor
    {
        private static readonly string DefaultPhotoUrl = "https://vk.com/images/camera_400.png";

        public static T Reguest<T>(string httpParams)
        {
            var response = SendRequest($"https://api.vk.com/method/{httpParams}&access_token={Settings.Default.token}");
            string responseText = String.Empty;
            var encoding = ASCIIEncoding.ASCII;
            var responseStream = response.GetResponseStream();
            using (var reader = new StreamReader(responseStream, encoding))
                responseText = reader.ReadToEnd();

            if (responseText.Contains("error"))
            {
                var errorModel = JsonConvert.DeserializeObject<VkErrorModel>(responseText);
                throw new VkResponseExeption(errorModel.error.error_code, errorModel.error.error_msg);
            }

            return JsonConvert.DeserializeObject<T>(responseText);
        }

        public static byte[] GetPhoto(string url, ref bool isLoadPhoto)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            var newUrl = url.Contains("_") ? url.Split('_')[0] + ".jpg" : url;

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    request = WebRequest.Create(i == 3 ? newUrl : newUrl.Replace(".jpg", String.Format("_{0}.jpg", i))) as HttpWebRequest;
                    response = request.GetResponse() as HttpWebResponse;
                    isLoadPhoto = true;
                    var rez = ReadFully(response.GetResponseStream());
                    if(!rez.Any())
                        throw new Exception();
                    return rez;
                }
                catch (Exception)
                {
                }
            }
   
            request = WebRequest.Create(DefaultPhotoUrl) as HttpWebRequest;
            response = request.GetResponse() as HttpWebResponse;
            return ReadFully(response.GetResponseStream());
        }

        public static UploadPhotoModel SendPhotos(string url, byte[] file)
        {
            var response = FormUpload.MultipartFormDataPost(url, file);

            string responseText = String.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new StreamReader(response.GetResponseStream(), encoding))
                responseText = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<UploadPhotoModel>(responseText);
        }

        private static HttpWebResponse SendRequest(string url)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            return request.GetResponse() as HttpWebResponse;
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
    }
}
