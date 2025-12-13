"""
Script para actualizar las unidades SAT de los productos desde el Excel
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
    'saco': 'XBX',  # Saco también se mapea a Caja/Paquete
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

def actualizar_unidades_sat():
    print("\n" + "="*70)
    print("ACTUALIZANDO UNIDADES SAT DE PRODUCTOS")
    print("="*70)
    
    conn = conectar()
    print("[OK] Conexion establecida")
    
    df = pd.read_excel('PRODUCTOS.xlsx')
    print(f"Total de productos en Excel: {len(df)}")
    
    cursor = conn.cursor()
    actualizados = 0
    sin_unidad = 0
    no_encontrados = 0
    
    for index, row in df.iterrows():
        try:
            codigo = str(row['Código']).strip() if pd.notna(row['Código']) else None
            unidad_nombre = str(row['Clave/Nombre Unidad de Medida Sat']).strip() if pd.notna(row['Clave/Nombre Unidad de Medida Sat']) else None
            
            if not codigo:
                continue
            
            # Si no hay unidad válida, usar genérica (Pieza)
            if not unidad_nombre:
                unidad_clave = 'H87'
                sin_unidad += 1
            else:
                # Buscar en el mapeo (case insensitive)
                unidad_lower = unidad_nombre.lower()
                unidad_clave = MAPEO_UNIDADES.get(unidad_lower, 'H87')
                
                if unidad_clave == 'H87' and unidad_lower != 'pieza':
                    sin_unidad += 1
                    print(f"[!] Unidad no mapeada '{unidad_nombre}' para {codigo}, usando Pieza")
            
            # Verificar que la clave existe en el catálogo
            cursor.execute("SELECT COUNT(*) FROM CatUnidadSAT WHERE ClaveUnidadSAT=?", (unidad_clave,))
            existe = cursor.fetchone()[0]
            
            if existe == 0:
                # Si no existe en catálogo, usar Pieza
                unidad_clave = 'H87'
                sin_unidad += 1
            
            # Buscar el producto
            cursor.execute("SELECT ProductoID FROM Productos WHERE CodigoInterno=?", (codigo,))
            result = cursor.fetchone()
            
            if not result:
                no_encontrados += 1
                continue
            
            producto_id = result[0]
            
            # Actualizar la unidad SAT
            cursor.execute("""
                UPDATE Productos 
                SET ClaveUnidadSAT = ?, UltimaAct = GETDATE()
                WHERE ProductoID = ?
            """, (unidad_clave, producto_id))
            
            conn.commit()
            actualizados += 1
            
            if actualizados % 50 == 0:
                print(f"  Actualizados: {actualizados}...")
                
        except Exception as e:
            conn.rollback()
            print(f"[ERROR] Fila {index+1} ({codigo}): {str(e)}")
    
    print(f"\n[RESULTADO]")
    print(f"  Productos actualizados: {actualizados}")
    print(f"  Sin unidad o no mapeada (usan Pieza): {sin_unidad}")
    print(f"  No encontrados en BD: {no_encontrados}")
    
    conn.close()
    print("\n[FIN] Actualizacion completada")

if __name__ == "__main__":
    actualizar_unidades_sat()
