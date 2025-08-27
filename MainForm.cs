using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WolClientApp
{
    public class MainForm : Form
    {
        private FlowLayoutPanel cardsPanel;
        private TextBox logBox;
        private WolClient _client;

        
        private readonly List<Device> devices = new()
        {
            new Device("Office PC", "pc1"),
            new Device("Living Room PC", "pc2"),
            new Device("Server", "pc3"),
            new Device("Test Machine", "pc4"),
            new Device("Laptop", "pc5"),
            new Device("Media Center", "pc6"),
        };

        public MainForm()
        {
            Text = "PowerUp - WOL Client";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(860, 600);
            BackColor = Color.FromArgb(244, 246, 249);
            
            // Replace this with your Node.js server URL (safe placeholder for GitHub)
            _client = new WolClient("http://localhost:5000");

            // Cards panel
            cardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(16),
                Height = 420
            };
            Controls.Add(cardsPanel);

            // Log box
            logBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10),
                BackColor = Color.White
            };
            Controls.Add(logBox);

            BuildCards();
        }

        private void BuildCards()
        {
            cardsPanel.Controls.Clear();
            foreach (var d in devices)
            {
                var card = CreateCard(d);
                cardsPanel.Controls.Add(card);
            }
        }

        private Control CreateCard(Device d)
        {
            var card = new Panel
            {
                Width = 250,
                Height = 170,
                Margin = new Padding(10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var name = new Label
            {
                Text = d.Name,
                AutoSize = false,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 34,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(name);

            var status = new Label
            {
                Text = "Status: unknown",
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 26,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(status);

            var btn = new Button
            {
                Text = "Wake Up",
                Height = 36,
                Width = 120,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Location = new Point((card.Width - btn.Width) / 2, 95);
            btn.Anchor = AnchorStyles.None;

            btn.Click += async (_, __) => await WakeDevice(d, status);

            card.Controls.Add(btn);

            return card;
        }

        private async Task WakeDevice(Device d, Label statusLabel)
        {
            try
            {
                Log($"[{d.Name}] Sending WOL request…");
                statusLabel.Text = "Status: sending…";

                string html = await _client.WakeDevice(d.Id);
                string result = ExtractH2Text(html);

                Log($"[{d.Name}] {result}");
                statusLabel.Text = $"Status: {result}";

                MessageBox.Show(result, d.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log($"[{d.Name}] Error: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Log(string message)
        {
            logBox.AppendText($"{DateTime.Now:HH:mm:ss} {message}{Environment.NewLine}");
        }

        private string ExtractH2Text(string html)
        {
            var match = Regex.Match(html, @"<h2[^>]*>(.*?)</h2>", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : html.Trim();
        }
    }

    public record Device(string Name, string Id);

    public class WolClient
    {
        private readonly string _serverUrl;

        public WolClient(string serverUrl)
        {
            _serverUrl = serverUrl;
        }

        public async Task<string> WakeDevice(string deviceId)
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                var response = await client.GetAsync($"{_serverUrl}/wake/{deviceId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
