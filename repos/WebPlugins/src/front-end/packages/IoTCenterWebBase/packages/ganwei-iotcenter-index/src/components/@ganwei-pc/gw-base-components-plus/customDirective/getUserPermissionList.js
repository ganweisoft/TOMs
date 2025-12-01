import axios from 'axios'
let packageId,pluginName,routeName;
const permissionList = async function() {
    try {
        let params = getpermissionMap()
        packageId = params.get("packageId")
        pluginName = params.get("pluginName")
        routeName = params.get("menuName")
        if(packageId && pluginName && routeName) {
            window[packageId] = {}
            let objData = {packageId: packageId, routeName: '/Index/jumpIframe/' + pluginName + "/" + routeName}
            axios.defaults.withCredentials = true; // 让ajax携带cookie
            let result = axios({
                method: 'get',
                url: '/api/UserRole/getPageControlPermission',
                params: objData,
                data: objData,
                headers: {
                    'Content-Type': 'application/json;charset=UTF-8',
                    'Accept-Language': window.sessionStorage.languageType || 'zh-CN'
                }
            })
            await result.then(res => {
                if(res && res.data) {
                    let dt = res.data.data
                    window[packageId][routeName] = (Array.isArray(dt) && dt.length > 0) ? dt : []
                }

            })
        } else {
            console.log('地址需要携带参数: packageId, pluginName, menuName');
        }
    } catch (error) {
        console.error('获取权限列表时出错:', error);
    }
};

function getpermissionMap() {
    let map = new Map()
    if(window.location.href.indexOf("?") == -1) return map;
    let parameters = window.location.href.split("?")[1]
    if (!parameters) return map
    let data = parameters.indexOf("&") != -1 ? parameters.split("&") : [parameters]
    for (let i = 0; i < data.length; i++) {
        let arry = data[i].indexOf("=") != -1 ?data[i].split("="): [data[i],data[i]]
        map.set(arry[0], arry[1])
    }
    return map;
}

export {permissionList ,packageId ,pluginName, routeName};


