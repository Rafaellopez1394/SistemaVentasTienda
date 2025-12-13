"""
Script para actualizar categorías de productos según la columna "Línea" del Excel
"""
import pandas as pd
import pyodbc

def conectar():
    conn_str = 'DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes'
    return pyodbc.connect(conn_str)

# Mapeo de Líneas del Excel a nombres de categorías
MAPEO_CATEGORIAS = {
    'ABARROTES': 'ABARROTES',
    'CONGELADO': 'CONGELADO',
    'VERDURAS': 'VERDURAS',
    'General': 'GENERAL',
}

def crear_o_obtener_categoria(conn, nombre_categoria):
    """Crea la categoría si no existe, o devuelve su ID"""
    cursor = conn.cursor()
    
    # Buscar si existe
    cursor.execute("SELECT CategoriaID FROM CatCategoriasProducto WHERE Nombre=?", (nombre_categoria,))
    result = cursor.fetchone()
    
    if result:
        return result[0]
    
    # Si no existe, crear
    cursor.execute("""
        INSERT INTO CatCategoriasProducto (Nombre, Estatus, FechaAlta, Usuario, UltimaAct)
        VALUES (?, 1, GETDATE(), 'IMPORTADOR', GETDATE())
    """, (nombre_categoria,))
    conn.commit()
    
    # Obtener el ID recién creado
    cursor.execute("SELECT CategoriaID FROM CatCategoriasProducto WHERE Nombre=?", (nombre_categoria,))
    return cursor.fetchone()[0]

def actualizar_categorias_productos():
    print("\n" + "="*70)
    print("ACTUALIZANDO CATEGORÍAS DE PRODUCTOS")
    print("="*70)
    
    conn = conectar()
    print("[OK] Conexion establecida")
    
    # Leer Excel
    df = pd.read_excel('PRODUCTOS.xlsx')
    print(f"Total de productos en Excel: {len(df)}")
    
    # Crear/obtener IDs de categorías
    print("\n[PASO 1] Verificando/creando categorías...")
    categorias_ids = {}
    for linea_excel, nombre_cat in MAPEO_CATEGORIAS.items():
        cat_id = crear_o_obtener_categoria(conn, nombre_cat)
        categorias_ids[linea_excel] = cat_id
        print(f"  {nombre_cat:20} -> ID: {cat_id}")
    
    # Actualizar productos
    print("\n[PASO 2] Actualizando productos...")
    cursor = conn.cursor()
    actualizados = 0
    sin_linea = 0
    no_encontrados = 0
    
    for index, row in df.iterrows():
        try:
            codigo = str(row['Código']).strip() if pd.notna(row['Código']) else None
            linea = str(row['Línea']).strip() if pd.notna(row['Línea']) else None
            
            if not codigo:
                continue
            
            # Si no hay línea, usar GENERAL
            if not linea or linea not in MAPEO_CATEGORIAS:
                linea = 'General'
                sin_linea += 1
            
            categoria_id = categorias_ids.get(linea)
            
            # Buscar producto
            cursor.execute("SELECT ProductoID FROM Productos WHERE CodigoInterno=?", (codigo,))
            result = cursor.fetchone()
            
            if not result:
                no_encontrados += 1
                continue
            
            producto_id = result[0]
            
            # Actualizar categoría
            cursor.execute("""
                UPDATE Productos 
                SET CategoriaID = ?, UltimaAct = GETDATE()
                WHERE ProductoID = ?
            """, (categoria_id, producto_id))
            
            conn.commit()
            actualizados += 1
            
            if actualizados % 50 == 0:
                print(f"  Actualizados: {actualizados}...")
                
        except Exception as e:
            conn.rollback()
            print(f"[ERROR] Fila {index+1} ({codigo}): {str(e)}")
    
    # Mostrar resumen
    print(f"\n[RESULTADO]")
    print(f"  Productos actualizados: {actualizados}")
    print(f"  Sin línea (asignados a GENERAL): {sin_linea}")
    print(f"  No encontrados en BD: {no_encontrados}")
    
    # Mostrar distribución final
    print("\n[DISTRIBUCIÓN FINAL]")
    cursor.execute("""
        SELECT c.Nombre, COUNT(p.ProductoID) as Total
        FROM CatCategoriasProducto c
        LEFT JOIN Productos p ON c.CategoriaID = p.CategoriaID
        GROUP BY c.Nombre
        HAVING COUNT(p.ProductoID) > 0
        ORDER BY Total DESC
    """)
    
    for row in cursor.fetchall():
        print(f"  {row[0]:20} -> {row[1]:3} productos")
    
    conn.close()
    print("\n[FIN] Actualizacion completada")

if __name__ == "__main__":
    actualizar_categorias_productos()
