﻿/*竞猜*/
/*倒计时 剩余总时间，期号，总时间，封盘时间,结果*/
var tiner;
var traptime;
function GetRTime(type, ctime, fcnum, totalime, stoptime)
{
    clearTimeout(tiner);
    clearTimeout(traptime);

    var nS = parseInt(ctime);

    //ns 开奖时间
    nS = nS - 1;
    if (nS <= -30)
        clearTimeout(tiner);
    if (nS > -30)
        tiner = setTimeout("GetRTime(" + type + "," + nS + "," + fcnum + "," + totalime + "," + stoptime + ")", 1000);

    if (nS > 0)
    {

        if (nS > stoptime && nS <= totalime)
        {
            var rems = nS - stoptime;
            //bett
            if (type != 13)
            {
                $(".remains").html(
                    '第 <i class="bold">' + fcnum + '</i>期  还有<span class="ltwarn">' + rems + '</span>秒停止下注!');
            } else
            {
                $(".remains").html(
                    '第 <i class="bold">' + fcnum + '</i>期  还有<span class="ltwarn">' + formatSeconds(rems) + '</span>停止下注!');
            }
            StrTimeOut = 1;
        }
        else if (nS > 0 && nS <= stoptime)
        {
            if (nS != 0)
            {
                if (type != 13)
                {
                    $(".remains").html(
                   '第 <i class="bold">' + fcnum + '</i>期  停止下注,还有<span class="ltwarn">' + nS + '</span>秒开奖!');
                } else
                {
                    $(".remains").html(
                   '第 <i class="bold">' + fcnum + '</i>期  停止下注,还有<span class="ltwarn">' + formatSeconds(nS) + '</span>秒开奖!');
                }

            } else
            {
                $("#jquery_jplayer_1").jPlayer('play');
                $(".remains").html(
                                '第 <i class="bold">' + fcnum + '</i>期  正在开奖,请稍后!');
            }
            if ($(".lot_btn_" + fcnum + " .ltoperate").text().toString().trim() != "正在开奖")
            {
                $(".lot_btn_" + fcnum).html("").html('<a href="#"><div class="ltoperate">正在开奖</div></a>');
            }
            StrTimeOut = -1;
        }

    }
    else if (nS >= -30 && nS <= 0)
    {
        //nS = nS - 1;
        StrTimeOut = -1;
        if (nS > -30)
        {
            $(".remains").html(
                '第 <i class="bold">' + fcnum + '</i>期  正在开奖,请稍后!');
        }
        if (nS == -5 || nS == -10 || nS == -15 || nS == -20 || nS == -25 || nS == -30)
        {
            $(".remains").html('Loading......');
            $.post("/nwlottery/lotteryopen", { "type": lotterytype, "expect": fcnum }, function (data)
            {

                if (data == "1")
                {
                    clearTimeout(tiner);
                    clearTimeout(traptime);
                    $("#jquery_jplayer_1").jPlayer('play');
                    if ($(".sec_head a:eq(0)").hasClass("hot") && isbett == 0)
                    {
                        $(".lot_content").load("/nwlottery/_index", { "type": lotterytype });
                    } else
                    {
                        $(".remains").html('第 <i class="bold">' + fcnum + '</i>期  已开奖,请刷新!');
                    }
                    //return;
                } else
                {
                    if (nS == -30)
                    {
                        if ($(".sec_head a:eq(0)").hasClass("hot") && isbett == 0)
                        {
                            clearTimeout(tiner);
                            clearTimeout(traptime);
                            $(".lot_content").load("/nwlottery/_index", { "type": lotterytype });
                        } else
                        {
                            $(".remains").html('第 <i class="bold">' + fcnum + '</i>期  已开奖,请刷新!');
                        }
                    }
                }
            });
        }
    }

}


