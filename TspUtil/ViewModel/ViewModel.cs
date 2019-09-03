using System.Collections.Concurrent;
using System.Drawing;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.VisualStyles;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace TspUtil
{
    public interface IViewModel
    {
        void AddLogMsg(string msg, int level = 0);
        void SaveDataFrame(string msg);
    }
}
