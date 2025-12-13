import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

# Ver algunas claves del catálogo
cursor.execute('SELECT TOP 10 ClaveProdServSAT, Descripcion FROM CatClaveProdServSAT ORDER BY ClaveProdServSAT')
rows = cursor.fetchall()
print('Claves en catálogo SAT:')
for row in rows:
    print(f'{row[0]} - {row[1][:60]}')

# Total en catálogo
cursor.execute('SELECT COUNT(*) FROM CatClaveProdServSAT')
total = cursor.fetchone()[0]
print(f'\nTotal de claves en catálogo: {total}')

# Buscar claves del Excel en el catálogo
claves_excel = ['50121612', '50171800', '50171830', '12171504', '50192900']
print(f'\nBuscando claves del Excel en el catálogo:')
for clave in claves_excel:
    cursor.execute('SELECT ClaveProdServSAT, Descripcion FROM CatClaveProdServSAT WHERE ClaveProdServSAT=?', (clave,))
    result = cursor.fetchone()
    if result:
        print(f'{clave} - ENCONTRADA: {result[1][:60]}')
    else:
        print(f'{clave} - NO ENCONTRADA')

conn.close()
