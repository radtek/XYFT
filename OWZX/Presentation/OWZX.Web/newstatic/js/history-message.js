$(function(){
	$(".mid-hm .hm-time .lot span").each(function(i,v){
					if( $(v).text() == 1){
						$(this).css("background-color","#EFAF03");
					}else if( $(v).text() == 2){
						$(this).css("background-color","#2792F8");
					}else if( $(v).text() == 3){
						$(this).css("background-color","#5D5D5D");
					}else if( $(v).text() == 4){
						$(this).css("background-color","#F16E0C");
					}else if( $(v).text() == 5){
						$(this).css("background-color","#38C8DA");
					}else if( $(v).text() == 6){
						$(this).css("background-color","#4B05F9");
					}else if( $(v).text() == 7){
						$(this).css("background-color","#AEAEAE");
					}else if( $(v).text() == 8){
						$(this).css("background-color","#F02624");
					}else if( $(v).text() == 9){
						$(this).css("background-color","#C11208");
					}else if( $(v).text() == 10){
						$(this).css("background-color","#27B309");
					}
				})
})