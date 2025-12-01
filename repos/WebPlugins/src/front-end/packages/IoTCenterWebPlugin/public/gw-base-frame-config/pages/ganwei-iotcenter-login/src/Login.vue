<template>
    <el-row class="login" v-cloak>
        <div class="loginBg">
            <el-select v-if="showLangSelect" v-model="languageSelected" @change="langChange" class="languageSelect">
                <template slot="prefix">
                    <i class="iconfont icon-gw-icon-diqiu"></i>
                </template>
                <el-option v-for="(item, index) in langOptions" :label="item.name" :value="item.value" :key="index"> </el-option>
            </el-select>
            <img :src="require('./images/login-bg.jpg')" alt />
        </div>
        <declaration v-if="showDeclare" :serviceTerms="serviceTerms" @agree="agree" />
        <section class="main">
            <aside class="login_form">
                <img class="bgImage" :src="require('./images/login-bg.jpg')" />
                <div class="logo">
                    <img :src="mainImg" alt="" :class="{ invisible: !mainImg }" />
                </div>
                <p class="number">
                    <span>
                        <el-input v-model="userName" @keyup.enter.native="login" :placeholder="$t('login.input.inputAccount')" autocomplete="off" clearable>
                            <div slot="prefix">
                                <i class="iconfont icon-denglu_zhanghu"></i>
                            </div>
                        </el-input>
                    </span>
                </p>
                <p class="password">
                    <span>
                        <el-input type="password" v-model="userPwd" @keyup.enter.native="login" :placeholder="$t('login.input.inputPassword')" autocomplete="off" show-password>
                            <div slot="prefix">
                                <i class="iconfont icon-denglu_mima"></i>
                            </div>
                        </el-input>
                    </span>
                </p>
                <p class="pverificationCode" v-if="!IsIgnoreFalidateCode">
                    <span class="verificationCode">
                        <el-input v-model="verificationCode" @keyup.enter.native="login" :placeholder="$t('login.input.inputCode')" autocomplete="off" clearable>
                            <div slot="prefix">
                                <i class="iconfont icon-denglu_yanzhengma"></i>
                            </div>
                        </el-input>
                        <span class="codeLoading" v-loading="errorLoading">
                            <img id="code_img" @click="drawCode()" />
                        </span>
                    </span>
                </p>
                <p class="btnLogin">
                    <span class="loading" v-if="loading">
                        {{ $t('login.button.logining') }}
                    </span>
                    <span class="loading" @click.stop="login" v-if="!loading">
                        {{ $t('login.button.loginNow') }}
                    </span>
                </p>
            </aside>
        </section>
        <footer class="footer">
            <div class="footer-btn" @mouseenter="onMouseEnter($event)" @mouseleave="onMouseLeave($event)">
                <el-button id="maintain-btn" type="primary" size="mini" v-if="showMaintain" @click.stop="toMaintain()">
                    {{ $t('login.button.updateLisens') }}
                </el-button>
            </div>
        </footer>
    </el-row>
</template>

