﻿namespace VkAutoPhotoUploader.Models
{
    public class CreateAlbumModel
    {
        public InnerModel response { get; set; }

        public class InnerModel
        {
            public int aid { get; set; }
        }
    }
}
