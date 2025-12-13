"""
Script para insertar las claves SAT del Excel en el catálogo de la base de datos
"""
import pandas as pd
import pyodbc

def conectar():
    conn_str = 'DRIVER={SQL Server};SERVER=.;DATABASE=DB_TIENDA;Trusted_Connection=yes'
    return pyodbc.connect(conn_str)

def insertar_claves_sat():
    print("\n" + "="*70)
    print("INSERTANDO CLAVES SAT DEL EXCEL AL CATALOGO")
    print("="*70)
    
    conn = conectar()
    print("[OK] Conexion establecida")
    
    # Leer Excel
    df = pd.read_excel('PRODUCTOS.xlsx')
    claves_excel = df['Clave Producto/Servicio Sat'].dropna().unique()
    print(f"Total claves unicas en Excel: {len(claves_excel)}")
    
    cursor = conn.cursor()
    insertadas = 0
    duplicadas = 0
    
    for clave in claves_excel:
        clave = str(clave).strip()
        
        if len(clave) < 8:
            continue
        
        try:
            # Verificar si ya existe
            cursor.execute("SELECT COUNT(*) FROM CatClaveProdServSAT WHERE ClaveProdServSAT=?", (clave,))
            existe = cursor.fetchone()[0]
            
            if existe == 0:
                # Insertar (descripción genérica porque no tenemos el catálogo completo)
                cursor.execute("""
                    INSERT INTO CatClaveProdServSAT (ClaveProdServSAT, Descripcion, Estatus, FechaAlta, Usuario, UltimaAct)
                    VALUES (?, ?, 1, GETDATE(), 'IMPORTADOR', GETDATE())
                """, (clave, f'Producto/Servicio {clave}'))
                conn.commit()
                insertadas += 1
                print(f"[+] {clave} insertada")
            else:
                duplicadas += 1
                
        except Exception as e:
            conn.rollback()
            print(f"[ERROR] {clave}: {e}")
    
    print(f"\n[RESULTADO]")
    print(f"  Claves insertadas: {insertadas}")
    print(f"  Ya existian: {duplicadas}")
    
    conn.close()
    print("\n[FIN] Insercion completada")

if __name__ == "__main__":
    insertar_claves_sat()
