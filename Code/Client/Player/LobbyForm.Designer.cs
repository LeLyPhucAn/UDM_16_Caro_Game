namespace Player
{
    partial class LobbyForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTopBar = new System.Windows.Forms.Panel();
            this.btnExitGame = new System.Windows.Forms.Button();
            this.lblPing = new System.Windows.Forms.Label();
            this.lblPlayerName = new System.Windows.Forms.Label();
            this.lblPlayerTitle = new System.Windows.Forms.Label();
            this.lblStatusDot = new System.Windows.Forms.Label();
            this.pnlRightBar = new System.Windows.Forms.Panel();
            this.btnJoinRoom = new System.Windows.Forms.Button();
            this.btnCreateRoom = new System.Windows.Forms.Button();
            this.lblStats = new System.Windows.Forms.Label();
            this.lblActionTitle = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.dgvRooms = new System.Windows.Forms.DataGridView();
            this.colRoomId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRoomName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPlayerCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblListDesc = new System.Windows.Forms.Label();
            this.lblListTitle = new System.Windows.Forms.Label();
            this.pnlTopBar.SuspendLayout();
            this.pnlRightBar.SuspendLayout();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRooms)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlTopBar.Controls.Add(this.btnExitGame);
            this.pnlTopBar.Controls.Add(this.lblPing);
            this.pnlTopBar.Controls.Add(this.lblPlayerName);
            this.pnlTopBar.Controls.Add(this.lblPlayerTitle);
            this.pnlTopBar.Controls.Add(this.lblStatusDot);
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Size = new System.Drawing.Size(1105, 60);
            this.pnlTopBar.TabIndex = 0;
            // 
            // btnExitGame
            // 
            this.btnExitGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExitGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.btnExitGame.FlatAppearance.BorderSize = 0;
            this.btnExitGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExitGame.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExitGame.Location = new System.Drawing.Point(950, 12);
            this.btnExitGame.Name = "btnExitGame";
            this.btnExitGame.Size = new System.Drawing.Size(130, 36);
            this.btnExitGame.TabIndex = 4;
            this.btnExitGame.Text = "THOÁT GAME";
            this.btnExitGame.UseVisualStyleBackColor = false;
            // 
            // lblPing
            // 
            this.lblPing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPing.AutoSize = true;
            this.lblPing.ForeColor = System.Drawing.Color.Gray;
            this.lblPing.Location = new System.Drawing.Point(760, 20);
            this.lblPing.Name = "lblPing";
            this.lblPing.Size = new System.Drawing.Size(170, 23);
            this.lblPing.TabIndex = 3;
            this.lblPing.Text = "Ping: 14ms | Online: 242";
            // 
            // lblPlayerName
            // 
            this.lblPlayerName.AutoSize = true;
            this.lblPlayerName.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold);
            this.lblPlayerName.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.lblPlayerName.Location = new System.Drawing.Point(145, 19);
            this.lblPlayerName.Name = "lblPlayerName";
            this.lblPlayerName.Size = new System.Drawing.Size(111, 23);
            this.lblPlayerName.TabIndex = 2;
            this.lblPlayerName.Text = "Namdeptrai";
            // 
            // lblPlayerTitle
            // 
            this.lblPlayerTitle.AutoSize = true;
            this.lblPlayerTitle.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold);
            this.lblPlayerTitle.Location = new System.Drawing.Point(50, 19);
            this.lblPlayerTitle.Name = "lblPlayerTitle";
            this.lblPlayerTitle.Size = new System.Drawing.Size(102, 23);
            this.lblPlayerTitle.TabIndex = 1;
            this.lblPlayerTitle.Text = "Người chơi:";
            // 
            // lblStatusDot
            // 
            this.lblStatusDot.AutoSize = true;
            this.lblStatusDot.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.lblStatusDot.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblStatusDot.Location = new System.Drawing.Point(20, 11);
            this.lblStatusDot.Name = "lblStatusDot";
            this.lblStatusDot.Size = new System.Drawing.Size(34, 37);
            this.lblStatusDot.TabIndex = 0;
            this.lblStatusDot.Text = "●";
            // 
            // pnlRightBar
            // 
            this.pnlRightBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(44)))), ((int)(((byte)(48)))));
            this.pnlRightBar.Controls.Add(this.btnJoinRoom);
            this.pnlRightBar.Controls.Add(this.btnCreateRoom);
            this.pnlRightBar.Controls.Add(this.lblStats);
            this.pnlRightBar.Controls.Add(this.lblActionTitle);
            this.pnlRightBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRightBar.Location = new System.Drawing.Point(805, 60);
            this.pnlRightBar.Name = "pnlRightBar";
            this.pnlRightBar.Size = new System.Drawing.Size(300, 633);
            this.pnlRightBar.TabIndex = 1;
            // 
            // btnJoinRoom
            // 
            this.btnJoinRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnJoinRoom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(55)))));
            this.btnJoinRoom.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnJoinRoom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJoinRoom.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnJoinRoom.Location = new System.Drawing.Point(25, 540);
            this.btnJoinRoom.Name = "btnJoinRoom";
            this.btnJoinRoom.Size = new System.Drawing.Size(250, 50);
            this.btnJoinRoom.TabIndex = 3;
            this.btnJoinRoom.Text = "THAM GIA PHÒNG";
            this.btnJoinRoom.UseVisualStyleBackColor = false;
            // 
            // btnCreateRoom
            // 
            this.btnCreateRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreateRoom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnCreateRoom.FlatAppearance.BorderSize = 0;
            this.btnCreateRoom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateRoom.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCreateRoom.Location = new System.Drawing.Point(25, 475);
            this.btnCreateRoom.Name = "btnCreateRoom";
            this.btnCreateRoom.Size = new System.Drawing.Size(250, 50);
            this.btnCreateRoom.TabIndex = 2;
            this.btnCreateRoom.Text = "TẠO PHÒNG MỚI";
            this.btnCreateRoom.UseVisualStyleBackColor = false;
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.ForeColor = System.Drawing.Color.DarkGray;
            this.lblStats.Location = new System.Drawing.Point(21, 75);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(155, 69);
            this.lblStats.TabIndex = 1;
            this.lblStats.Text = "Phòng trống: 1\nĐang chờ ghép: 2\nĐang thi đấu: 2";
            // 
            // lblActionTitle
            // 
            this.lblActionTitle.AutoSize = true;
            this.lblActionTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblActionTitle.Location = new System.Drawing.Point(20, 25);
            this.lblActionTitle.Name = "lblActionTitle";
            this.lblActionTitle.Size = new System.Drawing.Size(183, 28);
            this.lblActionTitle.TabIndex = 0;
            this.lblActionTitle.Text = "THAO TÁC NHANH";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.dgvRooms);
            this.pnlMain.Controls.Add(this.lblListDesc);
            this.pnlMain.Controls.Add(this.lblListTitle);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 60);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(805, 633);
            this.pnlMain.TabIndex = 2;
            // 
            // dgvRooms
            // 
            this.dgvRooms.AllowUserToAddRows = false;
            this.dgvRooms.AllowUserToDeleteRows = false;
            this.dgvRooms.AllowUserToResizeRows = false;
            this.dgvRooms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRooms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRooms.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(40)))));
            this.dgvRooms.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvRooms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvRooms.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(44)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(44)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRooms.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRooms.ColumnHeadersHeight = 45;
            this.dgvRooms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvRooms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRoomId,
            this.colRoomName,
            this.colPlayerCount,
            this.colStatus});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(40)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(52)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRooms.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvRooms.EnableHeadersVisualStyles = false;
            this.dgvRooms.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.dgvRooms.Location = new System.Drawing.Point(30, 80);
            this.dgvRooms.Name = "dgvRooms";
            this.dgvRooms.ReadOnly = true;
            this.dgvRooms.RowHeadersVisible = false;
            this.dgvRooms.RowHeadersWidth = 51;
            this.dgvRooms.RowTemplate.Height = 45;
            this.dgvRooms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRooms.Size = new System.Drawing.Size(745, 520);
            this.dgvRooms.TabIndex = 2;
            // 
            // colRoomId
            // 
            this.colRoomId.FillWeight = 20F;
            this.colRoomId.HeaderText = "Mã Phòng";
            this.colRoomId.Name = "colRoomId";
            this.colRoomId.ReadOnly = true;
            // 
            // colRoomName
            // 
            this.colRoomName.FillWeight = 40F;
            this.colRoomName.HeaderText = "Tên Phòng";
            this.colRoomName.Name = "colRoomName";
            this.colRoomName.ReadOnly = true;
            // 
            // colPlayerCount
            // 
            this.colPlayerCount.FillWeight = 20F;
            this.colPlayerCount.HeaderText = "Số Người";
            this.colPlayerCount.Name = "colPlayerCount";
            this.colPlayerCount.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.FillWeight = 20F;
            this.colStatus.HeaderText = "Trạng Thái";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // lblListDesc
            // 
            this.lblListDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblListDesc.AutoSize = true;
            this.lblListDesc.ForeColor = System.Drawing.Color.Gray;
            this.lblListDesc.Location = new System.Drawing.Point(582, 33);
            this.lblListDesc.Name = "lblListDesc";
            this.lblListDesc.Size = new System.Drawing.Size(193, 23);
            this.lblListDesc.TabIndex = 1;
            this.lblListDesc.Text = "Hiển thị 5 phòng hoạt động";
            // 
            // lblListTitle
            // 
            this.lblListTitle.AutoSize = true;
            this.lblListTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblListTitle.Location = new System.Drawing.Point(24, 25);
            this.lblListTitle.Name = "lblListTitle";
            this.lblListTitle.Size = new System.Drawing.Size(288, 32);
            this.lblListTitle.TabIndex = 0;
            this.lblListTitle.Text = "DANH SÁCH PHÒNG CHỜ";
            // 
            // FormLobby
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1105, 693);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlRightBar);
            this.Controls.Add(this.pnlTopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(950, 600);
            this.Name = "FormLobby";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Caro Arena - Lobby Browser";
            this.pnlTopBar.ResumeLayout(false);
            this.pnlTopBar.PerformLayout();
            this.pnlRightBar.ResumeLayout(false);
            this.pnlRightBar.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRooms)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopBar;
        private System.Windows.Forms.Label lblStatusDot;
        private System.Windows.Forms.Label lblPlayerTitle;
        private System.Windows.Forms.Label lblPlayerName;
        private System.Windows.Forms.Label lblPing;
        private System.Windows.Forms.Button btnExitGame;

        private System.Windows.Forms.Panel pnlRightBar;
        private System.Windows.Forms.Label lblActionTitle;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.Button btnCreateRoom;
        private System.Windows.Forms.Button btnJoinRoom;

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.Label lblListDesc;
        private System.Windows.Forms.DataGridView dgvRooms;

        private System.Windows.Forms.DataGridViewTextBoxColumn colRoomId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRoomName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPlayerCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
    }
}