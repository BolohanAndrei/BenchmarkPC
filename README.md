# ⚡ PC Performance Benchmark

A high-performance **C++ & Python** benchmarking suite designed to measure **CPU**, **Memory**, and **I/O** capabilities of modern computer systems.

---

## 🚀 About The Project

This tool provides a synthetic benchmark inspired by **SPEC CPU2017**, **Whetstone**, and **Dhrystone** to deliver relevant performance metrics across:

✅ Integer operations  
✅ Floating-point calculations  
✅ Memory & RAM access (coming soon)  
✅ Disk I/O operations (coming soon)

Developed in **C++** for raw system testing and **Python** for visualization & data analytics.

---

## 🎯 Core Objectives

- **CPU Performance**
  - Integer benchmark ➜ **MIPS**
  - Floating-point benchmark ➜ **MFLOPS**
- **Memory Performance**
  - Memory bandwidth (**MB/s**) & latency _(planned)_
- **Disk I/O Performance**
  - Large sequential file R/W speed _(planned)_
- **Cross-System Comparison**
  - Normalized **geometric mean** for fair ranking

---

## ✨ Features

### 🖥️ C++ Benchmark Engine — `main.cpp`

👉 Executes intensive CPU workloads

| Test Type      | Operations                    | Output |
| -------------- | ----------------------------- | ------ |
| Integer        | ADD, MUL, XOR, SHIFT, OR, AND | MIPS   |
| Floating-Point | ADD, MUL, DIV, FMA            | MFLOPS |

📤 Results are automatically appended to **benchmark_results.csv**

---

### 📊 Python Visualization — `visualize.py`

👉 Generates **interactive** charts using **Altair**

- Mean execution time by system & test type
- Mean performance score (MIPS/MFLOPS)

Output:

- `barchart_timp_mediu.html`
- `barchart_performanta_medie.html`

---

## 🛠️ Project Roadmap

| Phase              |     Status     | Description                                 |
| ------------------ | :------------: | ------------------------------------------- |
| Research & Design  |       ✅       | Benchmarks defined, architecture finalized  |
| Core CPU Benchmark | ⌛ In Progress | C++ MIPS/MFLOPS + CSV logging complete      |
| Memory & I/O Tests |    🚧 Next     | RAM & disk performance evaluation           |
| Reporting System   |   📅 Planned   | Auto hardware detection & performance index |
| Final Analysis     |   📅 Planned   | Comparative insights + final report         |

---

## 📌 Getting Started

### 🔹 Part 1 — Run the Benchmark (C++)

#### Requirements

- C++17 compatible compiler

#### Compile

```bash
g++ -o benchmark main.cpp -std=c++17 -O2
```

Run

```bash
./benchmark
```

Output ➜ Console + benchmark_results.csv

### 🔹 Part 2 — Visualize Results (Python)

### Requirements

```bash
pip install pandas altair openpyxl
```

⚠️ Convert benchmark_results.csv to benchmark_results.csv.xlsx
(Open in Excel → Save As → .xlsx)

Run the script

```bash
python visualize.py
```

✅ Open generated HTML charts in any browser

📂 Project Structure

```bash
📁 BenchmarkPC
 ├─ main.cpp                # C++ core benchmark engine
 ├─ visualize.py            # Python visualization tool
 ├─ benchmark_results.csv   # Runtime test logs
 ├─ *.html                  # Interactive output charts
 └─ README.md               # You're reading this 😉
```

🤝 Contributions

Contributions, optimization suggestions, and PRs are welcome!
Let’s build a reliable open-source benchmark together 🔥

📬 Contact

👤 Andrei Bolohan
✉️ bolohanandrei769@gmail.com

🔗 LinkedIn: https://www.linkedin.com/in/andrei-bolohan/

📌 Repository Link:
https://github.com/BolohanAndrei/BenchmarkPC

⭐ Show Your Support

If this project helps you —
please star the repo ⭐ to help visibility!
