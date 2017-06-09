$(function(){
				$(".mid-table tbody .tr td li").each(function(i,v){
					if( $(v).text() == 1){
						$(this).css("color","#EFAF03");
					}else if( $(v).text() == 2){
						$(this).css("color","#2792F8");
					}else if( $(v).text() == 3){
						$(this).css("color","#5D5D5D");
					}else if( $(v).text() == 4){
						$(this).css("color","#F16E0C");
					}else if( $(v).text() == 5){
						$(this).css("color","#38C8DA");
					}else if( $(v).text() == 6){
						$(this).css("color","#6022F8");
					}else if( $(v).text() == 7){
						$(this).css("color","#AEAEAE");
					}else if( $(v).text() == 8){
						$(this).css("color","#F02624");
					}else if( $(v).text() == 9){
						$(this).css("color","#C11208");
					}else if( $(v).text() == 10){
						$(this).css("color","#27B309");
					}
				})
				$(".mid-table table tbody tr td:nth-child(4)").each(function(i,v){
					if($(v).text() == "小"){
						$(this).css('color','dodgerblue');
					}else if($(v).text() == "大"){
						$(this).css("color","red");
					}
				})
				$(".mid-table table tbody tr td:nth-child(5)").each(function(i,v){
					if($(v).text() == "单"){
						$(this).css('color','dodgerblue');
					}else if($(v).text() == "双"){
						$(this).css("color","red");
					}
				})
				$(".mid-table table tbody tr td:nth-child(6),.mid-table table tbody tr td:nth-child(7),.mid-table table tbody tr td:nth-child(8),.mid-table table tbody tr td:nth-child(9),.mid-table table tbody tr td:nth-child(10)").each(function(i,v){
					if($(v).text() == "龙"){
						$(this).css('color','dodgerblue');
					}else if($(v).text() == "虎"){
						$(this).css("color","red");
					}
				})
			})