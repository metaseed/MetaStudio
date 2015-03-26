using Microsoft.Practices.Prism.Commands;
namespace Metaseed.MetaShell.Infrastructure
{
    /// <summary>
    /// the globle commands of the MetaStudio
    /// </summary>
    public class GloableCommands
    {
        static CompositeCommand _new = new CompositeCommand();
        public static CompositeCommand NewCommand
        {
            get
            {
                return _new;
            }
            set
            {
                _new = value;
            }
        }
        static CompositeCommand _open = new CompositeCommand();
        public static CompositeCommand OpenCommand
        {
            get
            {
                return _open;
            }
            set
            {
                _open = value;
            }
        }
        static CompositeCommand _save = new CompositeCommand();
        public static CompositeCommand SaveCommand
        {
            get
            {
                return _save;
            }
            set
            {
                _save = value;
            }
        }
        static CompositeCommand _saveAs = new CompositeCommand();
        public static CompositeCommand SaveAsCommand
        {
            get
            {
                return _saveAs;
            }
            set
            {
                _saveAs = value;
            }
        }
    }
}
