namespace Server
{
    partial class Server
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server));
            this.Indicator = new System.Windows.Forms.PictureBox();
            this.InfoText = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.clientIDLabel = new System.Windows.Forms.Label();
            this.TestButton = new System.Windows.Forms.Button();
            this.Test2Button = new System.Windows.Forms.Button();
            this.testBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Indicator)).BeginInit();
            this.SuspendLayout();
            // 
            // Indicator
            // 
            this.Indicator.BackColor = System.Drawing.Color.Transparent;
            this.Indicator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Indicator.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Indicator.ErrorImage = null;
            this.Indicator.Image = global::Server.Properties.Resources.off;
            this.Indicator.InitialImage = null;
            this.Indicator.Location = new System.Drawing.Point(12, 12);
            this.Indicator.Name = "Indicator";
            this.Indicator.Size = new System.Drawing.Size(225, 83);
            this.Indicator.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Indicator.TabIndex = 0;
            this.Indicator.TabStop = false;
            this.Indicator.Click += new System.EventHandler(this.Indicator_Click);
            // 
            // InfoText
            // 
            this.InfoText.AutoSize = true;
            this.InfoText.BackColor = System.Drawing.Color.Transparent;
            this.InfoText.Enabled = false;
            this.InfoText.Font = new System.Drawing.Font("Segoe Print", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoText.Location = new System.Drawing.Point(9, 98);
            this.InfoText.MaximumSize = new System.Drawing.Size(231, 280);
            this.InfoText.MinimumSize = new System.Drawing.Size(231, 280);
            this.InfoText.Name = "InfoText";
            this.InfoText.Size = new System.Drawing.Size(231, 280);
            this.InfoText.TabIndex = 1;
            this.InfoText.Text = "...";
            this.InfoText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // portTextBox
            // 
            this.portTextBox.Font = new System.Drawing.Font("NSimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.portTextBox.Location = new System.Drawing.Point(12, 418);
            this.portTextBox.MaximumSize = new System.Drawing.Size(38, 20);
            this.portTextBox.MaxLength = 5;
            this.portTextBox.MinimumSize = new System.Drawing.Size(38, 20);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(38, 21);
            this.portTextBox.TabIndex = 2;
            this.portTextBox.Text = "53399";
            this.portTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PortTextBox_KeyPress);
            // 
            // clientIDLabel
            // 
            this.clientIDLabel.AutoSize = true;
            this.clientIDLabel.BackColor = System.Drawing.Color.Transparent;
            this.clientIDLabel.Enabled = false;
            this.clientIDLabel.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.clientIDLabel.Location = new System.Drawing.Point(218, 420);
            this.clientIDLabel.MaximumSize = new System.Drawing.Size(19, 13);
            this.clientIDLabel.MinimumSize = new System.Drawing.Size(19, 13);
            this.clientIDLabel.Name = "clientIDLabel";
            this.clientIDLabel.Size = new System.Drawing.Size(19, 13);
            this.clientIDLabel.TabIndex = 3;
            this.clientIDLabel.Text = "0";
            this.clientIDLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(56, 416);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(75, 23);
            this.TestButton.TabIndex = 5;
            this.TestButton.Text = "Test";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // Test2Button
            // 
            this.Test2Button.Location = new System.Drawing.Point(137, 415);
            this.Test2Button.Name = "Test2Button";
            this.Test2Button.Size = new System.Drawing.Size(75, 23);
            this.Test2Button.TabIndex = 6;
            this.Test2Button.Text = "Test2";
            this.Test2Button.UseVisualStyleBackColor = true;
            this.Test2Button.Click += new System.EventHandler(this.Test2Button_Click);
            // 
            // testBox1
            // 
            this.testBox1.Location = new System.Drawing.Point(77, 390);
            this.testBox1.Name = "testBox1";
            this.testBox1.Size = new System.Drawing.Size(34, 20);
            this.testBox1.TabIndex = 7;
            this.testBox1.Text = "22";
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.BackgroundImage = global::Server.Properties.Resources.bg4;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(249, 450);
            this.Controls.Add(this.testBox1);
            this.Controls.Add(this.Test2Button);
            this.Controls.Add(this.TestButton);
            this.Controls.Add(this.clientIDLabel);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.InfoText);
            this.Controls.Add(this.Indicator);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Server";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DvcServer";
            this.Shown += new System.EventHandler(this.Server_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Server_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.Indicator)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Indicator;
        private System.Windows.Forms.Label InfoText;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label clientIDLabel;
        private System.Windows.Forms.Button TestButton;
        private System.Windows.Forms.Button Test2Button;
        private System.Windows.Forms.TextBox testBox1;
    }
}

