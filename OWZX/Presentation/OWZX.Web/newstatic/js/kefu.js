 window.alert = function (msg, type, url) {
        $("#window_alert").remove();
        var _alter = $("<div id='window_alert' class='alert'></div>");
        var _wrap = $("<div class='alert-wrap'></div>");
        var _wrapIcon = $("<div class='" + (type == 2 ? "alert-icon-warn" : "alert-icon-right") + " iconfont'></div>"),
            _wrapMsg = $("<div class='alert-msg'></div>").html(msg),
            __wrapClose = $("<div class='alert-close right iconfont'></div>");
        _wrap.append(_wrapIcon).append(_wrapMsg).append(__wrapClose);
        _alter.append(_wrap);
        _alter.appendTo("body");
        var left = $(window).width() / 2 - (_alter.width() / 2);
        _alter.offset({ left: left });
        setTimeout(function () {  _alter.remove(); }, 2000);
    }
    $.extend({
        getUrlVars: function(){
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for(var i = 0; i < hashes.length; i++)
        {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
        },
        getUrlVar: function(name){
        	try{
            	return $.getUrlVars()[name];
            }
	        catch(ex){
	        	return "";
	        }
        }
    });