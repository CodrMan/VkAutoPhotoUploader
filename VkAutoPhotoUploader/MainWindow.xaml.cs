using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using VkAutoPhotoUploader.Models;
using VkAutoPhotoUploader.Properties;
using VkAutoPhotoUploader.Repositories;

namespace VkAutoPhotoUploader
{
    public partial class MainWindow : Window
    {
        private readonly string GroupId = SettingRepository.GetSettings().GroupId;

        public MainWindow()
        {
            InitializeComponent();
            CreateSettings();

            this.txtLog.AppendText("\n");
            AddLog("Start programm!");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.auth || Settings.Default.timeOut < DateTime.Now)
            {
                new Authorization(this).Show();
                AddLog("Start authorization!");
            }
        }

        private void getProductsbtn_Click(object sender, RoutedEventArgs e)
        {
            //var products = WebParser.GetAllProducts();
            //JsonRepository.SaveProducts(products);

            var prod = ProductRepository.GetProducts().ProductInfos.Where(x => x.IsSync && !x.IsLoadPhoto).ToList();
        }

        private void startUploadbtn_Click(object sender, RoutedEventArgs e)
        {
            var products = ProductRepository.GetProducts();
            var groupByCatalog = products.ProductInfos.GroupBy(x => x.CatalogName);

            foreach (IEnumerable<ProductInfo> items in groupByCatalog)
            {
                if(items.First().CatalogName != "СТРОЙКА")
                    continue;

                var createAlbumHttpParams = String.Format("photos.createAlbum?title={0}&group_id={1}&upload_by_admins_only=1", items.First().CatalogName, GroupId);
                var albumId = WebProcessor.Reguest<CreateAlbumModel>(createAlbumHttpParams).response.aid;


                foreach (var item in items)
                {
                    if(item.IsSync)
                        continue;

                    try
                    {
                        var isLoadPhoto = false;

                        var caption = String.Format("{0}\n\nЦена: {1}\n\n{2}", item.Name, item.Price, item.ProductLink);

                        var uploadServerHttpParams = String.Format("photos.getUploadServer?album_id={0}&group_id={1}", albumId, GroupId);
                        var uploadServerModel = WebProcessor.Reguest<UploadServerModel>(uploadServerHttpParams);
                        var photoByte = WebProcessor.GetPhoto(item.PhotoUrl, ref isLoadPhoto);
                        var uploadPhotoModel = WebProcessor.SendPhotos(uploadServerModel.response.upload_url, photoByte);
                        var savePhotoHttpParams = String.Format("photos.save?album_id={0}&group_id={1}&server={2}&photos_list={3}&hash={4}&caption={5}",
                            albumId, GroupId, uploadPhotoModel.server, uploadPhotoModel.photos_list, uploadPhotoModel.hash, caption);
                        var saveResult = WebProcessor.Reguest<SavePhotoResultModel>(savePhotoHttpParams);

                        if (saveResult.response[0].id.Contains("photo"))
                        {
                            item.IsSync = true;
                            item.AlbumId = albumId;
                            item.PhotoId = saveResult.response[0].id;
                            item.IsLoadPhoto = isLoadPhoto;
                        }

                        Thread.Sleep(900);
                    }
                    catch (Exception ex)
                    {
                        item.IsSync = false;
                    }
                }
            }

            ProductRepository.SaveProducts(products, true);
        }

        private void CreateSettings()
        {
            Settings.Default.auth = true;
            Settings.Default.id = "";
            Settings.Default.token = "";
            Settings.Default.timeOut = DateTime.Now;
            Settings.Default.Save();
        }

        public void AddLog(string str)
        {
            this.txtLog.AppendText(DateTime.Now.ToString("dd.MM.yy  hh:mm:ss") + " : " + str + "\n");
            this.txtLog.Focus();
            this.txtLog.CaretIndex = this.txtLog.Text.Length;
        } //TODO event
    }
}
