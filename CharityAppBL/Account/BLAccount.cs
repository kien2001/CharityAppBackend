﻿using ActionResult;
using CharityAppBO.Account;
using CharityAppBO.Users;
using CharityAppDL.User;
using Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Users
{
    public class BLAccount : IBLAccount
    {
        private IDLAccount _dLAccount;
        public BLAccount(IDLAccount dLUser) {
            _dLAccount = dLUser;
        }

        /// <summary>
        /// Lấy thông tin user có id truyền vào
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ReturnResult GetUser(int id)
        {
            var result = new ReturnResult();
            var (user, numFollow, numCampaign) = _dLAccount.GetUser(id);
            if (user != null)
            {
                var roleId = user?.RoleId;
                if (roleId != null)
                {
                    if (int.Parse(roleId.ToString()) == (int)RoleUser.UserCharity)
                    {
                        var _user = CharityUtil.ConvertToType<UserCharityReturn>(user);

                        _user = CharityUtil.ToExpando(_user);
                        _user.NumCampaign = numCampaign;
                        _user.NumFollow = numFollow;
                        user = CharityUtil.ConvertToType<UserCharityReturns>(_user);
                    }
                    else if (int.Parse(roleId.ToString()) == (int)RoleUser.UserNormal)
                    {
                        user = CharityUtil.ConvertToType<UserNormalReturn>(user);
                    }
                }
                result.Ok(user);
                return result;
            }
            result.BadRequest(new List<string>() { "Người dùng không tồn tại" });
            return result;
        }
        /// <summary>
        /// Lấy tất cả user
        /// </summary>
        /// <returns></returns>
        public ReturnResult GetAllUser()
        {
            var result = new ReturnResult();
            var objReturn = new List<object>();
            var listUser = _dLAccount.GetAllUser();
            if (listUser != null && listUser.Count > 0)
            {
                foreach (var user in listUser)
                {
                    var roleId = user?.RoleId;
                    if (roleId != null)
                    {
                        if (int.Parse(roleId.ToString()) == (int)RoleUser.UserCharity)
                        {
                            objReturn.Add(CharityUtil.ConvertToType<UserCharityReturn>(user));
                        }
                        else if (int.Parse(roleId.ToString()) == (int)RoleUser.UserNormal)
                        {
                            objReturn.Add(CharityUtil.ConvertToType<UserNormalReturn>(user));
                        }
                    }
                }
                result.Ok(objReturn);
                return result;
            }
            result.BadRequest(new List<string>() { "Có lỗi xảy ra, vui lòng thử lại" });
            return result;
        }

        public ReturnResult ChangeStatus(UpdateStatusUser updateStatusUser)
        {
            var returnResult = new ReturnResult();
            var result = _dLAccount.UpdateStatusUser(updateStatusUser.Id, updateStatusUser.Status);
            if (result > 0)
            {
                returnResult.Ok(result);
                return returnResult;
            }
            returnResult.BadRequest(new List<string>() { "Không thể update" });
            return returnResult;
        }

        public ReturnResult ChangeStatusVerify(VerifyStatus verifyStatus)
        {
            var result = new ReturnResult();
            var _rs = 0;
            try
            {
                if (verifyStatus.IsAccepted)
                {
                    _rs = _dLAccount.ChangeStatusVerify(verifyStatus.CharityId, verifyStatus.IsAccepted, null);
                }
                else
                {
                    _rs = _dLAccount.ChangeStatusVerify(verifyStatus.CharityId, verifyStatus.IsAccepted, verifyStatus.Message);
                }
                if (_rs > 0)
                {
                    result.Ok(_rs);
                }
                else
                {
                    result.BadRequest(new List<string>() { "Có lỗi xảy ra, vui lòng thử lại" });
                }
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string>() { e.Message });
            }
           
            return result;
        }

        public ReturnResult GetAllVerifiedList()
        {
            var result = new ReturnResult();
            try
            {
                var _rs = _dLAccount.GetAllStatusVerify();
                if (_rs != null && _rs.Count > 0)
                {
                    result.Ok(_rs);
                }
                else
                {
                    result.BadRequest(new List<string>() { "Co loi xay ra" });
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
