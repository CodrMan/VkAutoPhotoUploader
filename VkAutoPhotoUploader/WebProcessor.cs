﻿using System;
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
        public static T Reguest<T>(string uri)
        {
            var response = SendRequest($"https://api.vk.com/method/{uri}&access_token={Settings.Default.token}");
            string responseText = String.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new StreamReader(response.GetResponseStream(), encoding))
                responseText = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<T>(responseText);
        }

        public static byte[] GetPhoto(string url)
        {
            var response = SendRequest(url);
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