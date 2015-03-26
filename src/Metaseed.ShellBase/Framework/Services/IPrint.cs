using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Imaging;
namespace Metaseed.MetaShell.Services
{
    public interface IPrint
    {
        string Title { get;}
        void PrintPage();
        void SaveAsPNG();
        RenderTargetBitmap GetInvertBitmap(bool toInvert);
    }
}
