"""
Script simplificado de importacion de Excel a DB_TIENDA
"""
import pandas as pd
import pyodbc

def conectar():
    conn_str = 'DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes'
    return pyodbc.connect(conn_str)

# Mapeo de nombres en español a claves SAT
MAPEO_UNIDADES = {
    'pieza': 'H87',
    'kilogramo': 'KGM',
    'caja': 'XBX',
    'saco': 'XBX',
    'litro': 'LTR',
    'gramo': 'GRM',
    'paquete': 'XPK',
    'docena': 'DPC',
    'par': 'PR',
    'conjunto': 'SET',
    'set': 'SET',
    'miligramo': 'MGM',
    'mililitro': 'MLT',
}

# Mapeo de Líneas del Excel a categorías
MAPEO_CATEGORIAS = {
    'ABARROTES': 'ABARROTES',
    'CONGELADO': 'CONGELADO',
    'VERDURAS': 'VERDURAS',
    'General': 'GENERAL',
}

def obtener_o_crear_categoria(conn, nombre_categoria):
    """Obtiene el ID de una categoría, la crea si no existe"""
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
    
    cursor.execute("SELECT CategoriaID FROM CatCategoriasProducto WHERE Nombre=?", (nombre_categoria,))
    return cursor.fetchone()[0]

def obtener_categoria_id(conn):
    """DEPRECATED: Usar obtener_o_crear_categoria en su lugar"""
    cursor = conn.cursor()
    cursor.execute("SELECT TOP 1 CategoriaID FROM CatCategoriasProducto WHERE Estatus=1")
    result = cursor.fetchone()
    if result:
        return result[0]
    else:
        # Crear categoria GENERAL
        cursor.execute("INSERT INTO CatCategoriasProducto (Nombre, Descripcion, Estatus, FechaRegistro) VALUES ('GENERAL', 'Categoria general', 1, GETDATE())")
        conn.commit()
        cursor.execute("SELECT CategoriaID FROM CatCategoriasProducto WHERE Nombre='GENERAL'")
        return cursor.fetchone()[0]

def importar_productos(conn, archivo_excel):
    print("\n" + "="*70)
    print("IMPORTANDO PRODUCTOS")
    print("="*70)
    
    df = pd.read_excel(archivo_excel)
    print(f"Total de filas en Excel: {len(df)}")
    
    cursor = conn.cursor()
    categoria_id = obtener_categoria_id(conn)
    
    insertados = 0
    duplicados = 0
    errores = 0
    
    for index, row in df.iterrows():
        try:
            codigo = str(row['Código']).strip() if pd.notna(row['Código']) else None
            nombre = str(row['Descripción']).strip() if pd.notna(row['Descripción']) else 'Sin descripcion'
            
            # Obtener categoría del Excel (columna "Línea")
            linea = str(row['Línea']).strip() if pd.notna(row['Línea']) else 'General'
            nombre_categoria = MAPEO_CATEGORIAS.get(linea, 'GENERAL')
            categoria_id = obtener_o_crear_categoria(conn, nombre_categoria)
            
            # Obtener clave SAT del Excel (si existe)
            clave_sat = str(row['Clave Producto/Servicio Sat']).strip() if pd.notna(row['Clave Producto/Servicio Sat']) else '01010101'
            # Si la clave está vacía o es inválida, usar genérica
            if not clave_sat or len(clave_sat) < 8:
                clave_sat = '01010101'
            
            # Obtener unidad de medida SAT del Excel
            unidad_nombre = str(row['Clave/Nombre Unidad de Medida Sat']).strip() if pd.notna(row['Clave/Nombre Unidad de Medida Sat']) else None
            if unidad_nombre:
                unidad_clave = MAPEO_UNIDADES.get(unidad_nombre.lower(), 'H87')
            else:
                unidad_clave = 'H87'  # Pieza por defecto
            
            if not codigo:
                print(f"[SKIP] Fila {index+1}: sin codigo")
                continue
            
            # Verificar duplicado
            cursor.execute("SELECT ProductoID FROM Productos WHERE CodigoInterno=?", (codigo,))
            if cursor.fetchone():
                duplicados += 1
                continue
            
            # Insertar
            cursor.execute("""
                INSERT INTO Productos (
                    Nombre, CategoriaID, CodigoInterno, 
                    Estatus, FechaAlta, TipoIVA, Exento, 
                    TasaIVAID, TasaIEPSID, 
                    ClaveProdServSAT, ClaveUnidadSAT,
                    Usuario, UltimaAct
                )
                VALUES (?, ?, ?, 1, GETDATE(), 1, 0, 3, 1, ?, ?, 'IMPORTADOR', GETDATE())
            """, (nombre, categoria_id, codigo, clave_sat, unidad_clave))
            
            conn.commit()
            insertados += 1
            if insertados % 50 == 0:
                print(f"  Procesados: {insertados}...")
            
        except Exception as e:
            print(f"[ERROR] Fila {index+1} ({codigo}): {str(e)[:100]}")
            errores += 1
            conn.rollback()
    
    print(f"\n[RESULTADO]")
    print(f"  Insertados: {insertados}")
    print(f"  Duplicados: {duplicados}")
    print(f"  Errores: {errores}")
    
    return insertados

