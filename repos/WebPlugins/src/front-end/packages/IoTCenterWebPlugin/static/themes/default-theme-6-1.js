
((_this) => {
    function getParameterMap () {
        let parameters = window.location.href.split("?")[1];
        map = new Map()
        if (!parameters) return map;
        let data = parameters.indexOf("&") != -1 ? parameters.split("&") : [parameters]
        for (let i = 0; i < data.length; i++) {
            let arry = data[i].split("=")
            map.set(arry[0], arry[1])
        }
        return map;
    }
    let parameters = getParameterMap();
    let theme = parameters.get('theme') || sessionStorage.theme || localStorage.theme ||'dark'
    sessionStorage.setItem('theme', theme)
    window.document.documentElement.setAttribute('data-theme', theme)

    const stylee_6_1 = document.createElement('link');
    stylee_6_1.rel = 'stylesheet';
    stylee_6_1.setAttribute('id', 'themeStyle-6-1')

    function setTheme (theme) {
        stylee_6_1.href = `/static/themes/${theme}-6-1.css`;
        document.head.appendChild(stylee_6_1);
        sessionStorage.setItem('theme', theme)
    }

    if (sessionStorage.theme) {
        setTheme(sessionStorage.theme)
    } else {
        let requestApi
        try {
            requestApi = _this.top.getConfigInfoData
        } catch (error) {
            requestApi = _this.getConfigInfoData
        }
        if (_this.top && requestApi) {
            requestApi().then(res => {
                setTheme(res && res.theme && res.theme.default || 'dark')
            }).catch(err => {
                setTheme('dark')
            })
        }
    }
})(window)
