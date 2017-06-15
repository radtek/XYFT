$(function() {
	$(".cut-choice li:first-child").click(function() {
		$(".chip-set").toggle();
		$(".rule,.bet").css("display", "none");
		$("#transport-options-p,.arrow").hide();
		$("#transport-options-o,.arrow").hide();
        $(".keyboards").hide();
        $(".auto").css("border","none").text("自定义金额");
	})
	
	$(".cut-choice li:nth-child(2)").click(function() {
		$(".rule").toggle();
		$(".chip-set,.bet").css("display", "none");
		$("#transport-options-p,.arrow").hide();
		$("#transport-options-o,.arrow").hide();
        $(".keyboards").hide();
        $(".auto").css("border","none").text("自定义金额");
	})
	$(".rule-close").click(function() {
		$(".rule").css("display", "none");
	})
	$(".cut-choice li:last-child").click(function() {
		$(".bet").toggle();
		$(".chip-set,.rule").css("display", "none");
		$("#transport-options-p,.arrow").hide();
		$("#transport-options-o,.arrow").hide();
        $(".keyboards").hide();
        $(".auto").css("border","none").text("自定义金额");
	})
	$(".bet-close").click(function() {
		$(".bet").css("display", "none");
	})
	//点击任意处关闭
	$(document).click(function() {
		$(".chip-set").hide();
		$(".rule").hide();
		$(".bet").hide();
		$(".self").hide();
		
			$('.tool-container .a ,.btn').removeClass("hidden");
			$('.tool-container .b').addClass("hidden");
		
	});
	$(".chip-set,.rule,.bet,.cut-choice li:first-child,.cut-choice li:nth-child(2),.cut-choice li:last-child,.self,.self-close,.btn").click(function(event) {
		event.stopPropagation();
	});

	//明细金额颜色
	$(".bet-detail li h3 span").each(function(i, v) {
		if($(v).text() < 0) {
			$(this).css("color", "red");
		} else if($(v).text() > 0) {
			$(this).css('color', 'royalblue');
		}
	})
	//========================================================
	//筹码设置
	$("ul.chip-money li span").each(function(i, v) {
		$(v).click(function() {
			if ($(v).is(".cl")) {
				$(v).removeClass("cl");
			}else{
				if ($(".cl").length==4||$(".cl").length>4) {
                    return;
				}else{
					$(v).addClass("cl");
				}
			}

		})
	});
	$(".define,.close").click(function(){
		if ($(".cl").length<4) {
		    tishi("请选择4个金额设置筹码。");
		}else{
			var array1=[];
			for (var i = 0; i < 4; i++) {
				array1[i]=$(".cl").eq(i).text();
			}
			
			for (var j = 0; j < 4; j++) {
				$("#transport-options-o>a i").eq(j).text(array1[j]);
			}
			$(".chip-set").hide();
		}
		

	})
	//====================================================================
	//pk10 投注

    function btnToolbar(flag,_this){
    	$(".keyboards").hide();
    	$(".auto").css("border","none").text("自定义金额");
        if (flag==1) {//初次点击车号
            //显示待选数字
            $(".btn-toolbar").removeClass("cur");
            $(_this).addClass("cur");
            $("#transport-options-p,.arrow").hide();
            $("#transport-options-o,.arrow").show();
            $("#transport-options-o").removeClass("add");

			if (_this.siblings(".one").text()=="跑第一名：") {
                $(".arrow").css({"position":"absolute","top":_this.offset().top-223,"left":_this.offset().left+_this.width()/4,"border-bottom":"10px solid #222","border-top":"none"}).show();
				if (_this.offset().left/$(document).width()<0.2) {
					$("#transport-options-o").css({"left":_this.offset().left,"top":_this.offset().top-215,"right":"auto"});
				}else if(_this.offset().left/$(document).width()>0.6){
					$("#transport-options-o").css({"right":$(document).width()-_this.offset().left-_this.width(),"top":_this.offset().top-215,"left":"auto"});
				}else{
					$("#transport-options-o").css({"left":_this.offset().left-90,"top":_this.offset().top-215,"right":"auto"});
				}
			}else{      
				$(".arrow").css({"position":"absolute","top":_this.offset().top-272,"left":_this.offset().left+_this.width()/4,"border-top":"10px solid #222","border-bottom":"none"}).show();
				if (_this.offset().left/$(document).width()<0.2) {
					$("#transport-options-o").css({"left":_this.offset().left,"top":_this.offset().top-304,"right":"auto"});
				}else if(_this.offset().left/$(document).width()>0.6){
					$("#transport-options-o").css({"right":$(document).width()-_this.offset().left-_this.width(),"top":_this.offset().top-304,"left":"auto"});
				}else{
					$("#transport-options-o").css({"left":_this.offset().left-90,"top":_this.offset().top-304,"right":"auto"});
				}
			}

        }else if (flag==2) {//已投注后点击车号
            //显示撤销和加注
            $("#transport-options-o,.arrow").hide();
            $("#transport-options-p").show();
            $(".btn-toolbar").removeClass("cur");
            $(_this).addClass("cur");
			if (_this.siblings(".one").text()=="跑第一名：") {
                $(".arrow").css({"position":"absolute","top":_this.offset().top-223,"left":_this.offset().left+_this.width()/4,"border-bottom":"10px solid #222","border-top":"none"}).show();
				if (_this.offset().left/$(document).width()<0.5) {
					$("#transport-options-p").css({"left":_this.offset().left,"top":_this.offset().top-215,"right":"auto"});
				}else if(_this.offset().left/$(document).width()>0.5){
					$("#transport-options-p").css({"right":$(document).width()-_this.offset().left-_this.width(),"top":_this.offset().top-215,"left":"auto"});
				}	    
			}else{      
				$(".arrow").css({"position":"absolute","top":_this.offset().top-272,"left":_this.offset().left+_this.width()/4,"border-top":"10px solid #222","border-bottom":"none"}).show();
				if (_this.offset().left/$(document).width()<0.5) {
					$("#transport-options-p").css({"left":_this.offset().left,"top":_this.offset().top-304,"right":"auto"});
				}else if(_this.offset().left/$(document).width()>0.5){
					$("#transport-options-p").css({"right":$(document).width()-_this.offset().left-_this.width(),"top":_this.offset().top-304,"left":"auto"});
				}	
			}
        }
	}

	$(".btn-toolbar").click(function(flag,_this){
        btnToolbar(1,$(this));
	});

	$(".a").each(function(){
	    $(this).click(function ()
	    {
	        if (uid == -1)
	        {
	            window.location.href = "/account/login";
	            return;
	        }
		    $(".keyboards").hide();
		    var atmoney; var bettype = ''; var room;
		    if (!$(this).parent().hasClass("add"))
		    {   //投注数目于左上角

		        atmoney = $(this).find("i").text();
		        bettype = $(".cur .bar-a").parent().find("i").data('type');
		        room = $(".cur .bar-a").parent().find("i").data('num');
                $(".cur .bar-a").text($(this).find("i").text());
				$("#transport-options-o,.arrow").hide();
				$(".cur").unbind().bind("click",function(){
					btnToolbar(2,$(this));
				})
			}else{   //加注数目于左上角
				if (parseInt($(this).find("i").text())+parseInt($(".cur .bar-a").text())>20000) {
					tishi("单注金额最高20000！");
				} else
				{
				    atmoney = $(this).find("i").text();
				    bettype = $(".cur .bar-a").parent().find("i").data('type');
				    room = $(".cur .bar-a").parent().find("i").data('num');
					$(".cur .bar-a").text(parseInt($(this).find("i").text())+parseInt($(".cur .bar-a").text()));
	                $(this).parent().removeClass("add").hide();
	                $(".arrow").hide();
				}
		    }

		    if (bettype != '' && uid > 0)
		    {
		        bett(room,atmoney, bettype, lotterytype, expect)
		    }
		})
	})

	$(".b").eq(0).click(function ()
	{    //撤销投注
	    if (uid == -1)
	    {
	        window.location.href = "/account/login";
	        return;
	    }
	    bettype = $(".cur .bar-a").parent().find("i").data('type');
	    room = $(".cur .bar-a").parent().find("i").data('num');
	    delbet(room,bettype, lotterytype, expect,$(".cur .bar-a").text());
	    
        $(".cur .bar-a").text("");
        $("#transport-options-p,.arrow").hide();
        $(".cur").unbind().bind("click",function(){
			btnToolbar(1,$(this));
		})
		$(".cur").removeClass("cur");
	})
	
	$(".b").eq(1).click(function ()
	{    //加注投注
	    if (uid == -1)
	    {
	        window.location.href = "/account/login";
	        return;
	    }
        //点击加注，弹出投注数目框，并给框加类add
        btnToolbar(1,$(".cur"));
        $("#transport-options-o").addClass("add");
	})
		
	$(".btn").click(function() {
		var tops=$(this).parent().css("top").replace("px","")-$(".self").css("height").replace("px","");
		if (tops>10) {
			$(".self").toggle().css("top",tops);
		}else{
			$(".self").toggle().css("top",10);
		}
         $("#transport-options-o,.arrow").hide();
	})
	$(".self-close").click(function(){
		$(".self").hide();
	})

	$(".auto").click(function ()
	{  //自定义金额
	    if (uid == -1)
	    {
	        window.location.href = "/account/login";
	        return;
	    }
		$(".auto").text("").css("border","1px solid #666");
		if($(".cur").siblings(".one").text()=="冠亚和："){
            $(".keyboards").show().css({"top":$("#transport-options-o").offset().top-409,"left":$("#transport-options-o").offset().left+$("#transport-options-o").width()-$(".keyboards").width()+6});
		}else{
			$(".keyboards").show().css({"top":$("#transport-options-o").offset().top-224,"left":$("#transport-options-o").offset().left+$("#transport-options-o").width()-$(".keyboards").width()+6});
		}

	})

	$(".keyboards td").click(function ()
	{
	    if (uid == -1)
	    {
	        window.location.href = "/account/login";
	        return;
	    }
		if ($(this).text()!="删除"&&$(this).text()!="确定") {
			if ($(this).text()==0 && $(".auto").text()=="") {				 
                    return; 
			}else{
            	$(".auto").text($(".auto").text()+$(this).text());
			}
		}else{
			if ($(this).text()=="删除") {
                var str=$(".auto").text();
                var len=str.split("").length;
                str=str.substring(0,len-1);
                $(".auto").text(str);
			}else if ($(this).text()=="确定") {
				if ($(".auto").text()>20000) {
	            	tishi("单注金额最高20000！");
				} else
				{
				    var atmoney; var bettype = ''; var room;
					if ($("#transport-options-o").hasClass("add")) {
			            //显示输入框，输入金额确定后与左上角数字相加
			            if (parseInt($(".auto").text())+parseInt($(".cur .bar-a").text())>20000) {
			                tishi("单注金额最高20000！");
			            	return false;
			            }else{
			                if ($(".auto").text() != "")
			                {
			                    atmoney = parseInt($(".auto").text());
			                    bettype = $(".cur .bar-a").parent().find("i").data('type');
			                    room = $(".cur .bar-a").parent().find("i").data('num');
				            	$(".cur .bar-a").text(parseInt($(".auto").text())+parseInt($(".cur .bar-a").text()));
				            	$(".cur").unbind().bind("click",function(){
									btnToolbar(2,$(this));
								})	 
			                } else
			                {
				            	$(".cur .bar-a").text(parseInt($(".cur .bar-a").text()));
				            }
				            $(".keyboards").hide();
			            }	            
					}else{
			            //显示输入框，输入金额确定后显示数字于左上角
			            $(".keyboards").hide();
			            if ($(".auto").text() != "")
			            {
			                atmoney = parseInt($(".auto").text());
			                bettype = $(".cur .bar-a").parent().find("i").data('type');
			                room = $(".cur .bar-a").parent().find("i").data('num');
			            	$(".cur .bar-a").text(parseInt($(".auto").text()));
			            	$(".cur").unbind().bind("click",function(){
								btnToolbar(2,$(this));
							})	 
			            }else{
			            	$(".cur .bar-a").text("");
			            }
					}	
					$("#transport-options-o,.arrow").hide();
					$(".auto").css("border", "none").text("自定义金额");
					if (bettype != '' && uid>0)
					{
					    bett(room,atmoney, bettype, lotterytype, expect)
					}
	            }
			}
		}
	})
})
function bett(roomid,money, type, lotterytype, expect)
{
    layer.load(2);
    $.post("/nwlottery/bett", { "btroom": roomid, "expect": expect, "money": money, "lotterytype": lotterytype, "bttype": type }, function (data)
    {
        setTimeout(function ()
        {
            layer.closeAll('loading');
        }, 2000)
        var dt = JSON.parse(data);
        if (dt.state == "success")
        {
            layer.msg("投注成功");
            //重新加载footer中数据
            ldfoot();
        } else
        {
            var nwmy = $(".cur .bar-a").text();
            if (nwmy > money)
            {
                $(".cur .bar-a").text(nwmy - money);
            } else
            {
                $(".cur .bar-a").text("");
            }
            tishi(dt.biz_content, { "title": "提示", icon: 2 });
        }
    })
}

function delbet(roomid, type, lotterytype, expect,money)
{
    layer.load(2);
    $.post("/nwlottery/delbett", { "btroom": roomid, "expect": expect, "lotterytype": lotterytype, "bttype": type }, function (data)
    {
        setTimeout(function ()
        {
            layer.closeAll('loading');
        }, 2000)
        var dt = JSON.parse(data);
        if (dt.state == "success")
        {
            layer.msg("撤销成功");
            //重新加载footer中数据
            ldfoot();
        } else
        {
            $(".cur .bar-a").text(money);
            tishi(dt.biz_content, { "title": "提示", icon: 2 });
        }
    })
}

function ldfoot()
{
    $.get("/nwlottery/userbetmoney", { "lotterytype": lotterytype, "expect": expect }, function (data)
    {
        if (data != "")
        {
            var my = data.split('|');
            $(".bala-money").text(my[0]);
            $(".bet-money").text(my[1]);
        }
    });
}

    