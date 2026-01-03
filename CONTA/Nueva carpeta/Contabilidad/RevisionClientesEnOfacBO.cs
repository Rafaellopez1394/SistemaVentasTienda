using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity;

namespace MobileBO.Contabilidad
{
    public class RevisionClientesEnOfacBO
    {
        public static ListaDeEntidades<Entity.Contabilidad.Clientes> GenerarListaClientes()
        {
            return MobileDAL.Contabilidad.RevisionClientesEnOfac.GeneraListaClientes();
        }



        // 🔹 Método corregido: define tipo del parámetro y adapta el retorno
        public static ListaDeEntidades<Entity.Contabilidad.Clientes> BuscarCliente(string nombre)
        {
            // Obtener la lista del DAL
            var lista = MobileDAL.Contabilidad.RevisionClientesEnOfac.BuscarCoincidenciasPorTipo(nombre);

            // Convertir List<> a ListaDeEntidades<>
            var listaEntidades = new ListaDeEntidades<Entity.Contabilidad.Clientes>();
            foreach (var item in lista)
            {
                listaEntidades.Add(item);
            }

            listaEntidades.AcceptChanges();
            return listaEntidades;
        }

        public static void BloqueaEnOfac(string clienteId, string usuario, string descripcion)
        {
            // Llamada directa al método DAL, sin retorno
            MobileDAL.Contabilidad.RevisionClientesEnOfac.AgregarPersonaBloqueada(clienteId,
                usuario,
                descripcion
            );
        }
        // 🔹 Método corregido: define tipo del parámetro y adapta el retorno
        public static ListaDeEntidades<Entity.Contabilidad.Clientes> BuscarClientes(string nombre)
        {
            // Obtener la lista del DAL
            var lista = MobileDAL.Contabilidad.RevisionClientesEnOfac.BuscarCoincidencias(nombre);

            // Convertir List<> a ListaDeEntidades<>
            var listaEntidades = new ListaDeEntidades<Entity.Contabilidad.Clientes>();
            foreach (var item in lista)
            {
                listaEntidades.Add(item);
            }

            listaEntidades.AcceptChanges();
            return listaEntidades;
        }

        public static ListaDeEntidades<Entity.Contabilidad.Clientes> ConsultaBloqueado()
        {
            // Obtener la lista del DAL
            return MobileDAL.Contabilidad.RevisionClientesEnOfac.GeneraListasuspendidos();
        }
        

        public static void  ActualizaBloqueoEnOfac(string clienteId, string usuario, string descripcion, string estatus)
        {
            // Obtener la lista del DAL
             MobileDAL.Contabilidad.RevisionClientesEnOfac.ActualizarPersonaBloqueada(clienteId,
                usuario,
                descripcion,
                estatus);
        }
    }
}
