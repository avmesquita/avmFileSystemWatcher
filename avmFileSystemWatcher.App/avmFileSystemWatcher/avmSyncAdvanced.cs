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
	/// BLOG POST BASED
	/// https://windows7bugs.wordpress.com/2017/01/26/c-a-folder-sync-tool/
	/// </summary>
	public class avmSyncAdvanced
	{
		private string _source = string.Empty;
		private string _destination = string.Empty;

		public avmSyncAdvanced(string source, string destination)
		{
			this._source = source;
			this._destination = destination;

			if (!Directory.Exists(this._source))
			{
				try
				{
					Directory.CreateDirectory(this._source);
				}
				catch
				{
					throw new Exception("SOURCE PATH IS INVALID.");
				}
				if (!Directory.Exists(this._source))
				{
					throw new Exception("SOURCE PATH IS INVALID.");
				}
			}

			if (!Directory.Exists(this._destination))
			{
				try
				{
					Directory.CreateDirectory(this._destination);
				}
				catch
				{
					throw new Exception("DESTINATION PATH IS INVALID.");
				}
				if (!Directory.Exists(this._destination))
				{
					throw new Exception("DESTINATION PATH IS INVALID.");
				}
			}
		}

		public void Sync()
		{
			try
			{
				Copy(_source, _destination);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private void Copy(string sourceDirectory, string targetDirectory)
		{
			DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
			DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

			CopyAll(diSource, diTarget);
		}

		private void CopyAll(DirectoryInfo source, DirectoryInfo target)
		{

			Directory.CreateDirectory(target.FullName);
			
			foreach (FileInfo fi in source.GetFiles())
			{

				DateTime created = fi.CreationTime;
				DateTime lastmodified = fi.LastWriteTime;

				if (File.Exists(Path.Combine(target.FullName, fi.Name)))
				{
					string tFileName = Path.Combine(target.FullName, fi.Name);
					FileInfo f2 = new FileInfo(tFileName);
					DateTime lm = f2.LastWriteTime;
					Console.WriteLine(@"FILE {0}\{1} ALREADY EXIST {2} LAST MODIFIED {3}", target.FullName, fi.Name, tFileName, lm);

					try
					{
						if (lastmodified > lm)
						{
							Console.WriteLine(@"SOURCE FILE {0}\{1} LAST MODIFIED {2} IS NEWER THAN THE TARGET FILE {3}\{4} LAST MODIFIED {5}",
								fi.DirectoryName, fi.Name, lastmodified.ToString(), target.FullName, fi.Name, lm.ToString());
							fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
						}
						else
						{
							Console.WriteLine(@"DESTINATION FILE {0}\{1} SKIPPED", target.FullName, fi.Name);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				else
				{
					Console.WriteLine(@"COPYING {0}\{1}", target.FullName, fi.Name);
					fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
				}
			}	
			
			foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir =
					target.CreateSubdirectory(diSourceSubDir.Name);
				CopyAll(diSourceSubDir, nextTargetSubDir);
			}
		}
	}
}
