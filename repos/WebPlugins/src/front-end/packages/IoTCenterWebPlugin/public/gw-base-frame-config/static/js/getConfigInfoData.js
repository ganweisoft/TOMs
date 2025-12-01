(
    function (_this) {
        _this.isGetConfigInfoData = _this.isGetConfigInfoData || false
        _this.getConfigInfoDataError = _this.getConfigInfoDataError || false
        _this.getConfigInfoData = function () {
            return new Promise((resolve, reject) => {
                if (!_this.getConfigInfoDataError) {
                    if (_this.configInfoData) {
                        resolve(_this.configInfoData)
                    } else {
                        if (_this.axios) {
                            if (!_this.isGetConfigInfoData) {
                                _this.isGetConfigInfoData = true
                                _this.axios({
                                    methed: 'get',
                                    url: '/IoT/api/v3/Frontconfiguration/GetFrontconfigurationData',
                                    headers: {
                                        'Content-Type': 'application/json;charset=UTF-8',
                                        'Accept-Language': window.sessionStorage.languageType || 'zh-CN'
                                    }
                                })
                                    .then(res => {
                                        const { code, data } = res ? res.data || {} : {}
                                        if (code == 200 && data) {
                                            _this.configInfoData = JSON.parse(data)
                                            resolve(JSON.parse(data))
                                        } else {
                                            reject({})
                                        }
                                    })
                                    .catch(err => {
                                        _this.getConfigInfoDataError = true
                                        reject({})
                                    })
                            } else {
                                let times = 0
                                let time = _this.setInterval(
                                    () => {
                                        if (_this.configInfoData || times > 300 || _this.getConfigInfoDataError) {
                                            _this.clearInterval(time)
                                            time = null
                                            _this.configInfoData ? resolve(_this.configInfoData) : reject({})
                                        }
                                        times = times + 1
                                    }, 100
                                )
                            }

                        } else {
                            reject({})
                        }
                    }
                } else {
                    reject({})
                }
            })
        }

    }
)(window.top)