using System.Collections.Generic;
using System.Drawing;

namespace App.Core.Interfaces
{
    public interface IHSM
    {
        byte[] Sign(byte[] documento, string Firmante, string location, string folio, string razon);
        byte[] Sign(byte[] documento, List<string> Firmante, int documentoId, string folio, string url, byte[] QR);
        string[] GetSigners();
    }
}