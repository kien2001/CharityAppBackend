using ActionResult;
using Base;
using CharityAppBO.Charity;
using CharityAppBO.Users;
using CharityAppDL.Charity;
using Microsoft.AspNetCore.Http;
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
        private IDLBase _base;
        public BLCharity(IDLCharity iDLCharity, IDLBase @base)
        {
            this._dlCharity = iDLCharity;
            _base = @base;
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

        public ReturnResult GetVerifiedImage(int charityId)
        {
            var result = new ReturnResult();
            try
            {
                var _rs = _dlCharity.GetVerifiedImage(charityId);
                if (_rs != null)
                {
                    result.Ok(_rs);
                }
                else
                {
                    result.BadRequest(new List<string> { "Có lỗi xảy ra, vui lòng thử lại" });
                }
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
            }
            return result;
           
        }

        public async Task<ReturnResult> SaveVerifiedImage(List<IFormFile> files, string message, int charityId)
        {
            var result = new ReturnResult();
            List<Task<string>> saveTasks = new List<Task<string>>();
            List<string> ContentTypeImage = new() { "image/jpeg", "image/png" };
            try
            {
                foreach (var file in files)
                {
                    if (file.Length < 2097152)
                    {
                        // xử lý file nhỏ
                        if (ContentTypeImage.Contains(file.Headers.ContentType.ToString()))
                        {
                            Task<string> fileUrl = _base.UploadFileFirebase(file, file.FileName);
                            saveTasks.Add(fileUrl);
                        }
                    }
                }
                string[] listImgUrl = await Task.WhenAll(saveTasks);
                if (listImgUrl.Length > 0)
                {
                    var _rs =  _dlCharity.SaveVerifiedImage(string.Join(", ", listImgUrl), message, charityId);
                    if (_rs > 0)
                    {
                        result.Ok(_rs);
                    }
                    else
                    {
                        result.BadRequest(new List<string> { "Có lỗi xảy ra, vui lòng thử lại" });
                    }
                }
                else
                {
                    result.BadRequest(new List<string> { "Có lỗi xảy ra trong quá trình upload file, vui lòng thử lại" });
                }
                return result;
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
                return result;
            }
        }
    }
}
