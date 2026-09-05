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
            pnlTop = new Panel();
            lblSpectators = new Label();
            lblBadge = new Label();
            lblTitle = new Label();
            btnLeaveRoom = new Button();
            pnlRight = new Panel();
            pnlChat = new Panel();
            btnSend = new Button();
            txtChatInput = new TextBox();
            rtbChatHistory = new RichTextBox();
            lblChatTitle = new Label();
            pnlStatus = new Panel();
            lblTurnValue = new Label();
            lblTurnText = new Label();
            lblPlayerO_Status = new Label();
            lblPlayerO = new Label();
            lblPlayerX_Status = new Label();
            lblCurrentTurn = new Label();
            lblStatusTitle = new Label();
            pnlMain = new Panel();
            pnlBoard = new Panel();
            pnlTop.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlChat.SuspendLayout();
            pnlStatus.SuspendLayout();
            pnlMain.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(lblSpectators);
            pnlTop.Controls.Add(lblBadge);
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(btnLeaveRoom);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1150, 70);
            pnlTop.TabIndex = 0;
            // 
            // lblSpectators
            // 
            lblSpectators.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblSpectators.AutoSize = true;
            lblSpectators.ForeColor = Color.Gray;
            lblSpectators.Location = new Point(1040, 25);
            lblSpectators.Name = "lblSpectators";
            lblSpectators.Size = new Size(95, 23);
            lblSpectators.TabIndex = 2;
            lblSpectators.Text = "Khán giả: 4";
            // 
            // lblBadge
            // 
            lblBadge.AutoSize = true;
            lblBadge.BackColor = Color.FromArgb(40, 70, 120);
            lblBadge.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblBadge.Location = new Point(260, 26);
            lblBadge.Name = "lblBadge";
            lblBadge.Padding = new Padding(5);
            lblBadge.Size = new Size(130, 30);
            lblBadge.TabIndex = 1;
            lblBadge.Text = "Standard 10x10";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(234, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "PHÒNG THI ĐẤU";
            // 
            // btnLeaveRoom
            // 
            btnLeaveRoom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLeaveRoom.BackColor = Color.FromArgb(217, 83, 79);
            btnLeaveRoom.FlatAppearance.BorderSize = 0;
            btnLeaveRoom.FlatStyle = FlatStyle.Flat;
            btnLeaveRoom.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLeaveRoom.Location = new Point(900, 18);
            btnLeaveRoom.Name = "btnLeaveRoom";
            btnLeaveRoom.Size = new Size(120, 35);
            btnLeaveRoom.TabIndex = 3;
            btnLeaveRoom.Text = "RỜI PHÒNG";
            btnLeaveRoom.UseVisualStyleBackColor = false;
            btnLeaveRoom.Click += btnLeaveRoom_Click;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(pnlChat);
            pnlRight.Controls.Add(pnlStatus);
            pnlRight.Dock = DockStyle.Right;
            pnlRight.Location = new Point(800, 70);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(15);
            pnlRight.Size = new Size(350, 680);
            pnlRight.TabIndex = 1;
            // 
            // pnlChat
            // 
            pnlChat.BackColor = Color.FromArgb(42, 44, 48);
            pnlChat.Controls.Add(btnSend);
            pnlChat.Controls.Add(txtChatInput);
            pnlChat.Controls.Add(rtbChatHistory);
            pnlChat.Controls.Add(lblChatTitle);
            pnlChat.Dock = DockStyle.Fill;
            pnlChat.Location = new Point(15, 205);
            pnlChat.Name = "pnlChat";
            pnlChat.Padding = new Padding(15);
            pnlChat.Size = new Size(320, 460);
            pnlChat.TabIndex = 1;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.BackColor = Color.FromArgb(0, 120, 215);
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSend.Location = new Point(235, 405);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(70, 40);
            btnSend.TabIndex = 3;
            btnSend.Text = "GỬI";
            btnSend.UseVisualStyleBackColor = false;
            // 
            // txtChatInput
            // 
            txtChatInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtChatInput.BackColor = Color.FromArgb(34, 36, 40);
            txtChatInput.BorderStyle = BorderStyle.FixedSingle;
            txtChatInput.ForeColor = Color.White;
            txtChatInput.Location = new Point(15, 405);
            txtChatInput.Multiline = true;
            txtChatInput.Name = "txtChatInput";
            txtChatInput.Size = new Size(210, 40);
            txtChatInput.TabIndex = 2;
            // 
            // rtbChatHistory
            // 
            rtbChatHistory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbChatHistory.BackColor = Color.FromArgb(34, 36, 40);
            rtbChatHistory.BorderStyle = BorderStyle.None;
            rtbChatHistory.ForeColor = Color.White;
            rtbChatHistory.Location = new Point(15, 50);
            rtbChatHistory.Name = "rtbChatHistory";
            rtbChatHistory.ReadOnly = true;
            rtbChatHistory.Size = new Size(290, 340);
            rtbChatHistory.TabIndex = 1;
            rtbChatHistory.Text = "";
            // 
            // lblChatTitle
            // 
            lblChatTitle.AutoSize = true;
            lblChatTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblChatTitle.Location = new Point(15, 15);
            lblChatTitle.Name = "lblChatTitle";
            lblChatTitle.Size = new Size(189, 25);
            lblChatTitle.TabIndex = 0;
            lblChatTitle.Text = "KÊNH TRÒ CHUYỆN";
            // 
            // pnlStatus
            // 
            pnlStatus.BackColor = Color.FromArgb(42, 44, 48);
            pnlStatus.Controls.Add(lblTurnValue);
            pnlStatus.Controls.Add(lblTurnText);
            pnlStatus.Controls.Add(lblPlayerO_Status);
            pnlStatus.Controls.Add(lblPlayerO);
            pnlStatus.Controls.Add(lblPlayerX_Status);
            pnlStatus.Controls.Add(lblCurrentTurn);
            pnlStatus.Controls.Add(lblStatusTitle);
            pnlStatus.Dock = DockStyle.Top;
            pnlStatus.Location = new Point(15, 15);
            pnlStatus.Name = "pnlStatus";
            pnlStatus.Size = new Size(320, 190);
            pnlStatus.TabIndex = 0;
            // 
            // lblTurnValue
            // 
            lblTurnValue.AutoSize = true;
            lblTurnValue.BackColor = Color.FromArgb(30, 70, 100);
            lblTurnValue.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTurnValue.ForeColor = Color.DeepSkyBlue;
            lblTurnValue.Location = new Point(150, 140);
            lblTurnValue.Name = "lblTurnValue";
            lblTurnValue.Padding = new Padding(5);
            lblTurnValue.Size = new Size(116, 33);
            lblTurnValue.TabIndex = 6;
            lblTurnValue.Text = "X (Nam123)";
            // 
            // lblTurnText
            // 
            lblTurnText.AutoSize = true;
            lblTurnText.Location = new Point(15, 145);
            lblTurnText.Name = "lblTurnText";
            lblTurnText.Size = new Size(129, 23);
            lblTurnText.TabIndex = 5;
            lblTurnText.Text = "Lượt đi hiện tại:";
            // 
            // lblPlayerO_Status
            // 
            lblPlayerO_Status.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPlayerO_Status.AutoSize = true;
            lblPlayerO_Status.ForeColor = Color.Gray;
            lblPlayerO_Status.Location = new Point(235, 100);
            lblPlayerO_Status.Name = "lblPlayerO_Status";
            lblPlayerO_Status.Size = new Size(79, 23);
            lblPlayerO_Status.TabIndex = 4;
            lblPlayerO_Status.Text = "Sẵn sàng";
            // 
            // lblPlayerO
            // 
            lblPlayerO.AutoSize = true;
            lblPlayerO.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPlayerO.ForeColor = Color.FromArgb(217, 83, 79);
            lblPlayerO.Location = new Point(15, 95);
            lblPlayerO.Name = "lblPlayerO";
            lblPlayerO.Size = new Size(123, 28);
            lblPlayerO.TabIndex = 3;
            lblPlayerO.Text = "O: Minh456";
            // 
            // lblPlayerX_Status
            // 
            lblPlayerX_Status.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPlayerX_Status.AutoSize = true;
            lblPlayerX_Status.ForeColor = Color.Gray;
            lblPlayerX_Status.Location = new Point(265, 65);
            lblPlayerX_Status.Name = "lblPlayerX_Status";
            lblPlayerX_Status.Size = new Size(45, 23);
            lblPlayerX_Status.TabIndex = 2;
            lblPlayerX_Status.Text = "Host";
            // 
            // lblCurrentTurn
            // 
            lblCurrentTurn.AutoSize = true;
            lblCurrentTurn.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblCurrentTurn.ForeColor = Color.DeepSkyBlue;
            lblCurrentTurn.Location = new Point(15, 60);
            lblCurrentTurn.Name = "lblCurrentTurn";
            lblCurrentTurn.Size = new Size(117, 28);
            lblCurrentTurn.TabIndex = 1;
            lblCurrentTurn.Text = "X: Nam123";
            lblCurrentTurn.Click += lblPlayerX_Click;
            // 
            // lblStatusTitle
            // 
            lblStatusTitle.AutoSize = true;
            lblStatusTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblStatusTitle.Location = new Point(15, 15);
            lblStatusTitle.Name = "lblStatusTitle";
            lblStatusTitle.Size = new Size(173, 25);
            lblStatusTitle.TabIndex = 0;
            lblStatusTitle.Text = "TRẠNG THÁI ĐẤU";
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(pnlBoard);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 70);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(800, 680);
            pnlMain.TabIndex = 2;
            // 
            // pnlBoard
            // 
            pnlBoard.Anchor = AnchorStyles.None;
            pnlBoard.BackColor = Color.FromArgb(34, 36, 40);
            pnlBoard.Location = new Point(150, 90);
            pnlBoard.Name = "pnlBoard";
            pnlBoard.Size = new Size(500, 500);
            pnlBoard.TabIndex = 0;
            // 
            // GameForm
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(34, 36, 40);
            ClientSize = new Size(1150, 750);
            Controls.Add(pnlMain);
            Controls.Add(pnlRight);
            Controls.Add(pnlTop);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.White;
            MinimumSize = new Size(1000, 700);
            Name = "GameForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Caro Arena - Phòng #101";
            Load += GameForm_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlRight.ResumeLayout(false);
            pnlChat.ResumeLayout(false);
            pnlChat.PerformLayout();
            pnlStatus.ResumeLayout(false);
            pnlStatus.PerformLayout();
            pnlMain.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBadge;
        private System.Windows.Forms.Label lblSpectators;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.Label lblCurrentTurn;
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