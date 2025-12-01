import {
    bus,
    destroyApp,
    preloadApp,
    setupApp} from 'wujie'
import {
    idToSandboxCacheMap
} from 'wujie/esm/common.js'

import hostMap from '../hostMap'
export function combineQuery (name, query, route) {
    let packageName = name;
    let queryArray = Object.entries(query);

    if (!this.$hostMap(packageName)) {
        throw new Error(`模块 ${packageName} 未在 hostMap 中定义入口`);
    }
    if (route || queryArray.length > 0) {
        return hostMap(packageName) + '/#/' + (route || '') + queryArray.reduce((pre, [key, value]) => pre + `${key}=${value}`, '?');
    }
    return hostMap(packageName);

}

// 管理嵌入模块
// 管理需要提供特殊功能的模块（非页面），可能多个模块需要预加载这些模块，如：二维码扫描，拍照图片上传
export class MFEManager {
    router = null
    route = null
    limit = 0
    theme = localStorage.theme || 'dark'
    myJavaFun = ''
    constructor({
        limit,
        router,
        route,
        myJavaFun
    }) {
        this.limit = limit
        this.router = router
        this.route = route
        this.myJavaFun = myJavaFun
        window.idToSandboxCacheMap = idToSandboxCacheMap
    }

    get size () {
        return idToSandboxCacheMap.size;
    }

    setTheme (value) {

        changeTheme(window, value)
        Array.from(idToSandboxCacheMap.entries()).forEach(([packageName, instance]) => {
            changeTheme(instance.wujie.iframe.contentWindow, value)
        });
        bus.$emit('changeTheme', value);
    }

    getUrl (appWindow, options) {
        let {
            name,
            query,
            route
        } = options
        if (!name) {
            name = appWindow.__WUJIE.id
        }
        return combineQuery(name, query, route)
    }

    preloadApp (appWindow, options) {
        if (this.limit && this.size >= this.limit) {
            console.warn("预加载子页面超过上限：" + this.limit)
            return
        }
        preloadApp(options);
    }

    setupApp (options) {

        setupApp(options);
    }

    destroyApp (appWindow) {
        appWindow.__WUJIE?.inject?.idToSandboxMap.clear();
        appWindow.__WUJIE.id && destroyApp(appWindow.__WUJIE.id);

    }

    setAlive (appWindow, isAlive = true) {
        let revokeSetAlive = null;
        if (appWindow.__POWERED_BY_WUJIE__ && appWindow.__WUJIE) {
            // 设置实例保活配置
            if (appWindow.__WUJIE.alive !== isAlive) {
                appWindow.__WUJIE.alive = isAlive;
                // 设置实例保活配置缓存
                setupApp({
                    name: appWindow.__WUJIE.id,
                    alive: isAlive
                })
                revokeSetAlive = () => {
                    this.setAlive(appWindow, !isAlive)
                }
            }
        }
        return revokeSetAlive;
    }

    startQrcode (appWindow, callback) {
        if (!appWindow) return;
        // 设置保活，避免无法接收二维码识别结果
        this.setAlive(appWindow, true);

        // 订阅二维码识别事件
        appWindow.$wujie?.bus.$on('app-qrcode-result', callback)
        // 启动二维码扫描
        this.router.push({
            path: '/index/jump/ganwei-app-qrcode',
            query: {
                from: appWindow.__WUJIE.id
            }
        })
    }

    $on (appWindow, event, callback) {
        appWindow.$wujie?.bus.$on(event, callback)
    }
}

export function setAlive (appWindow, isAlive = true) {
    if (appWindow.__POWERED_BY_WUJIE__ && appWindow.__WUJIE) {
        // 设置实例保活配置
        appWindow.__WUJIE.alive = isAlive;
        // 设置实例保活配置缓存
        setupApp({
            name: appWindow.__WUJIE.id,
            alive: isAlive
        })
    }
}

function changeTheme (appWindow, value) {
    appWindow.localStorage.theme = value;

    // 删除旧的主题样式
    let themeStyle = appWindow.document.getElementById('themeStyle');
    themeStyle.remove();

    // 重新请求新主题
    let stylee = document.createElement("link");
    stylee.setAttribute("id", "themeStyle");
    stylee.setAttribute("rel", "stylesheet");
    stylee.href = `/static/themes/${value}.css`;
    let head = appWindow.document.getElementsByTagName("head")[0];
    head.appendChild(stylee);
}
