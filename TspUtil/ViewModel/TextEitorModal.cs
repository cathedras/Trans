using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using GalaSoft.MvvmLight;
using log4net;
using ScintillaNET;
using Xceed.Wpf.AvalonDock.Layout;
using Color = System.Drawing.Color;
using Style = ScintillaNET.Style;

namespace TspUtil.ViewModel
{
    public class TextEitorModal:ViewModelBase
    {
       
       // private TextMarkerServices _text;
        private ToolTip _toolTip;
        private string _fileName = string.Empty;
        private Scintilla _editor;
        private int _maxLineNumberCharLength;
      //  private IHighlightingDefinition _xshd;
        private ScrollViewer _break;
        //private TextView _textView;
        private ListBox _listB;
       // private ObservableCollection<LineBreakPoint> _breakPoints;
        private string _headerName = string.Empty;
        private readonly static ILog _log = LogManager.GetLogger("exlog");
        private List<EditorContent> _allFileEditor;
        private double _lineHeight;

        public string HeaderName
        {
            get { return _headerName; }
            set
            {
                if (_headerName == value)return;
                _headerName = value; 
                RaisePropertyChanged();
            }
        }


        public List<EditorContent> AllFileEditor
        {
            get { return _allFileEditor??(_allFileEditor=new List<EditorContent>()); }
        }

        public Scintilla Editor
        {
            get { return _editor; }
            set { _editor = value; }
        }

        //private void InitSyntaxColoring()
        //{
        //    Editor.StyleResetDefault();
        //    Editor.Styles[ScintillaNET.Style.Default].Font = "Consolas";
        //    Editor.Styles[ScintillaNET.Style.Default].Size = 10;
        //    Editor.StyleClearAll();

        //    Editor.Styles[CustomLexer.StyleDefault].ForeColor = System.Drawing.Color.Black;
        //    Editor.Styles[CustomLexer.StyleKeyword].ForeColor = System.Drawing.Color.Orange;
        //    Editor.Styles[CustomLexer.StyleIdentifier].ForeColor = System.Drawing.Color.Teal;
        //    Editor.Styles[CustomLexer.StyleNumber].ForeColor = System.Drawing.Color.Purple;
        //    Editor.Styles[CustomLexer.StyleString].ForeColor = System.Drawing.Color.Red;
        //    Editor.Styles[CustomLexer.StyleComment].ForeColor = System.Drawing.Color.Green;

        //    Editor.Lexer = Lexer.Container;
        //}
        private static string regexStr = "mipi lr oe data cycle dsc dsi power down pwm driver video write read power on reset gpio ms s us delay pll external print debug usart all usb eth cphy dphy pclk mode lane timing enable disable colorbar nonburstevents nonburstPulses interface dualMode singleMode register order rgb bgr ch show image color off";

        private void InitSyntaxColoring()
        {
            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            Editor.StyleResetDefault();
            Editor.Styles[Style.Default].Font = "Consolas";
            Editor.Styles[Style.Default].Size = 10;
            Editor.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            Editor.Styles[Style.Cpp.Default].ForeColor = Color.Black;
            Editor.Styles[Style.Cpp.Comment].ForeColor = System.Drawing.Color.Green;
            Editor.Styles[Style.Cpp.CommentLine].ForeColor = System.Drawing.Color.Green;
            Editor.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            Editor.Styles[Style.Cpp.Number].ForeColor = System.Drawing.Color.Purple;
            Editor.Styles[Style.Cpp.Word].ForeColor = System.Drawing.Color.Orange;
            Editor.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            //Editor.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
           // Editor.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            Editor.Styles[Style.Cpp.StringEol].BackColor = System.Drawing.Color.Red;
            Editor.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
         //   Editor.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
            Editor.Lexer = Lexer.Cpp;

            // Set the keywords
            Editor.SetKeywords(0, $"{regexStr}");
        }
        public void ShowLineNumber()
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var length = Editor.Lines.Count.ToString().Length;
            if (this._maxLineNumberCharLength == length)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 25;
            Editor.Margins[0].Width = Editor.TextWidth(ScintillaNET.Style.LineNumber, new string('9', _maxLineNumberCharLength + 1)) + padding;
            this._maxLineNumberCharLength = length;
        }

        public void ShowCurrentLine()
        {
           // Editor.Margins[3].Width = Editor.TextWidth(Style.)
        }
        public static readonly int BOOKMARK_MARGIN = 1;
        public static readonly int BOOKMARK_MARKER = 1;
        private void InitBookmarkMargin()
        {

            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = Editor.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.RightText;
            margin.Mask = (uint)(1 << BOOKMARK_MARKER);
            margin.Cursor = MarginCursor.Arrow;

            var marker = Editor.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(System.Drawing.Color.FromArgb(0xff, 0xFF, 0x00, 0x3b));//Color.FromRgb(0xFF, 0x00,0x3b)
            marker.SetForeColor(System.Drawing.Color.Black);//IntToColor(0x000000)
            marker.SetAlpha(100);

            //var mg = Editor.Margins[2];
            //margin.Sensitive = true;
            //mg.Width = 20;
            //mg.Type = MarginType.Color;
            //mg.Mask = 1;

            //var mk = Editor.Markers[2];
            //mk.SetBackColor(Color.Brown);
        }

        public EditorContent CurFileEdit(string fname)
        {
            EditorContent edit=null;
            foreach (var editorContent in AllFileEditor)
            {
                if (editorContent.FileName.Equals(fname))
                {
                    edit = editorContent;
                    break;
                }
            }

            return edit;
        }

        public LayoutDocument CreateAnDocumentEditor(string text, string fileName, string filePath,out EditorContent editor)
        {
            var view = new TextEditorTemplate();
            var doc = new LayoutDocument();
            doc.Content = view;
            doc.Title = fileName;
            Editor.Text = text;
            editor = null;
            if (!AllFileEditor.Exists(p => p.FileName == fileName))
            {
                editor = new EditorContent(fileName, filePath, Editor);
                AllFileEditor.Add(editor);
            }
            return doc;
        }

        public void SaveFile(string curFile=null)
        {
            foreach (var editorContent in AllFileEditor)
            {
                if (!string.IsNullOrEmpty(curFile))
                {
                    editorContent.Save();
                    break;
                }
                else
                {
                    editorContent.Save();
                }
            }
        }

      

        public void InitPrograme(Scintilla textEditor)
        {
            Editor = textEditor;
            InitSyntaxColoring();
            ShowLineNumber();
            InitBookmarkMargin();
        }
    }

    public class EditorContent
    {
        private Scintilla _editor;
        private string _fileName;
        private string _filePath;


        public EditorContent(string fileName,string fnPath, Scintilla editor)
        {
            _fileName = fileName;
            _filePath = fnPath;
            _editor = editor;
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
            }
        }
        
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public Scintilla Editor
        {
            get { return _editor; }
            set { _editor = value; }
        }

        public void Save()
        {
            File.WriteAllText(FilePath,Editor.Text);
        }
    }
}
