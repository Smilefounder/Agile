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
                    }
                });
            }
        }
    }
};