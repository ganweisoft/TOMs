
import logoutDialog from './logoutDialog/index.vue'
// import PWEditDialog from './PWEditDialog/index.vue'
export default {
    components: {
        logoutDialog,
        // PWEditDialog,

    },
    inject: ['config', 'theme'],
    data () {
        return {
            // 登录者姓名
            loginUsername: window.sessionStorage.userName,
            showLogoutDialog: false,
            showPWEditDialog: false,
            showSystemInfoDialog: false,
            passwordPolicy: '',
            isAdmin: false,
            showRestartPlatform: false,
            showSafeModeDialog: false,
            organizationName: ''
        }
    },
    mounted () {
        this.passwordPolicy = sessionStorage.passwordPolicy || ''
    },
    methods: {
        openDialog (dialog) {
            this[dialog] = true
        },
        closeDialog (dialog) {
            this[dialog] = false
        },
        updateApplicationAuth () {
            this.$msgbox({
                title: this.$t('login.framePro.title.updateApplicationAuth'),
                message: this.$t('login.framePro.tips.updateApplicationAuthTip'),
                showCancelButton: true,
                type: 'warning',
                confirmButtonText: this.$t('login.publics.button.confirm'),
                cancelButtonText: this.$t('login.publics.button.cancel'),
                beforeClose: (action, instance, done) => {
                    if (action === 'confirm') {
                        instance.confirmButtonLoading = true;
                        setTimeout(() => {
                            this.$api.updateApplicationAuth().then(res => {
                                const { code, message } = res?.data || {}
                                if (code == 200) {
                                    this.$message.success(message)
                                } else {
                                    this.$message.warning(message)
                                }
                            }).finally(() => {
                                instance.confirmButtonLoading = false;
                                done()
                            })
                        }, 120);
                    } else { done() }
                }
            })
        }
    }
}
