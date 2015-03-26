using System.ComponentModel;
using Catel.MVVM;

namespace Metaseed.MetaShell.ViewModels
{
    using Views;
    /// <summary>
    /// Basic Class Of All ViewModel Class In MetaStudio
    /// </summary>
    public abstract class MetaViewModel : ViewModelBase
    {
        #region Fields
        //static ILog Log= LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaViewModel"/> class.
        /// </summary>
        public MetaViewModel()
            : base(true, true, false)//ignore ignoreMultipleModelsWarning
        {
           //var kk = this.HasErrors;
        }
        
        #endregion

        #region Properties
        //for the bug in catel 4.0
        //new public bool HasErrors
        //{
        //    get
        //    {
        //        return false; // ((INotifyDataErrorInfo)this).HasErrors;|| this.ChildViewModelsHaveErrors;
        //    }
        //}
 
        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        #endregion

        #region Commands
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        #endregion

        #region Methods
        /// <summary>
        /// only called when View is derived from MetaView
        /// </summary>
        /// <param name="view"></param>
        internal protected virtual void OnViewLoaded(MetaView view)
        {

        }
        //    public static object Load(Type type, Stream stream, SerializationMode mode, bool enableRedirects = false)
        //    {
        //        object obj = null;
        //        Log.Debug("Loading object '{0}' as '{1}'", new object[]
        //{
        //    type.Name,
        //    mode
        //});
        //        switch (mode)
        //        {
        //            case SerializationMode.Binary:
        //                try
        //                {
        //                    IBinarySerializer binarySerializer = SerializationFactory.GetBinarySerializer();
        //                    obj = binarySerializer.Deserialize(type, stream);
        //                    goto IL_B4;
        //                }
        //                catch (Exception exception)
        //                {
        //                    Log.Error(exception, "Failed to deserialize the binary object", new object[0]);
        //                    goto IL_B4;
        //                }
        //                break;
        //            case SerializationMode.Xml:
        //                break;
        //            default:
        //                goto IL_B4;
        //        }
        //        try
        //        {
        //            IXmlSerializer xmlSerializer = SerializationFactory.GetXmlSerializer();
        //            obj = xmlSerializer.Deserialize(type, stream);
        //        }
        //        catch (Exception exception2)
        //        {
        //            Log.Error(exception2, "Failed to deserialize the binary object", new object[0]);
        //        }
        //    IL_B4:
        //        Log.Debug("Loaded object", new object[0]);
        //        ModelBase modelBase = obj as ModelBase;
        //        if (modelBase != null)
        //        {
        //            //modelBase.Mode = mode;
        //        }
        //        return ((object)obj);
        //    }
        #endregion
    }
}