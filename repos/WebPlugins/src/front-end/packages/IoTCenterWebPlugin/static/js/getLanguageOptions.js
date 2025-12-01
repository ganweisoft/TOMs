
sessionStorage.isGetLanguageOptions = sessionStorage.isGetLanguageOptions == 'true' || false
window.getLanguageOptions = function () {
    return new Promise((resolve, reject) => {
        let LanguageOptions = sessionStorage.LanguageOptions || null
        if (LanguageOptions) {
            resolve(JSON.parse(LanguageOptions))
        } else {
            let data = [
                {
                    "name": "中文",
                    "value": "zh-CN",
                    "selected": true
                },
                {
                    "name": "English",
                    "value": "en-US",
                    "selected": false
                }
            ]
            sessionStorage.LanguageOptions = JSON.stringify(data)
            resolve(data)
        }
    })
}
