using System;

namespace VkAutoPhotoUploader
{
    /// <summary>
    /// Error Code
    /// </summary>
    class VkResponseExeption : ApplicationException
    {
        public int error_code { get; set; }
        public string messageDetails { get; set; }

        public VkResponseExeption(){}
        public VkResponseExeption(int error, string message)
        {
            error_code = error;
            messageDetails = message;
        }

        public override string Message
        {
            get
            {
                return string.Format("Error Code = {0}, Message = {1}", error_code, messageDetails);
            }
        }
    }
}
