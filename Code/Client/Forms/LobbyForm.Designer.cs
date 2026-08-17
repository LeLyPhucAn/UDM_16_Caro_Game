namespace Client.Forms
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            pnlTopBar = new Panel();
            btnExitGame = new Button();
            lblPing = new Label();
            lblPlayerName = new Label();
            lblPlayerTitle = new Label();
            lblStatusDot = new Label();
            pnlRightBar = new Panel();
            btnJoinRoom = new Button();
            btnCreateRoom = new Button();
            lblStats = new Label();
            lblActionTitle = new Label();
            pnlMain = new Panel();
            dgvRooms = new DataGridView();
            colRoomId = new DataGridViewTextBoxColumn();
            colRoomName = new DataGridViewTextBoxColumn();
            colPlayerCount = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            lblListDesc = new Label();
            lblListTitle = new Label();
            pnlTopBar.SuspendLayout();
            pnlRightBar.SuspendLayout();
            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRooms).BeginInit();
            SuspendLayout();
            // 
            // pnlTopBar
            // 
            pnlTopBar.BackColor = Color.FromArgb(30, 30, 30);
            pnlTopBar.Controls.Add(btnExitGame);
            pnlTopBar.Controls.Add(lblPing);
            pnlTopBar.Controls.Add(lblPlayerName);
            pnlTopBar.Controls.Add(lblPlayerTitle);
            pnlTopBar.Controls.Add(lblStatusDot);
            pnlTopBar.Dock = DockStyle.Top;
            pnlTopBar.Location = new Point(0, 0);
            pnlTopBar.Name = "pnlTopBar";
            pnlTopBar.Size = new Size(1105, 60);
            pnlTopBar.TabIndex = 0;
            // 
            // btnExitGame
            // 
            btnExitGame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExitGame.BackColor = Color.FromArgb(217, 83, 79);
            btnExitGame.FlatAppearance.BorderSize = 0;
            btnExitGame.FlatStyle = FlatStyle.Flat;
            btnExitGame.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnExitGame.Location = new Point(950, 12);
            btnExitGame.Name = "btnExitGame";
            btnExitGame.Size = new Size(130, 36);
            btnExitGame.TabIndex = 4;
            btnExitGame.Text = "THOÁT GAME";
            btnExitGame.UseVisualStyleBackColor = false;
            // 
            // lblPing
            // 
            lblPing.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPing.AutoSize = true;
            lblPing.ForeColor = Color.Gray;
            lblPing.Location = new Point(760, 20);
            lblPing.Name = "lblPing";
            lblPing.Size = new Size(193, 23);
            lblPing.TabIndex = 3;
            lblPing.Text = "Ping: 14ms | Online: 242";
            // 
            // lblPlayerName
            // 
            lblPlayerName.AutoSize = true;
            lblPlayerName.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            lblPlayerName.ForeColor = Color.DeepSkyBlue;
            lblPlayerName.Location = new Point(145, 19);
            lblPlayerName.Name = "lblPlayerName";
            lblPlayerName.Size = new Size(107, 23);
            lblPlayerName.TabIndex = 2;
            lblPlayerName.Text = "Namdeptrai";
            // 
            // lblPlayerTitle
            // 
            lblPlayerTitle.AutoSize = true;
            lblPlayerTitle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            lblPlayerTitle.Location = new Point(50, 19);
            lblPlayerTitle.Name = "lblPlayerTitle";
            lblPlayerTitle.Size = new Size(105, 23);
            lblPlayerTitle.TabIndex = 1;
            lblPlayerTitle.Text = "Người chơi:";
            // 
            // lblStatusDot
            // 
            lblStatusDot.AutoSize = true;
            lblStatusDot.Font = new Font("Segoe UI", 16F);
            lblStatusDot.ForeColor = Color.LimeGreen;
            lblStatusDot.Location = new Point(20, 11);
            lblStatusDot.Name = "lblStatusDot";
            lblStatusDot.Size = new Size(33, 37);
            lblStatusDot.TabIndex = 0;
            lblStatusDot.Text = "●";
            // 
            // pnlRightBar
            // 
            pnlRightBar.BackColor = Color.FromArgb(42, 44, 48);
            pnlRightBar.Controls.Add(btnJoinRoom);
            pnlRightBar.Controls.Add(btnCreateRoom);
            pnlRightBar.Controls.Add(lblStats);
            pnlRightBar.Controls.Add(lblActionTitle);
            pnlRightBar.Dock = DockStyle.Right;
            pnlRightBar.Location = new Point(805, 60);
            pnlRightBar.Name = "pnlRightBar";
            pnlRightBar.Size = new Size(300, 633);
            pnlRightBar.TabIndex = 1;
            // 
            // btnJoinRoom
            // 
            btnJoinRoom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnJoinRoom.BackColor = Color.FromArgb(50, 50, 55);
            btnJoinRoom.FlatAppearance.BorderColor = Color.Gray;
            btnJoinRoom.FlatStyle = FlatStyle.Flat;
            btnJoinRoom.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnJoinRoom.Location = new Point(25, 540);
            btnJoinRoom.Name = "btnJoinRoom";
            btnJoinRoom.Size = new Size(250, 50);
            btnJoinRoom.TabIndex = 3;
            btnJoinRoom.Text = "THAM GIA PHÒNG";
            btnJoinRoom.UseVisualStyleBackColor = false;
            // 
            // btnCreateRoom
            // 
            btnCreateRoom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCreateRoom.BackColor = Color.FromArgb(0, 120, 215);
            btnCreateRoom.FlatAppearance.BorderSize = 0;
            btnCreateRoom.FlatStyle = FlatStyle.Flat;
            btnCreateRoom.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCreateRoom.Location = new Point(25, 475);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(250, 50);
            btnCreateRoom.TabIndex = 2;
            btnCreateRoom.Text = "TẠO PHÒNG MỚI";
            btnCreateRoom.UseVisualStyleBackColor = false;
            // 
            // lblStats
            // 
            lblStats.AutoSize = true;
            lblStats.ForeColor = Color.DarkGray;
            lblStats.Location = new Point(21, 75);
            lblStats.Name = "lblStats";
            lblStats.Size = new Size(146, 69);
            lblStats.TabIndex = 1;
            lblStats.Text = "Phòng trống: 1\nĐang chờ ghép: 2\nĐang thi đấu: 2";
            // 
            // lblActionTitle
            // 
            lblActionTitle.AutoSize = true;
            lblActionTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblActionTitle.Location = new Point(20, 25);
            lblActionTitle.Name = "lblActionTitle";
            lblActionTitle.Size = new Size(193, 28);
            lblActionTitle.TabIndex = 0;
            lblActionTitle.Text = "THAO TÁC NHANH";
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(dgvRooms);
            pnlMain.Controls.Add(lblListDesc);
            pnlMain.Controls.Add(lblListTitle);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 60);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(805, 633);
            pnlMain.TabIndex = 2;
            // 
            // dgvRooms
            // 
            dgvRooms.AllowUserToAddRows = false;
            dgvRooms.AllowUserToDeleteRows = false;
            dgvRooms.AllowUserToResizeRows = false;
            dgvRooms.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvRooms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRooms.BackgroundColor = Color.FromArgb(34, 36, 40);
            dgvRooms.BorderStyle = BorderStyle.None;
            dgvRooms.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRooms.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(42, 44, 48);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(42, 44, 48);
            dataGridViewCellStyle1.SelectionForeColor = Color.White;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvRooms.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvRooms.ColumnHeadersHeight = 45;
            dgvRooms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvRooms.Columns.AddRange(new DataGridViewColumn[] { colRoomId, colRoomName, colPlayerCount, colStatus });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(34, 36, 40);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(50, 52, 56);
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvRooms.DefaultCellStyle = dataGridViewCellStyle2;
            dgvRooms.EnableHeadersVisualStyles = false;
            dgvRooms.GridColor = Color.FromArgb(60, 60, 60);
            dgvRooms.Location = new Point(30, 80);
            dgvRooms.Name = "dgvRooms";
            dgvRooms.ReadOnly = true;
            dgvRooms.RowHeadersVisible = false;
            dgvRooms.RowHeadersWidth = 51;
            dgvRooms.RowTemplate.Height = 45;
            dgvRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRooms.Size = new Size(745, 520);
            dgvRooms.TabIndex = 2;
            // 
            // colRoomId
            // 
            colRoomId.FillWeight = 20F;
            colRoomId.HeaderText = "Mã Phòng";
            colRoomId.MinimumWidth = 6;
            colRoomId.Name = "colRoomId";
            colRoomId.ReadOnly = true;
            // 
            // colRoomName
            // 
            colRoomName.FillWeight = 40F;
            colRoomName.HeaderText = "Tên Phòng";
            colRoomName.MinimumWidth = 6;
            colRoomName.Name = "colRoomName";
            colRoomName.ReadOnly = true;
            // 
            // colPlayerCount
            // 
            colPlayerCount.FillWeight = 20F;
            colPlayerCount.HeaderText = "Số Người";
            colPlayerCount.MinimumWidth = 6;
            colPlayerCount.Name = "colPlayerCount";
            colPlayerCount.ReadOnly = true;
            // 
            // colStatus
            // 
            colStatus.FillWeight = 20F;
            colStatus.HeaderText = "Trạng Thái";
            colStatus.MinimumWidth = 6;
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            // 
            // lblListDesc
            // 
            lblListDesc.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblListDesc.AutoSize = true;
            lblListDesc.ForeColor = Color.Gray;
            lblListDesc.Location = new Point(551, 33);
            lblListDesc.Name = "lblListDesc";
            lblListDesc.Size = new Size(224, 23);
            lblListDesc.TabIndex = 1;
            lblListDesc.Text = "Hiển thị 5 phòng hoạt động";
            // 
            // lblListTitle
            // 
            lblListTitle.AutoSize = true;
            lblListTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblListTitle.Location = new Point(24, 25);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Size = new Size(309, 32);
            lblListTitle.TabIndex = 0;
            lblListTitle.Text = "DANH SÁCH PHÒNG CHỜ";
            // 
            // LobbyForm
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(34, 36, 40);
            ClientSize = new Size(1105, 693);
            Controls.Add(pnlMain);
            Controls.Add(pnlRightBar);
            Controls.Add(pnlTopBar);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.White;
            MinimumSize = new Size(950, 600);
            Name = "LobbyForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Caro Arena - Lobby Browser";
            Load += LobbyForm_Load;
            pnlTopBar.ResumeLayout(false);
            pnlTopBar.PerformLayout();
            pnlRightBar.ResumeLayout(false);
            pnlRightBar.PerformLayout();
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRooms).EndInit();
            ResumeLayout(false);

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