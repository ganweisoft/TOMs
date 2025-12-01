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
#app {
    background-color: var(--frame-main-background);
    padding: 0px 20px 20px;
}

html {
    height: 100%;
}

.el-message-box .el-message-box__btns .el-button {
    width: auto !important;
}

.el-tooltip__popper.expression {
    max-width: 500px !important;
}

.el-form-item {
    flex-direction: column !important;
    align-items: flex-start !important;

    .label {
        display: flex !important;
        justify-content: flex-start;
        align-items: center;
    }

    .el-form-item__label {
        display: flex;
        justify-content: flex-start !important;
    }
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

.el-popover {
    width: fit-content !important;
    min-width: 0 !important;
    line-height: 28px !important;
    cursor: pointer !important;
    text-align: center;
    // border: 0.5px solid rgba(240, 244, 255, 0.16);
    // background-color: var(--btn-bgColor9);
}

.el-popover .el-button-group {
    padding-top: 5px;
    padding-bottom: 5px;
    display: flex;
}

.el-popover .el-button-group button .iconfont {
    font-size: 14px;
}

.el-popover .el-button-group .el-button {
    padding: 13px 21px !important;
}

.el-popover .el-upload .el-button--success {
    color: #3875ff;
}

.el-popover .el-button {
    width: 64px;
    padding-left: 0;
    padding-right: 0;
    margin: 0;
    text-align: center;
    border: none;

    .iconfont {
        font-size: 17px !important;
    }
}

.initPopper {
    background: #313747;
}

.el-popover .el-button--text {
    color: #3875ff;
}

.el-popover .el-button--primary {
    color: #3875ff;
}

.el-popover .el-button--danger {
    color: #f56c6c !important;
}

.el-popover .new-button-group {
    flex-direction: column;
    font-size: 0px;
}

.el-popover .new-button-group .el-button {
    width: unset;
    padding-top: 13px;
    padding-bottom: 13px;
    padding: 13px 21px;
    justify-content: flex-start !important;
    font-size: 14px;
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

.el-dialog {
    @include el-dialog;
}

.el-dialog .el-button {
    padding-left: 10px;
    padding-right: 10px;
}

.el-tooltip__popper {
    max-width: 280px !important;
    line-height: 28px !important;
    font-size: 14px !important;
}

.el-input__icon {
    display: flex;
    justify-content: center;
    align-items: center;
}

.el-collapse {
    .el-collapse-item {
        margin-bottom: 10px;
    }
}

.parameters {
    margin-top: 20px;

    .el-form {
        padding-bottom: 20px;

        .el-form-item.is-error {
            margin-bottom: 0px !important;
        }

        .el-form-item__error {
            padding-top: 0px !important;
        }
    }

    .el-form-item {
        display: flex;
        flex-direction: row !important;
        justify-content: flex-start !important;
        align-items: center !important;

        .el-form-item__label {
            width: auto !important;
        }

        .el-form-item__content {
            width: 100% !important;
        }
    }

    .parameter {
        display: flex;
        justify-content: flex-start;
        margin-top: 12px;

        .label {
            color: var(--con-textColor1);
            line-height: 32px;
            flex-shrink: 0;
        }

        .inputBox {
            width: 40% !important;
            margin-right: 20px;

            .errTip {
                color: #ff4949;
            }
        }

        .valueBox {
            flex: 1;
        }

        .delete {
            height: 32px;
            color: #ff4949;
            border-radius: 6px;
            font-size: 20px;
            cursor: pointer;
        }
    }
}

a:focus,
input:focus,
p:focus,
div:focus {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
}

.el-upload-list {
    display: none !important;
}

.el-message-box .message-box-deleteBtn {
    background-color: #f22433;
    border-color: #f22433;
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

.el-form-item__label {
    span {
        display: flex;
        justify-content: flex-start;
        align-items: center;
    }
}

.el-dialog__wrapper {
    display: flex;
    justify-content: center;
    align-items: center;

    .el-dialog {
        margin: 0px !important;

        .el-dialog__body {
            max-height: 80vh;
            overflow: auto;
        }
    }
}

.el-button.is-disabled {
    background-color: gray !important;
    border-color: gray !important;
}

.el-icon-warning {
    color: #faae13;
    font-size: 20px;
}

.flex-start-center {
    display: flex;
    justify-content: flex-start;
    align-items: center;
}
</style>
