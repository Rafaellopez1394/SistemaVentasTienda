// CapaModelo/DescomposicionProducto.cs
using System;
using System.Collections.Generic;

namespace CapaModelo
{
    /// <summary>
    /// Representa la descomposición de un producto grande en productos más pequeños
    /// Ejemplo: 1 costal de 20kg → 5 bolsas de 2kg + 10 bolsas de 1kg
    /// </summary>
    public class DescomposicionProducto
    {
        public int DescomposicionID { get; set; }
        public int ProductoOrigenID { get; set; }
        public string ProductoOrigenNombre { get; set; }
        public decimal CantidadOrigen { get; set; }
        public DateTime FechaDescomposicion { get; set; }
        public string Usuario { get; set; }
        public string Observaciones { get; set; }
        public bool Estatus { get; set; }
        
        // Lista de productos resultantes
        public List<DetalleDescomposicion> Detalle { get; set; }
    }

    /// <summary>
    /// Detalle de cada producto resultante de la descomposición
    /// </summary>
    public class DetalleDescomposicion
    {
        public int DetalleDescomposicionID { get; set; }
        public int DescomposicionID { get; set; }
        public int ProductoResultanteID { get; set; }
        public string ProductoResultanteNombre { get; set; }
        public decimal CantidadResultante { get; set; }
        public decimal? PesoUnidad { get; set; } // Peso de cada unidad en kg (ej: 1.0, 2.0, 0.5)
    }

    /// <summary>
    /// Payload para registrar una descomposición
    /// </summary>
    public class RegistrarDescomposicionPayload
    {
        public int ProductoOrigenID { get; set; }
        public decimal CantidadOrigen { get; set; }
        public int SucursalID { get; set; }
        public string Usuario { get; set; }
        public string Observaciones { get; set; }
        public List<DetalleDescomposicionPayload> Detalle { get; set; }
    }

    /// <summary>
    /// Detalle del payload de descomposición
    /// </summary>
    public class DetalleDescomposicionPayload
    {
        public int ProductoResultanteID { get; set; }
        public decimal CantidadResultante { get; set; }
        public decimal? PesoUnidad { get; set; }
    }

    /// <summary>
    /// Historial de descomposiciones para vista
    /// </summary>
    public class HistorialDescomposicion
    {
        public int DescomposicionID { get; set; }
        public DateTime FechaDescomposicion { get; set; }
        public string Usuario { get; set; }
        public string ProductoOrigen { get; set; }
        public decimal CantidadOrigen { get; set; }
        public string UnidadOrigen { get; set; }
        public string ProductosResultantes { get; set; }
        public string Observaciones { get; set; }
        public bool Estatus { get; set; }
    }
}




