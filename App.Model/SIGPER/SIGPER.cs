namespace App.Model.Sigper
{
    public class Sigper
    {
        public Sigper()
        {
            Funcionario = new PEDATPER();
            Unidad = new PLUNILAB();
            Jefatura = new PEDATPER();
            Secretaria = new PEDATPER();
            DatosLaborales = new PeDatLab();
            Contrato = new ReContra();
        }
        public PEDATPER Funcionario { get; set; }
        public PLUNILAB Unidad { get; set; }
        public PEDATPER Jefatura { get; set; }
        public PEDATPER Secretaria { get; set; }
        public PeDatLab DatosLaborales { get; set; }
        public ReContra Contrato { get; set; }
        public string SubSecretaria { get; set; }
    }
}