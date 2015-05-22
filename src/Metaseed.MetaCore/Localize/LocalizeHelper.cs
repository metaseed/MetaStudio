using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catel.Logging;
using WPFLocalizeExtension.Extensions;
using System.Windows;
using System.Windows.Controls;
namespace Metaseed
{
    //Event on CultureChange
    //http://wpflocalizeextension.codeplex.com/workitem/6870
    public static class LocalizeHelper
    {
        static readonly ILog Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// assemblyName + ":" + resourceName + ":" + key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        static public T Get<T>(string resourceKey)
        {
            T uiString;
            var locExtension = new LocExtension(resourceKey);
            locExtension.ResolveLocalizedValue<T>(out uiString);
            return uiString;
        }
        static public T Get<T>(string assemblyName, string resourceName, string key)
        {
            T uiString;
            var locExtension = new LocExtension(assemblyName + ":" + resourceName + ":" + key);
            locExtension.ResolveLocalizedValue<T>(out uiString);
            return uiString;
        }
        //https://github.com/SeriousM/WPFLocalizationExtension/blob/master/Tests/XamlLocalizationTest/Window1.xaml.cs#L50
        static public bool BindTo(DependencyObject objectToBind, DependencyProperty propertyToBind, string assemblyName, string resourceName, string key)
        {
            var loc = new LocExtension(assemblyName + ":" + resourceName + ":" + key);
            return loc.SetBinding(objectToBind, propertyToBind);
        }
        static public bool BindTo(DependencyObject objectToBind, DependencyProperty propertyToBind, string resourceKey)
        {
            if (resourceKey == null) return false;
            if (resourceKey.Count(c => c.Equals(':')) != 2)
            {
                Log.Info("localized resource key: "+resourceKey+" for object:"+objectToBind.ToString()+"; propety:"+propertyToBind.ToString()+" is not parsed, it is used directly."+"(Valid key format is assemblyName:resourceFileName:resourceKeyName)");
                objectToBind.SetValue(propertyToBind,resourceKey);
                return false;
            }
            objectToBind.ClearValue(propertyToBind);
            var loc = new LocExtension(resourceKey);
            return loc.SetBinding(objectToBind, propertyToBind);
        }
        static public bool BindToLoc(this DependencyObject objectToBind, DependencyProperty propertyToBind, string assemblyName, string resourceName, string key)
        {
            return BindTo(objectToBind, propertyToBind, assemblyName, resourceName, key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectToBind"></param>
        /// <param name="propertyToBind"></param>
        /// <param name="resourceKey">assemblyName:resourceName:key</param>
        /// <returns></returns>
        static public bool BindToLoc(this DependencyObject objectToBind, DependencyProperty propertyToBind, string resourceKey)
        {
            return BindTo(objectToBind, propertyToBind, resourceKey);
        }
    }
}
