using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame.Protocol.Messages.History;
using System.Collections.Generic;

namespace Client.Forms
{
    public class HistoryForm : Form
    {
        private ListView lstHistory;
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnClose;

        public HistoryForm(List<MatchHistoryItem> matches, string playerName)
        {
            this.Text = "Lịch sử trận đấu - " + playerName;
            this.Size = new Size(700, 460);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(34, 36, 40);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label();
            lblTitle.Text = "LỊCH SỬ TRẬN ĐẤU";
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.DeepSkyBlue;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(25, 15);

            lblSubtitle = new Label();
            lblSubtitle.Text = $"Người chơi: {playerName} - Tổng cộng: {matches?.Count ?? 0} trận";
            lblSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(26, 45);

            lstHistory = new ListView();
            lstHistory.View = View.Details;
            lstHistory.FullRowSelect = true;
            lstHistory.GridLines = true;
            lstHistory.BackColor = Color.FromArgb(28, 30, 34);
            lstHistory.ForeColor = Color.White;
            lstHistory.Font = new Font("Segoe UI", 9.5F);
            lstHistory.Location = new Point(25, 75);
            lstHistory.Size = new Size(635, 295);
            lstHistory.BorderStyle = BorderStyle.FixedSingle;

            lstHistory.Columns.Add("Mã trận", 80);
            lstHistory.Columns.Add("Bắt đầu", 160);
            lstHistory.Columns.Add("Kết thúc", 160);
            lstHistory.Columns.Add("Kết quả", 110);
            lstHistory.Columns.Add("Trạng thái", 100);

            if (matches != null && matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    var item = new ListViewItem(match.MatchId.ToString());
                    item.UseItemStyleForSubItems = false;
                    item.SubItems.Add(match.StartTime.ToString("dd/MM/yyyy HH:mm:ss"));
                    item.SubItems.Add(match.EndTime.HasValue ? match.EndTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "--");
                    
                    string res = string.IsNullOrWhiteSpace(match.Result) ? "Chưa rõ" : match.Result;
                    var subRes = item.SubItems.Add(res);
                    if (res.Contains("Win", StringComparison.OrdinalIgnoreCase))
                        subRes.ForeColor = Color.LimeGreen;
                    else if (res.Contains("Loss", StringComparison.OrdinalIgnoreCase))
                        subRes.ForeColor = Color.Salmon;
                    else if (res.Contains("Draw", StringComparison.OrdinalIgnoreCase))
                        subRes.ForeColor = Color.Gold;

                    item.SubItems.Add(string.IsNullOrWhiteSpace(match.Status) ? "Hoàn thành" : match.Status);
                    lstHistory.Items.Add(item);
                }
            }

            btnClose = new Button();
            btnClose.Text = "ĐÓNG";
            btnClose.BackColor = Color.FromArgb(217, 83, 79);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnClose.Size = new Size(110, 36);
            btnClose.Location = new Point(550, 380);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(lstHistory);
            this.Controls.Add(btnClose);
        }
    }
}
