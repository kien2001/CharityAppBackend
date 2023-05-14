using CharityAppBO.Charity;
using CharityAppBO.Users;
using Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.Charity
{
    public interface IDLCharity
    {
        List<CharityFollow> GetAllCharities(int? userId);


        CharityObj GetCharityById(int charityId, int? userId);

    }
}
