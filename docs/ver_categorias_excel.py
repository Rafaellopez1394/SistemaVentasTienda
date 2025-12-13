import pandas as pd

df = pd.read_excel('PRODUCTOS.xlsx')

print('Valores de columnas relacionadas con categorías:')
print('=' * 70)

# Clasificación
clasificaciones = df['Clasificación'].dropna().unique()
print(f'\nCLASIFICACIÓN ({len(clasificaciones)} valores únicos):')
for c in sorted(clasificaciones)[:20]:
    count = len(df[df['Clasificación'] == c])
    print(f'  - {c:40} ({count:3} productos)')

# Línea
lineas = df['Línea'].dropna().unique()
print(f'\n\nLÍNEA ({len(lineas)} valores únicos):')
for l in sorted(lineas)[:20]:
    count = len(df[df['Línea'] == l])
    print(f'  - {l:40} ({count:3} productos)')

# Departamento
departamentos = df['Departamento'].dropna().unique()
print(f'\n\nDEPARTAMENTO ({len(departamentos)} valores únicos):')
for d in sorted(departamentos)[:20]:
    count = len(df[df['Departamento'] == d])
    print(f'  - {d:40} ({count:3} productos)')
