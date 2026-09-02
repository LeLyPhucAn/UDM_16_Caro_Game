namespace Client.Forms
{
    partial class RoomForm
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
            lblRoomName = new System.Windows.Forms.Label();
            pnlPlayerX = new System.Windows.Forms.Panel();
            lblPlayerX_Status = new System.Windows.Forms.Label();
            lblPlayerX_Name = new System.Windows.Forms.Label();
            lblTitleX = new System.Windows.Forms.Label();
            pnlPlayerO = new System.Windows.Forms.Panel();
            lblPlayerO_Status = new System.Windows.Forms.Label();
            lblPlayerO_Name = new System.Windows.Forms.Label();
            lblTitleO = new System.Windows.Forms.Label();
            btnStartGame = new System.Windows.Forms.Button();
            btnLeaveRoom = new System.Windows.Forms.Button();
            pnlPlayerX.SuspendLayout();
            pnlPlayerO.SuspendLayout();
            SuspendLayout();
            // 
            // lblRoomName
            // 
            lblRoomName.Dock = System.Windows.Forms.DockStyle.Top;
            lblRoomName.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblRoomName.ForeColor = System.Drawing.Color.DeepSkyBlue;
            lblRoomName.Location = new System.Drawing.Point(0, 0);
            lblRoomName.Name = "lblRoomName";
            lblRoomName.Size = new System.Drawing.Size(634, 60);
            lblRoomName.TabIndex = 0;
            lblRoomName.Text = "PHÒNG: ";
            lblRoomName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlPlayerX
            // 
            pnlPlayerX.BackColor = System.Drawing.Color.FromArgb(45, 48, 54);
            pnlPlayerX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlPlayerX.Controls.Add(lblPlayerX_Status);
            pnlPlayerX.Controls.Add(lblPlayerX_Name);
            pnlPlayerX.Controls.Add(lblTitleX);
            pnlPlayerX.Location = new System.Drawing.Point(50, 100);
            pnlPlayerX.Name = "pnlPlayerX";
            pnlPlayerX.Size = new System.Drawing.Size(230, 180);
            pnlPlayerX.TabIndex = 1;
            // 
            // lblPlayerX_Status
            // 
            lblPlayerX_Status.Dock = System.Windows.Forms.DockStyle.Top;
            lblPlayerX_Status.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            lblPlayerX_Status.ForeColor = System.Drawing.Color.DarkGray;
            lblPlayerX_Status.Location = new System.Drawing.Point(0, 100);
            lblPlayerX_Status.Name = "lblPlayerX_Status";
            lblPlayerX_Status.Size = new System.Drawing.Size(228, 40);
            lblPlayerX_Status.TabIndex = 2;
            lblPlayerX_Status.Text = "Đang chờ...";
            lblPlayerX_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPlayerX_Name
            // 
            lblPlayerX_Name.Dock = System.Windows.Forms.DockStyle.Top;
            lblPlayerX_Name.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblPlayerX_Name.ForeColor = System.Drawing.Color.Gray;
            lblPlayerX_Name.Location = new System.Drawing.Point(0, 40);
            lblPlayerX_Name.Name = "lblPlayerX_Name";
            lblPlayerX_Name.Size = new System.Drawing.Size(228, 60);
            lblPlayerX_Name.TabIndex = 1;
            lblPlayerX_Name.Text = "Đang trống...";
            lblPlayerX_Name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitleX
            // 
            lblTitleX.Dock = System.Windows.Forms.DockStyle.Top;
            lblTitleX.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblTitleX.ForeColor = System.Drawing.Color.DeepSkyBlue;
            lblTitleX.Location = new System.Drawing.Point(0, 0);
            lblTitleX.Name = "lblTitleX";
            lblTitleX.Size = new System.Drawing.Size(228, 40);
            lblTitleX.TabIndex = 0;
            lblTitleX.Text = "Người chơi 1 (X)";
            lblTitleX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlPlayerO
            // 
            pnlPlayerO.BackColor = System.Drawing.Color.FromArgb(45, 48, 54);
            pnlPlayerO.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlPlayerO.Controls.Add(lblPlayerO_Status);
            pnlPlayerO.Controls.Add(lblPlayerO_Name);
            pnlPlayerO.Controls.Add(lblTitleO);
            pnlPlayerO.Location = new System.Drawing.Point(360, 100);
            pnlPlayerO.Name = "pnlPlayerO";
            pnlPlayerO.Size = new System.Drawing.Size(230, 180);
            pnlPlayerO.TabIndex = 2;
            // 
            // lblPlayerO_Status
            // 
            lblPlayerO_Status.Dock = System.Windows.Forms.DockStyle.Top;
            lblPlayerO_Status.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            lblPlayerO_Status.ForeColor = System.Drawing.Color.DarkGray;
            lblPlayerO_Status.Location = new System.Drawing.Point(0, 100);
            lblPlayerO_Status.Name = "lblPlayerO_Status";
            lblPlayerO_Status.Size = new System.Drawing.Size(228, 40);
            lblPlayerO_Status.TabIndex = 2;
            lblPlayerO_Status.Text = "Đang chờ...";
            lblPlayerO_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPlayerO_Name
            // 
            lblPlayerO_Name.Dock = System.Windows.Forms.DockStyle.Top;
            lblPlayerO_Name.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblPlayerO_Name.ForeColor = System.Drawing.Color.Gray;
            lblPlayerO_Name.Location = new System.Drawing.Point(0, 40);
            lblPlayerO_Name.Name = "lblPlayerO_Name";
            lblPlayerO_Name.Size = new System.Drawing.Size(228, 60);
            lblPlayerO_Name.TabIndex = 1;
            lblPlayerO_Name.Text = "Đang trống...";
            lblPlayerO_Name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitleO
            // 
            lblTitleO.Dock = System.Windows.Forms.DockStyle.Top;
            lblTitleO.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblTitleO.ForeColor = System.Drawing.Color.Tomato;
            lblTitleO.Location = new System.Drawing.Point(0, 0);
            lblTitleO.Name = "lblTitleO";
            lblTitleO.Size = new System.Drawing.Size(228, 40);
            lblTitleO.TabIndex = 0;
            lblTitleO.Text = "Người chơi 2 (O)";
            lblTitleO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStartGame
            // 
            btnStartGame.BackColor = System.Drawing.Color.SeaGreen;
            btnStartGame.Cursor = System.Windows.Forms.Cursors.Hand;
            btnStartGame.FlatAppearance.BorderSize = 0;
            btnStartGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnStartGame.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnStartGame.ForeColor = System.Drawing.Color.White;
            btnStartGame.Location = new System.Drawing.Point(140, 320);
            btnStartGame.Name = "btnStartGame";
            btnStartGame.Size = new System.Drawing.Size(200, 50);
            btnStartGame.TabIndex = 3;
            btnStartGame.Text = "BẮT ĐẦU TRẬN";
            btnStartGame.UseVisualStyleBackColor = false;
            btnStartGame.Click += new System.EventHandler(this.BtnStartGame_Click);
            // 
            // btnLeaveRoom
            // 
            btnLeaveRoom.BackColor = System.Drawing.Color.IndianRed;
            btnLeaveRoom.Cursor = System.Windows.Forms.Cursors.Hand;
            btnLeaveRoom.FlatAppearance.BorderSize = 0;
            btnLeaveRoom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnLeaveRoom.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnLeaveRoom.ForeColor = System.Drawing.Color.White;
            btnLeaveRoom.Location = new System.Drawing.Point(360, 320);
            btnLeaveRoom.Name = "btnLeaveRoom";
            btnLeaveRoom.Size = new System.Drawing.Size(140, 50);
            btnLeaveRoom.TabIndex = 4;
            btnLeaveRoom.Text = "RỜI PHÒNG";
            btnLeaveRoom.UseVisualStyleBackColor = false;
            btnLeaveRoom.Click += new System.EventHandler(this.BtnLeaveRoom_Click);
            // 
            // RoomForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(34, 36, 40);
            ClientSize = new System.Drawing.Size(634, 411);
            Controls.Add(btnLeaveRoom);
            Controls.Add(btnStartGame);
            Controls.Add(pnlPlayerO);
            Controls.Add(pnlPlayerX);
            Controls.Add(lblRoomName);
            ForeColor = System.Drawing.Color.White;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "RoomForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Caro Arena - Phòng chờ";
            pnlPlayerX.ResumeLayout(false);
            pnlPlayerO.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblRoomName;
        private System.Windows.Forms.Panel pnlPlayerX;
        private System.Windows.Forms.Label lblPlayerX_Status;
        private System.Windows.Forms.Label lblPlayerX_Name;
        private System.Windows.Forms.Label lblTitleX;
        private System.Windows.Forms.Panel pnlPlayerO;
        private System.Windows.Forms.Label lblPlayerO_Status;
        private System.Windows.Forms.Label lblPlayerO_Name;
        private System.Windows.Forms.Label lblTitleO;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.Button btnLeaveRoom;
    }
}