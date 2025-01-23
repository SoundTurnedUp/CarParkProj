namespace CarParkWFProj
{
    partial class MainMenu
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
            MenuLabel = new Label();
            EnterButton = new Button();
            LeaveButton = new Button();
            ManagementButton = new Button();
            SuspendLayout();
            // 
            // MenuLabel
            // 
            MenuLabel.AutoSize = true;
            MenuLabel.Location = new Point(215, 30);
            MenuLabel.Name = "MenuLabel";
            MenuLabel.Size = new Size(41, 15);
            MenuLabel.TabIndex = 0;
            MenuLabel.Text = "MENU";
            // 
            // EnterButton
            // 
            EnterButton.Location = new Point(178, 80);
            EnterButton.Name = "EnterButton";
            EnterButton.Size = new Size(125, 65);
            EnterButton.TabIndex = 1;
            EnterButton.Text = "Enter Car Park";
            EnterButton.UseVisualStyleBackColor = true;
            EnterButton.Click += EnterButton_Click;
            // 
            // LeaveButton
            // 
            LeaveButton.Location = new Point(178, 160);
            LeaveButton.Name = "LeaveButton";
            LeaveButton.Size = new Size(125, 65);
            LeaveButton.TabIndex = 2;
            LeaveButton.Text = "Leave Car Park";
            LeaveButton.UseVisualStyleBackColor = true;
            LeaveButton.Click += LeaveButton_Click;
            // 
            // ManagementButton
            // 
            ManagementButton.Location = new Point(178, 240);
            ManagementButton.Name = "ManagementButton";
            ManagementButton.Size = new Size(125, 65);
            ManagementButton.TabIndex = 3;
            ManagementButton.Text = "Management";
            ManagementButton.UseVisualStyleBackColor = true;
            ManagementButton.Click += ManagementButton_Click;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 361);
            Controls.Add(ManagementButton);
            Controls.Add(LeaveButton);
            Controls.Add(EnterButton);
            Controls.Add(MenuLabel);
            Name = "MainMenu";
            Text = "Main Menu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label MenuLabel;
        private Button EnterButton;
        private Button LeaveButton;
        private Button ManagementButton;
    }
}
