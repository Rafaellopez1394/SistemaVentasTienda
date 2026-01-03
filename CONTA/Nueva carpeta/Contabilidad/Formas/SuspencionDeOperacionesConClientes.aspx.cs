using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class SuspencionDeOperacionesConClientes : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvClientes.Visible = false;
                CargarListaSuspendidos(); // ✅ Cargar al abrir la página
            }
        }

        private void CargarListaSuspendidos()
        {
            try
            {
                var lista = MobileBO.Contabilidad.RevisionClientesEnOfacBO.ConsultaBloqueado();
                if (lista != null && lista.Count > 0)
                {
                    gvSuspendidos.DataSource = lista;
                    gvSuspendidos.DataBind();
                    gvSuspendidos.Visible = true;
                }
                else
                {
                    gvSuspendidos.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblResultado.Text = $"❌ Error al cargar lista de suspendidos: {ex.Message}";
                lblResultado.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtBuscar.Text?.Trim().ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    lblResultado.Text = "⚠️ Escriba un nombre para buscar.";
                    lblResultado.ForeColor = System.Drawing.Color.OrangeRed;
                    gvClientes.Visible = false;
                    return;
                }

                var clientes = BuscarCliente(nombre);

                if (clientes == null || clientes.Count == 0)
                {
                    lblResultado.Text = "🔍 No se encontraron coincidencias.";
                    gvClientes.Visible = false;
                    return;
                }

                gvClientes.DataSource = clientes.Select(c => new
                {
                    c.ClienteID,
                    NombreCliente = c.NombreCompleto,
                    c.FechaAlta,
                    c.AvalID,
                    c.NombreAval,
                    c.RepresentanteLegal,
                    c.EsAccionista,
                    c.EsAval,
                    c.EsRepresentante,
                    c.EsAccionistaMayoritario
                }).ToList();

                gvClientes.DataBind();
                gvClientes.Visible = true;
                lblResultado.Text = $"✅ {clientes.Count} resultado(s) encontrado(s).";
                lblResultado.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblResultado.Text = $"❌ Error: {ex.Message}";
                lblResultado.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void gvClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var row = gvClientes.SelectedRow;
            string nombreCliente = row.Cells[1].Text;
            string nombreAval = row.Cells[4].Text;

            lblSeleccion.Text = $"Cliente: {nombreCliente} | Aval: {nombreAval}";

            // Mostrar el modal Bootstrap
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarModal",
                "$('#modalDescripcion').modal('show');", true);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string descripcion = txtDescripcion.Text.Trim();
                var row = gvClientes.SelectedRow;

                if (row == null)
                {
                    lblResultado.Text = "⚠️ Debes seleccionar un cliente antes de guardar.";
                    lblResultado.ForeColor = System.Drawing.Color.OrangeRed;
                    return;
                }

                string clienteID = row.Cells[0].Text;
                string avalID = row.Cells[3].Text;
                string usuario = usuarioHidden.Value;
                MobileBO.Contabilidad.RevisionClientesEnOfacBO.BloqueaEnOfac(clienteID, usuario, descripcion);
                // Aquí guardarías la descripción en base de datos
                lblResultado.Text = "✅ Descripción guardada correctamente.";
                lblResultado.ForeColor = System.Drawing.Color.Green;

                // Cerrar el modal después de guardar
                ScriptManager.RegisterStartupScript(this, GetType(), "CerrarModal",
                    "$('#modalDescripcion').modal('hide');", true);

                txtDescripcion.Text = string.Empty;
            }
            catch (Exception ex)
            {
                lblResultado.Text = $"❌ Error al guardar: {ex.Message}";
                lblResultado.ForeColor = System.Drawing.Color.Red;
            }
        }

        // ✅ Este evento es para el GridView de SUSPENDIDOS (donde está el botón ⚙️ Cambiar Estatus)
        protected void gvSuspendidos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CambiarEstatus")
            {
                string bloqueoId = e.CommandArgument.ToString();

                // Guardamos el ID seleccionado
                ViewState["ClienteSeleccionado"] = bloqueoId;
                lblClienteSeleccionado.Text = "Cliente seleccionado: " + bloqueoId;

                // Mostramos el modal Bootstrap
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal",
                    "$('#modalCambiarEstatus').modal('show');", true);
            }
        }

        protected void btnGuardarEstatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["ClienteSeleccionado"] == null)
                {
                    lblResultado.Text = "⚠️ No hay cliente seleccionado.";
                    lblResultado.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string clienteId = ViewState["ClienteSeleccionado"].ToString();
                int nuevoEstatus = ddlEstatus.SelectedIndex + 1;
                string Estatus = nuevoEstatus.ToString();
                string motivo = txtMotivo.Text.Trim();

                string usuario = usuarioHidden.Value;


                MobileBO.Contabilidad.RevisionClientesEnOfacBO.ActualizaBloqueoEnOfac(clienteId, usuario,motivo , Estatus);
                lblResultado.Text = $"✅ Cliente actualizado a estatus: {nuevoEstatus}.";
                lblResultado.ForeColor = System.Drawing.Color.Green;

                ScriptManager.RegisterStartupScript(this, GetType(), "hideModal",
                    "$('#modalCambiarEstatus').modal('hide');", true);

                // Recargar la lista
                CargarListaSuspendidos();
            }
            catch (Exception ex)
            {
                lblResultado.Text = $"❌ Error al cambiar estatus: {ex.Message}";
                lblResultado.ForeColor = System.Drawing.Color.Red;
            }
        }
        protected string GetEstatusTexto(object estatusObj)
        {
            if (estatusObj == null) return string.Empty;

            string valor = estatusObj.ToString();

            switch (valor)
            {
                case "1":
                    return "🔒 BLOQUEADO";
                case "2":
                    return "🔓 DESBLOQUEADO";
                case "3":
                    return "🚫 PERMANENTE";
                default:
                    return "—";
            }
        }
        private static List<Entity.Contabilidad.Clientes> BuscarCliente(string nombre)
        {
            var origen = MobileBO.Contabilidad.RevisionClientesEnOfacBO.BuscarClientes(nombre);
            return origen?.Cast<Entity.Contabilidad.Clientes>().ToList() ?? new List<Entity.Contabilidad.Clientes>();
        }
    }
} 
