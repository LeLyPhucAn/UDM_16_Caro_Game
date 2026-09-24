using System;
using System.Collections.Generic;
using System.Drawing; // Bổ sung thư viện này để dùng Point và Size
using System.Windows.Forms;

namespace Client.Controls
{
    public partial class PlayerListControl : UserControl
    {
        // Sự kiện báo lên LobbyForm khi chọn Thách Đấu
        public event Action<string>? OnChallengePlayer;
        private ContextMenuStrip _contextMenu;

        public PlayerListControl()
        {
            InitializeComponent();

            _contextMenu = new ContextMenuStrip();
            var item = _contextMenu.Items.Add("Thách Đấu");
            item.Click += Challenge_Click;
            if (lstPlayers != null)
            {
                lstPlayers.ContextMenuStrip = _contextMenu;
                lstPlayers.MouseDown += LstPlayers_MouseDown;
            }
        }

        private void LstPlayers_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = lstPlayers.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    lstPlayers.SelectedIndex = index;
                }
            }
        }

        private void Challenge_Click(object? sender, EventArgs e)
        {
            if (lstPlayers.SelectedItem != null)
            {
                string player = lstPlayers.SelectedItem.ToString()!;
                OnChallengePlayer?.Invoke(player);
            }
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
                    lstPlayers.Items.Add(p);
                }
            }
        }

        private ListBox lstPlayers = null!;

        private void InitializeComponent()
        {
            lstPlayers = new ListBox();
            SuspendLayout();
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
            Name = "PlayerListControl";
            Size = new Size(927, 457);
            ResumeLayout(false);
        }
    }
}