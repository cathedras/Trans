using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Elcommon.AgLib;
using ElCommon.Util;

namespace TspUtil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            this.Loaded += MainWindow_Loaded;
        }

        protected override void OnClosed(EventArgs e)
        {
            Vm.SaveDataOnExit();
            if (File.Exists(Vm.GblInfo.FileListXml))
            {
                Vm.XmlDelete(Vm.GblInfo.FileListXml);
            }
            Vm.SaveFileOpenList(Vm.GblInfo.FileListXml);
            Environment.Exit(0);
            base.OnClosed(e);
        }

        private HotKey _hotKey;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _hotKey = new HotKey(this, HotKey.KeyFlags.MOD_CONTROL, Keys.Q);
                _hotKey.OnHotKey += _hotKey_OnHotKey;
            }
            catch (Exception exception)
            {
                Vm.AddLogMsg($"Fail to register hot-key. {exception.Message}", 1);
            }
            this.LocaClientAtCenter(Vm.GblInfo.ClientWidth, Vm.GblInfo.ClientHeight);
        }


        private void _hotKey_OnHotKey()
        {
           
        }

        public ViewModel Vm
        {
            get { return (ViewModel)GetValue(VmProperty); }
            set { SetValue(VmProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Vm.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VmProperty =
            DependencyProperty.Register("Vm", typeof(ViewModel), typeof(MainWindow), new PropertyMetadata(new ViewModel()));
    }
}
