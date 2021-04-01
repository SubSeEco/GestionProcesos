namespace App.Core.Interfaces
{
    public interface IMinsegpres
    {
        byte[] SignConOtp(byte[] documento, string otp, int id, string run, string nombre, bool tipoDocumento, int documentoId);
        
        byte[] SignSinOtp(byte[] documento, string otp, int id, string run, string nombre, string tipoDocumento, int documentoId);
    }
}
