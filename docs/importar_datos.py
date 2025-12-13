"""
Script para importar datos desde archivos Excel a la base de datos
Productos y Existencias
"""
import pandas as pd
import pyodbc
from datetime import datetime

# Configuracion de la conexion a SQL Server
SERVER = '.'  # Servidor local
DATABASE = 'DB_TIENDA'
TRUSTED_CONNECTION = 'yes'

def conectar_bd():
    """Conecta a la base de datos SQL Server"""
    try:
        conn_str = f'DRIVER={{SQL Server}};SERVER={SERVER};DATABASE={DATABASE};Trusted_Connection={TRUSTED_CONNECTION}'
        conn = pyodbc.connect(conn_str)
        print("✓ Conexion a base de datos exitosa")
        return conn
    except Exception as e:
        print(f"✗ Error al conectar: {e}")
        return None

def mostrar_estructura_archivos():
    """Muestra la estructura de los archivos Excel"""
    print("\n" + "="*80)
    print("ESTRUCTURA DE ARCHIVOS")
    print("="*80)
    
    try:
        # Leer PRODUCTOS.xlsx
        print("\n📄 PRODUCTOS.xlsx:")
        df_productos = pd.read_excel('PRODUCTOS.xlsx')
        print(f"  Filas: {len(df_productos)}")
        print(f"  Columnas: {list(df_productos.columns)}")
        print("\n  Primeras 3 filas:")
        print(df_productos.head(3).to_string())
        
        print("\n" + "-"*80)
        
        # Leer EXISTENCIAS.xlsx
        print("\n📄 EXISTENCIAS.xlsx:")
        df_existencias = pd.read_excel('EXISTENCIAS.xlsx')
        print(f"  Filas: {len(df_existencias)}")
        print(f"  Columnas: {list(df_existencias.columns)}")
        print("\n  Primeras 3 filas:")
        print(df_existencias.head(3).to_string())
        
        return df_productos, df_existencias
    except Exception as e:
        print(f"✗ Error al leer archivos: {e}")
        return None, None

def obtener_estructura_tablas(conn):
    """Obtiene la estructura de las tablas de productos en la BD"""
    print("\n" + "="*80)
    print("ESTRUCTURA DE TABLAS EN BASE DE DATOS")
    print("="*80)
    
    cursor = conn.cursor()
    
    # Obtener columnas de tabla PRODUCTO
    print("\n📊 Tabla: PRODUCTO")
    cursor.execute("""
        SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'PRODUCTO'
        ORDER BY ORDINAL_POSITION
    """)
    
    for row in cursor.fetchall():
        nullable = "NULL" if row[3] == 'YES' else "NOT NULL"
        max_len = f"({row[2]})" if row[2] else ""
        print(f"  {row[0]:<30} {row[1]}{max_len:<15} {nullable}")
    
    # Obtener columnas de tabla PRODUCTO_TIENDA (existencias)
    print("\n📊 Tabla: PRODUCTO_TIENDA (Existencias/Sucursal)")
    cursor.execute("""
        SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'PRODUCTO_TIENDA'
        ORDER BY ORDINAL_POSITION
    """)
    
    for row in cursor.fetchall():
        nullable = "NULL" if row[3] == 'YES' else "NOT NULL"
        max_len = f"({row[2]})" if row[2] else ""
        print(f"  {row[0]:<30} {row[1]}{max_len:<15} {nullable}")
    
    # Obtener categorias disponibles
    print("\n📊 Categorías disponibles:")
    try:
        cursor.execute("SELECT CategoriaID, Descripcion FROM CATEGORIA")
        categorias = cursor.fetchall()
        if len(categorias) == 0:
            print("  ⚠️  No hay categorías. Creando categoría por defecto...")
            cursor.execute("INSERT INTO CATEGORIA (Descripcion, Activo) VALUES ('GENERAL', 1)")
            conn.commit()
            cursor.execute("SELECT CategoriaID, Descripcion FROM CATEGORIA")
            categorias = cursor.fetchall()
        for cat in categorias:
            print(f"  ID: {cat[0]}, Nombre: {cat[1]}")
    except Exception as e:
        print(f"  ⚠️  Error al obtener categorías: {e}")
        categorias = []
    
    # Obtener sucursales disponibles
    print("\n📊 Sucursales disponibles:")
    try:
        cursor.execute("SELECT SucursalID, Nombre FROM TIENDA")
        sucursales = cursor.fetchall()
        if len(sucursales) == 0:
            print("  ⚠️  No hay sucursales registradas")
        for suc in sucursales:
            print(f"  ID: {suc[0]}, Nombre: {suc[1]}")
    except Exception as e:
        print(f"  ⚠️  Error al obtener sucursales: {e}")
        sucursales = []
    
    return categorias, sucursales

