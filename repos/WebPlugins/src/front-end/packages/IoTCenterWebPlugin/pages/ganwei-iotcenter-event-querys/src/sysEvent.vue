// 事件查询-系统事件
<template>
    <div class="sysEvent">
        <div class="header">
            <span class="title-number">{{ $t('sysEvent.leftHeaderTitle') }}（{{ total }}）</span>
            <div class="calendar">
                <span>{{ $t('sysEvent.headerTip') }}</span>
                <el-date-picker v-model="searchTime" type="datetimerange" value-format="yyyy-MM-dd HH:mm:ss"
                    :default-time="['00:00:00', '23:59:59']" :range-separator="$t('sysEvent.z')"
                    :start-placeholder="$t('sysEvent.appHeader.selectTimes')"
                    :end-placeholder="$t('sysEvent.appHeader.selectTimes')"></el-date-picker>
            </div>
            <el-button id="search" type="primary" @click="searchEvt">
                <i class="el-icon-search"></i>
                {{ $t('sysEvent.cx') }}
            </el-button>
        </div>

        <div id="eqTable">
            <el-table :data="tableData" v-loading="listShow" :height="tableHeight" border id="table" ref="multipleTable"
                style="width: 100%">
                <el-table-column prop="time" :label="$t('sysEvent.mainCenter.time')" width="200"></el-table-column>
                <el-table-column prop="event" :label="$t('sysEvent.leftHeaderTitle')">
                    <template slot-scope="scope">
                        <span>{{ scope.row.event }}</span>
                    </template>
                </el-table-column>
                <el-table-column prop="confirmName" :show-overflow-tooltip="true" width="130"
                    :label="$t('sysEvent.mainCenter.confirmor')"></el-table-column>
                <el-table-column :show-overflow-tooltip="true" :width="180" prop="confirmtime"
                    :label="$t('sysEvent.mainCenter.timeConfirm')"></el-table-column>
                <el-table-column prop="confirmRemark" :label="$t('sysEvent.mainCenter.remarks')" show-overflow-tooltip>
                    <template slot-scope="scope">
                        <span>{{ scope.row.confirmRemark }}</span>
                    </template>
                </el-table-column>
                <template slot="empty">
                    <div class="noDataTips" v-if="!listShow" :data-noData="$t('sysEvent.publics.noData')"></div>
                    <div v-else></div>
                </template>
            </el-table>
        </div>
        <div class="pagination">
            <el-pagination small background ref="pagination" @size-change="handleSizeChange"
                @current-change="handleCurrentChange" :pager-count="7" :page-sizes="[20, 50, 100]" :page-size="pageSize"
                :current-page="pageNo" layout="sizes, prev, pager, next, jumper,total" :total="total"></el-pagination>
        </div>
    </div>
</template>
<script>
export default {
    data () {
        return {
            searchTime: [],
            tableHeight: 0,
            listShow: true,
            tableData: [],
            pageSize: 20,
            pageNo: 1,
            total: 1,
            startTime: '',
            endTime: '',

            drawerAppHeader: false,
            appEndTimeValue: ''
        }
    },
    mounted () {
        this.searchTime.push(this.myUtils.getCurrentDate(1, '-') + ' 00:00:00')
        this.searchTime.push(this.myUtils.getCurrentDate(1, '-') + ' 23:59:59')
        this.appEndTimeValue = this.myUtils.getCurrentDate(1, '-') + ' 23:59:59'

        // 表格自适应
        let that = this
        let eqTable = document.getElementById('eqTable')
        window.onresize = function windowResize () {
            eqTable = document.getElementById('eqTable')
            if (eqTable) {
                that.tableHeight = eqTable.offsetHeight - 15
            }
        }
        this.searchEvt()
    },
    updated () {
        // 表格自适应
        let eqTable = document.getElementById('eqTable')
        if (eqTable) {
            this.tableHeight = eqTable.offsetHeight - 15
        }
    },
    methods: {
        // app关闭筛选弹窗
        closeDrawerAppHeader () {
            this.drawerAppHeader = false
        },

        // 根据日期查找设备事件
        searchEvt () {
            if (!this.searchTime) {
                this.$message({
                    title: this.$t('sysEvent.tips'),
                    type: 'warning'
                })
                return
            }
            this.drawerAppHeader = false
            let time = this.searchTime

            this.pageNo = 1
            this.$refs.pagination.internalCurrentPage = 1
            this.startTime = this.$moment(time[0]).format('YYYY-MM-DD HH:mm:ss')
            this.endTime = this.$moment(time[1]).format('YYYY-MM-DD HH:mm:ss')
            this.getSysList()
        },

        // 获取系统事件列表
        getSysList () {
            this.listShow = true
            this.tableData = []
            let data = {
                pageNo: this.pageNo,
                pageSize: this.pageSize,
                beginTime: this.startTime,
                endTime: this.endTime,
                sort: 'desc',
                eventName: ''
            }
            this.$api
                .evtSysList(data)
                .then(res => {
                    this.listShow = false
                    const { code, data, message } = res?.data || {}

                    if (code !== 200) {
                        this.$message.error(message)
                        return
                    }
                    let arr = data?.rows || []
                    this.total = data?.total || 0
                    if (arr instanceof Array) {
                        arr.forEach(item => {
                            item.confirmRemark = item.confirmRemark || '-'
                            item.confirmName = item.confirmName || '-'
                            item.event = item.event || '-'
                            item.time = item.time || '-'
                        })
                    }
                    this.tableData = arr
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                    this.listShow = false
                })

            if (!window.executeQueues) {
                window.executeQueues = {}
            }
            window.executeQueues.deleteQueryCache = () => {
                let delelteData = {
                    beginTime: data.beginTime,
                    endTime: data.endTime,
                }
                this.$api.deleteQueryCache(delelteData).then(res => {
                   // console.log(res)
                }).catch(err => {
                    console.log(err)
                })
            }
        },

        // 页码大小改变时触发事件
        handleSizeChange (pageSize) {
            this.pageNo = 1
            this.pageSize = pageSize
            this.$refs.pagination.internalCurrentPage = 1
            this.getSysList()
        },

        // 当前页码改变时触发
        handleCurrentChange (pageNo) {
            this.pageNo = pageNo
            this.getSysList()
        }
    }
}
</script>
<style scoped lang="scss" src="./css/sysEvent.scss"></style>
