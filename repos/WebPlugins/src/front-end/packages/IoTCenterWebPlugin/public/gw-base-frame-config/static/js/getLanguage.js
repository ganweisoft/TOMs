(
    async function (_this) {
        const broswerLanguage = {
            "zh": "zh-CN",
            "zh-CN": "zh-CN",
            "zh-TW": "zh-TW",
            "zh-HK": "zh-TW",
            "en": "en-US",
            "en-US": "en-US"
        }
        function requestLanguage (pluginName, menuName, packageId, vm, url) {
            return new Promise((resolve, reject) => {
                _this[`${pluginName}-${menuName}`] = _this[`${pluginName}-${menuName}`] || false
                if (!_this[`${pluginName}-${menuName}`]) {
                    _this[`${pluginName}-${menuName}`] = true
                    let data = {
                        packageId,
                        pluginName,
                        menuName
                    }
                    _this.axios.defaults.withCredentials = true; // 让ajax携带cookie
                    _this.axios({
                        method: 'get',
                        url: url,
                        params: data,
                        headers: {
                            'Content-Type': 'application/json;charset=UTF-8',
                            'Accept-Language': window.localStorage.languageType || 'zh-CN'
                        }
                    }).then(res => {
                        const { code, data, message } = res ? res.data || {} : {}
                        if (code == 200) {
                            resolve(data ? JSON.parse(data) : null)
                        } else {
                            vm && vm.$message.error(message || '')
                        }
                    }).catch(err => {
                        vm && vm.$message.error(err.data, err)
                        reject({})
                    }).finally(() => {
                        _this[`${pluginName}-${menuName}`] = false
                    })
                } else {
                    let times = 0
                    let time = _this.setInterval(
                        () => {
                            if ((_this.i18n && _this.i18n.messages[sessionStorage.languageType] && _this.i18n.messages[sessionStorage.languageType][menuName]) || times > 300) {
                                _this.clearInterval(time)
                                time = null
                                _this.i18n.messages[sessionStorage.languageType][menuName] ? resolve(_this.i18n.messages[sessionStorage.languageType][menuName]) : reject({})
                            }
                            times = times + 1
                        }, 100
                    )
                }
            })
        }
        function haveExist (languageType, data) {
            let exist = false
            data.forEach(item => {
                if (item.value == languageType) {
                    exist = true
                }
            })
            return exist
        }
        async function updateLanguageType () {
            await _this.getLanguageOptions().then(async data => {
                if (!localStorage.haveSetLanguageType) {
                    if (haveExist(broswerLanguage[navigator.language], data)) {
                        localStorage.languageType = sessionStorage.languageType = broswerLanguage[navigator.language]
                    } else {
                        if (_this.configInfoData && _this.configInfoData.defaultLanguageType) {
                            localStorage.languageType = sessionStorage.languageType = _this.configInfoData.defaultLanguageType
                        } else {
                            await _this.getConfigInfoData().then(data => {
                                if (data.defaultLanguageType) {
                                    localStorage.languageType = sessionStorage.languageType = data.defaultLanguageType || 'zh-CN'
                                }
                            }).catch(err => {
                                console.log(err)
                            })
                        }
                    }
                }
            }).catch(err => {
                console.log(err)
            })

        }
        await updateLanguageType()
        _this.getLanguage = async function (pluginName, menuName, packageId, vm, i18n, url, callback) {
            if (_this.i18n && _this.i18n.messages[sessionStorage.languageType] && _this.i18n.messages[sessionStorage.languageType][menuName]) {
                i18n._vm.messages[sessionStorage.languageType][menuName] = _this.i18n.messages[sessionStorage.languageType][menuName]
                if (_this.i18n.messages[sessionStorage.languageType]['login']) {
                    i18n._vm.messages[sessionStorage.languageType]['publics'] = _this.i18n.messages[sessionStorage.languageType]['login'].publics
                    i18n._vm.messages[sessionStorage.languageType]['menuJson'] = _this.i18n.messages[sessionStorage.languageType]['login'].menuJson
                }
                if (vm) {
                    vm.$nextTick(() => {
                        i18n._vm.locale = sessionStorage.languageType
                    })
                }

            } else {
                await requestLanguage(pluginName, menuName, packageId, vm, url).then(res => {
                    if (res) {
                        if (!_this.i18n) {
                            _this.i18n = {
                                messages: {}
                            }
                        }
                        if (!_this.i18n.messages[sessionStorage.languageType]) {
                            _this.i18n.messages[sessionStorage.languageType] = {}
                        }
                        _this.i18n.messages[sessionStorage.languageType][menuName] = res
                        i18n._vm.messages[sessionStorage.languageType][menuName] = res
                        if (menuName.toLowerCase().includes('login')) {
                            i18n._vm.messages[sessionStorage.languageType]['publics'] = res.publics || {}
                            i18n._vm.messages[sessionStorage.languageType]['menuJson'] = res.menuJson || {}
                        }
                        if (vm) {
                            vm.$nextTick(() => {
                                vm.$i18n.locale = sessionStorage.languageType
                            })
                        }
                    }
                }).catch(err => {
                    console.log(err)
                })
            }

            if (callback) {
                callback()
            }

            let language = JSON.parse(JSON.stringify(i18n._vm.messages[sessionStorage.languageType || 'zh-CN'][menuName] || ''))

            return language
        }
    }
)(window.top)