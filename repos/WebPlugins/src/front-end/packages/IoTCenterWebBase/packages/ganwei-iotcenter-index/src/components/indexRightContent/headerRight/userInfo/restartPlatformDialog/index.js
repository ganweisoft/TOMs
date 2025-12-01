import inputPassword from '@/components/inputPassword/index.vue'
export default {
    components: { inputPassword },
    props: {
        showDialog: {
            type: Boolean,
            default: false
        }
    },
    watch: {
        showDialog (val) {
            this.dialogVisible = val;
            setTimeout(() => {
                this.showPassword = true
            }, 100)
            if (!val) {
                this.$refs.form.resetFields()
            }
        },
        // 监听loading状态控制进度条显示
        isRebootWeb (value, newValue) {
            if (value) {
                this.increase()
            } else {
                this.increaseend()
            }
        }
    },
    data () {
        return {
            dialogVisible: false,
            form: {
                password: '',
                confirmPassword: '',
            },
            percentage: 0,
            timeStart: null,
            isRebootWeb: false,
            showPassword: false,
            loading: false
        }
    },
    computed: {
        rules () {
            return {
                password: [
                    {
                        type: 'string',
                        required: true,
                        message: this.$t('login.framePro.tips.inputPasswordTip'),
                        trigger: 'blur'
                    }
                ],
                confirmPassword: [
                    {
                        required: true,
                        trigger: 'blur',
                        validator: (rule, value, callback) => {
                            if (!value) {
                                callback(this.$t('login.framePro.tips.enterConfirmPas'));
                                return;
                            }
                            let isTheSame = this.form.password.trim() == this.form.confirmPassword.trim();
                            if (!isTheSame) {
                                callback(this.$t('login.framePro.tips.towPasAreNoSame'))
                                return
                            }
                            callback()
                        }
                    }
                ]
            }
        }
    },

    methods: {
        restartPlatform () {
            sessionStorage.restarting = true
            this.percentage = 0;
            this.$refs.form.validate(async (valid) => {
                if (valid) {
                    this.loading = true;
                    this.passwordLoading = true;
                    this.$api.HostVerifyLogin({
                        UserName: await this.$getCode.RSAEncrypt(sessionStorage.getItem('userName')),
                        Password: await this.$getCode.RSAEncrypt(this.form.password)
                    }).then(res => {
                        const { code, message } = res.data;
                        if (code === 200) {
                            this.closeDialog()
                            this.isRebootWeb = true
                            this.$api
                                .RebootHost()
                                .then(async res => {
                                    if (res.data.code == 200) {
                                        await this.GetServiceStatus()
                                    }
                                })
                                .catch(error => {
                                    this.isRebootWeb = false
                                    console.log('error:', error)
                                })
                        } else if (code === -1) {
                            this.$message.warning(message);
                            setTimeout(() => {
                                top.location.href = '/#/jumpIframeLogin/ganwei-iotcenter-login/Login';
                            }, 1000)
                        } else {
                            this.$message.error(message);
                        }
                    }).catch(err => {
                        this.$message.error(this.$t('login.framePro.tips.restartPlateformFail'));
                        console.log(err);
                    }).finally(() => {
                        this.passwordLoading = false;
                        this.confirmationVisible = false;
                        this.loading = false
                    })

                } else {
                    return false;
                }
            });
        },
        jumpLogin () {
            let url = (process.env.NODE_ENV === "development" ? this.$hostMap('ganwei-iotcenter-login') : '/ganwei-iotcenter-login/#/')
            window.location.href = url
            sessionStorage.restarting = false
        },
        async RebootWeb () {
            this.$api.RebootWeb().finally(() => {
                this.jumpLogin()
            })
        },
        GetServiceStatus () {
            this.$api
                .getServiceStatus()
                .then(async res => {
                    if (res.data.code === 200) {
                        this.isRebootWeb = false
                        this.$message.success(this.$t('login.framePro.tips.restartPlateformSuccess'))
                        this.RebootWeb()
                    } else {
                        setTimeout(() => {
                            this.GetServiceStatus()
                        }, 3000)
                    }
                })
                .catch(() => {
                    setTimeout(() => {
                        this.GetServiceStatus()
                    }, 3000)
                    // this.isRebootWeb = false
                })
        },
        increase () {
            this.timeStart = setInterval(() => {
                if (this.percentage < 95) {
                    this.percentage += 5
                }
                if (this.percentage > 100) {
                    this.percentage = 100
                }
            }, 1000)
        },
        increaseend () {
            this.percentage = 100
            clearInterval(this.timeStart)
        },
        closeDialog () {
            this.form = {
                password: '',
                confirmPassword: ''
            }
            this.$emit('closeDialog')
        }
    }
}