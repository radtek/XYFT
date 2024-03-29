﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWZX.Model
{
    /// <summary>
    /// 投注表
    /// </summary>
    public class MD_Bett
    {
        public Int64 id { get; set; }

        private int bettid;
        public int Bettid
        {
            get { return bettid; }
            set { bettid = value; }
        }

        private string account;
        /// <summary>
        /// 用户账号(手机号)
        /// </summary>
        public string Account
        {
            get { return account; }
            set { account = value.TrimEnd(); }
        }
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }
        private int uid;
        public int Uid
        {
            get { return uid; }
            set { uid = value; }
        }

        private int lotteryid;
        /// <summary>
        ///彩票类型id 
        /// </summary>
        public int Lotteryid
        {
            get { return lotteryid; }
            set { lotteryid = value; }
        }

        private string lottery;
        /// <summary>
        ///彩票类型
        /// </summary>
        public string Lottery
        {
            get { return lottery; }
            set { lottery = value; }
        }


        private int roomid;
        /// <summary>
        ///投注名次（如：跑第一名 1 冠亚和为0）
        /// </summary>
        public int Roomid
        {
            get { return roomid; }
            set { roomid = value; }
        }
        private string room;
        /// <summary>
        ///房间类型 
        /// </summary>
        public string Room
        {
            get { return room; }
            set { room = value; }
        }
        private int vipid;
        /// <summary>
        ///vip类型id
        /// </summary>
        public int Vipid
        {
            get { return vipid; }
            set { vipid = value; }
        }
        private string vip;
        /// <summary>
        /// vip类型
        /// </summary>
        public string Vip
        {
            get { return vip; }
            set { vip = value; }
        }
        private int bttypeid;
        /// <summary>
        ///投注类型id (彩票配置表中的id)
        /// </summary>
        public int Bttypeid
        {
            get { return bttypeid; }
            set { bttypeid = value; }
        }
        private string bttype;
        /// <summary>
        ///投注类型 （彩票配置表中对应的类型 如：猜数字 等）
        /// </summary>
        public string Bttype
        {
            get { return bttype; }
            set { bttype = value; }
        }
        private string lotterynum;
        /// <summary>
        ///投注期号 
        /// </summary>
        public string Lotterynum
        {
            get { return lotterynum; }
            set { lotterynum = value; }
        }

        private int money;
        /// <summary>
        /// 投注金额
        /// </summary>
        public int Money
        {
            get { return money; }
            set { money = value; }
        }
        /// <summary>
        /// 是否已经结算
        /// </summary>
        public bool IsRead { get; set; }

        public decimal luckresult;
        /// <summary>
        /// 中奖金额
        /// </summary>
        public decimal LuckResult {
            get { return luckresult; }
            set { luckresult = value; }
        }
        private string item;
        /// <summary>
        /// 投注项
        /// </summary>
        public string Item
        {
            get { return item; }
            set { item = value; }
        }
        /// <summary>
        /// 开奖号码
        /// </summary>
        public string OpenCode { get; set; }

        private DateTime addtime;
        public DateTime Addtime
        {
            get { return addtime; }
            set { addtime = value; }
        }

        public int TotalCount { get; set; }
    }
}
