<template>
    <div id="noAccess">
        <span>
            <span>
                {{ $t(netWorkLink) }}
                <a @click="quitLogin">{{ $t('login.noAccess.noAccess[1]') }}</a>
                {{ $t('login.noAccess.noAccess[2]') }}
            </span>
        </span>
    </div>
</template>
<script>
export default {
    data: function () {
        return {
            netWorkLink: '',
        }
    },

    watch: {
        '$i18n.messages' (val) {
            this.$forceUpdate()
        },
    },
    mounted () {
        let name = this.$route.query.name
        switch (name) {
            case '1':
                this.netWorkLink = 'login.noAccess.noAccess[0]';
                break;

            case '2':
                this.netWorkLink = 'login.noAccess.noAccess[3]';
                break;

            case '3':
                this.netWorkLink = 'login.noAccess.noAccess[5]';
                break;
        }
    },
    methods: {
        quitLogin () {
            let code = window.sessionStorage.getItem('iamAccessToken')
            window.sessionStorage.clear()
            try {
                // eslint-disable-next-line
                myJavaFun.OpenLocalUrl('login')
            } catch (e) {
                if (code == null || code == undefined || code == '') {
                    window.location.href = window.location.origin + '/'
                } else {
                    window.location.href = '/loginOut.html'
                }
            }
        },
    },
}
</script>

<style lang="scss" scoped>
#noAccess {
    width: 100%;
    height: 100%;
    font-size: 1rem;
    display: flex;
    justify-content: center;
    align-items: center;
    color: #bbb;

    a {
        margin: 0 2px;
        color: $btn-blue;
        cursor: pointer;
    }
}
</style>