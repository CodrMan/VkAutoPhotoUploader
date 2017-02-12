using System.IO;
using Newtonsoft.Json;
using VkAutoPhotoUploader.Models;

namespace VkAutoPhotoUploader.Repositories
{
    public class SettingRepository
    {
        private static AppSettings _appSettings;

        public static AppSettings GetSettings()
        {
            if (_appSettings != null)
                return _appSettings;

            var fileName = Path.GetFullPath("App_Data/AppSettings.json");
            _appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(fileName));

            return _appSettings;
        }
    }
}
