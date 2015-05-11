namespace Metaseed.Views
{
    public interface IContextUI
    {
        void Initialize();
        bool HasInitialized { get; }
        void Show(object objectWithContext);
        void Hide(object objectWithContext);
        //todo
        //void Close();
    }
}
