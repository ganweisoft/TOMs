export default {
    props: {
        showDialog: {
            type: Boolean,
            default: false
        }
    },

    data () {
        return {
            dialogVisible: false,
            submitLoading: false
        }
    },
    watch: {
        showDialog (val) {
            this.dialogVisible = val;
        },
        dialogVisible(val) {
            if(!val) this.closeDialog()
        }
    },
    methods: {
        quitLogin (typeName) {
            let isSsoLogin = window.sessionStorage.getItem('isSsoLogin')
            if (isSsoLogin) {
                this.$router.replace('/ganwei-iotcenter-login/ssoLogout')
                window.sessionStorage.clear()
                return
            }
            this.submitLoading = true
            typeName = typeName == 'get' ? 'get' : 'post'
            this.$api.loginOut(typeName).then(rt => {
                if (rt?.data?.code == 200) {
                    // 保留中英文操作记录
                    let keys = Object.keys(sessionStorage);
                    keys.forEach(key => {
                    if (key !== 'languageType') {
                        sessionStorage.removeItem(key);
                    }
                    });
                    this.$message.success(this.$t('login.framePro.tips.loggedOut'))
                    setTimeout(() => {
                        if (isSsoLogin == null || isSsoLogin == undefined || !isSsoLogin) {
                            if (process.env.NODE_ENV === "development") {
                                window.top.location.href = this.$hostMap('ganwei-iotcenter-login')
                            } else { window.top.location.href = window.top.location.origin + '/ganwei-iotcenter-login' }
                        } else {
                            this.$router.replace('/ganwei-iotcenter-login/ssoLogout')
                        }
                    }, 250)
                } else {
                    this.$message.error(this.$t('login.framePro.tips.logOutFail'))
                }
            }).catch(er=>{
                if(er.status == 405 && typeName == "post"){
                    this.quitLogin ('get')
                }
            }).finally(() => {
                this.submitLoading = false
            })
        },
        closeDialog () {
            this.$emit('closeDialog')
        }
    }

}
