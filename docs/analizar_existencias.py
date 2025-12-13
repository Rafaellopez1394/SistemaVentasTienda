import pandas as pd

df = pd.read_excel('EXISTENCIAS.xlsx')
print(f'Total filas: {len(df)}')
print(f'No nulos en Articulo: {df["Artículo"].notna().sum()}')
print("\nPrimeras 10 filas con datos:")
print(df[df["Artículo"].notna()].head(10))
