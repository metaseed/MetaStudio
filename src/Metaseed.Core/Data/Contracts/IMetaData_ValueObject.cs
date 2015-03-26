using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.Data.Contracts
{
    using Metaseed.Data;
    public interface IMetaData_ValueObject : IValueObject, IMetaData
    {
        IEnumerable<IMetaData_ValueObject> Children { get; }
    }
    
}
