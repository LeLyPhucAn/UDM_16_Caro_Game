using System;
using System.Collections.Generic;
using System.Drawing; // Bổ sung thư viện này để dùng Point và Size
using System.Windows.Forms;

namespace Client.Controls
{
    public partial class PlayerListControl : UserControl
    {
        public PlayerListControl()
        {
            InitializeComponent();
        }

        // Hàm này sẽ được LobbyForm gọi khi nhận dữ liệu từ Server
        public void UpdateList(List<string> players)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateList(players)));
                return;
            }

            lstPlayers.Items.Clear();
            if (players != null)
            {
                foreach (var p in players)
                {
                    lstPlayers.Items.Add("👤 " + p);
                }
            }
        }

        // Đã xóa bỏ dòng ListBox bị lặp
        private Label label2;
        private ListBox lstPlayers;

        private void InitializeComponent()
        {
            label2 = new Label();
            lstPlayers = new ListBox();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(424, 32);
            label2.Name = "label2";
            label2.Size = new Size(130, 20);
            label2.TabIndex = 0;
            label2.Text = "Người chơi Online";
            // 
            // lstPlayers
            // 
            lstPlayers.BackColor = SystemColors.WindowText;
            lstPlayers.BorderStyle = BorderStyle.None;
            lstPlayers.Dock = DockStyle.Fill;
            lstPlayers.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lstPlayers.ForeColor = Color.YellowGreen;
            lstPlayers.FormattingEnabled = true;
            lstPlayers.Location = new Point(0, 0);
            lstPlayers.Name = "lstPlayers";
            lstPlayers.Size = new Size(927, 457);
            lstPlayers.TabIndex = 1;
            // 
            // PlayerListControl
            // 
            Controls.Add(lstPlayers);
            Controls.Add(label2);
            Name = "PlayerListControl";
            Size = new Size(927, 457);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}