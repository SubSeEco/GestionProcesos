using System.Collections.Generic;

namespace App.Core.Interfaces
{
    public interface IHSM
    {
        byte[] Sign(byte[] documento, List<string> Firmante, int documentoId, string folio, string url, byte[] QR);
    }
}