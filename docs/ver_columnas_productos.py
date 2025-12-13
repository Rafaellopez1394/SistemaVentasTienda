import pyodbc

conn = pyodbc.connect('DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes')
cursor = conn.cursor()

# Ver todas las columnas NOT NULL
cursor.execute("""
    SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME='Productos' 
    ORDER BY ORDINAL_POSITION
""")

print("\n=== Columnas de la tabla Productos ===\n")
print(f"{'Columna':<30} {'Tipo':<15} {'NULL':<10} {'Max Length'}")
print("=" * 70)

obligatorias = []
for row in cursor.fetchall():
    col_name, data_type, is_nullable, max_length = row
    length_str = str(max_length) if max_length else ""
    print(f"{col_name:<30} {data_type:<15} {is_nullable:<10} {length_str}")
    
    if is_nullable == 'NO':
        obligatorias.append(col_name)

print("\n=== Columnas OBLIGATORIAS (NOT NULL) ===")
for col in obligatorias:
    print(f"  • {col}")

conn.close()
