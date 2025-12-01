<!--
 * @Author:Copyright (c) 2020-2025 Shenzhen Ganwei Software Technology Co., Ltd
 * @Date: 2022-08-16 11:21:48
 * @FilePath: ganwei-iotcenter-equip-lists\src\components\dialog\realtimeCurve\realtimeCurve.vue
 * @Description: 实时曲线
-->
<template>
    <el-dialog class="realtimeCurve" :title="`${currentSelect.equipName}·${currentSelect.ycyxName}`"
        :visible.sync="showDialog" @close="closeDialog" center>
        <div id="containerE"></div>
    </el-dialog>
</template>
<style lang="scss" src="./realtimeCurve.scss"></style>
<script src="./realtimeCurve.js"></script>