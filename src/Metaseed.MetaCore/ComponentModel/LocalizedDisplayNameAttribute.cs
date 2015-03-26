using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace Metaseed.ComponentModel
{
    using Metaseed;
   public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string _NameResourceKey;
        /// <summary>
        /// assemblyName:resourceName:key
        /// </summary>
        /// <param name="nameResourceKey"> assemblyName:resourceName:key</param>
        public LocalizedDisplayNameAttribute(string nameResourceKey)
            : base()
        {
            this._NameResourceKey = nameResourceKey;
        }

        public override string DisplayName
        {
            get
            {
                return LocalizeHelper.Get<string>(this._NameResourceKey);
            }
        }
    }
}
