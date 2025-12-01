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

.el-dialog__wrapper {
    display: flex;
    justify-content: center;
    align-items: center;

    .el-dialog {
        margin: 0px !important;

        .el-dialog__body {
            overflow: auto;
        }
    }
}

.el-message-box__message p {
    word-break: break-all;
}
</style>
