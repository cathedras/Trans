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
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using log4net;
using Xceed.Wpf.AvalonDock.Layout;

namespace TspUtil.ViewModel
{
    public class TextEitorModal:ViewModelBase
    {
       
        private TextMarkerServices _text;
        private ToolTip _toolTip;
        private string _fileName = string.Empty;
        private TextEditor _editor;
        private IHighlightingDefinition _xshd;
        private ScrollViewer _break;
        private TextView _textView;
        private ListBox listB;
        private ObservableCollection<LineBreakPoint> _breakPoints;
        private string _headerName = string.Empty;
        private readonly static ILog _log = LogManager.GetLogger("exlog");
        private List<EditorContent> _allFileEditor;

        public TextEditor Editor
        {
            get => _editor;
        }

        public TextMarkerServices Text
        {
            get => _text;
            set => _text = value;
        }
        public ObservableCollection<LineBreakPoint> BreakPoints
        {
            get => _breakPoints ?? (_breakPoints = new ObservableCollection<LineBreakPoint>());
        }

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

        public AlgProgmFiles ProgrammeLine(string curFilePath, Dictionary<string, AlgCmd> algMap,out IList<DocumentLine> edLines)
        {
            AlgProgmFiles apf = null;
            edLines = null;
            if (curFilePath != null && algMap!=null)
            {
                foreach (var editorContent in AllFileEditor)
                {
                    if (editorContent.FilePath == curFilePath)
                    {
                        IList<DocumentLine> tmp = null;
                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            tmp = editorContent.Editor.Document.Lines; 

                        });
                        edLines = tmp;
                        apf = new AlgProgmFiles(curFilePath, editorContent.Editor, algMap);
                        break;
                    }
                }
            }
            return apf;
        }

        public TextEditor CurFileEdit(string fname)
        {
            TextEditor edit = null;
            foreach (var editorContent in AllFileEditor)
            {
                if (editorContent.FileName == fname)
                {
                    edit = editorContent.Editor;
                    break;
                }
            }

            return edit;
        }

        public LayoutDocument CreateAnDocumentEditor(string text,string fileName,string filePath)
        {
            var view = new TextEditorTemplate();
            var doc = new LayoutDocument();
            doc.Content = view;
            doc.Title = fileName;
            _editor.Text = text;
            BreakPoints.Clear();
            for (int i = 0; i < _editor.LineCount; i++)
            {
                BreakPoints.Add(new LineBreakPoint($"{i + 1}"));
            }

            if (!AllFileEditor.Exists(p=>p.FileName==fileName))
            {
                AllFileEditor.Add(new EditorContent(fileName, text, filePath, _editor));
            }
            
            _break.Height = Editor.TextArea.TextView.DocumentHeight;
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
        public void InitPrograme(TextEditor editor, ScrollViewer list, ListBox lb)
        {
            listB = lb;
            _break = list;
            XshdSyntaxDefinition xssd = null;
            _editor = editor;
            _text = new TextMarkerServices(editor);
            _textView = editor.TextArea.TextView;
            _textView.BackgroundRenderers.Add(_text);
            _textView.LineTransformers.Add(_text);
            _textView.Services.AddService(typeof(TextMarkerServices), _text);
           
            try
            {
                using (XmlReader reader = new XmlTextReader("../CustomMode-Mode.xshd"))
                {
                    _xshd = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    
                    //var sa = new SaveXshdVisitor();
                }
            }
            catch (Exception ex)
            {
                _log.Debug($"加载xshd配置文件失败：{ex.Message}{ex.StackTrace}");
            }
  
            var setting = new XmlReaderSettings();
            setting.ValidationType = ValidationType.Schema;
            var xmlR = XmlReader.Create(File.Open("../CustomMode-Mode.xshd", FileMode.Open), setting);
            xssd = HighlightingLoader.LoadXshd(xmlR);
            var els = xssd.Elements;
            var exs = xssd.Extensions;
            var n = xssd.Name;


            var str = File.Create("../Visitor.xml");
            var wr = XmlWriter.Create(str);
            var keys = new XshdKeywords();
            keys.AcceptVisitor(new SaveXshdVisitor(wr));
            var allKeys = keys.Words;


            editor.SyntaxHighlighting = _xshd;
            _textView.MouseHover += MouseHover;
            //textView.MouseHoverStopped += TextEditorMouseHoverStopped;
            _textView.VisualLinesChanged += VisualLinesChanged;
            editor.TextArea.TextEntering += EditorEntering;
            editor.TextArea.TextEntered += EditorEntered;
            _textView.ScrollOffsetChanged += EditorScroll;


            //var els = xssd.Elements;
            //var exs = xssd.Extensions;
            //var n = xssd.Name;
        }

        CompletionWindow _completion;

        void EditorEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                // Open code completion after the user has pressed dot:
                _completion = new CompletionWindow(_editor.TextArea);
                IList<ICompletionData> data = _completion.CompletionList.CompletionData;
                data.Add(new CompletionData("Item1"));
                data.Add(new CompletionData("Item2"));
                data.Add(new CompletionData("Item3"));
                _completion.Show();
                _completion.Closed += delegate
                {
                    _completion = null;
                };
            }
        }

        void EditorScroll(object sender, EventArgs e)
        {
            var s = sender;
            var t = _break.ScrollableHeight / BreakPoints.Count;
            var ss = _break.VerticalOffset;
            _break.ScrollToVerticalOffset(_textView.VerticalOffset - 4);
            var he = _textView.DefaultLineHeight;

            //textView.CurrentLineBackground = new SolidColorBrush(Colors.AliceBlue);
        }

        void EditorEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completion != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completion.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }
        private void MouseHover(object sender, MouseEventArgs e)
        {
            var posx = e.GetPosition(_editor.TextArea.TextView) + _editor.TextArea.TextView.ScrollOffset;
            var pos = Editor.TextArea.TextView.GetPositionFloor(posx);
            bool inDocument = pos.HasValue;
            if (inDocument)
            {
                var logicalPosition = pos.Value.Location;
                int offset = _editor.Document.GetOffset(logicalPosition);

                var markersAtOffset = _text.GetMarkersAtOffset(offset);
                var markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

                if (markerWithToolTip != null)
                {
                    if (_toolTip == null)
                    {
                        _toolTip = new ToolTip();
                        _toolTip.Closed += ToolTipClosed;
                        _toolTip.PlacementTarget = Editor;
                        _toolTip.Content = new System.Windows.Controls.TextBlock
                        {
                            Text = markerWithToolTip.ToolTip,
                            TextWrapping = TextWrapping.Wrap
                        };
                        _toolTip.IsOpen = true;
                        e.Handled = true;
                    }
                }
            }
        }

        void ToolTipClosed(object sender, RoutedEventArgs e)
        {
            _toolTip = null;
        }
        void TextEditorMouseHoverStopped(object sender, MouseEventArgs e)
        {
            if (_toolTip != null)
            {
                _toolTip.IsOpen = false;
                e.Handled = true;
            }
        }
        private void VisualLinesChanged(object sender, EventArgs e)
        {
            var view = (TextView)sender;
            //view.LinkTextBackgroundBrush = new SolidColorBrush(Colors.LightSkyBlue);
            //view.CurrentLineBackground=new SolidColorBrush(Colors.Red);
            foreach (var editorContent in AllFileEditor)
            {
                if (editorContent.FilePath == App.Locator.Main.CurProgmFile)
                {
                    editorContent.SelectedLine = view.HighlightedLine;
                    break;
                }
            }
        }

    }

    public class EditorContent
    {
        private TextEditor _editor;
        private string _fileName;
        private string _filePath;
        private string _textContent;
        private int _selectedLine;


        public EditorContent(string fileName, string textContent,string fnPath,TextEditor editor)
        {
            _fileName = fileName;
            _textContent = textContent;
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

        public string TextContent
        {
            get { return _textContent; }
            set
            {
                _textContent = value;
            }
        }

        public int SelectedLine
        {
            get { return _selectedLine; }
            set { _selectedLine = value; }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public TextEditor Editor
        {
            get { return _editor; }
            set { _editor = value; }
        }

        public void Save()
        {
            var file = File.Open(FilePath, FileMode.Open, FileAccess.ReadWrite);
            if (file.CanWrite)
            {
                Editor.Save(file);
                file.Flush();
                file.Close();

            }
        }
    }
}
