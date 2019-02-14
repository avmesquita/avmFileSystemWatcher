using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avmFileSystemWatcher
{
	/// <summary>
	/// Study Case
	/// http://www.linhadecodigo.com.br/artigo/3097/monitorando-arquivos-e-diretorios-com-filesystemwatcher.aspx
	/// </summary>
	public class avmFileSystemWatcherProcess : IDisposable
	{
		private string Out = string.Empty;
		private const int _timeoutDefault = 30000;

		public string Log { get { return Out; } }

		FileSystemWatcher fsw = null;
		string sourcePath = string.Empty;
		string sourceWildcard = string.Empty;
		string destinationPath = string.Empty;
		int timeout = _timeoutDefault;

		// thinking about this... to use with unique event of class... maybe future...
		public bool running = false;

		public bool _stopCommand = false;

		public avmFileSystemWatcherProcess()
		{
			loadConfig();
		}

		public avmFileSystemWatcherProcess(string source = "", string destination = "", int _timeout = _timeoutDefault)
		{
			loadConfig(source, destination);

			if (timeout != _timeoutDefault)
			{
				this.timeout = _timeout;
			}
		}

		public bool Init()
		{
			try
			{
				if (this.fsw == null)
				{
					this.fsw = new FileSystemWatcher();
					this.fsw.Changed += new FileSystemEventHandler(fsw_Changed);
					this.fsw.Created += new FileSystemEventHandler(fsw_Created);
					this.fsw.Deleted += new FileSystemEventHandler(fsw_Deleted);
					this.fsw.Renamed += new RenamedEventHandler(fsw_Renamed);					
				}

				Load();

				return true;
			}
			catch //(Exception e)
			{
				return false;
			}
		}

		~avmFileSystemWatcherProcess()
		{
			if (fsw != null)
			{
				fsw.Dispose();
			}
			fsw = null;
		}

		private void loadConfig(string source = "", string destination = "", string wildcard = "*.*")
		{
			if (!string.IsNullOrEmpty(source) &&
				!string.IsNullOrEmpty(destination))
			{
				this.sourcePath = source;
				this.sourceWildcard = wildcard;
				this.destinationPath = destination;
			}
			else
			{
				this.sourcePath = ConfigurationManager.AppSettings["SOURCE"];
				this.sourceWildcard = ConfigurationManager.AppSettings["SOURCE-WILDCARD"];
				this.destinationPath = ConfigurationManager.AppSettings["DESTINATION"];
				try
				{
					this.timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TIMEOUT"]);
				}
				catch
				{
					this.timeout = _timeoutDefault;
				}
			}
		}
		private void Load()
		{
			try
			{
				// SOURCE PATH MUST EXIST
				if (!Directory.Exists(sourcePath))
				{
					try
					{
						Directory.CreateDirectory(sourcePath);
					}
					catch
					{
						throw new Exception("SOURCE PATH IS INVALID.");
					}
					if (!Directory.Exists(destinationPath))
					{
						throw new Exception("SOURCE PATH IS INVALID.");
					}
				}

				// SOURCE PATH
				this.fsw.Path = sourcePath;

				// TYPES
				this.fsw.Filter = this.sourceWildcard;

				// EVENTS TO NOTIFY
				this.fsw.NotifyFilter = NotifyFilters.Attributes |
										NotifyFilters.CreationTime |
										NotifyFilters.DirectoryName |
										NotifyFilters.FileName |
										NotifyFilters.LastAccess |
										NotifyFilters.LastWrite |
										NotifyFilters.Security |
										NotifyFilters.Size;

				// THIS PARAMETER, TURNS MONITOR ON
				this.fsw.EnableRaisingEvents = true;

				// INCLUDE SUB-FOLDERS ?
				this.fsw.IncludeSubdirectories = false;

				running = true;

				// WAIT TIMEOUT
				WaitForChangedResult wcr = fsw.WaitForChanged(WatcherChangeTypes.All, this.timeout);

				// TIMEOUT
				if (wcr.TimedOut)
				{
					Out += "WAITING FOR 30 SECONDS..." + Environment.NewLine;
					Console.WriteLine("WAITING FOR 30 SECONDS");
					validateForceStop();
				}
				else
				{
					string eventMessage = string.Format("EVENT: NAME {0} TYPE {1}", wcr.Name, wcr.ChangeType.ToString());
					Out += eventMessage + Environment.NewLine;
					Console.WriteLine(eventMessage);
				}
			}
			catch (Exception ex)
			{
				running = false;
				throw ex;
			}
		}

		private void fsw_Changed(object sender, FileSystemEventArgs e)
		{
			string msg = string.Format("UPDATED: FILE {0} | NAME {1} | EVENT {2}", e.FullPath, e.Name, e.ChangeType.ToString());

			Console.WriteLine(msg);

			Out +=  msg + Environment.NewLine;

			validateForceStop();
		}

		private void fsw_Created(object sender, FileSystemEventArgs e)
		{
			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(e.FullPath)))
				{
					try
					{						
						Directory.CreateDirectory(Path.GetDirectoryName(e.FullPath));
					}
					catch
					{
						throw new Exception("SOURCE PATH IS INVALID.");
					}
					if (!Directory.Exists(Path.GetDirectoryName(e.FullPath)))
					{
						throw new Exception("SOURCE PATH IS INVALID.");
					}
				}

				if (!Directory.Exists(destinationPath))
				{
					try
					{
						Directory.CreateDirectory(destinationPath);
					}
					catch
					{
						throw new Exception("DESTINATION PATH IS INVALID.");
					}
					if (!Directory.Exists(destinationPath))
					{
						throw new Exception("SOURCE PATH IS INVALID.");
					}
				}
				string msg1 = string.Format("CREATED: {0} | {1} | {2}", e.FullPath, e.Name, e.ChangeType.ToString());
				Out += msg1 + Environment.NewLine;

				string msg2 = string.Format("COPIED: {0} | {1}", e.FullPath, destinationPath + e.Name);
				Out +=  msg1 + Environment.NewLine;

				Console.WriteLine(msg1);
				Console.WriteLine(msg2);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(Environment.NewLine);
				Console.WriteLine(ex?.InnerException?.Message);
			}
			validateForceStop();
		}

		private void fsw_Deleted(object sender, FileSystemEventArgs e)
		{
			string msg = string.Format("DELETED: {0} | {1} | {2}", e.FullPath, e.Name, e.ChangeType.ToString());

			Console.WriteLine(msg);

			Out += msg + Environment.NewLine;

			validateForceStop();
		}

		private void fsw_Renamed(object sender, RenamedEventArgs e)
		{
			string msg = string.Format("NAME UPDATED: {0} | {1} | {2}", e.FullPath, e.Name, e.ChangeType.ToString());

			Console.WriteLine(msg);

			Out += msg;

			validateForceStop();
		}

		private void validateForceStop()
		{
			if (this._stopCommand)
			{
				_stopCommand = false;
				Environment.Exit(2);
			}
		}

		public void Dispose()
		{
			fsw.Dispose();
			GC.WaitForPendingFinalizers();
			GC.SuppressFinalize(this);
		}
	}
}
