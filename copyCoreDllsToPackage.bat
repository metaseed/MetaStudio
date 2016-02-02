d:
cd D:\JianzhongSong\Projects\Metaseed.MetaStudio\output\Debug

set pakckagePath=D:\JianzhongSong\Projects\Metaseed.MetaStudio\packages\
set MetaseedCore=Metaseed.Core.2.2\lib\net451
set MetaseedMetaCore=Metaseed.MetaCore.2.2\lib\net451
set MetaseedShellBase=Metaseed.ShellBase.2.2\lib\net451
xcopy /Y  Metaseed.Core.dll  %pakckagePath%%MetaseedCore%
xcopy /Y  Metaseed.Core.pdb  %pakckagePath%%MetaseedCore%
xcopy /Y  Metaseed.Core.xml  %pakckagePath%%MetaseedCore%
xcopy /Y  Metaseed.MetaCore.dll  %pakckagePath%%MetaseedMetaCore%
xcopy /Y  Metaseed.MetaCore.pdb  %pakckagePath%%MetaseedMetaCore%
xcopy /Y  Metaseed.MetaCore.xml  %pakckagePath%%MetaseedMetaCore%
xcopy /Y  Metaseed.ShellBase.dll %pakckagePath%%MetaseedShellBase%
xcopy /Y  Metaseed.ShellBase.pdb %pakckagePath%%MetaseedShellBase%
xcopy /Y  Metaseed.ShellBase.xml %pakckagePath%%MetaseedShellBase%