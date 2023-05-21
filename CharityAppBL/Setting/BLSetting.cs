using ActionResult;
using Base;
using CharityAppBO.Setting;
using CharityAppDL.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Setting
{
    public class BLSetting : IBLSetting
    {
        private readonly IDLSetting _DLSetting;
        private readonly IDLBase _dlBase;

        public BLSetting(IDLSetting setting, IDLBase dLBase)
        {
            _DLSetting = setting;
            _dlBase = dLBase;
        }
        public ReturnResult CheckPassword(int id, string password)
        {
            var result = new ReturnResult();
            var infoPassword = _DLSetting.GetPasswordById(id);
            if (infoPassword != null)
            {
                string passwordHash = infoPassword.Password;
                string passwordSalt = infoPassword.SaltPassword;
                if (passwordHash != null && passwordSalt != null)
                {
                    bool validatePassword = CharityUtil.VerifyPasswordHash(password, passwordHash, passwordSalt);
                    if (validatePassword)
                    {
                        result.Ok(true);
                    }
                    else
                    {
                        result.BadRequest(new List<string>()
                        {
                            "Mật khẩu không đúng, vui lòng thử lại"
                        });
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Cap nhat thong tin cua user charity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userCharityUpdate"></param>
        /// <returns></returns>
        public async Task<ReturnResult> UpdateCharityInfo(List<IFormFile> files, int id, UserCharityUpdate userCharityUpdate)
        {
            var result = new ReturnResult();
            try
            {
                List<Task<string>> saveTasks = new List<Task<string>>();
                List<string> ContentTypeImage = new() { "image/jpeg", "image/png" };
                if(files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length < 2097152)
                        {
                            // xử lý file nhỏ
                            if (ContentTypeImage.Contains(file.Headers.ContentType.ToString()))
                            {
                                Task<string> fileUrl = _dlBase.UploadFileFirebase(file, file.FileName);
                                saveTasks.Add(fileUrl);
                            }
                        }
                    }
                }
                string[] listImgUrl = await Task.WhenAll(saveTasks);
                if(listImgUrl.Length > 0)
                {
                    userCharityUpdate.CharityInfo.CharityImages = string.IsNullOrEmpty(userCharityUpdate.CharityInfo.CharityImages) ? string.Join(", ", listImgUrl) : string.Concat(userCharityUpdate.CharityInfo.CharityImages, ", ", string.Join(", ", listImgUrl));
                }
                int _rs = _DLSetting.UpdateCharityInfo(id, userCharityUpdate);
                if (_rs > 0)
                {
                    result.Ok(_rs);
                }
                else
                {
                    result.BadRequest(new List<string>() { "Update khong thanh cong" });
                }
                return result;
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string>() { e.Message });
                return result;
            }
        }

        /// <summary>
        /// Cap nhat mat khau cua user charity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatePassword"></param>
        /// <returns></returns>
        public ReturnResult UpdateCharityPassword(int id, UpdatePassword updatePassword)
        {
            var result = new ReturnResult();
            var (checkPassword, newPasswordHash) = CheckExistPassword(id, updatePassword.OldPassword, updatePassword.NewPassword);
            if (!checkPassword)
            {
                result.BadRequest(new List<string>()
                                    {
                                        "Mat khau khong khop voi mat khau hien tai, vui long thu lai"
                                    });
                return result;
            }

            int _rs = _DLSetting.UpdateCharityPassword(id, newPasswordHash);
            if (_rs > 0)
            {
                result.Ok(_rs);
            }
            else
            {
                result.BadRequest(new List<string>() { "Update khong thanh cong" });
            }
            return result;
        }

        public async Task<ReturnResult> UpdateInfo(int id, bool isUpdatePassword, UserNormalUpdate userNormalUpdate)
        {
            var result = new ReturnResult();
            string tableName = "user_account";
            var updateColumns = new Dictionary<string, string>();
            var excludeColumns = new List<string>()
            {
                "OldPassword", "NewPassword", "ConfirmNewPassword"
            };
            var whereCondition = new Dictionary<string, OperatorWhere>()
            {
                {"Id", new OperatorWhere(){ Operator= CharityAppBO.Operator.Equal, Value = id.ToString() } }
            };
            try
            {
                // chi update info, ko update password
                foreach (var property in userNormalUpdate.GetType().GetProperties())
                {
                    if (!excludeColumns.Contains(property.Name))
                    {
                        updateColumns.Add(property.Name, property.GetValue(userNormalUpdate).ToString());
                    }
                }
                int _rs = _DLSetting.UpdateInfo(tableName, updateColumns, whereCondition);
                if (_rs > 0)
                {
                    result.Ok(_rs);
                }
                else
                {
                    result.BadRequest(new List<string>() { "Update khong thanh cong" });
                }
                return result;
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string>() { e.Message });
                return result;
            }
        }

        /// <summary>
        /// Luu avatar len firebase
        /// </summary>
        /// <param name="avatarKey"></param>
        /// <returns>Link image</returns>
        /// <exception cref="Exception"></exception>
        //private async Task<string> SaveAndGetAvatar(string avatarKey)
        //{
        //    string avatarUrl = string.Empty;
        //    if (String.IsNullOrEmpty(avatarKey))
        //    {
        //        return avatarUrl;
        //    }
        //    try
        //    {
        //        var memoryStreamAvatarObj = _dlBase.GetDataRedis<FileSave>(avatarKey);
        //        if (memoryStreamAvatarObj != null)
        //        {
        //            var memoryStreamAvatar = new MemoryStream(memoryStreamAvatarObj.Data);
        //            avatarUrl = await _dlBase.UploadFileFirebase(memoryStreamAvatar, memoryStreamAvatarObj.FileName);

        //        }

        //        return avatarUrl;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
            
        //}

        private (bool, string) CheckExistPassword(int id, string password, string newPassword)
        {
            bool result = false;
            string newPasswordHash = String.Empty;
            var infoPassword = _DLSetting.GetPasswordById(id);
            if (infoPassword != null)
            {
                string passwordHash = infoPassword.Password;
                string passwordSalt = infoPassword.SaltPassword;
                if (passwordHash != null && passwordSalt != null)
                {
                    bool validatePassword = CharityUtil.VerifyPasswordHash(password, passwordHash, passwordSalt);
                    if (validatePassword)
                    {
                        result = true;
                        newPasswordHash = CharityUtil.CreatePasswordHash(newPassword, passwordSalt);
                    }
                    else
                    {
                        result = false;
                        newPasswordHash = String.Empty;
                    }
                }
            }
            return (result, newPasswordHash);
        }
    }
}