var lothmtime;
function LotHMime(type, ctime, fcnum, totalime, stoptime)
{
    clearTimeout(lothmtime);
    var nS = parseInt(ctime);

    //ns 开奖时间
    nS = nS - 1;

    if (nS > -30)
        lottime = setTimeout("LotHMime(" + type + "," + nS + "," + fcnum + "," + totalime + "," + stoptime + ")", 1000);

    if (nS > 0)
    {
        if (nS > stoptime && nS <= totalime)
        {
            var rems = nS - stoptime;
            //bett
            if (type != 13)
            {

                $(".lot-time").html(
                    '距 ' + fcnum + ' 期开奖倒计时：<span id="time_show">' + formatSeconds(nS) + '</span>');
            } else
            {

            }
            StrTimeOut = 1;
        }
        else if (nS >= 0 && nS <= stoptime)
        {
            if (type != 13)
            {
                var txt=
                $(".lot-time").html(
                '距 ' + fcnum + ' 期开奖倒计时：<span id="time_show">' + formatSeconds(nS) + '</span>'
                );
            } else
            {

            }

            StrTimeOut = -1;
        }

    } else
    {
        clearTimeout(lothmtime);
        //加载下一期
        $.post("/nwlottery/lastlottery", { "type": type }, function (data)
        {
            var dt = JSON.parse(data);
            if (dt.state == "success")
            {
                if (dt.biz_content.time == "已停售" || dt.biz_content.time == "维护中")
                {
                    $(".lot-time").html(
                               dt.biz_content.time);
                } else
                {
                    //新一期开始 更新期号，清除上期投注信息
                    
                    setTimeout("LotHMime(" + type + "," + dt.biz_content.time + "," + dt.biz_content.expect + "," + 300 + "," + 30 + ")", 1000);
                }
            } else
            { }
        })
    }

}

var lottime;
function LotRTime(type, ctime, fcnum, totalime, stoptime)
{
    clearTimeout(lottime);
    var nS = parseInt(ctime);

    //ns 开奖时间
    nS = nS - 1;

    if (nS > -30)
        lottime = setTimeout("LotRTime(" + type + "," + nS + "," + fcnum + "," + totalime + "," + stoptime + ")", 1000);

    if (nS > 0)
    {
        if (nS > stoptime && nS <= totalime)
        {
            var rems = nS - stoptime;
            //bett
            if (type != 13)
            {

                $(".ct-down").html(
                    '距本期封盘剩余：<span class="ct-time">' + rems + '</span> 秒');
            } else
            {

            }
            StrTimeOut = 1;
        }
        else if (nS >= 0 && nS <= stoptime)
        {
            if (type != 13)
            {
                $(".ct-down").html(
                '距本期开奖剩余：<span class="ct-time">' + nS + '</span> 秒');
            } else
            {

            }

            StrTimeOut = -1;
        }

    } else
    {
        clearTimeout(lottime);
        trap(type, fcnum);
        //加载下一期
        $.post("/nwlottery/lastlottery", { "type": type }, function (data)
        {
            var dt = JSON.parse(data);
            if (dt.state == "success")
            {
                if (dt.biz_content.time == "已停售" || dt.biz_content.time == "维护中")
                {
                    $(".ct-down").html(
                               dt.biz_content.time);
                } else
                {
                    //新一期开始 更新期号，清除上期投注信息
                    $(".bar-a").text("");
                    expect = dt.biz_content.expect;
                    setTimeout("LotRTime(" + type + "," + dt.biz_content.time + "," + dt.biz_content.expect + "," + 300 + "," + 30 + ")", 1000);
                }
            } else
            { }
        })
    }

}
var lottrap;
function trap(lotterytype, expect)
{
    $.get("/nwlottery/existslot", { "lotterytype": lotterytype, "expect": expect }, function (data)
    {
        if (data == "1")
        {
            clearTimeout(lottrap);
            //重新加载footer中数据
            $.get("/nwlottery/userbetmoney", { "lotterytype": lotterytype, "expect": expect }, function (data)
            {
                if (data != "")
                {
                    var my = data.split('|');
                    $(".bala-money").text(my[0]);
                    $(".bet-money").text(my[1]);
                }
            });
        } else if (data == "2")
        {
            lottrap = setTimeout("trap(" + lotterytype + "," + expect + ")", 1000);
        } else if (data == "3")
        {
            clearTimeout(lottrap);
        }
    })
}

function PrefixInteger(num, length)
{
    return (Array(length).join('0') + num).slice(-length);
}

function formatSeconds(value)
{
    var theTime = parseInt(value);// 秒 
    var theTime1 = 0;// 分 
    var theTime2 = 0;// 小时 
    // alert(theTime); 
    if (theTime > 60)
    {
        theTime1 = parseInt(theTime / 60);
        theTime = parseInt(theTime % 60);
        // alert(theTime1+"-"+theTime); 
        if (theTime1 > 60)
        {
            theTime2 = parseInt(theTime1 / 60);
            theTime1 = parseInt(theTime1 % 60);
        } 
    } 
    //var result = "" + parseInt(theTime);//+ "秒";
    //if (theTime1 > 0)
    //{
    //    result = "" + parseInt(theTime1) + ":" + result;
    //}
    //if (theTime2 > 0)
    //{
    //    result = "" + parseInt(theTime2) + ":" + result;
    //}
    var result = "" + PrefixInteger(theTime,2);//+ "秒";
    
    result = "" + PrefixInteger(theTime1,2) + ":" + result;
    
    return result;
}

