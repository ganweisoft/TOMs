
<template>
    <div class="app">
        <el-config-provider :locale="locale">
            <router-view v-cloak  />
        </el-config-provider>
    </div>
</template>

<script>
import enUs from 'element-plus/es/locale/lang/en'; // 引入中文语言包
import zhCn from 'element-plus/es/locale/lang/zh-cn'; // 引入中文语言包

import judgePermission from './mixins/judgePermission'
export default {
    mixins: [judgePermission],
    name: 'App',
    data () {
        return {
            locale: localStorage.languageType == "en-US" ? enUs : zhCn
        }
    },
    mounted () {
        if(sessionStorage.getItem('theme') && sessionStorage.getItem('theme') != 'null' && sessionStorage.getItem('theme') != 'undefined') {
            let style = sessionStorage.getItem('theme')
            window.document.documentElement.classList.add(style || 'dark')
        }
        sessionStorage.restarting = false;
    }
}
</script>

<style>
html {
    height: 100%;
    --left: calc(var(--maxActiveWidth) + 10);
    --maxActiveWidth: 220;
    --result: calc(var(--maxActiveWidth) - 64);
}

html body {
    height: 100%;
    font-family: 'Microsoft YaHei';
    overflow: auto;
}

a:focus,
input:focus,
p:focus,
div:focus {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
}

html body div {
    box-sizing: border-box;
}

.app,
#app {
    min-width: 900px;
    height: 100%;
    overflow: hidden;
}

.topToolTip {
    color: var(--frame-main-color) !important;
    border: 1px solid var(--poper-border) !important;
}

.topToolTip .el-popper__arrow::before {
    background: var(--select-dropdown-background) !important;
    border: 1px solid var(--poper-border) !important;
}

iframe {
    border: none;
}
</style>
