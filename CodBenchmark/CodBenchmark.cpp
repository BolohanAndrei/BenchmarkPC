#include <iostream>
#include <chrono>
#include <vector>
#include <cmath>
#include <cstdint>
#include <fstream>
#include <string>
#include <random>
#include <cstdio>
#include <cstring>
#include <iomanip> 

const int64_t ITERATIONS_PI = 10'500'000'000;  // Calibrat (SuperPi 1M)
const int64_t ITERATIONS_INT = 10'000'000'000;

const int NUM_TEST_RUNS = 5; 

volatile int64_t g_int_result = 0;
volatile double g_float_result = 0.0;

// --- FUNCȚIE SALVARE CSV ---
void salveaza_rezultat(const std::string& nume_pc, const std::string& nume_test, double timp_sec, double performanta, const std::string& unitate) {
    const std::string nume_fisier = "benchmark_results_phase3.csv";

    bool scrie_header = false;
    std::ifstream f_in(nume_fisier);
    if (!f_in.good() || f_in.peek() == std::ifstream::traits_type::eof()) {
        scrie_header = true;
    }
    f_in.close();

    std::ofstream fisier_out(nume_fisier, std::ios::app);
    if (!fisier_out.is_open()) {
        std::cerr << "Eroare CSV!" << std::endl;
        return;
    }

    if (scrie_header) {
        fisier_out << "NumePC,Test,Timp_Executie_sec,Performanta,Unitate" << std::endl;
    }

    fisier_out << nume_pc << "," << nume_test << "," << timp_sec << "," << performanta << "," << unitate << std::endl;
    fisier_out.close();
}

// --- TEST 1: Integer  ---
void test_integer_ops() {
    int64_t a = 12345, b = 67890, c = 101112;
    int64_t local_sum = 0;
    for (int64_t i = 0; i < ITERATIONS_INT; ++i) {
        a = a + b; b = a * c; c = b ^ 0xFF; a = c >> 3; b = a | 0xAA; c = b & a;
        local_sum += c;
    }
    g_int_result = local_sum;
}

// --- TEST 2: SuperPi  ---
void test_super_pi() {
    double pi_quarter = 0.0;
    double sign = 1.0;

    for (int64_t i = 0; i < ITERATIONS_PI; ++i) {
        pi_quarter += sign / (2.0 * i + 1.0);
        sign = -sign;
    }

    g_float_result = pi_quarter * 4.0;
}

// --- TEST 3: Memory Bandwidth ---
void test_memory_bandwidth(const std::string& nume_pc) {
    std::cout << "   -> Test Latime Banda Memorie (RAM)..." << std::endl;
    const int64_t BLOCK_SIZE = 1024LL * 1024 * 1024; // 1 GB

    std::vector<char> sursa(BLOCK_SIZE, (char)0xAA);
    std::vector<char> destinatie(BLOCK_SIZE);

    auto start = std::chrono::high_resolution_clock::now();
    std::memcpy(destinatie.data(), sursa.data(), BLOCK_SIZE);
    auto end = std::chrono::high_resolution_clock::now();

    double seconds = std::chrono::duration<double>(end - start).count();
    double speed_mbps = (BLOCK_SIZE / (1024.0 * 1024.0)) / seconds;

    std::cout << "      Timp: " << seconds << "s, Viteza: " << speed_mbps << " MB/s" << std::endl;
    salveaza_rezultat(nume_pc, "Mem_Bandwidth", seconds, speed_mbps, "MB/s");
}

// --- TEST 4: Memory Latency ---
void test_memory_latency(const std::string& nume_pc) {
    std::cout << "   -> Test Latenta Memorie..." << std::endl;
    const int64_t REGION_SIZE = 128 * 1024 * 1024; // 128 MB
    const int64_t ACCESS_COUNT = 50'000'000;

    std::vector<char> memory(REGION_SIZE, 1);

    uint64_t index = 0;
    uint64_t stride = 4099;
    volatile char val = 0;

    auto start = std::chrono::high_resolution_clock::now();
    for (int64_t i = 0; i < ACCESS_COUNT; ++i) {
        index = (index + stride) % REGION_SIZE;
        val = memory[index];
    }
    auto end = std::chrono::high_resolution_clock::now();

    double seconds = std::chrono::duration<double>(end - start).count();
    double latency_ns = (seconds * 1e9) / ACCESS_COUNT;

    std::cout << "      Timp: " << seconds << "s, Latenta (estimata): " << latency_ns << " ns" << std::endl;
    salveaza_rezultat(nume_pc, "Mem_Latency", seconds, latency_ns, "ns");
}

