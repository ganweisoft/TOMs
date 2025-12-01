/*
 * @Description:
 * @LastEditTime: 2024-09-30 18:19:44
 */
export default {
    inject: ['config', 'theme'],
    data() {
        return {
            dialogVisible: false,
            selectedTheme: sessionStorage.getItem('theme'),
            historyTheme: '',
            saveLoading: false,
            themeList: []
        }
    },
    mounted() {
        let currentList = this?.config?.theme?.supportThemes
        this.themeList = currentList.filter(item => {
            return item.enable != false
        })
    },
    methods: {
        changeTheme(theme){
            this.checkStyle(theme)
        },
        checkStyle(theme) {
            sessionStorage.setItem('theme', theme)
            this.myUtils.changeStyle()
            this.$bus.emit('themeChange')
            this.selectedTheme = theme
            // this.sendAiTheme()
        },
        showDialog () {
            this.historyTheme = sessionStorage.getItem('theme')
            this.dialogVisible = true
        },
        closeDialog () {
            this.dialogVisible = false
            this.checkStyle(this.historyTheme)
        },
        saveTheme () {
            sessionStorage.setItem('theme', this.selectedTheme)
            localStorage.setItem('theme', this.selectedTheme)
                // this.sendAiTheme()
            this.dialogVisible = false
        }
    }
}
