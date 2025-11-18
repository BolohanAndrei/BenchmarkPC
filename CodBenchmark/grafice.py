import pandas as pd
import altair as alt
import os

script_dir = os.path.dirname(os.path.abspath(__file__))
nume_fisier_date = "benchmark_results_phase3.csv"
cale_completa_fisier = os.path.join(script_dir, nume_fisier_date)

print(f"--- DASHBOARD COMPARATIV (Green Theme) ---")

try:
    df = pd.read_csv(cale_completa_fisier)
    
    df.columns = df.columns.str.strip()
    df['Timp_Executie_sec'] = pd.to_numeric(df['Timp_Executie_sec'], errors='coerce')
    df['Performanta'] = pd.to_numeric(df['Performanta'], errors='coerce')

    lista_teste = df['Test'].unique().tolist()
    
    input_dropdown = alt.binding_select(options=lista_teste, name='Selecteaza Testul: ')
    
    selection = alt.selection_point(
        fields=['Test'], 
        bind=input_dropdown,
        value=lista_teste[0]
    )

    base_perf = alt.Chart(df).encode(
        x=alt.X('NumePC:N', title=None, axis=alt.Axis(labels=True, labelAngle=0)),
        y=alt.Y('mean(Performanta):Q', title='Performanta (Mai mare e mai bine)'),
        color=alt.Color('NumePC:N', legend=None)
    ).add_params(
        selection
    ).transform_filter(
        selection
    )

    chart_perf = base_perf.mark_bar() + base_perf.mark_text(dy=-10).encode(text=alt.Text('mean(Performanta):Q', format='.2f'))
    chart_perf = chart_perf.properties(title='Performanta (Scor)', width=300, height=300)

    base_time = alt.Chart(df).encode(
        x=alt.X('NumePC:N', title=None, axis=alt.Axis(labels=True, labelAngle=0)),
        y=alt.Y('mean(Timp_Executie_sec):Q', title='Timp Executie (Mai mic e mai bine)'),
        color=alt.Color('NumePC:N', legend=None)
    ).add_params(
        selection
    ).transform_filter(
        selection
    )

    chart_time = base_time.mark_bar() + base_time.mark_text(dy=-10).encode(text=alt.Text('mean(Timp_Executie_sec):Q', format='.4f'))
    chart_time = chart_time.properties(title='Timp de Executie (Secunde)', width=300, height=300)

    chart_heatmap = alt.Chart(df).mark_rect().encode(
        x=alt.X('NumePC:N', title='Configuratie PC'),
        y=alt.Y('Test:N', title='Tip Test'),
        color=alt.Color('mean(Timp_Executie_sec):Q', 
                        title='Durata (sec)',
                        scale=alt.Scale(scheme='greens')), 
        tooltip=['NumePC', 'Test', 'mean(Timp_Executie_sec)']
    ).properties(
        title='Harta Termica: Durata Testelor (Verde Inchis = Durata Mare)',
        width=650,
        height=250
    )

    dashboard = alt.vconcat(
        alt.hconcat(chart_perf, chart_time).resolve_scale(color='independent'),
        chart_heatmap
    ).properties(
        title='Benchmark Dashboard'
    ).configure_view(
        stroke=None 
    )

    output_file = 'dashboard_green.html'
    dashboard.save(output_file)
    
    print(f"\nSUCCES! Deschide fisierul: {output_file}")

except FileNotFoundError:
    print(f"EROARE: Nu gasesc fisierul '{nume_fisier_date}'!")
except Exception as e:
    print(f"Eroare: {e}")