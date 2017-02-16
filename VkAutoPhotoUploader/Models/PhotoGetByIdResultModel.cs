namespace VkAutoPhotoUploader.Models
{
    public class PhotoGetByIdResultModel
    {
        public InnerModel[] response { get; set; }

        public class InnerModel
        {
            public int pid { get; set; }
        }
    }
}
