using LocalTransfer.Intertop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LocalTransfer
{
    static class Extence
    {
        static public byte[] RecvInsertRange(this NetworkStream Stream, long Size)
        {
            var RecvedData = new byte[Size];
            int RecvedSize = 0;

            do
            {
                RecvedSize += Stream.Read(RecvedData, RecvedSize, (int)(Size - RecvedSize));
            } while (RecvedSize < Size);
            return RecvedData;
        }
    }
    internal class ClientHolder
    {
        enum Mode
        {
            File = 0, Message = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DataHead
        {
            public byte rev;
            public byte flag;
            public long size;
        }
        public ClientHolder(TcpClient Client)
        {
            Task.Run(delegate 
            {
                var Stream = Client.GetStream();

                byte[] bHead = Stream.RecvInsertRange(16);
                DataHead Head = (DataHead)BytesToStuct(bHead, typeof(DataHead));
                if (Head.rev != 0xff)
                {
                    MessageBox.Show("Error:Bad Data Package!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                byte[] Body = new byte[1];
                switch((Mode)Head.flag)
                {
                    case Mode.Message:
                        {
                            Body = Stream.RecvInsertRange(Head.size);
                            WindowsNF.Show(Encoding.UTF8.GetString(Body));
                        }
                        break;
                    case Mode.File:
                        {
                            Body = Stream.RecvInsertRange(Head.size);
                            FileStream FS = new FileStream(Encoding.UTF8.GetString(Body), FileMode.Create, FileAccess.Write);
                            byte[] nbHead = Stream.RecvInsertRange(16);
                            DataHead nHead = (DataHead)BytesToStuct(nbHead, typeof(DataHead));

                            byte[] nBody = Stream.RecvInsertRange(nHead.size);
                            FS.Write(nBody, 0, nBody.Length);
                            

                            WindowsNF.ShowFileMsg(Encoding.UTF8.GetString(Body), FS.Name);
                            FS.Close();
                        }
                        break;
                    default:
                        break;
                }


                Stream.Close();
                Client.Close();
                Client.Dispose();
            });
        }


        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }
    }
}
