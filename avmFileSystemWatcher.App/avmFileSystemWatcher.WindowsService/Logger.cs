using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avmFileSystemWatcher.WindowsService
{
	public static class Logger
	{
		private static log4net.ILog log { get; set; }

		static Logger()
		{
			log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			XmlConfigurator.Configure();
		}

		public enum SystemLogTypes : int
		{
			Fatal = 0,
			Error = 1,
			Warn = 2,
			Info = 3,
			Debug = 4
		}

		/// <summary>
		/// Logs the given message
		/// </summary>
		/// <param name="log"></param>
		public static int Log(string message)
		{
			return Log(message, SystemLogTypes.Info);
		}

		/// <summary>
		/// Logs the given message
		/// </summary>
		/// <param name="log"></param>
		/// <param name="type"></param>
		public static int Log(string message, SystemLogTypes type)
		{
			var log = new Exception(message);

			return Log(log, type);
		}

		/// <summary>
		/// Logs this error and description
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="description"></param>
		public static int Log(Exception ex, string description)
		{
			var log = new Exception(ex.Message.ToString(), ex.InnerException);
			return Log(log, SystemLogTypes.Error);
		}

		/// <summary>
		/// Logs the given message
		/// </summary>
		/// <param name="log"></param>
		public static int Log(Exception ex, SystemLogTypes nivel = SystemLogTypes.Error)
		{
			switch (nivel)
			{
				case SystemLogTypes.Fatal:
					log.Fatal(ex);
					break;
				case SystemLogTypes.Error:
					log.Error(ex);
					break;
				case SystemLogTypes.Warn:
					log.Warn(ex);
					break;
				case SystemLogTypes.Info:
					log.Info(ex);
					break;
				case SystemLogTypes.Debug:
					log.Debug(ex);
					break;
				default:
					break;
			}
			return 0;
		}

		#region Constantes

		private readonly static string fileName = "avmFileSystemWatcher";
		private readonly static string file = string.Format(@"\{0}.log", fileName);

		#endregion
	}
}
