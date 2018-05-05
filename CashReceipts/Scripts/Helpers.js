
serialize = function (obj) {
    var str = [];
    for (var p in obj)
        if (obj.hasOwnProperty(p)) {
            str.push(serializeProperty(p, obj[p]));
        }
    return str.join("&");
}

serializeProperty = function (prop, value) {
    if (prop === "filters" && Object.prototype.toString.call(value) === '[object Array]') {
        var str = [];
        for (var i = 0; i < value.length; i++) {
            str.push(serializeProperty(value[i].field, value[i].value));
        }
        return str.join("&");
    }
    if (typeof value == "object")
        return serialize(value);
    else
        return encodeURIComponent(prop) + "=" + encodeURIComponent(value);
}