def importar_existencias_desde_productos(conn, archivo_excel):
    """Importa lotes usando datos de PRODUCTOS.xlsx (Precio Publico y Maximos)"""
    print("\n" + "="*70)
    print("IMPORTANDO LOTES DESDE PRODUCTOS.xlsx")
    print("="*70)
    
    df = pd.read_excel(archivo_excel)
    df = df[pd.notna(df['Código'])]  # Filtrar códigos nulos
    print(f"Total de productos: {len(df)}")
    
    cursor = conn.cursor()
    insertados = 0
    sin_datos = 0
    errores = 0
    
    for index, row in df.iterrows():
        try:
            codigo = str(row['Código']).strip()
            
            # Buscar ProductoID
            cursor.execute("SELECT ProductoID FROM Productos WHERE CodigoInterno=?", (codigo,))
            result = cursor.fetchone()
            if not result:
                continue
            
            producto_id = result[0]
            precio_venta = float(row['Precio Público']) if pd.notna(row['Precio Público']) else 0.0
            cantidad = int(row['Máximos']) if pd.notna(row['Máximos']) else 0
            
            if precio_venta <= 0 or cantidad <= 0:
                sin_datos += 1
                continue
            
            # Calcular precio de compra (70% del precio público = 30% margen)
            precio_compra = precio_venta * 0.7
            
            # Insertar lote
            cursor.execute("""
                INSERT INTO LotesProducto (
                    ProductoID, FechaEntrada, CantidadTotal, CantidadDisponible,
                    PrecioCompra, PrecioVenta, Usuario, Estatus, UltimaAct
                )
                VALUES (?, GETDATE(), ?, ?, ?, ?, 'IMPORTADOR', 1, GETDATE())
            """, (producto_id, cantidad, cantidad, precio_compra, precio_venta))
            
            conn.commit()
            insertados += 1
            if insertados % 50 == 0:
                print(f"  Procesados: {insertados}...")
            
        except Exception as e:
            print(f"[ERROR] Fila {index+1} ({codigo}): {str(e)[:80]}")
            errores += 1
            conn.rollback()
    
    print(f"\n[RESULTADO]")
    print(f"  Lotes insertados: {insertados}")
    print(f"  Sin datos validos: {sin_datos}")
    print(f"  Errores: {errores}")

def main():
    print("\n" + "="*70)
    print("IMPORTADOR DE PRODUCTOS Y EXISTENCIAS")
    print("="*70)
    
    conn = conectar()
    print("[OK] Conexion establecida")
    
    # Importar productos
    insertados_productos = importar_productos(conn, 'PRODUCTOS.xlsx')
    
    # Siempre preguntar por lotes/existencias
    resp = input("\n¿Continuar con importacion de lotes? (s/n): ")
    if resp.lower() == 's':
        importar_existencias_desde_productos(conn, 'PRODUCTOS.xlsx')
    else:
        print("\n[SKIP] Importacion de lotes omitida")
    
    conn.close()
    print("\n[FIN] Proceso completado")

if __name__ == "__main__":
    main()
