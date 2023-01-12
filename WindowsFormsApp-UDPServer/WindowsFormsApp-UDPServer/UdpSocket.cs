using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp_UDPServer
{
    public class UdpSocket
    {
        public Thread ThreadUDP { get; set; }
        bool Listening { get; set; }
        public int ListenPort { get; set; }
        public int SendPort { get; set; }

        public delegate void CallBack(string msgReceive);
        public event CallBack NewMessage;

        public delegate void RawData(byte[] data);
        public event RawData ReceiveData;

        public delegate void NewException(Exception ex, MethodBase method, params object[] args);
        public event NewException CatchNewException;

        public UdpSocket(int sendPort,int listenPort)
        {
            SendPort = sendPort;
            ListenPort = listenPort;
        }
        public UdpSocket Open()
        {
            try
            {
                Listening = true;

                ThreadUDP = new Thread(new ThreadStart(Listen));
                ThreadUDP.Name = "ListenUDP";
                ThreadUDP.Start();
            }
            catch (Exception ex)
            {
                CatchException(ex, MethodBase.GetCurrentMethod());
            }
            return this;
        }
        public UdpSocket Close(string msgOut = "")
        {
            try
            {
                Listening = false;

                Send(msgOut, IPAddress.Broadcast, ListenPort);
                Thread.Sleep(50);

                if (ThreadUDP.IsAlive)
                    ThreadUDP.Abort();
            }
            catch (Exception ex)
            {
                CatchException(ex, MethodBase.GetCurrentMethod());
            }
            return this;
        }
        void Listen()
        {
            try
            {
                using (UdpClient UDP = new UdpClient(ListenPort))
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, ListenPort);
                    while (Listening)
                    {
                        byte[] buffer = UDP.Receive(ref ep);
                        if (buffer.Length > 0)
                            ReceiveData?.Invoke(buffer);

                        string message = Encoding.UTF8.GetString(buffer);

                        if(message.Length > 0)
                            NewMessage?.Invoke(message);
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex, MethodBase.GetCurrentMethod());
            }
        }

        public void Send(string send)
        {
            try
            {
                Send(send, IPAddress.Broadcast, SendPort);
            }
            catch (Exception ex)
            {
                CatchException(ex, MethodBase.GetCurrentMethod());
            }
        }
        public void Send(string send, string ip)
        {
            try
            {
                Send(send, IPAddress.Parse(ip), SendPort);
            }
            catch (Exception ex)
            {
                CatchException(ex, MethodBase.GetCurrentMethod());
            }
        }
        public void Send(string send ,IPAddress ip ,int port)
        {
            try
            {
                IPEndPoint targetEP = new IPEndPoint(ip, port);
                using (UdpClient UDP = new UdpClient())
                {
                    UDP.Connect(targetEP);
                    Byte[] sendBytes = Encoding.UTF8.GetBytes(send);
                    UDP.Send(sendBytes, sendBytes.Length);
                    UDP.Close();
                }
            }
            catch (Exception ex)
            { CatchException(ex, MethodBase.GetCurrentMethod(), send, ip, port); }
        }
        public void Send(byte[] send)
        {
            try
            {
                IPEndPoint targetEP = new IPEndPoint(IPAddress.Broadcast, SendPort);
                using (UdpClient UDP = new UdpClient())
                {
                    UDP.Connect(targetEP);
                    UDP.Send(send, send.Length);
                    UDP.Close();
                }
            }
            catch (Exception ex)
            { CatchException(ex, MethodBase.GetCurrentMethod(), BitConverter.ToString(send)); }
        }
        protected void CatchException(Exception ex, MethodBase method, params object[] args)
        {
            try
            {
                CatchNewException?.Invoke(ex, method, args);    //通知主程式

                string msg = "\r\nEeception:\r\n";
                msg += ex.Message + "\r\n";

                msg += "\r\nMethod Name:" + method.Name;    //方法名稱
                msg += "\r\nParameters:";                   //參數內容
                for (int i = 0; i < args.Length; i++)
                {
                    if (method.GetParameters()[i] == null || args[i] == null)
                        msg += "\r\n null";
                    else
                        msg += "\r\n" + method.GetParameters()[i].Name + "=" + args[i].ToString();
                }

                msg += "\r\n\r\nStackTrace:\r\n";
                msg += ex.StackTrace;
                Debug.Write(msg + ex.Message, method.Name);
            }
            catch (Exception exc)
            {
                Debug.Write(exc.Message, MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
