using System.Collections.Generic;
using System.Drawing;

namespace App.Core.Interfaces
{
    public interface IHSM
    {
        //Método deprecado, se recomienda usar la nueva sobrecarga
        byte[] Sign(byte[] documento, string Firmante, string location, string folio, string razon);

        //Nuevo método, agrega tabla e verificación de documentos
        byte[] Sign(byte[] documento, List<string> Firmante, int documentoId, string folio, string url, byte[] QR);

        //string[] GetSigners();
    }
}