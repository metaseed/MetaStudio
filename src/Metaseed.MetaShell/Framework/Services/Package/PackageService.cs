using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Catel.IoC;
using Catel.MVVM;
using Catel.Logging;
using Catel.Data;
using Catel.Services;
using System.IO.Packaging;
using System.Net.Mime;
using System.IO;
using System.Collections.ObjectModel;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.AvalonDock.Layout;

namespace Metaseed.MetaShell.Services
{
    using ViewModels;
    using Infrastructure;
    using Properties;
    using Metaseed.Collections.Generic;
    using Metaseed.Windows.Controls;
    using Metaseed.Reflection;
    public class PackageService : ObservableObject, IPackageService
    {
        public static string Package_PanelLayoutPart_ID = "PanelLayoutID";
        public static string Package_PanelLayoutPart_Path = "/PanelLayout.xml";
        public static string Package_DocumentsUnopen_ID = "DocumentsUnopenID";
        public static string Package_DocumentsUnopen_Path = "/DocumentsUnopen.xml";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private IShellService _shellService;
        public IShellService ShellService
        {
            get
            {
                if (_shellService != null)
                {
                    return _shellService;
                }
                return _shellService = this.GetDependencyResolver().Resolve<IShellService>();
            }
        }

        #region Constructors
        public PackageService()
        {
            CurrentPackagePath = GloableStaticInstanse.StartupInputFilePathName;
            NewCommand = new DelegateCommand(NewCommand_Executed);
            GloableCommands.NewCommand.RegisterCommand(NewCommand);

            OpenCommand = new DelegateCommand(Open_Executed);

            SaveCommand = new DelegateCommand(Save_Executed, Save_CanExecuted);

            SaveAsCommand = new DelegateCommand(SaveAs_Executed);

            DelDocumentCommand = new Command<IPackageDocumentViewModel, IPackageDocumentViewModel>(OnDelDocumentCommandExecute, OnDelDocumentCommandCanExecute);

            OpenDocumentCommand = new Command<IPackageContent, IPackageContent>(OnOpenDocumentCommandExecute, OnOpenDocumentCommandCanExecute);

            SaveDocumentCommand = new Command<IPackageDocumentViewModel, IPackageDocumentViewModel>(OnSaveDocumentCommandExecute, OnSaveDocumentCommandCanExecute);

            SaveCurrentDocumentCommand = new Command(OnSaveCurrentDocumentCommandExecute, OnSaveCurrentDocumentCommandCanExecute);

            var handler = this.GetDependencyResolver().Resolve<IExceptionHandler>();
            if (handler != null) handler.SaveOnException += ShellViewModel_SaveOnException;
        }
        #endregion

        public ICommand NewCommand { get; private set; }
        readonly string _newFileName = AppEnvironment.AppPath + @"\new.meta";
        void NewCommand_Executed()
        {
            if (File.Exists(_newFileName))
            {
                OpenMetaFile(_newFileName, false);
                CurrentPackagePath = string.Empty;
            }
            else
            {
                _shellService.Messager.MessageBox.Show("No new.meta file in application directory.");
            }
        }

        #region Properties

        private const int RecentFileNumber = 20;

