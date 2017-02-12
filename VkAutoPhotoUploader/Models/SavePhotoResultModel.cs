namespace VkAutoPhotoUploader.Models
{
    public class SavePhotoResultModel
    {
        public InnerModel[] response { get; set; }

        public class InnerModel
        {
            public string id { get; set; }
        }
    }
}
