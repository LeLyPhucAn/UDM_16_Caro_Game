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
            this.Text = "Lịch sử thi đấu - " + playerName;
            this.Size = new Size(750, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(28, 30, 34);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // ======================================================
            // THỐNG KÊ KẾT QUẢ
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

            // ======================================================
            // TIÊU ĐỀ & THÔNG TIN NGƯỜI CHƠI (THEO MẪU THIẾT KẾ)
            // ======================================================
            var lblTitle = new Label
            {
                Text = "LỊCH SỬ THI ĐẤU",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.DeepSkyBlue,
                AutoSize = true,
                Location = new Point(22, 16)
            };

            var lblSub = new Label
            {
                Text = $"Người chơi: {playerName}   |   Tổng: {matches?.Count ?? 0} trận   [Thắng: {wins}]   [Hòa: {draws}]   [Thua: {losses}]",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 205, 215),
                AutoSize = true,
                Location = new Point(23, 50)
            };

            // ======================================================
            // BẢNG LỊCH SỬ (5 CỘT CHUẨN)
            // STT | THỜI GIAN | THỜI LƯỢNG | KẾT QUẢ | GHI CHÚ
            // ======================================================
            var lstHistory = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BackColor = Color.FromArgb(34, 36, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(22, 85),
                Size = new Size(690, 335),
                BorderStyle = BorderStyle.None,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };

            lstHistory.Columns.Add("STT", 65, HorizontalAlignment.Center);
            lstHistory.Columns.Add("THỜI GIAN", 155, HorizontalAlignment.Center);
            lstHistory.Columns.Add("THỜI LƯỢNG", 110, HorizontalAlignment.Center);
            lstHistory.Columns.Add("KẾT QUẢ", 100, HorizontalAlignment.Center);
            lstHistory.Columns.Add("GHI CHÚ", 250, HorizontalAlignment.Left);

            if (matches != null && matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    string stt = $"#{i + 1}";
                    string timeStr = match.StartTime.ToString("dd/MM/yyyy HH:mm");

                    // Tính thời lượng trận đấu theo định dạng mm:ss
                    string duration = "--";
                    if (match.EndTime.HasValue)
                    {
                        var span = match.EndTime.Value - match.StartTime;
                        if (span.TotalSeconds >= 0)
                        {
                            duration = $"{(int)span.TotalMinutes:D2}:{span.Seconds:D2}";
                        }
                    }

                    // Chuẩn hóa kết quả hiển thị (THẮNG / HÒA / THUA)
                    string rawRes = string.IsNullOrWhiteSpace(match.Result) ? "Chưa rõ" : match.Result;
                    string resDisplay;
                    Color resColor;

                    if (rawRes.Equals("Thắng", StringComparison.OrdinalIgnoreCase) || rawRes.Contains("Win", StringComparison.OrdinalIgnoreCase))
                    {
                        resDisplay = "THẮNG";
                        resColor = Color.FromArgb(46, 204, 113); // Xanh lá
                    }
                    else if (rawRes.Equals("Hòa", StringComparison.OrdinalIgnoreCase) || rawRes.Contains("Draw", StringComparison.OrdinalIgnoreCase))
                    {
                        resDisplay = "HÒA";
                        resColor = Color.FromArgb(241, 196, 15); // Vàng
                    }
                    else if (rawRes.Equals("Thua", StringComparison.OrdinalIgnoreCase) || rawRes.Contains("Loss", StringComparison.OrdinalIgnoreCase))
                    {
                        resDisplay = "THUA";
                        resColor = Color.FromArgb(231, 76, 60); // Đỏ
                    }
                    else
                    {
                        resDisplay = rawRes.ToUpper();
                        resColor = Color.DarkGray;
                    }

                    // Ghi chú (Đầu hàng, Hết giờ, Nước đi, v.v.)
                    string note = string.IsNullOrWhiteSpace(match.Status) ? "Hoàn thành" : match.Status;

                    var item = new ListViewItem(stt);
                    item.UseItemStyleForSubItems = false;
                    item.SubItems.Add(timeStr);
                    item.SubItems.Add(duration);

                    var subRes = item.SubItems.Add(resDisplay);
                    subRes.ForeColor = resColor;

                    item.SubItems.Add(note);
                    lstHistory.Items.Add(item);
                }
            }
            else
            {
                var emptyItem = new ListViewItem("-");
                emptyItem.SubItems.Add("Chưa có dữ liệu trận đấu");
                emptyItem.SubItems.Add("");
                emptyItem.SubItems.Add("");
                emptyItem.SubItems.Add("");
                lstHistory.Items.Add(emptyItem);
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
                Location = new Point(602, 435),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSub);
            this.Controls.Add(lstHistory);
            this.Controls.Add(btnClose);
        }
    }
}