        ObservableCollection<string> _recentFiles;
        public ObservableCollection<String> RecentFiles
        {
            get
            {
                if (_recentFiles == null)
                {
                    _recentFiles = new ObservableCollection<string>();
                    foreach (var item in Properties.Settings.Default.RecentFiles)
                    {
                        _recentFiles.Add(item);
                    }
                }
                return _recentFiles;
            }
        }
        void AddToRecentFiles(string fileName)
        {
            if (RecentFiles.Contains(fileName))
            {
                RecentFiles.Remove(fileName);
                Properties.Settings.Default.RecentFiles.Remove(fileName);
                RecentFiles.Insert(0, fileName);
                Properties.Settings.Default.RecentFiles.Insert(0, fileName);
            }
            else
            {
                if (RecentFiles.Count >= RecentFileNumber)
                {
                    RecentFiles.RemoveAt(RecentFileNumber - 1);
                    Properties.Settings.Default.RecentFiles.RemoveAt(RecentFileNumber - 1);
                }
                RecentFiles.Insert(0, fileName);
                Properties.Settings.Default.RecentFiles.Insert(0, fileName);
            }
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Commands
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public Command<IPackageDocumentViewModel, IPackageDocumentViewModel> DelDocumentCommand { get; private set; }
        public Command<IPackageContent, IPackageContent> OpenDocumentCommand { get; private set; }
        public Command<IPackageDocumentViewModel, IPackageDocumentViewModel> SaveDocumentCommand { get; private set; }
        public Command SaveCurrentDocumentCommand { get; private set; }
        #endregion

        #region Methods
        #region private methods

        public void Open_Executed()
        {
            var openFileService = this.GetDependencyResolver().Resolve<IOpenFileService>();
            openFileService.AddExtension = true;
            openFileService.CheckFileExists = true;
            openFileService.CheckPathExists = true;
            openFileService.Filter = "MetaStudio Package File(*.meta)|*.meta|All (*.*)|*.*";
            openFileService.FilterIndex = 0;
            if (string.IsNullOrEmpty(CurrentPackagePath))
            {
                if (RecentFiles.Count > 0 && File.Exists(RecentFiles[0]))
                {
                    openFileService.InitialDirectory = RecentFiles[0];
                }
                else
                {
                    openFileService.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                }
            }
            else
            {
                openFileService.InitialDirectory = CurrentPackagePath;
            }

            openFileService.Title = Resources.OpenFileDialogTitle;
            var bFile = openFileService.DetermineFile();
            if (bFile)
            {
                Open(openFileService.FileName);
            }

        }
        void ShellViewModel_SaveOnException(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CurrentPackagePath))
            {
                string filePath = CurrentPackagePath + ".BackupOnException.meta";
                Save(filePath);
                MessageBox.Show("Your Configuration Data Has Been Saved To File:" + filePath, "Save Your Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        bool Save_CanExecuted()
        {
            if (string.IsNullOrEmpty(CurrentPackagePath))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        void SafeSave()
        {
            if (File.Exists(CurrentPackagePath + ".backup"))
            {
                File.Delete(CurrentPackagePath + ".backup");
            }
            if (File.Exists(CurrentPackagePath))//save :exist;saveas :not exist
            {
                var backupSaveFilePath = CurrentPackagePath + ".backup";
                File.Copy(CurrentPackagePath, backupSaveFilePath);
            }

            Save(CurrentPackagePath);
            if (File.Exists(CurrentPackagePath + ".backup"))
            {
                File.Delete(CurrentPackagePath + ".backup");
            }
        }
        public void Save(string fileName)
        {
            //try
            {
                SavePackage(fileName);
                AddToRecentFiles(fileName);
                ShellService.Messager.Balloon.ShowCustomBalloon(Resources.Information, Resources.FileSavedMessage, BalloonIcon.Info, PopupAnimation.Slide, 4000);
            }
            //catch (Exception e)
            //{
            //    if (File.Exists(fileName))
            //    {
            //        File.Delete(fileName);
            //    }
            //    if (File.Exists(fileName + ".backup"))
            //    {
            //        File.Copy(fileName + ".backup", fileName);
            //    }
            //    MessageBox.Show("File Save Error!" + System.Environment.NewLine + e.Message + System.Environment.NewLine + e.StackTrace);
            //}
        }
        void SavePackage(string packageFilePath)
        {
            //(ShellService.Documents as ObservableCollection_Sortable<IDocumentViewModel>).Sort(d => {
            //    return d.GetType().GetAttributeValue((OrderAttribute a) => a.Value);
            //});
            //((ShellService)ShellService).DocumentsUnopen.Sort(d => d.NameText);
            var shellService = (ShellService as ShellService);
            if (shellService == null)
            {
                return;
            }
            using (var package = Package.Open(packageFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                //_EventAggregator.GetEvent<PackageSaveEvent>().Publish(package);
                PackageBeforeSaveEvent.SendWith(package);

                //save unopendocuments  
                SaveDocumentsUnopen(package, shellService.DocumentsUnopen);
                foreach (var docU in shellService.DocumentsUnopen)//resave ??
                {
                    if (docU.DocClosedButAlive != null)
                    {
                        var packageLayoutContentViewModel = docU.DocClosedButAlive as IPackageLayoutContentViewModel;
                        if (packageLayoutContentViewModel != null)
                        {
                            SavePackageContent(package, packageLayoutContentViewModel);
                        }
                    }
                }
                //save avalondock panel layout
                SavePanelLayout(package);
                //save documents and tools
                IEnumerable<ILayoutContentViewModel> layoutContentViewModels = ShellService.Documents.Concat<ILayoutContentViewModel>(ShellService.Tools);
                foreach (var layoutContentViewModel in layoutContentViewModels)
                {
                    //Put LayoutContentViewModel To Package
                    var packageLayoutContentViewModel = layoutContentViewModel as IPackageLayoutContentViewModel;
                    if (packageLayoutContentViewModel != null)
                    {
                        SavePackageContent(package, packageLayoutContentViewModel);
                    }
                }
                PackageAfterSaveEvent.SendWith(package);
            }
        }
        void Save_Executed()
        {
            ShellService.Messager.WaitMessage.Show(Resources.SaveMessage);
            GloableStaticInstanse.StopWatch.Restart();
            GloableStaticInstanse.AppStatus = AppStatus.SavingPackage;
            SafeSave();
            GloableStaticInstanse.AppStatus = AppStatus.Running; ;
            GloableStaticInstanse.StopWatch.Stop();
            ShellService.Messager.WaitMessage.Hide();
            Log.Info(Resources.FileSaveTimeFormat, GloableStaticInstanse.StopWatch.ElapsedMilliseconds);
        }
        void SaveAs_Executed()
        {
            var saveFileService = this.GetDependencyResolver().Resolve<ISaveFileService>();
            saveFileService.AddExtension = true;
            saveFileService.CheckFileExists = false;
            saveFileService.CheckPathExists = true;
            saveFileService.Filter = "MetaStudio Package File(*.meta)|*.meta|All (*.*)|*.*";
            saveFileService.FilterIndex = 0;
            saveFileService.InitialDirectory = string.IsNullOrEmpty(CurrentPackagePath) ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) : CurrentPackagePath;
            saveFileService.Title = Resources.SaveFileDialogTitle;
            var bFile = saveFileService.DetermineFile();
            if (bFile)
            {
                if (!string.IsNullOrEmpty(CurrentPackagePath))
                {
                    File.Copy(CurrentPackagePath, saveFileService.FileName);
                }
                else
                {
                    if (File.Exists(saveFileService.FileName))
                    {
                        var back = saveFileService.FileName + ".saveAsBackup";
                        if (File.Exists(back))
                        {
                            File.Delete(back);
                        }
                        File.Move(saveFileService.FileName, back);
                    }
                }
                CurrentPackagePath = saveFileService.FileName;
                try
                {
                    Save_Executed();
                    if (File.Exists(saveFileService.FileName + ".saveAsBackup"))
                    {
                        File.Delete(saveFileService.FileName + ".saveAsBackup");
                    }

                }
                catch (Exception e)
                {
                    if (File.Exists(saveFileService.FileName + ".saveAsBackup"))
                    {
                        File.Move(saveFileService.FileName + ".saveAsBackup", saveFileService.FileName);
                    }
                    throw;
                }


            }
        }

        private void OnDelDocumentCommandExecute(IPackageDocumentViewModel document)
        {
            ShellService.DeleteDocument(document);
        }
        private bool OnDelDocumentCommandCanExecute(IPackageDocumentViewModel document)
        {
            if (string.IsNullOrEmpty(CurrentPackagePath))
            {
                return false;
            }
            return ShellService.CouldDeleteDocument(document);
        }
        private void OnOpenDocumentCommandExecute(IPackageContent document)
        {
            if (document is DocumentUnopen)
            {
                var docUnopen = document as DocumentUnopen;
                if (docUnopen.KeepAliveWhenClose)
                {
                    var documentViewMoel = (IDocumentViewModel)docUnopen.DocClosedButAlive;
                    ShellService.OpenDocument(documentViewMoel);
                    ShellService.ActivateDocument(documentViewMoel);
                    ShellService.DocumentsUnopen.Remove(docUnopen);
                }
                else
                {
                    using (Package package = Package.Open(CurrentPackagePath, FileMode.Open))
                    {
                        var documentViewMoel = OpenPackageContent(package, docUnopen.ContentId) as DocumentViewModel;
                        ShellService.OpenDocument(documentViewMoel);
                        ShellService.ActivateDocument(documentViewMoel);
                        var shellService = (ShellService as ShellService);
                        ShellService.DocumentsUnopen.Remove(docUnopen);
                        SaveDocumentsUnopen(package, ShellService.DocumentsUnopen);
                        SavePanelLayout(package);
                    }
                }
            }
            else
            {
                ShellService.ActivateDocument(document as IDocumentViewModel);
            }

        }

        private bool OnOpenDocumentCommandCanExecute(IPackageContent document)
        {
            if (document is DocumentUnopen)
            {
                var unopenDoc = document as DocumentUnopen;
                if (unopenDoc.KeepAliveWhenClose)
                {
                    return true;
                }
                else
                {
                    if (string.IsNullOrEmpty(CurrentPackagePath))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private async void OnSaveDocumentCommandExecute(IPackageDocumentViewModel document)
        {
            var layoutContentViewModel = document as ILayoutContentViewModel;
            if (layoutContentViewModel != null)
            {
                bool result = await layoutContentViewModel.SaveViewModel();
                if (!result)
                {
                    Log.Info("'" + layoutContentViewModel.Title + "'" + "File Save Error!");
                    return;
                }
                Log.Info("'" + layoutContentViewModel.Title + "'" + Properties.Resources.FileSavedMessage);
                this.ShellService.Messager.Balloon.ShowCustomBalloon(layoutContentViewModel.Title, Properties.Resources.FileSavedMessage, BalloonIcon.Info, PopupAnimation.Fade, 3000);
            }

        }
        private bool OnSaveDocumentCommandCanExecute(IPackageDocumentViewModel document)
        {
            if (string.IsNullOrEmpty(CurrentPackagePath))
            {
                return false;
            }
            if (document != null)
            {
                var d = document as ViewModelBase;
                if (d != null && d.IsDirty)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnSaveCurrentDocumentCommandExecute()
        {
            using (var package = Package.Open(CurrentPackagePath, FileMode.Open))
            {
                SavePackageContent(package, ShellService.ActiveDocument as IPackageLayoutContentViewModel);
            }
            var layoutContentViewModel = ShellService.ActiveDocument;
            Log.Info("'" + layoutContentViewModel.Title + "'" + Properties.Resources.FileSavedMessage);
            this.ShellService.Messager.Balloon.ShowCustomBalloon(layoutContentViewModel.Title, Properties.Resources.FileSavedMessage, BalloonIcon.Info);
        }
        private bool OnSaveCurrentDocumentCommandCanExecute()
        {
            if (string.IsNullOrEmpty(CurrentPackagePath))
            {
                return false;
            }
            if (ShellService.ActiveDocument != null)
            {
                var viewModelBase = (ShellService.ActiveDocument) as ViewModelBase;
                if (viewModelBase != null && viewModelBase.IsDirty)
                {
                    return true;
                }
            }

            return false;
        }
        class OderOpenDocumentItem
        {
            //opened doc part
            public int Num;
            public LayoutDocument LayoutContent;
            public IDocumentViewModel LayoutContentViewModel;
            //alived unopen doc part
            public bool IsUnopenDoc;
            public string UnopenDocContentId;
            public DocumentUnopen DocUnopen;
        }

        readonly List<OderOpenDocumentItem> _oderOpenDocumentItemList = new List<OderOpenDocumentItem>();
        void OpenPackage(string packageFilePath)
        {
            if (!File.Exists(packageFilePath))
            {
                Log.Error("File Not Exist!");
                return;
            }
            _oderOpenDocumentItemList.Clear();
            int oderOpenDocumentItemNum = 0;
            var shellService = ((ShellService)(ShellService));
            using (Package package = Package.Open(packageFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                //_EventAggregator.GetEvent<PackageBeforeOpenEvent>().Publish(package);
                PackageBeforeOpenEvent.SendWith(package);
                //load documents unopen
                var packageRelationShipDocumentsUnopen = package.GetRelationship(Package_DocumentsUnopen_ID);
                var layoutPartDocumentsUnopen = package.GetPart(packageRelationShipDocumentsUnopen.TargetUri);
                var xmlSerilalizer = new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<DocumentUnopen>));
                var unopenDocs = xmlSerilalizer.Deserialize(layoutPartDocumentsUnopen.GetStream()) as ObservableCollection<DocumentUnopen>;
                if (unopenDocs != null)
                {
                    foreach (var item in unopenDocs)
                    {
                        shellService.DocumentsUnopen.Add(item);
                        if (item.KeepAliveWhenClose)
                        {
                            _oderOpenDocumentItemList.Add(new OderOpenDocumentItem()
                            {
                                IsUnopenDoc = true,
                                UnopenDocContentId = item.ContentId,
                                DocUnopen = item
                            });
                        }
                    }
                }
                //load avalondock panel layout
                var layoutRelationShip = package.GetRelationship(Package_PanelLayoutPart_ID);
                var layoutPart = package.GetPart(layoutRelationShip.TargetUri);
                var packageManger = this.GetDependencyResolver().Resolve<DockingManager>();
                var layoutSerializer = new XmlLayoutSerializer(packageManger);
                layoutSerializer.LayoutSerializationCallback += (s, e) =>
                {
                    //Get LayoutContentViewModel From Package
                    if (e.Model is LayoutAnchorable)
                    {
                        var layoutAnchorable = e.Model as LayoutAnchorable;
                        Debug.Assert(package != null, "package != null");
                        var contentViewModel = OpenPackageContent(package, e.Model.ContentId);
                        var toolViewModel = contentViewModel as IToolViewModel;
                        System.Diagnostics.Debug.Assert(toolViewModel != null);
                        e.Content = contentViewModel;
                        if (!ShellService.Tools.Contains_CompareByReference(toolViewModel))
                        {
                            ShellService.Tools.Add(toolViewModel);
                        }
                        toolViewModel.IsSelected = layoutAnchorable.IsSelected;
                        toolViewModel.IsActive = layoutAnchorable.IsActive;
                        toolViewModel.IsVisible = layoutAnchorable.IsVisible;
                        System.Diagnostics.Debug.Assert(ShellService.Documents.Count == 0);//the code of the avalondock behave in this kind of behavior, and the whole application assume the tools are loaded first.
                        return;
                    }
                    else if (e.Model is LayoutDocument)
                    {
                        var layoutDocument = e.Model as LayoutDocument;

                        //var contentViewModel = Services.ShellService.OpenPackageContent(package, e.Model.ContentId) as IDocumentViewModel;
                        //e.Content = contentViewModel;
                        //if (!ShellService.Documents.Contains_CompareByReference(contentViewModel))
                        //{
                        //    ShellService.Documents.Add(contentViewModel);
                        //}
                        //contentViewModel.IsSelected = layoutDocument.IsSelected;
                        //contentViewModel.IsActive = layoutDocument.IsActive;

                        _oderOpenDocumentItemList.Add(new OderOpenDocumentItem() { Num = oderOpenDocumentItemNum, LayoutContent = layoutDocument });
                        oderOpenDocumentItemNum++;
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        return;
                    }

                };

                layoutSerializer.Deserialize(layoutPart.GetStream());
                foreach (var item in _oderOpenDocumentItemList)
                {
                    if (!item.IsUnopenDoc)
                    {
                        //bug fix for avalondock start form 2.1.0, because the close() methord work like 2.0.0
                        //in 2.0.0 close(){
                        //ILayoutRoot root = base.Root;
                        //ILayoutContainer parentAsContainer = base.Parent;
                        //parentAsContainer.RemoveChild(this);
                        //if (root != null)
                        //{
                        //    root.CollectGarbage();
                        //}}
                        //if (item.LayoutContent.Parent != null)
                        //{
                        item.LayoutContent.Parent.RemoveChild(item.LayoutContent);
                        //}
                    }
                }
                //Order Open Document;
                _oderOpenDocumentItemList.Sort((item) =>
                {
                    string packagePartId;
                    string contentId = null;
                    contentId = item.IsUnopenDoc ? item.UnopenDocContentId : item.LayoutContent.ContentId;
                    Type packagePartType = ContentID_ToTypeAndPackagePartID(contentId, out packagePartId);
                    return packagePartType.GetAttributeValue((DocumentOpenOrderAttribute a) => a.Value);
                });

                IDocumentViewModel lastActiveDocument = null;
                foreach (var doc in _oderOpenDocumentItemList)
                {
                    if (doc.IsUnopenDoc)
                    {
                        var contentViewModel = OpenPackageContent(package, doc.UnopenDocContentId);
                        var documentViewModel = contentViewModel as IDocumentViewModel;
                        System.Diagnostics.Debug.Assert(documentViewModel != null);
                        doc.DocUnopen.DocClosedButAlive = documentViewModel;
                        doc.DocUnopen = null;
                    }
                    else
                    {
                        var contentViewModel = OpenPackageContent(package, doc.LayoutContent.ContentId);
                        var documentViewModel = contentViewModel as IDocumentViewModel;
                        System.Diagnostics.Debug.Assert(documentViewModel != null);
                        doc.LayoutContentViewModel = documentViewModel;
                        ShellService.OpenDocument(documentViewModel);
                        if (doc.LayoutContent.IsLastFocusedDocument)
                        {
                            lastActiveDocument = doc.LayoutContentViewModel;
                        }
                    }

                }
                _oderOpenDocumentItemList.RemoveAll(x => x.IsUnopenDoc);
                _oderOpenDocumentItemList.Sort((item) => item.Num);

                for (int i = 0; i < _oderOpenDocumentItemList.Count; i++)
                {
                    int indexInShellDocuments = ShellService.Documents.IndexOf(_oderOpenDocumentItemList[i].LayoutContentViewModel);
                    if (i != indexInShellDocuments)
                    {
                        ShellService.Documents.Move(indexInShellDocuments, i);
                    }
                    _oderOpenDocumentItemList[i].LayoutContentViewModel = null;
                }
                if (lastActiveDocument != null)
                {
                    ShellService.ActivateDocument(lastActiveDocument);
                }
                PackageAfterOpenEvent.SendWith(package);
                _oderOpenDocumentItemList.Clear();
            }
        }


        void OpenMetaFile(string fileName, bool addToRecentFile)
        {
            if (!ShellService.Clear())
            {
                Log.Info("Open File Canceled");
                return;
            }
            //if (!string.IsNullOrEmpty(ShellService.CurrentPackagePath))
            //{
            //    GloableCommands.NewCommand.Execute(null);
            //}

            if (GloableStaticInstanse.AppStatus != AppStatus.StaringAndOpeningPackage)
            {
                GloableStaticInstanse.AppStatus = AppStatus.OpeningPackage;
            }
            bool measureTime = false;
            if (!GloableStaticInstanse.StopWatch.IsRunning)
            {
                GloableStaticInstanse.StopWatch.Restart();
                measureTime = true;
            }
            CurrentPackagePath = fileName;
            string openMessage = Resources.OpenMessage;
            ShellService.Messager.WaitMessage.Show(openMessage);
            //TODO:
            // try
            {
                OpenPackage(fileName);
                if (addToRecentFile)
                {
                    AddToRecentFiles(fileName);
                }
            }

            //catch (Exception e1)
            //{
            //    MessageBox.Show("Could not Open File: " + e1.Message + e1.StackTrace);
            //    ExceptionMessage.ShowExceptionMessage(e1);
            //}
            // finally
            {
                //try
                {
                    ShellService.Messager.WaitMessage.Hide();
                }
                //catch (Exception e)
                //{
                //    MessageBox.Show(e.Message);
                //}

            }
            if (measureTime)
            {
                string message = string.Format(Properties.Resources.FileOpenTimeFormat, GloableStaticInstanse.StopWatch.ElapsedMilliseconds);
                Log.Info(message);
            }
            if (GloableStaticInstanse.AppStatus != AppStatus.StaringAndOpeningPackage)
            {
                GloableStaticInstanse.AppStatus = AppStatus.Running;
            }
        }
        #endregion private methods



        public void Open(string fileName)
        {
            //close all other windows
            foreach (Window win in Application.Current.Windows)
            {
                if (win != Application.Current.MainWindow)
                {
                    win.Close();
                }
            }
            if (!File.Exists(fileName))
            {
                MessageBox.Show(fileName, "The File Is Not Exist!");
                return;
            }
            if (fileName.EndsWith(".meta"))
            {
                OpenMetaFile(fileName, true);
            }
            else
            {
                ShellService.Messager.MessageBox.ShowInformation("Please Make Sure The Extension Of Input File Is .meta");
                return;
            }

        }

        #endregion
        string _currentPackagePath;
        public string CurrentPackagePath
        {
            get { return _currentPackagePath; }
            set
            {
                if (value != _currentPackagePath)
                {
                    _currentPackagePath = value;
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                    RaisePropertyChanged(() => this.CurrentPackagePath);
                }
            }
        }

        static internal void SaveDocumentsUnopen(Package package, Object documentsUnopen)
        {
            Uri partUriDocumentsUnopen = PackUriHelper.CreatePartUri(new Uri(Package_DocumentsUnopen_Path, UriKind.Relative));
            if (package.PartExists(partUriDocumentsUnopen))
            {
                package.DeletePart(partUriDocumentsUnopen);
            }
            PackagePart packagePartDocumentsUnopen = package.CreatePart(partUriDocumentsUnopen, MediaTypeNames.Text.Xml, CompressionOption.Normal);
            if (package.RelationshipExists(Package_DocumentsUnopen_ID))
            {
                package.DeleteRelationship(Package_DocumentsUnopen_ID);
            }
            if (packagePartDocumentsUnopen == null)
            {
                return;
            }
            package.CreateRelationship(packagePartDocumentsUnopen.Uri, TargetMode.Internal, "DocumentsUnopen", Package_DocumentsUnopen_ID);
            var xmlSerilalizer = new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<DocumentUnopen>));
            xmlSerilalizer.Serialize(packagePartDocumentsUnopen.GetStream(), documentsUnopen);
        }

        static internal void SavePanelLayout(Package package)
        {
            Uri partUriDocument = PackUriHelper.CreatePartUri(new Uri(Package_PanelLayoutPart_Path, UriKind.Relative));
            if (package.PartExists(partUriDocument))
            {
                package.DeletePart(partUriDocument);
            }
            PackagePart packagePartPanelLayout = package.CreatePart(partUriDocument, MediaTypeNames.Text.Xml, CompressionOption.Normal);
            if (package.RelationshipExists(Package_PanelLayoutPart_ID))
            {
                package.DeleteRelationship(Package_PanelLayoutPart_ID);
            }
            if (packagePartPanelLayout == null)
            {
                return;
            }
            package.CreateRelationship(packagePartPanelLayout.Uri, TargetMode.Internal, "AvalonDockPanelLayout", Package_PanelLayoutPart_ID);
            var packageManger = package.GetDependencyResolver().Resolve<DockingManager>();
            var layoutSerializer = new XmlLayoutSerializer(packageManger);
            layoutSerializer.Serialize(packagePartPanelLayout.GetStream());
        }
        #region Static Methords
        /// <summary>
        /// assembly qualified type name
        /// </summary>
        static public string PackagePartType(ILayoutContentViewModel contentViewModel)
        {
            var type = contentViewModel.GetType();
            return type.FullName + "," + type.Assembly.GetName().Name;
        }
        /// <summary>
        /// used by packagerelationship id of the package part.
        /// should not contains parts that maybe changed
        /// shold not contains "-_-"
        /// So the lenth is 38=2+36
        /// string packagePartID = contentID_Splits[1].Substring(0,38);
        /// </summary>
        static public string PackagePartID(ILayoutContentViewModel contentViewModel)
        {
            return "ID" + contentViewModel.ID.ToString();
        }

        static public string ContentId(ILayoutContentViewModel contentViewModel)
        {
            var sb = new StringBuilder();
            sb.Append(PackagePartType(contentViewModel));//used to get the type of this content
            sb.Append("-_-");
            sb.Append(PackagePartID(contentViewModel));//used to get the stream from package
            return sb.ToString();
        }
        /// <summary>
        /// the name of the file in package
        /// "/" + pluginAssembleName + "/" + ILayoutContentViewModel.PackagePartName + ".xml";
        /// should not contains changable part, because it is the url of the packagepart, if not we will get a lot dead packagepart in the package when we change the title and save the package part;
        /// </summary>
        static public string PackagePartName(ILayoutContentViewModel contentViewModel)
        {
            return contentViewModel.GetType().FullName + "-_-" + PackagePartID(contentViewModel);//this.GetType().FullName + "-_-" + Title + "-_-" + PackagePartID;
        }
        internal static void SetContentViewModelID(string contentId, ILayoutContentViewModel contentViewModel)
        {
            var contentIdSplits = contentId.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            System.Diagnostics.Debug.Assert(contentIdSplits.Length == 2);
            var layoutContentViewModel = contentViewModel as LayoutContentViewModel;
            if (layoutContentViewModel == null)
            {
                return;
            }
            layoutContentViewModel.ID = new Guid(contentIdSplits[1].Substring(2, 38));
        }
        internal static Type ContentID_ToTypeAndPackagePartID(string contentId, out string packagePartId)
        {
            var contentIdSplits = contentId.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            System.Diagnostics.Debug.Assert(contentIdSplits.Length == 2);
            packagePartId = contentIdSplits[1].Substring(0, 38);
            string packagePartTypeString = contentIdSplits[0];
            Type packagePartType = Type.GetType(packagePartTypeString);
            return packagePartType;
        }
        internal static ILayoutContentViewModel OpenPackageContent(string packageFilePath, string contentId)
        {
            ILayoutContentViewModel contentViewModel;
            using (Package package = Package.Open(packageFilePath, FileMode.Open))
            {
                contentViewModel = OpenPackageContent(package, contentId);
            }
            return contentViewModel;
        }
        internal static ILayoutContentViewModel OpenPackageContent(Package package, string contentId)
        {
            string packagePartId;
            Type packagePartType = ContentID_ToTypeAndPackagePartID(contentId, out packagePartId);
            var packageRelationshipOfContentViewModel = package.GetRelationship(packagePartId);
            var packagePartOfContentViewModel = package.GetPart(packageRelationshipOfContentViewModel.TargetUri);
            //ILayoutContentViewModel contentViewModel = (ILayoutContentViewModel)typeof(ModelBase).GetMethod("Load", new Type[] { typeof(Stream), typeof(SerializationMode), typeof(bool) }).MakeGenericMethod(packagePartType).Invoke(null, new object[] { packagePartOfContentViewModel.GetStream(), SerializationMode.Xml, false });
            var con = ModelBase.Load(packagePartType, packagePartOfContentViewModel.GetStream(), SerializationMode.Xml);
            var contentViewModel = con as ILayoutContentViewModel;
            if (contentViewModel == null)
            {
                return null;
            }
            contentViewModel.ContentId = contentId;
            return contentViewModel;
        }
        internal static void DeletePackagePart(Package package, string packagePartId)
        {
            var packageRelationshipOfContentViewModel = package.GetRelationship(packagePartId);
            package.DeletePart(packageRelationshipOfContentViewModel.TargetUri);
            package.DeleteRelationship(packagePartId);
        }
        internal static void DeletePackageContent(Package package, string contentId)
        {
            string packagePartId;
            Type packagePartType = ContentID_ToTypeAndPackagePartID(contentId, out packagePartId);
            DeletePackagePart(package, packagePartId);
        }
        internal static bool IsContentExist(Package package, string packagePartId)
        {
            return package.RelationshipExists(packagePartId);
        }
        public bool SavePackageContent(IPackageLayoutContentViewModel layoutContentViewModel)
        {
            try
            {
                using (Package package = Package.Open(CurrentPackagePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    SavePackageContent(package, layoutContentViewModel);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        internal static void SavePackageContent(Package package, IPackageLayoutContentViewModel layoutContentViewModel)
        {
            var pluginAssembleName = layoutContentViewModel.GetType().Assembly.GetName().Name;
            var layoutContentViewModelPackagePartPath = "/" + pluginAssembleName + "/" + layoutContentViewModel.PackagePartName + ".xml";
            Uri partUriLayoutContentViewModel = PackUriHelper.CreatePartUri(new Uri(layoutContentViewModelPackagePartPath, UriKind.Relative));
            if (package.PartExists(partUriLayoutContentViewModel))
            {
                package.DeletePart(partUriLayoutContentViewModel);
            }
            PackagePart packagePartLayoutContentViewModel = package.CreatePart(partUriLayoutContentViewModel, MediaTypeNames.Text.Xml, CompressionOption.Normal);
            if (packagePartLayoutContentViewModel == null)
            {
                return;
            }
            if (package.RelationshipExists(layoutContentViewModel.PackagePartID))
            {
                package.DeleteRelationship(layoutContentViewModel.PackagePartID);
            }
            package.CreateRelationship(partUriLayoutContentViewModel, TargetMode.Internal, layoutContentViewModel.PackagePartType, layoutContentViewModel.PackagePartID);
            layoutContentViewModel.SaveState(packagePartLayoutContentViewModel.GetStream());

        }
        #endregion Static Methords
    }
}
