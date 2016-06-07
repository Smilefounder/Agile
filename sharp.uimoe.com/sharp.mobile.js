var $ = {
    id: function (idstr) {
        return document.getElementById(idstr);
    },
    classname: function (classnamestr) {
        return document.getElementsByClassName(classnamestr);
    },
    name: function (namestr) {
        return document.getElementsByName(namestr);
    },
    tagname: function (tagstr) {
        return document.getElementsByTagName(tagstr);
    },
    ajax: function (set2) {
        var set1 = {
            type: "GET",
            url: "",
            data: {},
            complete: null
        };

        $.fn.copyto(set1, set2);

        var sb = '';
        for (var p in set1.data) {
            sb += p + "=" + set1.data[p] + "&";
        }

        if (sb.length > 0) {
            sb = sb.substr(0, sb.length - 1);
        }

        if (set1.type == "GET") {
            if (set1.url.indexOf("?") < 0) {
                sb = "?" + sb;
            }

            set1.url += sb;
        }

        var xhr = new XMLHttpRequest();
        xhr.open(set1.type, set1.url);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                set1.complete(xhr);
            }
        }

        if (set1.type == "GET") {
            xhr.send("");
            return;
        }

        xhr.send(sb);
    },
    fn: {
        copyto: function (target, source) {
            for (var p in target) {
                var val = source[p];
                if (val) {
                    target[p] = val;
                }
            }
        }
    },
    control: {
        alertcallback: null,
        alertreset: function () {
            var dialogdiv = $.id("dialogdiv");
            if (dialogdiv) {
                document.body.removeChild(dialogdiv);
            }

            var maskdiv = $.id("maskdiv");
            if (maskdiv) {
                document.body.removeChild(maskdiv);
            }

            if ($.control.alertcallback) {
                $.control.alertcallback();
            }
        },
        alert: function (message, title) {
            var set1 = {
                message: "内容",
                title: "标题"
            };

            var set2 = {
                message: message,
                title: title
            };

            $.fn.copyto(set1, set2);

            var maskdiv = $.id("maskdiv");
            if (!maskdiv) {
                maskdiv = document.createElement("div");
                maskdiv.setAttribute("id", "maskdiv");
                maskdiv.setAttribute("class", "mask");
                document.body.appendChild(maskdiv);
            }

            var dialogdiv = $.id("dialogdiv");
            if (!dialogdiv) {
                dialogdiv = document.createElement("div");
                dialogdiv.setAttribute("id", "dialogdiv");
                dialogdiv.setAttribute("class", "dialog_border");
                document.body.appendChild(dialogdiv);
            }

            var html = '';
            html += '<div class="dialog_hd">' + set1.title + '</div>';
            html += '<div class="dialog_bd">' + set1.message + '</div>';
            html += '<div class="dialog_ft">';
            html += '    <button class="btn" onclick="$.control.alertreset()">OK</button>';
            html += '</div>';
            dialogdiv.innerHTML = html;
        },
        promptcallback: null,
        promptreset: function () {
            var dialogdiv = $.id("dialogdiv");
            if (dialogdiv) {
                document.body.removeChild(dialogdiv);
            }

            var maskdiv = $.id("maskdiv");
            if (maskdiv) {
                document.body.removeChild(maskdiv);
            }

            if ($.control.promptcallback) {
                $.control.promptcallback();
            }
        },
        prompt: function (message, defaultstr) {
            var set1 = {
                message: "提示",
                defaultstr: ""
            };

            var set2 = {
                message: message,
                defaultstr: defaultstr
            };

            $.fn.copyto(set1, set2);

            var maskdiv = $.id("maskdiv");
            if (!maskdiv) {
                maskdiv = document.createElement("div");
                maskdiv.setAttribute("id", "maskdiv");
                maskdiv.setAttribute("class", "mask");
                document.body.appendChild(maskdiv);
            }

            var dialogdiv = $.id("dialogdiv");
            if (!dialogdiv) {
                dialogdiv = document.createElement("div");
                dialogdiv.setAttribute("id", "dialogdiv");
                dialogdiv.setAttribute("class", "dialog_border");
                document.body.appendChild(dialogdiv);
            }

            var html = '';
            html += '<div class="dialog_hd">' + set1.message + '</div>';
            html += '<div class="dialog_bd">';
            html += '<input class="text" type="text"/>';
            html += '</div>';
            html += '<div class="dialog_ft">';
            html += '    <button class="btn" onclick="$.control.promptreset()">OK</button>';
            html += '</div>';
            dialogdiv.innerHTML = html;
        },
        templates: {
            all: [],
            one: function (id) {
                for (var i = 0; i < $.control.templates.all.length; i++) {
                    var item = $.control.templates.all[i];
                    if (item.id == id) {
                        return item.template;
                    }
                }

                return "";
            }
        },
        listview: {
            parsetemplate: function (html) {
                return html;
            },
            show: function (id) {
                var containerdiv = $.id(id);
                if (!containerdiv) {
                    console.log("listview初始化失败，因为找不到元素:" + id);
                    return;
                }

                var url = containerdiv.getAttribute("data-url");
                if (!url) {
                    console.log("listview初始化失败，因为data-url属性尚未设置");
                    return;
                }

                var template = $.control.templates.one(id);
                if (!template) {
                    template = $.control.listview.parsetemplate(containerdiv.innerHTML);
                    if (template) {
                        $.control.templates.all.push({
                            id: id,
                            template: template
                        })
                    }
                }

                if (!template) {
                    console.log("listview初始化失败，因为模板尚未设置");
                    return;
                }

                var set1 = {
                    url: url,
                    template: template,
                    page: 1,
                    pagesize: 10,
                    recordcount: 0,
                    showpager: true,
                    pagerid: null
                };

                var page = containerdiv.getAttribute("data-page");
                if (page && !isNaN(page)) {
                    set1.page = parseInt(page);
                }

                var pagesize = containerdiv.getAttribute("data-pagesize");
                if (pagesize && !isNaN(pagesize)) {
                    set1.pagesize = parseInt(pagesize);
                }

                var showpager = containerdiv.getAttribute("data-showpager");
                if (showpager) {
                    set1.showpager = true;
                    set1.pagerid = containerdiv.getAttribute("data-pagerid");
                }
                else {
                    set1.showpager = false;
                }

                var data = {
                    page: set1.page,
                    pagesize: set1.pagesize
                };

                $.ajax({
                    url: set1.url,
                    type: "GET",
                    data: data,
                    complete: function (xhr) {
                        if (xhr.status != 200 && xhr.status != 304) {
                            return;
                        }

                        var response = null;
                        try {
                            response = JSON.parse(xhr.responseText);
                        }
                        catch (e) {
                            console.log("listview渲染失败，因为解析服务器响应时发生错误：" + e);
                        }

                        if (!response ||
                            response.error != 0 ||
                            !response.recordlist ||
                            response.recordlist.length == 0) {
                            containerdiv.innerHTML = "暂无数据";
                            return;
                        }

                        if (response.recordcount && !isNaN(response.recordcount)) {
                            set1.recordcount = response.recordcount;
                        }

                        var html = '';
                        for (var i = 0; i < response.recordlist.length; i++) {
                            var item = response.recordlist[i];
                            var pattern = set1.template;
                            for (var p in item) {
                                pattern = pattern.replace("@" + p, item[p]);
                            }

                            html += pattern;
                        }

                        containerdiv.innerHTML = html;

                        if (set1.showpager) {
                            $.control.pager.show(id, set1.pagerid, set1.page, set1.pagesize, set1.recordcount);
                        }
                    }

                });
            }
        },
        pager: {
            parsetemplate: function (html) {
                if (html) {
                    return html;
                }

                var sb = '';
                sb += '<div style="float:left;">';
                sb += '第<span>@page</span>/<span>@pagecount</span>页&nbsp;共<span>@recordcount</span>条记录';
                sb += '</div>';
                sb += '<div style="float:right;">';
                sb += '<a href="javascript:void(0)" onclick="@firstpage">首页</a>&nbsp;';
                sb += '<a href="javascript:void(0)" onclick="@prevpage">上一页</a>&nbsp;';
                sb += '<a href="javascript:void(0)" onclick="@nextpage">下一页</a>&nbsp;';
                sb += '<a href="javascript:void(0)" onclick="@lastpage">尾页</a>';
                sb += '</div>';
                sb += '<div style="clear:both;">';
                sb += '</div>';
                return sb;
            },
            changepage: function (id, page, pagesize, pagecount) {
                if (page < 1) {
                    alert("没有上一页了！");
                    return;
                }

                if (page > pagecount) {
                    alert("没有下一页了！");
                    return;
                }

                var listview = $.id(id);
                listview.setAttribute("data-page", page);
                listview.setAttribute("data-pagesize", pagesize);
                $.control.listview.show(id);
            },
            show: function (id, pagerid, page, pagesize, recordcount) {
                var pager = $.id(pagerid);
                if (!pager) {
                    console.log("初始化分页失败，因为未设置data-pagerid");
                    return;
                }

                var template = $.control.templates.one(pagerid);
                if (!template) {
                    template = $.control.pager.parsetemplate(pager.innerHTML);
                    if (!template) {
                        $.control.templates.all.push({
                            id: id,
                            template: template
                        })
                    }
                }

                var pagecount = Math.ceil(1.0 * recordcount / pagesize);
                var set1 = {
                    page: page,
                    pagecount: pagecount,
                    recordcount: recordcount
                };

                var pattern = template;
                pattern = pattern.replace("@page", set1.page);
                pattern = pattern.replace("@pagecount", set1.pagecount);
                pattern = pattern.replace("@recordcount", set1.recordcount);
                pattern = pattern.replace("@firstpage", '$.control.pager.changepage(\'' + id + '\',1,' + pagesize + ',' + pagecount + ')');
                pattern = pattern.replace("@prevpage", '$.control.pager.changepage(\'' + id + '\',' + (page - 1) + ',' + pagesize + ',' + pagecount + ')');
                pattern = pattern.replace("@nextpage", '$.control.pager.changepage(\'' + id + '\',' + (page + 1) + ',' + pagesize + ',' + pagecount + ')');
                pattern = pattern.replace("@lastpage", '$.control.pager.changepage(\'' + id + '\',' + pagecount + ',' + pagesize + ',' + pagecount + ')');
                pager.innerHTML = pattern;
            }
        }
    }
};