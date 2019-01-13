using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace avmFileSystemWatcher.WindowsService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			// TO YOU ;)
			if (Environment.UserInteractive)
			{				
				var avmSync = new avmFileSystemWatcherProcess();

				bool init = true;
				while (init)
				{
					init = avmSync.Init();
					Logger.Log(avmSync.Log);
				}
			}
			else
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[]
				{
				new avmService()
				};
				ServiceBase.Run(ServicesToRun);
			}
		}
	}
}
