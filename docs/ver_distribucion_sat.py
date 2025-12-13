import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

# Distribución de claves SAT
cursor.execute("""
    SELECT ClaveProdServSAT, COUNT(*) as Total 
    FROM Productos 
    GROUP BY ClaveProdServSAT 
    ORDER BY Total DESC
""")

print('Distribucion de Claves SAT en Productos:')
print('-' * 50)
total = 0
for row in cursor.fetchall():
    print(f'{row[0]:15} {row[1]:5} productos')
    total += row[1]

print('-' * 50)
print(f'TOTAL: {total} productos')

# Catálogo total
cursor.execute('SELECT COUNT(*) FROM CatClaveProdServSAT')
total_catalogo = cursor.fetchone()[0]
print(f'\nClaves en catálogo: {total_catalogo}')

conn.close()
