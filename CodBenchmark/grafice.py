import pandas as pd
import altair as alt
import os

script_dir = os.path.dirname(os.path.abspath(__file__))

nume_fisier_date = "benchmark_results.csv.xlsx"
cale_completa_fisier = os.path.join(script_dir, nume_fisier_date)

try:
    df_raw = pd.read_excel(cale_completa_fisier, sheet_name='benchmark_results')
    print(f"Am incarcat cu succes datele din: {cale_completa_fisier}")
    print("Primele randuri de date:")
    print(df_raw.head())

    df_raw['Timp_Executie_sec'] = pd.to_numeric(df_raw['Timp_Executie_sec'], errors='coerce')
    df_raw['Performanta'] = pd.to_numeric(df_raw['Performanta'], errors='coerce')

    barchart_time = alt.Chart(df_raw).mark_bar().encode(
        x=alt.X('Test', title='Tip Test', axis=None),
        y=alt.Y('mean(Timp_Executie_sec)', title='Timp Mediu Executie (secunde)'),
        color=alt.Color('NumePC', legend=alt.Legend(title="Nume PC")),
        column=alt.Column('Test', title='Tip Test', header=alt.Header(titleOrient="bottom", labelOrient="bottom")),
        tooltip=['NumePC', 'Test', 'mean(Timp_Executie_sec)']
    ).properties(
        title='Timpul Mediu de Executie pe PC si Tip Test'
    ).interactive()  

    barchart_time.save('barchart_timp_mediu.html')
    print("Salvat: barchart_timp_mediu.html")

    barchart_performance = alt.Chart(df_raw).mark_bar().encode(
        x=alt.X('NumePC', title='Nume PC', axis=None),
        y=alt.Y('mean(Performanta)', title='Performanta Medie'),
        color=alt.Color('NumePC', legend=alt.Legend(title="Nume PC")),
        column=alt.Column('Test', title='Tip Test (Unitati: MIPS pt Integer, MFLOPS pt Float)',
                          header=alt.Header(titleOrient="bottom", labelOrient="bottom")),
        tooltip=['NumePC', 'Test', 'mean(Performanta)', 'Unitate']
    ).properties(
        title='Performanta Medie pe PC si Tip Test'
    ).interactive()  

    barchart_performance.save('barchart_performanta_medie.html')
    print("Salvat: barchart_performanta_medie.html")

    print("\nToate graficele au fost generate cu succes!")

except FileNotFoundError:
    print(f"EROARE: Fisierul '{nume_fisier_date}' nu a fost gasit.")
    print("Asigurati-va ca fisierul se afla in acelasi director cu scriptul Python.")
except Exception as e:
    print(f"A aparut o eroare neasteptata: {e}")