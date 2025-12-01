
export default {
    created () {
        let map = window?.getParameterMap() || {}
        if (process.env.NODE_ENV == "development" && map) {
            sessionStorage.languageType = localStorage.languageType = map.get("languageType")
            sessionStorage.userName = map.get("userName")
            sessionStorage.passwordPolicy = map.get("passwordPolicy")
            if(!sessionStorage.getItem('theme') || sessionStorage.getItem('theme') == 'null' || sessionStorage.getItem('theme') == 'undefined') {
                sessionStorage.setItem('theme', map.get("theme"))
            }
        }
    },
    methods: {
        jumpLogin () {
            let url = (process.env.NODE_ENV === "development" ? this.$hostMap('ganwei-iotcenter-login') : '/ganwei-iotcenter-login/#/')
            window.location.href = url
        },
    }
}
