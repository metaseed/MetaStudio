/*
* Copyright (c) 2006, Brendan Grant (grantb@dahat.com)
* All rights reserved.
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*
*     * All original and modified versions of this source code must include the
*       above copyright notice, this list of conditions and the following
*       disclaimer.
*     * This code may not be used with or within any modules or code that is 
*       licensed in any way that that compels or requires users or modifiers
*       to release their source code or changes as a requirement for
*       the use, modification or distribution of binary, object or source code
*       based on the licensed source code. (ex: Cannot be used with GPL code.)
*     * The name of Brendan Grant may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY BRENDAN GRANT ``AS IS'' AND ANY EXPRESS OR
* IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
* OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
* EVENT SHALL BRENDAN GRANT BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; 
* OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
* WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
* OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
* ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using Metaseed.Win32;
using System.Security.Permissions;
using System.Diagnostics;
namespace Metaseed.Win32.FileAssociation
{
    /// <summary>
    /// Provides more streamlined interface for associating a single or multiple extensions with a single program.
    /// </summary>
    public class FileAssociationManager
    {
        /// <summary>
        /// Determines of the list of extensions are associated with the specified program id.
        /// </summary>
        /// <param name="progId">Program id to check against.</param>
        /// <param name="extensions">String array of extensions to check against the program id.</param>
        /// <returns>String array of extensions that were not associated with the program id.</returns>
        public string[] CheckAssociation(string progId, params string[] extensions)
        {
            List<string> notAssociated = new List<string>();

            foreach (string s in extensions)
            {
                FileAssociationInfo fai = new FileAssociationInfo(s);

                if (!fai.Exists || fai.ProgID != progId)
                    notAssociated.Add(s);
            }

            return notAssociated.ToArray();

        }

        /// <summary>
        /// Associates a single executable with a list of extensions.
        /// </summary>
        /// <param name="progId">Name of program id</param>
        /// <param name="executablePath">Path to executable to start including arguments.</param>
        /// <param name="extensions">String array of extensions to associate with program id.</param>
        /// <example>progId = "MyTextFile"
        /// executablePath = "notepad.exe"
        /// extensions = ".txt", ".text"</example>
        static public void Associate(string progId, string executablePath, string description, params string[] extensions)
        {

            foreach (string s in extensions)
            {
                FileAssociationInfo fai = new FileAssociationInfo(s);

                if (!fai.Exists)
                    fai.Create(progId);

                fai.ProgID = progId;
                string exeName = executablePath.Substring(executablePath.LastIndexOf('\\') + 1);
                fai.OpenWithList = new string[] { exeName };
                fai.OpenWithProgids = new string[] { progId };
                OpenWithListProcess(s, progId, exeName);
            }

            ProgramAssociationInfo pai = new ProgramAssociationInfo(progId);
            pai.Create(description, EditFlags.OpenIsSafe | EditFlags.AlwaysShowExtension,
                new ProgramVerb("open", "\"" + executablePath + "\"" + " \"%1\""));
            // pai.InfoTip="prop:FileDescription;Company;FileVersion;Create;Size";
            //pai.TileInfo = "prop:FileDescription;Company;FileVersion;Create;Size";
            pai.AlwaysShowExtension = true;
            pai.DefaultIcon = new ProgramIcon(executablePath, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension">.cocc</param>
        /// <param name="progID">CANOpenStackChip</param>
        /// <param name="exeName">CANOpenStackChip.exe</param>
        static public void OpenWithListProcess(string extension, string progID, string exeName)
        {
            string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + extension; // +@"\OpenWithList";
            RegistryKey key = Registry.CurrentUser;
            //RegistryKey lastKey = key;
            string[] parts = keyPath.Split('\\');
            if (parts == null || parts.Length == 0)
            {
                return;
            }

            for (int x = 0; x < parts.Length; x++)
            {
                key = key.OpenSubKey(parts[x], true);

                if (key == null)
                {
                    return;
                    //key = lastKey.CreateSubKey(parts[x]);
                }
                //lastKey = key;
            }
            key.SetValue("Application", exeName, RegistryValueKind.String);
            RegistryKey key0 = key.OpenSubKey("OpenWithList", true);
            if (key0 == null)
            {
                return;
            }
            string v = (string)(key0.GetValue("MRUList", null));
            if (v == null)
            {
                return;
            }
            string[] names = key0.GetValueNames();
            string name = null;
            for (int i = 0; i < key0.ValueCount; i++)
            {
                if (((string)key0.GetValue(names[i])).Equals(exeName))
                {
                    name = names[i];
                    break;
                }
            }
            Debug.Assert(v.Contains(name));
            StringBuilder sb = new StringBuilder(name);
            foreach (var item in names)
            {
                if (item != name && item != "MRUList")
                {
                    sb.Append(item);
                }
            }
            key0.SetValue("MRUList", sb.ToString(), RegistryValueKind.String);
            RegistryKey key1 = key.OpenSubKey("OpenWithProgids", true);
            if (key0 == null)
            {
                return;
            }
            key1.SetValue(progID, new Byte[0], RegistryValueKind.Binary);
        }
        /// <summary>
        /// Associates an already existing program id with a list of extensions.
        /// </summary>
        /// <param name="progId">The program id to associate extensions with.</param>
        /// <param name="extensions">String array of extensions to associate with program id.</param>
        public void Associate(string progId, params string[] extensions)
        {
            foreach (string s in extensions)
            {
                FileAssociationInfo fai = new FileAssociationInfo(s);

                if (!fai.Exists)
                    fai.Create(progId);

                fai.ProgID = progId;
            }
        }

    }
}
