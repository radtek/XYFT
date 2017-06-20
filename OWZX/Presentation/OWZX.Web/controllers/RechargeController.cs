using OWZX.Core;
using OWZX.Model;
using OWZX.Services;
using OWZX.Web.Framework;
using OWZX.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OWZX.Web.controllers
{
    /// <summary>
    /// 充值
    /// </summary>
    public class RechargeController : BaseWebController
    {
        //
        // GET: /Recharge/

        public ActionResult Charge()
        {
            return View();
        }
        public ActionResult ChargeList()
        {
            List<MD_Remit> list = NewUser.GetUserRemitList(1, 50, " where a.uid=" + WorkContext.Uid.ToString());
            RechargeListModel recharge = new RechargeListModel
            {
                PageModel = new PageModel(15, 1, list.Count > 0 ? list[0].TotalCount : 0),
                RechargeList = list
            };
            return View(recharge);
        }
        /// <summary>
        /// 选择转账卡
        /// </summary>
        /// <returns></returns>
        public ActionResult BankCharge()
        {
            return View();
        }
        /// <summary>
        /// 支付宝转账
        /// </summary>
        /// <returns></returns>
        public ActionResult AlipayCharge()
        {
            return View();
        }
        /// <summary>
        /// 微信转账
        /// </summary>
        /// <returns></returns>
        public ActionResult WeChatCharge()
        {
            return View();
        }

        /// <summary>
        /// 是否绑定手机
        /// </summary>
        /// <returns></returns>
        public ActionResult ExistsMobile()
        {
            try
            {
                PartUserInfo user = Users.GetPartUserById(WorkContext.Uid);

                if (user.Mobile.Trim() == string.Empty || user.Mobile == null)
                    return Content("1");//未绑定
                else
                    return Content("2");

            }
            catch (Exception ex)
            {
                return Content("3");
            }
        }

        #region 存取款
        public ActionResult CQOperate()
        {
            ViewData["type"] = WebHelper.GetQueryInt("type");
            ViewData["money"] = Users.GetPartUserById(WorkContext.Uid).TotalMoney;
            List<DrawInfoModel> list = Recharge.GetDrawList(1, -1, " where a.uid=" + WorkContext.Uid.ToString() + " and CONVERT(varchar(10),a.addtime,120)=CONVERT(varchar(10),getdate(),120)");
            int count = 3;
            if (list == null || list.Count == 0)
                count = 3;
            else if (list.Count >= 3)
                count = 0;
            else
                count = 3 - list.Count;
            ViewData["drawcount"] = count;
            ViewData["setdraw"] = Recharge.ValidateDrawPwdByUid(WorkContext.Uid).ToString().ToLower();
            return View();
        }
        #endregion

        protected sealed override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //不允许游客访问
            if (WorkContext.Uid < 1)
            {
                if (WorkContext.IsHttpAjax)//如果为ajax请求
                    filterContext.Result = Content("nologin");
                else//如果为普通请求
                    filterContext.Result = RedirectToAction("login", "account", new RouteValueDictionary { { "returnUrl", WorkContext.Url } });
            }
        }
    }
}
