using ActionResult;
using CharityAppDL.Charity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Charity
{
    public class BLCharity: IBLCharity
    {
        private IDLCharity _dlCharity;
        public BLCharity(IDLCharity iDLCharity)
        {
            this._dlCharity = iDLCharity;
        }

        public ReturnResult GetAllCharities()
        {
            var result = new ReturnResult();
            try
            {                
                result.Ok(_dlCharity.GetAllCharity());
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message});
            }
            return result;
        }

        public ReturnResult GetCharity(int charityId)
        {
            var result = new ReturnResult();
            try
            {
                var _rs = _dlCharity.GetAllCharity();
                if(_rs != null )
                {
                    result.Ok(_rs);
                }
                else
                {
                    result.BadRequest(new List<string> { "Tổ chức bạn tìm kiếm không có." });
                }
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
            }
            return result;
        }
    }
}
