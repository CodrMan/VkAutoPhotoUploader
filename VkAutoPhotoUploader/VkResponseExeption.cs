using System;

namespace VkAutoPhotoUploader
{
    class VkResponseExeption : ApplicationException
    {
        private readonly int _errorCode;
        private readonly string _messageDetails;

        public VkResponseExeption(){}

        public VkResponseExeption(int error, string message)
        {
            _errorCode = error;
            _messageDetails = message;
        }

        public override string Message
        {
            get
            {
                return string.Format(Properties.Resources.VkExceptionMessage, _errorCode, _messageDetails);
            }
        }
    }
}
