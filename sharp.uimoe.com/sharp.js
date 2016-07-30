var sharp = {
    fn: {
        copyTo: function (target, source) {
            for (var p in target) {
                if (source[p]) {
                    target[p] = source[p];
                }
            }
        },
        ajax: function (set2) {
            var set1 = {
                type: "GET",
                url: "",
                data: {},
                callback: function () {

                }
            };

            sharp.fn.copyTo(set1, set2);

            if (!set1.url) {
                return;
            }

            if (set1.type != "GET" && set1.type != "POST") {
                return;
            }

            var queryStr = sharp.fn.toQueryStr(set1.data);
            if (set1.type == "GET") {
                var parts = set1.url.split('?');
                if (parts.length > 1) {
                    if (queryStr) {
                        queryStr = parts[1] + "&" + queryStr;
                    }
                    else {
                        queryStr = parts[1];
                    }
                }

                if (queryStr) {
                    set1.url += "?" + queryStr;
                }
            }

            var xhr = new XMLHttpRequest();
            xhr.open(set1.type, set1.url);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    if (set1.callback && typeof (set1.callback) == "function") {
                        set1.callback(xhr);
                    }
                }
            }

            xhr.send(queryStr);
        },
        toQueryStr: function (data) {
            if (!data) {
                return "";
            }

            var sb = "";
            for (var p in data) {
                sb += p + "=" + data[p] + "&";
            }

            if (sb.length > 1) {
                sb = sb.substr(0, sb.length - 1);
            }

            return sb;
        }
    },
    control: {
        template: {
            all: [],
            one: function (id) {
                for (var i = 0; i < sharp.control.template.all.length; i++) {
                    var item = sharp.control.template.all[i];
                    if (item.id == id) {
                        return item.content;
                    }
                }

                return "";
            }
        },
        select: {
            show: function (id, callback) {
                var set1 = {
                    id: "",
                    callback: function () {

                    }
                };

                var set2 = {
                    id: id,
                    callback: callback
                };

                var container = document.getElementById(set2.id);
                if (!container) {
                    return;
                }

                if (set2.callback && typeof (set2.callback) != "function") {
                    set2.callback = null;
                }

                sharp.fn.copyTo(set1, set2);

                var data = {
                    action: "select"
                };

                sharp.fn.ajax({
                    type: "GET",
                    url: "ajax.ashx",
                    data: data,
                    callback: function (xhr) {
                        var error = 1;
                        if (xhr.status != 200 && xhr.status != 304) {
                            set1.callback(error);
                            return;
                        }

                        var response = {};

                        try {
                            response = JSON.parse(xhr.responseText);
                        }
                        catch (e) {

                        }

                        if (!response ||
                            response.error != 0 ||
                            !response.data ||
                            !response.data.RecordList) {
                            set1.callback(error);
                            return;
                        }

                        var template = sharp.control.template.one(set1.id);
                        if (!template) {
                            if (!container.innerHTML) {
                                return;
                            }

                            template = container.innerHTML;
                            sharp.control.template.all.push({ id: set1.id, content: template });
                        }

                        var html = '';
                        for (var i = 0; i < response.data.RecordList.length; i++) {
                            var template2 = template;
                            var item = response.data.RecordList[i];

                            for (var p in item) {
                                template2 = template2.replace(new RegExp("@" + p, "g"), item[p]);
                            }

                            html += template2;
                        }

                        container.innerHTML = html;

                        error = 0;
                        set1.callback(error);
                    }
                });
            }
        },
        ul: {
            show: function (id, callback) {
                var set1 = {
                    id: "",
                    callback: function () {

                    }
                };

                var set2 = {
                    id: id,
                    callback: callback
                };

                var container = document.getElementById(set2.id);
                if (!container) {
                    return;
                }

                if (set2.callback && typeof (set2.callback) != "function") {
                    set2.callback = null;
                }

                sharp.fn.copyTo(set1, set2);

                var data = {
                    action: "ul"
                };

                sharp.fn.ajax({
                    type: "GET",
                    url: "ajax.ashx",
                    data: data,
                    callback: function (xhr) {
                        var error = 1;
                        if (xhr.status != 200 && xhr.status != 304) {
                            set1.callback(error);
                            return;
                        }

                        var response = {};

                        try {
                            response = JSON.parse(xhr.responseText);
                        }
                        catch (e) {

                        }

                        if (!response ||
                            response.error != 0 ||
                            !response.data ||
                            !response.data.RecordList) {
                            set1.callback(error);
                            return;
                        }

                        var template = sharp.control.template.one(set1.id);
                        if (!template) {
                            if (!container.innerHTML) {
                                return;
                            }

                            template = container.innerHTML;
                            sharp.control.template.all.push({ id: set1.id, content: template });
                        }

                        var html = '';
                        for (var i = 0; i < response.data.RecordList.length; i++) {
                            var template2 = template;
                            var item = response.data.RecordList[i];

                            for (var p in item) {
                                template2 = template2.replace(new RegExp("@" + p, "g"), item[p]);
                            }

                            html += template2;
                        }

                        container.innerHTML = html;

                        error = 0;
                        set1.callback(error);
                    }
                });
            }
        },
        table: {
            show: function (id, callback) {
                var set1 = {
                    id: "",
                    callback: function () {

                    }
                };

                var set2 = {
                    id: id,
                    callback: callback
                };

                var container = document.getElementById(set2.id);
                if (!container) {
                    return;
                }

                if (set2.callback && typeof (set2.callback) != "function") {
                    set2.callback = null;
                }

                sharp.fn.copyTo(set1, set2);

                var showpager = container.getAttribute("data-showpager");
                var pagerid = container.getAttribute("data-pager");
                var pager = document.getElementById(pagerid);
                if (!pager) {
                    showpager = "";
                }

                var data = {
                    action: "table"
                };

                sharp.fn.ajax({
                    type: "GET",
                    url: "ajax.ashx",
                    data: data,
                    callback: function (xhr) {
                        var error = 1;
                        if (xhr.status != 200 && xhr.status != 304) {
                            set1.callback(error);
                            return;
                        }

                        var response = {};

                        try {
                            response = JSON.parse(xhr.responseText);
                        }
                        catch (e) {

                        }

                        if (!response ||
                            response.error != 0 ||
                            !response.data ||
                            !response.data.RecordList) {
                            set1.callback(error);
                            return;
                        }

                        var template_th = sharp.control.template.one(set1.id + "_th");
                        var template_td = sharp.control.template.one(set1.id + "_td");
                        if (!template_th && !template_td) {
                            if (!container.innerHTML) {
                                return;
                            }

                            for (var i = 0; i < container.childNodes.length ; i++) {
                                var child = container.childNodes[i];
                                var tagName = "";
                                if (child && child.tagName) {
                                    tagName = child.tagName.toLowerCase();
                                }

                                if (tagName == "tbody") {
                                    var trs = [];
                                    for (var i2 = 0; i2 < child.childNodes.length; i2++) {
                                        var subchild = child.childNodes[i2];
                                        var tagName2 = "";
                                        if (subchild && subchild.tagName) {
                                            tagName2 = subchild.tagName.toLowerCase()
                                        }

                                        if (tagName2 == "tr") {
                                            trs.push(subchild);
                                        }
                                    }

                                    if (trs.length >= 2) {
                                        template_th = trs[0].outerHTML;
                                        template_td = trs[1].outerHTML;
                                    }
                                    break;
                                }
                            }

                            if (!template_td) {
                                return;
                            }

                            if (template_th) {
                                sharp.control.template.all.push({ id: set1.id + "_th", content: template_th });
                            }

                            sharp.control.template.all.push({ id: set1.id + "_td", content: template_td });
                        }

                        var html = '';
                        if (template_th) {
                            html += template_th;
                        }

                        for (var i = 0; i < response.data.RecordList.length; i++) {
                            var template2 = template_td;
                            var item = response.data.RecordList[i];

                            for (var p in item) {
                                template2 = template2.replace(new RegExp("@" + p, "g"), item[p]);
                            }

                            html += template2;
                        }

                        container.innerHTML = html;

                        error = 0;
                        set1.callback(error);
                    }
                });
            }
        }
    }
};