using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using avmFileSystemWatcher;

namespace avmFileSystemWatcher.App
{
	class Program
	{
		static void Main(string[] args)
		{
			avmFileSystemWatcherProcess avmSync = null;
			if (args.Count() > 1)
			{
				if (args[1] == null || args[2] == null)
				{
					avmSync = new avmFileSystemWatcherProcess();
				}
				else
				{
					avmSync = new avmFileSystemWatcherProcess(args[1], args[2]);
				}				
			}
			else
			{
				avmSync = new avmFileSystemWatcherProcess();
			}

			while (avmSync != null && true)
			{
				avmSync.Init();				
			}
	   }
	}
}
