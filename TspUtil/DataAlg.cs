using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using log4net;


namespace TspUtil
{
    /// <summary>
    /// IMG数据处理
    /// </summary>
    public class DataBmpAlg
    {
        private static readonly ILog _log = LogManager.GetLogger("exlog");
        private readonly Gbl _gbl;
        protected readonly Bitmap _img = null;
        protected readonly int _bmpHeaderLen = 54;
        protected readonly byte[] _srcHeaders = null;
        public readonly byte[] _srcData = null;
        public byte[] _checkSum = null;
        private List<byte> _finalData = new List<byte>();
        private readonly List<byte>[] _exdata = null;
        private PadLoc _padLoc;
        private MaskForArgbItem _oddMask;
        private MaskForArgbItem _evenMask;
        protected int _bpp = 4;
        private bool _reverseLine = false;
        private bool _usingSimData = false;
        private bool _usingNoBanLst = false;
        private int _oddOffset = 0;
        private int _evenOffset = 0;
        private int _newCap = 0;
        private string _binFileName = string.Empty;
        public int ImgWidth => _img.Width;

        public int ImgHeight => _img.Height;

        public byte[] SrcHeaders => _srcHeaders;

        public byte[] CheckSum => _checkSum;

        public PadLoc PadLoc => _padLoc;

        public MaskForArgbItem OddMask => _oddMask;
        public MaskForArgbItem EvenMask => _evenMask;


        public List<byte> FinalData { get => _finalData; set => _finalData = value; }
        public bool UsingSimData { get => _usingSimData; set => _usingSimData = value; }
        public int OddOffset { get => _oddOffset; set => _oddOffset = value; }
        public int EvenOffset { get => _evenOffset; set => _evenOffset = value; }
        public string BinFileName { get => _binFileName; set => _binFileName = value; }
        public bool UsingNoBanLst { get => _usingNoBanLst; set => _usingNoBanLst = value; }

        public DataBmpAlg(Gbl gbl, byte[] by, MaskForArgbItem oddmask, MaskForArgbItem evenmask, PadLoc padLoc)
        {
            _gbl = gbl;
            _srcHeaders = new byte[_bmpHeaderLen];
            _oddMask = oddmask;
            _evenMask = evenmask;
            _padLoc = padLoc;
            _reverseLine = gbl.IsInverse;
            _usingSimData = gbl.UsingSimData;
            _oddOffset = _gbl.OddOffset;
            _evenOffset = _gbl.EvenOffset;
            _binFileName = _gbl.BinFileName;
            _usingNoBanLst = _gbl.UsingNoBoundLst;

            using (var ms = new MemoryStream(by, false))
            {
                if (_gbl.IsNetWorkSend)
                {
                    Array.Copy(by, 0, _srcHeaders, 0, _bmpHeaderLen);
                    _img = new Bitmap(ms);
                    _srcData = DecodeData();
                    _exdata = DataExtract(_srcData);
                    FinalData.AddRange(DataPackaging());
                    // _checkSum = GetAllCheckSum(_srcData);
                }
                else if (_gbl.IsSerialSend)
                {
                    _srcData = new byte[by.Length];
                    Array.Copy(by, 0, _srcData, 0, by.Length);
                    FinalData.AddRange(PackageBin());
                }
                else
                {
                    _log.Error("无法解析的格式");
                }
            }
        }
        private byte[] PackageBin()
        {
            var templst = new List<byte>();
            var exp = 256;
            var hex = HexStringToByteArray(_gbl.PadStr);
            var head = new byte[] { 0x80, 0x7f };
            int len = _srcData.Length / exp;
            for (int i = 0; i < len; i++)
            {
                byte[] temp = new byte[256];
                templst.AddRange(head);
                Array.Copy(_srcData, i * exp, temp, 0, exp);
                templst.AddRange(temp);
            }
            if (len * exp != _srcData.Length)
            {
                templst.AddRange(head);
                int remainder = _srcData.Length - len * exp;
                byte[] temps = new byte[256];
                Array.Copy(_srcData, len * exp, temps, 0, remainder);
                templst.AddRange(temps);
                for (int i = 0; i < exp - remainder; i++)
                {
                    templst.Add(hex[i % hex.Length]);
                }
            }

            return templst.ToArray();
        }

        private byte[] DecodeData()
        {
            var data = _img.LockBits(new Rectangle(0, 0, ImgWidth, ImgHeight), ImageLockMode.ReadOnly, _img.PixelFormat);
            var srcs = new List<byte>();
            var pt = data.Scan0;
            _bpp = data.Stride / _img.Width;

            for (int y = 0; y < data.Height; y++)
            {
                var row = IntPtr.Add(pt, y * data.Stride);
                if (_reverseLine)
                    row = IntPtr.Add(pt, (ImgHeight - y - 1) * data.Stride);
                for (int x = 0; x < data.Width; x++)
                {
                    var pixel = IntPtr.Add(row, x * _bpp);
                    for (int bit = 0; bit < _bpp; bit++)
                    {
                        if (!UsingSimData)
                        {
                            var xx = Marshal.ReadByte(pixel, bit);
                            srcs.Add(xx);
                        }
                        else
                        {
                            var xx = (byte)((bit + x * _bpp) % 255);
                            srcs.Add(xx);
                        }
                    }
                }
            }
            _img.UnlockBits(data);
            return srcs.ToArray();
        }

