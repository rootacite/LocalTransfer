using System;
using System.Windows;
using Windows.UI;
using System.Runtime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using LocalTransfer.Intertop;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Foundation.Collections;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace LocalTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly string HostIP = "192.168.3.138";
        static readonly int HostPort = 5544;
        TcpListener Server = new TcpListener(IPAddress.Parse(HostIP), HostPort);
        private void UserInterfaceCustomScale(double customScale)
        {
            // Change scale of window content
            this.LayoutTransform = new ScaleTransform(customScale, customScale, 0, 0);
            Width *= customScale;
            Height *= customScale;

            // Bring window center screen
            var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            Top = (screenHeight - Height) / 2;
            Left = (screenWidth - Width) / 2;
        }
        public MainWindow()
        {
            InitializeComponent();
            UserInterfaceCustomScale(1.0);
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                // Obtain the arguments from the notification
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                switch (args["action"])
                {
                    case "Copy":
                        {
                            string CB = args["data"];

                            Dispatcher.Invoke(delegate
                            {
                                Clipboard.SetDataObject(CB);
                            });
                            break;
                        }
                    case "CacheFile":
                        {
                            File.Delete(args["path"]);
                            break;
                        }
                    case "SaveAs":
                        {
                            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                            dlg.FileName = args["name"]; // Default file name
                            dlg.DefaultExt = ".*"; // Default file extension
                            dlg.Filter = "All Files (.*)|*.*"; // Filter files by extension
                            
                            var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            dlg.InitialDirectory = dir;

                            bool? result = dlg.ShowDialog();

                            // Process save file dialog box results
                            if (result == true)
                            {
                                File.Move(args["path"], dlg.FileName);
                            }
                            else
                            {
                                File.Delete(args["path"]);
                            }
                            break;
                        }
                    case "Deny":
                        {
                            File.Delete(args["path"]);
                            break;
                        }
                }
                
            };
            KeyDown += (e, v) =>
              {
                  // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater

              };

            Task.Run(delegate 
            {
                Server.Start();
                while (true)
                {
                    var Client = Server.AcceptTcpClient();
                    if(Client!=null)
                    {
                        new ClientHolder(Client);
                    }
                }
            });

            var notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "Local Transfer";
            notifyIcon.Icon = new System.Drawing.Icon("aalcn-zfqvf-001.ico");
            notifyIcon.Visible = true;

            notifyIcon.MouseDoubleClick += (e, v) =>
            {
                Close();
            };

        }
    }
}
