export default {
    data () {
        return {
            safeTipsText: 'login.tips.safeLevel.unSafeMode',
            safeTipsTime: 5000,
            safeMode: 1,
            hasGetData: false
        }
    },
    inject: ['config', 'theme'],
    mounted () {
        this.setSafeMode();
        window.onmessage = (e) => {
            if (e.data.updateSafeMode) {
                this.setSafeMode()
            }
        }
    },
    methods: {
        setSafeMode () {
            this.$api
                .getSafeLevelByGateway()
                .then(rt => {
                    if (rt?.data?.code == 200) {
                        this.safeMode = rt?.data?.data || ''
                        if (this.safeMode == 2) {
                            this.safeTipsText = 'login.tips.safeLevel.safeMode'
                        } else if (this.safeMode == 1) {
                            this.safeTipsText = 'login.tips.safeLevel.unSafeMode'
                        }
                        if (this.safeMode == 2) {
                            setTimeout(() => {
                                this.safeTipsText = ''
                            }, this.safeTipsTime)
                        }
                    }
                })
                .catch(e => {
                    console.log(e)
                }).finally(() => {
                    this.hasGetData = true
                })
        }
    }
}