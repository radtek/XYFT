using System;
using System.Data;
using System.Collections.Generic;

using OWZX.Core;
using OWZX.Services;
using OWZX.Web.Framework;
using OWZX.Model;

namespace OWZX.Web.Models
{
    /// <summary>
    /// 账变记录
    /// </summary>
    public class ChangeListModel
    {
        public PageModel PageModel { get; set; }
        public List<MD_Change> changeList { get; set; }
    }

    /// <summary>
    ///游戏记录
    /// </summary>
    public class BettListModel
    {
        public PageModel PageModel { get; set; }
        public List<MD_Bett> BettList { get; set; }
    }
}