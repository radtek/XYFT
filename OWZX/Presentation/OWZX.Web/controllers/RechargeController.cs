using OWZX.Model;
using OWZX.Services;
using OWZX.Web.Framework;
using OWZX.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}