<script>
const declaration = () => import('./components/declaration.vue')
// import changeLanguage from 'gw-base-utils-plus/changeLanguage'
export default {
    // mixins: [changeLanguage],
    components: {
        declaration
    },
    data () {
        return {
            langOptions: [],
            IsIgnoreFalidateCode: false,
            userName: '',
            userPwd: '',
            verificationCode: '',
            loading: false,
            verificationKey: '',
            showDeclare: false,
            footer: '',
            mainImg: '/static/images/index-logo-src.svg',
            errorLoading: true,
            groupLoad: true,
            showMaintain: false,
            url: '',
            theme: '',
            showLangSelect: false,
            languageSelected: sessionStorage.languageType || '中文',
            serviceTerms: {}
        }
    },
    created () {
        this.loginCreated()
        this.getLanguageLogos()
    },
    mounted () {
        this.userName = ''
        this.userPwd = ''
        this.verificationCode = ''
        if (window.localStorage.showDeclare !== undefined && window.localStorage.showDeclare === 'false') {
            this.showDeclare = false
        }
    },
    methods: {
        loginCreated () {
            this.myUtils.configInfoData(this).then(webConfig => {
                window.top.document.title = webConfig.titleConfig.platName
                if (webConfig.img.platLogo) {
                    let link = document.createElement('link')
                    link.href = webConfig.img.platLogo
                    link.setAttribute('rel', 'icon')
                    link.setAttribute('type', 'image/x-icon')
                    window.top.document.head.prepend(link)
                }

                this.showLangSelect = webConfig.showLangSelect
                if (!this.showLangSelect || !window.sessionStorage.languageType) {
                    window.sessionStorage.languageType = 'zh-CN'
                }
                this.mainImg = webConfig.img.loginImg

                this.theme = localStorage.theme || 'dark'
                window.document.documentElement.setAttribute('data-theme', this.theme)
                if (!localStorage.theme) {
                    localStorage.setItem('theme', webConfig.theme.default)
                }
            })

            this.$api
                .IsIgnoreFalidateCode()
                .then(res => {
                    if (res.data.code === 200) {
                        this.IsIgnoreFalidateCode = res.data.data
                    } else {
                        this.$message.error(res.data ? res.data.message : 'error', res)
                    }
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                })

            if (!this.IsIgnoreFalidateCode) {
                this.drawCode()
            }
        },
        langChange (val) {
            // 语言切换
            this.i18n.setLocal(val)
            this.i18n.getLanguage('Ganweisoft.IoTCenter.Module.Login', 'ganwei-iotcenter-login', 'login', this)

        },

        getLanguageLogos () {
            this.$api.getsupportedcultures().then((res) => {
                if (res.data.code == 200) {
                    const data = res.data.data;
                    if (data) {
                        this.langOptions = data
                        window.localStorage.langOptions = JSON.stringify(this.langOptions)
                    }
                } else {
                    this.$message.error(res.data.message, res)
                }
            }).catch(err => {
                this.$message.error(err.data, err)
            })
        },

        drawCode () {
            this.verificationCode = ''
            this.errorLoading = true
            this.$api
                .getVerificationCode()
                .then(res => {
                    if (res.data.code == 200) {
                        let image = document.getElementById('code_img')
                        this.verificationKey = res.data.data.verificationKey
                        if (image) {
                            image.src = res.data.data.verificationCode
                        }
                        this.errorLoading = false
                    } else {
                        this.$message.error(res.data ? res.data.message : 'error', res)
                    }

                    this.groupLoad = false
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                    console.log(err)
                    this.groupLoad = false
                })
        },

        // 提示
        info (type, msg) {
            this.$message({
                title: msg,
                type: type
            })
        },
        async login () {
            // 登录操作
            if (this.loading) {
                return false
            }
            this.loading = true
            this.$message.closeAll()
            if (!this.userName && !this.userPwd) {
                this.$message.error(this.$t('login.tips.ACTAndPWDCantBeNull'))
                this.loading = false
                return false
            } else if (!this.userName && this.userPwd) {
                this.$message.error(this.$t('login.tips.ACTCantBeNull'))
                this.loading = false
                return false
            } else if (this.userName && !this.userPwd) {
                this.$message.error(this.$t('login.tips.PWDCantBeNull'))
                this.loading = false
                return false
            } else if (!this.IsIgnoreFalidateCode && !this.verificationCode) {
                this.$message.error(this.$t('login.tips.codeCantBeNull'))
                this.loading = false
                this.drawCode()
                return false
            }
            if (this.userName.indexOf(' ') > -1) {
                this.$message.error(this.$t('login.tips.ACTCantIncludeSpace'))
                this.loading = false
                return false
            }
            let name = this.userName +"|"+this.myUtils.getCurrentDate(2,'-')
            this.$api
                .login({
                    userName: await this.$getCode.RSAEncrypt(name),
                    password: await this.$getCode.RSAEncrypt(this.userPwd),
                    verificationKey: this.verificationKey,
                    verificationCode: this.verificationCode.toUpperCase().replace(/(^\s*)|(\s*$)/g, ''),
                    TimeZone: Intl?.DateTimeFormat()?.resolvedOptions()?.timeZone || 'Asia/Shanghai',
                    verificationType: 2

                    // isapp: false
                })
                .then(rt => {
                    let data = rt.data
                    if (data.code == 200) {
                        this.loginData = data
                        if (!data.data.userTermsService) {
                            this.showDeclare = true
                        } else {
                            this.afterLogin()
                        }
                    } else {
                        this.drawCode()
                        this.$message.error(rt.data.message, rt)
                    }
                    this.loading = false
                })
                .catch(err => {
                    try {
                        this.$message.error(err.data, err)
                    } catch (e) {
                        this.$message.error(this.$t('login.systemError'))
                    }
                    console.log(err)
                    this.loading = false
                    this.drawCode()
                })
        },
        error (nodesc, msg) {
            this.$Notice.error({
                title: this.$t('login.tips.loginTips'),
                desc: msg
            })
        },

        onMouseEnter (e) {
            this.showMaintain = true
        },
        onMouseLeave (e) {
            this.showMaintain = false
        },

        // 进入维护
        toMaintain () {
            window.sessionStorage.clear()
            this.$router.push('/Maintain')
        },

        // 确认条款
        async agree () {
            try {
                let res = await this.$api.addUserServer()
                if (res.data.code !== 200) {
                    this.$message.error(res.data.message, res)
                }
            } catch (err) {
                this.$message.error(err.data, err)
                console.log(err)
            }
            this.afterLogin()
        },

        afterLogin () {
            let data = this.loginData
            localStorage.menuActive = '' // 导航菜单高亮
            localStorage.breadcrumbList = '' // 面包屑导航
            localStorage.historicalList = '' // 历史导航菜单
            window.sessionStorage.userName = this.userName

            // this.$store.dispatch('reflashSet')
            // parent.vm.$store.state.gwToken

            window.localStorage.ac_session = ''
            sessionStorage.personState = false
            sessionStorage.roleName = data.data.roleName
            this.userName = ''
            this.userPwd = ''
            this.verificationCode = ''
            if (data.data.passwordPolicy.passwordPolicy != null) {
                window.sessionStorage.removeItem('passwordPolicy')
                switch (data.data.passwordPolicy.passwordPolicy) {
                    case 0:
                        this.$message.warning(this.$t('login.tips.PWDOverContactAdmin'))
                        break
                    case 1:
                        window.sessionStorage.passwordPolicy = 1
                        this.$message.warning(this.$t('login.tips.PWDOverTimelyModify'))
                        window.top.location.href = '/#/Index'
                        break

                    default:
                        let reminderDaysInAdvance = data.data.passwordPolicy.reminderDaysInAdvance
                        if (reminderDaysInAdvance != null) {
                            if (reminderDaysInAdvance > 0) {
                                this.$message.warning(`${this.$t('login.tips.PWDAlso')}${reminderDaysInAdvance}${this.$t('login.tips.dayExpire')}`)
                            } else if (reminderDaysInAdvance == 0) {
                                this.$message.warning(this.$t('login.tips.PWDOverTodayModify'))
                            } else if (reminderDaysInAdvance < 0) {
                                this.$message.warning(this.$t('login.tips.PWDOverTimelyModify'))
                            }
                        } else {
                            this.$message({
                                title: this.$t('login.tips.loginSuccess'),
                                type: 'success'
                            })
                        }

                        // window.top.location.href = window.top.location.href.split('jumpIframeLogin')[0] + 'Index'
                        window.top.location.href = '/#/Index'
                        break
                }
            } else {
                // 兼容以往版本
                // window.top.location.href = window.top.location.href.split('jumpIframeLogin')[0] + 'Index'
                window.top.location.href = '/#/Index'
            }
        }
    }
}
</script>
<style lang="scss" src="./css/login.scss" scoped></style>
