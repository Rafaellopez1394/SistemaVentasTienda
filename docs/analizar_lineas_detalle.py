import pandas as pd

df = pd.read_excel('PRODUCTOS.xlsx')

# Ver distribución detallada por Línea y ejemplos de productos
print('ANÁLISIS DE LÍNEAS Y PRODUCTOS:')
print('=' * 80)

lineas = df['Línea'].value_counts()
for linea, count in lineas.items():
    print(f'\n{linea} ({count} productos):')
    print('-' * 80)
    
    # Mostrar primeros 10 productos de esta línea
    productos_linea = df[df['Línea'] == linea][['Código', 'Descripción']].head(10)
    for _, row in productos_linea.iterrows():
        desc = str(row['Descripción'])[:60]
        print(f'  {row["Código"]:20} | {desc}')
    
    if count > 10:
        print(f'  ... y {count - 10} productos más')

# Análisis de productos de ABARROTES para identificar subcategorías
print('\n\n' + '=' * 80)
print('ANÁLISIS DETALLADO DE ABARROTES (primeros 50 productos):')
print('=' * 80)

abarrotes = df[df['Línea'] == 'ABARROTES'][['Código', 'Descripción']].head(50)
for _, row in abarrotes.iterrows():
    desc = str(row['Descripción'])[:70]
    print(f'{row["Código"]:20} | {desc}')
