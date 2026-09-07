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
            this.lblPlayerX = new System.Windows.Forms.Label();
            this.lblPlayerO = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            // Khởi tạo BoardControl từ namespace Client.Controls
            this.boardControl1 = new Client.Controls.BoardControl();

            this.SuspendLayout();

            // 
            // lblPlayerX
            // 
            this.lblPlayerX.AutoSize = true;
            this.lblPlayerX.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPlayerX.ForeColor = System.Drawing.Color.Red;
            this.lblPlayerX.Location = new System.Drawing.Point(20, 20);
            this.lblPlayerX.Name = "lblPlayerX";
            this.lblPlayerX.Size = new System.Drawing.Size(76, 21);
            this.lblPlayerX.TabIndex = 0;
            this.lblPlayerX.Text = "Quân X: -";

            // 
            // lblPlayerO
            // 
            this.lblPlayerO.AutoSize = true;
            this.lblPlayerO.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPlayerO.ForeColor = System.Drawing.Color.Blue;
            this.lblPlayerO.Location = new System.Drawing.Point(620, 20);
            this.lblPlayerO.Name = "lblPlayerO";
            this.lblPlayerO.Size = new System.Drawing.Size(78, 21);
            this.lblPlayerO.TabIndex = 1;
            this.lblPlayerO.Text = "Quân O: -";

            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.Location = new System.Drawing.Point(200, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(400, 30);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Đang kết nối để xem trận đấu...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.IndianRed;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(640, 520);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(120, 40);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "THOÁT";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // 
            // boardControl1
            // 
            this.boardControl1.BackColor = System.Drawing.Color.White;
            this.boardControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boardControl1.Location = new System.Drawing.Point(175, 70);
            this.boardControl1.Name = "boardControl1";
            this.boardControl1.Size = new System.Drawing.Size(450, 450);
            this.boardControl1.TabIndex = 4;

            // 
            // SpectatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.boardControl1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblPlayerO);
            this.Controls.Add(this.lblPlayerX);
            this.Name = "SpectatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Khán giả - Game Caro Online";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblPlayerX;
        private System.Windows.Forms.Label lblPlayerO;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnExit;
        private Client.Controls.BoardControl boardControl1; // Khai báo control bàn cờ
    }
}