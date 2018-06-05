using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Documents;

namespace TspUtil
{
    /// <summary>
    /// IMG数据处理
    /// </summary>
    public class DataBmpAlg
    {
        private readonly Gbl _gbl;
        protected readonly Bitmap _img = null;
        protected readonly int _bmpHeaderLen = 54;
        protected readonly byte[] _srcHeaders = null;
        protected readonly byte[] _srcData = null;

        public int ImgWith => _img.Width;

        public int ImgHeight => _img.Height;

        protected int _bpp = 4;

        private bool _reverseLine = true;

        public DataBmpAlg(Gbl gbl, byte[] by)
        {
            _gbl = gbl;
            _srcHeaders = new byte[_bmpHeaderLen];
            Array.Copy(by, 0, _srcHeaders, 0, _bmpHeaderLen);

            using (var ms = new MemoryStream(by, false))
            {
                _img = new Bitmap(ms);
                _srcData = DecodeData();

                DataRearrange();
            }
        }
       
        private byte[] DecodeData()
        {
            var data = _img.LockBits(new Rectangle(0, 0, ImgWith, ImgHeight), ImageLockMode.ReadOnly, _img.PixelFormat);
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
                        var xx = Marshal.ReadByte(pixel, bit);
                        srcs.Add(xx);
                    }
                }
            }
            _img.UnlockBits(data);

            return srcs.ToArray();
        }

        private void DataRearrange()
        {
            var odd = _gbl.OddRgbA.ToLower().ToCharArray();
            var bgra = new[] {'b', 'g', 'r', 'a'};
            for (var i = 0; i < odd.Length; i++)
            {
                
            }

        }
    }
}