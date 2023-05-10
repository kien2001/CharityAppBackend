using ActionResult;
using Base;
using CharityAppBO.Setting;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Upload
{
    public class BLUpload : IBLUpload
    {
        private const string key_avatar = "_avatar";
        private readonly IDLBase _base;
        public BLUpload(IDLBase dLBase)
        {
            _base = dLBase;
        }
        public async Task<ReturnResult> UploadFile(IFormFile formFile)
        {
            var result = new ReturnResult();
            List<string> ContentTypeImage = new() { "image/jpeg", "image/png" };
            if (formFile == null || formFile.Length == 0)
            {
                result.BadRequest(new List<string> { "You have not chosen file yet." });
                return result;
            }
            try
            {
                if (formFile.Length < 2097152)
                {
                    // xử lý file nhỏ
                    // ảnh -> lưu vào cache
                    if (ContentTypeImage.Contains(formFile.Headers.ContentType.ToString()))
                    {
                        using var stream = new MemoryStream();
                        await formFile.CopyToAsync(stream);
                        stream.Position = 0;
                        string fileUrl = await _base.UploadFileFirebase(stream, formFile.FileName);
                        result.Ok(fileUrl);
                    }
                }
                //else
                //{
                //    // xử lý file lớn
                //}
                return result;
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message});
                return result;
            }
        }

    }
}
