import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

# Buscar tabla de categorías
cursor.execute("""
    SELECT TABLE_NAME 
    FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_TYPE='BASE TABLE' 
    AND (TABLE_NAME LIKE '%ateg%' OR TABLE_NAME LIKE '%Categ%')
    ORDER BY TABLE_NAME
""")

print('Tablas relacionadas con Categoria:')
for row in cursor.fetchall():
    print(f'  - {row[0]}')

# Ver columnas de Productos para encontrar CategoriaID
print('\n\nColumnas de Productos que tienen "Categ":')
cursor.execute("""
    SELECT COLUMN_NAME, DATA_TYPE 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME='Productos' 
    AND COLUMN_NAME LIKE '%ateg%'
""")

for row in cursor.fetchall():
    print(f'  - {row[0]} ({row[1]})')

conn.close()
