﻿using Newtonsoft.Json;
using OWZX.Core;
using OWZX.Model;
using OWZX.Services;
using OWZX.Web.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OWZX.Web.Controllers
{
    /// <summary>
    /// 彩票
    /// </summary>
    public class NWLotteryController : BaseWebController
    {
        #region 竞猜
        public ActionResult Index()
        {
            int lotterytype = WebHelper.GetQueryInt("type");
            DataTable dt = Lottery.GetLotteryUserInfo(WorkContext.Uid, lotterytype);
            StringBuilder strb = new StringBuilder();
            foreach (DataRow rw in dt.Rows)
            {
                strb.Append(rw["ac_money"].ToString() + "|" + rw["total_bett"].ToString());
            }
            ViewData["dt_user"] = strb.ToString();
            ViewData["lottery"] = lotterytype.ToString();
            DataTable list = Lottery.LastLottery(lotterytype.ToString()).Tables[1];
            if (list.Rows.Count > 0)
            {
                ViewData["lot_msg"] = list.Rows[0]["time"].ToString() == "维护中" ? "" : list.Rows[0]["time"].ToString();
                ViewData["expect"] = list.Rows[0]["expect"].ToString();
            }
            return View();
        }

        private object lkbtlow = new object();
        private object lkbtmin = new object();
        private object lkbthigh = new object();
        NameValueCollection parmas;
        /// <summary>
        /// 投注 （添加投注记录，扣除用户金额）
        /// </summary>
        /// <returns></returns>
        public ActionResult Bett()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string msg = Lottery.ValidateBett(WorkContext.Uid.ToString(), parmas["expect"], parmas["money"], parmas["bttype"]);
                if (msg != string.Empty)
                {
                    return APIResult("error", msg);
                }


                int btmoney = int.Parse(parmas["money"]);
                //判断投注的最高注数 是否有效

                if (btmoney > 20000)
                {
                    return APIResult("error", "单笔投注金额不能大于20000元");
                }
                return DealBettLow(parmas);
            }
            catch (Exception ex)
            {
                return APIResult("error", "投注失败");
            }
        }

        private ActionResult DealBettLow(NameValueCollection parmas)
        {
            MD_Bett bet = new MD_Bett
            {
                Account = WorkContext.Uid.ToString(),
                Lotterynum = parmas["expect"],
                Money = int.Parse(parmas["money"]),
                Bttype = parmas["bttype"],
                Lotteryid = int.Parse(parmas["lotterytype"])
            };

            bool betres = Lottery.AddBett(bet);
            if (betres)
                return APIResult("success", "投注成功");
            else
                return APIResult("error", "投注失败");
        }
        /// <summary>
        /// 删除投注
        /// </summary>
        /// <returns></returns>
        public ActionResult DelBett()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                bool msg = Lottery.DeleteBett(WorkContext.Uid.ToString(), int.Parse(parmas["lotterytype"]), parmas["expect"], parmas["bttype"]);
                if (!msg)
                {
                    return APIResult("error", "撤销失败");
                }
                else
                {
                    return APIResult("success", "撤销成功");
                }
               
            }
            catch (Exception ex)
            {
                return APIResult("error", "撤销失败");
            }
        }
        /// <summary>
        /// 投注记录
        /// </summary>
        /// <returns></returns>
        public ActionResult BettRecord()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string account = parmas["account"];
                int page = int.Parse(parmas["page"]);
                StringBuilder strb = new StringBuilder();
                string type = parmas["type"];

                string start = string.Empty;
                string end = string.Empty;
                if (parmas.AllKeys.Contains("start") && parmas.AllKeys.Contains("end"))
                {
                    start = parmas["start"];
                    end = parmas["end"];

                    string[] st = start.Split('-');
                    if (st[1].Length == 1)
                        st[1] = "0" + st[1];
                    start = string.Join("-", st);

                    string[] ed = end.Split('-');
                    if (ed[1].Length == 1)
                        ed[1] = "0" + ed[1];
                    end = string.Join("-", ed);
                }


                strb.Append(" where 1=1");
                if (type != string.Empty && type != "0")
                    strb.Append("  and c.type=" + type);
                if (start != string.Empty && end != string.Empty)
                    strb.Append("  and convert(varchar(10),a.addtime,120) between '" + start + "' and '" + end + "'");

                DataTable list = NewUser.GetUserBettList(page, 15, account, strb.ToString());
                if (list.Rows.Count == 0)
                {
                    return APIResult("error", "暂无投注记录");
                }

                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //jsetting.ContractResolver = new JsonLimitOutPut(new string[] { "Expect", "Result" }, true);
                string data = JsonConvert.SerializeObject(list, jsetting).ToLower();
                return APIResult("success", data, true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }
        /// <summary>
        /// 最新10期开奖结果
        /// </summary>
        /// <returns></returns>
        public ActionResult LastBett()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string type = parmas["type"];

                List<MD_Lottery> list = Lottery.LastLotteryList(int.Parse(type));
                if (list.Count == 0)
                {
                    return APIResult("error", "暂无开奖记录");
                }
                list.ForEach((x) =>
                {
                    if (x.Status == 1 && x.Result == null)
                    {
                        x.Expect = "第" + x.Expect + "期";
                        x.Result = "?+?+?=? (类型)";
                    }
                    else
                    {
                        x.Expect = "第" + x.Expect + "期";
                        string res = "(";
                        if (int.Parse(x.Resultnum) <= 13)
                        {
                            res += "小";
                        }
                        else
                        {
                            res += "大";
                        }

                        if (int.Parse(x.Resultnum) % 2 == 0)
                        {
                            res += ",双)";
                        }
                        else
                        {
                            res += ",单)";
                        }
                        x.Result += res;
                    }
                });

                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.ContractResolver = new JsonLimitOutPut(new string[] { "Expect", "Result" }, true);
                string data = JsonConvert.SerializeObject(list, jsetting).ToLower();
                return APIResult("success", data, true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }

        /// <summary>
        /// 走势图
        /// </summary>
        /// <returns></returns>
        public ActionResult Trend()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string type = parmas["type"];
                int page = int.Parse(parmas["page"]);
                DataTable list = Lottery.LotteryTrend(page, 15, type);
                if (list.Rows.Count == 0)
                {
                    return APIResult("error", "暂无开奖记录");
                }

                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                //jsetting.ContractResolver = new JsonLimitOutPut(new string[] { "Expect", "Result" }, true);
                string data = JsonConvert.SerializeObject(list, jsetting).ToLower();
                return APIResult("success", data, true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }

        /// <summary>
        /// 最新竞猜信息
        /// </summary>
        /// <returns></returns>
        public ActionResult LastLottery()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string type = parmas["type"];
                string resjson = string.Empty;


                if (type == "10")
                {
                    //游戏是否维护中
                    BaseInfo baseinfo = BSPConfig.BaseConfig.BaseList.Find(x => x.Key == "PK10");
                    if (baseinfo.Account.Trim() == "是")
                    {
                        resjson = "{\"expect\":\"?\",\"time\":\"维护中\"}";
                    }
                    else
                    {
                        TimeSpan startTime = DateTime.Parse("09:00").TimeOfDay;
                        TimeSpan endTime = DateTime.Parse("23:55").TimeOfDay;
                        TimeSpan tmNow = DateTime.Now.TimeOfDay;

                        if (tmNow <= startTime || tmNow >= endTime)
                        {
                            //禁止投注时间
                            resjson = "{\"expect\":\"?\",\"time\":\"已停售\"}";
                        }
                    }

                }
                else if (type == "11")
                {
                    BaseInfo baseinfo = BSPConfig.BaseConfig.BaseList.Find(x => x.Key == "幸运飞艇");
                    if (baseinfo.Account.Trim() == "是")
                    {
                        resjson = "{\"expect\":\"?\",\"time\":\"维护中\"}";
                    }
                    else
                    {
                        TimeSpan startTime = DateTime.Parse("20:00").TimeOfDay;
                        TimeSpan endTime = DateTime.Parse("21:00").TimeOfDay;
                        TimeSpan tmNow = DateTime.Now.TimeOfDay;

                        if (tmNow >= startTime && tmNow <= endTime)
                        {
                            //禁止投注时间
                            resjson = "{\"expect\":\"?\",\"time\":\"已停售\"}";
                        }
                    }
                }

                if (resjson == string.Empty)
                {
                    DataTable list = Lottery.LastLottery(type).Tables[1];
                    if (list.Rows.Count == 0)
                    {
                        return APIResult("error", "暂无竞猜信息");
                    }

                    JsonSerializerSettings jsetting = new JsonSerializerSettings();
                    jsetting.ContractResolver = new JsonLimitOutPut(new string[] { "expect", "time" }, true);
                    resjson = JsonConvert.SerializeObject(list, jsetting).ToLower();

                }

                return APIResult("success", resjson.Replace("[", "").Replace("]", ""), true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }
        /// <summary>
        /// 赔率说明
        /// </summary>
        /// <returns></returns>
        public ActionResult SetRemark()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string type = parmas["type"];

                DataTable list = Lottery.SetRemark(type);
                if (list.Rows.Count == 0)
                {
                    return APIResult("error", "获取失败");
                }

                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                string resjson = JsonConvert.SerializeObject(list, jsetting).ToLower();

                return APIResult("success", resjson, true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }
        /// <summary>
        /// 关于
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            try
            {

                BaseInfo info = BSPConfig.BaseConfig.BaseList.Find(x => x.Key == "关于配置");

                StringBuilder strb = new StringBuilder();
                strb.Append("{");
                string Image = BSPConfig.ShopConfig.SiteUrl + "/upload/imgs/" + info.Image;
                strb.Append("\"version\":\"" + info.BankAddress + "\",\"url\":\"" + info.Name + "\",\"qq\":\"" + info.Account + "\",\"wechat\":\"" + info.Bank + "\",\"img\":\"" + Image + "\"");
                strb.Append("}");
                return APIResult("success", strb.ToString(), true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }
        #endregion


    }
}
