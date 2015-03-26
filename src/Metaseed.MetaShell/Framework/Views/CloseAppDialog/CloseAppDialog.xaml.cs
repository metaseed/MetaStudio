using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Metaseed.MetaShell.Views
{
    public enum CloseAppDialogButtons { Quit,SaveAndQuit,Cancel}
    /// <summary>
    /// Interaction logic for CloseAppDiag.xaml
    /// </summary>
    public partial class CloseAppDialog : Window
    {
        public CloseAppDialogButtons SelectedButton ;
        public CloseAppDialog()
        {
            InitializeComponent();
            SelectedButton = CloseAppDialogButtons.Cancel;

        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            SelectedButton = CloseAppDialogButtons.Quit; this.Close();
        }

        private void QuitAndSave_Click(object sender, RoutedEventArgs e)
        {
            SelectedButton = CloseAppDialogButtons.SaveAndQuit; this.Close();
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            SelectedButton = CloseAppDialogButtons.Cancel; this.Close();
        }
    }
}
