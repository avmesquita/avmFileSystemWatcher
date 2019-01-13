namespace avmFileSystemWatcher.WindowsService
{
	partial class avmInstallerClass
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.avmServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.avmServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// avmServiceProcessInstaller
			// 
			this.avmServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
			this.avmServiceProcessInstaller.Password = null;
			this.avmServiceProcessInstaller.Username = null;
			// 
			// avmServiceInstaller
			// 
			this.avmServiceInstaller.Description = "avmFileSystemWatcher Windows Service To Sync Folders";
			this.avmServiceInstaller.DisplayName = "avmFileSystemWatcher Windows Service";
			this.avmServiceInstaller.ServiceName = "ServicoSincronizacaoRemessas";
			this.avmServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// avmInstallerClass
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.avmServiceInstaller,
            this.avmServiceProcessInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller avmServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller avmServiceInstaller;
	}
}