// --- TEST 5: I/O 1GB ---
void test_io_performance(const std::string& nume_pc) {
    std::cout << "   -> Test I/O Disc (1GB)..." << std::endl;
    const std::string FNAME = "bench_io.tmp";
    const int64_t FILE_SIZE = 1024LL * 1024 * 1024;
    std::vector<char> buffer(FILE_SIZE, 'X');

    // Write
    auto start_w = std::chrono::high_resolution_clock::now();
    std::ofstream f_out(FNAME, std::ios::binary);
    f_out.write(buffer.data(), FILE_SIZE);
    f_out.close();
    double sec_w = std::chrono::duration<double>(std::chrono::high_resolution_clock::now() - start_w).count();
    double speed_w = (FILE_SIZE / (1024.0 * 1024.0)) / sec_w;

    std::cout << "      Scriere: " << sec_w << "s, Viteza: " << speed_w << " MB/s" << std::endl;
    salveaza_rezultat(nume_pc, "IO_Write", sec_w, speed_w, "MB/s");

    // Read
    auto start_r = std::chrono::high_resolution_clock::now();
    std::ifstream f_in(FNAME, std::ios::binary);
    std::vector<char> buffer_in(FILE_SIZE);
    f_in.read(buffer_in.data(), FILE_SIZE);
    f_in.close();
    double sec_r = std::chrono::duration<double>(std::chrono::high_resolution_clock::now() - start_r).count();
    double speed_r = (FILE_SIZE / (1024.0 * 1024.0)) / sec_r;

    std::cout << "      Citire: " << sec_r << "s, Viteza: " << speed_r << " MB/s" << std::endl;
    salveaza_rezultat(nume_pc, "IO_Read", sec_r, speed_r, "MB/s");

    std::remove(FNAME.c_str());
}

int main() {
    std::string nume_pc = "PC1_Battery";

    std::cout << "=== BENCHMARK PC - AUTOMATED SUITE ===" << std::endl;
    std::cout << "Se vor executa " << NUM_TEST_RUNS << " rulari complete." << std::endl;

    for (int run = 1; run <= NUM_TEST_RUNS; ++run) {
        std::cout << "\n========================================" << std::endl;
        std::cout << "   PORNIRE RULARE " << run << " din " << NUM_TEST_RUNS << std::endl;
        std::cout << "========================================" << std::endl;

        // 1. Integer
        std::cout << "   -> Ruleaza Integer..." << std::endl;
        auto s = std::chrono::high_resolution_clock::now();
        test_integer_ops();
        double dur_int = std::chrono::duration<double>(std::chrono::high_resolution_clock::now() - s).count();
        double mips = (ITERATIONS_INT * 6.0 / dur_int) / 1e6;
        std::cout << "      Integer: " << dur_int << "s, " << mips << " MIPS" << std::endl;
        salveaza_rezultat(nume_pc, "CPU_Integer", dur_int, mips, "MIPS");

        // 2. SuperPi
        std::cout << "   -> Ruleaza SuperPi..." << std::endl;
        s = std::chrono::high_resolution_clock::now();
        test_super_pi();
        double dur_pi = std::chrono::duration<double>(std::chrono::high_resolution_clock::now() - s).count();
        double mflops = (ITERATIONS_PI * 2.0 / dur_pi) / 1e6;
        std::cout << "      SuperPi: " << dur_pi << "s, " << mflops << " MFLOPS (Simulat)" << std::endl;
        salveaza_rezultat(nume_pc, "CPU_SuperPi", dur_pi, mflops, "MFLOPS");

        // 3. Memory & IO
        test_memory_bandwidth(nume_pc);
        test_memory_latency(nume_pc);
        test_io_performance(nume_pc);
    }

    std::cout << "\n\n=== TOATE TESTELE S-AU FINALIZAT CU SUCCES! ===" << std::endl;
    std::cout << "Datele au fost salvate in 'benchmark_results_phase3.csv'" << std::endl;

    return 0;
}