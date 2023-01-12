﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp_SetNTPTime
{
    public class SetTime
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
        }
        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0xFF) << 24) + ((x & 0xFF00) << 8) + ((x & 0xFF0000) >> 8) + ((x & 0xFF000000u) >> 24));
        }

        public static DateTime getNetworkTime()
        {
            try
            {
                byte[] array = new byte[48];
                array[0] = 27;
                IPAddress[] addressList = Dns.GetHostEntry("time.windows.com").AddressList;
                IPEndPoint remoteEP = new IPEndPoint(addressList[0], 123);
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.Connect(remoteEP);
                    socket.ReceiveTimeout = 3000;
                    socket.Send(array);
                    socket.Receive(array);
                    socket.Close();
                }

                ulong x = BitConverter.ToUInt32(array, 40);
                ulong x2 = BitConverter.ToUInt32(array, 44);
                x = SwapEndianness(x);
                x2 = SwapEndianness(x2);
                ulong num = x * 1000 + x2 * 1000 / 4294967296uL;
                return new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)num).ToLocalTime();
            }
            catch
            {
                Console.WriteLine("ERR");
                return DateTime.Now;
            }
        }

        public static bool SetLocalTime(DateTime time)
        {
            Console.WriteLine("getNetworkTime:  " + time);

            bool result = false;
            SystemTime sysTime = default(SystemTime);
            try
            {
                DateTime dateTime = time;
                sysTime.wYear = Convert.ToUInt16(dateTime.Year);
                sysTime.wMonth = Convert.ToUInt16(dateTime.Month);
                sysTime.wDay = Convert.ToUInt16(dateTime.Day);
                sysTime.wHour = Convert.ToUInt16(dateTime.Hour);
                sysTime.wMinute = Convert.ToUInt16(dateTime.Minute);
                sysTime.wSecond = Convert.ToUInt16(dateTime.Second);

                result = SetLocalTime(ref sysTime);
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetLocalTimeByStr Error" + ex.Message);
            }

            return result;
        }
    }
}
