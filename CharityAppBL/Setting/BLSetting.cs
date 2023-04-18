using ActionResult;
using CharityAppBO.Setting;
using CharityAppDL.Setting;
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
        public BLSetting(IDLSetting setting)
        {
            _DLSetting = setting;
        }
        public ReturnResult CheckPassword(int id, string password)
        {
            var result = new ReturnResult();
            var infoPassword = _DLSetting.GetPasswordById(id);
            if(infoPassword != null)
            {
                string passwordHash = infoPassword.Password;
                string passwordSalt = infoPassword.SaltPassword;
                if(passwordHash != null && passwordSalt != null) 
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

        public ReturnResult UpdateInfo(int id, int roleId, string isUpdatePassword, object userUpdate)
        {
            var result = new ReturnResult();
            try
            {
                // user binh thuong
                if (roleId == 0)
                {
                    userUpdate = CharityUtil.ConvertToType<UserNormalUpdate>(userUpdate);
                }
                else if (roleId == 1)
                {
                    userUpdate = CharityUtil.ConvertToType<UserCharityUpdate>(userUpdate);
                }
                // update User khach
                if(isUpdatePassword == null)
                {
                    var checkPassword = CheckExistPassword(id, userUpdate.GetType().GetProperty("OldPassword").GetValue(userUpdate).ToString());
                    if (checkPassword)
                    {

                    }
                }
                else
                {
                    var _isUpdatePassword = bool.Parse(isUpdatePassword);
                    // update password cho to chuc
                    if (_isUpdatePassword)
                    {
                        var checkPassword = CheckExistPassword(id, userUpdate.GetType().GetProperty("OldPassword").GetValue(userUpdate).ToString());
                        if (checkPassword)
                        {

                        }
                        else
                        {
                            result.BadRequest(new List<string>()
                            {
                                "Mat khau ban nhap khong khop, vui long thu lai."
                            });
                        }
                    }
                    //update info cho to chuc
                    else
                    {

                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CheckExistPassword(int id, string password)
        {
            bool result = false;
            if(password != null) { 
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
                            result= true;
                        }
                        else
                        {
                            result= false;
                        }
                    }
                }
            }
            return result;
        }
    }
}
