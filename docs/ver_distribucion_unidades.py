import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

cursor.execute("""
    SELECT p.ClaveUnidadSAT, c.Descripcion, COUNT(*) as Total 
    FROM Productos p 
    LEFT JOIN CatUnidadSAT c ON p.ClaveUnidadSAT = c.ClaveUnidadSAT 
    GROUP BY p.ClaveUnidadSAT, c.Descripcion 
    ORDER BY Total DESC
""")

print('Distribucion de Unidades SAT en Productos:')
print('=' * 60)
for row in cursor.fetchall():
    print(f'{row[0]:10} {row[1]:30} {row[2]:5} productos')

conn.close()
