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

		public string Log { get { return Out; } }

		FileSystemWatcher fsw = null;
		string sourcePath = string.Empty;
		string destinationPath = string.Empty;
		bool running = false;

		public avmFileSystemWatcherProcess()
		{
			loadConfig();
		}

		public avmFileSystemWatcherProcess(string source = "", string destination = "")
		{
			loadConfig(source, destination);
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
			catch (Exception e)
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

		private void loadConfig(string source = "", string destination = "")
		{
			if (!string.IsNullOrEmpty(source) &&
				!string.IsNullOrEmpty(destination))
			{
				this.sourcePath = source;
				this.destinationPath = destination;
			}
			else
			{
				this.sourcePath = ConfigurationManager.AppSettings["SOURCE"];
				this.destinationPath = ConfigurationManager.AppSettings["DESTINATION"];
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
				this.fsw.Filter = "*.*";

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

				// WAIT TIMEOUT
				WaitForChangedResult wcr = fsw.WaitForChanged(WatcherChangeTypes.All, 30000);

				// TIMEOUT
				if (wcr.TimedOut)
				{
					Out = "FINALLY WAITING FOR 30 SECONDS...";
					Console.WriteLine("FINALLY WAITING FOR 30 SECONDS");
				}
				else
				{
					Out = string.Format("EVENT: {0} {1}", wcr.Name, wcr.ChangeType.ToString());
					Console.WriteLine(string.Format("EVENT: {0} {1}", wcr.Name, wcr.ChangeType.ToString()));
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private void fsw_Changed(object sender, FileSystemEventArgs e)
		{
			Console.WriteLine(String.Format("UPDATED: {0} {1}", e.FullPath, Environment.NewLine));
			Console.WriteLine(String.Format("NAME: {0} {1}", e.Name, Environment.NewLine));
			Console.WriteLine(String.Format("EVENT: {0} {1}", e.ChangeType, Environment.NewLine));
			Console.WriteLine(String.Format("----------------------- {0}", Environment.NewLine));

			Out = string.Format("UPDATED: {0} | {1} | {2}", e.FullPath, e.Name, e.ChangeType.ToString());
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

				Console.WriteLine(String.Format("CREATED: {0} {1}", e.FullPath, Environment.NewLine));
				Console.WriteLine(String.Format("NAME: {0} {1}", e.Name, Environment.NewLine));
				Console.WriteLine(String.Format("EVENT: {0} {1}", e.ChangeType, Environment.NewLine));
				Console.WriteLine(String.Format("----------------------- {0}", Environment.NewLine));
				File.Copy(e.FullPath, destinationPath + e.Name, true);
				File.Delete(sourcePath + e.Name);

				Console.WriteLine(String.Format("COPIED FROM {0} TO {1} {2}", e.FullPath, destinationPath + e.Name, Environment.NewLine));
				Console.WriteLine(String.Format("----------------------- {0}", Environment.NewLine));

				Out = string.Format("CREATED: {0} | {1} | {2}", e.FullPath, e.Name, e.ChangeType.ToString());
				Out = string.Format("COPIED: {0} | {1}", e.FullPath, destinationPath + e.Name);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(Environment.NewLine);
				Console.WriteLine(ex?.InnerException?.Message);
			}

		}

		private void fsw_Deleted(object sender, FileSystemEventArgs e)
		{
			Console.WriteLine(String.Format("DELETED: {0}, {1}", e.FullPath, Environment.NewLine));
			Console.WriteLine(String.Format("NAME: {0} {1}", e.Name, Environment.NewLine));
			Console.WriteLine(String.Format("EVENT: {0} {1}", e.ChangeType, Environment.NewLine));
			Console.WriteLine(String.Format("----------------------- {0}", Environment.NewLine));
			Out = string.Format("DELETED: {0} | {1} | {2}", e.FullPath, e.Name, e.ChangeType.ToString());

		}

		private void fsw_Renamed(object sender, RenamedEventArgs e)
		{
			Console.WriteLine(String.Format("NAME UPDATED: {0} {1}", e.FullPath, Environment.NewLine));
			Console.WriteLine(String.Format("NAME: {0} {1}", e.Name, Environment.NewLine));
			Console.WriteLine(String.Format("EVENT: {0} {1}", e.ChangeType, Environment.NewLine));
			Console.WriteLine(String.Format("----------------------- {0}", Environment.NewLine));
			Out = string.Format("NAME UPDATED: {0} | {1} | {2}", e.FullPath, e.Name, e.ChangeType.ToString());
		}

		public void Dispose()
		{
			fsw.Dispose();
			GC.WaitForPendingFinalizers();
			GC.SuppressFinalize(this);
		}
	}
}
