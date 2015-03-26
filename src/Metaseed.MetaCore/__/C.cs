using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.IsolatedStorage;
using Metaseed.Alogrithm;
namespace Metaseed._
{
    public class C
    {
        static SecureString plugin;
        static public string GetPlugInData()
        {
            unsafe
            {
                if (plugin != null)
                {
                    Char* pc = (Char*)Marshal.SecureStringToCoTaskMemUnicode(plugin);
                    //int dc = 0;
                    //for (Int32 index = 0; pc[index] != 0; index++)
                    //{
                    //    dc++;
                    //}
                    string s = new string(pc);
                    return s;
                }
                return string.Empty;
            }

        }
        /// <summary>
        /// verify checksum
        /// </summary>
        /// <param name="cs"></param>
        unsafe private static void CheckS(SecureString cs)
        {
            Char* pc = null;
            try
            {
                // Decrypt the SecureString into an unmanaged memory buffer
                pc = (Char*)Marshal.SecureStringToCoTaskMemUnicode(cs);
                // Access the unmanaged memory buffer that
                // contains the decrypted SecureString
                UInt16 checkSum = 0;
                for (Int32 index = 0; pc[index] != 0; index++)
                {
                    //Console.Write(pc[index]);
                    checkSum += *(Byte*)(&(pc[index]));
                    checkSum += *((Byte*)(&(pc[index])) + 1);
                    if (pc[index] == '@')
                    {
                        UInt16 checkSumValue = *(UInt16*)&(pc[index + 1]);
                        //if (checkSum != checkSumValue)
                        //{
                        //Console.WriteLine("Checksum not equal!!!!!!!!");
                        //}
                    }
                }


            }
            finally
            {
                // Make sure we zero and free the unmanaged memory buffer that contains
                // the decrypted SecureString characters
                if (pc != null)
                    Marshal.ZeroFreeCoTaskMemUnicode((IntPtr)pc);
            }
        }
        //crc
        unsafe private static void CheckC(SecureString cs)
        {
            Char* pc = null; List<Byte> data = new List<byte>();
            try
            {
                // Decrypt the SecureString into an unmanaged memory buffer
                pc = (Char*)Marshal.SecureStringToCoTaskMemUnicode(cs);
                // Access the unmanaged memory buffer that
                // contains the decrypted SecureString
                UInt16 checkSum = 0;
                UInt16 checkSumValue = 1;
                for (Int32 index = 0; pc[index] != 0; index++)
                {
                    //Console.Write(pc[index]); 
                    data.Add(*(Byte*)(&(pc[index])));
                    data.Add(*((Byte*)(&(pc[index])) + 1));
                    if (pc[index] == '@')
                    {
                        checkSumValue = *(UInt16*)&(pc[index + 1]);
                    }
                }
                checkSum = CRC16.GetCRC16(data, 0, data.Count - 2);
                //if (checkSum!=checkSumValue)
                //{
                //   Console.WriteLine("Checksum not equal!!!!!!!!");
                //}
            }
            finally
            {
                data.Clear();
                // Make sure we zero and free the unmanaged memory buffer that contains
                // the decrypted SecureString characters
                if (pc != null)
                    Marshal.ZeroFreeCoTaskMemUnicode((IntPtr)pc);
            }
        }
        unsafe public static void Parse(SecureString cs, ref int _t, ref int ct, ref string pluinStr, ref DateTime dt)
        {
            Char* pc = null;
            try
            {
                pc = (Char*)Marshal.SecureStringToCoTaskMemUnicode(cs);
                int dc = 0; int startIndex = 0; int dateIndex = 0; int d0In = 0; int d1In = 0;
                for (Int32 index = 0; pc[index] != 0; index++)
                {
                    if (pc[index] == '^')
                    {
                        startIndex = index + 1;
                    }
                    if (dc == 1)
                    {
                        if (pc[index] == '[')
                        {
                            dateIndex = index + 1;
                            string _ts = new string(pc, d0In + 1, index - (d0In + 1));
                            _t = int.Parse(_ts);
                        }
                        if (pc[index] == ']')
                        {
                            string dates = new string(pc, dateIndex, index - dateIndex);
                            string[] sss = dates.Split('-');
                            var mm = int.Parse(sss[0]);
                            var da = int.Parse(sss[1]);
                            var yy = int.Parse(sss[2]);
                           if(dt!=null) dt = new DateTime(yy, mm, da);
                            //var f = e - DateTime.Now;

                            //if (f.Ticks <= 0)
                            //{
                            //    return 0;
                            //}
                        }
                    }
                    if ((pc[index] == '$'))
                    {
                        if (dc == 0)
                        {
                            d0In = index;
                            string _ts = new string(pc, startIndex, index - startIndex);
                            ct = int.Parse(_ts);

                        }
                        if (dc == 1)
                        {
                            d1In = index;
                        }
                        dc++;
                    }
                    if (pc[index] == '@')
                    {
                        string _ts = new string(pc, (d1In + 1), index - (d1In + 1));
                        plugin = new SecureString(&pc[d1In + 1], index - (d1In + 1));
                        if(pluinStr!=null) pluinStr = _ts;
                    }

                }
            }
            finally
            {
                if (pc != null)
                    Marshal.ZeroFreeCoTaskMemUnicode((IntPtr)pc);
            }

        }
        private static bool haveChecked = false;
        public unsafe bool aaa(SecureString cs)
        {
            int _t = 1;//run times
            DateTime _d = new DateTime(2012, 3, 21);//date end
            int ct = 1;//change time
            string pluinStr = string.Empty;

            Parse(cs, ref _t, ref ct, ref pluinStr, ref _d);
            int runtimeNow = 0;
            // Get a new isolated store for this user, domain, and assembly.
            // Put the store into an IsolatedStorageFile object.

            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
                IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
            DateTime lastrun=DateTime.Now;
            if (isoStore.FileExists("SE/" + ct.ToString() + "/se.dat"))
            {
                using (IsolatedStorageFileStream isoStream1 = new IsolatedStorageFileStream("SE/" + ct.ToString() + "/se.dat", 
                    FileMode.Open, isoStore))
                {
                    //Console.WriteLine("Open a se file in the IS.");
                    //var sr = new StreamReader(isoStream1);
                    //string runtimesOld = sr.ReadLine();
                    //runtimeNow = int.Parse(runtimesOld);
                    //var dateOld = sr.ReadLine();
                    //lastrun=DateTime.Parse(dateOld);
                    var br = new BinaryReader(isoStream1);
                    runtimeNow=br.ReadInt32();
                    lastrun = new DateTime(br.ReadInt64());
                }
            }
            else
            {
                isoStore.CreateDirectory("SE/" + ct.ToString());
            }
            if (lastrun-DateTime.Now>new TimeSpan(8,0,0))
            {
                System.Windows.MessageBox.Show("请不要修改系统的时间!/r/nPlease Don't Modify The System Time!");
                return false;
            }
            //////////////////////////////////////////////////////////////////////////////////////
            var ds=(_d-System.DateTime.Now).Days;
            bool showsDays = false;
            if ((ds<=3)&&(ds>0))
            {
                var str = String.Format("软件授权将在{0}天内到期!!\r\n为了不影响您的生产进度，请尽快联系肖工索取新的授权！\r\n或直接联系Metasong QQ:63695325", ds);
                System.Windows.MessageBox.Show(str);
                System.Windows.MessageBox.Show(str);
                showsDays = true;
            }
            if (showsDays==false)
            {
                var times=(_t - runtimeNow);
                if ((times <= 16)&&(times >1))
                {
                    var str = String.Format("软件授权即将到期，只能再打开{0}次！\r\n为了不影响您的生产进度，请尽快升级软件，或联系肖工索取新的授权！\r\n或直接联系Metasong QQ:63695325", times - 1);
                    System.Windows.MessageBox.Show(str);
                    System.Windows.MessageBox.Show(str);
                }
            }
            
            ////////////////////////////////////////////////////////////////////////////////////////
            if (System.DateTime.Now > _d || runtimeNow > _t)
            {
                //KillFolder();
                string msg = "软件已到期，请联系开发者！\r\n或登录metaseed.com,进入下载中心，下载包含最新授权的CANOpenStudio软件，并解压后双击安装。\r\nThe Software Is Out Of Date,Please Contect With Metaseed!\r\nEmail:info@metaseed.com;QQ:63695325";
                System.Windows.MessageBox.Show(msg);
                System.Windows.MessageBox.Show(msg);
                return false;
                // KillExe();
            }

            using (IsolatedStorageFileStream isoStream2 = new IsolatedStorageFileStream("SE/" + ct.ToString() + "/se.dat", 
                FileMode.Create, isoStore))
            {
                //Console.WriteLine("Created a se file in the IS.");
                //StreamWriter sw = new StreamWriter(isoStream2);
                //sw.WriteLine(runtimeNow + 1);
                //sw.WriteLine(DateTime.Now.ToLocalTime());
                //sw.Flush();
                BinaryWriter bw = new BinaryWriter(isoStream2);
                if (haveChecked)
                {
                    bw.Write(runtimeNow);
                }
                else
                {
                    bw.Write(runtimeNow + 1);
                }
                bw.Write(DateTime.Now.Ticks);
                bw.Flush();
            }
            haveChecked = true;
            return true;
        }
    }
}
