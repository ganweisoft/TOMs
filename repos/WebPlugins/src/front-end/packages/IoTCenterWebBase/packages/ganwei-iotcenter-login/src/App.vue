<template>
    <div class="app">
        <el-config-provider :locale="locale">
            <router-view v-cloak/>
        </el-config-provider>
    </div>
</template>
<script>
import setDefaultLanguage from './mixins/setDefaultLanguage'
export default {
    mixins: [setDefaultLanguage],
    name: 'App',
    data () {
        return {
            isNoRequestObserveGetPublic: true
        };
    },
    mounted () {
        document.documentElement.setAttribute('class', localStorage.theme || 'dark')
    }
};
</script>
<style lang="scss">
html,
body,
#app,.app {
    width: 100%;
    height: 100%;
    padding: 0!important;
    overflow: hidden;
}

.el-select-dropdown {
    background-color: #fff;
    border: none !important;

    .el-select-dropdown__item {
        color: #333 !important;
    }
}

.el-message {
    background-color: #fff;
    border-color: #fff;
}

.el-popper .el-popper__arrow::before,
.el-select__popper.el-popper[data-popper-placement^=bottom] .el-popper__arrow::before {
    border-color: white !important;
    background-color: white !important
}

::-webkit-scrollbar-thumb {
    background-color: #e1e3e6 !important;
}

input:-webkit-autofill {
    -webkit-text-fill-color: black;
}

.el-message-box {
    background-color: white !important;

    .el-message-box__title,
    .el-message-box__message {
        color: #333 !important;
    }
}
</style>
