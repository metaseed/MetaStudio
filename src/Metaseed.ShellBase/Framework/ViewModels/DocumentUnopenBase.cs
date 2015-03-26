using System;
using   System.Xml.Serialization;
namespace Metaseed.MetaShell.ViewModels
{
    using Metaseed.Data;
    using Properties;
    using Infrastructure;
    [Serializable]
    public class DocumentUnopenBase : NameDescription_INotifyPropertyChanged
    {
        public DocumentUnopenBase()
        {
            PropertyChanged += DocumentUnopen_PropertyChanged;
            CurrentCultureChangedEvent.Register(this, (e) => RaisePropertyChanged("PackgeContentType"));
        }
        void DocumentUnopen_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("NameText"))
            {
                RaisePropertyChanged("Title");
            }
        }
        public string ContentId { get; set; }
        public string Title { get { return NameText; } }
        public bool KeepAliveWhenClose { get; set; }
        /// <summary>
        /// null: this doc is closed and killed
        /// </summary>
        [XmlIgnore]
        public ILayoutContentViewModel DocClosedButAlive { get; set; }
    }
}
