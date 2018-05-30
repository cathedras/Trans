using System.Drawing;
using System.IO;
using System.Linq;

namespace myzy.Util
{
    public class BitmapHelper
    {
        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            var format = image.RawFormat;
            byte[] buffer = null;
            using (var ms = new MemoryStream())
            {
                var arr = new[]
                {
                    System.Drawing.Imaging.ImageFormat.Jpeg,
                    System.Drawing.Imaging.ImageFormat.Png,
                    System.Drawing.Imaging.ImageFormat.Bmp,
                    System.Drawing.Imaging.ImageFormat.Gif,
                    System.Drawing.Imaging.ImageFormat.Icon,
                    System.Drawing.Imaging.ImageFormat.MemoryBmp,
                };
                var item = arr.ToList().FirstOrDefault(p => Equals(p, format));
                if (item != null)
                {
                    image.Save(ms, item);
                    buffer = new byte[ms.Length];
                    //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(buffer, 0, buffer.Length);
                }
            }
            return buffer;
        }

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        /// <summary>
        /// Convert Byte[] to a picture and Store it in file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            var format = image.RawFormat;
            if (format.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(System.Drawing.Imaging.ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(System.Drawing.Imaging.ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(System.Drawing.Imaging.ImageFormat.Icon))
            {
                file += ".icon";
            }
            FileInfo info = new FileInfo(file);
            Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }
    }
}