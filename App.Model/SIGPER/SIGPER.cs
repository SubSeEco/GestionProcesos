namespace App.Model.SIGPER
{
    public class SIGPER
    {
        public PEDATPER Funcionario { get; set; }
        public PLUNILAB Unidad { get; set; }
        public PEDATPER Jefatura { get; set; }
        public PEDATPER Secretaria { get; set; }
        public PeDatLab FunDatosLaborales { get; set; }
        public string SubSecretaria { get; set; }
    }
}