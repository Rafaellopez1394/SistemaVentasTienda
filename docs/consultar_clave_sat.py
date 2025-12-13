import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

# Ver estructura de ClaveUnidadSAT
cursor.execute("SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CatClaveUnidadSAT'")
print("\n=== Estructura CatClaveUnidadSAT ===")
for row in cursor.fetchall():
    print(f"{row[0]} - {row[1]} - NULL={row[2]}")

# Ver primeros registros
cursor.execute('SELECT TOP 10 * FROM CatClaveUnidadSAT')
print("\n=== Primeros registros ===")
columns = [column[0] for column in cursor.description]
print(" | ".join(columns))
for row in cursor.fetchall():
    print(" | ".join(str(x) for x in row))

conn.close()
