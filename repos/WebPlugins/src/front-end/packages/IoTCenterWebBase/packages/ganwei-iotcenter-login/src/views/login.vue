<template>
    <div class="login" v-cloak>
        <div class="backgroundImage">
            <img :src="homeBgImg" alt="" :class="{ invisible: !homeBgImg }" @error="showBgImg" v-if="homeBgImg"/>
        </div>
        <el-select v-if="showLangSelect" v-model="languageSelected" @change="langChange" class="languageSelect">
            <template v-slot:prefix>
                <i class="iconfont icon-gw-icon-diqiu"></i>
            </template>
            <el-option v-for="(item, index) in langOptions" :label="item.name" :value="item.value" :key="index">
            </el-option>
        </el-select>
        <section class="main" >
            <aside class="login_form">
                <div class="logo">
                    <img :src="mainImg" alt="" :class="{ invisible: !mainImg }" @error="showImg" />
                </div>
                <el-form ref="loginForm" :model="form" label-width="0px" :rules="rules">
                    <el-form-item prop="userName">
                        <el-input v-model="form.userName" @keyup.enter.native="showSlideCode"
                            :placeholder="txtTips[0]" autocomplete="off" clearable>
                            <template #prefix>
                                <i class="iconfont icon-denglu_zhanghu"></i>
                            </template>
                        </el-input>
                    </el-form-item>
                    <el-form-item prop="userPwd">
                        <el-input type="password"  v-model="form.userPwd" @keyup.enter.native="showSlideCode"
                            :placeholder="txtTips[1]" autocomplete="off">
                            <template #prefix>
                                <i class="iconfont icon-denglu_mima"></i>
                            </template>
                            <template #suffix>
                               <showPassword :userPwd="form.userPwd"></showPassword>
                             </template>
                        </el-input>
                    </el-form-item>
                    <el-form-item v-if="!IsIgnoreFalidateCode && verificationType != 1" prop="verificationCode">
                        <div class="verificationCode">
                            <el-input v-model="form.verificationCode" @keyup.enter.native="login"
                                :placeholder="txtTips[2]" autocomplete="off" clearable>
                                <template #prefix>
                                    <i class="iconfont icon-denglu_yanzhengma"></i>
                                </template>
                            </el-input>
                            <span class="codeLoading" v-loading="errorLoading">
                                <img id="code_img" @click="drawCode()" />
                            </span>
                        </div>
                    </el-form-item>

                    <!--  忘记密码功能暂时注释
                    <div class="loginExtend">
                      <el-button class="loginExtendButton" @click="forgotPassworrd">{{ txtTips[10] }}?</el-button>
                    </div>
                    -->
                </el-form>
                <el-button type="primary" class="submit" :disabled="loading" @click.stop="showSlideCode">
                    <span v-if="loading && !isLogin">{{ txtTips[3] }}</span>
                    <span v-if="isLogin && !loading">{{ txtTips[4] }}</span>
                    <span v-if="!isLogin && !loading">{{ txtTips[5] }}</span>
                </el-button>
            </aside>
        </section>
        <declaration v-if="showDeclare" @agree="agree" />
        <passwordModification :passwordDialogVisible="passwordDialogVisible" @closePassword="closePassword" />
    </div>
</template>

