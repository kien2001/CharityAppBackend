using ActionResult;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Charity
{
    public interface IBLCharity
    {
        ReturnResult GetAllCharities(int? userId);

        ReturnResult GetCharityById(int charityId, int? userId);

        Task<ReturnResult> SaveVerifiedImage(List<IFormFile> files, string message, int charityId);
    }
}
