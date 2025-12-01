// 定时任务-每周任务安排
<template>
    <div class="weekTask">
        <header class="week-header">
            <div class="header-left">
                <h1>{{ $t('weekTaskNew.main.title') }}</h1>
                <el-button slot="reference" icon="el-icon-circle-plus-outline" size="mini" circle
                    @click="newWeekTask" ></el-button>
            </div>

            <div class="header-right">
                <span>{{ $t('weekTaskNew.main.taskType') }}</span>
                <el-select v-model="taskType" size="small" filterable @change="changeTaskType"
                    :placeholder="$t('weekTaskNew.dialog.pleaseSelect')" clearable>
                    <el-option value="T" :label="$t('weekTaskNew.main.normalTask')"></el-option>
                    <el-option value="C" :label="$t('weekTaskNew.main.circleTask')"></el-option>
                </el-select>
            </div>
        </header>
        <div class="main">
            <FullCalendar defaultView="resourceTimeline" locale="zh-cn" weekNumberCalculation="ISO"
                showNonCurrentDates="false" :slotLabelFormat="slotLabelFormat" :eventTimeFormat="evnetTime"
                :aspectRatio="aspectRatio" :plugins="calendarPlugins" resourceAreaWidth="100px"
                :resourceLabelText="$t('weekTaskNew.main.time')" :resources="resources" :events="calendarEvents"
                @eventClick="handleEventClick" />
        </div>
        <el-dialog :title="$t('weekTaskNew.dialog.addTask')" :visible.sync="newDialog" width="592px" top="10vh" center
            :close-on-click-modal="false" @close="closeNewDialog">
            <div class="new-weekTask">
                <el-form ref="weekForm" :model="weekForm" :rules="weekFormRules" label-width="0px">
                    <el-form-item prop="week">
                        <div class="form-item">
                            <span class="form-title">{{ $t('weekTaskNew.dialog.addTaskTo') }}:</span>
                            <el-select filterable v-model="week" size="small"
                                :placeholder="$t('weekTaskNew.dialog.pleaseSelect')" multiple>
                                <el-option v-for="(item, index) in weekList" :value="item.value" :label="$t(item.label)"
                                    :key="index"></el-option>
                            </el-select>
                        </div>
                    </el-form-item>

                    <div class="inline">
                        <el-form-item prop="allTasks">
                            <div class="form-item">
                                <div class="check-box">
                                    <div>
                                        <div class="check-title">
                                            <el-checkbox :indeterminate="isPtIndeterminate" v-model="checkAllPtTask"
                                                @change="checkAllPtTaskChange">{{
                                                    $t('weekTaskNew.main.normalTask')
                                                }}</el-checkbox>
                                        </div>
                                        <div class="check-content">
                                            <el-checkbox-group v-model="checkedPtTask" @change="checkedPtTaskChange">
                                                <el-checkbox v-for="item in ptTaskList"
                                                    :label="item.tableType + item.tableId" :key="item.tableId">
                                                    {{ item.tableName }}
                                                </el-checkbox>
                                            </el-checkbox-group>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </el-form-item>
                        <el-form-item>
                            <div class="form-item">
                                <div class="check-box">
                                    <div>
                                        <div class="check-title">
                                            <el-checkbox :indeterminate="isXhIndeterminate" v-model="checkAllXhTask"
                                                @change="checkAllXhTaskChange">{{
                                                    $t('weekTaskNew.main.circleTask')
                                                }}</el-checkbox>
                                        </div>
                                        <div class="check-content">
                                            <el-checkbox-group v-model="checkedXhTask" @change="checkedXhTaskChange">
                                                <el-checkbox v-for="item in xhTaskList"
                                                    :label="item.tableType + item.tableId" :key="item.tableId">
                                                    {{ item.tableName }}
                                                </el-checkbox>
                                            </el-checkbox-group>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </el-form-item>
                    </div>
                </el-form>
            </div>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" plain size="small" @click="newDialog = false">{{
                    $t('weekTaskNew.publics.button.cancel') }}</el-button>
                <el-button type="primary" @click="saveNewWeekTask" size="small" :loading="saveTempEquipLoading">{{
                    $t('weekTaskNew.publics.button.confirm') }}</el-button>
            </span>
        </el-dialog>
    </div>
