# ⚡ PC Performance Benchmark

A comprehensive C# & Windows Forms benchmarking suite designed to measure the CPU, Memory, GPU, and Disk I/O capabilities of modern computer systems with a professional, easy-to-use interface.

---

## 🚀 About The Project

Originally inspired by standard benchmarks like SPEC CPU2017 and SuperPi, Titan Benchmark Pro has evolved from a console script into a full-fledged GUI application. It provides real-time feedback, hardware detection, and interactive comparison charts.

Key Capabilities:

✅ Integer Operations: 32-bit & 64-bit Arithmetic Logic Unit (ALU) stress testing.

✅ Floating-Point: Fused Multiply-Add (FMA) simulation for scientific calculation performance.

✅ Memory Stress: RAM Bandwidth (MB/s) & Latency (ns) measurement.

✅ GPU 2D Stress: GDI+ Rendering Benchmark to test Windows interface fluidity.

✅ Disk I/O: Sequential Read/Write speed tests for any selected drive.

---

## 🎯 Core Objectives

- **CPU Performance**
  - Integer 32-bit & 64-bit ➜ **MIPS**
  - Float FMA & SuperPi Sim ➜ **MFLOPS**
- **Memory Performance**
  - Block Copy (1GB) ➜ **MB/s**
  - Random Access ➜ **ns**
- **Disk I/O Performance**
  - Sequential R/W (1GB) ➜ **MB/s**
- **GPU**
  - 2D Rendering Stress ➜ **FPS**

---

## ✨ Features

🖥️ Modern GUI: Dark mode interface with responsive Grid Layout.

🔍 Hardware Detection: Automatically identifies CPU model, RAM capacity, GPU, and OS using WMI.

📊 Built-in Visualization: No external Python scripts needed! The app generates bar charts comparing your Score vs. Execution Time directly in the "Results" tab.

📁 Auto-Logging: All results are automatically appended to titan_results.csv for long-term tracking.

🛡️ Safe Disk Testing: Uses temporary folders for I/O testing to prevent data loss or permission errors.

---

## 🛠️ Project Roadmap

| Phase              |     Status     | Description                                 |
| ------------------ | :------------: | ------------------------------------------- |
| Research & Design  |       ✅       | Benchmarks defined, architecture finalized  |
| Core CPU Benchmark |       ✅       | C++ MIPS/MFLOPS + CSV logging complete      |
| Memory & I/O Tests |       ✅       | RAM & disk performance evaluation           |
| Reporting System   |       ✅       | Auto hardware detection & performance index |
| Final Analysis     |       ✅       | Comparative insights + final report         |

---

## 🛠️ Tech Stack

Language: C# (.NET Framework / .NET Core)

Framework: Windows Forms (WinForms)

Libraries:

- System.Management (Hardware Info)

- System.Windows.Forms.DataVisualization (Charts)

---

📌 Getting Started

Option 1: Run the Executable

1. Go to the [Releases](https://github.com/BolohanAndrei/BenchmarkPC/releases) page.

2. Download TitanBenchmarkPro.exe.

3. Run it directly on any Windows PC (No installation required).

Option 2: Build from Source (Visual Studio)

1. Clone the repo:
```
git clone [https://github.com/BolohanAndrei/BenchmarkPC.git](https://github.com/BolohanAndrei/BenchmarkPC.git)
```


2. Open the solution file (.sln) in Visual Studio 2019/2022.

3. Ensure NuGet packages are restored (Right-click Solution -> Restore NuGet Packages).

4. Select Release mode.

5. Hit Start.
6. 
---

🤝 Contributions

Contributions, optimization suggestions, and PRs are welcome!
Let’s build a reliable open-source benchmark together 🔥

---

📬 Contact

👤 Andrei Bolohan
✉️ bolohanandrei769@gmail.com

🔗 LinkedIn: https://www.linkedin.com/in/andrei-bolohan/

📌 Repository Link:
https://github.com/BolohanAndrei/BenchmarkPC

⭐ Show Your Support

If this project helps you —
please star the repo ⭐ to help visibility!
