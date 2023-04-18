using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.Setting
{
    public interface IDLSetting
    {
        dynamic GetPasswordById(int id); 
    }
}
