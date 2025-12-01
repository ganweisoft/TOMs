(function (_this) {
// 获取权限列表
let params, packageId, pluginName, routeName, objData, result;
_this.permissionList = async function() {
    try {
        params = getpermissionMap()
        packageId = params.get("packageId")
        pluginName = params.get("pluginName")
        routeName = params.get("menuName")
        if(packageId && pluginName && routeName) {
            _this["packageId"] = packageId
            _this["routeName"] = routeName
            _this[packageId] = {}
            objData = {packageId: packageId, routeName: '/Index/jumpIframe/' + pluginName + "/" + routeName}
            axios.defaults.withCredentials = true;
            result = axios({
                method: 'get',
                url: '/api/UserRole/getPageControlPermission',
                params: objData,
                data: objData,
                headers: {
                    'Content-Type': 'application/json;charset=UTF-8',
                    'Accept-Language': _this.sessionStorage.languageType || 'zh-CN'
                }
            })
            await result.then(res => {
                if(res && res.data) {
                    let dt = res.data.data
                    _this[packageId][routeName] = (Array.isArray(dt) && dt.length > 0) ? dt : []
                }

            })
        } else {
            console.log('地址需要携带参数: packageId, pluginName, menuName');
        }
    } catch (error) {
        console.error('获取权限列表时出错:', error);
    }
};

// 获取权限映射
function getpermissionMap() {
    let data, parameters, map = new Map()
    if(_this.location.href.indexOf("?") == -1) return map
    parameters = _this.location.href.split("?")[1]
    if (!parameters) return map
    data = parameters.indexOf("&") != -1 ? parameters.split("&") : [parameters]
    for (let i = 0; i < data.length; i++) {
        let arry = data[i].indexOf("=") != -1 ? data[i].split("=") : [data[i], data[i]]
        map.set(arry[0], arry[1])
    }
    return map
}
})(window)