        public static byte[] HexStringToByteArray(string s)
        {
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)Convert.ToByte(s.Substring(2 * i, 2), 16);
            return buffer;
        }
        /// <summary>
        /// 构造模拟数据,对期望的图片宽度和高度进行遍历，然后加入已经解析出来的图片,空白处加入用户输入的数据
        /// </summary>
        public List<byte> DataPackaging()
        {
            var data = new byte[_gbl.ExpByteHeight, _gbl.ExpByteWidth];
            var hex = HexStringToByteArray(_gbl.PadStr);
            var packageData = new List<byte>();
            if (PadLoc == PadLoc.Left)
            {
                for (int row = 0; row < _gbl.ExpByteHeight; row += 2)
                {
                    var item = new List<byte>();
                    if (row < ImgHeight)
                    {
                        var exp = _gbl.ExpByteWidth - item.Count;
                        //添加偶行(物理意义的奇行)
                        if (OddOffset == 0 && EvenOffset == 0)
                        {
                            item.AddRange(_exdata[row + OddOffset].Take(exp));
                        }
                        for (int col = 0; col < _gbl.ExpByteWidth - _exdata[row + OddOffset].Count; col++)
                        {
                            item.Add(hex[col % hex.Length]);
                        }

                        //添加奇行(物理意义的偶行)
                        if (OddOffset == 0 && EvenOffset == 0)
                        {
                            item.AddRange(_exdata[row + 1 + EvenOffset].Take(exp));
                        }
                        for (int col = 0; col < _gbl.ExpByteWidth - _exdata[row + 1 + EvenOffset].Count; col++)
                        {
                            item.Add(hex[col % hex.Length]);
                        }
                    }
                    else
                    {
                        for (int col = 0; col < _gbl.ExpByteWidth; col++)
                        {
                            item.Add(hex[col % hex.Length]);
                        }
                        for (int col = 0; col < _gbl.ExpByteWidth; col++)
                        {
                            item.Add(hex[col % hex.Length]);
                        }
                    }
                    //处理奇行
                    for (int i = 0; i < item.Count / 2; i++)
                    {
                        data[row, i] = item[i];
                    }
                    //处理偶行
                    for (int i = item.Count / 2; i < item.Count; i++)
                    {
                        data[row + 1, i - item.Count / 2] = item[i];
                    }
                }
                for (int row = 0; row < _gbl.ExpByteHeight; row++)
                {
                    for (int col = 0; col < _gbl.ExpByteWidth; col++)
                    {
                        packageData.Add(data[row, col]);
                    }
                }
            }
            else
            {
                for (int row = 0; row < _gbl.ExpByteHeight; row += 2)
                {
                    var item = new List<byte>();
                    if (row < ImgHeight)
                    {
                        var expOdd = Math.Min(_gbl.ExpByteWidth, _exdata[row].Count);
                        var expEven = Math.Min(_gbl.ExpByteWidth, _exdata[row + 1].Count);
                        //处理奇行
                        byte[] oddRow = _exdata[row + OddOffset].Take(expOdd).ToArray();
                        if (OddOffset == 0 && EvenOffset == 0)
                        {
                            item.AddRange(oddRow);
                        }
                        else if (OddOffset == 0 && EvenOffset < 0)
                        {
                            for (int widIdx = 0; widIdx < _gbl.ExpByteWidth - 4; widIdx += _newCap)
                            {
                                List<byte> items = new List<byte>();
                                byte[] fourData = new byte[_newCap];
                                Array.Copy(oddRow, widIdx, fourData, 0, _newCap);
                                for (int bppIdx = _newCap; bppIdx > 0; bppIdx -= 2)
                                {
                                    items.Add(fourData[bppIdx - 2]);
                                    items.Add(fourData[bppIdx - 1]);
                                }
                                item.AddRange(items);
                            }
                        }
                        for (int col = item.Count; col < _gbl.ExpByteWidth; col++)
                        {
                            item.Add(hex[(col - expOdd) % hex.Length]);
                        }

                        //处理偶行
                        byte[] evenRow = _exdata[row + 1 + EvenOffset].Take(expEven).ToArray();
                        if (OddOffset == 0 && EvenOffset == 0)
                        {
                            item.AddRange(evenRow);
                        }
                        else if (OddOffset == 0 && EvenOffset < 0)
                        {
                            item.AddRange(evenRow);
                        }
                        else if (OddOffset == 0 && EvenOffset > 0)
                        {
                            if (row + 1 + EvenOffset > _gbl.ExpByteHeight - 1)
                            {
                                break;
                            }
                        }

                        for (int col = item.Count; col < 2 * _gbl.ExpByteWidth; col++)
                        {
                            item.Add(hex[(col - expEven) % hex.Length]);
                        }
                    }
                    else
                    {
                        int offset = Math.Abs(OddOffset) - Math.Abs(EvenOffset) > 0 ? (Math.Abs(OddOffset) - Math.Abs(EvenOffset) == 0 ? 0 : _bpp - Math.Abs(OddOffset)) : _bpp - Math.Abs(EvenOffset);
                        int newWidth = _gbl.ExpByteWidth * offset;
                        for (int col = 0; col < newWidth; col++)
                        {
                            item.Add(hex[col % hex.Length]);
                        }
                    }
                    ////处理偶行
                    //for (int i = 0; i < item.Count / 2; i++)
                    //{
                    //    data[row, i] = item[i];
                    //}
                    ////处理奇行
                    //for (int i = item.Count / 2; i < item.Count; i++)
                    //{
                    //    data[row + 1, i - item.Count / 2] = item[i];
                    //}
                    //处理偶行
                    for (int i = 0; i < _gbl.ExpByteWidth; i++)
                    {
                        data[row, i] = item[i];
                    }
                    //处理奇行
                    for (int i = 0; i < _gbl.ExpByteWidth; i++)
                    {
                        data[row + 1, i] = item[i + _gbl.ExpByteWidth];
                    }
                }

                for (int row = 0; row < _gbl.ExpByteHeight; row++)
                {
                    for (int col = 0; col < _gbl.ExpByteWidth; col++)
                    {
                        packageData.Add(data[row, col]);
                    }
                }
            }

            return packageData;
        }

