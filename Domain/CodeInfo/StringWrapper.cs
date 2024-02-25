using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CodeInfo
{
    /// <summary>
    /// Class made to be able to "pass string as references" instead of passing by copy
    /// </summary>
    public class StringWrapper
    {
        public string? data;
        public StringWrapper(string? data)
        {
            this.data = data;
        }
        public StringWrapper()
        {
        }
    }
}
