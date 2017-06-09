using OWZX.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OWZX.Web.controllers
{
    public class BaseInfoController : BaseWebController
    {
        //
        // GET: /BaseInfo/
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CusService()
        {
            return View();
        }

    }
}
