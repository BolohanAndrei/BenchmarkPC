using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices; 
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BoloBenchmarkPro
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
        public const string CSV_FILE = "bolo_results.csv"; 

        public static long ResultInt64 = 0;
        public static int ResultInt32 = 0;
        public static double ResultDouble = 0.0;

        public static void RunInteger64Logic(long iterations)
        {
            long a = 12345; long b = 67890; long c = 101112; long local_sum = 0;
            unchecked
            {
                for (long i = 0; i < iterations; i++)
                {
                    a = a + b; b = a * c; c = b ^ 0xFF; a = c >> 3; b = a | 0xAA; c = b & a;
                    local_sum += c;
                }
            }
            ResultInt64 = local_sum;
        }

        public static void RunInteger32Logic(long iterations)
        {
            int a = 12345; int b = 67890; int c = 101112; int local_sum = 0;
            unchecked
            {
                for (long i = 0; i < iterations; i++)
                {
                    a = a + b; b = a * c; c = b ^ 0xFF; a = c >> 3; b = a | 0xAA; c = b & a;
                    local_sum += c;
                }
            }
            ResultInt32 = local_sum;
        }

        public static double RunFloatFMA(long iterations)
        {
            double x = 1.234; double y = 5.678; double z = 9.012; double local_sum = 0.0;
            for (long i = 0; i < iterations; i++)
            {
                x = x + y; y = x * z; z = y / 1.5; x = (x * y) + z;
                local_sum += x;
            }
            ResultDouble = local_sum;
            return local_sum;
        }

        public static void RunSuperPiSlice(long iterations)
        {
            double pi = 0; double sign = 1;
            for (long i = 0; i < iterations; i++)
            {
                pi += sign / (2.0 * i + 1.0); sign = -sign;
            }
            if (ResultDouble == 0) ResultDouble = pi;
        }

        public static double RunMatrixGFlops(int matrixSize)
        {
            double[] A = new double[matrixSize * matrixSize];
            double[] B = new double[matrixSize * matrixSize];
            double[] C = new double[matrixSize * matrixSize];

            Parallel.For(0, A.Length, i => { A[i] = 1.001; B[i] = 2.002; });

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.For(0, matrixSize, i =>
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < matrixSize; k++)
                    {
                        sum += A[i * matrixSize + k] * B[k * matrixSize + j];
                    }
                    C[i * matrixSize + j] = sum;
                }
            });

            sw.Stop();

            double ops = 2.0 * matrixSize * matrixSize * matrixSize;
            double gflops = (ops / sw.Elapsed.TotalSeconds) / 1e9;
            return gflops;
        }

        public static void SaveResult(string test, double time, double perf, string unit)
        {
            try
            {
                bool head = !File.Exists(CSV_FILE);
                using (StreamWriter w = new StreamWriter(CSV_FILE, true))
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
            this.Text = "Login"; this.Size = new Size(400, 200); this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog; this.BackColor = Color.FromArgb(30, 30, 30); this.ForeColor = Color.White;
            Label lbl = new Label { Text = "Nume PC:", Location = new Point(30, 40), AutoSize = true, Font = new Font("Segoe UI", 11), ForeColor = Color.White };
            txtName = new TextBox { Location = new Point(30, 70), Width = 320, Text = Environment.MachineName };
            Button btn = new Button { Text = "START", Location = new Point(250, 110), Width = 100, BackColor = Color.SeaGreen, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, DialogResult = DialogResult.OK };
            btn.Click += (s, e) => PCName = txtName.Text;
            this.Controls.Add(lbl); this.Controls.Add(txtName); this.Controls.Add(btn); this.AcceptButton = btn;
        }
    }

    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            this.Text = $"Bolo Benchmark - {Program.CurrentPCName}"; this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen; this.BackColor = Color.FromArgb(30, 30, 30); this.ForeColor = Color.White;
            InitLayout();
        }

        private void InitLayout()
        {
            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, FixedPanel = FixedPanel.Panel1, IsSplitterFixed = true, SplitterDistance = 300, SplitterWidth = 1 };
            this.Controls.Add(split);

            Panel left = split.Panel1; left.BackColor = Color.FromArgb(45, 45, 48); left.Padding = new Padding(20);
            Label lblTitle = new Label { Text = "SYSTEM INFO", Dock = DockStyle.Top, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.Cyan, Height = 40 };
            Label lblInfo = new Label { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), ForeColor = Color.LightGray, Text = "Scanning..." };
            left.Controls.Add(lblInfo); left.Controls.Add(lblTitle);

            Panel right = split.Panel2; right.BackColor = Color.FromArgb(30, 30, 30); right.Padding = new Padding(40);
            TableLayoutPanel grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3 };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F)); grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F)); grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));

            grid.Controls.Add(MkBtn("CPU BENCHMARK", Color.RoyalBlue, () => new CpuForm().Show()), 0, 0);
            grid.Controls.Add(MkBtn("RAM BENCHMARK", Color.MediumPurple, () => new RamForm().Show()), 1, 0);
            grid.Controls.Add(MkBtn("GPU STRESS", Color.Crimson, () => new GpuForm().Show()), 0, 1);
            grid.Controls.Add(MkBtn("DISK BENCHMARK", Color.DarkOrange, () => new DiskForm().Show()), 1, 1);
            grid.Controls.Add(MkBtn("RUN ALL", Color.SeaGreen, RunAll), 0, 2);
            grid.Controls.Add(MkBtn("RESULTS", Color.Teal, () => new ResForm().Show()), 1, 2);

            right.Controls.Add(grid);

            Task.Run(() => { string s = GetInfo(); this.Invoke(new Action(() => lblInfo.Text = s)); });
        }

        private Button MkBtn(string t, Color c, Action a)
        {
            Button b = new Button { Text = t, Dock = DockStyle.Fill, BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Margin = new Padding(15), Cursor = Cursors.Hand };
            b.Click += (s, e) => a(); return b;
        }

        private void RunAll() { new CpuForm().Show(); new RamForm().Show(); new GpuForm().Show(); new DiskForm().Show(); }

        private string GetInfo()
        {
            string s = "";
            try
            {
                using (var mos = new ManagementObjectSearcher("select Name, NumberOfCores, MaxClockSpeed from Win32_Processor"))
                    foreach (var o in mos.Get()) s += $"CPU:\n{o["Name"]}\nCores: {o["NumberOfCores"]} @ {o["MaxClockSpeed"]} MHz\n\n";
                using (var mos = new ManagementObjectSearcher("select Capacity from Win32_PhysicalMemory")) { long t = 0; foreach (var o in mos.Get()) t += Convert.ToInt64(o["Capacity"]); s += $"RAM:\n{t / 1073741824} GB\n\n"; }
                using (var mos = new ManagementObjectSearcher("select Name from Win32_VideoController")) foreach (var o in mos.Get()) s += $"GPU:\n{o["Name"]}\n\n";
                using (var mos = new ManagementObjectSearcher("select Caption from Win32_OperatingSystem")) foreach (var o in mos.Get()) s += $"OS:\n{o["Caption"]}\n";
            }
            catch { s = "N/A"; }
            return s;
        }
    }

    public class CpuForm : Form
    {
        TextBox log; ProgressBar bar; Button start; Label lblFreq;
        System.Windows.Forms.Timer tmr; PerformanceCounter cpuCounter;
        double baseClock = 0;

        public CpuForm() { Setup(); }
        void Setup()
        {
            this.Text = "CPU Benchmark"; this.Size = new Size(600, 600); this.BackColor = Color.FromArgb(40, 40, 40); this.StartPosition = FormStartPosition.CenterScreen;
            start = new Button { Text = "START", Dock = DockStyle.Top, Height = 50, BackColor = Color.RoyalBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            bar = new ProgressBar { Dock = DockStyle.Top, Height = 10 };
            lblFreq = new Label { Dock = DockStyle.Top, Height = 30, ForeColor = Color.Yellow, Font = new Font("Consolas", 14, FontStyle.Bold), Text = "Frequency: Measuring..." };
            log = new TextBox { Dock = DockStyle.Fill, Multiline = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ReadOnly = true };
            start.Click += Run;
            this.Controls.Add(log); this.Controls.Add(lblFreq); this.Controls.Add(bar); this.Controls.Add(start);

            Task.Run(() => {
                try
                {
                    using (var searcher = new ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor"))
                        foreach (var item in searcher.Get()) baseClock = Convert.ToDouble(item["MaxClockSpeed"]);
                }
                catch { baseClock = 2500; }
            });

            try
            {
                cpuCounter = new PerformanceCounter("Processor Information", "% Processor Performance", "_Total");
                tmr = new System.Windows.Forms.Timer { Interval = 500 };
                tmr.Tick += (s, e) => {
                    float percent = cpuCounter.NextValue();
                    double realFreq = baseClock * (percent / 100.0);
                    lblFreq.Text = $"Frequency: {(realFreq / 1000.0):F2} GHz";
                };
                tmr.Start();
            }
            catch { lblFreq.Text = "Freq: Admin Required"; }
        }

        async void Run(object s, EventArgs e)
        {
            start.Enabled = false; log.Clear(); bar.Value = 0;
            await Task.Run(() =>
            {
                Log("Int32 Test..."); var sw = Stopwatch.StartNew(); BenchmarkEngine.RunInteger32Logic(BenchmarkEngine.ITERATIONS_INT); sw.Stop();
                double mips32 = (BenchmarkEngine.ITERATIONS_INT * 6.0 / sw.Elapsed.TotalSeconds) / 1e6;
                Log($"Time: {sw.Elapsed.TotalSeconds:F4}s | Score: {mips32:F2} MIPS"); BenchmarkEngine.SaveResult("CPU_Int32", sw.Elapsed.TotalSeconds, mips32, "MIPS"); SetP(25);

                Log("Int64 Test..."); sw.Restart(); BenchmarkEngine.RunInteger64Logic(BenchmarkEngine.ITERATIONS_INT); sw.Stop();
                double mips64 = (BenchmarkEngine.ITERATIONS_INT * 6.0 / sw.Elapsed.TotalSeconds) / 1e6;
                Log($"Time: {sw.Elapsed.TotalSeconds:F4}s | Score: {mips64:F2} MIPS"); BenchmarkEngine.SaveResult("CPU_Int64", sw.Elapsed.TotalSeconds, mips64, "MIPS"); SetP(50);

                Log("FMA Float Test..."); sw.Restart(); BenchmarkEngine.RunFloatFMA(BenchmarkEngine.ITERATIONS_FLOAT); sw.Stop();
                double mflops = (BenchmarkEngine.ITERATIONS_FLOAT * 4.0 / sw.Elapsed.TotalSeconds) / 1e6;
                Log($"Time: {sw.Elapsed.TotalSeconds:F4}s | Score: {mflops:F2} MFLOPS"); BenchmarkEngine.SaveResult("CPU_FMA", sw.Elapsed.TotalSeconds, mflops, "MFLOPS"); SetP(75);

                Log("SuperPi 1M..."); sw.Restart(); long chunk = BenchmarkEngine.ITERATIONS_PI / 20; for (int i = 0; i < 20; i++) BenchmarkEngine.RunSuperPiSlice(chunk); sw.Stop();
                double piScore = (BenchmarkEngine.ITERATIONS_PI * 2.0 / sw.Elapsed.TotalSeconds) / 1e6;
                Log($"Time: {sw.Elapsed.TotalSeconds:F4}s | Score: {piScore:F2} MFLOPS"); BenchmarkEngine.SaveResult("CPU_SuperPi", sw.Elapsed.TotalSeconds, piScore, "MFLOPS"); SetP(100);
            });
            start.Enabled = true;
        }
        void Log(string m) => this.Invoke(new Action(() => log.AppendText(m + Environment.NewLine)));
        void SetP(int v) => this.Invoke(new Action(() => bar.Value = v));
    }

    public class RamForm : Form
    {
        [DllImport("kernel32.dll")]
        static extern bool SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        TextBox log; ProgressBar bar; Button startSpeed, startFill;

        public RamForm() { Setup(); }
        void Setup()
        {
            this.Text = "RAM Benchmark"; this.Size = new Size(500, 500);
            this.BackColor = Color.FromArgb(40, 40, 40); this.StartPosition = FormStartPosition.CenterScreen;
            startSpeed = new Button { Text = "SPEED TEST (Bandwidth/Latency)", Dock = DockStyle.Top, Height = 40, BackColor = Color.MediumPurple, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            startFill = new Button { Text = "STRESS FILL (Capacity Test)", Dock = DockStyle.Top, Height = 40, BackColor = Color.Indigo, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            bar = new ProgressBar { Dock = DockStyle.Top, Height = 15 };
            log = new TextBox { Dock = DockStyle.Fill, Multiline = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ReadOnly = true, ScrollBars = ScrollBars.Vertical };
            startSpeed.Click += RunSpeed;
            startFill.Click += RunFill;
            this.Controls.Add(log); this.Controls.Add(bar);
            this.Controls.Add(startFill); this.Controls.Add(startSpeed);
        }

        async void RunSpeed(object s, EventArgs e)
        {
            ToggleBtns(false); log.Clear(); bar.Value = 0;
            await Task.Run(() =>
            {
                long size = 1024L * 1024 * 1024; // 1GB
                Log("Allocating 1GB for Speed Test...");
                try
                {
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
                }
                catch (Exception ex) { Log("Err: " + ex.Message); }
                finally { ForceMemClear(); }
            });
            ToggleBtns(true);
        }

        async void RunFill(object s, EventArgs e)
        {
            ToggleBtns(false); log.Clear(); bar.Value = 0;
            if (!Environment.Is64BitProcess) Log("⚠️ Running in 32-bit! Max ~3GB limit.\r\n");

            await Task.Run(() =>
            {
                Log("Starting FILL Stress Test...");
                List<byte[]> chunks = new List<byte[]>();
                long totalAllocated = 0;
                int chunkSize = 256 * 1024 * 1024;
                Stopwatch sw = Stopwatch.StartNew();

                try
                {
                    var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
                    ulong totalPhys = computerInfo.TotalPhysicalMemory;
                    while (computerInfo.AvailablePhysicalMemory > 512 * 1024 * 1024)
                    {
                        try
                        {
                            byte[] chunk = new byte[chunkSize];
                            for (int k = 0; k < chunk.Length; k += 4096 * 10) chunk[k] = 0xFF;
                            chunks.Add(chunk);
                            totalAllocated += chunkSize;
                            computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
                            double percent = (double)totalAllocated / (double)(totalPhys - 500 * 1024 * 1024) * 100.0;
                            SetP((int)Math.Min(percent, 95));
                            if (totalAllocated % (1024 * 1024 * 1024) == 0)
                                this.Invoke(new Action(() => log.AppendText($"Filled: {totalAllocated / 1024 / 1024} MB...\r\n")));
                        }
                        catch (OutOfMemoryException) { Log("Process Limit Reached."); break; }
                    }
                }
                catch (Exception ex) { Log($"Stopped: {ex.Message}"); }

                sw.Stop();
                long finalMB = totalAllocated / 1024 / 1024;
                Log($"\r\nDONE. Filled {finalMB} MB in {sw.Elapsed.TotalSeconds:F2}s");
                BenchmarkEngine.SaveResult("RAM_Stress_Cap", sw.Elapsed.TotalSeconds, finalMB, "MB_Filled");

                Log("Holding memory for 2 seconds...");
                SetP(100);
                Thread.Sleep(2000);

                Log("Releasing Memory...");
                chunks = null;
                ForceMemClear();
                Log("Memory Cleared.");
            });
            ToggleBtns(true);
        }

        void ForceMemClear()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                try { SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1); } catch { }
            }
        }

        void ToggleBtns(bool en) { startSpeed.Enabled = en; startFill.Enabled = en; }
        void Log(string m) => this.Invoke(new Action(() => log.AppendText(m + Environment.NewLine)));
        void SetP(int v) => this.Invoke(new Action(() => bar.Value = Math.Min(100, v)));
    }

    public class GpuForm : Form
    {
        TextBox log; ProgressBar bar; Button start2D, startCompute;

        public GpuForm() { Setup(); }
        void Setup()
        {
            this.Text = "GPU & Compute Benchmark"; this.Size = new Size(500, 450);
            this.BackColor = Color.FromArgb(40, 40, 40); this.StartPosition = FormStartPosition.CenterScreen;
            start2D = new Button { Text = "START 2D RASTER", Dock = DockStyle.Top, Height = 40, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            startCompute = new Button { Text = "START COMPUTE (GFLOPS)", Dock = DockStyle.Top, Height = 40, BackColor = Color.Firebrick, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            bar = new ProgressBar { Dock = DockStyle.Top, Height = 10 };
            log = new TextBox { Dock = DockStyle.Fill, Multiline = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ReadOnly = true };
            start2D.Click += Run2D; startCompute.Click += RunCompute;
            this.Controls.Add(log); this.Controls.Add(bar); this.Controls.Add(startCompute); this.Controls.Add(start2D);
        }

        async void Run2D(object s, EventArgs e)
        {
            ToggleBtns(false); log.Clear(); Log("Running 2D Stress (5s)...");
            Form win = new Form { Text = "Stress", Size = new Size(800, 600), FormBorderStyle = FormBorderStyle.None, StartPosition = FormStartPosition.CenterScreen, BackColor = Color.Black, TopMost = true };
            PictureBox box = new PictureBox { Dock = DockStyle.Fill }; win.Controls.Add(box); win.Show();
            await Task.Run(() => {
                Bitmap bmp = new Bitmap(800, 600); Graphics g = Graphics.FromImage(bmp); Random r = new Random();
                var sw = Stopwatch.StartNew(); int f = 0;
                while (sw.ElapsedMilliseconds < 5000)
                {
                    g.Clear(Color.Black);
                    for (int i = 0; i < 300; i++) using (Brush b = new SolidBrush(Color.FromArgb(r.Next(255), r.Next(255), r.Next(255)))) g.FillRectangle(b, r.Next(800), r.Next(600), 40, 40);
                    try { if (box.InvokeRequired) box.Invoke(new Action(() => { box.Image = bmp; box.Refresh(); })); } catch { break; }
                    f++; SetP((int)(sw.ElapsedMilliseconds / 50.0));
                }
                sw.Stop();
                double fps = f / sw.Elapsed.TotalSeconds;
                Log($"FPS: {fps:F2}");
                BenchmarkEngine.SaveResult("GPU_2D", sw.Elapsed.TotalSeconds, fps, "FPS");
                this.Invoke(new Action(() => win.Close()));
            });
            ToggleBtns(true);
        }

        async void RunCompute(object s, EventArgs e)
        {
            ToggleBtns(false); log.Clear(); SetP(0);
            Log("Calculating GFLOPS (CPU SIMD Simulation)...");
            await Task.Run(() => {
                BenchmarkEngine.RunMatrixGFlops(400); SetP(20); 
                Log("Heavy Matrix Load...");
                int matrixSize = 1200;

                Stopwatch sw = Stopwatch.StartNew();
                double gflops = BenchmarkEngine.RunMatrixGFlops(matrixSize);
                sw.Stop();

                Log($"Score: {gflops:F2} GFLOPS");
                Log($"Time: {sw.Elapsed.TotalSeconds:F4}s");

                BenchmarkEngine.SaveResult("GPU_Compute", sw.Elapsed.TotalSeconds, gflops, "GFLOPS");
                SetP(100);
            });
            ToggleBtns(true);
        }
        void ToggleBtns(bool en) { start2D.Enabled = en; startCompute.Enabled = en; }
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
            await Task.Run(() => {
                string folder = Path.Combine(d, "BoloTemp"); string path = Path.Combine(folder, "test.dat");
                try
                {
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    long size = 512L * 1024 * 1024; byte[] buf = new byte[64 * 1024]; new Random().NextBytes(buf); long chunks = size / buf.Length;
                    Log($"Writing 512MB to {d}...");
                    var sw = Stopwatch.StartNew();
                    using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough)) for (long i = 0; i < chunks; i++) fs.Write(buf, 0, buf.Length);
                    sw.Stop();
                    double ws = (size / 1048576.0) / sw.Elapsed.TotalSeconds;
                    Log($"Write Speed: {ws:F2} MB/s");
                    BenchmarkEngine.SaveResult($"Disk_Write_{d[0]}", sw.Elapsed.TotalSeconds, ws, "MB/s");
                    Log("Reading..."); sw.Restart();
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read)) while (fs.Read(buf, 0, buf.Length) > 0) { }
                    sw.Stop();
                    double rs = (size / 1048576.0) / sw.Elapsed.TotalSeconds;
                    Log($"Read Speed: {rs:F2} MB/s");
                    BenchmarkEngine.SaveResult($"Disk_Read_{d[0]}", sw.Elapsed.TotalSeconds, rs, "MB/s");
                }
                catch (Exception ex) { Log("Err: " + ex.Message); }
                finally { try { if (File.Exists(path)) File.Delete(path); if (Directory.Exists(folder)) Directory.Delete(folder); } catch { } }
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
        Chart chart; 

        public ResForm()
        {
            this.Text = "Results - Average & Analytics";
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
            cmbTests.SelectedIndexChanged += (s, e) => UpdateChartData();

            ToolStripButton btnScore = new ToolStripButton("Show Score");
            btnScore.ForeColor = Color.White;
            btnScore.Click += (s, e) => { showTime = false; UpdateChartData(); };

            ToolStripButton btnTime = new ToolStripButton("Show Time");
            btnTime.ForeColor = Color.White;
            btnTime.Click += (s, e) => { showTime = true; UpdateChartData(); };

            toolStrip.Items.Add(lbl);
            toolStrip.Items.Add(cmbTests);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(btnScore);
            toolStrip.Items.Add(btnTime);

            this.Controls.Add(toolStrip);

            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            this.Controls.Add(tabs);

            InitTabs();      
            LoadTestList();  
        }

        void InitTabs()
        {
            TabPage chartPage = new TabPage("CHART ANALYSIS");
            chartPage.BackColor = Color.FromArgb(40, 40, 40);

            chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.BackColor = Color.Transparent;

            ChartArea a = new ChartArea();
            a.BackColor = Color.FromArgb(50, 50, 50);
            a.AxisX.LabelStyle.ForeColor = Color.White;
            a.AxisX.LineColor = Color.Gray;
            a.AxisX.MajorGrid.LineColor = Color.FromArgb(70, 70, 70);

            a.AxisY.LabelStyle.ForeColor = Color.White;
            a.AxisY.LineColor = Color.Gray;
            a.AxisY.MajorGrid.LineColor = Color.FromArgb(70, 70, 70);
            a.AxisY.TitleForeColor = Color.Cyan;
            a.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);

            chart.ChartAreas.Add(a);
            chartPage.Controls.Add(chart);
            tabs.TabPages.Add(chartPage);

            tabs.TabPages.Add(MkGridTab());
        }

        void LoadTestList()
        {
            var tests = GetUniqueTests();
            cmbTests.Items.Clear();
            if (tests.Count > 0)
            {
                foreach (var t in tests) cmbTests.Items.Add(t);
                cmbTests.SelectedIndex = 0;
            }
        }

        void UpdateChartData()
        {
            if (cmbTests.SelectedItem == null) return;
            string selectedTest = cmbTests.SelectedItem.ToString();

            var area = chart.ChartAreas[0];
            area.AxisY.Title = showTime ? "Time (Average Seconds)" : "Score (Average)";

            chart.Series.Clear();

            Series s = new Series();
            s.Name = showTime ? "Time" : "Score";
            s.ChartType = SeriesChartType.Column;
            s.Color = showTime ? Color.Orange : Color.DodgerBlue;
            s.IsValueShownAsLabel = true;
            s.LabelForeColor = Color.White;
            s.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            s.LabelFormat = "F2";

            double maxVal = 0;

            if (File.Exists(BenchmarkEngine.CSV_FILE))
            {
                var lines = File.ReadAllLines(BenchmarkEngine.CSV_FILE);

                var query = lines.Skip(1)
                    .Select(line => line.Split(','))
                    .Where(p => p.Length > 4 && p[1] == selectedTest)
                    .GroupBy(p => p[0]) 
                    .Select(g => new
                    {
                        PCName = g.Key,
                        AvgTime = g.Average(x => double.Parse(x[2], System.Globalization.CultureInfo.InvariantCulture)),
                        AvgScore = g.Average(x => double.Parse(x[3], System.Globalization.CultureInfo.InvariantCulture))
                    })
                    .OrderBy(x => showTime ? x.AvgTime : -x.AvgScore)
                    .ToList();

                foreach (var item in query)
                {
                    double val = showTime ? item.AvgTime : item.AvgScore;
                    if (val > maxVal) maxVal = val;
                    s.Points.AddXY(item.PCName, val);
                }
            }

            if (maxVal > 0) area.AxisY.Maximum = maxVal * 1.2;
            else area.AxisY.Maximum = Double.NaN; 

            chart.Series.Add(s);
        }

        List<string> GetUniqueTests()
        {
            var l = new List<string>();
            if (File.Exists(BenchmarkEngine.CSV_FILE))
            {
                var lines = File.ReadAllLines(BenchmarkEngine.CSV_FILE);
                foreach (var line in lines.Skip(1))
                {
                    var p = line.Split(',');
                    if (p.Length > 1 && !l.Contains(p[1])) l.Add(p[1]);
                }
            }
            l.Sort();
            return l;
        }

        TabPage MkGridTab()
        {
            TabPage p = new TabPage("RAW DATA (GRID)");
            DataGridView g = new DataGridView();
            g.Dock = DockStyle.Fill;
            g.BackgroundColor = Color.FromArgb(40, 40, 40);
            g.ForeColor = Color.Black;
            g.AllowUserToAddRows = false;
            g.ReadOnly = true;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (File.Exists(BenchmarkEngine.CSV_FILE))
            {
                var lines = File.ReadAllLines(BenchmarkEngine.CSV_FILE);
                if (lines.Length > 0)
                {
                    var h = lines[0].Split(',');
                    g.ColumnCount = h.Length;
                    for (int i = 0; i < h.Length; i++) g.Columns[i].Name = h[i];
                    for (int i = lines.Length - 1; i > 0; i--) g.Rows.Add(lines[i].Split(','));
                }
            }
            p.Controls.Add(g);
            return p;
        }
    }
}