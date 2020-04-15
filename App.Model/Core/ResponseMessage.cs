using System.Collections.Generic;
using System.Linq;

namespace App.Model.Core
{
    public class ResponseMessage
    {
        public List<string> Errors { get; private set; }
        public List<string> Warnings { get; private set; }
        public bool IsValid { get { return !Errors.Any(); } set { } }
        public int EntityId { get; set; }

        public ResponseMessage()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
        }
    }
}