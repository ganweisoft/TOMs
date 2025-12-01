// 定时任务-特殊日期安排
<template>
    <div class="specialTask">
        <div class="viewHeader">
            <div class="header-left">
                <h1>{{ $t('specialTask.main.title') }}</h1>
                <span @click="changeTabs('cal')" :class="{ tabActive: calActive }" class="change-box" v-if="!calActive">
                    <i class="iconfont icon-gw-icon-zhuanhuang"></i>{{ $t('specialTask.main.calView') }}
                </span>
                <span @click="changeTabs('list')" :class="{ tabActive: listActive }" class="change-box" v-else>
                    <i class="iconfont icon-gw-icon-zhuanhuang"></i>{{ $t('specialTask.main.listView') }}
                </span>
                <el-button slot="reference" icon="el-icon-circle-plus-outline" size="mini" circle
                    @click="addSpecialTask" ></el-button>
            </div>
            <div class="control">
                <el-input v-model="specSearch" @blur="() => {
                    specSearch = specSearch.trim()
                }
                    " @keyup.enter.native="speInput" :placeholder="$t('specialTask.main.searchInput')" size="small"
                    clearable>
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
                </el-input>
            </div>
        </div>
        <div class="viewBody" :class="{ tabStyle: isCal, 'padding-box': !isCal }" id="eqTable">
            <el-table v-loading="tableLoad" v-if="!isCal" :data="tableData" :height="tableHeight"
                :show-overflow-tooltip="false" @row-click="getRow" style="width: 100%" border
                :default-sort="{ prop: 'date', order: 'descending' }">
                <el-table-column prop="color" :label="$t('specialTask.main.mark')" width="88  ">
                    <!--插入颜色方块-->
                    <template slot-scope="scope">
                        <span class="colorBlock" :style="{ background: scope.row.color }"></span>
                    </template>
                </el-table-column>
                <el-table-column prop="dateName" :label="$t('specialTask.main.timeName')"
                    min-width="180"></el-table-column>
                <el-table-column prop="exeTask" :label="$t('specialTask.main.runTask')"
                    min-width="280"></el-table-column>
                <el-table-column prop="beginTime" :label="$t('specialTask.main.startTime')" min-width="180"
                    sortable></el-table-column>
                <el-table-column prop="endTime" :label="$t('specialTask.main.endTime')" min-width="180"
                    sortable></el-table-column>
                <!-- <el-table-column prop="control" :label="$t('specialTask.main.control')" width="120" v-if="!isLargeScreen">
                    <template slot-scope="scope">
                        <el-button type="text" size="small" @click="editModal = true">{{$t('specialTask.publics.button.edit')}}</el-button>
                        <el-button type="text" size="small" @click="delData">{{$t('specialTask.publics.button.deletes')}}</el-button>
                    </template>
                            </el-table-column> -->
                <el-table-column prop="control" :label="$t('specialTask.main.control')" width="120" fixed="right">
                    <template slot-scope="scope">
                        <i class="iconfont icon-gw-icon-bianji" @click="editModal = true" ></i>
                        <i class="iconfont icon-gw-icon-shangchu" @click="delData" ></i>
                    </template>
                </el-table-column>
                <template slot="empty">
                    <div class="noDataTips" :data-nodata="$t('specialTask.publics.noData')" v-if="!tableLoad"></div>
                    <div v-else></div>
                </template>
            </el-table>
            <FullCalendar v-if="isCal" ref="fullCalendar" :locale="locale" defaultView="dayGridMonth"
                :eventLimitText="$t('specialTask.main.viewAll')" :fixedWeekCount="false" :showNonCurrentDates="true"
                :eventLimit="true" :firstDay="0" :editable="false" :header="header" :buttonText="buttonText"
                :events="events" :plugins="plugins" :datesRender="datesRender" @eventClick="handleEventClick"
                @dateClick="handleDateClick" />
            <div class="viewPagnation" v-if="!isCal">
                <el-pagination small background @size-change="handleSizeChange" @current-change="handleCurrentChange"
                    :pager-count="7" :page-sizes="[20, 50, 100]" :page-size="rep.pageSize" :current-page="rep.pageNo"
                    layout="sizes, prev, pager, next, jumper,total" :total="rep.total"></el-pagination>
            </div>
        </div>
        <el-dialog :title="$t('specialTask.dialog.addTask')" top="25vh" :visible.sync="addModal" width="700px"
            @close="clearInput" :close-on-click-modal="false">
            <div class="formDetail">
                <el-form :rules="rules" :model="newListInfo" class="demo-form-inline" label-width="105px" ref="ruleForm"
                    :validate-on-rule-change="false">
                    <el-row :gutter="16">
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.timeName') + ':'" prop="dateName">
                                <el-input v-model="newListInfo.dateName" :maxlength="256"
                                    :placeholder="$t('specialTask.dialog.pleaseInputDayName')" size="small"></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.startTime') + ':'" prop="beginTime">
                                <el-date-picker class="datePicker" v-model="newListInfo.beginTime" type="date"
                                    size="small" :placeholder="$t('specialTask.dialog.selectDate')"
                                    :picker-options="pickerOptionsNewBegin"></el-date-picker>
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.endTime') + ':'" prop="endTime">
                                <el-date-picker v-model="newListInfo.endTime" type="date" size="small"
                                    :placeholder="$t('specialTask.dialog.selectDate')"
                                    :picker-options="pickerOptionsNewEnd"></el-date-picker>
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.normalTask') + ':'">
                                <el-select multiple v-model="newListInfo.normalTask" filterable
                                    :placeholder="$t('specialTask.dialog.pleaseSelect')" @change="TChoose" size="small">
                                    <el-option v-for="(item, index) in normalList" :key="index"
                                        :value="item.type + item.id" :label="item.name"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.circleTask') + ':'">
                                <el-select filterable multiple v-model="newListInfo.circleTask"
                                    :placeholder="$t('specialTask.dialog.pleaseSelect')" @change="CChoose" size="small">
                                    <el-option v-for="(item, index) in circleList" :key="index"
                                        :value="item.type + item.id" :label="item.name"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form>
            </div>
            <span slot="footer" class="dialog-footer">
                <el-button size="small" type="primary" plain @click="addModal = false">{{
                    $t('specialTask.publics.button.cancel') }}</el-button>
                <el-button size="small" type="primary" @click="addTask('ruleForm')" :loading="addLoading">{{
                    $t('specialTask.publics.button.confirm') }}</el-button>
            </span>
        </el-dialog>
        <el-dialog :title="$t('specialTask.dialog.editTask')" top="25vh" :visible.sync="editModal" @close="clearInput"
            width="700px">
            <div class="formDetail">
                <el-form :rules="rules" ref="ruleForm2" :model="editListInfo" label-width="105px"
                    class="demo-form-inline" :validate-on-rule-change="false">
                    <el-row :gutter="16">
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.timeName') + ':'" prop="dateName">
                                <el-input v-model="editListInfo.dateName" :maxlength="256"
                                    :placeholder="$t('specialTask.dialog.pleaseInput')" size="small"></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.startTime') + ':'" prop="beginTime">
                                <el-date-picker v-model="editListInfo.beginTime" type="date"
                                    :placeholder="$t('specialTask.dialog.selectDate')" size="small"
                                    :picker-options="pickerOptionsEditBegin"></el-date-picker>
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.endTime') + ':'" prop="endTime">
                                <el-date-picker v-model="editListInfo.endTime" type="date"
                                    :placeholder="$t('specialTask.dialog.selectDate')" size="small"
                                    :picker-options="pickerOptionsEditEnd"></el-date-picker>
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.normalTask') + ':'">
                                <el-select filterable multiple v-model="editListInfo.normalTask"
                                    :placeholder="$t('specialTask.dialog.pleaseSelect')" @change="TChoose" size="small">
                                    <el-option v-for="(item, index) in normalList" :key="index"
                                        :value="item.type + item.id" :label="item.name"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <el-form-item :label="$t('specialTask.main.circleTask') + ':'">
                                <el-select filterable multiple v-model="editListInfo.circleTask"
                                    :placeholder="$t('specialTask.dialog.pleaseSelect')" @change="CChoose" size="small">
                                    <el-option v-for="(item, index) in circleList" :key="index"
                                        :value="item.type + item.id" :label="item.name"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form>
            </div>
            <span slot="footer" class="dialog-footer">
                <el-button size="small" type="primary" plain @click="editModal = false">{{
                    $t('specialTask.publics.button.cancel') }}</el-button>
                <el-button size="small" type="primary" :disabled="btnAble" @click="saveData('ruleForm2')"
                    :loading="editLoading">{{ $t('specialTask.publics.button.save') }}</el-button>
            </span>
        </el-dialog>
    </div>
