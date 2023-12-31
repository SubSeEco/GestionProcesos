﻿using System.Collections.Generic;
using System.Linq;

namespace App.Model.Core
{
    public class ResponseMessage
    {
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public bool IsValid => !Errors.Any();
        public int EntityId { get; set; }

        public ResponseMessage()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
        }
    }
}