namespace FileMcpServer
{
    partial class DragDropFileList
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        private void DragDropFileList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void DragDropFileList_DragDrop(object sender, DragEventArgs e)
        {
            var droppedItems = (string[])e.Data.GetData(DataFormats.FileDrop);

            var allPaths = new List<string>();

            foreach (var path in droppedItems)
            {
                if (Directory.Exists(path))
                {
                    // Optionally, add all files in the folder recursively
                    var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    allPaths.AddRange(files);
                }
                else if (File.Exists(path))
                {
                    allPaths.Add(path);
                }
            }

            // Remove duplicates and add to ListBox
            foreach (var filePath in allPaths.Distinct())
            {
                if (!listBoxPaths.Items.Contains(filePath))
                    listBoxPaths.Items.Add(filePath);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBoxPaths = new ListBox();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBoxPaths.FormattingEnabled = true;
            listBoxPaths.ItemHeight = 15;
            listBoxPaths.Location = new Point(12, 12);
            listBoxPaths.Name = "listBox1";
            listBoxPaths.Size = new Size(560, 334);
            listBoxPaths.TabIndex = 0;
            // 
            // DragDropFileList
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 361);
            Controls.Add(listBoxPaths);
            Name = "DragDropFileList";
            Text = "Drag and Drop Files/Folders";
            DragDrop += DragDropFileList_DragDrop;
            DragEnter += DragDropFileList_DragEnter;
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBoxPaths;
    }
}
