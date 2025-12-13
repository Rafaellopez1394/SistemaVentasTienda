import pyodbc
import pandas as pd

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

# Ver unidades en catálogo
cursor.execute('SELECT ClaveUnidadSAT, Descripcion FROM CatUnidadSAT ORDER BY ClaveUnidadSAT')
print('Unidades en CatUnidadSAT:')
print('-' * 70)
for row in cursor.fetchall():
    print(f'{row[0]:10} - {row[1]}')

# Total en catálogo
cursor.execute('SELECT COUNT(*) FROM CatUnidadSAT')
total_cat = cursor.fetchone()[0]
print(f'\nTotal unidades en catálogo: {total_cat}')

# Ver qué unidades están en el Excel
df = pd.read_excel('PRODUCTOS.xlsx')
unidades_excel = df['Clave/Nombre Unidad de Medida Sat'].dropna().unique()
print(f'\n\nUnidades únicas en Excel: {len(unidades_excel)}')
print('-' * 70)
for unidad in sorted(unidades_excel)[:20]:
    print(f'{unidad}')

# Ver unidades actuales en productos
cursor.execute('SELECT ClaveUnidadSAT, COUNT(*) FROM Productos GROUP BY ClaveUnidadSAT')
print('\n\nUnidades actuales en Productos:')
print('-' * 70)
for row in cursor.fetchall():
    print(f'{row[0]:10} - {row[1]} productos')

conn.close()
