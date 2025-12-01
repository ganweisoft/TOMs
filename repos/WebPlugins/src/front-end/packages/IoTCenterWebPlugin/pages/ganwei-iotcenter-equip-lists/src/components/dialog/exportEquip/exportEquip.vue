<!--
 * @Author: Copyright (c) 2020-2025 Shenzhen Ganwei Software Technology Co., Ltd
 * @Date: 2022-08-16 13:14:38
 * @FilePath: ganwei-iotcenter-equip-lists\src\components\dialog\exportEquip\exportEquip.vue
 * @Description: 导出设备
-->
<template>
    <el-dialog class="exportDialog" :title="$t('publics.button.exports')" :visible.sync="showDialog" center :close-on-click-modal="false"
         @close="closeDialog">
        <div>
            <div>
                <el-select v-model="exportType" filterable>
                    <el-option v-for="(item, index) in typeList" :key="index" :label="$t(item.name)"
                        :value="item.value">
                    </el-option>
                </el-select>
            </div>
        </div>
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain @click="closeDialog" size="small">
                {{ $t('publics.button.cancel') }}</el-button>
            <el-button type="primary" @click="confirmExport" size="small" :loading="exportLoading">
                {{ $t('publics.button.confirm') }}
            </el-button>
        </span>
    </el-dialog>
</template>
<style lang="scss" src="./exportEquip.scss">
</style>
<script src="./exportEquip.js"></script>
