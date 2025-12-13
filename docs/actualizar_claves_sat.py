"""
Script para actualizar las claves SAT de los productos ya importados
"""
import pandas as pd
import pyodbc

def conectar():
    conn_str = 'DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes'
    return pyodbc.connect(conn_str)

def actualizar_claves_sat():
    print("\n" + "="*70)
    print("ACTUALIZANDO CLAVES SAT DE PRODUCTOS")
    print("="*70)
    
    conn = conectar()
    print("[OK] Conexion establecida")
    
    df = pd.read_excel('PRODUCTOS.xlsx')
    print(f"Total de productos en Excel: {len(df)}")
    
    cursor = conn.cursor()
    actualizados = 0
    sin_clave = 0
    no_encontrados = 0
    
    for index, row in df.iterrows():
        try:
            codigo = str(row['Código']).strip() if pd.notna(row['Código']) else None
            clave_sat = str(row['Clave Producto/Servicio Sat']).strip() if pd.notna(row['Clave Producto/Servicio Sat']) else None
            
            if not codigo:
                continue
            
            # Si no hay clave SAT válida, usar genérica
            if not clave_sat or len(clave_sat) < 8:
                clave_sat = '01010101'
                sin_clave += 1
            
            # Verificar que la clave SAT existe en el catálogo
            cursor.execute("SELECT COUNT(*) FROM CatClaveProdServSAT WHERE ClaveProdServSAT=?", (clave_sat,))
            existe = cursor.fetchone()[0]
            
            if existe == 0:
                # Si no existe en catálogo, usar genérica
                clave_sat = '01010101'
                sin_clave += 1
            
            # Buscar el producto
            cursor.execute("SELECT ProductoID FROM Productos WHERE CodigoInterno=?", (codigo,))
            result = cursor.fetchone()
            
            if not result:
                no_encontrados += 1
                continue
            
            producto_id = result[0]
            
            # Actualizar la clave SAT
            cursor.execute("""
                UPDATE Productos 
                SET ClaveProdServSAT = ?, UltimaAct = GETDATE()
                WHERE ProductoID = ?
            """, (clave_sat, producto_id))
            
            conn.commit()
            actualizados += 1
            
            if actualizados % 50 == 0:
                print(f"  Actualizados: {actualizados}...")
                
        except Exception as e:
            conn.rollback()
            print(f"[ERROR] Fila {index+1} ({codigo}): {str(e)}")
    
    print(f"\n[RESULTADO]")
    print(f"  Productos actualizados: {actualizados}")
    print(f"  Sin clave SAT (usan generica): {sin_clave}")
    print(f"  No encontrados en BD: {no_encontrados}")
    
    conn.close()
    print("\n[FIN] Actualizacion completada")

if __name__ == "__main__":
    actualizar_claves_sat()
