namespace App.Core.Interfaces
{
    public interface IHSM
    {
        byte[] Sign(byte[] documento, string Firmante, string location, string folio, string razon);
        string[] GetSigners();
    }
}