The files found herein are to facilitate the debugging experience while using Microsoft Visual Studio when using the git source control management system.  The files are divided into several directories: SourceServer, VisualStudio, InSystemPath.

The source indexing mechanism has the advantage of being able to retrieve the exact source file for a given library when it was compiled while stepping into (F11 in VS) that library code.  While not an excuse to avoid TDD or other forms of automated testing, it does give the advantage to be able to step into a compiled library where the source would not normally be readily available.

SourceServer
The files in this directory are to be placed inside of the “srcsrv” installation for Microsoft’s Debugging Tools for Windows.  Your exact installation directory may vary, but it is something like the following: C:\Program Files\Debugging Tools for Windows (x64)\srcsrv

VisualStudio
The file in this directory, srcsrv.ini, is simply letting Visual Studio know what commands are allowed to be executed from symbols (PDB files) that are loaded.  In our situation, we trust svn.exe and gitcontents.bat.  In our case, we put it in: C:\Program Files (x86)\Microsoft Visual Studio 9.0\Common7\IDE

InSystemPath
The file in this directory, gitcontents.bat, must be placed in any directory located in the %PATH% environment variable.  In our case, we put it in this directory: C:\Program Files (x86)\Microsoft Visual Studio 9.0\Common7\IDE so that we didn’t have to worry about setting up another path entry.

Indexing Usage:
After compiling your application, you’ll want to commit your changes.  Furthermore, you’ll want to push your changes to the “origin” repository (at least for this test run).  In a normal scenario, the source indexing mechanism will occur on a build server, so you needn’t worry about pushing changes to the origin repository.

You’re now read to index your source code files.  If your Microsoft Debugging Tools srcsrv directory is in the path, you could simply call the following command, while at the root directory of your code project:

gitindex.cmd /debug /source=. /symbols=.\MyProject\bin\Debug

You would then get a message indicating that the PDB files have been rewritten.

Visual Studio Debugging
Enable Source Server Support in the Visual Studio options. I also enable diagnostic messages.

Prior to testing this configuration, you’ll want to change your original project location (by changing the name, for example) to another name.  The reason for this is so that the standard source-loading mechanisms inside of Visual Studio will fail to load your source file from your local disk—we want to test that we can do this remotely.

Create a new console/application project, and add your project library as a dependency (the source indexed, compiled DLL) and make sure that the PDB file is right alongside the DLL library file.  Write some code that uses a class from your library project.  Set a breakpoint just above the code usage.  Run your project.  When you hit the breakpoint, step into your library.  VS will pause for a few seconds while it communicates with the remote repository and caches it locally.  It should then display the pulled source code on your screen.  Subsequent calls will be much faster because it’s caching the repository locally.

Other
You need to have git.exe in your path somewhere.  Also, when you are indexing your source code, Microsoft’s indexing mechanism runs using Perl.  We use ActiveState perl.  Be careful that you don’t have collisions between cygwin’s perl implementation and ActiveState’s if you have both in the path.  ActiveState seems to play better with source indexing.