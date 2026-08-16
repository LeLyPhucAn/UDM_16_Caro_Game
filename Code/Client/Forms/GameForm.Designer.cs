namespace Client.Forms
{
    partial class GameForm
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnLeaveRoom = new System.Windows.Forms.Button();
            this.lblSpectators = new System.Windows.Forms.Label();
            this.lblBadge = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlChat = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtChatInput = new System.Windows.Forms.TextBox();
            this.rtbChatHistory = new System.Windows.Forms.RichTextBox();
            this.lblChatTitle = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblTurnValue = new System.Windows.Forms.Label();
            this.lblTurnText = new System.Windows.Forms.Label();
            this.lblPlayerO_Status = new System.Windows.Forms.Label();
            this.lblPlayerO = new System.Windows.Forms.Label();
            this.lblPlayerX_Status = new System.Windows.Forms.Label();
            this.lblPlayerX = new System.Windows.Forms.Label();
            this.lblStatusTitle = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlBoard = new System.Windows.Forms.Panel();
            this.pnlTop.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlChat.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblSpectators);
            this.pnlTop.Controls.Add(this.lblBadge);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1150, 70);
            this.pnlTop.TabIndex = 0;
            // 
            // btnLeaveRoom
            // 
            this.btnLeaveRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeaveRoom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.btnLeaveRoom.FlatAppearance.BorderSize = 0;
            this.btnLeaveRoom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeaveRoom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLeaveRoom.Location = new System.Drawing.Point(900, 18);
            this.btnLeaveRoom.Name = "btnLeaveRoom";
            this.btnLeaveRoom.Size = new System.Drawing.Size(120, 35);
            this.btnLeaveRoom.TabIndex = 3;
            this.btnLeaveRoom.Text = "RỜI PHÒNG";
            this.btnLeaveRoom.UseVisualStyleBackColor = false;
            this.btnLeaveRoom.Click += new System.EventHandler(this.btnLeaveRoom_Click);

            // Thêm nút vào pnlTop
            this.pnlTop.Controls.Add(this.btnLeaveRoom);
            // 
            // lblSpectators
            // 
            this.lblSpectators.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpectators.AutoSize = true;
            this.lblSpectators.ForeColor = System.Drawing.Color.Gray;
            this.lblSpectators.Location = new System.Drawing.Point(1040, 25);
            this.lblSpectators.Name = "lblSpectators";
            this.lblSpectators.Size = new System.Drawing.Size(91, 23);
            this.lblSpectators.TabIndex = 2;
            this.lblSpectators.Text = "Khán giả: 4";
            // 
            // lblBadge
            // 
            this.lblBadge.AutoSize = true;
            this.lblBadge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.lblBadge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblBadge.Location = new System.Drawing.Point(260, 26);
            this.lblBadge.Name = "lblBadge";
            this.lblBadge.Padding = new System.Windows.Forms.Padding(5);
            this.lblBadge.Size = new System.Drawing.Size(130, 30);
            this.lblBadge.TabIndex = 1;
            this.lblBadge.Text = "Standard 10x10";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(227, 37);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "PHÒNG THI ĐẤU";
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.pnlChat);
            this.pnlRight.Controls.Add(this.pnlStatus);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(800, 70);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(15);
            this.pnlRight.Size = new System.Drawing.Size(350, 680);
            this.pnlRight.TabIndex = 1;
            // 
            // pnlChat
            // 
            this.pnlChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(44)))), ((int)(((byte)(48)))));
            this.pnlChat.Controls.Add(this.btnSend);
            this.pnlChat.Controls.Add(this.txtChatInput);
            this.pnlChat.Controls.Add(this.rtbChatHistory);
            this.pnlChat.Controls.Add(this.lblChatTitle);
            this.pnlChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChat.Location = new System.Drawing.Point(15, 205);
            this.pnlChat.Name = "pnlChat";
            this.pnlChat.Padding = new System.Windows.Forms.Padding(15);
            this.pnlChat.Size = new System.Drawing.Size(320, 460);
            this.pnlChat.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSend.Location = new System.Drawing.Point(235, 405);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(70, 40);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "GỬI";
            this.btnSend.UseVisualStyleBackColor = false;
            // 
            // txtChatInput
            // 
            this.txtChatInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChatInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(40)))));
            this.txtChatInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChatInput.ForeColor = System.Drawing.Color.White;
            this.txtChatInput.Location = new System.Drawing.Point(15, 405);
            this.txtChatInput.Multiline = true;
            this.txtChatInput.Name = "txtChatInput";
            this.txtChatInput.Size = new System.Drawing.Size(210, 40);
            this.txtChatInput.TabIndex = 2;
            // 
            // rtbChatHistory
            // 
            this.rtbChatHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbChatHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(40)))));
            this.rtbChatHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbChatHistory.ForeColor = System.Drawing.Color.White;
            this.rtbChatHistory.Location = new System.Drawing.Point(15, 50);
            this.rtbChatHistory.Name = "rtbChatHistory";
            this.rtbChatHistory.ReadOnly = true;
            this.rtbChatHistory.Size = new System.Drawing.Size(290, 340);
            this.rtbChatHistory.TabIndex = 1;
            this.rtbChatHistory.Text = "";
            // 
            // lblChatTitle
            // 
            this.lblChatTitle.AutoSize = true;
            this.lblChatTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblChatTitle.Location = new System.Drawing.Point(15, 15);
            this.lblChatTitle.Name = "lblChatTitle";
            this.lblChatTitle.Size = new System.Drawing.Size(176, 25);
            this.lblChatTitle.TabIndex = 0;
            this.lblChatTitle.Text = "KÊNH TRÒ CHUYỆN";
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(44)))), ((int)(((byte)(48)))));
            this.pnlStatus.Controls.Add(this.lblTurnValue);
            this.pnlStatus.Controls.Add(this.lblTurnText);
            this.pnlStatus.Controls.Add(this.lblPlayerO_Status);
            this.pnlStatus.Controls.Add(this.lblPlayerO);
            this.pnlStatus.Controls.Add(this.lblPlayerX_Status);
            this.pnlStatus.Controls.Add(this.lblPlayerX);
            this.pnlStatus.Controls.Add(this.lblStatusTitle);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStatus.Location = new System.Drawing.Point(15, 15);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(320, 190);
            this.pnlStatus.TabIndex = 0;
            // 
            // lblTurnValue
            // 
            this.lblTurnValue.AutoSize = true;
            this.lblTurnValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(70)))), ((int)(((byte)(100)))));
            this.lblTurnValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTurnValue.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.lblTurnValue.Location = new System.Drawing.Point(150, 140);
            this.lblTurnValue.Name = "lblTurnValue";
            this.lblTurnValue.Padding = new System.Windows.Forms.Padding(5);
            this.lblTurnValue.Size = new System.Drawing.Size(107, 33);
            this.lblTurnValue.TabIndex = 6;
            this.lblTurnValue.Text = "X (Nam123)";
            // 
            // lblTurnText
            // 
            this.lblTurnText.AutoSize = true;
            this.lblTurnText.Location = new System.Drawing.Point(15, 145);
            this.lblTurnText.Name = "lblTurnText";
            this.lblTurnText.Size = new System.Drawing.Size(128, 23);
            this.lblTurnText.TabIndex = 5;
            this.lblTurnText.Text = "Lượt đi hiện tại:";
            // 
            // lblPlayerO_Status
            // 
            this.lblPlayerO_Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPlayerO_Status.AutoSize = true;
            this.lblPlayerO_Status.ForeColor = System.Drawing.Color.Gray;
            this.lblPlayerO_Status.Location = new System.Drawing.Point(235, 100);
            this.lblPlayerO_Status.Name = "lblPlayerO_Status";
            this.lblPlayerO_Status.Size = new System.Drawing.Size(78, 23);
            this.lblPlayerO_Status.TabIndex = 4;
            this.lblPlayerO_Status.Text = "Sẵn sàng";
            // 
            // lblPlayerO
            // 
            this.lblPlayerO.AutoSize = true;
            this.lblPlayerO.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPlayerO.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.lblPlayerO.Location = new System.Drawing.Point(15, 95);
            this.lblPlayerO.Name = "lblPlayerO";
            this.lblPlayerO.Size = new System.Drawing.Size(126, 28);
            this.lblPlayerO.TabIndex = 3;
            this.lblPlayerO.Text = "O: Minh456";
            // 
            // lblPlayerX_Status
            // 
            this.lblPlayerX_Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPlayerX_Status.AutoSize = true;
            this.lblPlayerX_Status.ForeColor = System.Drawing.Color.Gray;
            this.lblPlayerX_Status.Location = new System.Drawing.Point(265, 65);
            this.lblPlayerX_Status.Name = "lblPlayerX_Status";
            this.lblPlayerX_Status.Size = new System.Drawing.Size(45, 23);
            this.lblPlayerX_Status.TabIndex = 2;
            this.lblPlayerX_Status.Text = "Host";
            // 
            // lblPlayerX
            // 
            this.lblPlayerX.AutoSize = true;
            this.lblPlayerX.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPlayerX.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.lblPlayerX.Location = new System.Drawing.Point(15, 60);
            this.lblPlayerX.Name = "lblPlayerX";
            this.lblPlayerX.Size = new System.Drawing.Size(117, 28);
            this.lblPlayerX.TabIndex = 1;
            this.lblPlayerX.Text = "X: Nam123";
            // 
            // lblStatusTitle
            // 
            this.lblStatusTitle.AutoSize = true;
            this.lblStatusTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatusTitle.Location = new System.Drawing.Point(15, 15);
            this.lblStatusTitle.Name = "lblStatusTitle";
            this.lblStatusTitle.Size = new System.Drawing.Size(166, 25);
            this.lblStatusTitle.TabIndex = 0;
            this.lblStatusTitle.Text = "TRẠNG THÁI ĐẤU";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlBoard);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 70);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(800, 680);
            this.pnlMain.TabIndex = 2;
            // 
            // pnlBoard
            // 
            this.pnlBoard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlBoard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(40)))));
            this.pnlBoard.Location = new System.Drawing.Point(150, 90);
            this.pnlBoard.Name = "pnlBoard";
            this.pnlBoard.Size = new System.Drawing.Size(500, 500);
            this.pnlBoard.TabIndex = 0;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1150, 750);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Caro Arena - Phòng #101";
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.pnlChat.ResumeLayout(false);
            this.pnlChat.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.pnlStatus.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBadge;
        private System.Windows.Forms.Label lblSpectators;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.Label lblPlayerX;
        private System.Windows.Forms.Label lblPlayerX_Status;
        private System.Windows.Forms.Label lblPlayerO_Status;
        private System.Windows.Forms.Label lblPlayerO;
        private System.Windows.Forms.Label lblTurnText;
        private System.Windows.Forms.Label lblTurnValue;
        private System.Windows.Forms.Panel pnlChat;
        private System.Windows.Forms.Label lblChatTitle;
        private System.Windows.Forms.RichTextBox rtbChatHistory;
        private System.Windows.Forms.TextBox txtChatInput;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlBoard;
        private System.Windows.Forms.Button btnLeaveRoom;
    }
}