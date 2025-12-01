import screenfull from 'screenfull'
export default {
    inject: ['config', 'theme'],
    mounted () {
        this.$bus.on('contentFullScreen', (type) => {
            this.getContentFullscreen(type)
        })
    },
    methods: {
        getContentFullscreen (type) {
            if (screenfull.isEnabled) {
                if ((!screenfull.isFullscreen && type == true) || (screenfull.isFullscreen && type == false)) {
                    let contentFrame = document.getElementById(`pane-${localStorage.editableTabsValue}`)
                    screenfull.toggle(contentFrame)
                }
            }
        },
    }
}