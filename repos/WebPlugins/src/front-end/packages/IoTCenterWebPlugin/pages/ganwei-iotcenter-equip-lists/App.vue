<template>
    <div id="app">
        <router-view />
    </div>
</template>

<script>
export default {
    name: 'App',
    mounted () {
        let style = window.localStorage.theme
        window.document.documentElement.setAttribute('data-theme', style || 'dark')
    }
}
</script>

<style lang="scss">
#app {
    background-color: var(--frame-main-background);
    padding: 0px 20px 20px;
}

html {
    height: 100%;
}

.el-message-box {
    .el-message-box__btns {
        padding-right: 20px !important;
    }
}

.el-message-box__errormsg {
    min-height: 0px !important;
}

.qxs {
    span {
        display: flex;
    }
}

.el-pagination__sizes .el-input .el-input__inner {
    font-size: 12px !important;
}

.el-pagination {
    font-weight: 400 !important;
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
    background-color: var(--frame-main-color);
}

html body {
    height: 100%;
    font-family: 'Microsoft YaHei';
    overflow: hidden;
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

.el-dialog__wrapper {
    display: flex;
    justify-content: center;
    align-items: center;
}

.el-dialog {
    margin: 0px !important;
    @include el-dialog;
}

.el-popover {
    padding: 10px;
    min-width: 0 !important;
    padding-top: 5px;
    padding-bottom: 5px;
    line-height: 28px !important;
    cursor: pointer !important;
    text-align: center;
}

.el-popover .el-button-group {
    padding-top: 5px;
    padding-bottom: 5px;
    display: flex;
}

.setModule .el-button {
    padding: 13px 21px !important;
    border-radius: 6px !important;
}

.el-popover .el-upload .el-button--success {
    color: #3875ff;
}

.el-popover .el-button {
    // min-width: 50px;
    width: 100%;
    margin: 0;
    text-align: center;
    border: none;
    // background-color: rgba(59, 67, 87, 0.64);
    display: block;
    padding: 5px;
}

.yd {
    width: 10px;
    height: 10px;
    border-radius: 50%;
}

.el-popover .el-button:hover {
    background-color: var(--popover-button-background__hover);
    color: var(--tab-selTXColor1);
}

.initPopper {
    background: #313747;
}

.el-popover .el-button:hover {
    // background-color: rgba(59, 67, 87, 1);
}

.el-time-panel .el-time-spinner__wrapper {
    overflow-x: hidden !important;
}

.el-input input {
    border: 0.5px solid rgba(240, 244, 255, 0.16);
}

.scenePopper .el-button {
    background: #313747 !important;
}

.scenePopper .el-button:hover {
    background: #3b4357 !important;
}

.el-date-range-picker .el-picker-panel__body .el-date-range-picker__time-header {
    border-bottom: 1px solid rgba(240, 244, 255, 0.16);
}

.el-date-range-picker .el-picker-panel__sidebar {
    border-right: 1px solid rgba(240, 244, 255, 0.16) !important;
}

.el-dialog .el-button {
    padding-left: 10px;
    padding-right: 10px;
}

.el-tooltip__popper {
    max-width: 280px !important;
    background-color: #252833 !important;
    color: #d3d8e2 !important;
    // border: 0.5px solid rgba(240, 244, 255, 0.16);
    line-height: 28px !important;
    font-size: 14px !important;
}

.el-input__icon {
    display: flex;
    justify-content: center;
    align-items: center;
}

.el-loading-mask {
    visibility: visible !important;
}

.equipList-table-popover{
  p{
    color: var(--frame-main-color);
    text-align: left;
    font-size: 16px;
    border-bottom: 1px solid var(--frame-main-border1);
    padding-bottom: 5px;
    margin-bottom: 5px;
  }
  .el-button-group{
    justify-content: center;
  }
}
</style>
