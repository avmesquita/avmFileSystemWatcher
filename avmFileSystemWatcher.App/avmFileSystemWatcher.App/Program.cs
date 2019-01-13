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
			try
			{				
				if (args.Count() > 1)
				{
					if (args[1] == null || args[2] == null)
					{
						avmSync = new avmFileSystemWatcherProcess();
					}
					else
					{
						if (args[3] != null)
						{
							avmSync = new avmFileSystemWatcherProcess(args[1], args[2], Convert.ToInt32(args[3]));
						}
						else
						{
							avmSync = new avmFileSystemWatcherProcess(args[1], args[2]);
						}
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

				avmSync.Dispose();
			}
			catch (Exception ex)
			{
				if (avmSync != null)
				{
					avmSync.Dispose();
					avmSync = null;
				}
				throw new Exception(string.Format("Puff! \rMESSAGE = {0} | INNER = {1} | STACK = {2} | SOURCE = {3}\rPLEASE, CREATE A ISSUE INTO THIS PROJECT.\r", ex.Message, ex.InnerException, ex.StackTrace, ex.Source));
			}
	   }
	}
}
