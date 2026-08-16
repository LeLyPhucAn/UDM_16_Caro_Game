namespace Client.Forms
{
    partial class LoginForm
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

        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            txtPlayerName = new TextBox();
            btnEnterLobby = new Button();
            btnExit = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(220, 116);
            label1.Name = "label1";
            label1.Size = new Size(0, 23);
            label1.TabIndex = 0;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.Highlight;
            label2.ImageAlign = ContentAlignment.MiddleRight;
            label2.Location = new Point(126, 52);
            label2.Name = "label2";
            label2.Size = new Size(356, 46);
            label2.TabIndex = 1;
            label2.Text = "GAME CARO ONLINE";
            // 
            // txtPlayerName
            // 
            txtPlayerName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPlayerName.BackColor = Color.FromArgb(64, 64, 64);
            txtPlayerName.ForeColor = Color.White;
            txtPlayerName.Location = new Point(100, 142);
            txtPlayerName.Name = "txtPlayerName";
            txtPlayerName.PlaceholderText = "Nhập tên người chơi...";
            txtPlayerName.Size = new Size(415, 30);
            txtPlayerName.TabIndex = 2;
            // 
            // btnEnterLobby
            // 
            btnEnterLobby.BackColor = Color.DodgerBlue;
            btnEnterLobby.FlatStyle = FlatStyle.Flat;
            btnEnterLobby.Font = new Font("Segoe UI", 10.1F, FontStyle.Bold);
            btnEnterLobby.Location = new Point(100, 222);
            btnEnterLobby.Name = "btnEnterLobby";
            btnEnterLobby.Size = new Size(415, 42);
            btnEnterLobby.TabIndex = 3;
            btnEnterLobby.Text = "VÀO SẢNH CHỜ";
            btnEnterLobby.UseVisualStyleBackColor = false;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.IndianRed;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExit.Location = new Point(100, 270);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(415, 39);
            btnExit.TabIndex = 4;
            btnExit.Text = "THOÁT";
            btnExit.UseVisualStyleBackColor = false;
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(34, 36, 40);
            ClientSize = new Size(582, 453);
            Controls.Add(btnExit);
            Controls.Add(btnEnterLobby);
            Controls.Add(txtPlayerName);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "FormLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Caro Client v1.0.4";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox txtPlayerName;
        private Button btnEnterLobby;
        private Button btnExit;
    }
}
