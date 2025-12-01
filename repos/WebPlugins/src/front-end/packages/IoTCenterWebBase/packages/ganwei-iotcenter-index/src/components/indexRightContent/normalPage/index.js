export default {

    data () {
        return {
            breadcrumbList: []
        }
    },
    inject: ['routesToName'],
    mounted () {
        localStorage.editableTabsValue = 'normalPage'
        this.$bus.on('openPage', data => {
            const { changePath, callback } = data
            this.$router.push(changePath)
            if (callback) {
                this.$nextTick(() => {
                    callback()
                })
            }
        })
    },
    watch: {
        $route: {
            handler (to) {
                let activeName = this.routesToName[to.fullPath];
                this.breadcrumbList = [
                    {
                        title: activeName
                    }
                ]
            },
            immediate: true
        }
    }
}