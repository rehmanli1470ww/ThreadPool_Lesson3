using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;



namespace ThreadPool_Lesson3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public string FullPath { get; set; }
        public string YeniPath { get; set; }
        public string Yeni1Path { get; set; }
        public AppSetting app { get; set; }
        public string Key { get; set; }
        public string Iv { get; set; }
        public MainWindow()
        {
            
            InitializeComponent();
            DataContext = this;
            YeniPath = @"D:\Desktop\Yeni.txt";
            Yeni1Path = @"D:\Desktop\Yeni1.txt";
         
            app= new AppSetting();
            Key = new ConfigurationBuilder().AddJsonFile("AppSettings.json").Build().GetSection("Keys")["Key"];
            Iv = new ConfigurationBuilder().AddJsonFile("AppSettings.json").Build().GetSection("Keys")["Iv"];
        }

        public string Path() 
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                return ofd.FileName;
            }
            return null;
        }

        private void FilePath(object sender, RoutedEventArgs e)
        {
            FullPath = Path();
            TBox.Text = FullPath;
        }
        
        private void StartMethod(object sender, RoutedEventArgs e)
        {
            var Check = EncriptionRB.IsChecked;
            var Check1 = DecriptionRB.IsChecked;
            var ProgresValue = ProgresBar.Value;
            var ProgresMax = ProgresBar.Maximum;
            
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                
                lock (this)
                {
                    if (Check == true)
                    {
                        using (FileStream fsInput = new FileStream(FullPath, FileMode.Open))
                        {
                            using (FileStream fsOutput = new FileStream("Temp.txt", FileMode.Create))
                            {
                                using (AesManaged aes = new AesManaged())
                                {

                                    aes.Key = Convert.FromBase64String(Key);
                                    aes.IV = Convert.FromBase64String(Iv);

                                    ICryptoTransform encryptor = aes.CreateEncryptor();
                                    using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                                    {
                                        fsInput.CopyTo(cs);

                                    }

                                }

                            }
                            fsInput.SetLength(0);
                            using (FileStream fsTemp = new FileStream("Temp.txt", FileMode.Open))
                            {
                                fsTemp.CopyTo(fsInput);
                            }
                        }
                        File.Delete("Temp.txt");

                    }
                    else if (Check1 == true)
                    {

                        using (FileStream fsInput = new FileStream(FullPath, FileMode.Open))
                        {
                            using (FileStream fsOutput = new FileStream("Temp.txt", FileMode.Create))
                            {
                                using (AesManaged aes = new AesManaged())
                                {

                                    aes.Key = Convert.FromBase64String(Key);
                                    aes.IV = Convert.FromBase64String(Iv);

                                    ICryptoTransform decryptor = aes.CreateDecryptor();
                                    using (CryptoStream cs = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
                                    {
                                        fsInput.CopyTo(cs);
                                    }
                                }

                            }
                            fsInput.SetLength(0);
                            using (FileStream fsTemp = new FileStream("Temp.txt", FileMode.Open))
                            {
                                fsTemp.CopyTo(fsInput);
                            }
                        }
                        File.Delete("Temp.txt");

                    }
                    
                }
                Thread.Sleep(500);

            });
            ProgresBar.Value = 0;
            ProgresBar.Maximum = 10000;

            for (int i = 0; i < ProgresBar.Maximum; i++)
            {
                ProgresBar.Value = i;

            }
            



        }

        private void CancelMethod(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
