<template>
    <div id="app">
        <router-view />
    </div>
</template>

<script>
export default {
    name: 'App',
    mounted() {
        let style = window.localStorage.theme
        window.document.documentElement.setAttribute('data-theme', style || 'dark')
    }
}
</script>

<style lang="scss">
html,
body,
#app {
    width: 100%;
    height: 100%;
}

#app {
    background: var(--frame-main-background);
    padding: 0px 20px 20px;
}

.crossBar {
    display: flex;
    justify-content: center;
    align-items: center;
}

.crossBar span {
    display: inline-block;
    width: 9px;
    height: 1px;
    /* background-color: #f0f4ff; */
    background-color: var(--con-textColor1);
}

.el-table__expand-icon {
    color: inherit !important;
}
</style>
