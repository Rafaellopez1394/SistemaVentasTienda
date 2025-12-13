import pandas as pd
import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

# Ver categorías actuales en BD
print('CATEGORÍAS ACTUALES EN BASE DE DATOS:')
print('=' * 70)
cursor.execute("""
    SELECT CategoriaID, Nombre, Estatus 
    FROM CatCategoriasProducto 
    ORDER BY CategoriaID
""")
for row in cursor.fetchall():
    print(f'ID: {row[0]:3} | {row[1]:30} | Estatus: {row[2]}')

# Ver distribución en Excel
df = pd.read_excel('PRODUCTOS.xlsx')
print('\n\nDISTRIBUCIÓN EN EXCEL (columna "Línea"):')
print('=' * 70)
lineas = df['Línea'].value_counts()
for linea, count in lineas.items():
    print(f'{linea:30} | {count:3} productos')

# Ver distribución actual en productos
print('\n\nDISTRIBUCIÓN ACTUAL EN PRODUCTOS (BD):')
print('=' * 70)
cursor.execute("""
    SELECT c.Nombre, COUNT(p.ProductoID) as Total
    FROM Productos p
    LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
    GROUP BY c.Nombre
    ORDER BY Total DESC
""")
for row in cursor.fetchall():
    print(f'{row[0]:30} | {row[1]:3} productos')

conn.close()

print('\n\n💡 Para actualizar las categorías de los productos según el Excel,')
print('   puedo crear un script que:')
print('   1. Mapee las "Líneas" del Excel a las categorías de la BD')
print('   2. Actualice cada producto con su categoría correcta')
