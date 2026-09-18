using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame.Protocol.Messages.History;
using System.Collections.Generic;

namespace Client.Forms
{
    public class HistoryForm : Form
    {
        public HistoryForm(List<MatchHistoryItem> matches, string playerName)
        {
            this.Text = "Lịch sử trận đấu - " + playerName;
            this.Size = new Size(720, 530);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(28, 30, 34);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // ======================================================
            // TIÊU ĐỀ
            // ======================================================
            var lblTitle = new Label
            {
                Text = "LỊCH SỬ TRẬN ĐẤU",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.DeepSkyBlue,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            var lblSub = new Label
            {
                Text = $"Người chơi: {playerName}  |  Tổng: {matches?.Count ?? 0} trận",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(21, 47)
            };

            // ======================================================
            // BADGES THỐNG KÊ: Thắng / Hòa / Thua
            // ======================================================
            int wins = 0, draws = 0, losses = 0;
            if (matches != null)
            {
                foreach (var m in matches)
                {
                    string r = m.Result ?? "";
                    if (r.Equals("Thắng", StringComparison.OrdinalIgnoreCase) || r.Contains("Win", StringComparison.OrdinalIgnoreCase))
                        wins++;
                    else if (r.Equals("Hòa", StringComparison.OrdinalIgnoreCase) || r.Contains("Draw", StringComparison.OrdinalIgnoreCase))
                        draws++;
                    else if (r.Equals("Thua", StringComparison.OrdinalIgnoreCase) || r.Contains("Loss", StringComparison.OrdinalIgnoreCase))
                        losses++;
                }
            }

            var pnlBadges = new FlowLayoutPanel
            {
                Location = new Point(20, 68),
                Size = new Size(660, 48),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0)
            };
            pnlBadges.Controls.Add(MakeBadge($"  THẮNG: {wins}  ", Color.FromArgb(25, 70, 45), Color.FromArgb(46, 204, 113)));
            pnlBadges.Controls.Add(MakeBadge($"  HÒA: {draws}  ", Color.FromArgb(70, 60, 20), Color.FromArgb(241, 196, 15)));
            pnlBadges.Controls.Add(MakeBadge($"  THUA: {losses}  ", Color.FromArgb(70, 28, 28), Color.FromArgb(231, 76, 60)));

            // ======================================================
            // BẢNG LỊCH SỬ (4 cột gọn)
            // ======================================================
            var lstHistory = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BackColor = Color.FromArgb(34, 36, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(20, 125),
                Size = new Size(660, 305),
                BorderStyle = BorderStyle.None,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };

            lstHistory.Columns.Add("Thời gian", 140);
            lstHistory.Columns.Add("Thời lượng", 90);
            lstHistory.Columns.Add("Kết quả", 85);
            lstHistory.Columns.Add("Đối thủ / Ghi chú", 335);

            if (matches != null && matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    string timeStr = match.StartTime.ToString("dd/MM/yyyy HH:mm");

                    // Tính thời lượng trận đấu
                    string duration = "--";
                    if (match.EndTime.HasValue)
                    {
                        var span = match.EndTime.Value - match.StartTime;
                        if (span.TotalSeconds >= 60)
                            duration = $"{(int)span.TotalMinutes}p{span.Seconds:D2}s";
                        else
                            duration = $"{(int)span.TotalSeconds}s";
                    }

                    string res = string.IsNullOrWhiteSpace(match.Result) ? "Chưa rõ" : match.Result;
                    string note = string.IsNullOrWhiteSpace(match.Status) ? "Hoàn thành" : match.Status;

                    var item = new ListViewItem(timeStr);
                    item.UseItemStyleForSubItems = false;
                    item.SubItems.Add(duration);

                    var subRes = item.SubItems.Add(res);
                    if (res.Equals("Thắng", StringComparison.OrdinalIgnoreCase) || res.Contains("Win", StringComparison.OrdinalIgnoreCase))
                        subRes.ForeColor = Color.FromArgb(46, 204, 113);
                    else if (res.Equals("Thua", StringComparison.OrdinalIgnoreCase) || res.Contains("Loss", StringComparison.OrdinalIgnoreCase))
                        subRes.ForeColor = Color.Salmon;
                    else if (res.Equals("Hòa", StringComparison.OrdinalIgnoreCase) || res.Contains("Draw", StringComparison.OrdinalIgnoreCase))
                        subRes.ForeColor = Color.Gold;
                    else
                        subRes.ForeColor = Color.DarkGray;

                    item.SubItems.Add(note);
                    lstHistory.Items.Add(item);
                }
            }
            else
            {
                lstHistory.Items.Add(new ListViewItem("Chưa có trận đấu nào."));
            }

            // ======================================================
            // NÚT ĐÓNG
            // ======================================================
            var btnClose = new Button
            {
                Text = "ĐÓNG",
                BackColor = Color.FromArgb(217, 83, 79),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(110, 36),
                Location = new Point(570, 450),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSub);
            this.Controls.Add(pnlBadges);
            this.Controls.Add(lstHistory);
            this.Controls.Add(btnClose);
        }

        // Tạo badge label hiển thị số liệu thống kê
        private Label MakeBadge(string text, Color back, Color fore)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                BackColor = back,
                ForeColor = fore,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(6, 4, 6, 4),
                Margin = new Padding(0, 0, 10, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
        }
    }
}
