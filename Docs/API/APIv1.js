function byName(query, lang, start, maxResults, nameMatchMode) {

    jQuery.get(host + "/Api/v1/Song/ByName?format=JSON", { query: query, lang: lang, start: start, maxResults: maxResults, nameMatchMode: nameMatchMode }, callback, "jsonp");

}

function byPV(host, service, pvId, callback) {

    jQuery.get(host + "/Api/v1/Song/ByPV?format=JSON", { service: service, pvId: pvId }, callback, "jsonp");

}

function byPVBase(host, service, pvId, callback) {

    jQuery.get(host + "/Api/v1/Song/ByPVBase?format=JSON", { service: service, pvId: pvId }, callback, "jsonp");

}

function parsePVUrl(host, pvUrl, callback) {

    jQuery.get(host + "/Api/v1/Song/ParsePVUrl?format=JSON", { pvUrl: pvUrl }, callback, "jsonp");

}