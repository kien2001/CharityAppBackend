using ActionResult;
using CharityAppBO.Charity;
using CharityAppBO.Users;
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

        public ReturnResult GetAllCharities(int? userId)
        {
            var result = new ReturnResult();
            try
            { 
                List<CharityFollow> charityFollows = new List<CharityFollow>();
                if (userId.HasValue)
                {
                    charityFollows = _dlCharity.GetAllCharities(userId);
                }
                else
                {
                    charityFollows = _dlCharity.GetAllCharities(null);
                }
                result.Ok(charityFollows);
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message});
            }
            return result;
        }

        public ReturnResult GetCharityById(int charityId, int? userId)
        {
            var result = new ReturnResult();
            try
            {
                CharityObj _rs = new CharityObj();
                if(userId == null)
                {
                    _rs = _dlCharity.GetCharityById(charityId, null);
                }
                else
                {
                    _rs = _dlCharity.GetCharityById(charityId, userId);
                }
                if (_rs != null)
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
