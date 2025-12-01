sessionStorage.isGetLanguageOptions = sessionStorage.isGetLanguageOptions == 'true' || false
window.getLanguageOptions = function () {
    return new Promise((resolve, reject) => {
        let LanguageOptions = sessionStorage.LanguageOptions || null
        if (LanguageOptions) {
            resolve(JSON.parse(LanguageOptions))
        } else {
            if (window.AxiosBuilder.axios) {
                if (sessionStorage.isGetLanguageOptions == 'false') {
                    sessionStorage.isGetLanguageOptions = true
                    window.AxiosBuilder.axios({
                        methed: 'get',
                        url: '/api/localization/getsupportedcultures',
                        headers: {
                            'Content-Type': 'application/json;charset=UTF-8',
                            'Accept-Language': window.sessionStorage.languageType || 'zh-CN'
                        }
                    })
                        .then(res => {
                            const { code, data } = res ? (res.data || {}) : {}
                            if (code == 200 && data) {
                                sessionStorage.LanguageOptions = JSON.stringify(data)
                                resolve(data)
                            } else {
                                reject({})
                            }
                        })
                        .catch(err => {
                            reject({})
                        })
                } else {
                    let times = 0
                    let time = window.setInterval(
                        () => {
                            let LanguageOptions = sessionStorage.LanguageOptions || null
                            if (LanguageOptions || times > 300) {
                                window.clearInterval(time)
                                time = null
                                LanguageOptions ? resolve(JSON.parse(LanguageOptions)) : reject({})
                            }
                            times = times + 1
                        }, 100
                    )
                }

            } else {
                reject({})
            }
        }
    })
}
