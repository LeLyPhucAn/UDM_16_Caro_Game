namespace Client.Forms
{
    partial class SpectatorForm
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
            lblPlayerX = new Label();
            lblPlayerO = new Label();
            lblStatus = new Label();
            btnExit = new Button();
            boardControl1 = new Client.Controls.BoardControl();
            SuspendLayout();
            // 
            // lblPlayerX
            // 
            lblPlayerX.AutoSize = true;
            lblPlayerX.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPlayerX.ForeColor = Color.Red;
            lblPlayerX.Location = new Point(23, 27);
            lblPlayerX.Name = "lblPlayerX";
            lblPlayerX.Size = new Size(100, 28);
            lblPlayerX.TabIndex = 0;
            lblPlayerX.Text = "Quân X: -";
            // 
            // lblPlayerO
            // 
            lblPlayerO.AutoSize = true;
            lblPlayerO.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPlayerO.ForeColor = Color.Blue;
            lblPlayerO.Location = new Point(709, 27);
            lblPlayerO.Name = "lblPlayerO";
            lblPlayerO.Size = new Size(102, 28);
            lblPlayerO.TabIndex = 1;
            lblPlayerO.Text = "Quân O: -";
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblStatus.Location = new Point(229, 20);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(457, 40);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Đang kết nối để xem trận đấu...";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.IndianRed;
            btnExit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(731, 693);
            btnExit.Margin = new Padding(3, 4, 3, 4);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(137, 53);
            btnExit.TabIndex = 3;
            btnExit.Text = "THOÁT";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // boardControl1
            // 
            boardControl1.BackColor = Color.White;
            boardControl1.BorderStyle = BorderStyle.FixedSingle;
            boardControl1.Location = new Point(200, 93);
            boardControl1.Margin = new Padding(3, 4, 3, 4);
            boardControl1.Name = "boardControl1";
            boardControl1.Size = new Size(514, 599);
            boardControl1.TabIndex = 4;
            // 
            // SpectatorForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(914, 800);
            Controls.Add(boardControl1);
            Controls.Add(btnExit);
            Controls.Add(lblStatus);
            Controls.Add(lblPlayerO);
            Controls.Add(lblPlayerX);
            Margin = new Padding(3, 4, 3, 4);
            Name = "SpectatorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Khán giả - Game Caro Online";
            Load += SpectatorForm_Load_1;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblPlayerX;
        private System.Windows.Forms.Label lblPlayerO;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnExit;
        private Client.Controls.BoardControl boardControl1; // Khai báo control bàn cờ
    }
}