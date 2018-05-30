using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace myzy.Util
{
    public partial class Pcomm
    {
        #region Extern Interface.

        #region AnyCPU

        // return Environment.Is64BitProcess ? sio_x64_:sio_x86_

        private static int sio_open(int port)
        {
            return Environment.Is64BitProcess
                ? sio_x64_open(port)
                : sio_x86_open(port);
        }

        private static int sio_ioctl(int port, int baud, int mode)
        {
            return Environment.Is64BitProcess
                ? sio_x64_ioctl(port, baud, mode)
                : sio_x86_ioctl(port, baud, mode);
        }

        public static int sio_DTR(int port, int mode)
        {
            return Environment.Is64BitProcess
                ? sio_x64_DTR(port, mode)
                : sio_x86_DTR(port, mode);
        }

        public static int sio_RTS(int port, int mode)
        {
            return Environment.Is64BitProcess
                ? sio_x64_RTS(port, mode)
                : sio_x86_RTS(port, mode);
        }

        public static int sio_close(int port)
        {
            return Environment.Is64BitProcess
                ? sio_x64_close(port)
                : sio_x86_close(port);
        }

        public static int sio_read(int port, IntPtr buf, int length)
        {
            return Environment.Is64BitProcess
                ? sio_x64_read(port, buf, length)
                : sio_x86_read(port, buf, length);
        }

        private static int sio_write(int port, IntPtr buf, int length)
        {
            return Environment.Is64BitProcess
                ? sio_x64_write(port, buf, length)
                : sio_x86_write(port, buf, length);
        }

        private static int sio_SetReadTimeouts(int port, int totalTimeouts, int intervalTimeouts)
        {
            return Environment.Is64BitProcess
                ? sio_x64_SetReadTimeouts(port, totalTimeouts, intervalTimeouts)
                : sio_x86_SetReadTimeouts(port, totalTimeouts, intervalTimeouts);
        }

        private static int sio_SetWriteTimeouts(int port, ref ulong totalTimeouts)
        {
            return Environment.Is64BitProcess
                ? sio_x64_SetWriteTimeouts(port, ref totalTimeouts)
                : sio_x86_SetWriteTimeouts(port, ref totalTimeouts);
        }

        private static int sio_GetWriteTimeouts(int port, ref ulong totalTimeouts)
        {
            return Environment.Is64BitProcess
                ? sio_x64_GetWriteTimeouts(port, ref totalTimeouts)
                : sio_x86_GetWriteTimeouts(port, ref totalTimeouts);
        }

        private static int sio_AbortRead(int port)
        {
            return Environment.Is64BitProcess
                ? sio_x64_AbortRead(port)
                : sio_x86_AbortRead(port);
        }


        private static int sio_AbortWrite(int port)
        {
            return Environment.Is64BitProcess
                ? sio_x64_AbortWrite(port)
                : sio_x86_AbortWrite(port);
        }

        private static int sio_getmode(int port)
        {
            return Environment.Is64BitProcess
                ? sio_x64_getmode(port)
                : sio_x86_getmode(port);
        }

        private static int sio_getbaud(int port)
        {
            return Environment.Is64BitProcess
                ? sio_x64_getbaud(port)
                : sio_x86_getbaud(port);
        }

        private static int sio_flowctrl(int port, int mode)
        {
            return Environment.Is64BitProcess
                ? sio_x64_flowctrl(port, mode)
                : sio_x86_flowctrl(port, mode);
        }

        private static int sio_flush(int port, int func)
        {
            return Environment.Is64BitProcess
                ? sio_x64_flush(port, func)
                : sio_x86_flush(port, func);
        }

        #endregion

        #region X64

        [DllImport("PComm.dll", EntryPoint = "sio_open", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_open(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_ioctl", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_ioctl(int port, int baud, int mode);

        [DllImport("PComm.dll", EntryPoint = "sio_DTR", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern int sio_x64_DTR(int port, int mode);

        [DllImport("PComm.dll", EntryPoint = "sio_RTS", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern int sio_x64_RTS(int port, int mode);

        [DllImport("PComm.dll", EntryPoint = "sio_close", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern int sio_x64_close(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_read", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern int sio_x64_read(int port, IntPtr buf, int length);

        [DllImport("PComm.dll", EntryPoint = "sio_write", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_write(int port, IntPtr buf, int length);

        [DllImport("PComm.dll", EntryPoint = "sio_SetReadTimeouts", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_SetReadTimeouts(int port, int totalTimeouts, int intervalTimeouts);

        [DllImport("PComm.dll", EntryPoint = "sio_SetWriteTimeouts", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_SetWriteTimeouts(int port, ref ulong intervalTimeouts);

        [DllImport("PComm.dll", EntryPoint = "sio_GetWriteTimeouts", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_GetWriteTimeouts(int port, ref ulong totalTimeouts);

        [DllImport("PComm.dll", EntryPoint = "sio_AbortRead", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_AbortRead(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_AbortWrite", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_AbortWrite(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_getmode", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_getmode(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_getbaud", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_getbaud(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_flowctrl", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_flowctrl(int port, int mode);

        [DllImport("PComm.dll", EntryPoint = "sio_flush", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x64_flush(int port, int func);

        #endregion

        #region X86

        [DllImport("PComm.dll", EntryPoint = "sio_open", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_open(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_ioctl", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_ioctl(int port, int baud, int mode);

        [DllImport("PComm.dll", EntryPoint = "sio_DTR", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        public static extern int sio_x86_DTR(int port, int mode);

        [DllImport("PComm.dll", EntryPoint = "sio_RTS", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        public static extern int sio_x86_RTS(int port, int mode);

        [DllImport("PComm.dll", EntryPoint = "sio_close", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        public static extern int sio_x86_close(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_read", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        public static extern int sio_x86_read(int port, IntPtr buf, int length);

        [DllImport("PComm.dll", EntryPoint = "sio_write", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_write(int port, IntPtr buf, int length);

        [DllImport("PComm.dll", EntryPoint = "sio_SetReadTimeouts", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_SetReadTimeouts(int port, int totalTimeouts, int intervalTimeouts);

        [DllImport("PComm.dll", EntryPoint = "sio_SetWriteTimeouts",
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_SetWriteTimeouts(int port, ref ulong totalTimeouts);

        [DllImport("PComm.dll", EntryPoint = "sio_GetWriteTimeouts", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_GetWriteTimeouts(int port, ref ulong totalTimeouts);

        [DllImport("PComm.dll", EntryPoint = "sio_AbortRead", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_AbortRead(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_AbortWrite", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_AbortWrite(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_getmode", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_getmode(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_getbaud", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_getbaud(int port);

        [DllImport("PComm.dll", EntryPoint = "sio_flowctrl", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_flowctrl(int port, int mode);

        [DllImport("PComm.dll", EntryPoint = "sio_flush", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Ansi)]
        private static extern int sio_x86_flush(int port, int func);

        #endregion

        #endregion

        public int sio_flush(int func)
        {
            return sio_flush(Gl_Int_Port, func);
        }

        public int sio_ioctl(int baud, int mode)
        {
            return sio_ioctl(Gl_Int_Port, baud, mode);
        }

        public int sio_DTR(int mode)
        {
            return sio_DTR(Gl_Int_Port, mode);
        }

        public int sio_RTS(int mode)
        {
            return sio_RTS(Gl_Int_Port, mode);
        }

        public int sio_close()
        {
            return sio_close(Gl_Int_Port);
        }

        public int sio_read(ref byte[] buf, int length)
        {
            var ptr = Marshal.AllocHGlobal(length * Marshal.SizeOf(buf[0]));
            var ret = sio_read(Gl_Int_Port, ptr, length);
            if (ret > 0)
            {
                Marshal.Copy(ptr, buf, 0, ret);
            }
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        public int sio_write(byte[] buf)
        {
            var ptr = Marshal.AllocHGlobal(buf.Length * Marshal.SizeOf(buf[0]));

            for (int i = 0; i < buf.Length; i++)
            {
                Marshal.WriteByte(IntPtr.Add(ptr, i), buf[i]);
            }
            var ret = sio_write(Gl_Int_Port, ptr, buf.Length);

            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public int sio_SetReadTimeouts(int totalTimeouts, int intervalTimeouts)
        {
            return sio_SetReadTimeouts(Gl_Int_Port, totalTimeouts, intervalTimeouts);
        }

        public int sio_SetWriteTimeouts(ref ulong totalTimeouts)
        {
            return sio_SetWriteTimeouts(Gl_Int_Port,ref totalTimeouts);
        }

        public int sio_AbortRead()
        {
            return sio_AbortRead(Gl_Int_Port);
        }

        public int sio_AbortWrite()
        {
            return sio_AbortWrite(Gl_Int_Port);
        }

        public int sio_getmode()
        {
            return sio_getmode(Gl_Int_Port);
        }

        public int sio_getbaud()
        {
            return sio_getbaud(Gl_Int_Port);
        }

        public int sio_flowctrl(int mode)
        {
            return sio_flowctrl(Gl_Int_Port, mode);
        }

        // Function return error code
        private const int SIO_OK = 0;
        private const int SIO_BADPORT = -1;
        private const int SIO_OUTCONTROL = -2;
        private const int SIO_NODATA = -4;
        private const int SIO_OPENFAIL = -5;
        private const int SIO_RTS_BY_HW = -6;
        private const int SIO_BADPARAM = -7;
        private const int SIO_WIN32FAIL = -8;
        private const int SIO_BOARDNOTSUPPORT = -9;
        private const int SIO_ABORT_WRITE = -11;
        private const int SIO_WRITETIMEOUT = -12;

        // Self Define function return error code
        private const int ERR_NOANSWER = -101;

        // Baud rate
        private const int B50 = 0x0;
        private const int B75 = 0x1;
        private const int B110 = 0x2;
        private const int B134 = 0x3;
        private const int B150 = 0x4;
        private const int B300 = 0x5;
        private const int B600 = 0x6;
        private const int B1200 = 0x7;
        private const int B1800 = 0x8;
        private const int B2400 = 0x9;
        private const int B4800 = 0xA;
        private const int B7200 = 0xB;
        private const int B9600 = 0xC;
        private const int B19200 = 0xD;
        private const int B38400 = 0xE;
        private const int B57600 = 0xF;
        private const int B115200 = 0x10;
        private const int B230400 = 0x11;
        private const int B460800 = 0x12;
        private const int B921600 = 0x13;

        // Mode setting Data bits define
        private const int BIT_5 = 0x0;
        private const int BIT_6 = 0x1;
        private const int BIT_7 = 0x2;
        private const int BIT_8 = 0x3;
        // Mode setting Stop bits define
        private const int STOP_1 = 0x0;
        private const int STOP_2 = 0x4;
        // Mode setting Parity define
        private const int P_EVEN = 0x18;
        private const int P_ODD = 0x8;
        private const int P_SPC = 0x38;
        private const int P_MRK = 0x28;
        private const int P_NONE = 0x0;

        // Private Key name
        public const string KEY_PORT = "Port";
        public const string KEY_BAUDRATE = "Baud_Rate";
        public const string KEY_PARITY = "Parity";
        public const string KEY_BYTESIZE = "Byte_Size";
        public const string KEY_STOPBITS = "Stop_Bits";
        public const string KEY_BEFOREDELAY = "Before_Delay";
        public const string KEY_BYTEDELAY = "Byte_Delay";
        public const string KEY_READINTERVALTIMEOUT = "Read_Interval_Timeout";
        public const string KEY_READTOTALTIMEOUT = "Read_Total_Timeout";
        public const string KEY_WRITEINTERVALTIMEOUT = "Write_Interval_Timeout";
        public const string KEY_WRITETOTALTIMEOUT = "Write_Total_Timeout";

        // Port param
        private int Gl_Int_Port = 1;
        private int Gl_Int_Baudrate = B9600;
        private int Gl_Int_Parity = P_NONE;
        private int Gl_Int_ByteSize = BIT_8;
        private int Gl_Int_StopBits = STOP_1;
        // Delay param
        private int Gl_Int_BeforeDelay = 0;
        private int Gl_Int_ByteDelay = 0;
        private int Gl_Int_ReadIntervalTimeout = 50;
        private int Gl_Int_ReadTotalTimeout = 3000;

        //private int Gl_Int_WriteIntervalTimeout = 0;
        private ulong Gl_Int_WriteTotalTimeout = 0;

        /// <summary>
        /// ½âÎöÍ¨Ñ¶²ÎÊý
        /// </summary>
        /// <param name="Hb_CommParam"></param>
        private void AnalyseCommParam(Hashtable htCommParam)
        {
            //Port
            //更改为COM1命令方式
            // Gl_Int_Port = int.Parse(htCommParam[KEY_PORT].ToString());
            var com = htCommParam[KEY_PORT].ToString();
            var regex = new Regex(@"COM(\d*)", RegexOptions.IgnoreCase);
            var match = regex.Match(com);
            if (match.Success)
            {
                Gl_Int_Port = int.Parse(match.Groups[1].Value);
            }

            //Baud rate
            if (htCommParam.Contains(KEY_BAUDRATE))
            {
                switch (htCommParam[KEY_BAUDRATE].ToString())
                {
                    case "50":
                        Gl_Int_Baudrate = B50;
                        break;
                    case "75":
                        Gl_Int_Baudrate = B75;
                        break;
                    case "110":
                        Gl_Int_Baudrate = B110;
                        break;
                    case "134":
                        Gl_Int_Baudrate = B134;
                        break;
                    case "150":
                        Gl_Int_Baudrate = B150;
                        break;
                    case "300":
                        Gl_Int_Baudrate = B300;
                        break;
                    case "600":
                        Gl_Int_Baudrate = B600;
                        break;
                    case "1200":
                        Gl_Int_Baudrate = B1200;
                        break;
                    case "1800":
                        Gl_Int_Baudrate = B1800;
                        break;
                    case "2400":
                        Gl_Int_Baudrate = B2400;
                        break;
                    case "4800":
                        Gl_Int_Baudrate = B4800;
                        break;
                    case "7200":
                        Gl_Int_Baudrate = B7200;
                        break;
                    case "9600":
                        Gl_Int_Baudrate = B9600;
                        break;
                    case "19200":
                        Gl_Int_Baudrate = B19200;
                        break;
                    case "38400":
                        Gl_Int_Baudrate = B38400;
                        break;
                    case "57600":
                        Gl_Int_Baudrate = B57600;
                        break;
                    case "115200":
                        Gl_Int_Baudrate = B115200;
                        break;
                    case "230400":
                        Gl_Int_Baudrate = B230400;
                        break;
                    case "460800":
                        Gl_Int_Baudrate = B460800;
                        break;
                    case "921600":
                        Gl_Int_Baudrate = B921600;
                        break;
                    default:
                        Gl_Int_Baudrate = B9600;
                        break;
                }
            }
            //Parity
            if (htCommParam.Contains(KEY_PARITY))
            {
                switch (htCommParam[KEY_PARITY].ToString())
                {
                    case "Even":
                        Gl_Int_Parity = P_EVEN;
                        break;
                    case "Odd":
                        Gl_Int_Parity = P_ODD;
                        break;
                    case "Space":
                        Gl_Int_Parity = P_SPC;
                        break;
                    case "Mark":
                        Gl_Int_Parity = P_MRK;
                        break;
                    case "None":
                        Gl_Int_Parity = P_NONE;
                        break;
                    default:
                        Gl_Int_Parity = P_NONE;
                        break;
                }
            }
            //Byte Size
            if (htCommParam.Contains(KEY_BYTESIZE))
            {
                switch (htCommParam[KEY_BYTESIZE].ToString())
                {
                    case "5":
                        Gl_Int_ByteSize = BIT_5;
                        break;
                    case "6":
                        Gl_Int_ByteSize = BIT_6;
                        break;
                    case "7":
                        Gl_Int_ByteSize = BIT_7;
                        break;
                    case "8":
                        Gl_Int_ByteSize = BIT_8;
                        break;
                    default:
                        Gl_Int_ByteSize = BIT_8;
                        break;
                }
            }
            //Stop Bits
            if (htCommParam.Contains(KEY_STOPBITS))
            {
                switch (htCommParam[KEY_STOPBITS].ToString())
                {
                    case "1":
                        Gl_Int_StopBits = STOP_1;
                        break;
                    case "2":
                        Gl_Int_StopBits = STOP_2;
                        break;
                    default:
                        Gl_Int_StopBits = STOP_1;
                        break;
                }
            }
            //Before Delay
            if (htCommParam.Contains(KEY_BEFOREDELAY))
            {
                int.TryParse(htCommParam[KEY_BEFOREDELAY].ToString(), out Gl_Int_BeforeDelay);
            }
            //Byte Delay
            if (htCommParam.Contains(KEY_BYTEDELAY))
            {
                int.TryParse(htCommParam[KEY_BYTEDELAY].ToString(), out Gl_Int_ByteDelay);
            }
            //Read Interval Timeout
            if (htCommParam.Contains(KEY_READINTERVALTIMEOUT))
            {
                int.TryParse(htCommParam[KEY_READINTERVALTIMEOUT].ToString(), out Gl_Int_ReadIntervalTimeout);
            }
            //After Delay
            if (htCommParam.Contains(KEY_READTOTALTIMEOUT))
            {
                int.TryParse(htCommParam[KEY_READTOTALTIMEOUT].ToString(), out Gl_Int_ReadTotalTimeout);
            }
            //write Interval Timeout
            //if (htCommParam.Contains(KEY_WRITEINTERVALTIMEOUT))
            //{
            //    int.TryParse(htCommParam[KEY_WRITEINTERVALTIMEOUT].ToString(), out Gl_Int_WriteIntervalTimeout);
            //}
            //After Delay
            if (htCommParam.Contains(KEY_WRITETOTALTIMEOUT))
            {
                ulong.TryParse(htCommParam[KEY_WRITETOTALTIMEOUT].ToString(), out Gl_Int_WriteTotalTimeout);
            }
        }

        /// <summary>
        /// ³õÊ¼»¯´®¿ÚÍ¨Ñ¶
        /// </summary>
        /// <param name="Hb_CommParam"></param>
        /// <returns>´íÎóÂë</returns>
        public int InitComm(Hashtable htCommParam)
        {
            AnalyseCommParam(htCommParam);
            //Open port
            int iRtnCode = sio_open(Gl_Int_Port);
            if (iRtnCode != SIO_OK)
            {
                return iRtnCode;
            }
            //Configure communication parameters
            int mode = Gl_Int_Parity | Gl_Int_ByteSize | Gl_Int_StopBits;
            iRtnCode = sio_ioctl(Gl_Int_Port, Gl_Int_Baudrate, mode);
            if (iRtnCode != SIO_OK)
            {
                return iRtnCode;
            }
            //Flow control
            iRtnCode = sio_flowctrl(Gl_Int_Port, 0);
            if (iRtnCode != SIO_OK)
            {
                return iRtnCode;
            }
            //DTR
            iRtnCode = sio_DTR(Gl_Int_Port, 1);
            if (iRtnCode != SIO_OK)
            {
                return iRtnCode;
            }
            //RTS
            iRtnCode = sio_RTS(Gl_Int_Port, 1);
            if (iRtnCode != SIO_OK)
            {
                return iRtnCode;
            }
            //Set timeout values for sio_read
            iRtnCode = sio_SetReadTimeouts(Gl_Int_Port, Gl_Int_ReadTotalTimeout, Gl_Int_ReadIntervalTimeout);
            if (iRtnCode != SIO_OK)
            {
                return iRtnCode;
            }

            iRtnCode = sio_SetWriteTimeouts(Gl_Int_Port, ref Gl_Int_WriteTotalTimeout);
            if (iRtnCode != SIO_OK)
            {
                return iRtnCode;
            }
            return 0;
        }

        ///// <summary>
        ///// 发生字符串帧，
        ///// </summary>
        ///// <param name="strSendFrame">发送帧</param>
        ///// <param name="strRecFrame">接受帧</param>
        ///// <param name="iNewBaudrate">新的波特率</param>
        ///// <returns>错误码</returns>
        //public int SendFrame(string strSendFrame, ref string strRecFrame, int iNewBaudrate = 0)
        //{
        //    int iRtnCode;

        //    byte[] recbuffer = new byte[4096];
        //    int oldBaud = 0;

        //    //将数据转换为字节数组
        //    var buffer = Array.ConvertAll(strSendFrame.ToCharArray(), Convert.ToByte);
        //    var iFrameLen = buffer.Length;

        //    //Write data
        //    if (Gl_Int_ByteDelay == 0)
        //    {
        //        iRtnCode = sio_write(Gl_Int_Port, ref buffer, iFrameLen);
        //        if (iRtnCode < 0)
        //        {
        //            return iRtnCode;
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i <= iFrameLen - 1; i++)
        //        {
        //            iRtnCode = sio_write(Gl_Int_Port, ref buffer, 1);
        //            if (iRtnCode < 0)
        //            {
        //                return iRtnCode;
        //            }
        //            Thread.Sleep(Gl_Int_ByteDelay);
        //        }
        //    }
        //    //Change baudrate
        //    if (iNewBaudrate < 0) //No need answer
        //    {
        //        return 0;
        //    }
        //    // ReSharper disable once RedundantIfElseBlock
        //    else if (iNewBaudrate == 0)
        //    {
        //        //Use same baudrate.
        //    }
        //    else if (iNewBaudrate > 0) //Need change baudrate
        //    {
        //        int mode = sio_getmode(Gl_Int_Port);
        //        oldBaud = sio_getbaud(Gl_Int_Port);
        //        if (oldBaud != iNewBaudrate)
        //        {
        //            Thread.Sleep(180);
        //            sio_ioctl(Gl_Int_Port, iNewBaudrate, mode);
        //        }
        //    }
        //    //Read data
        //    iRtnCode = sio_read(Gl_Int_Port, ref recbuffer[0], recbuffer.Length);
        //    if (iRtnCode < 0)
        //    {
        //        return iRtnCode;
        //    }
        //    else if (iRtnCode == 0)
        //    {
        //        return ERR_NOANSWER;
        //    }
        //    int iRecFrameLen = iRtnCode;

        //    if (iNewBaudrate > 0 && iNewBaudrate != oldBaud)
        //    {
        //        //restore the baud.
        //        int mode = sio_getmode(Gl_Int_Port);
        //        Thread.Sleep(180);
        //        sio_ioctl(Gl_Int_Port, oldBaud, mode);
        //    }

        //    //将字节数据转换为字符串
        //    var revPtr = Marshal.AllocHGlobal(iRecFrameLen);
        //    for (int i = 0; i < iRecFrameLen - 1; i++)
        //    {
        //        Marshal.WriteByte(revPtr, i, recbuffer[i]);
        //    }
        //    var dummy = Marshal.PtrToStringAnsi(revPtr);
        //    if (revPtr != IntPtr.Zero)
        //        Marshal.FreeHGlobal(revPtr);
        //    strRecFrame = dummy;

        //    return 0;
        //}

        /// <summary>
        /// 关闭串口通讯
        /// </summary>
        /// <returns>错误码</returns>
        public int CloseComm()
        {
            int iRtnCode = sio_close(Gl_Int_Port);
            if (iRtnCode != SIO_OK)
            {
                return iRtnCode;
            }
            return 0;
        }

        /// <summary>
        /// 获取通讯错误消息
        /// </summary>
        /// <param name="iErrCode">错误码</param>
        /// <returns>错误消息</returns>
        public string GetCommErrMsg(int iErrCode)
        {
            switch (iErrCode)
            {
                case SIO_OK:
                    return "成功";
                case SIO_BADPORT:
                    return "串口号无效,检测串口号!";
                case SIO_OUTCONTROL:
                    return "主板不是MOXA兼容的智能主板!";
                case SIO_NODATA:
                    return "没有可读的数据!";
                case SIO_OPENFAIL:
                    return "打开串口失败,检查串口是否被占用!";
                case SIO_RTS_BY_HW:
                    return "不能控制串口因为已经通过sio_flowctrl设定为自动H/W流控制";
                case SIO_BADPARAM:
                    return "串口参数错误,检查串口参数!";
                case SIO_WIN32FAIL:
                    return "调用Win32函数失败!";
                case SIO_BOARDNOTSUPPORT:
                    return "串口不支持这个函数!";
                case SIO_ABORT_WRITE:
                    return "用户终止写数据块!";
                case SIO_WRITETIMEOUT:
                    return "写数据超时!";
                case ERR_NOANSWER:
                    return "无应答!";
                default:
                    return iErrCode.ToString();
            }
        }
    }
}


