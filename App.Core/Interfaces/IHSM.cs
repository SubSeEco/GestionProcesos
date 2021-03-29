using System.Collections.Generic;
using System.Drawing;

namespace App.Core.Interfaces
{
    public interface IHSM
    {
        byte[] Sign(byte[] documento, List<string> Firmante, int documentoId, string folio, string url, byte[] QR);
    }
}