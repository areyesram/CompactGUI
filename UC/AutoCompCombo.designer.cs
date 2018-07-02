namespace Aryes.UC
{
    partial class AutoCompCombo
    {
        private System.Windows.Forms.ContextMenuStrip menuStripPath;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem menuPathCut;
        private System.Windows.Forms.ToolStripMenuItem menuPathCopy;
        private System.Windows.Forms.ToolStripMenuItem menuPathPaste;
        private System.Windows.Forms.ToolStripMenuItem menuPathDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuPathSelect;
        private System.Windows.Forms.ToolStripMenuItem menuPathBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem menuPathRecursive;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStripPath = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuPathCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPathCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPathPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPathDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPathSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuPathBrowse = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPathRecursive = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStripPath.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripPath
            // 
            this.menuStripPath.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPathCut,
            this.menuPathCopy,
            this.menuPathPaste,
            this.menuPathDelete,
            this.menuPathSelect,
            this.toolStripSeparator,
            this.menuPathBrowse,
            this.menuPathRecursive});
            this.menuStripPath.Name = "contextMenuStrip1";
            this.menuStripPath.Size = new System.Drawing.Size(125, 164);
            this.menuStripPath.Opening += new System.ComponentModel.CancelEventHandler(this.menuStripPath_Opening);
            // 
            // menuPathCut
            // 
            this.menuPathCut.Name = "menuPathCut";
            this.menuPathCut.Size = new System.Drawing.Size(124, 22);
            this.menuPathCut.Text = "Cut";
            this.menuPathCut.Click += new System.EventHandler(this.menuPathCut_Click);
            // 
            // menuPathCopy
            // 
            this.menuPathCopy.Name = "menuPathCopy";
            this.menuPathCopy.Size = new System.Drawing.Size(124, 22);
            this.menuPathCopy.Text = "Copy";
            this.menuPathCopy.Click += new System.EventHandler(this.menuPathCopy_Click);
            // 
            // menuPathPaste
            // 
            this.menuPathPaste.Name = "menuPathPaste";
            this.menuPathPaste.Size = new System.Drawing.Size(124, 22);
            this.menuPathPaste.Text = "Paste";
            this.menuPathPaste.Click += new System.EventHandler(this.menuPathPaste_Click);
            // 
            // menuPathDelete
            // 
            this.menuPathDelete.Name = "menuPathDelete";
            this.menuPathDelete.Size = new System.Drawing.Size(124, 22);
            this.menuPathDelete.Text = "Delete";
            this.menuPathDelete.Click += new System.EventHandler(this.menuPathDelete_Click);
            // 
            // menuPathSelect
            // 
            this.menuPathSelect.Name = "menuPathSelect";
            this.menuPathSelect.Size = new System.Drawing.Size(124, 22);
            this.menuPathSelect.Text = "Select All";
            this.menuPathSelect.Click += new System.EventHandler(this.menuPathSelect_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(121, 6);
            // 
            // menuPathBrowse
            // 
            this.menuPathBrowse.Name = "menuPathBrowse";
            this.menuPathBrowse.Size = new System.Drawing.Size(124, 22);
            this.menuPathBrowse.Text = "Browse...";
            this.menuPathBrowse.Click += new System.EventHandler(this.menuPathBrowse_Click);
            // 
            // menuPathRecursive
            // 
            this.menuPathRecursive.Checked = true;
            this.menuPathRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuPathRecursive.Name = "menuPathRecursive";
            this.menuPathRecursive.Size = new System.Drawing.Size(124, 22);
            this.menuPathRecursive.Text = "Recursive";
            this.menuPathRecursive.Click += new System.EventHandler(this.menuPathRecursive_Click);
            // 
            // AutoCompCombo
            // 
            this.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.ContextMenuStrip = this.menuStripPath;
            this.menuStripPath.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
