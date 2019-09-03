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

namespace TspUtil
{
    /// <summary>
    /// Interaction logic for TextEditorTemplate.xaml
    /// </summary>
    public partial class TextEditorTemplate : UserControl
    {
        public TextEditorTemplate()
        {
            InitializeComponent();
            App.Locator.TextModal.InitPrograme(textEditor, BreakDown, BreakList);
        }

        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                //foreach (var content in App.Locator.TextModal.AllFileEditor)
                //{
                //    if (App.Locator.Main.CurProgmFile == content.FilePath && !content.TextContent.Equals(textEditor.Text))
                //    {

                //        break;
                //    }
                //}
            });
            
        }
    }
}
