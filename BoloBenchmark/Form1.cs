using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TitanBenchmarkPro
{
    static class Program
    {
        public static string CurrentPCName = "";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var f = new InputNameForm())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    CurrentPCName = f.PCName;
                    Application.Run(new MainMenuForm());
                }
            }
        }
    }

    public static class BenchmarkEngine
    {
        public const long ITERATIONS_INT = 2_500_000_000;
        public const long ITERATIONS_FLOAT = 2_500_000_000;
        public const long ITERATIONS_PI = 5_000_000_000;

        public static long ResultInt64 = 0;
        public static int ResultInt32 = 0;
        public static double ResultDouble = 0.0;

        public static void RunInteger64Logic(long iterations)
        {
            long a = 12345;
            long b = 67890;
            long c = 101112;
            long local_sum = 0;
            unchecked
            {
                for (long i = 0; i < iterations; i++)
                {
                    a = a + b;
                    b = a * c;
                    c = b ^ 0xFF;
                    a = c >> 3;
                    b = a | 0xAA;
                    c = b & a;
                    local_sum += c;
                }
            }
            ResultInt64 = local_sum;
        }

        public static void RunInteger32Logic(long iterations)
        {
            int a = 12345;
            int b = 67890;
            int c = 101112;
            int local_sum = 0;
            unchecked
            {
                for (long i = 0; i < iterations; i++)
                {
                    a = a + b;
                    b = a * c;
                    c = b ^ 0xFF;
                    a = c >> 3;
                    b = a | 0xAA;
                    c = b & a;
                    local_sum += c;
                }
            }
            ResultInt32 = local_sum;
        }

        public static double RunFloatFMA(long iterations)
        {
            double x = 1.234;
            double y = 5.678;
            double z = 9.012;
            double local_sum = 0.0;
            for (long i = 0; i < iterations; i++)
            {
                x = x + y;
                y = x * z;
                z = y / 1.5;
                x = (x * y) + z;
                local_sum += x;
            }
            ResultDouble = local_sum;
            return local_sum;
        }

        public static void RunSuperPiSlice(long iterations)
        {
            double pi = 0;
            double sign = 1;
            for (long i = 0; i < iterations; i++)
            {
                pi += sign / (2.0 * i + 1.0);
                sign = -sign;
            }
            if (ResultDouble == 0) ResultDouble = pi;
        }

        public static void SaveResult(string test, double time, double perf, string unit)
        {
            try
            {
                string file = "titan_results.csv";
                bool head = !File.Exists(file);
                using (StreamWriter w = new StreamWriter(file, true))
                {
                    if (head) w.WriteLine("PC,Test,Time,Score,Unit");
                    w.WriteLine($"{Program.CurrentPCName},{test},{time:F4},{perf:F2},{unit}");
                }
            }
            catch { }
        }
    }

    public class InputNameForm : Form
    {
        public string PCName { get; private set; }
        private TextBox txtName;

        public InputNameForm()
        {
            this.Text = "Login";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            Label lbl = new Label();
            lbl.Text = "Nume PC:";
            lbl.Location = new Point(30, 40);
            lbl.AutoSize = true;
            lbl.Font = new Font("Segoe UI", 11);

            txtName = new TextBox();
            txtName.Location = new Point(30, 70);
            txtName.Width = 320;
            txtName.Text = Environment.MachineName;

            Button btn = new Button();
            btn.Text = "START";
            btn.Location = new Point(250, 110);
            btn.Width = 100;
            btn.BackColor = Color.SeaGreen;
            btn.FlatStyle = FlatStyle.Flat;
            btn.DialogResult = DialogResult.OK;
            btn.Click += (s, e) => PCName = txtName.Text;

            this.Controls.Add(lbl);
            this.Controls.Add(txtName);
            this.Controls.Add(btn);
            this.AcceptButton = btn;
        }
    }

    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            this.Text = $"Titan Benchmark - {Program.CurrentPCName}";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            InitLayout();
        }

        private void InitLayout()
        {
            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.FixedPanel = FixedPanel.Panel1;
            split.IsSplitterFixed = true;
            split.SplitterDistance = 300;
            split.SplitterWidth = 1;
            this.Controls.Add(split);

            Panel left = split.Panel1;
            left.BackColor = Color.FromArgb(45, 45, 48);
            left.Padding = new Padding(20);

            Label lblTitle = new Label();
            lblTitle.Text = "SYSTEM INFO";
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.Cyan;
            lblTitle.Height = 40;

            Label lblInfo = new Label();
            lblInfo.Dock = DockStyle.Fill;
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.ForeColor = Color.LightGray;
            lblInfo.Text = "Scanning...";

            left.Controls.Add(lblInfo);
            left.Controls.Add(lblTitle);

            Panel right = split.Panel2;
            right.BackColor = Color.FromArgb(30, 30, 30);
            right.Padding = new Padding(40);

            TableLayoutPanel grid = new TableLayoutPanel();
            grid.Dock = DockStyle.Fill;
            grid.ColumnCount = 2;
            grid.RowCount = 3;
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));

            grid.Controls.Add(MkBtn("CPU BENCHMARK", Color.RoyalBlue, () => new CpuForm().Show()), 0, 0);
            grid.Controls.Add(MkBtn("RAM BENCHMARK", Color.MediumPurple, () => new RamForm().Show()), 1, 0);
            grid.Controls.Add(MkBtn("GPU STRESS", Color.Crimson, () => new GpuForm().Show()), 0, 1);
            grid.Controls.Add(MkBtn("DISK BENCHMARK", Color.DarkOrange, () => new DiskForm().Show()), 1, 1);
            grid.Controls.Add(MkBtn("RUN ALL", Color.SeaGreen, RunAll), 0, 2);
            grid.Controls.Add(MkBtn("RESULTS", Color.Teal, () => new ResForm().Show()), 1, 2);

            right.Controls.Add(grid);

            Task.Run(() =>
            {
                string s = GetInfo();
                this.Invoke(new Action(() => lblInfo.Text = s));
            });
        }

        private Button MkBtn(string t, Color c, Action a)
        {
            Button b = new Button();
            b.Text = t;
            b.Dock = DockStyle.Fill;
            b.BackColor = c;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            b.Margin = new Padding(15);
            b.Cursor = Cursors.Hand;
            b.Click += (s, e) => a();
            return b;
        }

        private void RunAll()
        {
            new CpuForm().Show();
            new RamForm().Show();
            new GpuForm().Show();
            new DiskForm().Show();
        }

        private string GetInfo()
        {
            string s = "";
            try
            {
                using (var mos = new ManagementObjectSearcher("select Name, NumberOfCores, MaxClockSpeed from Win32_Processor"))
                    foreach (var o in mos.Get()) s += $"CPU:\n{o["Name"]}\nCores: {o["NumberOfCores"]} @ {o["MaxClockSpeed"]} MHz\n\n";
                using (var mos = new ManagementObjectSearcher("select Capacity from Win32_PhysicalMemory"))
                {
                    long t = 0; foreach (var o in mos.Get()) t += Convert.ToInt64(o["Capacity"]);
                    s += $"RAM:\n{t / (1024 * 1024 * 1024)} GB\n\n";
                }
                using (var mos = new ManagementObjectSearcher("select Name from Win32_VideoController"))
                    foreach (var o in mos.Get()) s += $"GPU:\n{o["Name"]}\n\n";
                using (var mos = new ManagementObjectSearcher("select Caption from Win32_OperatingSystem"))
                    foreach (var o in mos.Get()) s += $"OS:\n{o["Caption"]}\n";
            }
            catch { s = "N/A"; }
            return s;
        }
    }

    public class CpuForm : Form
    {
        TextBox log; ProgressBar bar; Button start;
        public CpuForm() { Setup(); }
        void Setup()
        {
            this.Text = "CPU Benchmark"; this.Size = new Size(600, 500); this.BackColor = Color.FromArgb(40, 40, 40); this.StartPosition = FormStartPosition.CenterScreen;
            start = new Button { Text = "START", Dock = DockStyle.Top, Height = 50, BackColor = Color.RoyalBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            bar = new ProgressBar { Dock = DockStyle.Top, Height = 10 };
            log = new TextBox { Dock = DockStyle.Fill, Multiline = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ReadOnly = true };
            start.Click += Run;
            this.Controls.Add(log); this.Controls.Add(bar); this.Controls.Add(start);
        }
        async void Run(object s, EventArgs e)
        {
            start.Enabled = false; log.Clear(); bar.Value = 0;
            await Task.Run(() =>
            {
                Log("Int32 Test...");
                var sw = Stopwatch.StartNew();
                BenchmarkEngine.RunInteger32Logic(BenchmarkEngine.ITERATIONS_INT);
                sw.Stop();
                double mips32 = (BenchmarkEngine.ITERATIONS_INT * 6.0 / sw.Elapsed.TotalSeconds) / 1e6;
                Log($"Time: {sw.Elapsed.TotalSeconds:F4}s | Score: {mips32:F2} MIPS");
                BenchmarkEngine.SaveResult("CPU_Int32", sw.Elapsed.TotalSeconds, mips32, "MIPS");
                SetP(25);

                Log("Int64 Test...");
                sw.Restart();
                BenchmarkEngine.RunInteger64Logic(BenchmarkEngine.ITERATIONS_INT);
                sw.Stop();
                double mips64 = (BenchmarkEngine.ITERATIONS_INT * 6.0 / sw.Elapsed.TotalSeconds) / 1e6;
                Log($"Time: {sw.Elapsed.TotalSeconds:F4}s | Score: {mips64:F2} MIPS");
                BenchmarkEngine.SaveResult("CPU_Int64", sw.Elapsed.TotalSeconds, mips64, "MIPS");
                SetP(50);

                Log("FMA Float Test...");
                sw.Restart();
                BenchmarkEngine.RunFloatFMA(BenchmarkEngine.ITERATIONS_FLOAT);
                sw.Stop();
                double mflops = (BenchmarkEngine.ITERATIONS_FLOAT * 4.0 / sw.Elapsed.TotalSeconds) / 1e6;
                Log($"Time: {sw.Elapsed.TotalSeconds:F4}s | Score: {mflops:F2} MFLOPS");
                BenchmarkEngine.SaveResult("CPU_FMA", sw.Elapsed.TotalSeconds, mflops, "MFLOPS");
                SetP(75);

                Log("SuperPi 1M...");
                sw.Restart();
                long chunk = BenchmarkEngine.ITERATIONS_PI / 20;
                for (int i = 0; i < 20; i++) { BenchmarkEngine.RunSuperPiSlice(chunk); }
                sw.Stop();
                double piScore = (BenchmarkEngine.ITERATIONS_PI * 2.0 / sw.Elapsed.TotalSeconds) / 1e6;
                Log($"Time: {sw.Elapsed.TotalSeconds:F4}s | Score: {piScore:F2} MFLOPS");
                BenchmarkEngine.SaveResult("CPU_SuperPi", sw.Elapsed.TotalSeconds, piScore, "MFLOPS");
                SetP(100);
            });
            start.Enabled = true;
        }
        void Log(string m) => this.Invoke(new Action(() => log.AppendText(m + Environment.NewLine)));
        void SetP(int v) => this.Invoke(new Action(() => bar.Value = v));
    }

    public class RamForm : Form
    {
        TextBox log; ProgressBar bar; Button start;
        public RamForm() { Setup(); }
        void Setup()
        {
            this.Text = "RAM Benchmark"; this.Size = new Size(500, 400); this.BackColor = Color.FromArgb(40, 40, 40); this.StartPosition = FormStartPosition.CenterScreen;
            start = new Button { Text = "START", Dock = DockStyle.Top, Height = 50, BackColor = Color.MediumPurple, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            bar = new ProgressBar { Dock = DockStyle.Top, Height = 10 };
            log = new TextBox { Dock = DockStyle.Fill, Multiline = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ReadOnly = true };
            start.Click += Run;
            this.Controls.Add(log); this.Controls.Add(bar); this.Controls.Add(start);
        }
        async void Run(object s, EventArgs e)
        {
            start.Enabled = false; log.Clear(); bar.Value = 0;
            await Task.Run(() =>
            {
                long size = 1024L * 1024 * 1024;
                Log("Allocating 1GB...");
                byte[] src = new byte[size]; byte[] dst = new byte[size];
                new Random().NextBytes(src);

                Log("Bandwidth Test...");
                var sw = Stopwatch.StartNew();
                Array.Copy(src, dst, size);
                sw.Stop();
                double speed = (size / (1024.0 * 1024.0)) / sw.Elapsed.TotalSeconds;
                Log($"Speed: {speed:F2} MB/s");
                BenchmarkEngine.SaveResult("Mem_Bandwidth", sw.Elapsed.TotalSeconds, speed, "MB/s");
                SetP(50);

                Log("Latency Test...");
                long count = 50000000; long mask = 128 * 1024 * 1024 - 1; long idx = 0;
                sw.Restart();
                for (long i = 0; i < count; i++) { idx = (idx + 4099) & mask; var v = src[idx]; }
                sw.Stop();
                double lat = (sw.Elapsed.TotalSeconds * 1e9) / count;
                Log($"Latency: {lat:F2} ns");
                BenchmarkEngine.SaveResult("Mem_Latency", sw.Elapsed.TotalSeconds, lat, "ns");
                SetP(100);
            });
            start.Enabled = true;
        }
        void Log(string m) => this.Invoke(new Action(() => log.AppendText(m + Environment.NewLine)));
        void SetP(int v) => this.Invoke(new Action(() => bar.Value = v));
    }

    public class GpuForm : Form
    {
        TextBox log; ProgressBar bar; Button start;
        public GpuForm() { Setup(); }
        void Setup()
        {
            this.Text = "GPU Benchmark"; this.Size = new Size(500, 400); this.BackColor = Color.FromArgb(40, 40, 40); this.StartPosition = FormStartPosition.CenterScreen;
            start = new Button { Text = "START", Dock = DockStyle.Top, Height = 50, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            bar = new ProgressBar { Dock = DockStyle.Top, Height = 10 };
            log = new TextBox { Dock = DockStyle.Fill, Multiline = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ReadOnly = true };
            start.Click += Run;
            this.Controls.Add(log); this.Controls.Add(bar); this.Controls.Add(start);
        }
        async void Run(object s, EventArgs e)
        {
            start.Enabled = false; log.Clear(); Log("Running 2D Stress (5s)...");
            Form win = new Form { Text = "Stress", Size = new Size(800, 600), FormBorderStyle = FormBorderStyle.None, StartPosition = FormStartPosition.CenterScreen, BackColor = Color.Black };
            PictureBox box = new PictureBox { Dock = DockStyle.Fill }; win.Controls.Add(box); win.Show();
            await Task.Run(() =>
            {
                Bitmap bmp = new Bitmap(800, 600); Graphics g = Graphics.FromImage(bmp); Random r = new Random();
                var sw = Stopwatch.StartNew(); int f = 0;
                while (sw.ElapsedMilliseconds < 5000)
                {
                    g.Clear(Color.Black);
                    for (int i = 0; i < 500; i++)
                        using (Brush b = new SolidBrush(Color.FromArgb(r.Next(255), r.Next(255), r.Next(255))))
                            g.FillRectangle(b, r.Next(800), r.Next(600), 50, 50);
                    try { if (box.InvokeRequired) box.Invoke(new Action(() => { box.Image = bmp; box.Refresh(); })); } catch { break; }
                    f++; SetP((int)(sw.ElapsedMilliseconds / 50.0));
                }
                sw.Stop();
                double fps = f / sw.Elapsed.TotalSeconds;
                Log($"FPS: {fps:F2}");
                BenchmarkEngine.SaveResult("GPU_2D", sw.Elapsed.TotalSeconds, fps, "FPS");
                this.Invoke(new Action(() => win.Close()));
            });
            start.Enabled = true;
        }
        void Log(string m) => this.Invoke(new Action(() => log.AppendText(m + Environment.NewLine)));
        void SetP(int v) => this.Invoke(new Action(() => bar.Value = Math.Min(100, v)));
    }

    public class DiskForm : Form
    {
        TextBox log; Button start; ComboBox drv;
        public DiskForm() { Setup(); }
        void Setup()
        {
            this.Text = "Disk Benchmark"; this.Size = new Size(500, 400); this.BackColor = Color.FromArgb(40, 40, 40); this.StartPosition = FormStartPosition.CenterScreen;
            Panel top = new Panel { Dock = DockStyle.Top, Height = 50 };
            drv = new ComboBox { Left = 10, Top = 15, Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var d in DriveInfo.GetDrives()) if (d.IsReady) drv.Items.Add(d.Name);
            if (drv.Items.Count > 0) drv.SelectedIndex = 0;
            start = new Button { Text = "RUN", Left = 100, Top = 10, Width = 100, Height = 30, BackColor = Color.DarkOrange, FlatStyle = FlatStyle.Flat, ForeColor = Color.White };
            start.Click += Run;
            top.Controls.Add(drv); top.Controls.Add(start);
            log = new TextBox { Dock = DockStyle.Fill, Multiline = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ReadOnly = true };
            this.Controls.Add(log); this.Controls.Add(top);
        }
        async void Run(object s, EventArgs e)
        {
            string d = drv.Text; start.Enabled = false; log.Clear();
            await Task.Run(() =>
            {
                string folder = Path.Combine(d, "TitanTemp");
                string path = Path.Combine(folder, "test.dat");
                try
                {
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    long size = 512L * 1024 * 1024;
                    byte[] buf = new byte[64 * 1024]; new Random().NextBytes(buf);
                    long chunks = size / buf.Length;

                    Log($"Writing 512MB to {d}...");
                    var sw = Stopwatch.StartNew();
                    using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
                        for (long i = 0; i < chunks; i++) fs.Write(buf, 0, buf.Length);
                    sw.Stop();
                    double ws = (size / 1024.0 / 1024.0) / sw.Elapsed.TotalSeconds;
                    Log($"Write Speed: {ws:F2} MB/s");
                    BenchmarkEngine.SaveResult($"Disk_Write_{d[0]}", sw.Elapsed.TotalSeconds, ws, "MB/s");

                    Log("Reading...");
                    sw.Restart();
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        while (fs.Read(buf, 0, buf.Length) > 0) { }
                    sw.Stop();
                    double rs = (size / 1024.0 / 1024.0) / sw.Elapsed.TotalSeconds;
                    Log($"Read Speed: {rs:F2} MB/s");
                    BenchmarkEngine.SaveResult($"Disk_Read_{d[0]}", sw.Elapsed.TotalSeconds, rs, "MB/s");
                }
                catch (Exception ex) { Log("Err: " + ex.Message); }
                finally
                {
                    try { if (File.Exists(path)) File.Delete(path); if (Directory.Exists(folder)) Directory.Delete(folder); } catch { }
                }
            });
            start.Enabled = true;
        }
        void Log(string m) => this.Invoke(new Action(() => log.AppendText(m + Environment.NewLine)));
    }

    public class ResForm : Form
    {
        TabControl tabs;
        bool showTime = false;
        ToolStripComboBox cmbTests;

        public ResForm()
        {
            this.Text = "Results";
            this.Size = new Size(1000, 600);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.StartPosition = FormStartPosition.CenterScreen;

            ToolStrip toolStrip = new ToolStrip();
            toolStrip.BackColor = Color.FromArgb(50, 50, 50);
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;

            ToolStripLabel lbl = new ToolStripLabel("Test Type:");
            lbl.ForeColor = Color.White;

            cmbTests = new ToolStripComboBox();
            cmbTests.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTests.FlatStyle = FlatStyle.Flat;
            cmbTests.SelectedIndexChanged += (s, e) => RefreshChart();

            ToolStripButton btnScore = new ToolStripButton("Show Score");
            btnScore.ForeColor = Color.White;
            btnScore.Click += (s, e) => { showTime = false; RefreshChart(); };

            ToolStripButton btnTime = new ToolStripButton("Show Time");
            btnTime.ForeColor = Color.White;
            btnTime.Click += (s, e) => { showTime = true; RefreshChart(); };

            toolStrip.Items.Add(lbl);
            toolStrip.Items.Add(cmbTests);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(btnScore);
            toolStrip.Items.Add(btnTime);

            this.Controls.Add(toolStrip);

            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            this.Controls.Add(tabs);

            LoadTests();
        }

        void LoadTests()
        {
            var tests = GetUniqueTests();
            cmbTests.Items.Clear();
            if (tests.Count > 0)
            {
                foreach (var t in tests) cmbTests.Items.Add(t);
                cmbTests.SelectedIndex = 0;
            }
            tabs.TabPages.Add(MkGridTab());
        }

        void RefreshChart()
        {
            if (tabs.TabPages.ContainsKey("CHART"))
            {
                var chartTab = tabs.TabPages["CHART"];
                tabs.TabPages.Remove(chartTab);
            }
            if (cmbTests.SelectedItem != null)
            {
                tabs.TabPages.Insert(0, MkTab(cmbTests.SelectedItem.ToString()));
                tabs.SelectedIndex = 0;
            }
        }

        List<string> GetUniqueTests()
        {
            var l = new List<string>();
            if (File.Exists("titan_results.csv"))
            {
                var lines = File.ReadAllLines("titan_results.csv");
                foreach (var line in lines.Skip(1))
                {
                    var p = line.Split(',');
                    if (p.Length > 1 && !l.Contains(p[1])) l.Add(p[1]);
                }
            }
            return l;
        }

        TabPage MkTab(string test)
        {
            TabPage p = new TabPage(test);
            p.Name = "CHART";
            p.BackColor = Color.FromArgb(40, 40, 40);

            Chart c = new Chart();
            c.Dock = DockStyle.Fill;
            c.BackColor = Color.Transparent;

            ChartArea a = new ChartArea();
            a.BackColor = Color.FromArgb(50, 50, 50);
            a.AxisX.LabelStyle.ForeColor = Color.White;
            a.AxisX.LineColor = Color.Gray;
            a.AxisY.LabelStyle.ForeColor = Color.White;
            a.AxisY.LineColor = Color.Gray;
            a.AxisY.Title = showTime ? "Time (s)" : "Score";
            a.AxisY.TitleForeColor = Color.Cyan;
            c.ChartAreas.Add(a);

            Series s = new Series();
            s.ChartType = SeriesChartType.Column;
            s.Color = showTime ? Color.Orange : Color.DodgerBlue;
            s.IsValueShownAsLabel = true;
            s.LabelForeColor = Color.White;

            if (File.Exists("titan_results.csv"))
            {
                var lines = File.ReadAllLines("titan_results.csv");
                foreach (var line in lines.Skip(1))
                {
                    var x = line.Split(',');
                    if (x.Length > 4 && x[1] == test)
                    {
                        double val = showTime ? double.Parse(x[2]) : double.Parse(x[3]);
                        s.Points.AddXY(x[0], val);
                    }
                }
            }
            c.Series.Add(s);
            p.Controls.Add(c);
            return p;
        }

        TabPage MkGridTab()
        {
            TabPage p = new TabPage("ALL DATA");
            DataGridView g = new DataGridView();
            g.Dock = DockStyle.Fill;
            g.BackgroundColor = Color.FromArgb(40, 40, 40);
            g.ForeColor = Color.Black;

            if (File.Exists("titan_results.csv"))
            {
                var lines = File.ReadAllLines("titan_results.csv");
                if (lines.Length > 0)
                {
                    var h = lines[0].Split(',');
                    g.ColumnCount = h.Length;
                    for (int i = 0; i < h.Length; i++) g.Columns[i].Name = h[i];
                    foreach (var l in lines.Skip(1)) g.Rows.Add(l.Split(','));
                }
            }
            p.Controls.Add(g);
            return p;
        }
    }
}