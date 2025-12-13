import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

# Buscar la tabla correcta de unidades SAT
cursor.execute("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE '%Unidad%SAT%' OR TABLE_NAME LIKE '%ClaveUnidad%'")
print("\n=== Tablas relacionadas con Unidad SAT ===")
tablas = cursor.fetchall()
for t in tablas:
    print(f"  • {t[0]}")

if tablas:
    tabla = tablas[0][0]
    print(f"\n=== Estructura de {tabla} ===")
    cursor.execute(f"SELECT TOP 10 * FROM {tabla}")
    columns = [column[0] for column in cursor.description]
    print(" | ".join(columns))
    print("-" * 100)
    for row in cursor.fetchall():
        print(" | ".join(str(x) if x else '' for x in row))

conn.close()
