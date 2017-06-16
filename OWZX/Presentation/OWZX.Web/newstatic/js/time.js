	 


function timerInterval(intDiff, callback) {
    tiemsmange = window.setInterval(function () { 
		var day=0,
			hour=0,
			minute=0,
			second=0;//时间默认值        
		if(intDiff > 0){
			day = Math.floor(intDiff / (60 * 60 * 24));
			hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
			minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
			second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
		}
		if (minute <= 9) minute = '0' + minute;
		if (second <= 9) second = '0' + second;
				    
		//等待开奖结果
		if(minute <= 0 && second <= 0){
			$('#time_show').css('display','none');
			$('#now').css('display','inline-block');
		}else{
			$('#time_show').css('display','inline-block');
			$('#now').css('display','none');
		}
    
		if (intDiff == -30) {
		    console.log(intDiff);
		    if (!!callback && typeof callback === "function")
		        callback();
		}
		$('#time_show').html('<s></s>'+minute + ':' + second);
		intDiff--;
		}, 1000);
}  