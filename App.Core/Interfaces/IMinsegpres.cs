using System.Collections.Generic;
using System.Drawing;

namespace App.Core.Interfaces
{
    public interface IMinsegpres
    {
        byte[] Sign(byte[] documento, string OTP, int id, string Run, string Nombre, bool TipoDocumento);

    }
}
