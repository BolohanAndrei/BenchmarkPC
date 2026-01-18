# ⚡ BoloBenchmark Pro

**BoloBenchmark Pro** is a comprehensive benchmarking suite developed in C# (.NET) designed to evaluate the raw performance capabilities of modern computer systems. Originally developed as an academic project for the **Technical University of Cluj-Napoca**, it has evolved into a professional Windows Forms application offering real-time visualization, hardware stress testing, and comparative analysis.

## 🚀 About The Project

In the context of diverse modern hardware, objective performance evaluation is critical. BoloBenchmark Pro provides a unified solution to test critical components (CPU, RAM, GPU, Disk) using synthetic algorithms inspired by industry standards like SPEC CPU2017, Whetstone, and Dhrystone.

**Key differentiators:**
* **Native Performance in Managed Code:** Utilizes C# `unchecked` blocks to disable overflow checking, achieving arithmetic execution speeds comparable to C++.
* **Modular Architecture:** The core logic is isolated in a static `BenchmarkEngine`, ensuring the GUI does not overhead the test results.
* **Multithreading:** All tests run on separate threads to prevent UI freezing and ensure accurate timing.

---

## 🎯 Methodology & Metrics

### 🧠 CPU Performance
Evaluates the Architecture via ALU and FPU stress testing.
* **Integer Operations (32/64-bit):** Massive execution of arithmetic and bitwise logic ($A = A + B$, $C = B \oplus 0xFF$).
    * *Metric:* **MIPS** (Million Instructions Per Second).
* **Floating-Point (FMA):** Simulation of Fused Multiply-Add instructions ($X = (X \times Y) + Z$).
    * *Metric:* **MFLOPS** (Million Floating-Point Operations Per Second).
* **SuperPi (Leibniz Series):** Single-threaded calculation of $\pi$ using the infinite Leibniz series to stress cache and precision.
    * *Metric:* Calculation Time / Score.

### 💾 Memory Subsystem
* **Bandwidth:** Measures transfer speeds using optimized `Array.Copy` on 1GB blocks.
    * *Metric:* **MB/s**.
* **Latency:** Uses "pointer chasing" techniques to force random access and cache misses.
    * *Metric:* **ns** (Nanoseconds).
* **Stress Test:** Dynamic allocation of 256MB chunks until the OS physical memory limit is reached (Crash Protection included).

### 🎮 GPU & Graphics
* **2D Rasterization:** GDI+ stress test rendering thousands of semi-transparent rectangles.
    * *Metric:* **FPS** (Frames Per Second).
* **Compute Simulation:** Parallel execution of mathematical operations across all available logical cores to simulate shader workloads.
    * *Metric:* **GFLOPS** (Simulated).

### 💿 Storage I/O
* **Sequential R/W:** Creates a temporary file in a safe local directory (`BoloTemp`) to measure sustained throughput without permission errors.
    * *Metric:* **MB/s**.

---

## ✨ Features

* **🖥️ Modern Dashboard:** Dark-themed, responsive GUI based on TableLayoutPanel grid architecture.
* **🔍 Auto-Detection:** Automatically identifies CPU model, RAM size, GPU name, and OS version via WMI (`System.Management`).
* **📊 Integrated Visualization:** Built-in charts allow instant comparison of Score vs. Time for all performed tests.
* **📂 Data Logging:** Results are automatically appended to `bolo_results.csv` for historical tracking and cross-system comparison.
* **🛡️ Safety First:** Implements memory limit protections (stops at 1GB free RAM) and safe temporary file handling.

---

## 🛠️ Project Roadmap

This project follows a structured development lifecycle:

| Phase | Focus | Description | Status |
| :--- | :--- | :--- | :---: |
| **Phase 1** | **Research** | Analysis of SPEC CPU2017 & Amdahl's Law. Definition of metrics (MIPS, MFLOPS). | ✅ |
| **Phase 2** | **CPU Core** | Implementation of arithmetic/logic modules and execution time measurement. | ✅ |
| **Phase 3** | **Memory & I/O** | Implementation of block transfer (Bandwidth), Random Access (Latency), and File I/O. | ✅ |
| **Phase 4** | **Automation & GUI** | Hardware detection, automatic reporting, comparative graphs, and UI polishing. | ✅ |
| **Phase 5** | **Final Analysis** | Comparative testing between systems, geometric mean analysis, and final documentation. | ✅ |

---

## 🛠️ Tech Stack

* **Language:** C# (.NET Framework 4.7.2+)
* **Framework:** Windows Forms (WinForms)
* **Libraries:**
    * `System.Management` (WMI Hardware Info)
    * `System.Windows.Forms.DataVisualization` (Charting)
    * `System.Threading.Tasks` (Parallelism)

---

## 📌 Getting Started

### Option 1: Run the Executable
1.  Go to the **Releases** page.
2.  Download `BoloBenchmarkPro.exe`.
3.  Run directly on any Windows PC (No installation required).

### Option 2: Build from Source
Clone the repository:
```bash
git clone https://github.com/BolohanAndrei/BenchmarkPC.git
```
Open the solution .sln in Visual Studio 2022.

Important: Select Release mode and ensure the platform is set to x64 to allow full RAM allocation during stress tests.

Build and Run.

## 📬 Contact

Andrei Bolohan

Email: bolohanandrei769@gmail.com

⭐ Show Your Support: If you find this project useful for your studies or testing, please give it a star!
