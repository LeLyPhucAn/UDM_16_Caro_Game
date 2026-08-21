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
            // 1. KHỞI TẠO TẤT CẢ CÁC ĐỐI TƯỢNG TRƯỚC (Tránh lỗi Null)
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtPlayerName = new System.Windows.Forms.TextBox();
            this.btnEnterLobby = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblDecoX1 = new System.Windows.Forms.Label();
            this.lblDecoO1 = new System.Windows.Forms.Label();
            this.lblDecoX2 = new System.Windows.Forms.Label();
            this.lblDecoO2 = new System.Windows.Forms.Label();
            this.lblNamePrompt = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.lblTitle.Location = new System.Drawing.Point(95, 45);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(332, 45);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "GAME CARO ONLINE";

            // 
            // txtPlayerName
            // 
            this.txtPlayerName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtPlayerName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPlayerName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtPlayerName.ForeColor = System.Drawing.Color.White;
            this.txtPlayerName.Location = new System.Drawing.Point(110, 160);
            this.txtPlayerName.Name = "txtPlayerName";
            this.txtPlayerName.PlaceholderText = "Tên (3-15 ký tự, không dấu cách)";
            this.txtPlayerName.Size = new System.Drawing.Size(300, 29);
            this.txtPlayerName.TabIndex = 1;
            this.txtPlayerName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            // 
            // btnEnterLobby
            // 
            this.btnEnterLobby.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnEnterLobby.FlatAppearance.BorderSize = 0;
            this.btnEnterLobby.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnterLobby.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnEnterLobby.ForeColor = System.Drawing.Color.White;
            this.btnEnterLobby.Location = new System.Drawing.Point(110, 225);
            this.btnEnterLobby.Name = "btnEnterLobby";
            this.btnEnterLobby.Size = new System.Drawing.Size(300, 45);
            this.btnEnterLobby.TabIndex = 2;
            this.btnEnterLobby.Text = "VÀO SẢNH CHỜ";
            this.btnEnterLobby.UseVisualStyleBackColor = false;

            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(110, 285);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(300, 45);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "THOÁT";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // 
            // lblDecoX1
            // 
            this.lblDecoX1.AutoSize = true;
            this.lblDecoX1.Font = new System.Drawing.Font("Comic Sans MS", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDecoX1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.lblDecoX1.Location = new System.Drawing.Point(30, 30);
            this.lblDecoX1.Name = "lblDecoX1";
            this.lblDecoX1.Size = new System.Drawing.Size(43, 45);
            this.lblDecoX1.TabIndex = 4;
            this.lblDecoX1.Text = "X";

            // 
            // lblDecoO1
            // 
            this.lblDecoO1.AutoSize = true;
            this.lblDecoO1.Font = new System.Drawing.Font("Comic Sans MS", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDecoO1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.lblDecoO1.Location = new System.Drawing.Point(440, 30);
            this.lblDecoO1.Name = "lblDecoO1";
            this.lblDecoO1.Size = new System.Drawing.Size(45, 45);
            this.lblDecoO1.TabIndex = 5;
            this.lblDecoO1.Text = "O";

            // 
            // lblDecoX2
            // 
            this.lblDecoX2.AutoSize = true;
            this.lblDecoX2.Font = new System.Drawing.Font("Comic Sans MS", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDecoX2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.lblDecoX2.Location = new System.Drawing.Point(440, 320);
            this.lblDecoX2.Name = "lblDecoX2";
            this.lblDecoX2.Size = new System.Drawing.Size(43, 45);
            this.lblDecoX2.TabIndex = 6;
            this.lblDecoX2.Text = "X";

            // 
            // lblDecoO2
            // 
            this.lblDecoO2.AutoSize = true;
            this.lblDecoO2.Font = new System.Drawing.Font("Comic Sans MS", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDecoO2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.lblDecoO2.Location = new System.Drawing.Point(30, 320);
            this.lblDecoO2.Name = "lblDecoO2";
            this.lblDecoO2.Size = new System.Drawing.Size(45, 45);
            this.lblDecoO2.TabIndex = 7;
            this.lblDecoO2.Text = "O";

            // 
            // lblNamePrompt
            // 
            this.lblNamePrompt.AutoSize = true;
            this.lblNamePrompt.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblNamePrompt.ForeColor = System.Drawing.Color.LightGray;
            this.lblNamePrompt.Location = new System.Drawing.Point(107, 135);
            this.lblNamePrompt.Name = "lblNamePrompt";
            this.lblNamePrompt.Size = new System.Drawing.Size(110, 17);
            this.lblNamePrompt.TabIndex = 8;
            this.lblNamePrompt.Text = "Nhập tên hiển thị:";

            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.ClientSize = new System.Drawing.Size(520, 400);
            this.Controls.Add(this.lblNamePrompt);
            this.Controls.Add(this.lblDecoO2);
            this.Controls.Add(this.lblDecoX2);
            this.Controls.Add(this.lblDecoO1);
            this.Controls.Add(this.lblDecoX1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEnterLobby);
            this.Controls.Add(this.txtPlayerName);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Caro Client - Đăng nhập";
            this.ResumeLayout(false);
            this.PerformLayout();
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