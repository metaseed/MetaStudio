using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
namespace Metaseed.ComponentModel
{
    using Metaseed;
    using Reflection;
    public  class LocalizedCategoryAttribute : CategoryAttribute
    {
        private readonly string _ResourceKey;
        /// <summary>
        /// assemblyName:resourceName:key
        /// </summary>
        /// <param name="nameResourceKey"> assemblyName:resourceName:key</param>
        public LocalizedCategoryAttribute(string categoryResourceKey)
            : base(categoryResourceKey)
        {
            _ResourceKey = categoryResourceKey;
        }
       
        //new public string Category
        //{
        //    get
        //    {
        //        return LocalizeHelper.Get<string>(_ResourceKey);
        //    }
        //}
        protected override string GetLocalizedString(string value)
        {
            this.SetNonPublicField("localized", false);
            return LocalizeHelper.Get<string>(value);
            //var myField = typeof(LocalizedCategoryAttribute).BaseType.GetField("localized", BindingFlags.Instance | BindingFlags.NonPublic);
           // myField.SetValue false);
            
        }
        //static volatile LocalizedCategoryAttribute _Information;
        
        //static public LocalizedCategoryAttribute Information
        //{
        //    get
        //    {
        //        if (_Information==null)
        //        {
        //            _Information=new LocalizedCategoryAttribute()
        //        }
        //        return _Information;
        //    }
        //}

    }
}