def importar_productos(conn, df_productos, categoria_default=1):
    """Importa productos a la base de datos"""
    print("\n" + "="*80)
    print("IMPORTANDO PRODUCTOS")
    print("="*80)
    
    cursor = conn.cursor()
    insertados = 0
    errores = 0
    duplicados = 0
    
    # Limpiar datos - remover filas sin código
    df_productos = df_productos[df_productos['Código'].notna()]
    
    print(f"\nTotal de productos a procesar: {len(df_productos)}")
    
    for idx, row in df_productos.iterrows():
        try:
            codigo = str(row['Código']).strip()
            descripcion = str(row['Descripción']).strip() if pd.notna(row['Descripción']) else ''
            
            # Verificar si ya existe
            cursor.execute("SELECT ProductoID FROM PRODUCTO WHERE Codigo = ?", (codigo,))
            if cursor.fetchone():
                duplicados += 1
                continue
            
            # Mapear columnas del Excel a la tabla PRODUCTO
            sql = """
                INSERT INTO PRODUCTO (
                    Codigo, ValorCodigo, Nombre, Descripcion, CategoriaID, Activo
                )
                VALUES (?, 0, ?, ?, ?, 1)
            """
            
            cursor.execute(sql, (
                codigo,
                descripcion[:100],  # Nombre (limitado a 100 caracteres)
                descripcion[:100],  # Descripción
                categoria_default   # Categoría por defecto
            ))
            
            conn.commit()
            insertados += 1
            
            if insertados % 50 == 0:
                print(f"  ... {insertados} productos insertados")
            
        except Exception as e:
            errores += 1
            print(f"  ✗ Error en fila {idx + 1} (Código: {codigo}): {e}")
            conn.rollback()
    
    print(f"\n✓ Productos insertados: {insertados}")
    print(f"⚠ Duplicados omitidos: {duplicados}")
    print(f"✗ Errores: {errores}")
    return insertados > 0

