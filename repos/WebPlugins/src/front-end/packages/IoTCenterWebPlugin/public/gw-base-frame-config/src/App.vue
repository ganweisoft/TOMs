<template>
    <div id="app">
        <router-view v-cloak />
    </div>
</template>

<script>

export default {
    name: 'App',
    data () {
        return {}
    },
    mounted () {
        let style = window.localStorage.theme
        window.document.documentElement.setAttribute('data-theme', style || 'dark')
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
a:focus,
input:focus,
p:focus,
div:focus {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
}

html body div {
    box-sizing: border-box;
}

#app {
    height: 100%;
    overflow: hidden;
}

.loginSet {
    top: 38px !important;
}
.loginSet .el-button {
    width: 100% !important;
}

.editBaseCheckBox {
    width: 370px !important;
    padding: 12px;
}
.editBaseCheckBox .el-checkbox-group {
    display: flex;
    flex-direction: row;
    justify-content: flex-start;
    flex-wrap: wrap;
}
.editBaseCheckBox .el-autocomplete {
    width: 100%;
    margin-bottom: 12px;
    height: 32px;
}
.editBaseCheckBox .el-autocomplete .el-input,
.editBaseCheckBox .el-autocomplete div {
    height: 32px;
}
.editBaseCheckBox .el-autocomplete input {
    height: 100% !important;
    background-color: black;
}
.el-select .el-tag__close.el-icon-close::before {
    color: white;
}
.el-popover .buttons .el-button {
    color: white !important;
}
.conditionAnalysePopper {
    width: 300px !important;
}
.conditionAnalysePopper .conditionAnalyse {
    /* width: 400px; */
    padding: 7px 12px;
}
.conditionAnalysePopper .conditionAnalyse .el-row {
    border-bottom: 1px dashed;
    border-color: rgba(240, 244, 255, 0.16);
    color: #f0f4ff;
    margin-top: 12px;
    padding-bottom: 4px;
}
.conditionAnalysePopper .conditionAnalyse .el-row .el-col {
    text-align: left;
}
.conditionAnalysePopper .conditionAnalyse .el-row:first-child {
    margin-top: 0px !important;
}

.el-notification .el-notification__content .msgText {
    color: rgba(240, 244, 255, 0.64);
}
</style>
