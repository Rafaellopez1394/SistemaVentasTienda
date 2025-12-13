import pandas as pd

df = pd.read_excel('EXISTENCIAS.xlsx')
print("Columnas en EXISTENCIAS.xlsx:")
print(df.columns.tolist())
print(f"\nPrimeras 5 filas:")
print(df.head())