        /// <summary>
        /// 高低位反转
        /// </summary>
        /// <param name="packageData"></param>
        /// <returns></returns>
        public List<byte> HighLowBytesRevert(List<byte> packageData)
        {
            var lst = new List<byte>();

            //按2字节反序
            var index = 0;
            var count = packageData.Count / 2;
            while (index < count)
            {
                if (2 * index + 1 < packageData.Count)
                {
                    lst.Add(packageData[2 * index + 1]);
                }

                if (2 * index < packageData.Count)
                {

                    lst.Add(packageData[2 * index + 0]);
                }
                index++;
            }

            return lst;
        }
        //C408软件中的d808checksum计算方式
        public int[] ListBytes = new int[]
        {
            1,2,13,31,53,73,101,127,151,179,199,233,263,283,317,353,383,419,443,467,503,547,
            577,607,641,661,701,739,769,811,839,877,911,947,983,1019,1049,1087,1109,1153,1193
            ,1229,1277,1297,1321,1381,1429,1453,1487,1523,1559,1597,1619,1663,1699,1741,1783,1823,
            1871,1901,1949,1993,2017,2063,2089,2131,2161,2221,2267,2293,2339,2371,2393
        };
        //
        /// <summary>
        /// 数据提取，将期望的数据提取到list里面去
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public List<byte>[] DataExtract(byte[] src)
        {
            var extractArr = new List<byte>[ImgHeight];
            var rgData = new List<byte>();
            var bgData = new List<byte>();
            Func<int, string, List<byte>> act = (row, expRgbA) =>
             {
                 //this argl is the expected pixel that every color vlue should be settled to the pixel
                 var argl = ArgbItemHelper.PraseArgbText(expRgbA);
                 //The next argl is what the user want to catch the pixels' number, in theory, the number that user want is no end.
                 var cap = (argl.Max(p => p.Idx) + 1) * _bpp;
                 _newCap = argl.Count;
                 var lst = new List<byte>();
                 for (int widthIdx = 0; widthIdx < ImgWidth; widthIdx += cap / _bpp)
                 {
                     var srcIndex = widthIdx * _bpp;
                     var len = (srcIndex + cap) < _bpp * ImgWidth ? cap : (_bpp * ImgWidth - srcIndex);
                     var banLst = new List<int>();
                     if (!UsingNoBanLst)
                     {
                         for (int i = 0; i < cap / _bpp; i++)// 
                         {
                             var cur = widthIdx + i + 1;
                             if (ListBytes.Contains(row + 1))
                             {
                                 if (ListBytes.Contains(cur))
                                 {
                                     banLst.Add(i % (cap / _bpp));
                                 }
                             }
                         }
                     }
                     else
                     {
                         for (int i = 0; i < cap / _bpp; i++)// 
                         {
                             banLst.Add(i % (cap / _bpp));
                         }

                     }

                     var resrc = new byte[len];
                     Array.ConstrainedCopy(src, srcIndex + row * ImgWidth * _bpp, resrc, 0, resrc.Length);
                     var fourData = GetData(resrc, argl);

                     CheckSumColorModel(len, resrc, banLst, ref rgData, ref bgData);
                     lst.AddRange(fourData);
                 }
                 return lst;
             };

            for (int row = 0; row < ImgHeight; row++)
            {
                var argl = OddMask.ExpRGBA;
                if ((row + 1) % 2 == 1)
                {
                    argl = OddMask.ExpRGBA;
                }
                else
                {
                    argl = EvenMask.ExpRGBA;
                }
                extractArr[row] = act(row, argl);
            }
            _checkSum = GetNewCheckSum(rgData, bgData);
            return extractArr;
        }

