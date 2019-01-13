using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace avmFileSystemWatcher.WindowsService
{
	public partial class avmService : ServiceBase
	{
		private avmFileSystemWatcherProcess avmSync = null;

		public avmService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			avmSync = new avmFileSystemWatcherProcess();
			while (true)
			{
				avmSync.Init();
				Logger.Log(avmSync.Log);
			}
		}

		protected override void OnStop()
		{
			avmSync.Dispose();
			avmSync = null;
		}
	}
}
