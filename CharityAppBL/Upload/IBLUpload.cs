using ActionResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Upload
{
    public interface IBLUpload
    {
        Task<ReturnResult> UploadFile(int id, IFormFile formFile);
    }
}
