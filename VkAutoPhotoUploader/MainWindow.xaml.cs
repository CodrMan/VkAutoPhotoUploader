using System;
using System.Windows;
using VkAutoPhotoUploader.Properties;

namespace VkAutoPhotoUploader
{
    public partial class MainWindow : Window
    {
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

        private void startUploadbtn_Click(object sender, RoutedEventArgs e)
        {
            
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
        }
    }
}
