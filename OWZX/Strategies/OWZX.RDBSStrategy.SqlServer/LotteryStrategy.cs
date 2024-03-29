﻿using OWZX.Core;
using recharge = OWZX.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWZX.Model;

namespace OWZX.RDBSStrategy.SqlServer
{
    /// <summary>
    /// SqlServer策略之彩票部分类
    /// </summary>
    public partial class RDBSStrategy : IRDBSStrategy
    {
        private object lkaddlot = new object();
        private object lkstate = new object();
        #region 彩票
        /// <summary>
        /// 获取最新彩票记录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataSet LastLotteryForAPI(string type)
        {
            lock (lkstate)
            {
                string sql = string.Format(@" 
                --逻辑：北京 投注4:30s 封盘30s ,到开奖时间加载新的一期；
                --加拿大 投注3分，封盘30s,到开奖时间加载新的一期；
                declare @type int ={0}
                declare @min varchar(5), @sec varchar(5),@expect varchar(50),@totalsec varchar(5)

                select top 1 opencode from owzx_lotteryrecord where type=10 and status=2 order by lotteryid desc

                if(@type=10) 
                begin

                if exists(select top 1 1 from owzx_lotteryrecord where type=10 and 
                (DATEDIFF(second,opentime,getdate()) >= -300 and DATEDIFF(second,opentime,getdate())<=0) order by lotteryid )
                begin
                select top 1 @expect=expect, 
                @totalsec= CONVERT(VARCHAR(10),DATEDIFF(SECOND,getdate(),opentime))
                from owzx_lotteryrecord where type=10  and (DATEDIFF(second,opentime,getdate()) >= -300 and DATEDIFF(second,opentime,getdate())<=0)
                order by lotteryid

                select @expect expect,@totalsec time
               
                end
                else
                begin
                select '?' expect,'维护中' time
                end

                end
                else if(@type=11) 
                begin

                if exists(select top 1 1 from owzx_lotteryrecord where type=11 and 
                (DATEDIFF(second,opentime,getdate()) >= -300 and DATEDIFF(second,opentime,getdate())<=0) order by lotteryid )
                begin
                select top 1 @expect=expect, 
                @totalsec= CONVERT(VARCHAR(10),DATEDIFF(SECOND,getdate(),opentime))
                from owzx_lotteryrecord where type=11 and (DATEDIFF(second,opentime,getdate()) >= -300 and DATEDIFF(second,opentime,getdate())<=0)
                order by lotteryid

                select @expect expect,@totalsec time
                end
                else
                begin
                select '?' expect,'维护中' time
                end

                end
                ", type);
                return RDBSHelper.ExecuteDataset(sql);
            }
        }

        /// <summary>
        /// 获取最新彩票记录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataSet LastLottery(string type)
        {
            lock (lkstate)
            {
                string sql = string.Format(@" 

                declare @type int ={0}
                declare @min varchar(5), @sec varchar(5),@expect varchar(50),@totalsec varchar(5)

                select top 1 opencode from owzx_lotteryrecord where type=10 and status=2 order by lotteryid desc

                 if(@type=10) 
                begin

                if exists(select top 1 1 from owzx_lotteryrecord where type=10 and 
                (DATEDIFF(second,opentime,getdate()) >= -300 and DATEDIFF(second,opentime,getdate())<=0) order by lotteryid )
                begin
                select top 1 @expect=expect, 
                @totalsec= CONVERT(VARCHAR(10),DATEDIFF(SECOND,getdate(),opentime))
                from owzx_lotteryrecord where type=10  and (DATEDIFF(second,opentime,getdate()) >= -300 and DATEDIFF(second,opentime,getdate())<=0)
                order by lotteryid

                select @expect expect,@totalsec time

                end
                else if exists(select 1 from  owzx_lotteryrecord where type=10 and  status=0 
                and convert(varchar(10),opentime,120)=convert(varchar(10),getdate(),120))
                begin
                select top 1 @expect=expect,@totalsec =CONVERT(VARCHAR(10),DATEDIFF(SECOND,getdate(),opentime))
                from  owzx_lotteryrecord where type=10 and  status=0 
                and convert(varchar(10),opentime,120)=convert(varchar(10),getdate(),120) and datediff(second,getdate(),opentime)>0
                select isnull(@expect,'') expect,isnull(@totalsec,'') time
                end
                else
                begin
                select '?' expect,'维护中' time
                end

                end
                else if(@type=11) 
                begin

                if exists(select top 1 1 from owzx_lotteryrecord where type=11 and 
                (DATEDIFF(second,opentime,getdate()) >= -300 and DATEDIFF(second,opentime,getdate())<=0) order by lotteryid )
                begin
                select top 1 @expect=expect, 
                @totalsec= CONVERT(VARCHAR(10),DATEDIFF(SECOND,getdate(),opentime))
                from owzx_lotteryrecord where type=11 and (DATEDIFF(second,opentime,getdate()) >= -300 and DATEDIFF(second,opentime,getdate())<=0)
                order by lotteryid

                select @expect expect,@totalsec time
                end
                else if exists(select 1 from  owzx_lotteryrecord where type=11 and  status=0 
                and convert(varchar(10),opentime,120)=convert(varchar(10),getdate(),120))
                begin
                select top 1 @expect=expect,@totalsec =CONVERT(VARCHAR(10),DATEDIFF(SECOND,getdate(),opentime))
                from  owzx_lotteryrecord where type=11 and  status=0 
                and convert(varchar(10),opentime,120)=convert(varchar(10),getdate(),120) and datediff(second,getdate(),opentime)>0
                select isnull(@expect,'') expect,isnull(@totalsec,'') time
                end
                else
                begin
                select '?' expect,'维护中' time
                end

                end
                ", type);
                return RDBSHelper.ExecuteDataset(sql);
            }
        }

        /// <summary>
        /// 添加新的竞猜记录
        /// </summary>
        /// <param name="lottery"></param>
        /// <returns></returns>
        public bool AddBJRecord(MD_Lottery lottery)
        {
            try
            {
                string sql = string.Format(@"
begin try
declare @count int,@diff int,@index int=1
declare @lastfcnum varchar(20)='',@lotterTime varchar(30)

select @count=count(1) from owzx_lotteryrecord a 
where a.status!=2 and LEFT(a.opentime,10)=CONVERT(varchar(10),getdate(),120)

set @diff=5-@count

select top 1 @lastfcnum=expect,@lotterTime=convert(varchar(25),cast(opentime as datetime),120) 
from owzx_lotteryrecord
where status=0 and LEFT(opentime,10)=CONVERT(varchar(10),getdate(),120) 
order by lotteryid desc

if(@lastfcnum='')
begin
select @lastfcnum='{1}',@lotterTime='{2}'
end


while(@index<=@diff)
begin

if(CONVERT(varchar(16),cast(@lotterTime as datetime),120)=(CONVERT(varchar(11),getdate(),120)+'23:55'))
begin
set @lotterTime=convert(varchar(11),DATEADD(day,1,getdate()),120)+'09:05'
end
else
begin
set @lotterTime=convert(varchar(25),dateadd(Minute,5,convert(varchar(25),cast(@lotterTime as datetime),120)),120)
end

if(@lotterTime>=convert(varchar(11),getdate(),120)+'09:00' 
and @lotterTime <=convert(varchar(11),getdate(),120)+'23:57') or (@lotterTime>=convert(varchar(11),dateadd(day,1,getdate()),120)+'09:00'
and @lotterTime <=convert(varchar(11),dateadd(day,1,getdate()),120)+'23:57')
begin

set @lastfcnum=cast(@lastfcnum as int)+1

set @lotterTime=left(@lotterTime,16) 

if not exists(select 1 from owzx_lotteryrecord where expect=@lastfcnum)
begin
insert into owzx_lotteryrecord(type,expect,opentime,status)
values({0},@lastfcnum,@lotterTime,0)

if exists(select 1 from owzx_lotteryrecord where expect=@lastfcnum)
  select '添加成功'

end
else
begin
select '福彩期号已经存在,请检查'
end

set @index=@index+1

end
else
begin
select '添加完成'
break
end

end

select '添加完成'

end try
begin catch
select ERROR_MESSAGE()
end catch
", lottery.Type, lottery.Expect, lottery.Opentime);

                string res = RDBSHelper.ExecuteScalar(CommandType.Text, sql).ToString();

                if (res.Contains("成功") || res.Contains("完成"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 添加新的竞猜记录
        /// </summary>
        /// <param name="lottery"></param>
        /// <returns></returns>
        public bool AddCanadaRecord(MD_Lottery lottery)
        {
            try
            {
                string sql = string.Format(@"
begin try
declare @count int,@diff int,@index int=1
declare @lastfcnum varchar(20)='',@lotterTime varchar(30)

select @count=count(1) from owzx_lotteryrecord a 
where a.status!=2 and LEFT(a.opentime,10)=CONVERT(varchar(10),getdate(),120)

set @diff=5-@count

select top 1 @lastfcnum=expect,@lotterTime=convert(varchar(25),cast(opentime as datetime),120) 
from owzx_lotteryrecord
where status=0 and LEFT(opentime,10)=CONVERT(varchar(10),getdate(),120) 
order by lotteryid desc

if(@lastfcnum='')
begin
select @lastfcnum='{1}',@lotterTime='{2}'
end


while(@index<=@diff)
begin

if(CONVERT(varchar(16),cast(@lotterTime as datetime),120) >= (CONVERT(varchar(11),getdate(),120)+'20:00') 
and CONVERT(varchar(16),cast(@lotterTime as datetime),120)<=(CONVERT(varchar(11),getdate(),120)+'21:00'))
begin
set @lotterTime=convert(varchar(11),DATEADD(day,1,getdate()),120)+'21:03:30'
end
else
begin
set @lotterTime=convert(varchar(25),dateadd(Second,30,dateadd(Minute,3,convert(varchar(25),cast(@lotterTime as datetime),120))),120)
end

if(@lotterTime>=convert(varchar(11),getdate(),120)+'21:00' 
or @lotterTime <=convert(varchar(11),getdate(),120)+'20:00') or (@lotterTime>=convert(varchar(11),dateadd(day,1,getdate()),120)+'21:00'
or @lotterTime <=convert(varchar(11),dateadd(day,1,getdate()),120)+'20:00')
begin

set @lastfcnum=cast(@lastfcnum as int)+1

--set @lotterTime=left(@lotterTime,16) 

if not exists(select 1 from owzx_lotteryrecord where expect=@lastfcnum)
begin
insert into owzx_lotteryrecord(type,expect,opentime,status)
values({0},@lastfcnum,@lotterTime,0)

if exists(select 1 from owzx_lotteryrecord where expect=@lastfcnum)
  select '添加成功'

end
else
begin
select '福彩期号已经存在,请检查'
end

set @index=@index+1

end
else
begin
select '添加完成'
break
end

end

select '添加完成'

end try
begin catch
select ERROR_MESSAGE()
end catch
", lottery.Type, lottery.Expect, lottery.Opentime);

                string res = RDBSHelper.ExecuteScalar(CommandType.Text, sql).ToString();

                if (res.Contains("成功") || res.Contains("完成"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 添加彩票记录 (记录彩票开奖时间和期号)
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        public string AddLottery(MD_Lottery lot)
        {
            DbParameter[] parms = {
                                       
                                        GenerateInParam("@type", SqlDbType.Int, 4, lot.Type),
                                        GenerateInParam("@expect", SqlDbType.VarChar,100, lot.Expect),
                                        GenerateInParam("@opentime", SqlDbType.DateTime, 25, lot.Opentime),
                                        GenerateInParam("@status", SqlDbType.SmallInt, 4, lot.Status)
                                       
                                    };
            string commandText = string.Format(@"
begin try

INSERT INTO owzx_lotteryrecord(type,expect,opentime,status)
VALUES (@type,@expect,@opentime,@status)

select '添加成功' state
end try
begin catch
select ERROR_MESSAGE() state
end catch
");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }


        /// <summary>
        /// 添加彩票记录 (上期结束，添加新的记录)
        /// </summary>
        /// <param name="type">北京10 加拿大11</param>
        /// <param name="starttime">彩票开始时间</param>
        /// <param name="endtime">彩票截止时间</param>
        /// <returns></returns>
        public string AddLottery(int type, string starttime, string endtime)
        {
            //lock (lkaddlot)
            //{
                string commandText = string.Format(@"
begin try
declare @lasttime varchar(25)
select @lasttime=(
select top 1 (case {0} when 10 then convert(varchar(25),DATEADD(Minute,5,opentime),120) 
when 11 then convert(varchar(25),DATEADD(Second,30,DATEADD(Minute,3,opentime)),120)  end) from owzx_lotteryrecord where type={0}  order by lotteryid desc
)

if({0}=10)
begin
set @lasttime=DATEADD(Minute,5,@lasttime)
end
else if({0}=11)
begin
set @lasttime=DATEADD(Second,30,DATEADD(Minute,3,@lasttime))
end


if(@lasttime>='{1}' and @lasttime<='{2}')
begin
--select '彩票已停止投注' state

if({0}=10)
begin
INSERT INTO owzx_lotteryrecord(type,expect,opentime,status)
select a.type,cast(a.expect as int)+1,convert(varchar(25),DATEADD(day,1,(CAST(convert(varchar(10),getdate(),120)+' 09:05:00' as datetime))),120),0 
from owzx_lotteryrecord a
join (select top 1 * from owzx_lotteryrecord where type={0}  order by lotteryid desc ) b on a.lotteryid=b.lotteryid
end
else if({0}=11)
begin
INSERT INTO owzx_lotteryrecord(type,expect,opentime,status)
select a.type,cast(a.expect as int)+1,convert(varchar(25),CAST(convert(varchar(10),getdate(),120)+' 21:03:30' as datetime),120),0 
from owzx_lotteryrecord a
join (select top 1 * from owzx_lotteryrecord where type={0}  order by lotteryid desc ) b on a.lotteryid=b.lotteryid
end

select '添加成功' state



return
end

if({0}=10)
begin
INSERT INTO owzx_lotteryrecord(type,expect,opentime,status)
select a.type,cast(a.expect as int)+1,DATEADD(Minute,5,a.opentime),0 
from owzx_lotteryrecord a
join (select top 1 * from owzx_lotteryrecord where type={0}  order by lotteryid desc ) b on a.lotteryid=b.lotteryid
end
else if({0}=11)
begin
INSERT INTO owzx_lotteryrecord(type,expect,opentime,status)
select a.type,cast(a.expect as int)+1,DATEADD(Second,30,DATEADD(Minute,3,a.opentime)),0 
from owzx_lotteryrecord a
join (select top 1 * from owzx_lotteryrecord where type={0}  order by lotteryid desc ) b on a.lotteryid=b.lotteryid
end

select '添加成功' state
end try
begin catch
select ERROR_MESSAGE() state
end catch
", type, starttime, endtime);
                return RDBSHelper.ExecuteScalar(commandText).ToString();
            //}
        }
        /// <summary>
        /// 更新彩票记录 (更新开奖信息)
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        public string UpdateLottery(MD_Lottery lot)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@type", SqlDbType.Int, 4, lot.Type),
                                    GenerateInParam("@expect", SqlDbType.VarChar,100, lot.Expect),
                                    GenerateInParam("@status", SqlDbType.SmallInt, 4, lot.Status),
                                    GenerateInParam("@opencode", SqlDbType.VarChar, 100, lot.Opencode),
                                    GenerateInParam("@orderresult", SqlDbType.VarChar,100, lot.Orderresult),
                                    GenerateInParam("@first", SqlDbType.VarChar,30, lot.First),
                                    GenerateInParam("@second", SqlDbType.VarChar,30, lot.Second),
                                    GenerateInParam("@three", SqlDbType.VarChar,30, lot.Three),
                                    GenerateInParam("@result", SqlDbType.VarChar, 10, lot.Result),
                                    GenerateInParam("@resultnum", SqlDbType.VarChar, 5, lot.Resultnum),
                                    GenerateInParam("@resulttype", SqlDbType.VarChar, 20, lot.ResultType)
                                    
                                       
                                    };
            string commandText = string.Format(@"
begin try
if exists(select 1 from owzx_lotteryrecord where type=@type and expect=@expect)
begin
update a set   
a.opencode = @opencode
,a.orderresult = @orderresult
,a.first = @first
,a.second = @second
,a.three = @three
,a.result = @result
,a.resultnum = @resultnum
,a.status = @status,a.resulttype=@resulttype,a.updatetime=convert(varchar(25),getdate(),120)
from owzx_lotteryrecord a where a.type=@type and a.expect=@expect
end

select '更新成功' state
end try
begin catch
select ERROR_MESSAGE() state
end catch
");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }

        /// <summary>
        /// 更新竞猜记录为 封盘状态
        /// </summary>
        /// <returns></returns>
        public string UpdateLotteryStatus()
        {
            string commandText = string.Format(@"
begin try

if exists(select 1 from owzx_lotteryrecord where type=10 and (DATEDIFF(second,getdate(),opentime) between 0 and 30))
begin
update a set   
a.status = 1
from owzx_lotteryrecord a where a.type=10 and (DATEDIFF(second,getdate(),a.opentime) between 0 and 30)
end

if exists(select 1 from owzx_lotteryrecord where type=11 and (DATEDIFF(second,getdate(),opentime) between 0 and 30))
begin
update a set   
a.status = 1
from owzx_lotteryrecord a where a.type=11 and (DATEDIFF(second,getdate(),a.opentime) between 0 and 30)
end


select '更新成功' state
end try
begin catch
select ERROR_MESSAGE() state
end catch
");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, null).ToString();
        }


        /// <summary>
        /// 删除彩票记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteLottery(string id)
        {
            string commandText = string.Format(@"
            begin try
            if exists(select 1 from owzx_lotteryrecord where lotteryid in ({0}))
            begin
            delete from owzx_lotteryrecord where lotteryid in ({0})
            select '删除成功' state
            end
            else
            begin
            select '记录已被删除' state
            end
            end try
            begin catch
            select ERROR_MESSAGE() state
            end catch
            ",
             id);
            return RDBSHelper.ExecuteScalar(commandText).ToString();
        }


        /// <summary>
        ///获取彩票记录(分页)
        /// </summary>
        /// <param name="pageNumber">页索引</param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">没有where</param>
        /// <returns></returns>
        public DataTable GetLotteryList(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
  drop table #list

SELECT ROW_NUMBER() over(order by a.lotteryid desc) id
      ,a.lotteryid
      ,a.type
      ,a.expect
      ,a.opencode
      ,a.opentime
      ,a.orderresult
      ,a.first
      ,a.second
      ,a.three
      ,a.result
      ,a.resultnum,a.resulttype
      ,a.status
      ,a.addtime    
  into  #list
  FROM owzx_lotteryrecord a
  {0}

declare @total int
select @total=(select count(1)  from #list)

if(@pagesize=-1)
begin
select *,@total TotalCount from #list
end
else
begin
select *,@total TotalCount from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteTable(CommandType.Text, commandText, parms)[0];
        }

        /// <summary>
        ///获取彩票记录(分页)
        /// </summary>
        /// <param name="type">彩票类型</param>
        /// <returns></returns>
        public DataTable LastLotteryList(int type)
        {

            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
  drop table #list


SELECT a.lotteryid,a.status
,a.type ,a.expect,a.opencode ,a.opentime,a.orderresult
,a.first,a.second ,a.three ,a.result ,a.resultnum,a.resulttype ,a.addtime    
into  #list
FROM owzx_lotteryrecord a
where a.type={0} and a.status=1 and datediff(second,opentime,getdate()) between 0 and 30
union all
SELECT top 10 a.lotteryid,a.status
,a.type ,a.expect,a.opencode ,a.opentime,a.orderresult
,a.first,a.second ,a.three ,a.result ,a.resultnum,a.resulttype ,a.addtime  FROM owzx_lotteryrecord a
where a.type={0} and a.status=2
order by a.lotteryid desc

select * from #list order by lotteryid desc 

end try
begin catch
select ERROR_MESSAGE() state
end catch

", type);

            return RDBSHelper.ExecuteTable(CommandType.Text, commandText, null)[0];
        }

        /// <summary>
        ///获取彩票记录(分页)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">有where 条件需要and</param>
        /// <returns></returns>
        public DataTable GetBJ28LotteryList(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
  drop table #list

SELECT ROW_NUMBER() over(order by a.lotteryid desc) id
      ,a.lotteryid
      ,a.type
      ,a.expect
      ,a.opencode
      ,a.opentime
      ,a.orderresult
      ,a.first
      ,a.second
      ,a.three
      ,a.result
      ,a.resultnum,a.resulttype
      ,a.status
      ,a.addtime    
  into  #list
  FROM owzx_lotteryrecord a where a.type=10
  {0}

declare @total int
select @total=(select count(1)  from #list)

if(@pagesize=-1)
begin
select *,@total TotalCount from #list
end
else
begin
select *,@total TotalCount from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteTable(CommandType.Text, commandText, parms)[0];
        }


        /// <summary>
        ///获取彩票记录(分页)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">有where 条件需要and</param>
        /// <returns></returns>
        public DataTable GetCanada28LotteryList(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
  drop table #list

SELECT ROW_NUMBER() over(order by a.lotteryid desc) id
      ,a.lotteryid
      ,a.type
      ,a.expect
      ,a.opencode
      ,a.opentime
      ,a.orderresult
      ,a.first
      ,a.second
      ,a.three
      ,a.result
      ,a.resultnum,a.resulttype
      ,a.status
      ,a.addtime    
  into  #list
  FROM owzx_lotteryrecord a where a.type=11
  {0}

declare @total int
select @total=(select count(1)  from #list)

if(@pagesize=-1)
begin
select *,@total TotalCount from #list
end
else
begin
select *,@total TotalCount from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteTable(CommandType.Text, commandText, parms)[0];
        }
        /// <summary>
        /// 获取彩票走势图
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="type">彩票类型id</param>
        /// <returns></returns>
        public DataTable LotteryTrend(int pageNumber, int pageSize, string type)
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };
            string sql = string.Format(@"
if OBJECT_ID('tempdb..#list') is not null
drop table #list

if OBJECT_ID('tempdb..#listpage') is not null
drop table #listpage

select a.expect,a.resultnum,b.item,b.item type
into #list
from owzx_lotteryrecord a
join (select * from owzx_lotteryset where roomtype=20) b on b.item in ('大','小','单','双','大单','小单','大双','小双') 
 and CHARINDEX(','+a.resultnum+',',','+b.nums+',')>0
where a.type={0} and a.status=2

SELECT ROW_NUMBER() over(order by expect desc) id, expect,resultnum,isnull([大],'') [大],isnull([小],'')[小],isnull([单],'')[单],
isnull([双],'')[双],isnull([大单],'')[大单],isnull([小单],'')[小单],isnull([大双],'')[大双],isnull([小双],'')[小双] 
into #listpage
FROM #list pivot( 
max(type) FOR item in ([大],[小],[单],[双],[大单],[小单],[大双],[小双])
 )a
order by expect desc

select id, expect,resultnum,[大] big,[小] small,[单] single,[双] dble,[大单] lsingle,[小单] ssingle,[大双] ldble,[小双] sdble  from #listpage where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
", type);
            return RDBSHelper.ExecuteTable(sql, parms)[0];
        }

        public DataTable GetLotteryResult(int pageNumber, int pageSize, string type)
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };
            string sql = string.Format(@"
if OBJECT_ID('tempdb..#list') is not null
drop table #list

if OBJECT_ID('tempdb..#listpage') is not null
drop table #listpage

select ROW_NUMBER() over(order by expect desc) id, a.expect,a.orderresult,a.type
into #list 
from owzx_lotteryrecord a 
where a.type={0} and a.status=2
order by expect desc
 

select *  from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
", type);
            return RDBSHelper.ExecuteTable(sql, parms)[0];
        }

        /// <summary>
        ///是否存在北京28彩票记录
        /// </summary>
        /// <param name="condition">没有where</param>
        /// <returns></returns>
        public bool ExistsLottery(string condition = "")
        {
            string commandText = string.Format(@"
SELECT count(1)   
  FROM owzx_lotteryrecord a 
  {0}
", condition);
            return RDBSHelper.Exists(commandText);
        }
        /// <summary>
        ///是否存在北京28彩票记录
        /// </summary>
        /// <param name="condition">有where 条件需要and</param>
        /// <returns></returns>
        public bool ExistsBJ28(string condition = "")
        {

            string commandText = string.Format(@"
SELECT count(1)   
  FROM owzx_lotteryrecord a where a.type=10
  {0}

", condition);

            return RDBSHelper.Exists(commandText);
        }


        /// <summary>
        ///获取彩票记录(分页)
        /// </summary>
        /// <param name="condition">有where 条件需要and</param>
        /// <returns></returns>
        public bool ExistsCanada28(string condition = "")
        {
            string commandText = string.Format(@"

SELECT  count(1)    
  FROM owzx_lotteryrecord a where a.type=11
  {0}
", condition);

            return RDBSHelper.Exists(commandText);
        }
        /// <summary>
        /// bj28最新竞猜记录是否已经超时
        /// </summary>
        /// <returns></returns>
        public bool ExistsBjTimeOut()
        {
            string sql = string.Format(@"
if object_id('tempdb..#list') is not null
drop table #list
select top 1 * 
into #list
from owzx_lotteryrecord a where a.type=10 order by lotteryid desc

select count(1) from #list where DATEDIFF(SECOND,opentime,getdate())>=300
             ");
            return RDBSHelper.Exists(sql);
        }
        /// <summary>
        /// canada28最新竞猜记录是否已经超时
        /// </summary>
        /// <returns></returns>
        public bool ExistsCanTimeOut()
        {
            string sql = string.Format(@"
if object_id('tempdb..#list') is not null
drop table #list
select top 1 * 
into #list
from owzx_lotteryrecord a where a.type=11 order by lotteryid desc

select count(1) from #list where DATEDIFF(SECOND,opentime,getdate())>=210
             ");
            return RDBSHelper.Exists(sql);
        }
        /// <summary>
        /// 验证投注信息是否正确
        /// </summary>
        
        /// <returns></returns>
        public string ValidateBett(string uid, string expect, string money, string bttype)
        {
            string sql = string.Format(@"declare @uid int={0},@expect varchar(100)='{1}',@money decimal(18,2)={2},
 @bttype varchar(10)='{3}'
  
  if  not exists(select 1 from owzx_lotteryset where rtrim(item)=@bttype )
  or not exists(select 1 from owzx_users where uid=@uid )
  begin
  select '投注信息异常' msg
  end
  --else if exists(select 1 from owzx_bett a join owzx_users b on a.uid=b.uid and  uid=@uid and a.lotterynum=@expect)
  --begin
  --select '已经投注,不能重复投注' msg
  --end
  else if not exists(select 1 from owzx_lotteryrecord where expect=@expect)
  begin
  select '竞猜信息不存在' msg
  end
  else if exists(select 1 from owzx_lotteryrecord where expect=@expect and status=1)
  begin
  select '已封盘,禁止投注' msg
  end
  else if exists(select 1 from owzx_lotteryrecord where expect=@expect and (status=2 or convert(varchar(19),opentime,120)<=convert(varchar(19),getdate(),120)))
  begin
  select '竞猜已结束,禁止投注' msg
  end
  else if exists(select 1 from owzx_users where uid=@uid  and isnull(totalmoney,0)<@money)
  begin
  select '账户余额不足,请充值' msg
  end
  else
  begin
  select '' msg
  end", uid, expect, money, bttype);

            return RDBSHelper.ExecuteScalar(sql).ToString();
        }
        /// <summary>
        /// 验证投注信息是否正确
        /// </summary>
        /// <param name="account"></param>
        /// <param name="expect"></param>
        /// <param name="money"></param>
        /// <param name="room"></param>
        /// <param name="vip"></param>
        /// <param name="bttypeid"></param>
        /// <returns></returns>
        public string ValidateBettForRoom(string account,string expect,string money,string room,string vip,int bttypeid)
        {
            string sql = string.Format(@"declare @account varchar(15)='{0}',@expect varchar(100)='{1}',@money decimal(18,2)={2},
  @room varchar(10)='{3}',@vip varchar(10)='{4}',@bttypeid int={5}
  
  if not exists(select 1 from owzx_sys_basetype where RTRIM(type) like '%'+@room+'%' )
  or not exists(select 1 from owzx_sys_basetype where lower(RTRIM(type))=lower(@vip) )
  or not exists(select 1 from owzx_lotteryset where bttypeid=@bttypeid )
  or not exists(select 1 from owzx_users where rtrim(mobile)=@account )
  begin
  select '投注信息异常' msg
  end
  --else if exists(select 1 from owzx_bett a join owzx_users b on a.uid=b.uid and rtrim(b.mobile)=@account and a.lotterynum=@expect)
  --begin
  --select '已经投注,不能重复投注' msg
  --end
  else if not exists(select 1 from owzx_lotteryrecord where expect=@expect)
  begin
  select '竞猜信息不存在' msg
  end
  else if exists(select 1 from owzx_lotteryrecord where expect=@expect and status=1)
  begin
  select '已封盘,禁止投注' msg
  end
  else if exists(select 1 from owzx_lotteryrecord where expect=@expect and (status=2 or convert(varchar(19),opentime,120)<=convert(varchar(19),getdate(),120)))
  begin
  select '竞猜已结束,禁止投注' msg
  end
  else if exists(select 1 from owzx_users where rtrim(mobile)=@account and isnull(totalmoney,0)<@money)
  begin
  select '账户余额不足,请充值' msg
  end
  else
  begin
  select '' msg
  end", account,expect,money,room,vip,bttypeid);

            return RDBSHelper.ExecuteScalar(sql).ToString();
        }
        #endregion

        #region 投注
        /// <summary>
        /// 添加投注记录
        /// </summary>
        /// <param name="bett"></param>
        /// <returns></returns>
        public string AddBett(MD_Bett bett)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@account", SqlDbType.VarChar, 20, bett.Account),
                                    GenerateInParam("@lotteryid", SqlDbType.Int, 4, bett.Lotteryid),
                                    GenerateInParam("@lotterynum", SqlDbType.VarChar, 50, bett.Lotterynum),
                                    GenerateInParam("@money", SqlDbType.Int, 4, bett.Money),
                                     GenerateInParam("@bttype", SqlDbType.VarChar, 10, bett.Bttype),
                                     GenerateInParam("@roomid", SqlDbType.Int, 4, bett.Roomid)
                                    };
            string commandText = string.Format(@"
begin try
begin tran t1

INSERT INTO owzx_bett([uid],[lotteryid],roomid,[bttypeid],[lotterynum],[money],isread)
    select rtrim(@account),@lotteryid,@roomid,
   (select bttypeid from owzx_lotteryset where roomtype=20 and rtrim(item)=@bttype),@lotterynum,@money,0
  

--账变记录
INSERT INTO [owzx_accountchange]([uid],[changemoney],[remark],accounted)
select rtrim(@account),-@money,'投注',(select isnull(totalmoney,0) from owzx_users where uid=rtrim(@account))

--扣除用户金额
update a 
set a.totalmoney=a.totalmoney-@money
from owzx_users a where uid=rtrim(@account)

select '添加成功' state
commit tran t1

end try
begin catch
rollback tran t1
select ERROR_MESSAGE() state
end catch
");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }
        /// <summary>
        /// 更新投注记录 
        /// </summary>
        /// <param name="draw"></param>
        /// <returns></returns>
        public string UpdateBett(MD_Bett bett)
        {
            //            DbParameter[] parms = {
            //                                      GenerateInParam("@drawid", SqlDbType.Int, 4, bett.Drawid),
            //                                      GenerateInParam("@state", SqlDbType.VarChar, 1, bett.State),
            //                                      GenerateInParam("@exception", SqlDbType.VarChar, 100, bett.Exception),
            //                                      GenerateInParam("@updateuser", SqlDbType.VarChar, 30, bett.Updateuser)
            //                                    };
            //            string commandText = string.Format(@"
            //begin try
            //begin tran t1
            //if exists(select 1 from [{0}userdraw] where drawid=@drawid)
            //begin
            //UPDATE [{0}userdraw]
            //   SET state = @state,exception=@exception,updateuser=@updateuser,updatetime=convert(varchar(25),getdate(),120)
            //where drawid=@drawid
            //
            //--提现失败 返还佣金
            //if(@state='1')
            //begin
            //update a set  a.totalmoney=a.totalmoney+b.money
            //from {0}users a 
            //join {0}userdraw b on a.uid=b.uid and b.drawid=@drawid
            //end
            //
            //select '修改成功' state
            //commit tran t1
            //end
            //else
            //begin
            //select '提现记录已被删除' state
            //commit tran t1
            //end
            //end try
            //begin catch
            //rollback tran t1
            //select ERROR_MESSAGE() state
            //end catch
            //");
            //return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
            return "";
        }
        /// <summary>
        /// 删除投注记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteBett(string uid, int lotterytype,string expect, string bttype,string roomid)
        {
            string commandText = string.Format(@"
            begin try
            if exists(select 1 from owzx_lotteryrecord where type={3} and expect={1} and status=0)
            begin
            if exists(  select 1 from owzx_bett a 
  join owzx_lotteryset b on a.bttypeid=b.bttypeid
  where a.uid={0} and a.lotteryid={3} and a.lotterynum={1} and RTRIM(b.item)='{2}' and a.roomid={4})
            begin
            update a
            set a.totalmoney=isnull(a.totalmoney,0)+b.money
            from owzx_users a
            join owzx_bett b on a.uid=b.uid and a.uid={0} 
            join owzx_lotteryset c on b.bttypeid=c.bttypeid
            where  b.lotteryid={3} and b.lotterynum={1} and RTRIM(c.item)='{2}' and b.roomid={4}          

            delete a from owzx_bett a 
  join owzx_lotteryset b on a.bttypeid=b.bttypeid
  where a.uid={0} and a.lotteryid={3} and a.lotterynum={1} and RTRIM(b.item)='{2}' and a.roomid={4}

            select '删除成功' state
            end
            else
            begin
            select '记录已被删除' state
            end
            end
            else
            begin
             select '当期竞猜已经封盘，不能撤销投注' state
            end
            end try
            begin catch
            select ERROR_MESSAGE() state
            end catch
            ", uid, expect, bttype, lotterytype,roomid);
            return RDBSHelper.ExecuteScalar(commandText).ToString();
        }
        /// <summary>
        /// 删除投注记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteBettForId(string id)
        {
            string commandText = string.Format(@"
            begin try
            if exists(select 1 from owzx_bett where bettid in ({0}))
            begin
            delete from owzx_bett where bettid in ({0})
            select '删除成功' state
            end
            else
            begin
            select '记录已被删除' state
            end
            end try
            begin catch
            select ERROR_MESSAGE() state
            end catch
            ", id);
            return RDBSHelper.ExecuteScalar(commandText).ToString();
        }
        /// <summary>
        ///  获取投注记录(按照期数分组)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">没有where</param>
        /// <returns></returns>
        public DataTable GetBettListGpByNum(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
drop table #list
if OBJECT_ID('tempdb..#listgp') is not null
drop table #listgp



SELECT 
a.[bettid],a.[uid],a.[lotteryid],a.[bttypeid],a.[money],a.[lotterynum],a.isread,a.[addtime],b.mobile account,
e.type lottery,f.type bttype,c.item,cc.luckresult,aa.opencode,a.roomid,rtrim(cast(a.roomid as char(2))) room,b.username,aa.opentime
into  #list
FROM owzx_bett a
join owzx_users b on a.uid=b.uid
join owzx_lotteryset c on a.bttypeid=c.bttypeid
join owzx_lotteryrecord aa on a.lotterynum=aa.expect
left join owzx_bettprofitloss cc on a.bettid=cc.bettid
join owzx_sys_basetype e on a.lotteryid=e.systypeid
join owzx_sys_basetype f on c.type=f.systypeid
{0}

select ROW_NUMBER() over(order by opentime desc) id,lottery,lotterynum,
convert(varchar(19),opentime,120) opentime,sum(money) bettmoney,sum(luckresult) luckresult  
into #listgp
from #list group by lottery,lotterynum,opentime order by opentime desc 

declare @total int
select @total=(select count(1)  from #listgp)

if(@pagesize=-1)
begin
select *,@total TotalCount from #listgp
end
else
begin
select  *,@total TotalCount from #listgp where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }
        /// <summary>
        ///  获取投注记录(分页)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">没有where</param>
        /// <returns></returns>
        public DataTable GetBettList(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
drop table #list

SELECT ROW_NUMBER() over(order by a.bettid desc) id,
a.[bettid],a.[uid],a.[lotteryid],a.[bttypeid],a.[money],a.[lotterynum],a.isread,a.[addtime],b.mobile account,
e.type lottery,f.type bttype,c.item,cc.luckresult,aa.opencode,a.roomid,rtrim(cast(a.roomid as char(2))) room,b.username
into  #list
FROM owzx_bett a
join owzx_users b on a.uid=b.uid
join owzx_lotteryset c on a.bttypeid=c.bttypeid
join owzx_lotteryrecord aa on a.lotterynum=aa.expect
left join owzx_bettprofitloss cc on a.bettid=cc.bettid
join owzx_sys_basetype e on a.lotteryid=e.systypeid
join owzx_sys_basetype f on c.type=f.systypeid
{0}

declare @total int
select @total=(select count(1)  from #list)

if(@pagesize=-1)
begin
select *,@total TotalCount from #list
end
else
begin
select  *,@total TotalCount from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }
        /// <summary>
        ///  获取投注记录(分页)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">没有where</param>
        /// <returns></returns>
        public DataTable GetBettListForRoom(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
drop table #list

SELECT ROW_NUMBER() over(order by a.bettid desc) id,
a.[bettid],a.[uid],a.[lotteryid],a.[bttypeid],a.[money],a.[lotterynum],a.isread,a.[addtime],b.mobile account,e.type lottery,d.type room,g.type vip,f.type bttype,c.item,cc.luckresult
into  #list
FROM owzx_bett a
join owzx_users b on a.uid=b.uid
join owzx_lotteryset c on a.bttypeid=c.bttypeid
join owzx_lotteryrecord aa on a.lotterynum=aa.expect
left join owzx_bettprofitloss cc on a.uid=cc.uid and aa.lotteryid=cc.lotteryid
join owzx_sys_basetype d on a.roomid=d.systypeid
join owzx_sys_basetype e on a.lotteryid=e.systypeid
join owzx_sys_basetype f on c.type=f.systypeid
join owzx_sys_basetype g on a.vipid=g.systypeid
{0}

declare @total int
select @total=(select count(1)  from #list)

if(@pagesize=-1)
begin
select *,@total TotalCount from #list
end
else
begin
select  *,@total TotalCount from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }
        /// <summary>
        /// 获取当前竞猜各种类型的投注注数
        /// </summary>
        /// <param name="expect"></param>
        /// <returns></returns>
        public DataTable GetBettTotal(string expect)
        {
            string sql = string.Format(@"
if OBJECT_ID('tempdb..#listitem') is not null
drop table #listitem

select b.expect,a.bettid,c.item 
into #listitem
from owzx_bett a
join owzx_lotteryrecord b on a.lotteryid=b.type and b.expect='{0}'
join owzx_lotteryset c on a.bttypeid=c.bttypeid


if OBJECT_ID('tempdb..#listdt') is not null
drop table #listdt

select expect,item,count(bettid) itemtotal
into #listdt
from (
select expect,bettid,item from #listitem
where item in ('大','小','单','双','红','绿','蓝','豹子')
union all
select expect,bettid,'极值' item from #listitem
where item in ('极大','极小')
union all
select expect,bettid,'猜数字' item from #listitem
where item like '%[0-9]%'
union all
select expect,bettid,'组合' item from #listitem
where item in ('大单','小双','小单','大双')
) a group by expect,item


SELECT ROW_NUMBER() over(order by expect desc) id, expect,isnull([大],'') [大],isnull([小],'')[小],isnull([单],'')[单],
isnull([双],'')[双],isnull([组合],'')[组合],isnull([猜数字],'')[猜数字],isnull([极值],'')[极值]
,isnull([红],'')[红] ,isnull([绿],'')[绿],isnull([蓝],'')[蓝],isnull([豹子],'')[豹子],(select SUM(itemtotal) from #listdt) total

FROM #listdt pivot( 
sum(itemtotal) FOR item in ([大],[小],[单],[双],[组合],[猜数字],[极值],[红],[绿],[蓝],[豹子])
 )a
order by expect desc", expect);
            return RDBSHelper.ExecuteTable(sql, null)[0];
        }
        /// <summary>
        /// 验证投注操作是否异常
        /// </summary>
        /// <param name="expect">投注期号</param>
        /// <param name="bttypeid">投注类型id</param>
        /// <param name="money">投注金额</param>
        /// <param name="type">房间类型</param>
        /// <returns></returns>
        public string ValidateBetMoney(string expect, int bttypeid, int money, string type)
        {
            string sql = string.Format(@"

declare @expect varchar(50)='{0}',@roomtype varchar(10)='{3}'
declare @bttypeid int={1},@money int={2}

if OBJECT_ID('tempdb..#listitem') is not null
drop table #listitem

select b.expect,a.money,c.item,a.roomid,e.type
into #listitem
from owzx_bett a
join owzx_lotteryrecord b on a.lotteryid=b.type and a.lotterynum=@expect
join owzx_lotteryset c on a.bttypeid=c.bttypeid
join owzx_sys_basetype e on a.roomid=e.systypeid

--select * from #listitem



if OBJECT_ID('tempdb..#listdt') is not null
drop table #listdt

select expect,item,sum(money) itemtotal
into #listdt
from (
select expect,money,item from #listitem
where item in ('大','小','单','双','红','绿','蓝','豹子')
union all
select expect,money,'极值' item from #listitem
where item in ('极大','极小')
union all
select expect,money,'猜数字' item from #listitem
where item like '%[0-9]%'
union all
select expect,money,'组合' item from #listitem
where item in ('大单','小双','小单','大双')
) a group by expect,item

--select * from #listdt 
--初级
--1、单注10-20000，总注80000封顶
--2、大小单双20000封顶，极值5000封顶，猜数字5000封顶，组合10000封顶，红绿蓝20000封顶，豹子5000封顶

--中级
--1、单注50-30000，总注100000封顶
--2、大小单双30000封顶，极值10000封顶，猜数字10000封顶，组合20000封顶，红绿蓝20000封顶，豹子10000封顶

--高级
--1、单注500-200000，总注1000000封顶
--2、大小单双200000封顶，极值50000封顶，猜数字50000封顶，组合100000封顶，红绿蓝50000封顶，豹子20000封顶

declare @maxbett int
select @maxbett=(select SUM(money)+@money from owzx_bett where lotterynum=@expect)


if(@roomtype='初级' and @maxbett>80000 ) 
begin
select '总投注金额不能大于80000元宝' msg
end
else if(@roomtype ='中级' and @maxbett>100000 ) 
begin
select '总投注金额不能大于100000元宝' msg
end
else if(@roomtype='高级' and @maxbett>1000000 ) 
begin
select '总投注金额不能大于1000000元宝' msg
end
else
begin

declare @type varchar(50)
set @type=(select case 
when item in ('大单','小双','小单','大双') then '组合'
when item like '%[0-9]%' then '猜数字'
when item in ('极大','极小') then '极值'
else item end
from owzx_lotteryset where bttypeid=@bttypeid)

--select @type

declare @totalnum int=0
select @totalnum=itemtotal from #listdt where item=@type

if(@roomtype='初级')
begin

if(@type in ('大','小','单','双','红','绿','蓝') and (@totalnum+@money)>20000)
begin
select '投注类型【'+@type+'】总注20000封顶' msg

end
else if(@type in('极值','猜数字' ,'豹子')and (@totalnum+@money)>5000)
begin
select '投注类型【'+@type+'】总注5000封顶' msg
end
else if(@type='组合' and (@totalnum+@money)>10000) 
begin
select '投注类型【'+@type+'】总注10000封顶' msg
end
else
begin
select '验证通过' msg
end

end
else if(@roomtype ='中级')
begin
if(@type in ('大','小','单','双') and (@totalnum+@money)>30000)
begin
select '投注类型【'+@type+'】总注30000封顶' msg

end
else if(@type in('极值','猜数字' ,'豹子')and (@totalnum+@money)>10000)
begin
select '投注类型【'+@type+'】总注10000封顶' msg
end
else if(@type in('组合','红','绿','蓝') and (@totalnum+@money)>20000) 
begin
select '投注类型【'+@type+'】总注20000封顶' msg
end
else
begin
select '验证通过' msg
end
end
else if(@roomtype ='高级')
begin
if(@type in ('大','小','单','双') and (@totalnum+@money)>200000)
begin
select '投注类型【'+@type+'】总注200000封顶' msg

end
else if(@type in('极值','猜数字','红','绿','蓝' )and (@totalnum+@money)>50000)
begin
select '投注类型【'+@type+'】总注50000封顶' msg
end
else if(@type ='组合' and (@totalnum+@money)>100000) 
begin
select '投注类型【'+@type+'】总注100000封顶' msg
end
else if(@type='豹子' and (@totalnum+@money)>20000) 
begin
select '投注类型【'+@type+'】总注20000封顶' msg
end
else
begin
select '验证通过' msg
end
end
else
begin
select '房间类型错误' msg
end

end

", expect, bttypeid,money,type);
            return RDBSHelper.ExecuteScalar(sql).ToString();
        }

        #endregion

        #region 彩票投注赔率
        /// <summary>
        /// 赔率说明
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable SetRemark(string type)
        {
            string sql = string.Format(@"
                    if OBJECT_ID('tempdb..#list') is not null
                    drop table #list 

                    if OBJECT_ID('tempdb..#listtp') is not null
                    drop table #listtp 

                    SELECT [type], [odds], max(a.bttypeid) id,
                    [item] = (  
                    SELECT [item] + ','  
                    FROM   owzx_lotteryset AS b  
                    WHERE  b.roomtype = a.roomtype and b.[type] = a.[type]  and b.[odds] = a.[odds]  
                    order by b.roomtype,b.bttypeid 
                    FOR XML PATH('') 
                    ) 
                    into #list 
                    FROM  owzx_lotteryset AS a 
                    where a.roomtype={0}
                    group by roomtype,[type],[odds] 
                    order by type ,id desc

                    select type,odds,
                    replace(
                    replace(
                    replace(
                    replace(
                    replace(
                    replace(
                    replace(
                    replace(
                    replace(LEFT(item,len(item)-1),'极大,极小','极值(极大,极小)')
                    ,'大单,小双','大单|小双')
                    ,'小单,大双','小单|大双')
                    ,'大,小,单,双','大小单双')
                     ,'大,单,双,小','大小单双')
                    ,'红,绿,蓝','红绿蓝')
                    ,'大单,小单|大双,小双','小单|大双')
                     ,'大单,大双,小单,小双','小单|大双')
                     ,'红,蓝,绿','红绿蓝') item 
                    into #listtp 
                    from #list 
                     
                    if({0} in (21,22))
                    begin
                    insert into #listtp(type,odds,item)
                    select 12,'4.0','大单|小双'
                    end
                    select '1:'+odds odds,item from (
                    select top 4  odds,item from #listtp where type=12 order by cast(odds as decimal(18,1)) desc ,item
                    ) a
                    union all
                    select * from (select top 2  '1:'+odds odds,item from #listtp where type=14 order by odds ) a
                    union all
                    select  '1:'+odds odds,item from #listtp where type=13 ", type);
            return RDBSHelper.ExecuteTable(sql, null)[0];
        }

        /// <summary>
        /// 添加投注赔率
        /// </summary>
        /// <param name="lotset"></param>
        /// <returns></returns>
        public string AddLotterySet(MD_LotterySet lotset)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@lotterytype", SqlDbType.VarChar, 20, lotset.Lotterytype),
                                    GenerateInParam("@type", SqlDbType.VarChar, 50, lotset.Type),
                                    GenerateInParam("@item", SqlDbType.VarChar, 10, lotset.Item),
                                    GenerateInParam("@odds", SqlDbType.VarChar, 10, lotset.Odds),
                                    GenerateInParam("@nums", SqlDbType.VarChar, 50, lotset.Nums)
                                    };
            string commandText = string.Format(@"
begin try
begin tran t1
INSERT INTO owzx_lotteryset([lotterytype]
           ,[type]
           ,[item]
           ,[odds]
           ,[nums]
           )
VALUES(@lotterytype,@type,@item,@odds,@nums)

select '添加成功' state
commit tran t1

end try
begin catch
rollback tran t1
select ERROR_MESSAGE() state
end catch
");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }
        /// <summary>
        /// 更新投注赔率
        /// </summary>
        /// <param name="lotset"></param>
        /// <returns></returns>
        public string UpdateLotterySet(MD_LotterySet lotset)
        {
//            DbParameter[] parms = {
//                                    GenerateInParam("@lotterytype", SqlDbType.VarChar, 20, lotset.Lotterytype),
//                                    GenerateInParam("@type", SqlDbType.VarChar, 50, lotset.Type),
//                                    GenerateInParam("@item", SqlDbType.VarChar, 10, lotset.Item),
//                                    GenerateInParam("@odds", SqlDbType.VarChar, 10, lotset.Odds),
//                                    GenerateInParam("@nums", SqlDbType.VarChar, 50, lotset.Nums)
//                                    };
//            string commandText = string.Format(@"
//begin try
//begin tran t1
//if exists(select 1 from owzx_lotteryset where lotterytype=@lotterytype and type=@type and item=@item)
//begin
//UPDATE owzx_lotteryset
//   SET odds = @odds,nums=@nums
//where lotterytype=@lotterytype and type=@type and item=@item
//
//
//select '修改成功' state
//commit tran t1
//end
//else
//begin
//select '记录已被删除' state
//commit tran t1
//end
//end try
//begin catch
//rollback tran t1
//select ERROR_MESSAGE() state
//end catch
//");
            DbParameter[] parms = {
                                        GenerateInParam("@bttypeid", SqlDbType.Int,4, lotset.Bttypeid),
                                        GenerateInParam("@odds", SqlDbType.VarChar,20, lotset.Odds)
                                       
                                    };
            string commandText = string.Format(@"
begin try
begin tran t1

if exists(select 1 from owzx_lotteryset where bttypeid=@bttypeid)
begin
update a set a.[odds]=@odds
from [owzx_lotteryset] a where  bttypeid=@bttypeid
       
select '修改成功' state
commit tran t1

end
else
begin
select '记录已被删除' state
commit tran t1
end

end try
begin catch
rollback tran t1
select ERROR_MESSAGE() state
end catch

");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }

        /// <summary>
        /// 删除投注赔率
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteLotterySet(string id)
        {
            string commandText = string.Format(@"
            begin try
            if exists(select 1 from owzx_lotteryset where bttypeid in ({0}))
            begin
            delete from owzx_lotteryset where bttypeid in ({0})
            select '删除成功' state
            end
            else
            begin
            select '记录已被删除' state
            end
            end try
            begin catch
            select ERROR_MESSAGE() state
            end catch
            ", id);
            return RDBSHelper.ExecuteScalar(commandText).ToString();
        }

        /// <summary>
        ///  获取投注赔率(分页)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">没有where</param>
        /// <returns></returns>
        public DataTable GetLotterySetList(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
drop table #list

SELECT ROW_NUMBER() over(order by a.bttypeid ) id,
a.[bttypeid]--,a.[lotterytype],b.type as lottery
,a.[type],c.type as settype,a.[item],a.odds odds,a.[nums],a.[addtime],a.roomtype,d.type as room
into  #list
FROM owzx_lotteryset a
--join owzx_sys_basetype b on a.lotterytype=b.systypeid
join owzx_sys_basetype c on a.type=c.systypeid
join owzx_sys_basetype d on a.roomtype=d.systypeid
{0}

declare @total int
select @total=(select count(1)  from #list)

if(@pagesize=-1)
begin
select *,@total TotalCount from #list
end
else
begin
select  *,@total TotalCount from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }

        /// <summary>
        ///  获取投注赔率
        /// </summary>
        /// <param name="condition">没有where</param>
        /// <returns></returns>
        public DataSet GetLotterySetList(string condition = "")
        {

            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
drop table #list

SELECT ROW_NUMBER() over(order by a.bttypeid ) id,
a.[bttypeid],--a.[lotterytype],b.type as lottery,
a.[type],c.type as settype,a.[item],'1:'+a.[odds] odds ,a.[nums],a.[addtime],a.roomtype,d.type as room
into  #list
FROM owzx_lotteryset a
--join owzx_sys_basetype b on a.lotterytype=b.systypeid
join owzx_sys_basetype c on a.type=c.systypeid
join owzx_sys_basetype d on a.roomtype=d.systypeid
{0}


--select distinct --lotterytype,lottery,
--type,settype from #list

select bttypeid,item,odds,nums from #list order by bttypeid 
--where type=12 order by bttypeid

--select bttypeid,item,odds,nums from #list where type=13 order by bttypeid

--select bttypeid,item,odds,nums from #list where type=14 order by bttypeid

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, null);
        }
        #endregion

        #region 记录等待计算用户奖金彩票
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="bett"></param>
        /// <returns></returns>
        public string AddWaitPay(MD_WaitPayBonus pay)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@expect", SqlDbType.VarChar, 100, pay.Expect),
                                    GenerateInParam("@isread", SqlDbType.Bit, 10, pay.Isread)
                                    };
            string commandText = string.Format(@"
begin try
begin tran t1

INSERT INTO owzx_waitpaybonus([expect],[isread])
VALUES(@expect,@isread)

select '添加成功' state
commit tran t1

end try
begin catch
rollback tran t1
select ERROR_MESSAGE() state
end catch
");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }
        /// <summary>
        /// 更新记录 
        /// </summary>
        /// <param name="draw"></param>
        /// <returns></returns>
        public string UpdateWaitPay(MD_WaitPayBonus pay)
        {
            return "";
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteWaitPay(string id)
        {
            string commandText = string.Format(@"
            begin try
            if exists(select 1 from owzx_waitpaybonus where bonusid in ({0}))
            begin
            delete from owzx_waitpaybonus where bonusid in ({0})
            select '删除成功' state
            end
            else
            begin
            select '记录已被删除' state
            end
            end try
            begin catch
            select ERROR_MESSAGE() state
            end catch
            ", id);
            return RDBSHelper.ExecuteScalar(commandText).ToString();
        }

        /// <summary>
        ///  获取记录(分页)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">没有where</param>
        /// <returns></returns>
        public DataTable GetWaitPayList(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
drop table #list

SELECT ROW_NUMBER() over(order by a.bonusid desc) id,
a.[bonusid],a.[expect],a.[isread],a.[addtime],a.[updatetime]
into  #list
FROM owzx_waitpaybonus a
{0}

declare @total int
select @total=(select count(1)  from #list)

if(@pagesize=-1)
begin
select *,@total TotalCount from #list
end
else
begin
select  *,@total TotalCount from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }

        #endregion

        #region 计算用户奖金
        /// <summary>
        /// 计算竞猜投注结果
        /// </summary>
        /// <returns></returns>
        public string ExcuteBettResult()
        {
            return RDBSHelper.ExecuteScalar(CommandType.StoredProcedure, "owzx_payuserbonus", null).ToString();
        }
        /// <summary>
        /// 计算北京28竞猜投注结果
        /// </summary>
        /// <param name="lotterynum"></param>
        /// <returns></returns>
        public string ExcuteBJBettResult(string lotterynum)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@lotterynum", SqlDbType.VarChar, 20, lotterynum)
                                    };
            return RDBSHelper.ExecuteScalar(CommandType.StoredProcedure, "", parms).ToString();
        }
        /// <summary>
        /// 计算加拿大28竞猜投注结果
        /// </summary>
        /// <param name="lotterynum"></param>
        /// <returns></returns>
        public string ExcuteCanadaBettResult(string lotterynum)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@lotterynum", SqlDbType.VarChar, 20, lotterynum)
                                    };
            return RDBSHelper.ExecuteScalar(CommandType.StoredProcedure, "", parms).ToString();
        }
        #endregion

        #region 房间信息

        /// <summary>
        /// 添加房间信息
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public string AddRoom(MD_LotteryRoom room)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@lotterytype", SqlDbType.Int, 4, room.Lotterytype),
                                    GenerateInParam("@room", SqlDbType.Int, 4 ,room.Room),
                                    //GenerateInParam("@maxuser", SqlDbType.Int, 4, room.Maxuser),
                                    GenerateInParam("@backrate", SqlDbType.Int, 4, room.Backrate),
                                    GenerateInParam("@enter", SqlDbType.Int, 4, room.Enter),
                                    };
            string commandText = string.Format(@"
begin try
begin tran t1

INSERT INTO [owzx_lotteryroom]([lotterytype]
           ,[room]
           --,[maxuser]
           ,[backrate]
           ,[enter])
VALUES(@lotterytype,@room,
--@maxuser,
@backrate,@enter)

select '添加成功' state
commit tran t1

end try
begin catch
rollback tran t1
select ERROR_MESSAGE() state
end catch
");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }
        /// <summary>
        /// 更新房间信息
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public string UpdateRoom(MD_LotteryRoom room)
        {
            DbParameter[] parms = {
                                                  GenerateInParam("@roomid", SqlDbType.Int, 4, room.Roomid),
                                                  //GenerateInParam("@maxuser", SqlDbType.Int, 4, room.Maxuser),
                                                  GenerateInParam("@backrate", SqlDbType.Int, 4, room.Backrate),
                                                  GenerateInParam("@enter", SqlDbType.Int, 4, room.Enter)
                                                };
            string commandText = string.Format(@"
            begin try
            begin tran t1
            if exists(select 1 from owzx_lotteryroom where roomid=@roomid)
            begin
            UPDATE owzx_lotteryroom
               SET backrate=@backrate,enter=@enter
            where roomid=@roomid
            --maxuser = @maxuser,
            select '修改成功' state
            commit tran t1
            end
            else
            begin
            select '记录已被删除' state
            commit tran t1
            end
            end try
            begin catch
            rollback tran t1
            select ERROR_MESSAGE() state
            end catch
            ");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }

        /// <summary>
        /// 删除房间信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteRoom(string id)
        {
            string commandText = string.Format(@"
            begin try
            if exists(select 1 from owzx_lotteryroom where roomid in ({0}))
            begin
            delete from owzx_lotteryroom where roomid in ({0})
            select '删除成功' state
            end
            else
            begin
            select '记录已被删除' state
            end
            end try
            begin catch
            select ERROR_MESSAGE() state
            end catch
            ", id);
            return RDBSHelper.ExecuteScalar(commandText).ToString();
        }

        /// <summary>
        ///  获取房间信息(分页)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">没有where 主表 a  彩票类型表 b 房间类型表c</param>
        /// <returns></returns>
        public DataTable GetRoomList(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
drop table #list

SELECT ROW_NUMBER() over(order by a.roomid ) id,
a.[roomid] ,a.[lotterytype],a.[room],a.[maxuser],a.vipmaxuser,d.[backrate],a.[enter],a.[addtime],b.type lotteryname,c.type roomname
into  #list
from owzx_lotteryroom a
join owzx_sys_basetype b on a.lotterytype=b.systypeid
join owzx_sys_basetype c on a.room=c.systypeid
join owzx_backrate  d on a.backrate=d.rateid 
{0}

declare @total int
select @total=(select count(1)  from #list)

if(@pagesize=-1)
begin
select *,@total TotalCount from #list
end
else
begin
select  *,@total TotalCount from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }
        #endregion

        #region 回水规则

        /// <summary>
        /// 添加回水规则
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public string AddRateRule(MD_BackRate rate)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@roomtypeid", SqlDbType.Int, 4, rate.Roomtypeid),
                                    GenerateInParam("@minloss", SqlDbType.Decimal, 18 ,rate.Minloss),
                                    GenerateInParam("@maxloss", SqlDbType.Decimal, 18, rate.Maxloss),
                                    GenerateInParam("@backrate", SqlDbType.Int, 4, rate.Backrate)
                                    };
            string commandText = string.Format(@"
begin try
begin tran t1

INSERT INTO [owzx_backrate]([roomtypeid]
           ,[minloss]
           ,[maxloss]
           ,[backrate])
VALUES(@roomtypeid,@minloss,@maxloss,@backrate)

select '添加成功' state
commit tran t1

end try
begin catch
rollback tran t1
select ERROR_MESSAGE() state
end catch
");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }
        /// <summary>
        /// 更新回水规则
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public string UpdateRateRule(MD_BackRate rate)
        {
            DbParameter[] parms = {
                                      GenerateInParam("@rateid", SqlDbType.Int, 4, rate.Rateid),
                                    GenerateInParam("@roomtypeid", SqlDbType.Int, 4, rate.Roomtypeid),
                                    GenerateInParam("@minloss", SqlDbType.Decimal, 18 ,rate.Minloss),
                                    GenerateInParam("@maxloss", SqlDbType.Decimal, 18, rate.Maxloss),
                                    GenerateInParam("@backrate", SqlDbType.Int, 4, rate.Backrate)
                                    };
            string commandText = string.Format(@"
            begin try
            begin tran t1
            if exists(select 1 from owzx_backrate where rateid=@rateid)
            begin
            UPDATE owzx_backrate
               SET roomtypeid = @roomtypeid,minloss=@minloss,maxloss=@maxloss,backrate=@backrate
            where rateid=@rateid
            
            select '修改成功' state
            commit tran t1
            end
            else
            begin
            select '记录已被删除' state
            commit tran t1
            end
            end try
            begin catch
            rollback tran t1
            select ERROR_MESSAGE() state
            end catch
            ");
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }

        /// <summary>
        /// 删除回水规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteRateRule(string id)
        {
            string commandText = string.Format(@"
            begin try
            if exists(select 1 from owzx_backrate where rateid in ({0}))
            begin
            delete from owzx_backrate where rateid in ({0})
            select '删除成功' state
            end
            else
            begin
            select '记录已被删除' state
            end
            end try
            begin catch
            select ERROR_MESSAGE() state
            end catch
            ", id);
            return RDBSHelper.ExecuteScalar(commandText).ToString();
        }

        /// <summary>
        ///  获取回水规则(分页)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="condition">没有where 主表 a  房间类型表 b </param>
        /// <returns></returns>
        public DataTable GetRateRuleList(int pageNumber, int pageSize, string condition = "")
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };


            string commandText = string.Format(@"
begin try
if OBJECT_ID('tempdb..#list') is not null
drop table #list

SELECT ROW_NUMBER() over(order by a.rateid ) id,
a.[rateid]
      ,a.[roomtypeid]
      ,a.[minloss]
      ,a.[maxloss]
      ,a.[backrate]
      ,a.[addtime],b.type room
into  #list
from owzx_backrate a
join owzx_sys_basetype b on a.roomtypeid=b.systypeid
{0}

declare @total int
select @total=(select count(1)  from #list)

if(@pagesize=-1)
begin
select *,@total TotalCount from #list
end
else
begin
select  *,@total TotalCount from #list where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", condition);

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }
        #endregion

        #region 报表

        /// <summary>
        /// 盈利报表 彩票类型不参数分组,包含回水
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="pageNumber"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable GetProfitListNoLottery(string type, int pageSize, int pageNumber, string start, string end)
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };

            string bet = string.Empty; string back = string.Empty;

            if (start != string.Empty)
            {
                bet += " and convert(varchar(10),b.opentime,120)>='" + start + "'";

                back += " and convert(varchar(10),addtime,120)>='" + start + "'";
            }
            if (end != string.Empty)
            {

                bet += " and convert(varchar(10),b.opentime,120)<='" + end + "'";
                back += " and convert(varchar(10),addtime,120)<='" + end + "'";
            }
            string sql = string.Format(@"
begin try


declare @type varchar(20)='{0}'

if OBJECT_ID('tempdb..#list') is not null
drop table #list

select a.uid,lotterynum expect,money,a.bettid,c.luckresult ,convert(varchar(10),b.opentime,120) opentime,
b.type
into #list
from owzx_bett a
join owzx_lotteryrecord b on a.lotteryid=b.type and a.lotterynum=b.expect
join owzx_bettprofitloss c on a.bettid=c.bettid 
where isread=1 {1}


if OBJECT_ID('tempdb..#listday') is not null
drop table #listday

if OBJECT_ID('tempdb..#listresult') is not null
drop table #listresult

create table #listresult
(
id int IDENTITY(1,1),
opentime varchar(10),
betttotal decimal(18,2),
luckresult decimal(18,2),
backtotal decimal(18,2),
profitresult decimal(18,2),
)


select a.*,isnull(money,0) backtotal 
into #listday
from (
select opentime,SUM(money) betttotal,SUM(luckresult) luckresult
from #list a
group by opentime )a
left join (
select sum(money) money,addtime from 
(
select money,convert(varchar(10),addtime,120) addtime 
from owzx_userback 
where status=2 {2}
) a  group by addtime
) b on a.opentime=b.addtime

if(@type='每日盈亏')
begin
insert into #listresult(opentime, betttotal,luckresult,backtotal, profitresult)
select opentime, betttotal,luckresult,backtotal,
(case when luckresult>0 then betttotal-luckresult-backtotal else betttotal+luckresult-backtotal end) profitresult 
from #listday a
order by opentime
end
else if(@type='每周盈亏')
begin
insert into #listresult(opentime, betttotal,luckresult,backtotal, profitresult)
select opentime,betttotal,luckresult,backtotal,
(case when luckresult>0 then betttotal-luckresult-backtotal else betttotal+luckresult-backtotal end) profitresult 
from (
select opentime, SUM(betttotal) betttotal,SUM(luckresult) luckresult,SUM(backtotal) backtotal  from (
select datepart(WK,opentime) opentime, betttotal, luckresult,backtotal from #listday 
) a group by opentime
) a 

order by opentime
end
else if(@type='每月盈亏')
begin
insert into #listresult(opentime, betttotal,luckresult,backtotal,profitresult)
select opentime,betttotal,luckresult,backtotal,
(case when luckresult>0 then betttotal-luckresult-backtotal else betttotal+luckresult-backtotal end) profitresult 
from (
select opentime,SUM(betttotal) betttotal,SUM(luckresult) luckresult,SUM(backtotal) backtotal  from (
select datepart(month,opentime) opentime, betttotal,luckresult,backtotal from #listday 
) a group by opentime
) a order by opentime
end

declare @total int
select @total=(select count(1)  from #listresult)

if(@pagesize=-1)
begin
select *,@total TotalCount from #listresult
end
else
begin
select *,@total TotalCount from #listresult where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", type, bet, back);

            DataTable dt = RDBSHelper.ExecuteTable(sql, parms)[0];
            return dt;

        }

        /// <summary>
        /// 盈利报表 彩票类型不参数分组,包含回水
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pageSize">-1 取全部</param>
        /// <param name="pageNumber"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable GetProfitListGpByUser(int pageSize, int pageNumber, string start, string end)
        {
            DbParameter[] parms = {
                                      GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize),
                                      GenerateInParam("@pageindex", SqlDbType.Int, 4, pageNumber)
                                  };

            string bet = string.Empty; string back = string.Empty;

            if (start != string.Empty)
            {
                bet += " and convert(varchar(10),b.opentime,120)>='" + start + "'";

            }
            if (end != string.Empty)
            {
                bet += " and convert(varchar(10),b.opentime,120)<='" + end + "'";
            }
            string sql = string.Format(@"
begin try

if OBJECT_ID('tempdb..#list') is not null
drop table #list

select a.uid,lotterynum expect,money,a.bettid,c.luckresult ,convert(varchar(10),b.opentime,120) opentime,
b.type,d.username
into #list
from owzx_bett a
join owzx_lotteryrecord b on a.lotteryid=b.type and a.lotterynum=b.expect
join owzx_bettprofitloss c on a.bettid=c.bettid 
join owzx_users d on a.uid=d.uid
where isread=1 {0}


if OBJECT_ID('tempdb..#listday') is not null
drop table #listday

if OBJECT_ID('tempdb..#listresult') is not null
drop table #listresult

create table #listresult
(
id int IDENTITY(1,1),
username varchar(20),
betttotal decimal(18,2),
luckresult decimal(18,2)
)


select a.*
into #listday
from (
select username,SUM(money) betttotal,SUM(luckresult) luckresult
from #list a
group by username )a
union all
select * from (
select '合计' username,isnull(SUM(money),0) betttotal,isnull(SUM(luckresult),0) luckresult
from #list ) a where betttotal>0



insert into #listresult(username, betttotal,luckresult)
select username,betttotal,luckresult 
from #listday a


declare @total int
select @total=(select count(1)  from #listresult)

if(@pagesize=-1)
begin
select *,@total TotalCount from #listresult
end
else
begin
select *,@total TotalCount from #listresult where id>@pagesize*(@pageindex-1) and id <=@pagesize*@pageindex
end

end try
begin catch
select ERROR_MESSAGE() state
end catch

", bet);

            DataTable dt = RDBSHelper.ExecuteTable(sql, parms)[0];
            return dt;

        }
        #endregion

        #region 获取竞猜页用户信息

        public DataTable GetLotteryUserInfo(int uid,string expect="", int lotterytype=-1)
        {
            string sql = string.Empty;
            if (lotterytype > 0)
            {
                sql = string.Format(@"declare @ac_money decimal(18,1)=0.0,@total_bett decimal(18,1)=0.0
select @ac_money=ISNULL(totalmoney,0) from owzx_users where uid={1}

select @total_bett=SUM(money)
from owzx_bett a
join owzx_lotteryrecord b on a.lotteryid=b.type and a.lotterynum=b.expect
and a.lotteryid={0} and a.uid={1} and b.status=0 and b.expect='{2}' and datediff(second,getdate(),b.opentime)>0 
group by a.lotteryid,a.uid,b.expect

select @ac_money ac_money,@total_bett total_bett
", lotterytype, uid,expect);
            }
            else
            {
                sql = string.Format(@"
declare @ac_money decimal(18,1)=0.0,@total_bett decimal(18,1)=0.0
select @ac_money=ISNULL(totalmoney,0) from owzx_users where uid={0}

select @total_bett=SUM(money)
from owzx_bett 
where uid={0} and isread=0 group by uid

select @ac_money ac_money,@total_bett total_bett
", uid);
            }
            return RDBSHelper.ExecuteTable(sql,null)[0];

        }
        #endregion
    }
}
