
<template>
    <el-table :data="tableDataYx" border >
        <template slot="empty">
            <div class="noDataTips" :data-noData="$t('equipListsIot.publics.noData')" v-if="!loading"></div>
            <div v-else></div>
        </template>
        <el-table-column  prop="yxNo" :label="$t('equipListsIot.table.listTitleYx.ycYxNo')" :key="2" width="auto"
            min-width="7%"> </el-table-column>
        <el-table-column  :label="$t('equipListsIot.table.listTitleYx.alarmState')" :key="3" width="auto" min-width="7%">
            <template slot-scope="scope">
                <span class="yd" :style="{ 'background-color': getColor(scope.row.isAlarm) }"></span>
            </template>
        </el-table-column>
        <el-table-column  prop="yxNm" :show-overflow-tooltip="true" :label="$t('equipListsIot.table.listTitleYx.ycYxName')"
            :key="4" width="auto" min-width="10%"> </el-table-column>
        <el-table-column  prop="value" :label="$t('equipListsIot.table.listTitleYx.value')" :key="5" width="auto"
            min-width="10%">
            <template slot-scope="scope">
                <span :style="{ color: getColor(scope.row.txIsAlarm) }">
                    <span>{{ scope.row.value }}</span>
                </span>
            </template>
        </el-table-column>
        <el-table-column  prop="procAdvice" :show-overflow-tooltip="true"
            :label="$t('equipListsIot.table.listTitleYx.suggestion')" :key="10" width="auto" min-width="20%">
            <template slot-scope="scope">
                <div
                    v-if="(scope.row.equipState == 'normal' || scope.row.equipState == 'alarm') && scope.row.procAdviceD !== ''">
                    <span v-if="scope.row.equipState == 'normal'">
                        {{ scope.row.levelR < scope.row.levelD ? scope.row.procAdviceR : scope.row.procAdviceD }} </span>
                            <span v-if="scope.row.equipState == 'alarm'">
                                {{ scope.row.levelR > scope.row.levelD ? scope.row.procAdviceR : scope.row.procAdviceD }}
                            </span>
                </div>
                <div class="crossBar" v-else>
                    <span class="crossBar"></span>
                </div>
            </template>
        </el-table-column>
    </el-table>
</template>

<script src="./yxTable.js"></script>
