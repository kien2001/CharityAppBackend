using ActionResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Charity
{
    public interface IBLCharity
    {
        ReturnResult GetAllCharities();
    }
}
