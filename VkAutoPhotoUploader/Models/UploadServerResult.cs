namespace VkAutoPhotoUploader.Models
{
    public class UploadServerResult
    {
        public InnerModel response { get; set; }

        public class InnerModel
        {
            public int album_id { get; set; }
            public int user_id { get; set; }
            public string upload_url { get; set; }
        }
    }
}
