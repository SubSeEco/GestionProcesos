using System.Collections.Generic;
using System.Drawing;

namespace App.Core.Interfaces
{
    public interface IMinsegpres
    {
        byte[] Sign(byte[] documento, string OTP, string tokenJWT, int id, string Run);

    }
}
