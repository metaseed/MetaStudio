using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.__
{
    class usage
    {
        void usage() {
            var key = File.ReadAllBytes(Helper.AppPath + "\\app.ico");
            string keyp = Convert.ToBase64String(key);
            System.Security.SecureString ss = A.Ds(Helper.AppPath + "\\License.dat", keyp);
            if (!new C().aaa(ss)) { Application.Current.Shutdown(); }
        }
    }
}
