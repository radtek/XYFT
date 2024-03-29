﻿using System;
using System.Collections.Generic;

using OWZX.Core;
using OWZX.Services;
using OWZX.Web.Framework;

namespace OWZX.Web.Models
{
    /// <summary>
    /// 问题模型类
    /// </summary>
    public class QuestionModel
    {
        public HelpInfo HelpInfo { get; set; }
        public List<HelpInfo> HelpList { get; set; }
    }
}