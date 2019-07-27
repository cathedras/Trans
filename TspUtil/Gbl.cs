using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using ElCommon.Util;

namespace TspUtil
{
    public class Gbl : IntrinsicCfg
    {
        public Gbl()
        {
            ClientHeight = 640;
            ClientWidth = 1024;
            ExpByteWidth = 828;
            ExpByteHeight = 1792;
            OddRgbA = "r0g0b0";
            EvenRgbA = "r0g0b0";
            OddOffset = 0;
            EvenOffset = 0;

            PadStr = "00";
            PadLoc = $"{TspUtil.PadLoc.Left}";
            SelectCom = "COM1";
            SelectSpeed = "115200";
            SelectDataBits = "8";
            SelectParity = "none";
            SelectStopBits = 1;
            TotalTime = 40;
            IntevaTime = 5;

            BinFileName = "";
            IsSerialSend = false;
            IsNetWorkSend = false;
            IsEthSim = false;
            UsingSimData = false;
            LocalIpAddress = "127.0.0.1";
            RemoteIpAddress = "127.0.0.1";
            Port = 8080;
            IsInverse = false;
            IsAddSizeToHeader = true;
            UsingNoBoundLst = false;
            IsCrossData = false;
            IsInferiorData = false;
            //Todo
            FileListXml = "FileInfoItem.xml";
            InfriorHalf = 900;
            HalfImageIndex = 912;
            InferiorTxt = "ff";
        }

        [SectionName("PIC")] public int ClientWidth { get; set; }
        [SectionName("PIC")] public int ClientHeight { get; set; }
        [SectionName("PIC")] public int ExpByteWidth { get; set; }
        [SectionName("PIC")] public int ExpByteHeight { get; set; }

        [SectionName("PIC")] public string OddRgbA { get; set; }
        [SectionName("PIC")] public string EvenRgbA { get; set; }
        [SectionName("PIC")] public int OddOffset { get;  set; }
        [SectionName("PIC")] public int EvenOffset { get;  set; }
        [SectionName("PIC")] public string PadStr { get; set; }
        [SectionName("PIC")] public string PadLoc { get; set; }


        [SectionName("PCOMM")] public string SelectCom { get; set; }
        [SectionName("PCOMM")] public string SelectSpeed { get; set; }
        [SectionName("PCOMM")] public string SelectDataBits { get; set; }
        [SectionName("PCOMM")] public string SelectParity { get; set; }
        [SectionName("PCOMM")] public int SelectStopBits { get; set; }
        [SectionName("PCOMM")] public int TotalTime { get; set; }
        [SectionName("PCOMM")] public int IntevaTime { get; set; }

        public string BinFileName { get; set; }
        public bool IsSerialSend { get; set; }
        public bool IsNetWorkSend { get; set; }
        public bool IsEthSim { get; set; }
        public int SimFrameLen { get; set; } = 1024;
        public int FrameLen { get; set; } = 1024;
        public int FrameRetrySize { get; set; } = 4 * 64;
        public int SendBufferSize { get; set; } = 1024 * 4;
        public bool UsingSimData { get; set; }
        [SectionName("NETWORK")] public string LocalIpAddress { get; set; }
        [SectionName("NETWORK")] public string RemoteIpAddress { get; set; }
        [SectionName("NETWORK")] public int Port { get; set; }
        public bool IsInverse { get; set; }

        public bool IsAddSizeToHeader { get; set; }
        public bool UsingNoBoundLst { get; set; }
        public bool IsCrossData { get; set; }
        public bool IsInferiorData { get; set; }
        public string FileListXml { get; set; }
        public int InfriorHalf { get; set; }
        public int HalfImageIndex { get; set; }
        public string InferiorTxt { get; set; }
        public bool HighLowBytesRevert { get; set; } = false;
        public int LongTimeoutForElapsed { get; set; } = 60 * 1000;

        public bool IsCmdRun { get; set; } = true;
    }
}