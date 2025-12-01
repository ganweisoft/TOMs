
((_this) => {
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
            _this.iconfontList = ["font", "appFont"]
            setIcon()
            
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
