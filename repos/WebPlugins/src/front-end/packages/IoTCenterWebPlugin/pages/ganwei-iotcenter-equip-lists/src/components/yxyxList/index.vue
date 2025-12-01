
<template>
    <el-dialog class="ycyxList" :visible.sync="showDialog" :show-close='false' width="600px" @closed='close'
        :close-on-click-modal="false">
        <span slot='title'>
            <div class="header">
                <span>
                    {{ currentSelect.ycyxName }}
                </span>
                <span class="close" @click="close">×</span>
            </div>
        </span>
        <div class="select" v-if="showTimeSelect">
            <el-date-picker @change='timeChange' size="small" v-model="timeSelect" :default-time="defaultTime"
                :picker-options="pickerOptions" type="datetimerange" value-format="yyyy-MM-dd HH:mm"
                format="yyyy-MM-dd HH:mm" range-separator="-" :start-placeholder="$t('equipListsIot.input.selectStartTime')"
                :end-placeholder="$t('equipListsIot.input.selectEndTime')">
            </el-date-picker>
            <el-button type="primary" size="small" @click="btnGetData" :loading='loading'>{{
                $t('equipListsIot.button.search') }}</el-button>
        </div>

        <el-table :data="currentList" v-loading='loading' :height="tableHeight">
            <template slot="empty">
                <div class="noDataTips" :data-noData="$t('equipListsIot.publics.noData')" v-if="!loading"></div>
                <div v-else></div>
            </template>

            <el-table-column prop="time" :label="$t('equipListsIot.label.date')" :show-overflow-tooltip='true' width="200">
            </el-table-column>
            <el-table-column prop="value" :label="$t('equipListsIot.label.value')" :show-overflow-tooltip='true'
                min-width="140">
            </el-table-column>
        </el-table>

        <span slot="footer" v-if="showTimeSelect" class="dialog-footer">
            <el-pagination :pager-count="4" small background @current-change="handleCurrentChange"
                :current-page.sync="pageNo" :page-size="pageSize" layout=" prev, pager, next, jumper,total" :total="total">
            </el-pagination>
        </span>

    </el-dialog>
</template>

<script src='./index.js'></script>
<style lang="scss" src='./index.scss' scoped ></style>