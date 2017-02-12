using System;
using System.IO;
using System.Net;
using System.Text;

namespace VkAutoPhotoUploader
{
    public static class FormUpload
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public static HttpWebResponse MultipartFormDataPost(string postUrl, byte[] file)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            byte[] formData = GetMultipartFormData(formDataBoundary, file);

            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
                throw new NullReferenceException("request is not a http request");

            request.Method = "POST";
            request.ContentType = contentType;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }
        
        private static byte[] GetMultipartFormData(string boundary, byte[] file)
        {
            Stream formDataStream = new MemoryStream();

            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"filename.jpg\";\r\nContent-Type: image/jpeg\r\n\r\n", boundary);

            formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));
            formDataStream.Write(file, 0, file.Length);
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }
}
