var or = 160;
var ir = 50;
var mWidth = 60;
var mDLen = Math.sqrt(2 * Math.pow(mWidth,2));
var width = document.body.clientWidth/2;
var height = document.body.clientHeight/2;
var wd = document.body.clientWidth;
if (height>300){
	height = height - 20;
	$("#bg").css("margin-top","90px");
}else{
	height = 290 ;
	$("#bg").css('margin-top',"70px");
}
if(wd>400){
	$("#bg").css('margin-top',"150px");
}
//第1菜单块中心点与以o(width,width)为圆心的的X轴的夹角为-90(-PI/2), 求菜单块中心点坐标
var m1X = parseInt( (Math.cos( -1 * Math.PI / 2 ) * (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + width - mWidth/2 );
var m1Y = parseInt( (Math.sin( -1 * Math.PI / 2 ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + height - mWidth/2 );
$("#m1").width(mWidth);
$("#m1").height(mWidth);
$("#m1").offset( {top:m1Y,left:m1X} );

//第2菜单块中心点与以o(width,width)为圆心的的X轴的夹角为-45(-PI/4), 求菜单块中心点坐标
var m2X = parseInt( (Math.cos( -1 * Math.PI / 4 ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + width - mWidth/2 );
var m2Y = parseInt( (Math.sin( -1 * Math.PI / 4 ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + height - mWidth/2 );
$("#m2").width(mWidth);
$("#m2").height(mWidth);
$("#m2").offset( {top:m2Y,left:m2X} );

//第3菜单块中心点与以o(width,width)为圆心的的X轴的夹角为0(0), 求菜单块中心点坐标
var m3X = parseInt( (Math.cos( 0 ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + width - mWidth/2 );
var m3Y = parseInt( (Math.sin( 0 ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + height - mWidth/2 );
$("#m3").width(mWidth);
$("#m3").height(mWidth);
$("#m3").offset( {top:m3Y,left:m3X} );

//第4菜单块中心点与以o(width,width)为圆心的的X轴的夹角为45(PI/4), 求菜单块中心点坐标
var m4X = parseInt( (Math.cos( Math.PI / 8 ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + width - mWidth/2 );
var m4Y = parseInt( (Math.sin( Math.PI / 8 ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + height - mWidth/2 );
$("#m4").width(mWidth);
$("#m4").height(mWidth);
$("#m4").offset( {top:m4Y,left:m4X} );

//第5菜单块中心点与以o(width,width)为圆心的的X轴的夹角为90(PI/2), 求菜单块中心点坐标
var m5X = parseInt( (Math.cos( Math.PI / 2 ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + width - mWidth/2 );
var m5Y = parseInt( (Math.sin( Math.PI / 2 ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + height - mWidth/2 );
$("#m5").width(mWidth);
$("#m5").height(mWidth);
$("#m5").offset( {top:m5Y,left:m5X} );

//第6菜单块中心点与以o(width,width)为圆心的的X轴的夹角为135(0.75PI), 求菜单块中心点坐标
var m6X = parseInt( (Math.cos( 0.85 * Math.PI ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + width - mWidth/2 );
var m6Y = parseInt( (Math.sin( 0.85 * Math.PI ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + height - mWidth/2 );
$("#m6").width(mWidth);
$("#m6").height(mWidth);
$("#m6").offset( {top:m6Y,left:m6X} );

//第7菜单块中心点与以o(width,width)为圆心的的X轴的夹角为180(PI), 求菜单块中心点坐标
var m7X = parseInt( (Math.cos( Math.PI ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + width - mWidth/2 );
var m7Y = parseInt( (Math.sin( Math.PI ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + height - mWidth/2 );
$("#m7").width(mWidth);
$("#m7").height(mWidth);
$("#m7").offset( {top:m7Y,left:m7X} );

//第8菜单块中心点与以o(width,width)为圆心的的X轴的夹角为-135(PI), 求菜单块中心点坐标
var m8X = parseInt( (Math.cos( -0.75 * Math.PI ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + width - mWidth/2 );
var m8Y = parseInt( (Math.sin( -0.75 * Math.PI ) *  (ir + ((or - ir - mDLen)/2) + mDLen/2) ) + height - mWidth/2 );
$("#m8").width(mWidth);
$("#m8").height(mWidth);
$("#m8").offset( {top:m8Y,left:m8X} );

//===============================================

var preX,preY;//上一次鼠标点的坐标
var curX,curY;//本次鼠标点的坐标
var preAngle;//上一次鼠标点与圆心(width,width)的X轴形成的角度(弧度单位)
var transferAngle;//当前鼠标点与上一次preAngle之间变化的角度

var a = 0;

for(var i = 0 ; i < 15 ; i++){
	$("body").append("<br>");
}

//点击事件
$("#outerDiv").mousedown(function(event){
	preX = event.clientX;
	preY = event.clientY;
	//计算当前点击的点与圆心(width,width)的X轴的夹角(弧度) --> 上半圆为负(0 ~ -180), 下半圆未正[0 ~ 180]
	preAngle = Math.atan2(preY - height, preX - width);
	//移动事件
	$("html").mousemove(function(event){
		curX = event.clientX;
		curY = event.clientY;
		//计算当前点击的点与圆心(width,width)的X轴的夹角(弧度) --> 上半圆为负(0 ~ -180), 下半圆未正[0 ~ 180]
		var curAngle = Math.atan2(curY - height, curX - width);
		transferAngle = curAngle - preAngle;
		a += (transferAngle * 180 / Math.PI);
		$('#outerDiv').rotate(a);
		
		for( var i = 1 ; i <= 8 ; i++ ){
$("#m"+i).rotate(-a);
		}
		preX = curX;
		preY = curY;
		preAngle = curAngle;
	});
	//释放事件
	$("html").mouseup(function(event){
		$("html").unbind("mousemove");
	});
});
		    //触屏事件
$("#outerDiv").on("touchstart",function(event){
	preX = event.clientX;
	preY = event.clientY;
	//计算当前点击的点与圆心(width,width)的X轴的夹角(弧度) --> 上半圆为负(0 ~ -180), 下半圆未正[0 ~ 180]
	preAngle = Math.atan2(preY - height, preX - width);
	//移动事件
	$("html").on("touchmove",function(event){
		curX = event.clientX;
		curY = event.clientY;
		//计算当前点击的点与圆心(width,width)的X轴的夹角(弧度) --> 上半圆为负(0 ~ -180), 下半圆未正[0 ~ 180]
		var curAngle = Math.atan2(curY - height, curX - width);
		transferAngle = curAngle - preAngle;
		a += (transferAngle * 180 / Math.PI);
		$('#outerDiv').rotate(a);
		
		for( var i = 1 ; i <= 8 ; i++ ){
$("#m"+i).rotate(-a);
		}
		preX = curX;
		preY = curY;
		preAngle = curAngle;
	});
	//释放事件
	$("html").on("touchend",function(event){
		$("html").unbind("touchmove");
	});
});