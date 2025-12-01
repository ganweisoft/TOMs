/**
 *
 * @class QueryParser
 *
 * @example
 * http://127.0.0.1:8120/?languageType=zh-CN#/Index/jumpIframe/ganwei-iotcenter-event-querys/eqEvent?languageType=zh-CN
 * 1. window.location.search
 * 2. route.query
 */
export default class QueryParser {
    location = ''
    constructor(route) {
        this.searchParams = new URLSearchParams(window.location.search);
        if (route) {
            this.addQuery(route.query);
        }
    }

    setLocation (location) {
        this.location = location
    }

    addURLSearchParams (outerUrlSearchParams) {
        outerUrlSearchParams.forEach((v, k) => {
            this.searchParams.set(k, v)
        })
    }

    addQuery (query) {
        if (typeof query === "object" && query !== null) {
            for (let key in query) {
                let value = query[key];
                if (value) {
                    this.searchParams.set(key, value);
                }
            }
        }
    }

    getFullPath () {
        return this.location + '?' + this.searchParams.toString()
    }

    getQueryString () {
        return '?' + this.searchParams.toString()
    }
}
