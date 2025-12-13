import pandas as pd

df = pd.read_excel('PRODUCTOS.xlsx', nrows=5)

print('Columnas del Excel PRODUCTOS.xlsx:')
print('=' * 60)
for i, col in enumerate(df.columns, 1):
    print(f'{i:2}. {col}')

print(f'\n\nPrimeras 3 filas de ejemplo:')
print('=' * 60)
print(df[['Código', 'Descripción']].head(3).to_string(index=False))
