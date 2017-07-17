namespace VisualTester
{
    partial class Mainform
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
            this.launchTestButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // launchTestButton
            // 
            this.launchTestButton.Location = new System.Drawing.Point(244, 186);
            this.launchTestButton.Name = "launchTestButton";
            this.launchTestButton.Size = new System.Drawing.Size(75, 23);
            this.launchTestButton.TabIndex = 0;
            this.launchTestButton.Text = "Launch test";
            this.launchTestButton.UseVisualStyleBackColor = true;
            this.launchTestButton.Click += new System.EventHandler(this.launchTestButton_Click);
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 400);
            this.Controls.Add(this.launchTestButton);
            this.Name = "Mainform";
            this.Text = "Visual tester";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button launchTestButton;
    }
}

