using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using ScintillaNET;
using TspUtil.ViewModel;

namespace TspUtil
{
    /// <summary>
    /// Interaction logic for TextEditorTemplate.xaml
    /// </summary>
    public partial class TextEditorTemplate : UserControl
    {
        private static string regexStr = "mipi lr dsc dsi power down pwm driver video write power on reset gpio ms s us delay pll lane timing enable disable colorbar nonburstevents interface dualmode order rgb bgr ch show image color";
        public TextEditorTemplate()
        {
            InitializeComponent();
            //App.Locator.TextModal.Editor = TextEditor;
            App.Locator.TextModal.InitPrograme(TextEditor);
        }

        private CustomLexer cSharpLexer = new CustomLexer(regexStr);

        /// <summary>
        /// 自定义高亮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scintilla_StyleNeeded(object sender, StyleNeededEventArgs e)
        {
           // var startPos = TextEditor.GetEndStyled();
           // var endPos = e.Position;
           //// App.Locator.Main.AddLogMsg($"{startPos++},{endPos}");
           // cSharpLexer.Style(TextEditor, startPos, endPos);
        }
        /// <summary>
        /// 行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_OnTextChanged(object sender, EventArgs e)
        {
             App.Locator.TextModal.ShowLineNumber();
        }
        /// <summary>
        /// 断点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scintilla_OnMarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == TextEitorModal.BOOKMARK_MARGIN)
            {
                // Do we have a marker for this line?
                uint mask = (uint)(1 << TextEitorModal.BOOKMARK_MARKER);
                var line = TextEditor.Lines[TextEditor.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(TextEitorModal.BOOKMARK_MARKER);
                }
                else
                {
                    // Add bookmark
                    line.MarkerAdd(TextEitorModal.BOOKMARK_MARKER);
                    
                }
            }
        }
        /// <summary>
        /// 自动完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scintilla_OnCharAdded(object sender, CharAddedEventArgs e)
        {
            var currentPos = TextEditor.CurrentPosition;
            var wordStartPos = TextEditor.WordStartPosition(currentPos, true);

            // Display the autocompletion list
            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0)
            {
                if (!TextEditor.AutoCActive)
                    TextEditor.AutoCShow(lenEntered, regexStr);
            }
        }

        private void TextEditor_OnCursorChanged(object sender, EventArgs e)
        {
            var typ = sender;
        }

        private void TextEditorTemplate_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;
            TextEditor.Width = (int) size.Width;
            TextEditor.Height = (int) size.Height;
        }
    }
}
