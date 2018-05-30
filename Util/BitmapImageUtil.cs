using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace myzy.Util
{
    public static class BitmapImageUtil
    {
        public static Bitmap CreateBitmap(this BitmapSource source)
        {
            Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
            new Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this Bitmap bitmap)
        {

            /*
             /*-------------------------------------------------------------------------
            //Imaging.CreateBitmapSourceFromHBitmap方法，基于所提供的非托管位图和调色板信息的指针，
            //返回一个托管的BitmapSource
            ---------------------------------------------------------------------------*/

            IntPtr hbitmaPtr = bitmap.GetHbitmap(); //从GDI+ Bitmap创建GDI位图对象
            ImageSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hbitmaPtr, IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hbitmaPtr);
            return bitmapSource;
        }

        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            BitmapSource bs;
            IntPtr hBitPtr = IntPtr.Zero;
            try
            {
                hBitPtr = bitmap.GetHbitmap();
                bs = Imaging.CreateBitmapSourceFromHBitmap(hBitPtr, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception e)
            {
                bs = null;
            }
            finally
            {
                DeleteObject(hBitPtr);
            }
            return bs;
        }
        
        public static ImageSource ToImageSource2(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }

        // 这个是没有附加转换的，：）  
        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        public static ImageSource ChangeIconToImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;

        }

        public static BitmapImage CreateBitmapImage(this Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }
        
        public static BitmapImage CreateBitmapImageWithBys(byte[] byteArray)
        {
            BitmapImage bmp = null;
            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }
            return bmp;
        }

        public static byte[] GetByteArrayFormBitmapImage(BitmapImage bmp)
        {
            byte[] byteArray = null;
            try
            {
                var ms = bmp.StreamSource;
                if (ms != null && ms.Length > 0)
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        var oldPos = ms.Position;
                        ms.Position = 0;
                        byteArray = br.ReadBytes((int)ms.Length);
                        ms.Position = oldPos;
                    }
                }
            }
            catch
            {
                // ignored
            }
            return byteArray;
        }
    }
}