<script>
import declaration from '@/components/declaration/declaration.vue'
import passwordModification from '@/components/passwordModification/passwordModification.vue'
import { useI18n } from 'vue-i18n'
import showPassword from '@components/@ganwei-pc/gw-base-components-plus/showPassword/indexOption.vue'
export default {
    components: {
        declaration,
        passwordModification,
        showPassword
    },
    data () {
        return {
            form: {
                userName: '',
                userPwd: '',
                verificationCode: '',
                verificationKey: '',
            },
            verificationType: 1,
            isLogin: false,
            langOptions: [],
            IsIgnoreFalidateCode: true,
            loading: false,
            showDeclare: false,
            mainImg: '',
            homeBgImg: '',
            errorLoading: true,
            showMaintain: false,
            showLangSelect: false,
            languageSelected: '',
            outerHeight: '',
            loginFormHeight: '',
            i18nInstance: null,
            defaultTheme: '',
            txtTips: [],
            passwordDialogVisible: false
        }
    },

    watch: {
        txtTips (val) {
            setTimeout(() => {
                if (this.$refs.loginForm) {
                    this.$refs.loginForm.clearValidate()
                }
            }, 50)
        }
    },

    computed: {
        rules () {
            return {
                userName: [
                    { required: true, message: this.txtTips[7], trigger: 'blur' },
                ],
                userPwd: [
                    { required: true, message: this.txtTips[8], trigger: 'blur' },
                ],
                verificationCode: [
                    { required: true, message: this.txtTips[9], trigger: 'blur' },
                ],
            }
        }
    },

    created () {
        this.loginCreated()
        //this.mainImg = sessionStorage.mainImg
        this.langOptions = localStorage.langOptions ? JSON.parse(localStorage.langOptions) : []
        this.languageSelected = localStorage.languageType
        this.i18nInstance = useI18n()
        this.initTxtTips()
    },
    mounted () {
        window.showSlideCode = this.showSlideCode.bind(this)
        window.slideCodeLogin = this.slideCodeLogin.bind(this)
    },
    methods: {
        initTxtTips(){
            this.txtTips = [
                this.$t('login.input.inputAccount'),
                this.$t('login.input.inputPassword'),
                this.$t('login.input.inputCode'),
                this.$t('login.button.logining'),
                this.$t('login.button.enterSystem'),
                this.$t('login.button.loginNow'),
                this.$t('login.button.updateLisens'),
                this.$t('login.tips.ACTCantBeNull'),
                this.$t('login.tips.PWDCantBeNull'),
                this.$t('login.tips.codeCantBeNull'),
                this.$t('login.button.forgotPassword')
            ]
        },
        getImageUrlWithoutAuth(originUrl, imageCode) {
            // 静态文件路径
            if(originUrl.startsWith('/static/images')) {
                return originUrl
            }

            // 上传图片路径
            if(originUrl.startsWith('/file/')) {
                return `/IoT/api/v3/FrontConfiguration/DownloadImage?subject=${imageCode}&fileDir=${originUrl}`
            }
            return ''
        },
        async loginCreated () {
                if (window.top.getConfigInfoData) {
                    await window.top.getConfigInfoData().then(webConfig => {
                        this.defaultTheme = webConfig?.theme?.default || "dark"
                        this.showLangSelect = webConfig?.showLangSelect  // 默认选项
                        this.mainImg = this.getImageUrlWithoutAuth(webConfig?.img?.loginImg, "loginImg")
                        this.homeBgImg = this.getImageUrlWithoutAuth(webConfig?.img?.loginBgImg || "/static/images/login-bg-img.png", 'loginBgImg')
                        sessionStorage.setItem("bgImg", this.homeBgImg)
                        this.verificationType = (webConfig?.verificationType) ? 1 : 0
                    }).catch((err) => {
                        console.log(err)
                    })
                }

            // 验证码配置
            this.IsIgnoreFalidateCode = false
            this.drawCode()
            this.outerHeight = 518
            this.loginFormHeight = 518
        },
        async langChange (val) {
            // 语言切换
            try {
                localStorage.languageType = sessionStorage.languageType = val
                sessionStorage.haveSetLanguageType = true
                this.$i18n.locale = val
                this.initTxtTips()
            } catch (error) {
                console.log(error)
            }
        },
        drawCode () {
            if (this.IsIgnoreFalidateCode || this.verificationType == 1) {
                return;
            }
            this.form.verificationCode = ''
            this.errorLoading = true
            this.$api
                .getVerificationCode()
                .then(res => {
                    const { code, data, message } = res?.data || {}
                    if (code == 200) {
                        let image = document.getElementById('code_img')
                        this.form.verificationKey = data?.verificationKey || ''
                        if (image) {
                            image.src = data?.verificationCode || ''
                        }
                        this.errorLoading = false
                    } else {
                        this.$message.error(message)
                    }

                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                })
        },

        // 提示
        info (type, msg) {
            this.$message({
                title: msg,
                type: type
            })
        },
        login () {
            // 登录操作
            this.$refs.loginForm.validate(async valid => {
                if (valid) {
                    this.form.userName = this.form.userName.trim()
                    this.form.userPwd = this.form.userPwd.trim()
                    this.loading = true
                    this.$message.closeAll()
                    let name = this.form.userName //+ "|" + this.myUtils.getCurrentDate(2, '-')
                    this.$api
                        .login({
                            userName: await this.$getCode.RSAEncrypt(name),
                            password: await this.$getCode.RSAEncrypt(this.form.userPwd),
                            verificationKey: this.form.verificationKey,
                            verificationCode: this.verificationType == 1 ? this.form.verificationCode : this.form.verificationCode?.toUpperCase()?.replace(/(^\s*)|(\s*$)/g, ''),
                            //TimeZone: Intl?.DateTimeFormat()?.resolvedOptions()?.timeZone || 'Asia/Shanghai',
                            verificationType: this.verificationType
                        })
                        .then(res => {
                            const { code, data, message } = res?.data || {}
                            if (code == 200) {
                                this.isLogin = true
                                this.loginData = data || {}
               
                                    if(this.verificationType == 1 && !this.IsIgnoreFalidateCode) {
                                        window.slideAuthSuccess()
                                    }
                                    this.afterLogin()
                         
                            } else {
                                this.isLogin = false
                                if(!this.IsIgnoreFalidateCode) {   // 未关闭验证码
                                    if(this.verificationType == 1) {  // 滑块验证码
                                        if(code === 40005 || code === 40001 || code == 400) { // 账号密码错误，关闭弹窗
                                            window.popupHandle("coverUp", "__Verification", "flipInX");
                                        } else {
                                            window.slideAuthError()
                                        }
                                    } else {
                                        this.drawCode()
                                    }
                                }
                                this.$message.error(message)
                            }
                        })
                        .catch(err => {
                            try {
                                this.$message.error(err.data, err)
                            } catch (e) {
                                this.$message.error(this.$t('login.systemError'))
                            }
                            if(!this.IsIgnoreFalidateCode) {
                                if(this.verificationType == 1) {
                                    window.slideAuthError()
                                } else {
                                    this.drawCode()
                                }
                            }
                            this.isLogin = false
                        }).finally(() => {
                            this.loading = false
                        })
                }
            })

        },

        // 确认条款
        async agree () {
            try {
                let res = await this.$api.addUserServer()
                if (res?.data?.code !== 200) {
                    this.$message.error(res?.data?.message)
                }
            } catch (err) {
                this.$message.error(err?.data, err)
            }
            this.afterLogin()
        },

        afterLogin () {
            let data = this.loginData
            window.sessionStorage.userName = this?.form?.userName
            let userRetrunInfo = this.filterUserInfo(data?.personReserve)
            if(userRetrunInfo?.userTheme) {
                sessionStorage.setItem('theme', userRetrunInfo?.userTheme)
            } else {
                localStorage.setItem('theme', this.defaultTheme || "dark")
                sessionStorage.setItem('theme', this.defaultTheme || "dark")
            }
            if (data?.passwordPolicy?.passwordPolicy != null) {
                window.sessionStorage.removeItem('passwordPolicy')
                // 密码规则 0：禁止登录 1：强制修改 2：不限制登录
                switch (data.passwordPolicy.passwordPolicy) {
                    case 0:
                        this.$message.warning(this.$t('login.tips.PWDOverContactAdmin'))
                        break
                    case 1:
                        window.sessionStorage.passwordPolicy = 1
                        this.$message.warning(this.$t('login.tips.PWDOverTimelyModify'))
                        setTimeout(() => {
                            if (process.env.NODE_ENV === "development") {
                                window.top.location.href = this.$hostMap('ganwei-iotcenter-index') + "?languageType=" + localStorage.languageType + "&userName=" + window.sessionStorage.userName + "&passwordPolicy=" + window.sessionStorage.passwordPolicy + "&theme=" + sessionStorage.getItem('theme') + "&userId=" + window.sessionStorage.userId + "&CSRF_TOKEN=" + window.sessionStorage.CSRF_TOKEN
                            } else { window.top.location.href = '/index.html' }
                        }, 250)
                        break

                    default:
                        let reminderDaysInAdvance = data.passwordPolicy.reminderDaysInAdvance  // 提前多少天提醒
                        if (reminderDaysInAdvance != null) {
                            if (reminderDaysInAdvance > 0) {
                                this.$message.warning(`${this.$t('login.tips.PWDAlso')}${reminderDaysInAdvance}${this.$t('login.tips.dayExpire')}`)
                            } else if (reminderDaysInAdvance == 0) {
                                this.$message.warning(this.$t('login.tips.PWDOverTodayModify'))
                            } else if (reminderDaysInAdvance < 0) {
                                this.$message.warning(this.$t('login.tips.PWDOverTimelyModify'))
                            }
                        } else {
                            this.$message.success(this.$t('login.tips.loginSuccess'))
                        }
                        setTimeout(() => {
                            if (process.env.NODE_ENV === "development") {
                                window.top.location.href = this.$hostMap('ganwei-iotcenter-index') + "?languageType=" + localStorage.languageType + "&userName=" + window.sessionStorage.userName + "&passwordPolicy=" + window.sessionStorage.passwordPolicy + "&theme=" + sessionStorage.getItem('theme') + "&userId=" + window.sessionStorage.userId + "&CSRF_TOKEN=" + window.sessionStorage.CSRF_TOKEN
                            } else { window.top.location.href = '/index.html' }
                        }, 250)

                        break
                }
            } else {
                this.$message.success(this.$t('login.tips.loginSuccess'))
                setTimeout(() => {
                    if (process.env.NODE_ENV === "development") {
                        window.top.location.href = this.$hostMap('ganwei-iotcenter-index') + "?languageType=" + localStorage.languageType + "&userName=" + window.sessionStorage.userName + "&passwordPolicy=" + window.sessionStorage.passwordPolicy + "&theme=" + sessionStorage.getItem('theme') + "&userId=" + window.sessionStorage.userId + "&CSRF_TOKEN=" + window.sessionStorage.CSRF_TOKEN
                    } else { window.top.location.href = '/index.html' }
                }, 250)
            }
        },

        showImg () {
            if (process.env.NODE_ENV === "development") { this.mainImg = `/static/images/index-logo-src.svg` }
        },

        showBgImg(){
            this.homeBgImg = "/static/images/login-bg-img.png"
        },

        filterUserInfo (item) {
            let obj = {}
            if(Array.isArray(item) && item.length > 0) {
                for (let i of item) {
                    if(i.key && i.value) {
                        obj[i.key] = i.value
                    }
                }
            }
            return obj
        },

        showSlideCode () {
            this.$refs.loginForm.validate(valid => {
                if (valid) {
                    // 滑块验证 且 未关闭验证码
                    if(this.verificationType == 1 && !this.IsIgnoreFalidateCode) {
                        // 请完成安全验证 拖动图片验证 验证成功  点击刷新验证码
                        sessionStorage.setItem('slideTitle', this.$t('login.tips.slideTitle'))
                        sessionStorage.setItem('slideTipsLabel', this.$t('login.tips.slideTipsLabel'))
                        sessionStorage.setItem('slideSuccessLabel', this.$t('login.tips.slideSuccessLabel'))
                        sessionStorage.setItem('slideClickLabel', this.$t('login.tips.slideClickLabel'))
                        window.changespec("300*200", '/IoT/api/v3/Auth/GetSlideVerificationCode');
                    } else {
                        this.login()
                    }
                }
            })
        },

        slideCodeLogin(point){
            this.form.verificationCode = point
            this.form.verificationKey = sessionStorage.getItem('verificationKey')
            sessionStorage.removeItem('verificationKey')
            this.login()
        },

        // 忘记密码
        forgotPassworrd(){
            // 清除邮箱
            window.sessionStorage.removeItem("eamilUrl");
            this.passwordDialogVisible = true
        },

        // 关闭密码弹窗
        closePassword(){
            this.passwordDialogVisible = false
        }

    }
}
</script>
<style lang="scss" src="@/assets/css/login.scss" scoped></style>
