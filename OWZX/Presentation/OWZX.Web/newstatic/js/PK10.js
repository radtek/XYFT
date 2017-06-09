$(function() {
	$(".cut-choice li:first-child").click(function() {
		$(".chip-set").toggle();
		$(".rule,.bet").css("display", "none");
	})
	$(".close").click(function() {
		$(".chip-set").css("display", "none");
	})
	$(".cut-choice li:nth-child(2)").click(function() {
		$(".rule").toggle();
		$(".chip-set,.bet").css("display", "none");
	})
	$(".rule-close").click(function() {
		$(".rule").css("display", "none");
	})
	$(".cut-choice li:last-child").click(function() {
		$(".bet").toggle();
		$(".chip-set,.rule").css("display", "none");
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
			// var txt = $(v).html();
			// var html = " ";
			// if($(v).is(".cl")) {
			// 	$(this).removeClass("cl");
			// 	$(".chip-choice span").each(function(i, c) {
			// 		if($(v).text() == $(c).text()) {
			// 			$(c).remove();
			// 		}
			// 	})
			// 	$(".chip-choice").children(txt).remove();
			// } else {
			// 	$(this).addClass("cl");
			// 	html += '<span class="cl1">' + txt + '</span>';
			// 	$(".chip-choice").append(html);
			// }
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
	$(".define").click(function(){
		if ($(".cl").length<4) {
			alert("请选择4个金额设置筹码。");
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
        if (flag==1) {//初次点击车号
            //显示待选数字
            $(".btn-toolbar").removeClass("cur");
            $(_this).addClass("cur");
            $("#transport-options-p,.arrow").hide();
            $("#transport-options-o,.arrow").show();
            $("#transport-options-o").removeClass("add");
            /*$(".arrow").css({"top":_this.offset().top-273,"left":_this.offset().left+_this.width()/4,"border-top":"10px solid #222","border-bottom":"none"});
			if (_this.offset().left/$(document).width()<0.3) {
				$("#transport-options-o").css({"left":_this.offset().left,"top":_this.offset().top-304,"right":"auto"});
			}else if(_this.offset().left/$(document).width()>0.7){
				$("#transport-options-o").css({"right":$(document).width()-_this.offset().left-_this.width(),"top":_this.offset().top-304,"left":"auto"});
			}else{
				$("#transport-options-o").css({"left":_this.offset().left-100,"top":_this.offset().top-304,"right":"auto"});
			}*/

			if (_this.siblings(".one").text()=="跑第一名：") {
                $(".arrow").css({"position":"absolute","top":_this.offset().top-223,"left":_this.offset().left+_this.width()/4,"border-bottom":"10px solid #222","border-top":"none"}).show();
				if (_this.offset().left/$(document).width()<0.3) {
					$("#transport-options-o").css({"left":_this.offset().left,"top":_this.offset().top-215,"right":"auto"});
				}else if(_this.offset().left/$(document).width()>0.7){
					$("#transport-options-o").css({"right":$(document).width()-_this.offset().left-_this.width(),"top":_this.offset().top-215,"left":"auto"});
				}else{
					$("#transport-options-o").css({"left":_this.offset().left-100,"top":_this.offset().top-215,"right":"auto"});
				}
			}else{      
				$(".arrow").css({"position":"absolute","top":_this.offset().top-272,"left":_this.offset().left+_this.width()/4,"border-top":"10px solid #222","border-bottom":"none"}).show();
				if (_this.offset().left/$(document).width()<0.3) {
					$("#transport-options-o").css({"left":_this.offset().left,"top":_this.offset().top-304,"right":"auto"});
				}else if(_this.offset().left/$(document).width()>0.7){
					$("#transport-options-o").css({"right":$(document).width()-_this.offset().left-_this.width(),"top":_this.offset().top-304,"left":"auto"});
				}else{
					$("#transport-options-o").css({"left":_this.offset().left-100,"top":_this.offset().top-304,"right":"auto"});
				}
			}

        }else if (flag==2) {//已投注后点击车号
            //显示撤销和加注
            $("#transport-options-o,.arrow").hide();
            $("#transport-options-p").show();
            $(".btn-toolbar").removeClass("cur");
            $(_this).addClass("cur");
            /*$(".arrow").css({"position":"absolute","top":_this.offset().top-272,"left":_this.offset().left+_this.width()/4,"border-top":"10px solid #222","border-bottom":"none"}).show();
			if (_this.offset().left/$(document).width()<0.5) {
				$("#transport-options-p").css({"left":_this.offset().left,"top":_this.offset().top-304,"right":"auto"});
			}else if(_this.offset().left/$(document).width()>0.5){
				$("#transport-options-p").css({"right":$(document).width()-_this.offset().left-_this.width(),"top":_this.offset().top-304,"left":"auto"});
			}*/
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
		$(this).click(function(){
			if (!$(this).parent().hasClass("add")) {   //投注数目于左上角
                $(".cur .bar-a").text($(this).find("i").text());
				$("#transport-options-o,.arrow").hide();
				$(".cur").unbind().bind("click",function(){
					btnToolbar(2,$(this));
				})
			}else{   //加注数目于左上角
                $(".cur .bar-a").text(parseInt($(this).find("i").text())+parseInt($(".cur .bar-a").text()));
                $(this).parent().removeClass("add").hide();
                $(".arrow").hide();
			}
		})
	})

	$(".b").eq(0).click(function(){    //撤销投注
        $(".cur .bar-a").text("");
        $("#transport-options-p,.arrow").hide();
        $(".cur").unbind().bind("click",function(){
			btnToolbar(1,$(this));
		})
		$(".cur").removeClass("cur");
	})
	
	$(".b").eq(1).click(function(){    //加注投注
        //点击加注，弹出投注数目框，并给框加类add
        btnToolbar(1,$(".cur"));
        $("#transport-options-o").addClass("add");
	})
		
	
		/*$(".a").each(function(i, v) {
			$(v).click(function(){
				var $a = $(this).text();
				var $div = $('div.pressed');
				var html = " ";
				html += '<a class="bar-a"> '+ $a +'</a>';
				$div.prepend(html);
				$div.removeClass('pressed');
				if($a != ''){
					$('.tool-container .a ,.btn').addClass("hidden");
					$('.tool-container .b').removeClass("hidden");
				}
			})
		})*/
		/*$(".b").each(function(i, v) {
			$(v).click(function(){
				zttz=1;
				if(i==0)
				{
					//撤销
					var $div = $('div.pressed');
					var html = " ";
					$div.prepend(html);
					$('.tool-container .a ,.btn').removeClass("hidden");
					$('.tool-container .b').addClass("hidden");
				}
				else
				{
					//加注
					$('.tool-container .a ,.btn').removeClass("hidden");
					$('.tool-container .b').addClass("hidden");
				}
			})
		})*/
	$(".btn").click(function() {
		var tops=$(this).parent().css("top").replace("px","")-$(".self").css("height").replace("px","");
		/*console.log($('#transport-options-o').css("top").replace("px",""));
		console.log($(".self").css("height").replace("px",""));
		console.log(tops);*/
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
})