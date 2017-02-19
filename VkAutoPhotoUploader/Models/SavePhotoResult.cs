namespace VkAutoPhotoUploader.Models
{
    public class SavePhotoResult
    {
        public InnerModel[] response { get; set; }

        public class InnerModel
        {
            public string id { get; set; }
        }
    }
}
