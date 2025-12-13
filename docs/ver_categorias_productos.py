import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

# Ver distribución de productos por categoría
cursor.execute("""
    SELECT 
        c.CategoriaID,
        c.Nombre as Categoria,
        COUNT(p.ProductoID) as TotalProductos
    FROM CatCategoriasProducto c
    LEFT JOIN Productos p ON c.CategoriaID = p.CategoriaID
    GROUP BY c.CategoriaID, c.Nombre
    ORDER BY TotalProductos DESC
""")

print('Distribucion de Productos por Categoria:')
print('=' * 70)
for row in cursor.fetchall():
    print(f'ID: {row[0]:3} | {row[1]:30} | {row[2]:5} productos')

# Ver algunos ejemplos de productos con su categoría
print('\n\nEjemplos de Productos con sus Categorias:')
print('=' * 70)
cursor.execute("""
    SELECT TOP 20
        p.CodigoInterno,
        p.Nombre,
        c.Nombre as Categoria
    FROM Productos p
    LEFT JOIN CatCategoriasProducto c ON p.CategoriaID = c.CategoriaID
    ORDER BY p.CodigoInterno
""")

for row in cursor.fetchall():
    print(f'{row[0]:20} | {row[1]:40} | {row[2]}')

conn.close()