        /// <summary>
        /// 对argb的数据进行提取并保存到一个byte数组里面
        /// </summary>
        /// <param name="src"></param>
        /// <param name="argblst"></param>
        /// <returns></returns>
        public byte[] GetData(byte[] src, List<ArgbItem> argblst)
        {
            var maxIndex = argblst.Max(p => p.Idx);
            if (argblst.Count != 0)
            {
                var tempList = new List<byte>();
                for (int i = 0; i < argblst.Count; i++)
                {
                    var n = argblst[i].Idx * _bpp + (int)argblst[i].ArgbValue;
                    if (n < src.Length)//处理索引越界问题
                    {
                        tempList.Add(src[n]);
                    }
                }
                return tempList.ToArray<byte>();
            }
            else
            {
                return src;
            }
        }


        /// <summary>
        /// 按照bgra数据排列的图片进行四字节提取，可移植
        /// </summary>
        /// <param name="capData">要提取的四字节数据</param>
        /// <param name="argblst">bgr数据代表的排列列表</param>
        /// <param name="n">指提取几个像素总字节数，比如两个像素是6字节，</param>
        /// <param name="allow">根据查找到的数据，对所需要提取的数据像素指定是否要提取</param>
        /// <returns>返回提取到的四字节数据中的数据</returns>

        public List<byte> GetCheckSumData(byte[] capData, List<Argb> argblst, int n, List<int> allow)
        {
            var tmp = new List<byte>();
            for (int i = 0; i < argblst.Count; i++)//0 1 2 3 
            {
                var _n = (int)i / n;//0 1
                if (!allow.Contains(_n))
                {
                    continue;
                }

                var capFour = new byte[_bpp];
                Array.ConstrainedCopy(capData, _n * _bpp, capFour, 0, _bpp);
                var index = (int)argblst[i];
                var data = capFour[index];
                tmp.Add(data);
            }
            return tmp;
        }
        /// <summary>
        /// 根据上文的数据进行数据模板创建和获取rgb数据
        /// </summary>
        /// <param name="len">单次提取数据的个数</param>
        /// <param name="src">原数据</param>
        /// <param name="allowLst">允许列表,根据像素数提取数据</param>
        /// <param name="rgData"></param>
        /// <param name="bgData"></param>
        /// <returns>像素数</returns>
        public int CheckSumColorModel(int len, byte[] src, List<int> allowLst, ref List<byte> rgData, ref List<byte> bgData)
        {
            int n = len / _bpp;
            var rg = new List<Argb>();
            var bg = new List<Argb>();
            for (int i = 0; i < n; i++)
            {
                rg.Add(Argb.G);
                rg.Add(Argb.R);
                bg.Add(Argb.G);
                bg.Add(Argb.B);
            }

            rgData.AddRange(GetCheckSumData(src, rg, n, allowLst));
            bgData.AddRange(GetCheckSumData(src, bg, n, allowLst));
            return n;
        }
        /// <summary>
        /// 获取checksum的计算值
        /// </summary>
        /// <param name="rgData"></param>
        /// <param name="bgData"></param>
        /// <returns></returns>
        public byte[] GetNewCheckSum(List<byte> rgData, List<byte> bgData)
        {
            Func<List<byte>, int> act = (src) =>
            {
                int sum = 0;
                for (int i = 0; i < src.Count; i += 2)
                {
                    sum += src[i] * 256 + src[i + 1];
                    if (sum > 65536)
                    {
                        sum -= 65536;
                    }
                }
                sum = ~sum;
                return sum;

            };
            byte[] CheckSum = new byte[8];
            var checksumRg = new List<byte>();
            var checksumBg = new List<byte>();
            checksumRg.AddRange(BitConverter.GetBytes(act(rgData)));
            checksumBg.AddRange(BitConverter.GetBytes(act(bgData)));
            CheckSum[0] = checksumRg[0]; CheckSum[1] = checksumRg[1];
            CheckSum[4] = checksumBg[0]; CheckSum[5] = checksumBg[1];
            return CheckSum;
        }


    }

}