def importar_existencias(conn, df_productos, sucursal_default=1):
    """Importa precios y existencias a PRODUCTO_TIENDA"""
    print("\n" + "="*80)
    print("IMPORTANDO PRECIOS Y EXISTENCIAS")
    print("="*80)
    
    cursor = conn.cursor()
    insertados = 0
    errores = 0
    productos_no_encontrados = 0
    
    # Limpiar datos
    df_productos = df_productos[df_productos['Código'].notna()]
    
    print(f"\nTotal de registros a procesar: {len(df_productos)}")
    
    for idx, row in df_productos.iterrows():
        try:
            codigo = str(row['Código']).strip()
            
            # Buscar el ProductoID
            cursor.execute("SELECT ProductoID FROM PRODUCTO WHERE Codigo = ?", (codigo,))
            resultado = cursor.fetchone()
            
            if not resultado:
                productos_no_encontrados += 1
                continue
            
            producto_id = resultado[0]
            
            # Obtener precios y existencias del Excel
            precio_venta = float(row['Precio Público']) if pd.notna(row['Precio Público']) else 0
            stock_max = int(row['Máximos']) if pd.notna(row['Máximos']) else 0
            stock_min = int(row['Mínimos']) if pd.notna(row['Mínimos']) else 0
            
            # Verificar si ya existe el registro
            cursor.execute("""
                SELECT ProductoSucursalID FROM PRODUCTO_TIENDA 
                WHERE ProductoID = ? AND SucursalID = ?
            """, (producto_id, sucursal_default))
            
            if cursor.fetchone():
                # Actualizar
                sql = """
                    UPDATE PRODUCTO_TIENDA 
                    SET PrecioUnidadVenta = ?, 
                        Stock = CASE WHEN Iniciado = 0 THEN ? ELSE Stock END,
                        Iniciado = 1
                    WHERE ProductoID = ? AND SucursalID = ?
                """
                cursor.execute(sql, (precio_venta, stock_max, producto_id, sucursal_default))
            else:
                # Insertar
                sql = """
                    INSERT INTO PRODUCTO_TIENDA (
                        ProductoID, SucursalID, PrecioUnidadCompra, 
                        PrecioUnidadVenta, Stock, Activo, Iniciado
                    )
                    VALUES (?, ?, 0, ?, ?, 1, 1)
                """
                cursor.execute(sql, (producto_id, sucursal_default, precio_venta, stock_max))
            
            conn.commit()
            insertados += 1
            
            if insertados % 50 == 0:
                print(f"  ... {insertados} registros procesados")
            
        except Exception as e:
            errores += 1
            print(f"  ✗ Error en fila {idx + 1} (Código: {codigo}): {e}")
            conn.rollback()
    
    print(f"\n✓ Registros procesados: {insertados}")
    print(f"⚠ Productos no encontrados: {productos_no_encontrados}")
    print(f"✗ Errores: {errores}")

def main():
    """Funcion principal"""
    print("\n" + "="*80)
    print("IMPORTADOR DE DATOS - SISTEMA DE VENTAS")
    print("="*80)
    
    # 1. Mostrar estructura de archivos Excel
    df_productos, df_existencias = mostrar_estructura_archivos()
    
    if df_productos is None:
        return
    
    # 2. Conectar a base de datos
    conn = conectar_bd()
    if conn is None:
        return
    
    # 3. Mostrar estructura de tablas
    categorias, sucursales = obtener_estructura_tablas(conn)
    
    # 4. Seleccionar categoría y sucursal por defecto
    print("\n" + "="*80)
    print("CONFIGURACIÓN DE IMPORTACIÓN")
    print("="*80)
    
    categoria_id = 1
    if len(categorias) > 0:
        print(f"\nCategoría por defecto: ID {categorias[0][0]} - {categorias[0][1]}")
        cambiar = input("¿Deseas cambiar la categoría? (si/no): ")
        if cambiar.lower() in ['si', 's', 'yes', 'y']:
            categoria_id = int(input("Ingresa el ID de la categoría: "))
    
    sucursal_id = 1
    if len(sucursales) > 0:
        print(f"\nSucursal por defecto: ID {sucursales[0][0]} - {sucursales[0][1]}")
        cambiar = input("¿Deseas cambiar la sucursal? (si/no): ")
        if cambiar.lower() in ['si', 's', 'yes', 'y']:
            sucursal_id = int(input("Ingresa el ID de la sucursal: "))
    
    # 5. Confirmación
    print("\n" + "="*80)
    print("RESUMEN DE IMPORTACIÓN")
    print("="*80)
    print(f"  Productos a importar: {len(df_productos[df_productos['Código'].notna()])}")
    print(f"  Categoría: {categoria_id}")
    print(f"  Sucursal: {sucursal_id}")
    print("="*80)
    
    respuesta = input("\n¿Deseas continuar con la importación? (si/no): ")
    
    if respuesta.lower() in ['si', 's', 'yes', 'y']:
        # Paso 1: Importar productos
        if importar_productos(conn, df_productos, categoria_id):
            # Paso 2: Importar precios y existencias
            importar_existencias(conn, df_productos, sucursal_id)
        else:
            print("\n⚠️  No se importaron productos, saltando existencias")
    else:
        print("\nImportación cancelada")
    
    conn.close()
    print("\n✓ Proceso finalizado")

if __name__ == "__main__":
    main()
