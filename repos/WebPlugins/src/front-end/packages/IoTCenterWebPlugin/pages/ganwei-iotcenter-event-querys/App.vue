<template>
    <div id="app">
        <router-view />
    </div>
</template>

<script>
export default {
    name: 'App',
    mounted () {
        let style = window.localStorage.theme;
        window.document.documentElement.setAttribute('data-theme', style || 'dark')
    }
};
</script>

<style lang="scss">
html,
body,
#app {
    width: 100%;
    height: 100%;
    background-color: var(--frame-main-background)!important;
}

.editBaseCheckBox {
    width: 370px !important;
    padding: 12px !important;
}

.editBaseCheckBox .el-checkbox-group {
    display: flex;
    flex-direction: row;
    justify-content: flex-start;
    flex-wrap: wrap;
}

.editBaseCheckBox .el-popover__title {
    text-align: center;
    color: #f0f4ff;
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

.el-input__suffix {
    display: flex !important;
    justify-content: center;
    align-items: center;
}

.conditionAnalysePopper {
    // width: 300px !important;
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
    @include font_color("con-textColor1");
    text-align: left;
}

.conditionAnalysePopper .conditionAnalyse .el-row:first-child {
    margin-top: 0px !important;
}

.el-input__icon {
    display: flex !important;
    justify-content: center;
    align-items: center;
}

.el-popover__title {
    @include font_color('con-textColor1');
}

.el-message-box.multi-line {
    .el-message-box__content p {
        line-height: 21px;
    }
}</style>
