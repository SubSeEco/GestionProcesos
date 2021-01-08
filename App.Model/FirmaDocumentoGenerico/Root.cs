using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model.FirmaDocumentoGenerico
{
    public class File
    {
        public string content { get; set; }
        public string status { get; set; }
        public string contentType { get; set; }
        public string description { get; set; }
        public string checksum_original { get; set; }
        public string documentStatus { get; set; }
    }

    public class Metadata
    {
        public bool otpExpired { get; set; }
        public int filesSigned { get; set; }
        public int signedFailed { get; set; }
        public int objectsReceived { get; set; }
    }

    public class Root
    {
        public List<File> files { get; set; }
        public Metadata metadata { get; set; }
        public long idSolicitud { get; set; }

        public string timestamp { get; set; }
        public int status { get; set; }
        public string error { get; set; }
    }
}
