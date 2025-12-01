
<template>
    <div class="app">
        <el-config-provider :locale="locale">
            <router-view v-cloak />
        </el-config-provider>
    </div>
</template>

<script lang="ts">
import {ElConfigProvider} from 'element-plus'
import enUs from 'element-plus/es/locale/lang/en';
import zhCn from 'element-plus/es/locale/lang/zh-cn'
export default {
    name: 'App',
    components: {
        ElConfigProvider
    },

    data() {
        return {
            locale: localStorage.languageType != 'zh-CN' ? enUs : zhCn
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
    overflow: hidden;
}

html body div {
    box-sizing: border-box;
}

.app,
#app {
    height: 100%;
}

.el-popper{
    min-width: auto!important;
}

</style>
