using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace Metaseed.Data
{
    public interface INameDescription
    {
        string NameText { get; }
        string Description { get; }
    }
}
