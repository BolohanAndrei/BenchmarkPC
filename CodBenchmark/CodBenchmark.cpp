#include <iostream>
#include <chrono>
#include <vector>
#include <cmath>
#include <cstdint>
#include <fstream>
#include <string>

const int64_t ITERATIONS = 10000000000; 


volatile int64_t g_int_result = 0;
volatile double g_float_result = 0.0;

void test_integer_ops() {
    int64_t a = 12345;
    int64_t b = 67890;
    int64_t c = 101112;
    int64_t local_sum = 0; 

    for (int64_t i = 0; i < ITERATIONS; ++i) {
        a = a + b;
        b = a * c;
        c = b ^ 0xFF;
        a = c >> 3;
        b = a | 0xAA;
        c = b & a;

     
        local_sum += c;
    }

   
    g_int_result = local_sum;
}

void test_float_ops() {
    double x = 1.234;
    double y = 5.678;
    double z = 9.012;
    double local_sum = 0.0; 

    for (int64_t i = 0; i < ITERATIONS; ++i) {
        x = x + y;
        y = x * z;
        z = y / 1.5;
        x = std::fma(x, y, z);

        local_sum += x;
    }

    g_float_result = local_sum;
}


void salveaza_rezultate(const std::string& nume_pc, double timp_int, double mips, double timp_float, double mflops) {
    std::ofstream fisier_out("benchmark_results.csv", std::ios::app);
    if (!fisier_out.is_open()) {
        std::cerr << "Eroare: Nu am putut deschide fisierul CSV!" << std::endl;
        return;
    }
    fisier_out.seekp(0, std::ios::end);
    if (fisier_out.tellp() == 0) {
        fisier_out << "NumePC,Test,Timp_Executie_sec,Performanta,Unitate" << std::endl;
    }
    fisier_out << nume_pc << ",Integer," << timp_int << "," << mips << ",MIPS" << std::endl;
    fisier_out << nume_pc << ",Float," << timp_float << "," << mflops << ",MFLOPS" << std::endl;
    fisier_out.close();
    std::cout << "Rezultatele au fost salvate in 'benchmark_results.csv'" << std::endl;
}


int main() {
    std::string nume_pc = "PC1 Performance";
    std::cout << "Nume PC: " << nume_pc << std::endl;
    std::cout << "Numar iteratii: " << ITERATIONS << std::endl << std::endl;

    // --- Măsurare Test Integer (MIPS) ---
    std::cout << "Pornire Test Integer (MIPS)..." << std::endl;
    auto start_int = std::chrono::high_resolution_clock::now();
    test_integer_ops();
    auto end_int = std::chrono::high_resolution_clock::now();
    std::chrono::duration<double> elapsed_int = end_int - start_int;
    double seconds_int = elapsed_int.count();

    int64_t total_integer_ops = ITERATIONS * 6; 
    double mips = (total_integer_ops / seconds_int) / 1'000'000.0;

    std::cout << "Timp (int): " << seconds_int << "s, MIPS: " << mips << std::endl;

    // --- Măsurare Test Floating Point (MFLOPS) ---
    std::cout << "Pornire Test Floating Point (MFLOPS)..." << std::endl;
    auto start_float = std::chrono::high_resolution_clock::now();
    test_float_ops();
    auto end_float = std::chrono::high_resolution_clock::now();
    std::chrono::duration<double> elapsed_float = end_float - start_float;
    double seconds_float = elapsed_float.count();

    int64_t total_float_ops = ITERATIONS * 4; 
    double mflops = (total_float_ops / seconds_float) / 1'000'000.0;

    std::cout << "Timp (float): " << seconds_float << "s, MFLOPS: " << mflops << std::endl;

    // --- Salvarea datelor ---
    salveaza_rezultate(nume_pc, seconds_int, mips, seconds_float, mflops);

    return 0;
}