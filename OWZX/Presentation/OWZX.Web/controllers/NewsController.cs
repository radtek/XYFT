﻿using System;
using System.Web.Mvc;

using OWZX.Core;
using OWZX.Services;
using OWZX.Web.Framework;
using OWZX.Web.Models;

namespace OWZX.Web.Controllers
{
    /// <summary>
    /// 新闻控制器类
    /// </summary>
    public partial class NewsController : BaseWebController
    {
        /// <summary>
        /// 详细信息
        /// </summary>
        public ActionResult Details()
        {
            //新闻id
            int newsId = GetRouteInt("newsId");
            if (newsId == 0)
                newsId = WebHelper.GetQueryInt("newsId");

            NewsInfo newsInfo = News.GetNewsById(newsId);
            if (newsInfo == null)
                return PromptView("/", "你访问的页面不存在");

            NewsModel model = new NewsModel();
            model.NewsInfo = newsInfo;
            model.NewsTypeList = News.GetNewsTypeList();

            return View(model);
        }

        /// <summary>
        /// 新闻列表
        /// </summary>
        public ActionResult List()
        {
            string newsTitle = WebHelper.GetQueryString("newsTitle");
            int newsTypeId = WebHelper.GetQueryInt("newsTypeId");
            int page = WebHelper.GetQueryInt("page");

            string condition = News.GetNewsListCondition(newsTypeId, newsTitle);
            PageModel pageModel = new PageModel(10, page, News.GetNewsCount(condition));
            NewsListModel model = new NewsListModel()
            {
                PageModel = pageModel,
                NewsList = News.GetNewsList(pageModel.PageSize, pageModel.PageNumber, condition),
                NewsTitle = newsTitle,
                NewsTypeId = newsTypeId,
                NewsTypeList = News.GetNewsTypeList()
            };

            return View(model);
        }
    }
}
