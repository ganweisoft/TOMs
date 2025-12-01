window.getConfigInfoData = function () {
    return new Promise((resolve, reject) => {
        let configInfoData = sessionStorage.configInfoData || null
        if (configInfoData) {
            resolve(JSON.parse(configInfoData))
        } else {
            if (window.AxiosBuilder.axios) {
                reject({})
            } else {
                reject({})
            }
        }
    })
}
