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
using System.Windows.Shapes;

namespace TspUtil
{
    /// <summary>
    /// Interaction logic for NormalAbout.xaml
    /// </summary>
    public partial class NormalAbout : Window
    {
        public NormalAbout(string hwVer,string mcuVer,string fpgaVer,string swVer,string description)
        {
            InitializeComponent();
            HwVer.Text = hwVer;
            McuVer.Text = mcuVer;
            FpgaVer.Text = fpgaVer;
            SwVer.Text = swVer;
            Descrition.Text = description;
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
