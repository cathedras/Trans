using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Xml;
using Elcommon.AgLib;
using ElCommon.Util;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace TspUtil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ICommand validateCommand = new RoutedUICommand("Validate XML", "Validate", typeof(MainWindow),
       new InputGestureCollection { new KeyGesture(Key.V, ModifierKeys.Control | ModifierKeys.Shift) });


        public MainWindow()
        {

            InitializeComponent();

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            this.Loaded += MainWindow_Loaded;
            
            
        }
     
        public static ICommand ValidateCommand
        {
            get { return validateCommand; }
        }

        private void Validate(object sender, ExecutedRoutedEventArgs e)
        {
            //IServiceProvider sp = textEditor;
            //var markerService = (TextMarkerServices)sp.GetService(typeof(TextMarkerServices));
            //markerService.Clear();

            //try
            //{
            //    var document = new XmlDocument { XmlResolver = null };
            //    document.LoadXml(textEditor.Document.Text);
            //}
            //catch (XmlException ex)
            //{
            //    ProgrammeUtil.DisplayValidationError(textEditor,Vm.Text,ex.Message, ex.LinePosition, ex.LineNumber);
            //}
        }
       
        protected override void OnClosed(EventArgs e)
        {
            App.Locator.Main.SaveDataOnExit();
            if (File.Exists(App.Locator.Main.GblInfo.FileListXml))
            {
                App.Locator.Main.XmlDelete(App.Locator.Main.GblInfo.FileListXml);
            }
            App.Locator.Main.SaveFileOpenList(App.Locator.Main.GblInfo.FileListXml);
            Environment.Exit(0);
            base.OnClosed(e);
        }

        private HotKey _hotKey;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _hotKey = new HotKey(this, HotKey.KeyFlags.MOD_CONTROL, System.Windows.Forms.Keys.Q);
                _hotKey.OnHotKey += _hotKey_OnHotKey;
            }
            catch (Exception exception)
            {
                App.Locator.Main.AddLogMsg($"Fail to register hot-key. {exception.Message}", 1);
            }
            this.LocaClientAtCenter(App.Locator.Main.GblInfo.ClientWidth, App.Locator.Main.GblInfo.ClientHeight);
            App.Locator.Main.Pane = LayoutDocumentPane;
        }


        private void _hotKey_OnHotKey()
        {
           
        }

        //public ViewModel Vm
        //{
        //    get { return (ViewModel)GetValue(VmProperty); }
        //    set { SetValue(VmProperty, value); }
        //}
        
        //// Using a DependencyProperty as the backing store for Vm.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty VmProperty =
        //    DependencyProperty.Register("Vm", typeof(ViewModel), typeof(MainWindow), new PropertyMetadata(new ViewModel()));

        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FrameLog_OnSizeChange(object sender, SizeChangedEventArgs e)
        {
            //LogScroll.ScrollToEnd();
           // LogScrollPic.ScrollToEnd();

        }

        private void OnToolWindow1Hiding(object sender, CancelEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnLayoutRootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //var t = sender.GetType();
        }

        private void DockingManager_OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the document?", "Tips",
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                foreach (var item in App.Locator.Main.ProgrammeFiles)
                {
                    if (item.Des == e.Document.Title)
                    {
                        item.IsActived = false;
                        var edi = App.Locator.TextModal.AllFileEditor;
                        for (int i = 0; i < edi.Count; i++)
                        {
                            if (edi[i].FileName == item.Des)
                            {
                                App.Locator.TextModal.AllFileEditor.RemoveAt(i);
                                if (!App.Locator.TextModal.AllFileEditor.Any())
                                {
                                    App.Locator.Main.CurProgmFile = string.Empty;
                                }
                                break;
                            }
                        }
                        break;
                    }
                }

            }
        }

        private void LayoutDocumentPane_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var s = (LayoutDocumentPane)sender;
            var index = s.SelectedContentIndex;
            if (index >= 0)
            {
                foreach (var item in App.Locator.Main.ProgrammeFiles)
                {
                    var p = s.Children[index].Title;
                    if (p.Equals(item.Des))
                    {
                        App.Locator.Main.CurProgmFile = item.FnPath;
                        break;
                    }
                }
            }
            
        }
    }
}
