﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostDnFhirR4Api.Class
{
    public class TransientException : Exception
    {
        public TransientException(string message) :
          base(message)
        {
        }
    }
}
