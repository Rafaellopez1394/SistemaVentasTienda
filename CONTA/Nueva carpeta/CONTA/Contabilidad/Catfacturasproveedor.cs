using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatfacturasproveedor
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatfacturasproveedor : ISprocBase
    {
        DataSet Catfacturasproveedor_Select(string facturaproveedorid, string UUID, string Proveedorid, string Empresaid);

        DataSet Catfacturasproveedor_Select(string facturaproveedorid, string UUID, string Proveedorid, string Empresaid, string emisorrfc, string receptorrfc, string usocfdi, DateTime? fechainicial, DateTime? fechafinal);

        DataSet Catfacturasproveedor_Select();
        DataSet Catfacturasproveedor_Select(string CompraID);
        DataSet ValidarExistenciaFacturaProveedor(string Uuid);

        int Catfacturasproveedor_Save(
            ref string facturaproveedorid,
            string compraid,
            string empresaid,
            string proveedorid,
            string factura,
            decimal subtotal,
            decimal iva,
            decimal retIva,
            decimal retIsr,
            decimal ieps,
            decimal total,
            string uuid,
            DateTime fechatimbre,
            string xml,
            string rutaxml,
            bool generapasivo,
            bool dlls,
            decimal abonado,
            DateTime fechapago,
            bool pagada,
            DateTime fecha,
            int estatus,
            string usuario,
            ref int ultimaAct);
        int Catfacturasproveedor_Delete(string Compraid);
    }

    #endregion //Interfaz ICatfacturasproveedor

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catfacturasproveedor
    /// </summary>
    public class Catfacturasproveedor
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catfacturasproveedor()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catfacturasproveedor A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catfacturasproveedor BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catfacturasproveedor elemento = new Entity.Contabilidad.Catfacturasproveedor();
            if (!Convert.IsDBNull(row["FacturaProveedorID"]))
            {
                elemento.Facturaproveedorid = row["FacturaProveedorID"].ToString();
            }
            if (!Convert.IsDBNull(row["CompraID"]))
            {
                elemento.Compraid = row["CompraID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["ProveedorID"]))
            {
                elemento.Proveedorid = row["ProveedorID"].ToString();
            }
            if (!Convert.IsDBNull(row["Factura"]))
            {
                elemento.Factura = row["Factura"].ToString();
            }
            if (!Convert.IsDBNull(row["Subtotal"]))
            {
                elemento.Subtotal = decimal.Parse(row["Subtotal"].ToString());
            }
            if (!Convert.IsDBNull(row["IVA"]))
            {
                elemento.Iva = decimal.Parse(row["IVA"].ToString());
            }
            if (!Convert.IsDBNull(row["RET_IVA"]))
            {
                elemento.RetIva = decimal.Parse(row["RET_IVA"].ToString());
            }
            if (!Convert.IsDBNull(row["RET_ISR"]))
            {
                elemento.RetIsr = decimal.Parse(row["RET_ISR"].ToString());
            }
            if (!Convert.IsDBNull(row["IEPS"]))
            {
                elemento.Ieps = decimal.Parse(row["IEPS"].ToString());
            }
            if (!Convert.IsDBNull(row["Total"]))
            {
                elemento.Total = decimal.Parse(row["Total"].ToString());
            }
            if (!Convert.IsDBNull(row["UUID"]))
            {
                elemento.Uuid = row["UUID"].ToString();
            }
            if (!Convert.IsDBNull(row["FechaTimbre"]))
            {
                elemento.Fechatimbre = DateTime.Parse(row["FechaTimbre"].ToString()).ToShortDateString();
            }
            if (!Convert.IsDBNull(row["XML"]))
            {
                elemento.Xml = row["XML"].ToString();
            }
            if (!Convert.IsDBNull(row["RutaXml"]))
            {
                elemento.Rutaxml = row["RutaXml"].ToString();
            }
            if (!Convert.IsDBNull(row["GeneraPasivo"]))
            {
                elemento.Generapasivo = bool.Parse(row["GeneraPasivo"].ToString());
            }
            if (!Convert.IsDBNull(row["Dlls"]))
            {
                elemento.Dlls = bool.Parse(row["Dlls"].ToString());
            }
            if (!Convert.IsDBNull(row["Abonado"]))
            {
                elemento.Abonado = decimal.Parse(row["Abonado"].ToString());
            }
            if (!Convert.IsDBNull(row["FechaPago"]))
            {
                elemento.Fechapago = DateTime.Parse(row["FechaPago"].ToString());
            }
            if (!Convert.IsDBNull(row["Pagada"]))
            {
                elemento.Pagada = bool.Parse(row["Pagada"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
            }
            if (getChilds) {
                elemento.Detalle = MobileDAL.Contabilidad.Catfacturasproveedordet.TraerCatfacturasproveedordetPorFactura(elemento.Facturaproveedorid);
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Catfacturasproveedor> listaCatfacturasproveedor)
        {
            ICatfacturasproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedor>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catfacturasproveedor elemento in listaCatfacturasproveedor)
                {
                    codigo = elemento.Facturaproveedorid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Catfacturasproveedor_Save(
                        ref codigo,
                         (elemento.Compraid != null) ? elemento.Compraid : null,
                         (elemento.Empresaid != null) ? elemento.Empresaid : null,
                         (elemento.Proveedorid != null) ? elemento.Proveedorid : null,
                         (elemento.Factura != null) ? elemento.Factura : null,
                         elemento.Subtotal,
                         elemento.Iva,
                         elemento.RetIva,
                         elemento.RetIsr,
                         elemento.Ieps,
                         elemento.Total,
                         (elemento.Uuid != null) ? elemento.Uuid : null,
                         (elemento.Fechatimbre != null && elemento.Fechatimbre != string.Empty) ? DateTime.Parse(elemento.Fechatimbre) : DateTime.MinValue,
                         (elemento.Xml != null) ? elemento.Xml : null,
                         (elemento.Rutaxml != null) ? elemento.Rutaxml : null,
                         elemento.Generapasivo,
                         elemento.Dlls,
                         elemento.Abonado,
                         (elemento.Fechapago != null) ? elemento.Fechapago : DateTime.MinValue,
                         elemento.Pagada,
                         (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                         elemento.Estatus,
                         (elemento.Usuario != null) ? elemento.Usuario : null,
                        ref ultimaAct);
                    elemento.Facturaproveedorid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();                
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasproveedor()
        {
            ICatfacturasproveedor proc = null;
            try
            {
                List<Entity.Contabilidad.Catfacturasproveedor> listaCatfacturasproveedor = new List<Entity.Contabilidad.Catfacturasproveedor>();
                proc = Utilerias.GenerarSproc<ICatfacturasproveedor>();
                DataSet dsCatfacturasproveedor = proc.Catfacturasproveedor_Select();
                foreach (DataRow row in dsCatfacturasproveedor.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catfacturasproveedor elemento = BuildEntity(row, true);
                    listaCatfacturasproveedor.Add(elemento);
                }                
                return listaCatfacturasproveedor;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Catfacturasproveedor TraerCatfacturasproveedor(string facturaproveedorid,string uuid)
        {
            ICatfacturasproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedor>();
                Entity.Contabilidad.Catfacturasproveedor elemento = null;
                DataSet ds = null;
                ds = proc.Catfacturasproveedor_Select(facturaproveedorid, uuid, null, null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasproveedor(string facturaproveedorid, string uuid, string proveedorid, string empresaid, string emisorrfc, string receptorrfc, string usocfdi, DateTime? fechainicial, DateTime? fechafinal)
        {
            ICatfacturasproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedor>();
                List<Entity.Contabilidad.Catfacturasproveedor> elementos = new List<Entity.Contabilidad.Catfacturasproveedor>();
                DataSet ds = null;
                ds = proc.Catfacturasproveedor_Select(facturaproveedorid, uuid, proveedorid, empresaid, emisorrfc, receptorrfc, usocfdi, fechainicial, fechafinal);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catfacturasproveedor elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                    elementos.Add(elemento);
                }
                return elementos;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static System.Data.DataSet TraerCatfacturasproveedorDS()
        {
            ICatfacturasproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedor>();
                DataSet ds = proc.Catfacturasproveedor_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasPorProveedorEmpresa(string proveedorid, string empresaid)
        {
            ICatfacturasproveedor proc = null;
            try
            {
                List<Entity.Contabilidad.Catfacturasproveedor> listaCatfacturasproveedor = new List<Entity.Contabilidad.Catfacturasproveedor>();
                proc = Utilerias.GenerarSproc<ICatfacturasproveedor>();
                DataSet dsCatfacturasproveedor = proc.Catfacturasproveedor_Select(null, null, proveedorid, empresaid);
                foreach (DataRow row in dsCatfacturasproveedor.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catfacturasproveedor elemento = BuildEntity(row, true);
                    listaCatfacturasproveedor.Add(elemento);
                }
                return listaCatfacturasproveedor;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Catfacturasproveedor> TraerCatfacturasproveedorPorCompraID(string CompraID)
        {
            ICatfacturasproveedor proc = null;
            try
            {
                List<Entity.Contabilidad.Catfacturasproveedor> listaCatfacturasproveedor = new List<Entity.Contabilidad.Catfacturasproveedor>();
                proc = Utilerias.GenerarSproc<ICatfacturasproveedor>();
                DataSet dsCatfacturasproveedor = proc.Catfacturasproveedor_Select(CompraID);
                foreach (DataRow row in dsCatfacturasproveedor.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catfacturasproveedor elemento = BuildEntity(row, true);
                    listaCatfacturasproveedor.Add(elemento);
                }
                return listaCatfacturasproveedor;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static void Catfacturasproveedor_Delete(string CompraID)
        {
            ICatfacturasproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedor>(false);
                proc.Transaction = proc.Connection.BeginTransaction();
                proc.Catfacturasproveedor_Delete(CompraID);
                proc.Transaction.Commit();
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }
        public static int ValidarExistenciaFacturaProveedor(string uuid)
        {
            ICatfacturasproveedor proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatfacturasproveedor>();
                var dataSet = proc.ValidarExistenciaFacturaProveedor(uuid);
                int resultado = (int)dataSet.Tables[0].Rows[0]["ExisteFactura"];
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Métodos Públicos
    }
}