</template>
<script>
import draggable from 'vuedraggable'
import FullCalendar from '@fullcalendar/vue'
import dayGridPlugin from '@fullcalendar/daygrid'
import timeGridPlugin from '@fullcalendar/timegrid'
import interactionPlugin from '@fullcalendar/interaction'
import listPlugin from '@fullcalendar/list'
import '@fullcalendar/core/main.css'
import '@fullcalendar/daygrid/main.css'
import '@fullcalendar/timegrid/main.css'
import '@fullcalendar/list/main.css'

import $ from 'jquery'
export default {
    name: 'specialTask',
    data () {
        return {
            tableHeight: null,
            tableLoad: false,
            events: [],
            btnAble: false,
            params: {
                dateName: ''
            },
            isCal: true,
            calActive: true,
            listActive: false,
            plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin, listPlugin],
            header: {
                left: 'prevYear,prev,title,next,today,nextYear',
                center: '',
                right: ''
            },
            buttonText: {
                day: '12331'
            },
            addModal: false,
            editModal: false,
            addLoading: false,
            editLoading: false,
            newDrawer: false,
            editDrawer: false,
            TChoice: [],
            CChoice: [],
            rep: {
                pageNo: 1,
                pageSize: 30,
                total: 1
            },
            pageType: 'normal',
            pageType2: 'normal',
            newListInfo: {
                dateName: '',
                beginTime: this.$moment().format('YYYY-MM-DD'),
                endTime: this.$moment().format('YYYY-MM-DD'),
                normalTask: [],
                circleTask: []
            },
            pickerOptionsNewBegin: {
                disabledDate (time) {
                    return time.getTime() < new Date().getTime() - +24 * 60 * 60 * 1000
                }
            },
            pickerOptionsNewEnd: {
                disabledDate: this.disabledNewDate
            },
            pickerOptionsEditBegin: {
                disabledDate (time) {
                    return time.getTime() < new Date().getTime() - +24 * 60 * 60 * 1000
                }
            },
            pickerOptionsEditEnd: {
                disabledDate: this.disabledEditDate
            },
            editListInfo: {
                dateName: '',
                beginTime: '',
                endTime: '',
                normalTask: [],
                circleTask: []
            },
            task: {
                pageNo: 1,
                pageSize: 20,
                total: 0
            },
            normalList: [],
            circleList: [],
            monthChoosed: false,
            cMonth: 1,
            tableData: [],
            specSearch: '',
            activeName: 'second',
            beginTime: '',
            endTime: '',
            dateValue: new Date(),
            repTaskChoice: [],
            speScout: '',

            clientWidth: 0,
            clientHeight: 0,

            locale: this.$i18n.locale || 'zh-CN'
        }
    },
    components: {
        draggable,
        FullCalendar
    },
    watch: {
        specSearch (val, oldValue) {
            if (!val && oldValue.trim()) {
                this.speInput(null, true)
            }
        },
        '$i18n.locale' (val) {
            this.locale = val
            this.$refs['ruleForm'] && this.$refs['ruleForm'].clearValidate()
            this.$refs['ruleForm2'] && this.$refs['ruleForm2'].clearValidate()
        }
    },
    computed: {
        rules: function () {
            return {
                dateName: [
                    {
                        required: true,
                        message: this.$t('specialTask.dialog.pleaseInputDayName')
                    }
                ],
                beginTime: [
                    {
                        required: true,
                        message: this.$t('specialTask.dialog.pleaseSeleceStartTime')
                    }
                ],
                endTime: [
                    {
                        required: true,
                        message: this.$t('specialTask.dialog.pleaseSeleceEndTime')
                    }
                ]
            }
        }
    },
    created () {
        this.clientWidth = document.body.clientWidth
        this.clientHeight = document.body.clientHeight
        let that = this
        window.onresize = function windowResize () {
            that.clientWidth = document.body.clientWidth
            that.clientHeight = document.body.clientHeight
        }
    },
    mounted () {
        this.getDivideTask()

        // this.getRepList();
        this.getTbList()

        // 表格自适应
        let that = this
        let eqTable = document.getElementById('eqTable')
        if (eqTable) {
            that.tableHeight = eqTable.offsetHeight - 83
        }
        window.onresize = function windowResize () {
            eqTable = document.getElementById('eqTable')
            if (eqTable) {
                that.tableHeight = eqTable.offsetHeight - 83
            }
        }

    },
    updated () {
        // 表格自适应
    },
    methods: {
        // 根据开始时间改变结束时间范围
        disabledNewDate (time) {
            if (this.newListInfo.beginTime) {
                return time.getTime() < this.newListInfo.beginTime
            }
            return time.getTime() < new Date().getTime() - 24 * 60 * 60 * 1000
        },
        disabledEditDate (time) {
            if (this.editListInfo.beginTime) {
                return time.getTime() < this.editListInfo.beginTime
            }
            return time.getTime() < new Date().getTime() - 24 * 60 * 60 * 1000
        },

        // 改变导航栏触发的方法
        changeTabs (type) {
            if (type === 'cal') {
                this.listActive = false
                this.calActive = true
                this.isCal = true
                $('#light li').removeClass('lightYear')
            } else {
                this.getTbList()
                this.listActive = true
                this.calActive = false
                this.isCal = false
            }
        },

        speInput (e, change) {
            this.specSearch = this.specSearch.trim()
            this.speScout = this.specSearch
            if (this.specSearch || change) {
                if (this.specSearch === '') {
                    this.pageType2 = 'normal'
                    this.rep.pageNo = 1
                    this.rep.pageSize = 20
                    this.getTbList()
                    return
                }

                this.pageType2 = 'scout'
                this.rep.pageNo = 1
                this.rep.pageSize = 20
                this.getTbScoutList()
            }
        },

        // 选择普通任务触发的方法
        TChoose (val) {
            this.TChoice = val
        },

        // 选择循环任务触发的方法
        CChoose (val) {
            this.CChoice = val
        },

        addSpecialTask () {
            this.addModal = true
            this.$nextTick(() => {
                this.$refs['ruleForm'].resetFields()
            })
            this.TChoice = []
            this.CChoice = []
        },

        // 保存任务
        saveData (refStr) {
            this.editListInfo.dateName = this.editListInfo.dateName.trim()
            if (this.editListInfo.dateName.length > 255) {
                this.$message({
                    showClose: true,
                    title: this.$t('specialTask.message.nameMsg'),
                    type: 'warning'
                })
                return
            }
            if (this.editListInfo.beginTime > this.editListInfo.endTime) {
                this.$message({
                    type: 'warning',
                    title: this.$t('specialTask.message.timeMsg')
                })
                return
            }
            this.btnAble = true
            this.$refs[refStr].validate(valid => {
                if (valid) {
                    let choiceSet = this.TChoice.concat(this.CChoice)
                    choiceSet = choiceSet.join('+')
                    if (!choiceSet || choiceSet === '+') {
                        this.$message({
                            type: 'warning',
                            title: this.$t('specialTask.main.selectTask')
                        })
                        return;
                    }
                    this.editLoading = true
                    this.$api
                        .tkSpEdit({
                            id: this.cId,
                            dateName: this.editListInfo.dateName,

                            beginDate: this.timeFormat(this.editListInfo.beginTime) + 'T00:00:00',
                            endDate: this.timeFormat(this.editListInfo.endTime) + 'T23:59:59',
                            // beginDate: this.myUtils.setDate(this.timeFormat(this.editListInfo.beginTime) + ' 00:00:00'),
                            // endDate: this.myUtils.setDate(this.timeFormat(this.editListInfo.endTime) + ' 23:59:59'),
                            tableId: choiceSet === '+' ? '' : choiceSet
                        })
                        .then(rt => {
                            let dat = rt.data
                            let msg = rt.data.message
                            if (dat.code === 200) {
                                this.$message({
                                    type: 'success',
                                    title: this.$t('specialTask.publics.tips.editSuccess')
                                })
                                this.btnAble = false
                                this.getTbList(this.rep.pageSize, this.rep.pageNo)
                                this.editModal = false
                            } else {
                                this.$message.error(msg, rt)
                                this.btnAble = false
                            }
                            this.editLoading = false
                        })
                        .catch(e => {
                            this.$message.error(e.data, e)
                            this.editLoading = false
                        })
                } else {
                    return false
                }
            })
        },

        // 新增任务
        addTask (refStr) {
            this.newListInfo.dateName = this.newListInfo.dateName.trim()
            if (this.newListInfo.dateName.length > 255) {
                this.$message({
                    showClose: true,
                    title: this.$t('specialTask.message.nameMsg'),
                    type: 'warning'
                })
                return
            }
            if (this.newListInfo.beginTime > this.newListInfo.endTime) {
                this.$message({
                    type: 'warning',
                    title: this.$t('specialTask.message.timeMsg')
                })
                return
            }

            this.btnAble = true
            this.$refs[refStr].validate(valid => {
                if (valid) {

                    let choiceSet = this.TChoice.concat(this.CChoice)
                    choiceSet = choiceSet.join('+')
                    if (!choiceSet || choiceSet === '+') {
                        this.$message({
                            type: 'warning',
                            title: this.$t('specialTask.main.selectTask')
                        })
                        return;
                    }
                    this.addLoading = true
                    this.$api
                        .tkSpAdd({
                            dateName: this.newListInfo.dateName,

                            beginDate: this.timeFormat(this.newListInfo.beginTime) + 'T00:00:00',
                            endDate: this.timeFormat(this.newListInfo.endTime) + 'T23:59:59',
                            // beginDate: this.myUtils.setDate(this.timeFormat(this.newListInfo.beginTime) + ' 00:00:00'),
                            // endDate: this.myUtils.setDate(this.timeFormat(this.newListInfo.endTime) + ' 23:59:59'),
                            tableId: choiceSet === '+' ? '' : choiceSet,
                            color: this.color16()
                        })
                        .then(rt => {
                            let dat = rt.data
                            let msg = dat.message
                            if (dat.code === 200) {
                                this.$message({
                                    type: 'success',
                                    title: this.$t('specialTask.publics.tips.addSuccess')
                                })
                                this.getTbList(this.rep.pageSize, this.rep.pageNo)
                                this.clearInput()
                                this.addModal = false
                                this.newDrawer = false
                            } else {
                                this.$message.error(msg, rt)
                            }
                            this.btnAble = false
                            this.addLoading = false
                        })
                        .catch(err => {
                            this.$message.error(err.data, err)
                            console.log(err)
                            this.btnAble = false
                            this.addLoading = false
                        })
                }
                return false
            })
        },

        // 清空表单数据
        clearInput () {
            try {
                this.$refs['ruleForm2'].resetFields()
            } catch (e) { }
            this.newListInfo.dateName = ''
            this.newListInfo.beginTime = this.myUtils.getCurrentDate(1, '-')
            this.newListInfo.endTime = this.myUtils.getCurrentDate(1, '-')
            this.newListInfo.normalTask = []
            this.newListInfo.circleTask = []
            this.editListInfo.dateName = ''
            this.editListInfo.beginTime = ''
            this.editListInfo.endTime = ''
            this.editListInfo.normalTask = []
            this.editListInfo.circleTask = []
            this.TChoice = []
            this.CChoice = []
            this.addModal = false
            this.editModal = false
            this.btnAble = false
        },

        // 获取循环任务与普通任务
        getDivideTask () {
            this.$api
                .tkGetRepository()
                .then(rt => {
                    let dat = rt.data.data
                    let code = rt.data.code
                    if (code !== 200) {
                        let msg = rt.data.message
                        this.$message.error(msg, rt)
                        return
                    }
                    let list = dat.rows || []
                    let target = ''
                    let obj = {}
                    for (let i = 0; i < list.length; i++) {
                        target = list[i]
                        obj = {}
                        obj.name = target.tableName
                        obj.id = target.tableId
                        obj.type = target.tableType
                        if (obj.type === 'T') {
                            this.normalList.push(obj)
                        } else if (obj.type === 'C') {
                            this.circleList.push(obj)
                        }
                    }
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                })
        },

        // 删除数据
        delData () {
            this.$msgbox({
                title: this.$t('specialTask.message.tips'),
                message: this.$t('specialTask.message.delMsg'),
                showCancelButton: true,
                confirmButtonText: this.$t('specialTask.publics.button.confirm'),
                cancelButtonText: this.$t('specialTask.publics.button.cancel'),
                beforeClose: (action, instance, done) => {
                    if (action === 'confirm') {
                        instance.confirmButtonLoading = true
                        setTimeout(() => {
                            this.$api
                                .tkSpDel({
                                    type: 'Id',
                                    data: this.cId
                                })
                                .then(rt => {
                                    let code = rt.data.code
                                    let msg = rt.data.message
                                    if (code === 200) {
                                        if (this.tableData.length === 1) {
                                            this.rep.pageNo--
                                        }
                                        this.getTbList(this.rep.pageSize, this.rep.pageNo)
                                        this.$message({
                                            type: 'success',
                                            title: this.$t('specialTask.publics.tips.deleteSuccess')
                                        })
                                    } else {
                                        this.$message.error(msg, rt)
                                    }
                                    instance.confirmButtonLoading = false
                                    done()
                                })
                                .catch(() => {
                                    instance.confirmButtonLoading = false
                                    done()
                                })
                                .catch(err => {
                                    this.$message.error(err.data, err)
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
        },

        // 获取表格当前行数据
        getRow (set) {
            this.cId = set.id
            this.editListInfo.dateName = set.dateName
            this.editListInfo.beginTime = new Date(set.beginTime + ' 00:00:00').getTime()
            this.editListInfo.endTime = new Date(set.endTime + ' 23:59:59').getTime()
            this.TChoice = []
            this.CChoice = []
            let target = ''
            let task = set.taskNo.split('+')
            this.editListInfo.normalTask = []
            this.editListInfo.circleTask = []
            for (let i = 0; i < task.length; i++) {
                target = task[i]
                if (target.slice(0, 1) === 'T') {
                    this.editListInfo.normalTask.push(target)
                    this.TChoice.push(target)
                } else if (target.slice(0, 1) === 'C') {
                    this.editListInfo.circleTask.push(target)
                    this.CChoice.push(target)
                }
            }
        },

        // 获取特殊日期安排列表
        getTbList (pageSize, pageNo) {
            this.tableLoad = true
            this.monthChoosed = false
            this.rep.pageSize = pageSize || 20
            this.rep.pageNo = pageNo || 1
            this.$api
                .tkSpGet({
                    pageSize: this.rep.pageSize,
                    pageNo: this.rep.pageNo,
                    searchName: this.speScout,
                    beginTime: this.beginTime,
                    endTime: this.endTime
                })
                .then(rt => {
                    let dat = rt.data.data
                    let code = rt.data.code
                    if (code !== 200) {
                        let msg = rt.data.message
                        this.$message.error(msg, rt)
                        return
                    }
                    this.rep.total = dat.totalCount
                    let set = dat.rows || []
                    let obj = {}
                    let target = ''
                    this.tableData = []
                    for (let i = 0; i < set.length; i++) {
                        target = set[i]
                        obj = {}
                        obj.id = target.id
                        obj.dateName = target.dateName
                        obj.exeTask = target.tableID === '+' || target.tableID === '' ? '-' : this.getTaskName(target.tableID.split('+'))
                        obj.taskNo = target.tableID
                        obj.color = target.color
                        obj.beginTime = this.timeFormat(new Date(target.beginDate.replace(/-/g, '/')).getTime())
                        obj.endTime = this.timeFormat(new Date(target.endDate.replace(/-/g, '/')).getTime())
                        this.tableData.push(obj)
                    }
                    this.initCal() // 日历视图的需要再新增一个根据起始月份搜索特殊日期任务（无需分页）
                    this.tableLoad = false
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                    console.log(err)
                    this.tableLoad = false
                })
        },

        // 随机生成16进制颜色
        color16 () {
            let r = Math.floor(Math.random() * 256)
            let g = Math.floor(Math.random() * 256)
            let b = Math.floor(Math.random() * 256)
            let color = '#' + r.toString(16) + g.toString(16) + b.toString(16)
            return color
        },

        // 初始化日历视图
        initCal (start) {
            this.oneMonth(start)
            this.showLine()
        },

        // 获取指定月份的起止时间点
        oneMonth (start) {
            this.params.startDate = this.$moment(start).subtract(1, 'months').endOf('month').format('YYYY-MM-DD')
            this.params.endDate = this.$moment(start).add(1, 'months').startOf('month').format('YYYY-MM-DD')
        },

        // 日历视图：查询指定月份信息
        datesRender (info) {
            let date = info.view.activeStart

            // 查询月份数据
            this.initCal(date)
        },

        // 日历视图：日期线
        showLine () {
            this.events = []
            this.tableData.forEach(item => {
                this.events.push({
                    backgroundColor: item.color,
                    borderColor: item.color,
                    title: item.dateName,
                    start: item.beginTime,
                    end: this.$moment(item.endTime).add(1, 'days').format('YYYY-MM-DD'), // 这里日历视图显示会少一天
                    extendedProps: item
                })
            })
        },

        // 日历视图：点击事件
        handleEventClick (set) {
            let dateInfo = set.event.extendedProps
            this.getRow(dateInfo)
            this.editModal = true
        },

        // 日历视图：点击日期
        handleDateClick (set) {
            let date = set.date
            this.newListInfo.beginTime = date.getTime()
            this.newListInfo.endTime = date.getTime()
            this.addModal = true
        },

        // 时间格式转换
        timeFormat (stamp) {
            let now = new Date(stamp),
                y = now.getFullYear(),
                m = ('0' + (now.getMonth() + 1)).slice(-2),
                d = ('0' + now.getDate()).slice(-2)
            return y + '-' + m + '-' + d
        },

        // 获取特殊日期安排搜索列表
        getTbScoutList () {
            this.getTbList()
        },

        // 获取任务名称
        getTaskName (set) {
            let target = ''
            let cTarget = ''
            let tTarget = ''
            let result = ''
            for (let i = 0; i < set.length; i++) {
                target = set[i]
                if (target.slice(0, 1) === 'C') {
                    for (let j = 0; j < this.circleList.length; j++) {
                        cTarget = this.circleList[j]
                        if (cTarget.id == target.slice(1)) {
                            result += cTarget.name + ','
                            break
                        }
                    }
                } else if (target.slice(0, 1) === 'T') {
                    for (let k = 0; k < this.normalList.length; k++) {
                        tTarget = this.normalList[k]
                        if (tTarget.id == target.slice(1)) {
                            result += tTarget.name + ','
                            break
                        }
                    }
                }
            }
            result = result.slice(0, result.length - 1)
            return result
        },

        // 页码大小改变时触发事件
        handleSizeChange (pageSize) {
            this.rep.pageSize = pageSize
            this.getTbList(this.rep.pageSize, this.rep.pageNo)
        },

        // 当前页码改变时触发
        handleCurrentChange (pageNo) {
            this.rep.pageNo = pageNo
            this.getTbList(this.rep.pageSize, this.rep.pageNo)
        },

        // 页码大小改变时触发事件
        handleSizeChange2 (pageSize) {
            this.rep.pageSize = pageSize
            if (this.pageType2 === 'normal') {
                this.getTbList()
            } else if (this.pageType2 === 'scout') {
                this.getTbScoutList()
            } else if (this.pageType === 'monthChoice') {
                this.$api
                    .tkSpGet({
                        pageSize: this.rep.pageSize,
                        pageNo: this.rep.pageNo,
                        beginTime: this.beginTime,
                        endTime: this.endTime
                    })
                    .then(rt => {
                        let dat = rt.data.data
                        let code = rt.data.code
                        if (code !== 200) {
                            let msg = rt.data.message
                            this.$message.error(msg, rt)
                            return
                        }
                        this.rep.total = dat.totalCount
                        let set = dat.rows || []
                        let obj = {}
                        let target = ''
                        this.tableData = []
                        for (let i = 0; i < set.length; i++) {
                            target = set[i]
                            obj = {}
                            obj.id = target.id
                            obj.dateName = target.dateName
                            obj.exeTask = target.tableID === '+' ? '-' : this.getTaskName(target.tableID.split('+'))
                            obj.taskNo = target.tableID
                            obj.beginTime = this.timeFormat(new Date(target.beginDate).getTime() + 28800000)
                            obj.endTime = this.timeFormat(new Date(target.endDate).getTime() + 28800000)
                            this.tableData.push(obj)
                        }
                    })
                    .catch(err => {
                        this.$message.error(err.data, err)
                    })
            }
        },

        // 当前页码改变时触发
        handleCurrentChange2 (pageNo) {
            this.rep.pageNo = pageNo
            if (this.pageType2 === 'normal') {
                this.getTbList()
            } else if (this.pageType2 === 'scout') {
                this.getTbScoutList()
            } else if (this.pageType === 'monthChoice') {
                this.$api
                    .tkSpGet({
                        pageSize: this.rep.pageSize,
                        pageNo: this.rep.pageNo,
                        beginTime: this.beginTime,
                        endTime: this.endTime
                    })
                    .then(rt => {
                        let dat = rt.data.data
                        let code = rt.data.code
                        if (code !== 200) {
                            let msg = rt.data.message
                            this.$message.error(msg, rt)
                            return
                        }
                        this.rep.total = dat.totalCount
                        let set = dat.rows || []
                        let obj = {}
                        let target = ''
                        this.tableData = []
                        for (let i = 0; i < set.length; i++) {
                            target = set[i]
                            obj = {}
                            obj.id = target.id
                            obj.dateName = target.dateName
                            obj.exeTask = target.tableID === '+' ? '-' : this.getTaskName(target.tableID.split('+'))
                            obj.taskNo = target.tableID
                            obj.beginTime = this.timeFormat(new Date(target.beginDate).getTime() + 28800000)
                            obj.endTime = this.timeFormat(new Date(target.endDate).getTime() + 28800000)
                            this.tableData.push(obj)
                        }
                    })
                    .catch(err => {
                        this.$message.error(err.data, err)
                    })
            }
        },

        // 日期转换具体数字
        dateTurnNum (val) {
            let result = ''
            val = val.split('-')[1]
            result = parseInt(val)
            return result
        },

        // 根据年、月寻找起始时间
        findInterval () {
            let monthSum = 0
            let month = this.cMonth
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) {
                monthSum = 31
            } else if (month == 2) {
                if (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0)) {
                    monthSum = 29
                } else {
                    monthSum = 28
                }
            } else {
                monthSum = 30
            }
            month = this.isZero(month)
            this.beginTime = year + '-' + month + '-' + '01'
            this.endTime = year + '-' + month + '-' + monthSum
        },

        // 时分秒格式化
        isZero (m) {
            return m < 10 ? '0' + m : m
        },

        // 数字译中文
        returnMonthText (val) {
            switch (val) {
                case 1:
                    return this.$t('specialTask.message.January')
                case 2:
                    return this.$t('specialTask.message.February')
                case 3:
                    return this.$t('specialTask.message.March')
                case 4:
                    return this.$t('specialTask.message.April')
                case 5:
                    return this.$t('specialTask.message.May')
                case 6:
                    return this.$t('specialTask.message.June')
                case 7:
                    return this.$t('specialTask.message.July')
                case 8:
                    return this.$t('specialTask.message.August')
                case 9:
                    return this.$t('specialTask.message.September')
                case 10:
                    return this.$t('specialTask.message.October')
                case 11:
                    return this.$t('specialTask.message.November')
                case 12:
                    return this.$t('specialTask.message.December')
            }
        }
    }
}
</script>
<style lang="scss" src="./css/specialTask.scss" scoped></style>
