

; ((_this) => {
    _this.isGetIconfontList = _this.isGetIconfontList || false
    function setIcon () {
        let iconfontList = _this.iconfontList || []
        iconfontList.forEach(item => {
            try {
                let style = document.createElement('link')
                style.rel = 'stylesheet'
                style.id = `${item}-${item}`
                style.href = `/static/fonts/${item}/iconfont.css`
                document.head.prepend(style)
            } catch (error) {
                console.log(error)
            }

        })
    }

    if (_this.iconfontList) {
        setIcon()
    } else {
        if (!_this.isGetIconfontList) {
            _this.isGetIconfontList = true
            _this.axios && _this.axios({
                method: 'get',
                url: '/IoT/api/v3/Auth/icon-fonts',
                headers: {
                    'Content-Type': 'application/json;charset=UTF-8',
                    'X-Requested-With': 'IoT-XMLHttpRequest',
                    'Accept-Language': _this.localStorage.languageType || 'zh-CN',
                }
            }).then(res => {
                if (res.data && res.data.code == 200) {
                    let iconfontList = res.data.data || []
                    _this.iconfontList = iconfontList
                    setIcon()
                }
            }).catch(err => {
                console.log(err)
            })
        } else {
            let times = 0
            let time = _this.setInterval(
                () => {
                    if (_this.iconfontList || times > 300) {
                        _this.clearInterval(time)
                        time = null
                        setIcon()
                    }
                    times = times + 1
                }, 100
            )
        }

    }
})(window)
