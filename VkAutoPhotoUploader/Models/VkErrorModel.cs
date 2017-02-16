namespace VkAutoPhotoUploader.Models
{
    public class VkErrorModel
    {
        public InnerModel error { get; set; }

        public class InnerModel
        {
            public int error_code { get; set; }
            public string error_msg { get; set; }
        }
    }
}
