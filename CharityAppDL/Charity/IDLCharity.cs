using CharityAppBO.Charity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.Charity
{
    public interface IDLCharity
    {
        List<CharityItem> GetAllCharity();
    }
}
