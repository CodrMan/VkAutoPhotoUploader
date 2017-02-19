namespace VkAutoPhotoUploader.Models
{
    public class CreateAlbumResult
    {
        public InnerModel response { get; set; }

        public class InnerModel
        {
            public int aid { get; set; }
        }
    }
}
