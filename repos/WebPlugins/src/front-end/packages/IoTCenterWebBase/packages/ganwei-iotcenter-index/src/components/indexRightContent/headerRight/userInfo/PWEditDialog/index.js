import inputPassword from '@/components/inputPassword/index.vue'

import forceEdit from './forceEdit'
export default {
    components: {
        inputPassword
    },
    mixins: [forceEdit],
    props: {
        showDialog: {
            type: Boolean,
            default: false
        },
        passwordPolicy: {
            type: [Number, String],
            default: ''
        }
    },
    watch: {
        showDialog (val) {
            if (val) {
                this.getRule()
            }
            this.dialogVisible = val;
        }
    },
    data () {
        return {
            dialogVisible: false,
            changePwd: {
                oldPwd: '',
                newPwd: '',
                confPwd: ''
            },
            // 修改密码校验规则
            regPwdLength: /./,
            regPwd1: /./,
            regPwd2: new RegExp('[\u4E00-\u9FFF]+'),
            regPwd3: /^[\S]*$/,

            regPwdLengthMsg: '',
            regPwdMsg: '',
            pwdHaveName: undefined,
            minCharacters: 0,
            pwdCharacters: 0,
            pwdMinCharactersMsg: undefined,
        }
    },
    computed: {
        rules () {
            return {
                oldPwd: [
                    {
                        type: 'string',
                        required: true,
                        validator: (rule, value, callback) => {
                            if (!value) {
                                callback(new Error(this.$t('login.framePro.tips.enterOriginPas')))
                            }
                            callback()
                        },
                        trigger: 'blur'
                    }
                ],
                newPwd: [
                    {
                        validator: this.validatePass,
                        required: true,
                        trigger: 'blur'
                    }
                ],
                confPwd: [
                    {
                        validator: this.validateConfPass,
                        required: true,
                        trigger: 'blur'
                    }
                ]
            }
        }
    },

    methods: {
        changeCode () {
            this.$refs['changePasswordForm'].validate(async valid => {
                if (valid) {
                    let data = {
                        userName: window.sessionStorage.userName,
                        oldPassword: await this.$getCode.RSAEncrypt(this.changePwd.oldPwd),
                        newPassword: await this.$getCode.RSAEncrypt(this.changePwd.newPwd)
                    }
                    this.$api.getUpdUserInfoData(data).then(rt => {
                        let code = rt?.data?.code
                        if (code == 200) {
                            this.$message.success(this.$t('login.framePro.tips.changeSuccess'))
                            this.quitLogin()
                        } else if (code == 5001) {
                            this.$message.warning(rt?.data?.message)
                            return
                        } else if (code == -100) {
                            this.$message.error(rt?.data?.message)
                            setTimeout(() => {
                                this.quitLogin()
                            }, 1000)
                        } else {
                            if (rt?.data?.message) {
                                this.$message.error(
                                    rt?.data?.message
                                )
                            }
                        }
                    })
                }
            })
        },
        quitLogin (typeName) {
            let isSsoLogin = window.sessionStorage.getItem('isSsoLogin')
            if (isSsoLogin) {
                this.$router.replace('/ganwei-iotcenter-login/ssoLogout')
                window.sessionStorage.clear()
                return
            }
            typeName = typeName == 'get' ? 'get' : 'post'
            this.$api.loginOut(typeName).then(rt => {
                if (rt?.data?.code == 200) {
                    window.sessionStorage.clear()
                    this.$message.success(this.$t('login.framePro.tips.loggedOut'))
                    if (isSsoLogin == null || isSsoLogin == undefined || !isSsoLogin) {
                        window.location.href = window.location.origin + '/ganwei-iotcenter-login'
                    } else {
                        this.$router.replace('/ganwei-iotcenter-login/ssoLogout')
                    }
                } else {
                    this.$message.error(this.$t('login.framePro.tips.logOutFail'))
                }
            }).catch(er=>{
                if(er.status == 405 && typeName == "post"){
                    this.quitLogin ('get')
                }
            })
        },

        pwdInputChange (val) {
            if (window.sessionStorage.accountRule) {
                let rule = JSON.parse(window.sessionStorage.accountRule)
                if (rule.password.minCharacters != 0) {
                    let len = val.length
                    let arr = []
                    for (let i = 0; i < len; i++) {
                        if (arr.indexOf(val[i]) == -1) {
                            arr.push(val[i])
                        }
                        this.pwdCharacters = arr.length
                        if (arr.length >= rule.password.minCharacters) {
                            break
                        }
                    }
                }
            }
        },
        getRule () {
            this.$api
                .GetAccountPasswordRule()
                .then(res => {
                    const { code, data, message } = res?.data || {}
                    if (code === 200) {
                        if (data) {
                            window.sessionStorage.accountRule = JSON.stringify(data || '{}')
                            this.setRule()
                        }
                    } else if (message) {
                        this.$message.error(message)
                    }
                })
                .catch(er => {
                    console.log(er)
                })
        },

        setRule () {
            let rule = JSON.parse(window.sessionStorage.accountRule)
            let regArr = ['(?=.*[a-z])', '(?=.*[A-Z])', '(?=.*[0-9])', '(?=.*[`~!@#$%^&*()_+={}|:;"\'<>,.?/-])']
            let msgArr = [
                this.$t('login.framePro.tips.lowercaseLetter'),
                this.$t('login.framePro.tips.capitalize'),
                this.$t('login.framePro.tips.number'),
                this.$t('login.framePro.tips.specialSymbols')
            ]

            // 密码规则
            if (rule.password.enabled === true) {
                // 密码长度
                this.regPwdLength = new RegExp('^.{' + rule.password.length + ',32}$')
                this.regPwdLengthMsg = this.$t('login.framePro.tips.passwordLength') + `${rule.password.length}-32` + this.$t('login.framePro.tips.characterNumber')

                // 密码中必须包含的类型
                let regStr = '^'
                let newMsgArr = []
                for (let item of rule.password.elements) {
                    regStr += regArr[item]
                    newMsgArr.push(msgArr[item])
                }
                this.regPwd1 = new RegExp(regStr)
                this.regPwdMsg = this.$t('login.framePro.tips.passwordHas') + newMsgArr.join('、')

                if (!rule.password.allowedUserName) {
                    this.pwdHaveName = this.$t('login.framePro.tips.passwordNoHasUser')
                } else {
                    this.pwdHaveName = undefined
                }

                if (rule.password.minCharacters != 0) {
                    this.minCharacters = rule.password.minCharacters
                    this.pwdMinCharactersMsg = new Error(this.$t('login.framePro.tips.containAtLeast') + `${rule.password.minCharacters}` + this.$t('login.framePro.tips.differentCharacters'))
                } else {
                    this.pwdMinCharactersMsg = undefined
                }
            } else {
                this.regPwdLength = /./
                this.regPwd1 = /./
            }
        },

        validatePass (rule, value, callback) {
            if (value === '') {
                callback(new Error(this.$t('login.framePro.tips.inputNewPassword')))
            } else if (!this.regPwdLength.test(value)) {
                callback(new Error(this.regPwdLengthMsg))
            } else if (value != '' && !this.regPwd1.test(value)) {
                callback(new Error(this.regPwdMsg))
            } else if (this.regPwd2.test(value)) {
                callback(new Error(this.$t('login.framePro.tips.noChinese')))
            } else if (!this.regPwd3.test(value)) {
                callback(new Error(this.$t('login.framePro.tips.noSpaces')))
            } else if (this.pwdCharacters < this.minCharacters) {
                callback(this.pwdMinCharactersMsg)
            } else if (value == this.changePwd.oldPwd) {
                callback(new Error(this.$t('login.framePro.tips.newAndOldPwd')))
            } else if (value.indexOf(window.sessionStorage.userName) != -1 && window.sessionStorage.userName != '') {
                callback(this.pwdHaveName)
            } else {
                callback()
            }
        },

        validateConfPass (rule, value, callback) {
            if (value === '') {
                callback(new Error(this.$t('login.framePro.tips.enterConfirmPas')))
            } else if (value != this.changePwd.newPwd) {
                callback(new Error(this.$t('login.framePro.tips.towPasAreNoSame')))
            } else {
                callback()
            }
        },

        closeDialog () {
            if (this.passwordPolicy != 1) {
                this.changePwd.oldPwd = ''
                this.changePwd.newPwd = ''
                this.changePwd.confPwd = ''
                this.$refs['changePasswordForm'].resetFields()
                this.$emit('closeDialog')
            }
        }
    }
}
