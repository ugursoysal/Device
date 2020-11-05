namespace Device
{
    partial class Manager
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manager));
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.loadAccountsButton = new System.Windows.Forms.Button();
            this.SaveAccountsButton = new System.Windows.Forms.Button();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.ClearAccountsButton = new System.Windows.Forms.Button();
            this.AddAccountButton = new System.Windows.Forms.Button();
            this.RemoveAccountButton = new System.Windows.Forms.Button();
            this.StartAllButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.closeButton = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.userTextBox = new System.Windows.Forms.TextBox();
            this.passTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.startButton.Location = new System.Drawing.Point(724, 46);
            this.startButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(88, 27);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = false;
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.stopButton.Location = new System.Drawing.Point(819, 46);
            this.stopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(61, 27);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.loadButton.Location = new System.Drawing.Point(887, 46);
            this.loadButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(88, 27);
            this.loadButton.TabIndex = 1;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = false;
            this.loadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // loadAccountsButton
            // 
            this.loadAccountsButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.loadAccountsButton.Location = new System.Drawing.Point(721, 291);
            this.loadAccountsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.loadAccountsButton.Name = "loadAccountsButton";
            this.loadAccountsButton.Size = new System.Drawing.Size(122, 27);
            this.loadAccountsButton.TabIndex = 1;
            this.loadAccountsButton.Text = "Load Accounts";
            this.loadAccountsButton.UseVisualStyleBackColor = false;
            this.loadAccountsButton.Click += new System.EventHandler(this.LoadAccountsButton_Click);
            // 
            // SaveAccountsButton
            // 
            this.SaveAccountsButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.SaveAccountsButton.Location = new System.Drawing.Point(849, 291);
            this.SaveAccountsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SaveAccountsButton.Name = "SaveAccountsButton";
            this.SaveAccountsButton.Size = new System.Drawing.Size(121, 27);
            this.SaveAccountsButton.TabIndex = 1;
            this.SaveAccountsButton.Text = "Save Accounts";
            this.SaveAccountsButton.UseVisualStyleBackColor = false;
            this.SaveAccountsButton.Click += new System.EventHandler(this.SaveAccountsButton_Click);
            // 
            // portTextBox
            // 
            this.portTextBox.Font = new System.Drawing.Font("NSimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.portTextBox.Location = new System.Drawing.Point(926, 326);
            this.portTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.portTextBox.MaximumSize = new System.Drawing.Size(44, 20);
            this.portTextBox.MaxLength = 5;
            this.portTextBox.MinimumSize = new System.Drawing.Size(44, 20);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(44, 21);
            this.portTextBox.TabIndex = 3;
            this.portTextBox.Text = "53399";
            // 
            // ClearAccountsButton
            // 
            this.ClearAccountsButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.ClearAccountsButton.Location = new System.Drawing.Point(721, 257);
            this.ClearAccountsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ClearAccountsButton.Name = "ClearAccountsButton";
            this.ClearAccountsButton.Size = new System.Drawing.Size(91, 27);
            this.ClearAccountsButton.TabIndex = 4;
            this.ClearAccountsButton.Text = "Clear";
            this.ClearAccountsButton.UseVisualStyleBackColor = false;
            this.ClearAccountsButton.Click += new System.EventHandler(this.ClearAccountsButton_Click);
            // 
            // AddAccountButton
            // 
            this.AddAccountButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.AddAccountButton.Location = new System.Drawing.Point(816, 257);
            this.AddAccountButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AddAccountButton.Name = "AddAccountButton";
            this.AddAccountButton.Size = new System.Drawing.Size(82, 27);
            this.AddAccountButton.TabIndex = 4;
            this.AddAccountButton.Text = "Add";
            this.AddAccountButton.UseVisualStyleBackColor = false;
            this.AddAccountButton.Click += new System.EventHandler(this.AddAccountButton_Click);
            // 
            // RemoveAccountButton
            // 
            this.RemoveAccountButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.RemoveAccountButton.Location = new System.Drawing.Point(905, 257);
            this.RemoveAccountButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.RemoveAccountButton.Name = "RemoveAccountButton";
            this.RemoveAccountButton.Size = new System.Drawing.Size(66, 27);
            this.RemoveAccountButton.TabIndex = 4;
            this.RemoveAccountButton.Text = "Remove";
            this.RemoveAccountButton.UseVisualStyleBackColor = false;
            this.RemoveAccountButton.Click += new System.EventHandler(this.RemoveAccountButton_Click);
            // 
            // StartAllButton
            // 
            this.StartAllButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.StartAllButton.Location = new System.Drawing.Point(724, 13);
            this.StartAllButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StartAllButton.Name = "StartAllButton";
            this.StartAllButton.Size = new System.Drawing.Size(122, 27);
            this.StartAllButton.TabIndex = 1;
            this.StartAllButton.Text = "Start All";
            this.StartAllButton.UseVisualStyleBackColor = false;
            this.StartAllButton.Click += new System.EventHandler(this.StartAllButton_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.MenuBar;
            this.button2.Location = new System.Drawing.Point(852, 13);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 27);
            this.button2.TabIndex = 1;
            this.button2.Text = "Stop All";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.SaveAccountsButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.MenuBar;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Location = new System.Drawing.Point(200, 14);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(515, 335);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.closeButton.ForeColor = System.Drawing.Color.DarkRed;
            this.closeButton.Location = new System.Drawing.Point(721, 324);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(198, 23);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(13, 13);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(181, 334);
            this.listBox1.TabIndex = 7;
            // 
            // userTextBox
            // 
            this.userTextBox.Location = new System.Drawing.Point(722, 231);
            this.userTextBox.MaxLength = 40;
            this.userTextBox.Name = "userTextBox";
            this.userTextBox.Size = new System.Drawing.Size(124, 22);
            this.userTextBox.TabIndex = 8;
            // 
            // passTextBox
            // 
            this.passTextBox.Location = new System.Drawing.Point(852, 231);
            this.passTextBox.Name = "passTextBox";
            this.passTextBox.PasswordChar = '*';
            this.passTextBox.Size = new System.Drawing.Size(121, 22);
            this.passTextBox.TabIndex = 8;
            // 
            // Manager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSlateGray;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(980, 360);
            this.Controls.Add(this.passTextBox);
            this.Controls.Add(this.userTextBox);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.RemoveAccountButton);
            this.Controls.Add(this.AddAccountButton);
            this.Controls.Add(this.ClearAccountsButton);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.SaveAccountsButton);
            this.Controls.Add(this.StartAllButton);
            this.Controls.Add(this.loadAccountsButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.startButton);
            this.Font = new System.Drawing.Font("Open Sans Semibold", 8F, System.Drawing.FontStyle.Bold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(980, 360);
            this.MinimumSize = new System.Drawing.Size(980, 360);
            this.Name = "Manager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manager";
            this.Load += new System.EventHandler(this.Manager_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Manager_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button loadAccountsButton;
        private System.Windows.Forms.Button SaveAccountsButton;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Button ClearAccountsButton;
        private System.Windows.Forms.Button AddAccountButton;
        private System.Windows.Forms.Button RemoveAccountButton;
        private System.Windows.Forms.Button StartAllButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox userTextBox;
        private System.Windows.Forms.TextBox passTextBox;
    }
}

