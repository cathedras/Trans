using System.Windows;

namespace myzy.AgCustom
{
    /// <summary>
    /// 一些常用Arc包装
    /// </summary>
    public static class AgCustomHelperEx
    {
        public static void LocaClientAtCenter(this Window wnd, int width = 1024, int height = 768)
        {
            if (wnd != null)
            {
                var screenHeight = SystemParameters.FullPrimaryScreenHeight;
                var screenWith = SystemParameters.FullPrimaryScreenWidth;
                wnd.Height = height;
                wnd.Width = width;
                wnd.Top = (screenHeight - wnd.Height) / 2;
                wnd.Left = (screenWith - wnd.Width) / 2;
            }
        }
        
    }
}