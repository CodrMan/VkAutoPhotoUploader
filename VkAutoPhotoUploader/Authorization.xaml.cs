﻿using System;
using System.Windows;
using System.Windows.Navigation;

using VkAutoPhotoUploader.Properties;
using VkAutoPhotoUploader.Repositories;


namespace VkAutoPhotoUploader
{
    public partial class Authorization : Window
    {
        private readonly MainWindow _window;
        private readonly string _appId = SettingRepository.GetSettings().AppId;

        public Authorization(MainWindow window)
        {
            InitializeComponent();
            _window = window;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            webBrowser.Navigate(String.Format(Properties.Resources.OauthUrl, _appId));
        }

        private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                string url = webBrowser.Source.ToString();
                string l = url.Split('#')[1];

                if (l[0] == 'a')
                {
                    Settings.Default.auth = true;
                    Settings.Default.id = l.Split('=')[3];
                    Settings.Default.token = l.Split('&')[0].Split('=')[1];
                    Settings.Default.timeOut = DateTime.Now.AddHours(24);
                    Settings.Default.Save();
                    _window.AddLog("Authorization success!");
                    this.Close();
                }
            }
            catch
            {
            }
        }
    }
}
