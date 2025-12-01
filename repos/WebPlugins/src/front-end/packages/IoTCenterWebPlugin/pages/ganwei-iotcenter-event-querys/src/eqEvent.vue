<template>
    <div class="eqEvent">
        <div class="left" id="leftSide">
            <div class="title">
                <div>
                    <p>{{ $t('eqEvent.leftHeaderTitle') }}({{ equipNumber }})</p>
                </div>
                <el-input size="small" v-model="scout" @keyup.enter.native="scoutEq"
                    :placeholder="$t('eqEvent.inputSearchEquip')" clearable>
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
                </el-input>
            </div>

            <div class="equipList_main" v-loading="nodeLoad">
                <!-- <span v-if="!equipList" class="promptResults">{{$t('eqEvent.publics.noData')}}</span> -->
                <!-- <dynamicMenu :dataList="equipList" :selectedMode="0" :isCloseStatus="true"></dynamicMenu> -->
                <myTree @getTotal="getTotal" ref="myTree" show-checkbox></myTree>
            </div>
            <widthSetting leftSide="leftSide" rightSide="rightSide" :minWidth="370" />
        </div>

        <div class="right" id="rightSide">
            <div class="rightTitle">
                <div class="leftName">{{ $t('eqEvent.mainHeader.leftTitle') }}</div>
                <div class="rightOp">
                    <div class="select-box">
                        <span>{{ $t('eqEvent.mainHeader.typeNm') }}</span>
                        <el-select v-model="eventType" filterable :placeholder="$t('eqEvent.appHeader[2]')">
                            <el-option v-for="(item, index) in eventTypeList" :key="index" :label="$t(item.name)"
                                :value="item.type"> </el-option>
                        </el-select>
                    </div>
                    <div class="calendar">
                        <el-date-picker @change="checkTime" v-model="searchTime" value-format="yyyy-MM-dd HH:mm:ss"
                            type="datetimerange" :default-time="['00:00:00', '23:59:59']" :range-separator="$t('eqEvent.z')"
                            :start-placeholder="$t('eqEvent.appHeader.start')" :picker-options="pickerOptions"
                            :end-placeholder="$t('eqEvent.appHeader.end')"></el-date-picker>
                    </div>
                    <el-button id="search" type="primary" @click="searchEvt(true)">
                        <i class="el-icon-search"></i>
                        {{ $t('eqEvent.cx') }}
                    </el-button>
                </div>
            </div>
            <div id="eqTable">
                <el-table :data="tableData" v-loading="tbLoad" :height="tableHeight" border id="table" ref="multipleTable"
                    style="width: 100%" show-overflow-tooltip>
                    <el-table-column prop="time" :label="$t('eqEvent.mainCenter.time')" width="200"></el-table-column>
                    <el-table-column prop="equipNo" :label="$t('eqEvent.mainCenter.equipNo')"
                        min-width="200"></el-table-column>
                    <el-table-column prop="equipNm" :show-overflow-tooltip="true" width="150"
                        :label="$t('eqEvent.mainCenter.equipName')"></el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :width="180" prop="event"
                        :label="$t('eqEvent.mainCenter.equipEvent')"></el-table-column>
                    <el-table-column prop="alarmLevel" :label="$t('eqEvent.mainCenter.level')"
                        min-width="200"></el-table-column>
                    <el-table-column prop="planNo" :label="$t('eqEvent.mainCenter.plan')" width="150"></el-table-column>
                    <el-table-column prop="confirmName" :label="$t('eqEvent.mainCenter.confirmor')"
                        width="150"></el-table-column>
                    <el-table-column prop="confirmTime" :label="$t('eqEvent.mainCenter.confirmTime')" width="200">
                    </el-table-column>
                    <el-table-column prop="confirmRemark" :min-width="180" :label="$t('eqEvent.mainCenter.confirmRemarks')"
                        :show-overflow-tooltip="true"> </el-table-column>
                    <template slot="empty">
                        <div class="noDataTips" v-if="!tbLoad" :data-noData="$t('eqEvent.publics.noData')"></div>
                        <div v-else></div>
                    </template>
                </el-table>
            </div>
            <div class="pagination">
                <el-pagination small background @size-change="handleSizeChange2" ref="pagination"
                    @current-change="handleCurrentChange2" :pager-count="7" :page-sizes="[100, 200, 300]"
                    :page-size="recordPage.pageSize" :current-page="recordPage.pageNo"
                    layout="sizes, prev, pager, next, jumper,total" :total="recordPage.total"></el-pagination>
            </div>
        </div>
    </div>
</template>
<script>
import eqEvent from './js/eqEvent.js'
export default eqEvent
</script>
<style scoped lang="scss" src="./css/eqEvent.scss"></style>
