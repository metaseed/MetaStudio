using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace Metaseed.Data
{
    [Serializable]
    public class NameDescription_INotifyPropertyChanged : INotifyPropertyChanged, INameDescription
    {
        private string _Description = String.Empty;
        private string _NameText = String.Empty;

        public virtual XElement XML
        {
            get
            {
                var x = new XElement(GetType().Name,
                    new XElement("Name", NameText),
                    new XElement("Description", Description));
                return x;
            }
            set
            {
                XElement sigXml = value;
                NameText = sigXml.Element("Name").Value;
                Description = sigXml.Element("Description").Value;
            }
        }

        public virtual string NameText
        {
            get { return _NameText; }
            set
            {
                if (value == _NameText)
                    return;

                _NameText = value;
                RaisePropertyChanged("NameText");
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                if (value == _Description)
                    return;

                _Description = value;
                RaisePropertyChanged("Description");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}