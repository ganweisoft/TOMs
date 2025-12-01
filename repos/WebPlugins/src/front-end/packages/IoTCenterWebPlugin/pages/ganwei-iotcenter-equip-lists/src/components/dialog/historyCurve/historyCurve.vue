<!--
 * @Author: Copyright (c) 2020-2025 Shenzhen Ganwei Software Technology Co., Ltd
 * @Date: 2022-08-16 11:21:19
 * @FilePath: ganwei-iotcenter-equip-lists\src\components\dialog\historyCurve\historyCurve.vue
 * @Description: 历史曲线
-->
<template>
    <el-dialog
        class="historyEchar"
        :title="$t('equipListsIot.button.history') + ':' + currentSelect.equipName + '·' + currentSelect.ycyxName"
        :close-on-click-modal="false"
        :visible.sync="showDialog"
        @close="closeDialog"
    >
        <div class="header_time">
            <el-date-picker
                class="input-class"
                v-model="chartHistoryTime"
                size="mini"
                type="datetimerange"
                :picker-options="pickerOptions"
                range-separator="-"
                :start-placeholder="$t('equipListsIot.input.startTime')"
                :end-placeholder="$t('equipListsIot.input.endTime')"
                format="yyyy-MM-dd HH:mm:ss"
                value-format="yyyy-MM-dd HH:mm:ss"
                :default-time="defaultTime"
                @change="checkTime"
                align="left"
            >
            </el-date-picker>
            <el-button type="primary" size="mini" @click="onInquire()">{{ $t('publics.button.search') }}</el-button>
        </div>
        <div class="las_li_history" v-loading="loading">
            <template v-if="historyLength > 0">
                <div id="historyData-details"></div>
                <div id="historyData-timezoom"></div>
            </template>
            <div class="noDataTips noData" :data-noData="$t('equipListsIot.publics.noData')" v-if="historyLength == 0 && !loading"></div>
        </div>
    </el-dialog>
</template>
<style lang="scss" src="./historyCurve.scss"></style>
<script src="./historyCurve.js"></script>

<style>
.ew-resize {
    cursor: 'ew-resize';
}

.highcharts-button.highcharts-reset-zoom text {
    transform: translateY(2px);
}
</style>
