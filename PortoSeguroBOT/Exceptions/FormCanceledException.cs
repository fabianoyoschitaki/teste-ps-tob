using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoSeguroBOT.Exceptions
{
    public class FormCanceledException : Exception
    {
        public FormCanceledException()
        {
        }

        public FormCanceledException(string message) : base(message)
        {
        }

        public FormCanceledException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}