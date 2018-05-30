using System;

namespace myzy.Util
{
    public class OldWindow : System.Windows.Forms.IWin32Window
    {
        private readonly IntPtr _handle;

        public OldWindow(IntPtr handle)
        {
            _handle = handle;
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }
    }
}