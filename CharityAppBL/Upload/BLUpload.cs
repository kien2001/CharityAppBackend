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
        public async Task<ReturnResult> UploadFile(int id, IFormFile formFile)
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
                        string key = id.ToString() + key_avatar;
                        using var stream = new MemoryStream();
                        await formFile.CopyToAsync(stream);
                        var objSave = new FileSave()
                        {
                            FileName = formFile.FileName,
                            Data = stream.ToArray()
                        };
                        _base.SaveDataRedis(key, objSave, null);
                        result.Ok(key);
                        return result;

                    }
                }
                //else
                //{
                //    // xử lý file lớn
                //}
                result.BadRequest(new List<string> { "Có lỗi xảy ra, vui lòng thử lại" });
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
