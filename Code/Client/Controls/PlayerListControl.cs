using System;
using System.Collections.Generic;
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

        private Label label1;
        private ListBox lstPlayers;

        private void InitializeComponent()
        {

        }
    }
}