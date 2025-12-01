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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ListBoxPaths = new ListBox();
            RemoveButton = new Button();
            label1 = new Label();
            RemoveAllButton = new Button();
            SuspendLayout();
            // 
            // ListBoxPaths
            // 
            ListBoxPaths.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ListBoxPaths.FormattingEnabled = true;
            ListBoxPaths.ItemHeight = 15;
            ListBoxPaths.Location = new Point(12, 27);
            ListBoxPaths.Name = "ListBoxPaths";
            ListBoxPaths.SelectionMode = SelectionMode.MultiExtended;
            ListBoxPaths.Size = new Size(560, 334);
            ListBoxPaths.TabIndex = 0;
            ListBoxPaths.SelectedIndexChanged += ListBoxPaths_SelectedIndexChanged;
            ListBoxPaths.Resize += ListBoxPaths_Resize;
            // 
            // RemoveButton
            // 
            RemoveButton.Enabled = false;
            RemoveButton.Location = new Point(12, 367);
            RemoveButton.Name = "RemoveButton";
            RemoveButton.Size = new Size(75, 23);
            RemoveButton.TabIndex = 1;
            RemoveButton.Text = "Remove";
            RemoveButton.UseVisualStyleBackColor = true;
            RemoveButton.Click += RemoveButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(556, 15);
            label1.TabIndex = 2;
            label1.Text = "Drag and drop to add files. Select multiple items with SHIFT and CTRL. Press DELETE to remove selection.";
            // 
            // RemoveAllButton
            // 
            RemoveAllButton.Enabled = false;
            RemoveAllButton.Location = new Point(93, 367);
            RemoveAllButton.Name = "RemoveAllButton";
            RemoveAllButton.Size = new Size(75, 23);
            RemoveAllButton.TabIndex = 3;
            RemoveAllButton.Text = "Remove All";
            RemoveAllButton.UseVisualStyleBackColor = true;
            RemoveAllButton.Click += RemoveAllButton_Click;
            // 
            // DragDropFileList
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 402);
            Controls.Add(RemoveAllButton);
            Controls.Add(label1);
            Controls.Add(RemoveButton);
            Controls.Add(ListBoxPaths);
            MinimumSize = new Size(600, 440);
            Name = "DragDropFileList";
            Text = "Drag and Drop Files/Folders";
            DragDrop += DragDropFileList_DragDrop;
            DragEnter += DragDropFileList_DragEnter;
            KeyDown += DragDropFileList_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ListBoxPaths;
        private Button RemoveButton;
        private Label label1;
        private Button RemoveAllButton;
    }
}
