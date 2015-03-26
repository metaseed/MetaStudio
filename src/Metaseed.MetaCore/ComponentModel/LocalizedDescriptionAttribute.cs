using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace Metaseed.ComponentModel
{
    using Metaseed;
   public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _DescriptionResourceKey;
        /// <summary>
        /// assemblyName:resourceName:key
        /// </summary>
        /// <param name="nameResourceKey"> assemblyName:resourceName:key</param>
        public LocalizedDescriptionAttribute(string nameResourceKey)
            : base()
        {
            this._DescriptionResourceKey = nameResourceKey;
        }

        public override string Description
        {
            get
            {
                return LocalizeHelper.Get<string>(this._DescriptionResourceKey);
            }
        }
    }
}
