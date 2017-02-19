using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

using VkAutoPhotoUploader.Models;
using VkAutoPhotoUploader.Properties;


namespace VkAutoPhotoUploader
{
    static class WebProcessor
    {
        public static T VkReguest<T>(string httpParams)
        {
            var request = WebRequest.Create(String.Format(Resources.ApiVkUrl, httpParams, Settings.Default.token)) as HttpWebRequest;
            var response = request.GetResponse() as HttpWebResponse;
            return Reguest<T>(response);
        }

        public static UploadPhotoResult SendPhotos(string url, byte[] file)
        {
            var response = FormUpload.MultipartFormDataPost(url, file);
            return Reguest<UploadPhotoResult>(response);
        }

        private static T Reguest<T>(HttpWebResponse response)
        {
            string responseText = String.Empty;
            using (var reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
                responseText = reader.ReadToEnd();

            if (responseText.Contains("error"))
            {
                var errorModel = JsonConvert.DeserializeObject<VkErrorResult>(responseText);
                throw new VkResponseExeption(errorModel.error.error_code, errorModel.error.error_msg);
            }

            return JsonConvert.DeserializeObject<T>(responseText);
        }
    }
}
