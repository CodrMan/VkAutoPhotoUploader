using System.IO;
using Newtonsoft.Json;

using VkAutoPhotoUploader.Entities;


namespace VkAutoPhotoUploader.Repositories
{
    public class SettingRepository
    {
        private static AppSettings _appSettings;

        public static AppSettings GetSettings()
        {
            if (_appSettings != null)
                return _appSettings;

            var fileName = Path.GetFullPath(Properties.Resources.PathSettingsFile);
            _appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(fileName));

            return _appSettings;
        }
    }
}