/*数字增加，分割*/
function transStr(str)
{
    str = str.toString()
    var begin = "";
    var after = "";
    var l;
    var str2 = "";
    if (str.indexOf(".") > 0)
    {
        begin = str.substring(0, str.indexOf("."));
        after = str.substring(str.indexOf("."), str.length);
    }
    else
    {
        begin = str;
    }

    l = begin.length / 3;
    if (l > 1)
    {
        for (var i = 0; i < l;)
        {
            str2 = "," + begin.substring(begin.length - 3, begin.length) + str2;
            begin = begin.substring(0, begin.length - 3);
            l = begin.length / 3;
        }
        if (after.length < 3)
        {
            str2 = begin + str2 + after;
        } else
        {
            str2 = begin + str2 + after
        }
        return str2.substring(1);
    }
    else
    {
        if (after.length < 3)
        {
            return str;

        } else
        {
            return str;
        }
    }
}
//暂停时间
var samplingRate = function (interval)
{
    var mark;
    mark = 0;
    return function ()
    {
        var now;
        now = Date.now();
        if (now - mark < interval)
        {
            return false;
        }
        return mark = now;
    };
};

var sampling = samplingRate(1000);


function chgTimes(numID, times)
{
    //if (sampling())
    {
        var numIDx = "#" + numID;
        $(numIDx).attr("unable", "false");
        if (parseInt($(numIDx).val() * times) > 0)
        {
            var result = parseInt($(numIDx).val() * times);
            if (result < 1)
            {
                $("#totalvalue").text(parseInt($("#totalvalue").text()) + parseInt("1") - parseInt($(numIDx).val()));
                $(numIDx).val("1");
            } else if (result.toString().length > 9)
            {
                var res = result.toString().substr(0, 9);
                $("#totalvalue").text(parseInt($("#totalvalue").text()) + parseInt(res) - parseInt($(numIDx).val()));
                $(numIDx).val(res);
            } else
            {
                $("#totalvalue").text(parseInt($("#totalvalue").text()) + result - parseInt($(numIDx).val()));
                $(numIDx).val(result);
            }
        }
    }
}

function loadpage(type, expect, betid)
{
    if (type == "bett" || type == "bettrecords" || type == "models" || type == "autobett")
    {
        if (uid <= 0)
        {
            layer.alert("<div style='text-align:center;'>请登录</div>", { title: "提示" });
            return;
        }
    }
    isbett = 0;
    switch (type)
    {
        case 'lottery':
            $(".lot_content").load("/nwlottery/_index", { "type": lotterytype, "page": 1 });
            break;
        case 'bett':
            isbett = 1;
            if (lotterytype == 13)
            {
                $(".temp_content").load("/nwlottery/_bettpagelhc", { "type": lotterytype, "expect": expect });
            } else
            {
                $(".temp_content").load("/nwlottery/_bettpage", { "type": lotterytype, "expect": expect });
            }

            break;
        case 'rule':
            $(".temp_content").load("/nwlottery/_ltrule", { "type": lotterytype });
            break;
        case 'bettrecords':
            $(".temp_content").load("/nwlottery/_bettrecord", { "type": lotterytype, "page": 1 });
            break;
        case 'models':
            if (lotterytype == 13)
            {
                $(".temp_content").load("/nwlottery/_bettmodelhc", { "type": lotterytype });
            } else
            {
                $(".temp_content").load("/nwlottery/_bettmode", { "type": lotterytype });
            }
            break;
        case 'autobett':
            $(".temp_content").load("/nwlottery/_autobett", { "type": lotterytype });
            break;
        case 'details':
            if (lotterytype == 13)
            {
                $(".temp_content").load("/nwlottery/_bettdetailslhc", { "type": lotterytype, "bettid": betid });
            } else
            {
                $(".temp_content").load("/nwlottery/_bettdetails", { "type": lotterytype, "bettid": betid });
            }
            break;

    }
}

