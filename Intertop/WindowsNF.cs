using System;
using System.Windows;
using Windows.UI;
using System.Runtime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.Uwp.Notifications;

namespace LocalTransfer.Intertop
{
    internal class WindowsNF
    {
       static public void Show(string Msg)
        {
            new ToastContentBuilder()
                      .AddArgument("action", "CacheMsg")
                      .AddText("Recived a message from mobile device:")
                      .AddText(Msg)

                      .AddButton(new ToastButton()
                      .SetContent("Copy")
                      .AddArgument("action", "Copy")
                      .AddArgument("data", Msg)
                      .SetBackgroundActivation())

                      .Show(); 
        }

        static public void ShowFileMsg(string Msg, string Path)
        {
            new ToastContentBuilder()
                      .AddArgument("action", "CacheFile")
                      .AddArgument("path", Path)
                      .AddText("Recived a File from mobile device:")
                      .AddText(Msg)

                      .AddButton(new ToastButton()
                      .SetContent("Save As")
                      .AddArgument("action", "SaveAs")
                      .AddArgument("path", Path)
                      .AddArgument("name", Msg)
                      .SetBackgroundActivation())

                      .AddButton(new ToastButton()
                      .SetContent("Deny")
                      .AddArgument("action", "Deny")
                      .AddArgument("path", Path)
                      .SetBackgroundActivation())

                      .Show();
        }
    }
}
