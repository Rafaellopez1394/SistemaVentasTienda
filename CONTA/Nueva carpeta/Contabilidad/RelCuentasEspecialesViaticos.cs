using System;

namespace Entity.Contabilidad
{

    /// <sumary>
    /// Representa una entidad de Relcuentasespecialesviatico para su trabajo en memoria
    /// </sumary>
    [Serializable]
    public class Relcuentasespecialesviatico
    {
        #region Atributos
        public string Relcuentaespecialviaticoid { get; set; }
        public string Cuentagastos { get; set; }
        public string Cuentagastosreemplazo { get; set; }
        public string Cuentadeudor { get; set; }
        public string Cuentadeudorreemplazo { get; set; }
        public string Cuentaacreedor { get; set; }
        public string Cuentaacreedorreemplazo { get; set; }
        public DateTime Fecha { get; set; }
        public int Estatus { get; set; }
        public string Usuario { get; set; }
        public string Nombreempleado { get; set; }
        public int UltimaAct { get; set; }
        #endregion //Atributos

        #region Constructor

        ///<sumary>
        ///Constructor
        ///</sumary>
        public Relcuentasespecialesviatico()
        {
            Relcuentaespecialviaticoid = System.Guid.Empty.ToString();
        }

        #endregion Constructor

    }
}
