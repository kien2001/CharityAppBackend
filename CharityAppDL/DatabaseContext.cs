using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CharityBackendDL
{
    public class DatabaseContext
    {
        public static string ConnectionString;

        public static IConfigurationSection ConfigJwt;
    }
}
