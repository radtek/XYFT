﻿using System;
using System.Data;

using OWZX.Core;

namespace OWZX.Services
{
    /// <summary>
    /// 后台活动专题操作管理类
    /// </summary>
    public partial class AdminTopic : Topics
    {
        /// <summary>
        /// 创建活动专题
        /// </summary>
        /// <param name="topicInfo">活动专题信息</param>
        public static void CreateTopic(TopicInfo topicInfo)
        {
            OWZX.Data.Topics.CreateTopic(topicInfo);
        }

        /// <summary>
        /// 更新活动专题
        /// </summary>
        /// <param name="topicInfo">活动专题信息</param>
        public static void UpdateTopic(TopicInfo topicInfo)
        {
            OWZX.Data.Topics.UpdateTopic(topicInfo);
            OWZX.Core.BSPCache.Remove(CacheKeys.SHOP_TOPIC_INFO + topicInfo.TopicId);
            OWZX.Core.BSPCache.Remove(CacheKeys.SHOP_TOPIC_INFO + topicInfo.SN);
        }

        /// <summary>
        /// 删除活动专题
        /// </summary>
        /// <param name="topicId">活动专题id</param>
        public static void DeleteTopicById(int topicId)
        {
            TopicInfo topicInfo = AdminGetTopicById(topicId);
            if (topicInfo != null)
            {
                OWZX.Data.Topics.DeleteTopicById(topicId);
                OWZX.Core.BSPCache.Remove(CacheKeys.SHOP_TOPIC_INFO + topicInfo.TopicId);
                OWZX.Core.BSPCache.Remove(CacheKeys.SHOP_TOPIC_INFO + topicInfo.SN);
            }
        }

        /// <summary>
        /// 后台获得活动专题
        /// </summary>
        /// <param name="topicId">活动专题id</param>
        /// <returns></returns>
        public static TopicInfo AdminGetTopicById(int topicId)
        {
            if (topicId > 0)
                return OWZX.Data.Topics.AdminGetTopicById(topicId);
            return null;
        }

        /// <summary>
        /// 后台获得活动专题列表条件
        /// </summary>
        /// <param name="sn">编号</param>
        /// <param name="title">标题</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static string AdminGetTopicListCondition(string sn, string title, string startTime, string endTime)
        {
            return OWZX.Data.Topics.AdminGetTopicListCondition(sn, title, startTime, endTime);
        }

        /// <summary>
        /// 后台获得活动专题列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static DataTable AdminGetTopicList(int pageSize, int pageNumber, string condition)
        {
            return OWZX.Data.Topics.AdminGetTopicList(pageSize, pageNumber, condition);
        }

        /// <summary>
        /// 后台获得活动专题数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetTopicCount(string condition)
        {
            return OWZX.Data.Topics.AdminGetTopicCount(condition);
        }
    }
}
