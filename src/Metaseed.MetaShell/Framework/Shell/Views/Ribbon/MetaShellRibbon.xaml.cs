using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Catel.IoC;
using Microsoft.Practices.Prism.Commands;
namespace Metaseed.MetaShell.Views
{
    using Services;
    using ViewModels;

    public partial class MetaShellRibbon
    {
        public MetaShellRibbon()
        {
            DocumentsCompositeCollection = new CompositeCollection();
            InitializeComponent();
            this.Loaded += ShellRibbon_Loaded;
            OpenRecentFileCommand = new DelegateCommand<string>(OpenRecentFileCommand_Executed);
        }

        void ShellRibbon_Loaded(object sender, RoutedEventArgs e)
        {
            var unopenDocsContainer=new CollectionContainer();
            var unopenDocsBinding = new Binding("DocumentsUnopen")
            {
                Source = ((ShellService)(base.ShellViewModel.ShellService))
            };
            BindingOperations.SetBinding(unopenDocsContainer, CollectionContainer.CollectionProperty, unopenDocsBinding);
            DocumentsCompositeCollection.Add(unopenDocsContainer);
            var openDocsContainer = new CollectionContainer();
            var openDocsBinding = new Binding("Documents")
            {
                Source = ShellViewModel.ShellService
            };
            BindingOperations.SetBinding(openDocsContainer, CollectionContainer.CollectionProperty, openDocsBinding);
            DocumentsCompositeCollection.Add(openDocsContainer);
            //DocumentsCompositeCollection.Add( ((ShellService)(ShellViewModel.ShellService)).DocumentsUnopen );
            //DocumentsCompositeCollection.Add(ShellViewModel.ShellService.Documents );
        }

        public CompositeCollection DocumentsCompositeCollection { get; set; }
        //public RibbonTabItem HomeRibbonTab { get { return RibbonTabHome; } }
        //public RibbonGroupBox FileRibbonGroupBox { get { return RibbonGroupBoxFile; } }

        private void AddPluginButton_Click(object sender, RoutedEventArgs e)
        {
            int a = 0;
            var b = 7/a;
            b = b++;
            Microsoft.Win32.FileDialog dialog;
            dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Add Plugin To The Application(Chose A CANStudio Plugin File)";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Filter = "PlugIn(*.dll)|*.dll|All (*.*)|*.*";
            dialog.DefaultExt = ".dll";
            if (dialog.ShowDialog() == true)
            {
                string appPath = Metaseed.AppEnvironment.AppPath;
                string fileName = dialog.FileName.Split('\\').Last();
                string fullFileName = appPath + @"\Plugin\" + fileName;
                if (System.IO.File.Exists(fullFileName))
                {
                    if (MessageBoxResult.OK == MessageBox.Show("There already has a plugin named " + fileName + System.Environment.NewLine + "Are you want to replace it with the selected new one?", "Replacement", MessageBoxButton.OKCancel))
                    {
                        try
                        {
                            System.IO.File.Delete(fullFileName + ".del");
                        }
                        catch (Exception)
                        {
                            //http://social.msdn.microsoft.com/forums/en-US/wpf/thread/0dca0561-d3e1-4f75-8675-28da404eefde
                            MessageBox.Show("You must restart the application to updated plugin again!!");
                            return;
                        }

                        System.IO.File.Move(fullFileName, fullFileName + ".del");
                        System.IO.File.Copy(dialog.FileName, fullFileName);
                        MessageBox.Show("You must restart the application to use the updated plugin!!");
                    }
                    else
                    {
                        MessageBox.Show("the already exiting plugin is not updated");
                    }
                }
                else
                {
                    System.IO.File.Copy(dialog.FileName, fullFileName);
                    MessageBox.Show("You must restart the application to use the plugin");
                }
            }
        }

        private void AppClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
        public ICommand OpenRecentFileCommand { get; private set; }
        private void RecentFile_Click(object sender, RoutedEventArgs e)
        {
            var s = (Fluent.MenuItem)sender;
            var file = s.Header.ToString();
            ((MetaShellViewModel)ShellViewModel).PackageService.Open(file);
        }

        void OpenRecentFileCommand_Executed(string file)
        {
            ((MetaShellViewModel)ShellViewModel).PackageService.Open(file);
        }
        Dictionary<string, IPrint> _printableViews = null;
        Dictionary<string, IPrint> PrintableViews
        {
            get
            {
                if (_printableViews != null)
                {
                    return _printableViews;
                }
                var rv = ShellViewModel.ShellService.Documents.OfType<IPrint>().ToDictionary(document => document.Title);
                //IShellService shellService = ServiceLocator.Current.GetInstance<IShellService>();
                _printableViews = rv;
                return _printableViews;
            }
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintableViews[selection_print.Text].PrintPage();
        }

        private void Print_SelectionChanged_Click(object sender, SelectionChangedEventArgs e)
        {
            PrintPreview.Source = PrintableViews[selection_print.SelectedItem.ToString()].GetInvertBitmap(false);
        }

        private void Invert_Checked(object sender, RoutedEventArgs e)
        {
            PrintPreview.Source = PrintableViews[selection_print.Text].GetInvertBitmap(true);
        }

        private void Invert_UnChecked(object sender, RoutedEventArgs e)
        {
            PrintPreview.Source = PrintableViews[selection_print.Text].GetInvertBitmap(false);
        }

        private void PintSaveAs_Click(object sender, RoutedEventArgs e)
        {
            PrintableViews[selection_print.Text].SaveAsPNG();
        }

        private void PrintBackStage_Click(object sender, MouseButtonEventArgs e)
        {
            selection_print.ItemsSource = PrintableViews.Keys;
            if (selection_print.SelectedIndex == -1)
            {
                selection_print.SelectedIndex = 0;
            }
        }

        private void langusge_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShellViewModel.ShellService.CurrentCulture.Equals(new System.Globalization.CultureInfo("zh-hans")))//Chinese
            {
                const string fontName = "微软雅黑"; //"Microsoft YaHei";
                var testFont = new System.Drawing.Font(fontName, 18.0f);
                if (testFont.Name != fontName)
                {
                    // The font we tested doesn't exist, so fallback to Times.
                    //this.Font = new Font("Times New Roman", 16.0f,
                    //         FontStyle.Regular, GraphicsUnit.Pixel);
                    var r = MessageBox.Show("若没有ClearType字库可能造成中文显示不清晰。\n检测到系统内没有安装微软雅黑字体，是否安装？", "安装字库", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (r == MessageBoxResult.Yes)
                    {
                        string fontDir = Metaseed.AppEnvironment.AppPath + @"\XPFont\VistaFont_CHS.EXE";
                        if (System.IO.File.Exists(fontDir))
                        {
                            System.Diagnostics.Process.Start(fontDir);
                        }
                        else
                        {
                            System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/zh-cn/details.aspx?FamilyID=b15774c0-5b42-48b4-8ba8-9293fdc72099");
                        }
                    }
                }
            }
        }

        private void OnHelpClick(object sender, RoutedEventArgs e)
        {
            Version version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            MessageBox.Show("Version:" + version.ToString()+Environment.NewLine+"http://www.metaseed.com", "Application Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        private void OnBrowserClick(object sender, RoutedEventArgs e)
        {
            var browserDoc = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<Metaseed.Modules.Browser.ViewModels.BrowserDocumentViewModel>();
            browserDoc.ShellService.OpenDocument(browserDoc);
        }



    }
}
