using System.Collections.Generic;

namespace App.Core.Interfaces
{
    public interface IHsm
    {
        byte[] Sign(byte[] documento, List<string> firmante, int documentoId, string folio, string url, byte[] qr);
    }
}