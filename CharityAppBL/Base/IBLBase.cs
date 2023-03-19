using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public interface IBLBase
    {
        public string CreatePasswordHash(string password);
    }
}
