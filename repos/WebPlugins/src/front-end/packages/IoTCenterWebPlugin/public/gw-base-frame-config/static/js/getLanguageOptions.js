(
    function (_this) {
        _this.isGetLanguageOptions = _this.isGetLanguageOptions || false
        _this.getLanguageOptionsError = _this.GetLanguageOptionsError || false
        _this.getLanguageOptions = function () {
            return new Promise((resolve, reject) => {
                if (!_this.getLanguageOptionsError) {
                    if (_this.LanguageOptions) {
                        resolve(_this.LanguageOptions)
                    } else {
                        if (_this.axios) {
                            if (!_this.isGetLanguageOptions) {
                                _this.isGetLanguageOptions = true
                                _this.axios({
                                    methed: 'get',
                                    url: '/api/localization/getsupportedcultures',
                                    headers: {
                                        'Content-Type': 'application/json;charset=UTF-8',
                                        'Accept-Language': window.sessionStorage.languageType || 'zh-CN'
                                    }
                                })
                                    .then(res => {
                                        const { code, data } = res ? res.data || {} : {}
                                        if (code == 200 && data) {
                                            _this.LanguageOptions = data
                                            resolve(data)
                                        } else {
                                            reject({})
                                        }
                                    })
                                    .catch(err => {
                                        _this.GetLanguageOptionsError = true
                                        reject({})
                                    })
                            } else {
                                let times = 0
                                let time = _this.setInterval(
                                    () => {
                                        if (_this.LanguageOptions || times > 300 || _this.GetLanguageOptionsError) {
                                            _this.clearInterval(time)
                                            time = null
                                            _this.LanguageOptions ? resolve(_this.LanguageOptions) : reject({})
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