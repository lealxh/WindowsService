namespace Datatec.WindowsService
{
    partial class ProjectInstaller
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
            this.DatatecServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.DatatecService = new System.ServiceProcess.ServiceInstaller();
            // 
            // DatatecServiceInstaller
            // 
            this.DatatecServiceInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.DatatecServiceInstaller.Password = null;
            this.DatatecServiceInstaller.Username = null;
            // 
            // DatatecService
            // 
            this.DatatecService.ServiceName = "Datatec";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DatatecServiceInstaller,
            this.DatatecService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller DatatecServiceInstaller;
        private System.ServiceProcess.ServiceInstaller DatatecService;
    }
}