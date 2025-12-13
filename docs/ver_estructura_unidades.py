import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

cursor.execute("""
    SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME='CatUnidadSAT' 
    ORDER BY ORDINAL_POSITION
""")

print('Columnas de CatUnidadSAT:')
print('-' * 60)
for row in cursor.fetchall():
    print(f'{row[0]:30} {row[1]:15} NULL={row[2]}')

conn.close()
