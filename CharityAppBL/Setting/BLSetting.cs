using ActionResult;
using Base;
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

        public ReturnResult UpdateInfo<T>(int id, int roleId, bool isUpdatePassword, T userUpdate) where T : class
        {
            var result = new ReturnResult();
            string tableName = "user_account";
            string oldPassword = "";
            if (userUpdate.GetType().GetProperty("OldPassword").GetValue(userUpdate) != null)
            {
                oldPassword = userUpdate.GetType().GetProperty("OldPassword").GetValue(userUpdate).ToString();
            }
            var updateColumns = new Dictionary<string, string>();
            var passwordColumns = new List<string>()
            {
                "OldPassword", "NewPassword", "ConfirmNewPassword"
            };
            var whereCondition = new Dictionary<string, OperatorWhere>()
            {
                {"Id", new OperatorWhere(){ Operator= CharityAppBO.Operator.Equal, Value = id.ToString() } }
            };
            switch (roleId)
            {
                case (int)RoleUser.Admin:
                    break;

                case (int)RoleUser.UserNormal:
                    if (!isUpdatePassword)
                    {
                        // chi update info, ko update password
                        foreach (var property in userUpdate.GetType().GetProperties())
                        {
                            if (!passwordColumns.Contains(property.Name.Trim()) && property.GetValue(userUpdate) != null)
                            {
                                updateColumns.Add(property.Name, property.GetValue(userUpdate).ToString());
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
                    }
                    else
                    {
                        // update ca password va info
                        string newPassword = userUpdate.GetType().GetProperty("NewPassword").GetValue(userUpdate).ToString();
                        var (checkPassword, newPasswordHash) = CheckExistPassword(id, oldPassword, newPassword);
                        if (!checkPassword)
                        {
                            result.BadRequest(new List<string>()
                                    {
                                        "Mat khau khong khop voi mat khau hien tai, vui long thu lai"
                                    });
                            return result;
                        }
                        foreach (var property in userUpdate.GetType().GetProperties())
                        {
                            if (!passwordColumns.Contains(property.Name.Trim()))
                            {
                                updateColumns.Add(property.Name, property.GetValue(userUpdate).ToString());
                            }
                            else
                            {
                                if(property.Name == "NewPassword")
                                {
                                    updateColumns.Add("Password", newPasswordHash);
                                }
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
                    }
                    break;

                case (int)RoleUser.UserCharity:
                    if (isUpdatePassword)
                    {
                        // chi update password
                        string newPassword = userUpdate.GetType().GetProperty("NewPassword").GetValue(userUpdate).ToString();
                        var (checkPassword, newPasswordHash) = CheckExistPassword(id, oldPassword, newPassword);
                        if (!checkPassword)
                        {
                            result.BadRequest(new List<string>()
                                    {
                                        "Mat khau khong khop voi mat khau hien tai, vui long thu lai"
                                    });
                            return result;
                        }

                        updateColumns.Add("Password", newPasswordHash);
                        int _rs = _DLSetting.UpdateInfo(tableName, updateColumns, whereCondition);
                        if (_rs > 0)
                        {
                            result.Ok(_rs);
                        }
                        else
                        {
                            result.BadRequest(new List<string>() { "Update khong thanh cong" });
                        }
                    }
                    else
                    {
                        // chi update info
                        foreach (var property in userUpdate.GetType().GetProperties())
                        {
                            if (!passwordColumns.Contains(property.Name.Trim()) && property.GetValue(userUpdate) != null)
                            {
                                updateColumns.Add(property.Name, property.GetValue(userUpdate).ToString());
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
                    }
                    break;
                default:
                    result.InternalServer(null);
                    break;
            }
            return result;
        }

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
