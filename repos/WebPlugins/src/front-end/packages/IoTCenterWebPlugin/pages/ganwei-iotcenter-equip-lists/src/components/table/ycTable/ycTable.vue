<template>
    <el-table :data="tableDataYc" border >
        <template slot="empty">
            <div class="noDataTips" :data-noData="$t('equipListsIot.publics.noData')" v-if="!loading"></div>
            <div v-else></div>
        </template>
        <el-table-column prop="ycNo" :label="$t('equipListsIot.table.listTitleYc.ycYxNo')" :key="12" width="auto" min-width="7%"> </el-table-column>
        <el-table-column prop="isAlarm" :label="$t('equipListsIot.table.listTitleYc.alarmState')" :key="13" width="auto" min-width="7%">
            <template slot-scope="scope">
                <span class="yd" :style="{ 'background-color': getColor(scope.row.isAlarm) }"></span>
            </template>
        </el-table-column>
        <el-table-column
            prop="ycNm"
            :label="$t('equipListsIot.table.listTitleYc.ycYxName')"
            :show-overflow-tooltip="true"
            :key="14"
            width="auto"
            min-width="10%"
        ></el-table-column>
        <el-table-column prop="value" :show-overflow-tooltip="true" :label="$t('equipListsIot.table.listTitleYc.value')" :key="15" width="auto" min-width="10%">
            <template slot-scope="scope">
                <preview :style="{ color: getColor(scope.row.txIsAlarm) }" :value="scope.row.value" :data="scope.row">
                    <span v-if="domConversion(scope.row.value)" v-html="scope.row.value"> </span>
                    <span v-else> {{ scope.row.value }}{{ scope.row.unit ? scope.row.unit.replace(/\[.\]/, '') : '' }} </span>
                </preview>
            </template>
        </el-table-column>
        <el-table-column :show-overflow-tooltip="true" :label="$t('equipListsIot.table.listTitleYc.suggestion')" width="auto" min-width="20%" :key="20">
            <template slot-scope="scope">
                <div class="suggestion">
                    <div v-if="scope.row.isAlarm != 'dotNoComm'">
                        <div v-if="scope.row.value > scope.row.valMax" v-html="getRender(scope.row.outmaxEvt)"></div>
                        <div v-else-if="scope.row.value < scope.row.valMin" v-html="getRender(scope.row.outminEvt)"></div>
                        <div class="crossBar" v-else-if="scope.row.isAlarm == 'dotNormal'"><span class="crossBar"></span></div>
                    </div>
                    <div v-else v-html="getRender(scope.row.procAdvice)"></div>
                </div>
            </template>
        </el-table-column>
    </el-table>
</template>
<script src="./ycTable.js"></script>
