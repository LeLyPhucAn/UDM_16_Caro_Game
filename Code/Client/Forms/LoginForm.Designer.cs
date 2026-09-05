namespace Client.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

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
            lblTitle = new Label();
            txtPlayerName = new TextBox();
            btnEnterLobby = new Button();
            btnExit = new Button();
            lblDecoX1 = new Label();
            lblDecoO1 = new Label();
            lblDecoX2 = new Label();
            lblDecoO2 = new Label();
            lblNamePrompt = new Label();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 152, 219);
            lblTitle.Location = new Point(109, 60);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(417, 54);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "GAME CARO ONLINE";
            // 
            // txtPlayerName
            // 
            txtPlayerName.BackColor = Color.FromArgb(64, 64, 64);
            txtPlayerName.BorderStyle = BorderStyle.FixedSingle;
            txtPlayerName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtPlayerName.ForeColor = Color.White;
            txtPlayerName.Location = new Point(126, 213);
            txtPlayerName.Margin = new Padding(3, 4, 3, 4);
            txtPlayerName.Name = "txtPlayerName";
            txtPlayerName.PlaceholderText = "Tên (3-15 ký tự, không dấu cách)";
            txtPlayerName.Size = new Size(343, 34);
            txtPlayerName.TabIndex = 1;
            txtPlayerName.TextAlign = HorizontalAlignment.Center;
            // 
            // btnEnterLobby
            // 
            btnEnterLobby.BackColor = Color.FromArgb(52, 152, 219);
            btnEnterLobby.FlatAppearance.BorderSize = 0;
            btnEnterLobby.FlatStyle = FlatStyle.Flat;
            btnEnterLobby.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnEnterLobby.ForeColor = Color.White;
            btnEnterLobby.Location = new Point(126, 300);
            btnEnterLobby.Margin = new Padding(3, 4, 3, 4);
            btnEnterLobby.Name = "btnEnterLobby";
            btnEnterLobby.Size = new Size(343, 60);
            btnEnterLobby.TabIndex = 2;
            btnEnterLobby.Text = "VÀO SẢNH CHỜ";
            btnEnterLobby.UseVisualStyleBackColor = false;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.FromArgb(217, 83, 79);
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(126, 380);
            btnExit.Margin = new Padding(3, 4, 3, 4);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(343, 60);
            btnExit.TabIndex = 3;
            btnExit.Text = "THOÁT";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // lblDecoX1
            // 
            lblDecoX1.AutoSize = true;
            lblDecoX1.Font = new Font("Comic Sans MS", 24F, FontStyle.Bold);
            lblDecoX1.ForeColor = Color.FromArgb(52, 152, 219);
            lblDecoX1.Location = new Point(34, 40);
            lblDecoX1.Name = "lblDecoX1";
            lblDecoX1.Size = new Size(53, 55);
            lblDecoX1.TabIndex = 4;
            lblDecoX1.Text = "X";
            // 
            // lblDecoO1
            // 
            lblDecoO1.AutoSize = true;
            lblDecoO1.Font = new Font("Comic Sans MS", 24F, FontStyle.Bold);
            lblDecoO1.ForeColor = Color.FromArgb(217, 83, 79);
            lblDecoO1.Location = new Point(503, 40);
            lblDecoO1.Name = "lblDecoO1";
            lblDecoO1.Size = new Size(56, 55);
            lblDecoO1.TabIndex = 5;
            lblDecoO1.Text = "O";
            // 
            // lblDecoX2
            // 
            lblDecoX2.AutoSize = true;
            lblDecoX2.Font = new Font("Comic Sans MS", 24F, FontStyle.Bold);
            lblDecoX2.ForeColor = Color.FromArgb(52, 152, 219);
            lblDecoX2.Location = new Point(503, 427);
            lblDecoX2.Name = "lblDecoX2";
            lblDecoX2.Size = new Size(53, 55);
            lblDecoX2.TabIndex = 6;
            lblDecoX2.Text = "X";
            // 
            // lblDecoO2
            // 
            lblDecoO2.AutoSize = true;
            lblDecoO2.Font = new Font("Comic Sans MS", 24F, FontStyle.Bold);
            lblDecoO2.ForeColor = Color.FromArgb(217, 83, 79);
            lblDecoO2.Location = new Point(34, 427);
            lblDecoO2.Name = "lblDecoO2";
            lblDecoO2.Size = new Size(56, 55);
            lblDecoO2.TabIndex = 7;
            lblDecoO2.Text = "O";
            // 
            // lblNamePrompt
            // 
            lblNamePrompt.AutoSize = true;
            lblNamePrompt.Font = new Font("Segoe UI", 9.75F);
            lblNamePrompt.ForeColor = Color.LightGray;
            lblNamePrompt.Location = new Point(122, 180);
            lblNamePrompt.Name = "lblNamePrompt";
            lblNamePrompt.Size = new Size(149, 23);
            lblNamePrompt.TabIndex = 8;
            lblNamePrompt.Text = "Nhập tên hiển thị:";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(33, 37, 41);
            ClientSize = new Size(594, 533);
            Controls.Add(lblNamePrompt);
            Controls.Add(lblDecoO2);
            Controls.Add(lblDecoX2);
            Controls.Add(lblDecoO1);
            Controls.Add(lblDecoX1);
            Controls.Add(btnExit);
            Controls.Add(btnEnterLobby);
            Controls.Add(txtPlayerName);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Caro Client - Đăng nhập";
            Load += LoginForm_Load_1;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtPlayerName;
        private System.Windows.Forms.Button btnEnterLobby;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblDecoX1;
        private System.Windows.Forms.Label lblDecoO1;
        private System.Windows.Forms.Label lblDecoX2;
        private System.Windows.Forms.Label lblDecoO2;
        private System.Windows.Forms.Label lblNamePrompt;
    }
}