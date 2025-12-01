<!--
 * @Author: Copyright (c) 2020-2025 Shenzhen Ganwei Software Technology Co., Ltd
 * @Date: 2022-08-16 13:36:14
 * @FilePath: ganwei-iotcenter-equip-lists\src\components\dialog\selectExportType\selectExportType.vue
 * @Description: 导出类型选择
-->
<template>
    <el-dialog :title="$t('equipListsIot.title.exportType')" :visible.sync="showDialog"
        class="exportTypeSelect" width="450px" @close="closeDialog" center>
        <el-row>
            <div class="left">
                <span class="title">
                    {{ $t('equipListsIot.title.exportCurve') }}
                </span>
                <el-popover placement="bottom" trigger="hover" popper-class="conditionAnalysePopper">
                    <div class="conditionAnalyse">
                        <el-row>
                            <el-col :span="6">{{ $t(`equipListsIot.button.exportByDay`) }}:</el-col>
                            <el-col :span="18"> {{ $t(`equipListsIot.tips.exportByDayTip`) }}</el-col>
                        </el-row>
                        <el-row>
                            <el-col :span="6">{{ $t(`equipListsIot.button.exportByCombined`) }}:</el-col>
                            <el-col :span="18"> {{ $t(`equipListsIot.tips.exportByCombinedTip`) }}</el-col>
                        </el-row>
                    </div>
                    <span slot="reference" class='analysis'>
                        <i class="el-icon-question"></i>
                    </span>
                </el-popover>
            </div>
            <div class="right">
                <el-select v-model="isMerge" filterable>
                    <el-option v-for="item in exportTypeOption" :key="item.value" :label="$t(item.label)"
                        :value="item.value">
                    </el-option>
                </el-select>
            </div>
        </el-row>

        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain @click="closeDialog" size="small">
                {{ $t('publics.button.cancel') }}
            </el-button>
            <el-button type="primary" @click="confirm" size="small">
                {{ $t('publics.button.confirm') }}
            </el-button>
        </span>
    </el-dialog>
</template>
<style lang="scss" src="./selectExportType.scss">
</style>
<script src="./selectExportType.js"></script>