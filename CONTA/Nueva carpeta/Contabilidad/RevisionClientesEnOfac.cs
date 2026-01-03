using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{

    public interface IRevisionClientesEnOfac : ISprocBase
    {
        DataSet BuscarEmpresaExpuestas();
        DataSet Personasbloqueadasofac_Select1();

        // Nuevo método con parámetro
        [SprocName("BuscarCoincidenciasClientesTipo")]
        DataSet BuscarCoincidenciasClientesTipo(string NombreBuscado);

        [SprocName("BuscarCoincidenciasClientes")]
        DataSet BuscarCoincidenciasClientes(string NombreBuscado);

        [SprocName("SP_AgregarPersonasBloqueadas")]
        int SP_AgregarPersonasBloqueadas(string ClienteID, string Usuario, string Descripcion);


        int SP_ActualizaPersonasBloqueadasOfac(string ClienteID, string Usuario, string Descripcion, string estatus);
    }

    public class RevisionClientesEnOfac
    {
        public RevisionClientesEnOfac()
        {
        }
        public static ListaDeEntidades<Entity.Contabilidad.Clientes> GeneraListasuspendidos()
        {
            IRevisionClientesEnOfac proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Clientes> ListaClientesSuspendidos = new ListaDeEntidades<Entity.Contabilidad.Clientes>();
                proc = Utilerias.GenerarSproc<IRevisionClientesEnOfac>();
                DataSet obtenerPersonasbloqueadasofac = proc.Personasbloqueadasofac_Select1();
                var lista = new List<Entity.Contabilidad.Clientes>();
                if (obtenerPersonasbloqueadasofac != null && obtenerPersonasbloqueadasofac.Tables.Count > 0)
                {
                    foreach (DataRow row in obtenerPersonasbloqueadasofac.Tables[0].Rows)
                    {
                        var persona = new Entity.Contabilidad.Clientes
                        {
                            BloqueoID =row["BloqueoID"].ToString(),
                            TipoPersona = Convert.ToInt32(row["TipoPersona"]),
                            Nombre = row["Nombre"].ToString(),
                            ApellidoPaterno = row["ApellidoPaterno"].ToString(),
                            ApellidoMaterno = row["ApellidoMaterno"].ToString(),
                            Nacionalidad = row["Nacionalidad"].ToString(),
                            TipoDocumento = row["TipoDocumento"].ToString(),
                            NumeroDocumento = row["NumeroDocumento"].ToString(),
                            ListaOFAC = row["ListaOFAC"].ToString(),
                            FechaInclusion = Convert.ToDateTime(row["FechaInclusion"]),
                            Motivo = row["Motivo"].ToString(),
                            UsuarioRegistro = row["UsuarioRegistro"].ToString(),
                            FechaRegistro = Convert.ToDateTime(row["FechaRegistro"]),
                            Estatus = Convert.ToInt32(row["Estatus"])
                    };
                        ListaClientesSuspendidos.Add(persona);
                    }
                }
                return ListaClientesSuspendidos;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static ListaDeEntidades<Entity.Contabilidad.Clientes> GeneraListaClientes()
        {
            IRevisionClientesEnOfac proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Clientes> ListaClientes = new ListaDeEntidades<Entity.Contabilidad.Clientes>();
                proc = Utilerias.GenerarSproc<IRevisionClientesEnOfac>();
                DataSet BuscarEmpresaExpuestas = proc.BuscarEmpresaExpuestas();
                foreach (DataRow row in BuscarEmpresaExpuestas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Clientes elemento = BuildEntity(row, true);
                    ListaClientes.Add(elemento);
                }
                ListaClientes.AcceptChanges();
                return ListaClientes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Clientes> BuscarCoincidenciasPorTipo(string NombreBuscado)
        {
            IRevisionClientesEnOfac proc = null;
            var lista = new List<Entity.Contabilidad.Clientes>();

            try
            {
                proc = Utilerias.GenerarSproc<IRevisionClientesEnOfac>();
                DataSet ds = proc.BuscarCoincidenciasClientesTipo(NombreBuscado);

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var cliente = new Entity.Contabilidad.Clientes();

                        if (!Convert.IsDBNull(row["RazonSocial"]))
                            cliente.RazonSocial = row["RazonSocial"].ToString();

                        if (!Convert.IsDBNull(row["Nombre"]))
                            cliente.Nombre = row["Nombre"].ToString();
                        

                        lista.Add(cliente);
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar coincidencias de clientes por tipo", ex);
            }
        }

        public static List<Entity.Contabilidad.Clientes> BuscarCoincidencias(string NombreBuscado)
        {
            IRevisionClientesEnOfac proc = null;
            var lista = new List<Entity.Contabilidad.Clientes>();

            try
            {
                proc = Utilerias.GenerarSproc<IRevisionClientesEnOfac>();
                DataSet ds = proc.BuscarCoincidenciasClientes(NombreBuscado);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var cliente = new Entity.Contabilidad.Clientes();

                        // --- Datos del cliente principal ---
                        cliente.ClienteID = row.IsNull(0) ? null : row[0].ToString();
                        cliente.NombreCompleto = row.IsNull(1) ? null : row[1].ToString();
                        cliente.RazonSocial = row.IsNull(1) ? null : row[1].ToString();

                        if (!row.IsNull(2))
                        {
                            DateTime fechaTemp;
                            if (DateTime.TryParse(row[2].ToString(), out fechaTemp))
                                cliente.FechaAlta = fechaTemp;
                        }

                        // --- Datos del aval ---
                        cliente.AvalID = row.IsNull(3) ? null : row[3].ToString();
                        cliente.NombreAval = row.IsNull(4) ? null : row[4].ToString();
                        cliente.RepresentanteLegal = row.IsNull(5) ? null : row[5].ToString();

                        // --- Indicadores booleanos (evita DBNull) ---
                        cliente.EsAccionista = !row.IsNull(6) && Convert.ToInt32(row[6]) == 1;
                        cliente.EsAval = !row.IsNull(7) && Convert.ToInt32(row[7]) == 1;
                        cliente.EsRepresentante = !row.IsNull(8) && Convert.ToInt32(row[8]) == 1;
                        cliente.EsAccionistaMayoritario = !row.IsNull(9) && Convert.ToInt32(row[9]) == 1;

                        // --- Determina tipo ---
                        cliente.Tipo = cliente.EsAval ? "Aval" : "Cliente";

                        lista.Add(cliente);
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar coincidencias de clientes", ex);
            }
        }

        public static void AgregarPersonaBloqueada(string clienteId, string usuario, string descripcion)
        {
            IRevisionClientesEnOfac proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IRevisionClientesEnOfac>();
                proc.SP_AgregarPersonasBloqueadas(clienteId, usuario, descripcion);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar", ex);
            }
        }

        public static void ActualizarPersonaBloqueada(string clienteId, string usuario, string descripcion, string estatus)
        {
            IRevisionClientesEnOfac proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IRevisionClientesEnOfac>();
                proc.SP_ActualizaPersonasBloqueadasOfac(clienteId, usuario, descripcion, estatus);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar", ex);
            }
        }

        private static Entity.Contabilidad.Clientes BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Clientes elemento = new Entity.Contabilidad.Clientes();
            if (!Convert.IsDBNull(row["NombreCompleto"]))
            {
                elemento.NombreCompleto = row["NombreCompleto"].ToString();
            }

            if (!Convert.IsDBNull(row["Nombre"]))
            {
                elemento.Nombre = row["Nombre"].ToString();
            }

            if (!Convert.IsDBNull(row["ApellidoPaterno"]))
            {
                elemento.ApellidoPaterno = row["ApellidoPaterno"].ToString();
            }

            if (!Convert.IsDBNull(row["ApellidoMaterno"]))
            {
                elemento.ApellidoMaterno = row["ApellidoMaterno"].ToString();
            }

            if (!Convert.IsDBNull(row["Descripcion"]))
            {
                elemento.Descripcion = row["Descripcion"].ToString();
            }

            return elemento;
        }
    }
}