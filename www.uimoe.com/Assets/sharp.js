var sharp = {
    textbox: {
        show: function (id) {
            var item = sharp.id(id);
            if (!item) {
                return;
            }

            var itype = item.getAttribute("type");
            if (itype != "text") {
                return;
            }

            var iregex = item.getAttribute("data-regex");
            if (iregex) {
                item.setAttribute("onchange", "sharp.textboxtextchanged(this)");
            }

            var idescription = item.getAttribute("data-description");
            if (idescription) {
                item.setAttribute("onmouseenter", "sharp.textboxmouseenter(this,event)");
                item.setAttribute("onmouseleave", "sharp.textboxmouseleave(this,event)");
            }
        }
    },
    textboxtextchanged: function (sender) {
        var ivalue = sender.value;
        var iclass = sender.getAttribute("class");
        var iregex = sender.getAttribute("data-regex");
        var rgx = new RegExp(iregex);
        var passed = rgx.test(ivalue);
        if (passed) {
            iclass = iclass.replace("-error", "");
            sender.setAttribute("class", iclass);
        }
        else {
            iclass = iclass.replace("-red", "").replace("-green", "").replace("-blue", "").replace("-purple", "").replace("-error", "");
            sender.setAttribute("class", iclass + "-error");
        }
    },
    textboxmouseenter: function (sender, evt) {
        var iclass = sender.getAttribute("class");
        if (iclass.indexOf("textbox-error") < 0) {
            return;
        }

        var tooltip = sharp.id("tooltipdiv");
        if (tooltip) {
            tooltip.innerHTML = "";
        }
        else {
            tooltip = document.createElement("div");
            tooltip.setAttribute("id", "tooltipdiv");
            document.body.appendChild(tooltip);
        }

        tooltip.setAttribute("class", "tooltip-red");
        var xy = sharp.locationxy(sender);
        var ht = 45;
        if (xy.y < ht) {
            xy.y += ht;
        }
        else {
            xy.y -= ht;
        }

        tooltip.style.left = xy.x + "px";
        tooltip.style.top = xy.y + "px";

        var idescription = sender.getAttribute("data-description");
        if (idescription) {
            tooltip.innerHTML = idescription;
        }
    },
    textboxmouseleave: function (sender, evt) {
        var tooltip = document.getElementById("tooltipdiv");
        if (tooltip) {
            document.body.removeChild(tooltip);
        }
    },
    mouseposition: function (evt) {
        var e = evt || event;
        var scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
        var scrollY = document.documentElement.scrollTop || document.body.scrollTop;
        var x = e.pageX || e.clientX + scrollX;
        var y = e.pageY || e.clientY + scrollY;
        return { x: x, y: y };
    },
    locationxy: function (element) {
        var oparent = element.offsetParent;
        var oleft = element.offsetLeft;
        var otop = element.offsetTop;
        while (oparent) {
            oleft += oparent.offsetLeft;
            otop += oparent.offsetTop;
            oparent = oparent.offsetParent;
        }
        return { x: oleft, y: otop };
    },
    locationx: function (id) {
        var xy = sharp.locationxy(id);
        return xy.x;
    },
    locationy: function (id) {
        var xy = sharp.locationxy(id);
        return xy.y;
    },
    resizeinput: function (ids, wt) {
        for (var i = 0; i < ids.length; i++) {
            var item = sharp.id(ids[i]);
            if (item) {
                var width = document.documentElement.clientWidth - wt;
                item.style.width = width + "px";
            }
        }
    },
    center: function (id) {
        var container = sharp.id(id);
        if (!container) {
            return;
        }

        var ht = document.documentElement.clientHeight;
        var wt = document.documentElement.clientWidth;

        var ht2 = parseInt(container.style.height);
        var wt2 = parseInt(container.style.width);

        container.style.position = "fixed";
        container.style.left = 0.5 * (wt - wt2) + "px";
        container.style.top = 0.5 * (ht - ht2) + "px";
    },
    id: function (id) {
        return document.getElementById(id);
    },
    name: function (name) {
        return document.getElementsByName(name);
    },
    tag: function (tag) {
        return document.getElementsByTagName(tag);
    },
    classname: function (classname) {
        return document.getElementsByClassName(classname);
    },
    get: function (url, callback, before, error) {
        if (before && typeof (before) == "function") {
            before();
        }

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    if (callback && typeof (callback) == "function") {
                        callback(xhr.responseText);
                        return;
                    }
                }
            }

            if (error && typeof (error) == "function") {
                error();
            }
        }
        xhr.send();
    },
    post: function (url, data, callback, before, error) {
        if (before && typeof (before) == "function") {
            before();
        }

        var xhr = new XMLHttpRequest();
        xhr.open("POST", url);
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    if (callback && typeof (callback) == "function") {
                        callback(xhr.responseText);
                        return;
                    }
                }
            }

            if (error && typeof (error) == "function") {
                error();
            }
        }

        var tempstr = '';
        for (var i in data) {
            tempstr += encodeURI(i) + "=" + encodeURI(data[i]) + "&";
        }

        tempstr = tempstr.substr(0, tempstr.length - 1);
        xhr.send(tempstr);
    },
    uploadfile: function (id, url, callback, before, error) {
        if (before && typeof (before) == "function") {
            before();
        }

        var xhr = new XMLHttpRequest();
        xhr.open("POST", url);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    if (callback && typeof (callback) == "function") {
                        callback(xhr.responseText);
                        return;
                    }
                }
            }

            if (error && typeof (error) == "function") {
                error();
            }
        }

        var file1 = sharp.id(id);
        var data = new FormData();
        data.append("file1", file1.files[0]);
        xhr.send(data);
    },
    val: function (str) {
        if (!str) {
            return '';
        }

        return str;
    },
    serialize: function (id) {
        var data = {};
        var form1 = sharp.id(id);
        if (!form1) {
            return data;
        }

        var nodes1 = form1.childNodes;
        for (var i1 = 0; i1 < nodes1.length; i1++) {
            var n1 = nodes1[i1];
            if (!n1.getAttribute) {
                continue;
            }

            var nname = n1.getAttribute("name");
            if (!nname || nname.length == 0 || sharp.trim(nname).length == 0) {
                continue;
            }

            if (n1.tagName && (n1.tagName.toLowerCase() == "input")) {
                var ntype = n1.getAttribute("type");
                if (ntype && (ntype.toLowerCase() == "text" || ntype.toLowerCase() == "password" || ntype.toLowerCase() == "hidden")) {
                    data[nname] = n1.value;
                    continue;
                }

                if (ntype && (ntype.toLowerCase() == "checkbox")) {
                    var ischecked = n1.getAttribute("checked");
                    if (ischecked) {
                        data[nname] = true;
                    }
                    else {
                        data[nname] = false;
                    }

                    continue;
                }
            }

            if (n1.tagName && n1.tagName.toLowerCase() == "textarea") {
                data[nname] = n1.value;
                continue;
            }

            if (n1.tagName && n1.tagName.toLowerCase() == "select") {
                if (n1.options && n1.selectedIndex) {
                    var option = n1.options[n1.selectedIndex];
                    if (option) {
                        data[nname] = option.value;
                    }
                }

                continue;
            }
        }

        return data;
    },
    requestparams: function (url) {
        if (!url) {
            url = window.location.href;
        }

        var obj = {};
        var parts1 = url.split('?');
        if (parts1.length != 2) {
            return obj;
        }

        var parts2 = parts1[1].split('&');
        for (var i = 0; i < parts2.length; i++) {
            var parts3 = parts2[i].split('=');
            if (parts3.length == 2) {
                obj[parts3[0]] = parts3[1];
            }
        }

        return obj;
    },
    parsedatetimefromjson: function (json) {
        if (!json || sharp.trim(json).length == 0) {
            return "";
        }

        var dt = eval('new ' + json.replace('/', ''));
        return dt;
    },
    datetimestring: function (dt, format) {
        format = format.replace("yyyy", dt.getFullYear());
        format = format.replace("MM", (dt.getMonth() + 1));
        format = format.replace("dd", dt.getDay());
        format = format.replace("HH", dt.getHours());
        format = format.replace("mm", (dt.getMinutes() + 1));
        format = format.replace("ss", dt.getSeconds());
        return format;
    },
    jsontodatetimestring: function (json, format) {
        var dt = sharp.parsedatetimefromjson(json);
        return sharp.datetimestring(dt, format);
    },
    trim: function (str) {
        if (!str) {
            return "";
        }

        return str.replace(/\s/, "")
    },
    open: function (url, target, features, replace) {
        if (!target) {
            target = "_blank";
        }

        if (!features) {
            var ht = 480;
            var wt = 800;
            var left = 0.5 * (screen.width - wt);
            var top = 0.5 * (screen.height - ht);
            features = "height=" + ht + ",width=" + wt + ",top=" + top + ",left=" + left + ",toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no";
        }

        if (!replace) {
            replace = false;
        }

        window.open(url, target, features, replace);
    },
    view: function () {
        if (sharp.isphonerequest()) {
            var url = window.location.href;
            window.location.href = url.replace(".html", "_m.html");
        }
    },
    isphonerequest: function () {
        var ua = navigator.userAgent.toLowerCase();
        if (ua.indexOf("iphone") >= 0) {
            return true;
        }

        if (ua.indexOf("ipad") >= 0) {
            return true;
        }

        if (ua.indexOf("android") >= 0) {
            return true;
        }

        if (ua.indexOf("windows phone") >= 0) {
            return true;
        }

        return false;
    },
    device: function () {
        var ua = navigator.userAgent.toLowerCase();
        if (ua.indexOf("windows nt") >= 0) {
            return "Desktop";
        }

        if (ua.indexOf("iphone") >= 0) {
            return "iPhone";
        }

        if (ua.indexOf("ipad") >= 0) {
            return "iPad";
        }

        if (ua.indexOf("macintosh") >= 0) {
            return "Mac";
        }

        return "";
    },
    os: function () {
        var ua = navigator.userAgent.toLowerCase();
        if (ua.indexOf("windows nt 10.0") >= 0) {
            return "Windows10";
        }

        if (ua.indexOf("windows nt 6.3") >= 0) {
            return "Windows8.1";
        }

        if (ua.indexOf("windows nt 6.2") >= 0) {
            return "Windows8";
        }

        if (ua.indexOf("windows nt 6.1") >= 0) {
            return "Windows7";
        }

        if (ua.indexOf("windows nt 6.0") >= 0) {
            return "Vista";
        }

        if (ua.indexOf("windows nt 5.2") >= 0) {
            return "Server2003";
        }

        if (ua.indexOf("windows nt 5.1") >= 0) {
            return "XP";
        }

        if (ua.indexOf("windows nt 5.0") >= 0) {
            return "Windows2000";
        }

        if (ua.indexOf("iphone") >= 0) {
            return "iOS";
        }

        if (ua.indexOf("ipad") >= 0) {
            return "iOS";
        }

        if (ua.indexOf("android") >= 0) {
            return "Android";
        }

        if (ua.indexOf("windows phone") >= 0) {
            return "Windows Phone";
        }

        if (ua.indexOf("os x") >= 0) {
            return "OS X";
        }

        if (ua.indexOf("solaris") >= 0) {
            return "Solaris";
        }

        if (ua.indexOf("linux") >= 0) {
            return "Linux";
        }

        if (ua.indexOf("unix") >= 0) {
            return "Unix";
        }

        return "";
    },
    browser: function () {
        var ua = navigator.userAgent.toLowerCase();
        if (ua.indexOf("Firefox") >= 0) {
            return "Firefox";
        }

        if (ua.indexOf("Opera") >= 0) {
            return "Opera";
        }

        if (ua.indexOf("Webkit") >= 0) {
            return "Chrome";
        }

        if (ua.indexOf("Trident") >= 0) {
            return "IE";
        }

        return "";
    },
    copyright: {
        show: function (id) {
            var container = sharp.id(id);
            if (!container) {
                return;
            }

            var message = container.innerHTML;
            if (sharp.trim(message).length == 0 && navigator.userAgent.indexOf("Firefox") > 0) {
                container.style.display = "none";
                return;
            }

            message += "&nbsp;建议使用Firefox访问此页面";

            var leftdiv = document.createElement("div");
            leftdiv.style.float = "left";
            leftdiv.innerHTML = "&nbsp;" + message;
            container.appendChild(leftdiv);

            var rightdiv = document.createElement("div");
            rightdiv.style.float = "right";
            rightdiv.style.marginRight = "5px";
            container.appendChild(rightdiv);

            var closespan = document.createElement("span");
            closespan.innerHTML = "x";
            closespan.setAttribute("onclick", "sharp.copyright.close('" + id + "')");
            rightdiv.appendChild(closespan);

            var clearfix = document.createElement("div");
            clearfix.style.clear = "both";
            container.appendChild(clearfix);
        },
        close: function (id) {
            var container = sharp.id(id);
            if (!container) {
                return;
            }

            container.style.display = "none";
        }
    },
    alert: function (message, theme, ht, wt) {
        if (theme == "red" || theme == "green" || theme == "blue" || theme == "purple") {
            theme = "-" + theme;
        }
        else {
            theme = "";
        }

        if (!ht || isNaN(ht)) {
            ht = 320;
        }

        if (!wt || isNaN(wt)) {
            wt = 480;
        }

        var alertmaskdiv = document.getElementById("alertmaskdiv");
        if (!alertmaskdiv) {
            alertmaskdiv = document.createElement("div");
            alertmaskdiv.setAttribute("id", "alertmaskdiv");
        }

        alertmaskdiv.setAttribute("class", "messageboxmask");
        document.body.appendChild(alertmaskdiv);

        var alertdiv = document.getElementById("alertdiv");
        if (!alertdiv) {
            alertdiv = document.createElement("div");
            alertdiv.setAttribute("id", "alertdiv");
        }
        else {
            alertdiv.innerHTML = "";
        }

        alertdiv.setAttribute("class", "messageboxborder" + theme);
        alertdiv.style.width = wt + "px";
        alertdiv.style.height = ht + "px";
        alertdiv.style.left = 0.5 * (document.documentElement.clientWidth - wt) + "px";
        alertdiv.style.top = 0.5 * (document.documentElement.clientHeight - ht) + "px";
        document.body.appendChild(alertdiv);

        var alertcontentdiv = document.createElement("div");
        alertcontentdiv.setAttribute("class", "messageboxcontent" + theme);
        alertcontentdiv.style.width = (wt - 20) + "px";
        alertcontentdiv.style.height = (ht - 75) + "px";
        alertcontentdiv.innerHTML = message;
        alertdiv.appendChild(alertcontentdiv);

        var alertokbutton = document.createElement("input");
        alertokbutton.setAttribute("type", "button");
        alertokbutton.setAttribute("class", "button" + theme);
        alertokbutton.setAttribute("value", "好的");
        alertokbutton.style.position = "relative";
        alertokbutton.style.left = (wt - 80) + "px";
        alertokbutton.style.top = "5px";
        alertokbutton.onclick = sharp.alertreset;
        alertdiv.appendChild(alertokbutton);
    },
    alertreset: function () {
        var alertmaskdiv = document.getElementById("alertmaskdiv");
        if (alertmaskdiv) {
            document.body.removeChild(alertmaskdiv);
        }

        var alertdiv = document.getElementById("alertdiv");
        if (alertdiv) {
            document.body.removeChild(alertdiv);
        }
    },
    getinput: function (callback, title, input, regex, errormessage, theme, ht, wt) {
        if (theme == "red" || theme == "green" || theme == "blue" || theme == "purple") {
            theme = "-" + theme;
        }
        else {
            theme = "";
        }

        if (!ht || isNaN(ht)) {
            ht = 320;
        }

        if (!wt || isNaN(wt)) {
            wt = 480;
        }

        var alertmaskdiv = document.getElementById("alertmaskdiv");
        if (!alertmaskdiv) {
            alertmaskdiv = document.createElement("div");
            alertmaskdiv.setAttribute("id", "alertmaskdiv");
        }

        alertmaskdiv.setAttribute("class", "messageboxmask");
        document.body.appendChild(alertmaskdiv);

        var getinputdiv = document.getElementById("getinputdiv");
        if (!getinputdiv) {
            getinputdiv = document.createElement("div");
            getinputdiv.setAttribute("id", "getinputdiv");
        }
        else {
            getinputdiv.innerHTML = "";
        }

        getinputdiv.setAttribute("class", "messageboxborder" + theme);
        getinputdiv.style.width = wt + "px";
        getinputdiv.style.height = ht + "px";
        getinputdiv.style.left = 0.5 * (document.documentElement.clientWidth - wt) + "px";
        getinputdiv.style.top = 0.5 * (document.documentElement.clientHeight - ht) + "px";
        document.body.appendChild(getinputdiv);

        var getinputtitle = document.createElement("div");
        getinputtitle.setAttribute("class", "messageboxtitle" + theme);
        getinputtitle.innerHTML = title;
        getinputdiv.appendChild(getinputtitle);

        var getinputcontent = document.createElement("div");
        getinputcontent.setAttribute("class", "virtualarea");
        getinputdiv.appendChild(getinputcontent);

        var getinputtextarea = document.createElement("textarea");
        if (title) {
            getinputtextarea.setAttribute("placeholder", title);
        }

        if (regex) {
            getinputtextarea.setAttribute("data-regex", regex);
        }

        if (errormessage) {
            getinputtextarea.setAttribute("data-errormessage", errormessage);
        }
        getinputtextarea.setAttribute("class", "textbox" + theme);
        getinputtextarea.style.width = (wt - 30) + "px";
        getinputtextarea.style.height = (ht - 125) + "px";
        getinputtextarea.innerHTML = input;
        getinputcontent.appendChild(getinputtextarea);

        var getinputsplitdiv = document.createElement("div");
        getinputsplitdiv.setAttribute("class", "messageboxsplitter" + theme);
        getinputcontent.appendChild(getinputsplitdiv);

        var alertyesbutton = document.createElement("input");
        alertyesbutton.setAttribute("type", "button");
        alertyesbutton.setAttribute("class", "button-green");
        alertyesbutton.setAttribute("value", "确定");
        if (callback) {
            alertyesbutton.setAttribute("onclick", "sharp.getinputcompleted('" + callback + "')");
        }

        alertyesbutton.style.position = "relative";
        alertyesbutton.style.left = (wt - 85) + "px";
        alertyesbutton.style.top = "5px";
        alertyesbutton.onclick = sharp.getinputreset;
        getinputcontent.appendChild(alertyesbutton);

        var alertnobutton = document.createElement("input");
        alertnobutton.setAttribute("type", "button");
        alertnobutton.setAttribute("class", "button" + theme);
        alertnobutton.setAttribute("value", "取消");
        alertnobutton.style.position = "relative";
        alertnobutton.style.left = (wt - 240) + "px";
        alertnobutton.style.top = "5px";
        alertnobutton.onclick = sharp.getinputreset;
        getinputcontent.appendChild(alertnobutton);
    },
    getinputcompleted: function (callback) {
        callback("111");
    },
    getinputreset: function () {
        var alertmaskdiv = document.getElementById("alertmaskdiv");
        if (alertmaskdiv) {
            document.body.removeChild(alertmaskdiv);
        }

        var alertdiv = document.getElementById("getinputdiv");
        if (alertdiv) {
            document.body.removeChild(alertdiv);
        }
    },
    showmessage: function (message, duration, theme) {
        if (!duration || isNaN(duration)) {
            duration = 2000;
        }

        if (theme == "red" || theme == "green" || theme == "blue" || theme == "purple") {
            theme = "-" + theme;
        }
        else {
            theme = "";
        }

        var messagediv = document.getElementById("messagediv");
        if (!messagediv) {
            messagediv = document.createElement("div");
            messagediv.setAttribute("id", "messagediv");
        }
        else {
            messagediv.innerHTML = "";
        }

        messagediv.setAttribute("class", "showmessageborder" + theme);
        messagediv.style.left = 0.5 * (document.documentElement.clientWidth) + "px";
        messagediv.style.top = "0";
        messagediv.innerHTML = message;
        document.body.appendChild(messagediv);

        setTimeout(function () {
            sharp.showmessagereset();
        }, duration);
    },
    showmessagereset: function () {
        var messagediv = document.getElementById("messagediv");
        if (messagediv) {
            document.body.removeChild(messagediv);
        }
    },
    showtips: function (message, duration, theme, ht, wt) {
        if (!duration || isNaN(duration)) {
            duration = 2000;
        }

        if (theme == "red" || theme == "green" || theme == "blue" || theme == "purple") {
            theme = "-" + theme;
        }
        else {
            theme = "";
        }

        if (!ht || isNaN(ht)) {
            ht = 32;
        }

        if (!wt || isNaN(wt)) {
            wt = document.documentElement.clientWidth;
        }

        var tipsdiv = document.getElementById("tipsdiv");
        if (!tipsdiv) {
            tipsdiv = document.createElement("div");
            tipsdiv.setAttribute("id", "tipsdiv");
        }
        else {
            tipsdiv.innerHTML = "";
        }

        tipsdiv.setAttribute("class", "tipsborder" + theme);
        tipsdiv.innerHTML = message;
        document.body.appendChild(tipsdiv);

        setTimeout(function () {
            sharp.showtipsreset();
        }, duration);
    },
    showtipsreset: function () {
        var messagediv = document.getElementById("tipsdiv");
        if (messagediv) {
            document.body.removeChild(messagediv);
        }
    },
    foreach: function (list, func) {
        for (var i = 0; i < list.length; i++) {
            var item = list[i];
            func(item);
        }
    },
    copy: function (target, source) {
        for (var i in source) {
            var p = source[i];
            if (target[i] && p) {
                target[i] = p;
            }
        }
    },
    dropdownlist: {
        options: null,
        show: function (id) {
            var container = sharp.id(id);
            if (!container) {
                return;
            }

            //clear options
            sharp.dropdownlist.options = [];

            var nodes1 = container.childNodes;
            for (var i1 = 0; i1 < nodes1.length; i1++) {
                var n1 = nodes1[i1];
                if (n1.tagName && n1.tagName.toLowerCase() == "select") {
                    var nodes2 = n1.childNodes;
                    for (var i2 = 0; i2 < nodes2.length; i2++) {
                        var n2 = nodes2[i2];
                        if (n2.tagName && n2.tagName.toLowerCase() == "option") {
                            var name = n2.innerHTML;
                            var value = n2.getAttribute("value");
                            sharp.dropdownlist.options.push({ name: name, value: value });
                        }
                    }
                }
            }

            //init dropdownlist
            container.innerHTML = "";
            container.setAttribute("class", "dropdownlist");
            container.style.width = "200px";

            var tbx = document.createElement("input");
            tbx.setAttribute("type", "text");
            tbx.setAttribute("class", "underlinebox");
            container.appendChild(tbx);

            var btn = document.createElement("input");
            btn.setAttribute("type", "button");
            btn.setAttribute("class", "circus");
            btn.setAttribute("value", "+");
            btn.setAttribute("onclick", "sharp.dropdownlistclicked(this,'" + id + "')");
            container.appendChild(btn);

            var ul = document.createElement("ul");
            ul.setAttribute("class", "listbox");
            ul.style.display = "none";
            container.appendChild(ul);

            var options = sharp.dropdownlist.options;
            for (var i = 0; i < options.length; i++) {
                var item = options[i];
                var o = document.createElement("li");
                o.setAttribute("data-value", item.value);
                o.setAttribute("onclick", "sharp.dropdownlistitemclicked(this,'" + id + "')");
                o.innerHTML = item.name;
                ul.appendChild(o);
            }
        },
        getselected: function (id) {
            var element = sharp.id(id);
            if (!element) {
                return;
            }

            var name = element.getAttribute("data-name");
            var value = element.getAttribute("data-value");
            return { name: name, value: value };
        },
        setselected: function (id, value) {
            var element = sharp.id(id);
            if (!element) {
                return;
            }

            var nodes = element.childNodes;
            if (!nodes || nodes.length == 0) {
                return;
            }

            for (var i = 0; i < nodes.length; i++) {
                var n = nodes[i];
                if (n.tagName && n.tagName.toLowerCase() == "ul") {
                    var items = n.childNodes;
                    for (var k = 0; k < items.length; k++) {
                        var nitem = items[k];
                        var nvalue = nitem.getAttribute("data-value");
                        if (nvalue == value) {
                            nitem.onclick();
                            return;
                        }
                    }
                }
            }
        }
    },
    dropdownlistclicked: function (sender, id) {
        var element = sharp.id(id);
        if (!element) {
            return;
        }

        var nodes = element.childNodes;
        if (!nodes || nodes.length == 0) {
            return;
        }

        var isshow = element.getAttribute("data-isshow");
        for (var i = 0; i < nodes.length; i++) {
            var n = nodes[i];
            if (n.tagName && n.tagName.toLowerCase() == "ul") {
                if (isshow) {
                    element.removeAttribute("data-isshow");
                    sender.setAttribute("value", "+");
                    n.style.display = "none";
                }
                else {
                    element.setAttribute("data-isshow", "isshow");
                    sender.setAttribute("value", "-");
                    n.style.display = "block";
                }
            }
        }
    },
    dropdownlistitemclicked: function (sender, id) {
        var element = sharp.id(id);
        if (!element) {
            return;
        }

        var nodes = element.childNodes;
        if (!nodes || nodes.length == 0) {
            return;
        }

        var display = sender.innerHTML;
        var value = sender.getAttribute("data-value");
        element.setAttribute("data-name", display);
        element.setAttribute("data-value", value);

        for (var i = 0; i < nodes.length; i++) {
            var n = nodes[i];
            var ttype = n.getAttribute("type");
            if (n.tagName && n.tagName.toLowerCase() == "input" && ttype == "text") {
                n.value = display;
                continue;
            }

            if (n.tagName && n.tagName.toLowerCase() == "input" && ttype == "button") {
                sharp.dropdownlistclicked(n, id);
                continue;
            }
        }
    },
    datagrid: {
        template1: null,
        template2: null,
        show: function (id) {
            var container = sharp.id(id);
            if (!container) {
                return;
            }

            var url = container.getAttribute("data-url");
            if (!url) {
                return;
            }

            var showpager = container.getAttribute("data-showpager");
            var callback = container.getAttribute("data-loaded");

            var page = container.getAttribute("data-page");
            if (!page || isNaN(page)) {
                page = 1;
                container.setAttribute("data-page", page);
            }

            var pagesize = container.getAttribute("data-pagesize");
            if (!pagesize || isNaN(pagesize)) {
                pagesize = 10;
                container.setAttribute("data-pagesize", pagesize);
            }

            var data = {};
            var formid = container.getAttribute("data-form");
            if (formid) {
                data = sharp.serialize(formid);
            }

            data.page = page;
            data.pagesize = pagesize;

            sharp.post(url, data, function (responseText) {
                var response = JSON.parse(responseText);
                if (response.error != 0) {
                    if (callback) {
                        eval(callback);
                    }
                    return;
                }

                if (!response.data || response.data.length == 0) {
                    if (callback) {
                        eval(callback);
                    }
                    return;
                }

                if (response.recordcount && isNaN(response.recordcount) == false) {
                    container.setAttribute("data-recordcount", response.recordcount);
                }

                var autogeneratecolumns = container.getAttribute("data-autogeneratecolumns");
                if (autogeneratecolumns) {
                    var html = '';
                    html += '<tr>';
                    var firstitem = response.data[0];
                    for (var fp in firstitem) {
                        html += '<th>' + fp + '</th>'
                    }
                    html += '</tr>';

                    for (var i = 0; i < response.data.length; i++) {
                        html += '<tr>';
                        var item = response.data[i];
                        for (var ip in item) {
                            html += '<td>' + item[ip] + '</td>'
                        }
                        html += '</tr>';
                    }

                    container.innerHTML = html;
                    if (showpager) {
                        sharp.datagridpager.show(id);
                    }

                    if (callback) {
                        eval(callback);
                    }
                    return;
                }

                if (!sharp.datagrid.template1 || !sharp.datagrid.template1) {
                    var nodes1 = container.childNodes;
                    for (var i1 = 0; i1 < nodes1.length; i1++) {
                        var n1 = nodes1[i1];
                        if (n1.tagName && n1.tagName.toLowerCase() == "tbody") {
                            var nodes2 = n1.childNodes;
                            for (var i2 = 0; i2 < nodes2.length; i2++) {
                                var n2 = nodes2[i2];
                                if (n2.tagName && n2.tagName.toLowerCase() == "tr") {
                                    var trhtml = n2.innerHTML;
                                    if (trhtml.indexOf("th") >= 0) {
                                        sharp.datagrid.template1 = trhtml;
                                        n1.removeChild(n2);
                                        continue;
                                    }

                                    if (trhtml.indexOf("td") >= 0) {
                                        sharp.datagrid.template2 = trhtml;
                                        n1.removeChild(n2);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }

                var ghtml = '';
                ghtml += sharp.datagrid.template1;

                var pattern = sharp.datagrid.template2;
                for (var gi = 0; gi < response.data.length; gi++) {
                    ghtml += '<tr>';
                    var gitem = response.data[gi];
                    var gpattern = pattern;
                    for (var gip in gitem) {
                        gpattern = gpattern.replace(new RegExp("@" + gip, "g"), sharp.val(gitem[gip]));
                    }
                    ghtml += gpattern;
                    ghtml += '</tr>';
                }

                container.innerHTML = ghtml;
                if (showpager) {
                    sharp.datagridpager.show(id);
                }

                if (callback) {
                    eval(callback);
                }
            });
        }
    },
    datagridpager: {
        template: null,
        show: function (datagridid) {
            var datagrid = sharp.id(datagridid);
            if (!datagrid) {
                return;
            }

            var datagridpagerid = datagrid.getAttribute("data-pager");
            if (!datagridpagerid) {
                return;
            }

            var container = sharp.id(datagridpagerid);
            if (!container) {
                return;
            }

            var page = datagrid.getAttribute("data-page");
            if (!page || isNaN(page)) {
                page = 1;
            }

            var pagesize = datagrid.getAttribute("data-pagesize");
            if (!pagesize || isNaN(pagesize)) {
                pagesize = 10;
            }

            var recordcount = datagrid.getAttribute("data-recordcount");
            if (!recordcount || isNaN(recordcount)) {
                recordcount = 0;
            }

            if (recordcount == 0) {
                container.style.display = "none";
                return;
            }

            var pagecount = Math.ceil(1.0 * recordcount / pagesize);
            if (!sharp.datagridpager.template) {
                sharp.datagridpager.template = container.innerHTML;
            }

            var html = sharp.datagridpager.template;

            html = html.replace("@page", page);
            html = html.replace("@pagecount", pagecount);
            html = html.replace("@recordcount", recordcount);
            html = html.replace("prevpage()", "sharp.datagridpager.changepage('" + datagridid + "'," + (parseInt(page) - 1) + "," + pagesize + "," + pagecount + ")");
            html = html.replace("nextpage()", "sharp.datagridpager.changepage('" + datagridid + "'," + (parseInt(page) + 1) + "," + pagesize + "," + pagecount + ")");

            container.innerHTML = html;
        },
        changepage: function (datagridid, page, pagesize, pagecount) {
            var datagrid = sharp.id(datagridid);
            if (!datagrid) {
                return;
            }

            if (page < 1 || page > pagecount) {
                return;
            }

            datagrid.setAttribute("data-page", page);
            datagrid.setAttribute("data-pagesize", pagesize);
            sharp.datagrid.show(datagridid);
        }
    },
    listview: {
        template: null,
        show: function (id) {
            var container = sharp.id(id);
            if (!container) {
                return;
            }

            var url = container.getAttribute("data-url");
            if (!url) {
                return;
            }

            var showpager = container.getAttribute("data-showpager");
            var callback = container.getAttribute("data-loaded");

            var page = container.getAttribute("data-page");
            if (!page || isNaN(page)) {
                page = 1;
                container.setAttribute("data-page", page);
            }

            var pagesize = container.getAttribute("data-pagesize");
            if (!pagesize || isNaN(pagesize)) {
                pagesize = 10;
                container.setAttribute("data-pagesize", pagesize);
            }

            var data = {};
            var formid = container.getAttribute("data-form");
            if (formid) {
                data = sharp.serialize(formid);
            }

            data.page = page;
            data.pagesize = pagesize;

            sharp.post(url, data, function (responseText) {
                var response = JSON.parse(responseText);
                if (response.error != 0) {
                    if (callback) {
                        eval(callback);
                    }
                    return;
                }

                if (!response.data || response.data.length == 0) {
                    if (callback) {
                        eval(callback);
                    }
                    return;
                }

                if (response.recordcount && isNaN(response.recordcount) == false) {
                    container.setAttribute("data-recordcount", response.recordcount);
                }

                if (!sharp.listview.template) {
                    sharp.listview.template = container.innerHTML;
                }

                var html = '';
                for (var i = 0; i < response.data.length; i++) {
                    var item = response.data[i];
                    var pattern = sharp.listview.template;
                    for (var p in item) {
                        pattern = pattern.replace(new RegExp("@" + p, "g"), sharp.val(item[p]));
                    }
                    html += pattern;
                }

                container.innerHTML = html;
                if (showpager) {
                    sharp.listviewpager.show(id);
                }

                if (callback) {
                    eval(callback);
                }
            });
        }
    },
    listviewpager: {
        template: null,
        show: function (listviewid) {
            var listview = sharp.id(listviewid);
            if (!listview) {
                return;
            }

            var listviewpagerid = listview.getAttribute("data-pager");
            if (!listviewpagerid) {
                return;
            }

            var container = sharp.id(listviewpagerid);
            if (!container) {
                return;
            }

            var page = listview.getAttribute("data-page");
            if (!page || isNaN(page)) {
                page = 1;
            }

            var pagesize = listview.getAttribute("data-pagesize");
            if (!pagesize || isNaN(pagesize)) {
                pagesize = 10;
            }

            var recordcount = listview.getAttribute("data-recordcount");
            if (!recordcount || isNaN(recordcount)) {
                recordcount = 0;
            }

            if (recordcount == 0) {
                container.style.display = "none";
                return;
            }

            var pagecount = Math.ceil(1.0 * recordcount / pagesize);
            if (!sharp.listviewpager.template) {
                sharp.listviewpager.template = container.innerHTML;
            }

            var html = sharp.listviewpager.template;

            html = html.replace("@page", page);
            html = html.replace("@pagecount", pagecount);
            html = html.replace("@recordcount", recordcount);
            html = html.replace("prevpage()", "sharp.listviewpager.changepage('" + listviewid + "'," + (parseInt(page) - 1) + "," + pagesize + "," + pagecount + ")");
            html = html.replace("nextpage()", "sharp.listviewpager.changepage('" + listviewid + "'," + (parseInt(page) + 1) + "," + pagesize + "," + pagecount + ")");

            container.innerHTML = html;
        },
        changepage: function (listviewid, page, pagesize, pagecount) {
            var listview = sharp.id(listviewid);
            if (!listview) {
                return;
            }

            if (page < 1 || page > pagecount) {
                return;
            }

            listview.setAttribute("data-page", page);
            listview.setAttribute("data-pagesize", pagesize);
            sharp.listview.show(listviewid);
        }
    }
};