</template>
<script>
import FullCalendar from '@fullcalendar/vue'
import resourceTimelinePlugin from '@fullcalendar/resource-timeline'
import '@fullcalendar/core/main.css'
import '@fullcalendar/timeline/main.css'
import '@fullcalendar/resource-timeline/main.css'
import '@fullcalendar/list/main.css'
import $ from 'jquery'
export default {
    name: 'weekTask',
    components: {
        FullCalendar
    },
    data () {
        return {
            // 顶部下拉框
            taskType: '',
            equipType: '',

            // 原始每周任务数据
            originalData: {},

            // 新增每周任务框
            newDialog: false,
            saveTempEquipLoading: false,
            weekList: [
                { label: 'weekTaskNew.main.mon', value: 'mon' },
                { label: 'weekTaskNew.main.tues', value: 'tues' },
                { label: 'weekTaskNew.main.wed', value: 'wed' },
                { label: 'weekTaskNew.main.thurs', value: 'thurs' },
                { label: 'weekTaskNew.main.fri', value: 'fri' },
                { label: 'weekTaskNew.main.sat', value: 'sat' },
                { label: 'weekTaskNew.main.sun', value: 'sun' }
            ],
            week: [],

            // 是否全选
            checkAllPtTask: false,
            checkAllXhTask: false,

            // 是否半选
            isPtIndeterminate: false,
            isXhIndeterminate: false,

            // 多选框组选中值
            checkedPtTask: [],
            checkedXhTask: [],

            // 多选框组所有值
            ptTaskValueList: [],
            xhTaskValueList: [],

            // 普通任务列表
            ptTaskList: [],
            xhTaskList: [],

            colorList: ['#3875FF', '#F5BB36', '#7358F5', '#63E03F', '#EB2FBC'],
            colorListNum: 0,
            appearTaskId: [],

            clickId: '',
            clickWeekDay: '',

            // 以下为FullCalendar相关配置项
            calendarPlugins: [resourceTimelinePlugin],
            aspectRatio: 1.8,
            slotLabelFormat: {
                hour: 'numeric',
                minute: '2-digit',
                hour12: false

                // omitZeroMinute: true,
                // meridiem: 'short'
            },
            evnetTime: {
                hour: 'numeric',
                minute: '2-digit',
                hour12: false
            },
            calendarEvents: [],
            calendarAllEvents: [],
            calendarAllStr: ''
        }
    },
    watch: {
        '$i18n.locale' () {
            window.location.reload()
        }
    },
    computed: {
        resources () {
            return [
                {
                    id: 'mon',
                    title: this.$t('weekTaskNew.main.mon')
                },
                {
                    id: 'tues',
                    title: this.$t('weekTaskNew.main.tues')
                },
                {
                    id: 'wed',
                    title: this.$t('weekTaskNew.main.wed')
                },
                {
                    id: 'thurs',
                    title: this.$t('weekTaskNew.main.thurs')
                },
                {
                    id: 'fri',
                    title: this.$t('weekTaskNew.main.fri')
                },
                {
                    id: 'sat',
                    title: this.$t('weekTaskNew.main.sat')
                },
                {
                    id: 'sun',
                    title: this.$t('weekTaskNew.main.sun')
                }
            ]
        },
        weekForm () {
            return {
                week: this.week,
                allTasks: [...this.checkedPtTask, ...this.checkedXhTask]
            }
        },
        weekFormRules () {
            return {
                week: [{ type: 'array', required: true, message: this.$t('weekTaskNew.message.selectDay'), trigger: 'change' }],
                allTasks: [{ type: 'array', required: true, message: this.$t('weekTaskNew.message.selectTask'), trigger: 'change' }]
            }
        }
    },
    mounted () {
        let _this = this
        this.getTaskList()
        this.getWeekTaskList()

        let date = new Date()
        let weekDay = date.getDay()

        // 选中样式
        $('.fc-resource-area tbody tr').eq(weekDay).addClass('active').find('.fc-widget-content>div').addClass('border-half-fixed')
        $('.fc-time-area .fc-content tbody td>div')
            .eq(weekDay - 1)
            .addClass('border-fixed')
        $('.fc-chrono .fc-widget-header:first-child .fc-cell-text').text('00:00')
        $('.fc-chrono .fc-widget-header:nth-child(3n+2) .fc-cell-text,.fc-chrono .fc-widget-header:nth-child(3n+3) .fc-cell-text').css({ display: 'none' })
        $('.fc-time-area .fc-content tbody tr').click(function () {
            $(this).find('td>div').addClass('border')
            $(this).siblings('tr').find('td>div').removeClass('border')
            let index = $('.fc-time-area .fc-content tbody tr').index($(this))
            $('.fc-resource-area tbody tr').eq(index).addClass('active').siblings('tr').removeClass('active')
        })
        setTimeout(() => {
            $('.fc-resource-area tbody tr')
                .click(function () {
                    $(this).addClass('active').siblings('tr').removeClass('active')
                    let index = $('.fc-resource-area tr').index($(this))
                    $('.fc-time-area .fc-content tbody td>div').removeClass('border')
                    $('.fc-time-area .fc-content tbody td>div')
                        .eq(index - 1)
                        .addClass('border')
                })
                .dblclick(function () {
                    // 双击弹出新增
                    let index = $('.fc-resource-area tr').index($(this))
                    _this.newDialog = true
                    _this.week = [_this.weekList[index - 1].value]
                })
        }, 1000)

    },
    methods: {
        // 获取所有任务
        getTaskList () {
            this.$api
                .tkGetRepository({
                    searchName: ''
                })
                .then(res => {
                    if (res.status === 200 && res.data.code === 200) {
                        let data = res.data.data.rows || []
                        this.ptTaskList = []
                        this.xhTaskList = []
                        for (let item of data) {
                            if (item.tableType === 'T') {
                                this.ptTaskList.push(item)

                                // 保存所有普通任务值
                                this.ptTaskValueList.push(item.tableType + item.tableId)
                            } else if (item.tableType === 'C') {
                                this.xhTaskList.push(item)

                                // 保存所有循环任务值
                                this.xhTaskValueList.push(item.tableType + item.tableId)
                            }
                        }
                    } else {
                        this.$message.error(res.data.message, res)
                    }
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                })
        },

        // 获取周任务
        getWeekTaskList () {
            this.$api
                .tkWeekList()
                .then(res => {
                    if (res.status === 200 && res.data.code === 200) {
                        let data = res.data.data
                        if (JSON.stringify(data) !== this.calendarAllStr) {
                            this.calendarAllStr = JSON.stringify(data)
                        } else {
                            return false
                        }
                        this.calendarEvents = []
                        this.originalData = {}
                        for (let i in data) {
                            let arr = []
                            if (data[i].tProcTaskLists) {
                                for (let item of data[i].tProcTaskLists) {
                                    this.getcalendarEvents(i, item)
                                    arr.push(item.tableType + item.tableId)
                                }
                            }
                            if (data[i].cProcTaskLists) {
                                for (let item of data[i].cProcTaskLists) {
                                    this.getcalendarEvents(i, item)
                                    arr.push(item.tableType + item.tableId)
                                }
                            }
                            this.originalData[i] = arr.join('+')
                        }
                        this.calendarAllEvents = JSON.parse(JSON.stringify(this.calendarEvents))
                    } else {
                        this.$message.error(res.data.message, res)
                    }

                    // 获取接口数据data更新完后执行
                    this.$nextTick(() => {
                        this.addEvent()
                        $('.fc-scroller').scrollLeft(0)
                    })
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                }).finally(l => {
                    if (this.taskType)
                        this.changeTaskType(this.taskType)
                })
        },

        setText (str) {
            let arr = str.split('-')
            arr.shift()
            return arr.join('-')
        },

        // 给任务条增加事件
        addEvent () {
            let _this = this

            let length = $('.fc-timeline-event').length

            for (let i = 0; i < length; i++) {
                let text = $('.fc-timeline-event').eq(i).find('.fc-title').text()
                if (text.startsWith(`${this.$t('weekTaskNew.main.normalTask')}-`)) {
                    $(`.fc-timeline-event:eq(${i}) .fc-title-wrap`).prepend('<i class="iconfont iconputongrenwu"></i>')
                    $('.fc-timeline-event').eq(i).find('.fc-title').text(this.setText(text))
                } else if (text.startsWith(`${this.$t('weekTaskNew.main.circleTask')}-`)) {
                    $(`.fc-timeline-event:eq(${i}) .fc-title-wrap`).prepend('<i class="iconfont iconxunhuanrenwu"></i>')
                    $('.fc-timeline-event').eq(i).find('.fc-title').text(this.setText(text))
                }

                let thisTitle = $('.fc-timeline-event').eq(i).find('.fc-title').text()
                $('.fc-timeline-event').eq(i).attr('title', thisTitle)
            }

            setTimeout(() => {
                // 增加删除按钮并隐藏,修改任务条宽度
                $('.fc-timeline-event').children('.icon-tubiao20_shanchu').remove()
                $('.fc-timeline-event').prepend(`<i class="iconfont icon-tubiao20_shanchu hidden"></i>`)

                $('.fc-timeline-event')
                    .hover(
                        function () {
                            let titleLen = Number($(this).find('.fc-title').width()) + 40
                            $(this).css({
                                'min-width': titleLen + 'px',
                                'z-index': 1
                            })
                            $(this).find('.icon-tubiao20_shanchu').removeClass('hidden')
                        },
                        function () {
                            $(this).css({
                                'min-width': '3rem',
                                'z-index': 0
                            })
                            $(this).find('.icon-tubiao20_shanchu').addClass('hidden')
                        }
                    )
                    .on('click', '.icon-tubiao20_shanchu', function () {
                        // 点击删除按钮事件执行内容
                        _this
                            .$msgbox({
                                title: _this.$t('weekTaskNew.message.tips'),
                                message: _this.$t('weekTaskNew.message.delMsg') + $(this).next().find('span').html() + '?',
                                showCancelButton: true,
                                confirmButtonText: _this.$t('weekTaskNew.publics.button.confirm'),
                                cancelButtonText: _this.$t('weekTaskNew.publics.button.cancel'),
                                beforeClose: (action, instance, done) => {
                                    if (action === 'confirm') {
                                        instance.confirmButtonLoading = true

                                        // 找到对应周几删除指定任务项
                                        let arr = _this.originalData[_this.clickWeekDay].split('+')
                                        let index = arr.indexOf(_this.clickId)
                                        arr.splice(index, 1)
                                        let obj = JSON.parse(JSON.stringify(_this.originalData))
                                        obj[_this.clickWeekDay] = arr.join('+')

                                        setTimeout(() => {
                                            // 调用删除接口
                                            _this.$api
                                                .tkWeekEdit(obj)
                                                .then(res => {
                                                    if (res.status === 200 && res.data.code === 200) {
                                                        _this.getWeekTaskList()
                                                        _this.$message({
                                                            type: 'success',
                                                            title: _this.$t('weekTaskNew.publics.tips.deleteSuccess')
                                                        })
                                                        _this.newDialog = false
                                                    } else {
                                                        _this.$message.error(res.data.message, res)
                                                    }
                                                    instance.confirmButtonLoading = false
                                                    done()
                                                })
                                                .catch(err => {
                                                    _this.$message.error(err.data, err)
                                                    instance.confirmButtonLoading = false
                                                    done()
                                                })
                                        }, 150)
                                    } else {
                                        done()
                                    }
                                }
                            })
                            .then()
                            .catch(() => {
                                // console.log(err);
                            })

                    })
            }, 1000)
        },

        // 获取calender数据
        getcalendarEvents (day, item) {
            let obj = {}
            obj.resourceId = day

            obj.title = item.tableName;
            // obj.title = item.tableType == 'T' ? `${this.$t('weekTaskNew.main.normalTask')}-` + item.tableName : `${this.$t('weekTaskNew.main.circleTask')}-` + item.tableName
            obj.type = item.tableType
            obj.taskId = item.tableType + item.tableId

            let date = new Date()
            let today = `${date.getFullYear()}-${this.addZero(date.getMonth() + 1, 2)}-${this.addZero(date.getDate(), 2)}`
            obj.start = `${today}T${item.beginTime.split(' ')[1]}`
            obj.end = `${today}T${item.endTime.split(' ')[1]}`

            if (this.appearTaskId.indexOf(item.tableId) === -1) {
                this.appearTaskId.push(item.tableId)
                obj.backgroundColor = this.colorList[this.colorListNum]
                obj.borderColor = this.colorList[this.colorListNum]
                if (this.colorListNum == this.colorList.length - 1) this.colorListNum = 0
                else this.colorListNum++
            } else {
                let num = this.appearTaskId.indexOf(item.tableId) % 5
                obj.backgroundColor = this.colorList[num]
                obj.borderColor = this.colorList[num]
            }
            this.calendarEvents.push(obj)
        },

        // 补零
        addZero (num, len) {
            return String(num).length > len ? num : (Array(len).join(0) + num).slice(-len)
        },

        // 任务类型筛选事件
        changeTaskType (val) {

            if (val == '') this.calendarEvents = JSON.parse(JSON.stringify(this.calendarAllEvents))
            else {
                this.calendarEvents = []
                for (const item of this.calendarAllEvents) {
                    if (item.type == val) {
                        this.calendarEvents.push(item)
                    }
                }
            }
            this.$nextTick(() => {
                this.addEvent()
            })
        },

        newWeekTask () {
            this.newDialog = true
        },

        // 全选事件
        checkAllPtTaskChange (val) {
            this.checkedPtTask = val ? this.ptTaskValueList : []
            this.isPtIndeterminate = false
        },
        checkAllXhTaskChange (val) {
            this.checkedXhTask = val ? this.xhTaskValueList : []
            this.isXhIndeterminate = false
        },

        // 选中新增任务列表事件
        checkedPtTaskChange (val) {
            let checkedCount = val.length
            this.checkAllPtTask = checkedCount === this.ptTaskValueList.length
            this.isPtIndeterminate = checkedCount > 0 && checkedCount < this.ptTaskValueList.length
        },
        checkedXhTaskChange (val) {
            let checkedCount = val.length
            this.checkAllXhTask = checkedCount === this.xhTaskValueList.length
            this.isXhIndeterminate = checkedCount > 0 && checkedCount < this.xhTaskValueList.length
        },

        // 保存新增周任务
        saveNewWeekTask () {
            this.$refs.weekForm.validate(valid => {
                if (valid) {
                    let obj = JSON.parse(JSON.stringify(this.originalData))
                    for (let day of this.week) {
                        let arr = obj[day].split('+')
                        for (let item of this.checkedPtTask) {
                            if (arr.indexOf(item) === -1) {
                                if (arr[0] == '') {
                                    arr[0] = item
                                } else {
                                    arr.push(item)
                                }
                            }
                        }
                        for (let item of this.checkedXhTask) {
                            if (arr.indexOf(item) === -1) {
                                if (arr[0] == '') {
                                    arr[0] = item
                                } else {
                                    arr.push(item)
                                }
                            }
                        }
                        obj[day] = arr.join('+')
                    }
                    this.saveTempEquipLoading = true
                    this.$api
                        .tkWeekEdit(obj)
                        .then(res => {
                            if (res.status === 200 && res.data.code === 200) {
                                this.getWeekTaskList()
                                this.$message({
                                    type: 'success',
                                    title: this.$t('weekTaskNew.publics.tips.addSuccess')
                                })
                                this.newDialog = false
                                this.taskType = ''
                            } else {
                                this.$message.error(res.data.message, res)
                            }
                            this.saveTempEquipLoading = false
                        })
                        .catch(err => {
                            this.$message.error(err.data, err)
                            this.saveTempEquipLoading = false
                        })
                }
            })
        },

        closeNewDialog () {
            this.week = []
            this.checkAllPtTask = false
            this.checkAllXhTask = false
            this.isPtIndeterminate = false
            this.isXhIndeterminate = false
            this.checkedPtTask = []
            this.checkedXhTask = []
            this.$refs['weekForm'].resetFields()
        },

        // 点击视图中任务事件
        handleEventClick (set) {
            this.clickWeekDay = set.event._def.resourceIds[0]
            this.clickId = set.event._def.extendedProps.taskId
        }
    }
}
</script>
<style lang="scss" src="./css/weekTaskNew.scss" scoped></style>
