import draggable from 'vuedraggable'
import screenfull from 'screenfull'
import { val } from 'dom7'
const myTree = () => import('gw-base-components-plus/treeV2')
export default {
    name: 'taskRepository',
    data () {
        return {
            visible: false,
            treeLoading: false,

            searchName: '',
            taskLoad: false,

            // 任务库列表
            taskRepository: [],

            pageNo: 1,
            total: 0,
            pageSize: 20,

            // 用于判断是新增还是编辑
            isNewPtTask: true,
            isNewXhTask: true,

            treeDialog: false,
            ptTaskDetail: false,
            xhTaskDetail: false,

            searchTreeName: '',
            searchEquipName: '',

            treeRefs: null,
            treeData: {
                noEdit: true,
                equipSetValue: [],
                equipSetName: [],
                value: ''
            },

            ptLoading: false,
            xhLoading: false,

            // 控制项的下拉列表
            sysList: [],
            sysControlTable: [],
            sysSelectValue: [],
            eqpList: [],
            eqpTreeList: [],

            defaultProps: {
                children: 'children',
                label: 'title'
            },

            casProp: {
                value: 'key',
                label: 'title',
                children: 'children'
            },

            // 新增普通任务的表单
            ptForm: {
                tableId: '',
                tableName: '',
                comment: '',
                procTaskSys: []
            },

            // 表格选中项
            sysSelection: [],
            eqpSelection: [],

            // 循环任务表单
            xhForm: {
                tableId: '',
                tableName: '',
                comment: '',
                cycleTask: {
                    // 循环任务表单信息
                    beginTime: '1900-01-01 00:00:00', // 开始时间
                    endTime: '1900-01-01 00:00:00', // 结束时间
                    intervalTime: [new Date(2020, 1, 1, 0, 0, 0), new Date(2020, 1, 1, 23, 59, 59)], // 开始时间与结束时间
                    zhenDianDo: '0',
                    zhidingDo: '0',
                    implement: '1',
                    cycleMustFinish: false, // 是否执行完整循环
                    zhidingTime: new Date(2020, 1, 1, 0, 0, 0), // 指定开始时间
                    isMaxCycle: false, // 最大循环次数
                    maxCycleNum: 0 // 最大循环次数
                },
                cycleTaskContent: []
            },

            checkAll: false,
            isIndeterminate: false,
            checkedArr: [],

            activeNames: [],

            // 循环任务右侧新增项
            xhSysForm: {
                control: ''
            },
            xhEqpForm: {
                control: [],
                value: 0
            },
            xhTimeForm: {
                day: undefined,
                hour: undefined,
                min: undefined,
                second: undefined,
                totalSeconds: undefined
            },

            // 用于储存表单设备控制值的diabled状态
            cascaderPtArr: [],
            cascaderXh: true,

            // 悬停样式的类名
            dialogBodyOuter: null,
            dialogBodyInner: null,
            disabled: true,

            // 2024-1-16 新增启停控制
            typeList: [
                {
                    label: 'taskRepository.dialog.taskSysItem',
                    value: 's'
                },
                {
                    label: 'taskRepository.dialog.taskEqpItem',
                    value: 'e'
                }
            ],
            rowRecordIndex: 0,
            loading: false,
            xhControlItem: 2
        }
    },
    components: {
        draggable,
        myTree
    },
    computed: {
        // 秒转化为天-时-分-秒
        getTime () {
            return function (val, type) {
                let str = ''
                val = Number(val)
                switch (type) {
                    case 'd': str = Math.floor(val / (24 * 3600)) === 0 ? 0 : Math.floor(val / (24 * 3600)); break;
                    case 'h': str = Math.floor((val % (24 * 3600)) / 3600) === 0 ? 0 : Math.floor((val % (24 * 3600)) / 3600); break;
                    case 'm': str = Math.floor((val % 3600) / 60) === 0 ? 0 : Math.floor((val % 3600) / 60); break;
                    case 's': str = Math.floor(val % 60) === 0 ? 0 : Math.floor(val % 60); break;
                    default: break;
                }
                return Number(str)
            }
        },
        dragOptions () {
            return {
                animation: 0,
                group: 'description',
                disabled: false,
                ghostClass: 'originPosition',
                dragClass: 'moveGhost',
                chosenClass: 'originPosition'
            }
        },
        ptRule () {
            return {
                tableName: [
                    {
                        type: 'string',
                        required: true,
                        message: this.$t('taskRepository.dialog.alarmMsgName')
                    },
                    {
                        max: 255,
                        message: this.$t('taskRepository.message.inputNameLength')
                    }
                ],
                comment: [
                    {
                        max: 255,
                        message: this.$t('taskRepository.message.inputCommentLength')
                    }
                ],
                procTaskSys: [
                    {
                        type: 'array',
                        required: true,
                        message: this.$t('taskRepository.message.addCtrlItem'),
                        trigger: 'change'
                    }
                ]
            }
        },
        xhRule () {
            return {
                tableName: [
                    {
                        type: 'string',
                        required: true,
                        message: this.$t('taskRepository.dialog.alarmMsgName')
                    },
                    {
                        max: 255,
                        message: this.$t('taskRepository.message.inputNameLength')
                    }
                ],
                comment: [
                    {
                        max: 255,
                        message: this.$t('taskRepository.message.inputCommentLength')
                    }
                ],
                cycleTask: {
                    intervalTime: [
                        {
                            type: 'array',
                            required: true,
                            validator: this.validateTime,
                            // message: this.$t('taskRepository.dialog.alarmMsgIntervalTime')
                        }
                    ]
                },
                cycleTaskContent: [
                    {
                        type: 'array',
                        required: true,
                        validator: (rule, value, callback) => {
                            let isHaveCtrl = value.findIndex(item => item.type == 'E' || item.type == 'S') > -1
                            if (!isHaveCtrl) {
                                callback(this.$t('taskRepository.message.addCtrlItem'))
                                return
                            }
                            callback()
                        }
                    }
                ],
                control: [{ required: false, message: this.$t('taskRepository.message.selectSysItem'), trigger: 'blur' }],
                eqControl: [{ type: 'array', required: false, message: this.$t('taskRepository.message.selectEqpItem'), trigger: 'change' }],
                totalSeconds: [{ required: false, message: this.$t('taskRepository.message.setTimeItem'), trigger: 'blur' }]
            }
        },
        procTaskSysRules () {
            return {
                procCode: [{ required: true, message: this.$t('taskRepository.message.selectSysItem'), trigger: 'change' }],
                linkValue: [{ required: true, message: this.$t('taskRepository.message.selectSysItem'), trigger: 'change' }],
                equipSetName: [
                    {
                        type: 'array',
                        required: true,
                        message: this.$t('taskRepository.message.selectEqpItem'),
                        trigger: 'change'
                    }
                ]
            }

        }
    },
    mounted () {
        this.getRepository()
        this.evtSysControl()
    },
    methods: {
        validateTime (_, value, callback) {
            let [time1, time2] = value || []
            if (time1 && time2) {
                time1 = new Date(time1).getTime()
                time2 = new Date(time2).getTime()
                if (time1 == time2) {
                    return callback(new Error(this.$t('taskRepository.dialog.oneSecondTimeTips')))
                }
            }
            return callback()
        },
        // 全屏、不全屏
        getContentFullscreen (type) {
            if (screenfull.isEnabled) {
                let contentFrame = document.getElementById(`dialog-body-xh-left`)
                screenfull.toggle(contentFrame)
            }
        },
        inputNumber (time) {
            let str = String(this.xhTimeForm[time])
            if (str.indexOf('.')) {
                this.xhTimeForm[time] = Number(str.split('.')[0])
            } else {
                this.xhTimeForm[time] = Number(str.replace(/[^\d]/g, ''))
            }
        },
        replaceInput (value, index) {
            let str = value.trim()
            let reg = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\]./?~！@#￥……&*（）——|{}【】‘；：”“'。，、？]")
            if (reg.test(str)) {
                str = ''
            }
            if (index != undefined) {
                this.ptForm.procTaskSys[index].value = str || ''
            } else {
                this.xhEqpForm.value = str || ''
            }
        },

        // 获取任务库列表
        getRepository () {
            this.taskLoad = true
            this.$api
                .tkGetRepository({
                    pageSize: this.pageSize,
                    pageNo: this.pageNo,
                    searchName: this.searchName
                })
                .then(res => {
                    let dat = res.data.data
                    let code = res.data.code
                    if (code !== 200) {
                        let msg = res.data.message
                        this.$message.error(msg, res)
                        return
                    }
                    let list = dat.rows || []
                    let target = ''
                    let obj = {}
                    this.total = dat.totalCount
                    this.taskRepository = []
                    for (let i = 0; i < list.length; i++) {
                        target = list[i]
                        obj = {}
                        obj.tableName = target.tableName
                        obj.tableId = target.tableId
                        obj.remark = target.remark === '' ? '-' : target.remark
                        obj.tableType = target.tableType
                        this.taskRepository.push(obj)
                    }
                    this.taskLoad = false
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                    console.log(err)
                    this.taskLoad = false
                })
        },

        addPtTask () {
            this.ptTaskDetail = true
            this.visible = true
            this.ptForm.procTaskSys = []
        },

        addXhTask () {
            this.xhTaskDetail = true
            this.$nextTick(() => {
                this.$refs['xhForm'].resetFields()
                this.xhForm.tableName = ''
            })
        },

        // 获取系统控制
        evtSysControl () {
            this.$api
                .evtSysControl()
                .then(res => {
                    if (res.data && res.data.code === 200) {
                        this.sysList = res.data.data
                        // 处理成 el-cascader所需级联格式
                        let setData = res.data.data.map(item => {
                            return item.procName
                        }), setTable = Array.from(new Set(setData)),
                            resultObject = []
                        if (setTable.length > 0) {
                            setTable.forEach(element => {
                                resultObject.push({
                                    value: element,
                                    label: element,
                                    children: []
                                })
                            });
                        }
                        if (res.data.data.length > 0 && resultObject.length > 0) {
                            res.data.data.forEach(element => {
                                resultObject.forEach(elementChild => {
                                    if (element.procName == elementChild.value) {
                                        let obj = element
                                        obj.value = obj.label = element.cmdNm
                                        obj.linkValue = []
                                        elementChild.children.push(obj)
                                    }
                                })
                            });
                        }
                        this.sysControlTable = resultObject
                    } else {
                        this.$message.error(res.data.message, res)
                    }
                })
                .catch(er => {
                    this.$message.error(er.data, er)
                })
        },
        linkChange (arry, index) {
            if (!arry) return
            this.ptForm.procTaskSys[index].procName = arry[0]
            this.ptForm.procTaskSys[index].cmdNm = arry[1]
            this.ptForm.procTaskSys[index].linkValue = arry
            this.ptForm.procTaskSys[index].procCode = this.getSysAttribute(arry[0], arry[1], 'procCode')
        },
        // 获取设备控制
        GetEquipSetParmTreeList () {
            this.$api
                .getGroupEquip()
                .then(res => {
                    this.treeLoading = false
                    if (res.data.code === 200) {
                        this.eqpTreeList = res.data.data
                    } else {
                        this.$message.error(res.data.message, res)
                    }
                })
                .catch(er => {
                    this.treeLoading = false
                    this.$message.error(er.data, er)
                })
        },

        inputChange () {
            this.searchTreeName = this.searchTreeName.trim()
            this.$refs.myTree.filterMethod(this.searchTreeName)
        },

        // 树状结构点击事件
        handleNodeClick (data) {
            if (data.isSetting) {
                if (this.ptTaskDetail || this.xhTaskDetail) {
                    this.treeData.noEdit = data.setType !== 'V'// 设置设置值是否可输入，选择遥测时可输入，选择遥信则不可输入
                    this.treeData.value = this.treeData.noEdit ? (data.setValue || '') : '0'
                    this.treeData.equipSetValue = [data.equipNo, data.setNo]
                    this.treeData.defaultValue = data.value || ''
                    this.treeData.equipSetName = data.equipName + '-' + data.title
                }
                if (this.xhTaskDetail) {
                    this.confirmTree()
                }

            } else {
                this.treeData.noEdit = true
                this.treeData.value = ''
                this.treeData.equipSetValue = []
            }
        },

        cancelTree () {
            this.searchTreeName = ''
            this.eqpTreeList = []
            this.treeLoading = false
        },

        closedTree () {
            // this.eqpTreeList = this.eqpList;
        },

        confirmTree () {
            if (this.treeData.equipSetValue.length == 0) {
                return this.$message.warning(this.$t('taskRepository.message.selectEqpItem'))
            }

            if (this.ptTaskDetail) {
                this.$set(this.cascaderPtArr, this.treeRefs - 1, this.treeData.noEdit)
                this.ptForm.procTaskSys[this.treeRefs - 1].value = this.treeData.noEdit ? this.treeData.value : '0'

                // 设置选中测点
                this.ptForm.procTaskSys[this.treeRefs - 1].equipSetValue = JSON.parse(JSON.stringify(this.treeData.equipSetValue))
                this.ptForm.procTaskSys[this.treeRefs - 1].equipSetName = this.treeData.equipSetName
                this.ptForm.procTaskSys[this.treeRefs - 1].value = this.treeData.defaultValue
                this.ptForm.procTaskSys[this.treeRefs - 1].noEdit = this.treeData.noEdit

            } else if (this.xhTaskDetail) {

                // 设置选中测点
                this.cascaderXh = this.treeData.noEdit
                this.xhEqpForm.control = JSON.parse(JSON.stringify(this.treeData.equipSetValue))
                this.xhEqpForm.controlName = this.treeData.equipSetName
                this.xhEqpForm.value = this.treeData.value
            }
            this.treeDialog = false
        },

        focusCascader (ref) {
            this.treeDialog = true
            this.treeLoading = true
            this.treeRefs = ref || this.treeRefs
        },

        searchList (e, change) {
            this.searchName = this.searchName.trim()
            if (this.searchName || change) {
                this.pageNo = 1
                this.getRepository()
            }
        },

        currentChange (val) {
            this.pageNo = val
            this.getRepository()
        },

        // 页码大小改变时触发事件
        sizeChange (pageSize) {
            this.pageSize = pageSize
            this.getRepository()
        },

        // 表格样式
        cellStyle ({ row, column, rowIndex, columnIndex }) {
            if (columnIndex === 1) {
                // 指定坐标
                return 'color: transparent !important'
            }
        },

        // 普通任务系统控制列表选中事件
        sysSelectionChange (val) {
            this.sysSelection = val
        },

        // 普通任务设备控制列表选中事件
        eqpSelectionChange (val) {
            this.eqpSelection = val
        },

        // 普通任务增加系统控制项
        addPtTaskSys () {
            this.$refs.procTaskSys.validate((valid) => {
                if (valid) {
                    this.ptForm.procTaskSys.push({
                        time: new Date(1900, 1, 1, new Date().getHours(), new Date().getMinutes(), new Date().getSeconds()),
                        timeDur: new Date(1900, 1, 1, 0, 0, 5),
                        procCode: '',
                        cmdNm: '',
                        procType: 's',
                        equipSetName: '',
                        value: '',
                        isCanRun: true,
                        linkValue: []
                    })
                    this.$nextTick(() => {
                        let dom = document.getElementById('taskSysTable');
                        if (dom) {
                            let scrollDom = dom.getElementsByClassName('el-table__body-wrapper')[0];
                            let scrollHeight = scrollDom.getElementsByClassName('el-table__body')[0] ? scrollDom.getElementsByClassName('el-table__body')[0].clientHeight : 0;
                            scrollDom.scrollTo(0, scrollHeight)
                        }
                    })
                }
            })
        },

        // 普通任务删除系统控制项
        deletePtTaskSys () {
            if (this.sysSelection.length === 0) {
                this.$message({
                    type: 'warning',
                    title: this.$t('taskRepository.message.selectDelSysItem')
                })
                return
            }
            let currentObject = [];
            for (let j in this.ptForm.procTaskSys) {
                if (!this.sysSelection.includes(this.ptForm.procTaskSys[j])) {
                    currentObject.push(this.ptForm.procTaskSys[j])
                }
            }
            this.sysSelection = []
            this.ptForm.procTaskSys = currentObject
        },

        // 普通任务增加设备控制项
        addPtTaskEqp () {
            this.ptForm.procTaskSys.push({
                time: new Date(1900, 1, 1, new Date().getHours(), new Date().getMinutes(), new Date().getSeconds()),
                timeDur: new Date(1900, 1, 1, 0, 0, 5),
                procCode: '',
                cmdNm: '',
                procType: 's',
                equipSetName: '',
                value: '',
                isCanRun: true
            })
            this.$set(this.cascaderPtArr, this.ptForm.procTaskSys.length - 1, true)
        },

        // 普通任务删除设备控制项
        deletePtTaskEqp () {
            if (this.eqpSelection.length === 0) {
                this.$message({
                    type: 'warning',
                    title: this.$t('taskRepository.message.selectDelEqpItem')
                })
                return
            }
            for (let i in this.eqpSelection) {
                for (let j in this.ptForm.procTaskSys) {
                    if (this.eqpSelection[i].index === this.ptForm.procTaskSys[j].index) {
                        this.ptForm.procTaskSys.splice(j, 1)
                    }
                }
            }
        },

        handleCheckAllChange (val) {
            this.checkedArr = val ? this.xhForm.cycleTaskContent : []
            this.isIndeterminate = false
        },
        handleCheckedChange (value) {
            this.setCheckState()
        },

        delItem () {
            let flag = true
            for (let i = 0; i < this.xhForm.cycleTaskContent.length; flag ? i++ : i) {
                for (let j = 0; j < this.checkedArr.length; j++) {
                    if (this.xhForm.cycleTaskContent[i].nowTime == this.checkedArr[j].nowTime) {
                        this.xhForm.cycleTaskContent.splice(i, 1)
                        flag = false
                        break
                    } else {
                        flag = true
                    }
                }
            }
            this.checkedArr = []

            this.setCheckState()
        },

        setCheckState () {
            if (this.xhForm.cycleTaskContent.length == 0) {
                this.checkAll = false
                this.isIndeterminate = false
            } else if (this.checkedArr.length == this.xhForm.cycleTaskContent.length) {
                this.checkAll = true
                this.isIndeterminate = false
            } else if (this.checkedArr.length < this.xhForm.cycleTaskContent.length && this.checkedArr.length > 0) {
                this.checkAll = false
                this.isIndeterminate = true
            } else {
                this.checkAll = false
                this.isIndeterminate = false
            }
        },

        // 增加循环任务系统控制项
        addSysItem () {
            this.$refs['xhSysForm'].validate(valid => {
                if (valid) {
                    this.xhForm.cycleTaskContent.push({
                        type: 'S',
                        procCode: this.getSysAttribute(this.xhSysForm.control[0], this.xhSysForm.control[1], 'procCode'),
                        firstLevelDirectory: this.xhSysForm.control[0] + '/',
                        cmdNm:  this.xhSysForm.control[1],//this.xhSysForm.control[0] + '/' +
                        nowTime: +new Date(),
                        icon: 'iconfont icon-tubiao14_shezhi',
                        color: '#BA38FF'
                    })
                    // this.xhSysForm.control = ''
                    this.setCheckState()
                    let formsVliadte = ['cycleTaskContent', 'xhForm'].map(form => this.ValidatePromisfy(form))
                }
            })
        },

        // 初始控制项
        initSysSelected () {
            this.xhControlItem = 1;
            if (this.sysControlTable.length > 0 && this.sysControlTable[0].children.length > 0 && this.xhSysForm.control == '') {
                this.xhSysForm.control = [this.sysControlTable[0].value, this.sysControlTable[0].children[0].cmdNm]
            }
        },

        // 增加循环任务设备控制项
        addEqpItem () {
            this.$refs.xhEqpForm.validate(valid => {
                if (valid) {
                    this.xhForm.cycleTaskContent.push({
                        type: 'E',
                        equipNo: this.xhEqpForm.control[0],
                        setNo: this.xhEqpForm.control[1],
                        value: this.xhEqpForm.value ? String(this.xhEqpForm.value) : '',
                        equipSetNm: this.xhEqpForm.controlName,
                        nowTime: +new Date(),
                        attribute: this.xhEqpForm.value ? 'yc' : 'yx',
                        icon: 'iconfont icon-gw-icon-yijimenu-xitongyunwei',
                        color: '#3875FF'
                    })
                    this.cascaderXh = true
                    this.setCheckState()
                    let formsVliadte = ['cycleTaskContent', 'xhForm'].map(form => this.ValidatePromisfy(form))
                }
            })
        },

        // 添加控制项
        addConcatControlItem () {
            // console.log(this.xhEqpForm)
            // this.confirmTree()
            if (this.xhSysForm.control && this.xhControlItem == 1) this.addSysItem()
            if (this.xhEqpForm.controlName && this.xhControlItem == 2) {
                this.addEqpItem()
            }
            if ((this.xhTimeForm.day || this.xhTimeForm.hour || this.xhTimeForm.min || this.xhTimeForm.second) && this.xhControlItem == 3) this.addTimeItem()

        },

        isUndefined (val) {
            let num = typeof val === 'undefined' ? 0 : Number(val)
            return num
        },

        // 增加循环任务时间间隔
        addTimeItem () {
            this.$refs['xhTimeForm'].validate(valid => {
                if (valid) {
                    this.xhForm.cycleTaskContent.push({
                        type: 'T',
                        sleepTime: this.xhTimeForm.totalSeconds,
                        sleepUnit: 'S',
                        nowTime: +new Date(),
                        icon: 'iconfont icon-gw-icon-menu-dingshibaobiao',
                        color: '#2EBFDF',
                        day: this.xhTimeForm.day || 0,
                        hour: this.xhTimeForm.hour || 0,
                        minutes: this.xhTimeForm.min || 0,
                        second: this.xhTimeForm.second || 0,
                    })
                    this.xhTimeForm = {
                        day: undefined,
                        hour: undefined,
                        min: undefined,
                        second: undefined,
                        totalSeconds: undefined
                    }
                    this.setCheckState()
                }
            })
        },

        // 根据系统控制编号获取名称
        getSysAttribute (taskCode, taskName, id) {
            let currentValue = ''
            for (let item of this.sysList) {
                let setValue = (id == 'procCode' ? item.procName : item.procCode)
                if (setValue == taskCode && item.cmdNm === taskName) {  // 系统控制1、2级名称对比，从而获取procode或者其他
                    currentValue = item[id]
                }
            }
            return currentValue
        },

        setTimeValue (value, index, type) {

            let totalSeconds =
                this.isUndefined(this.xhForm.cycleTaskContent[index].day) * 24 * 60 * 60 +
                this.isUndefined(this.xhForm.cycleTaskContent[index].hour) * 60 * 60 +
                this.isUndefined(this.xhForm.cycleTaskContent[index].minutes) * 60 +
                this.isUndefined(this.xhForm.cycleTaskContent[index].second)
            this.xhForm.cycleTaskContent[index].sleepTime = totalSeconds

            if (isNaN(value)) {
                this.xhForm.cycleTaskContent[index][type] = 0
            }

            this.$forceUpdate()
        },

        // 编辑任务
        showEdit (val, type) {

            this.loading = true;
            // 普通任务
            if (type === 'T') {
                this.ptTaskDetail = true
                this.isNewPtTask = false
                this.$api
                    .GetCommonTaskData({
                        tableId: val
                    })
                    .then(res => {
                        if (res.data && res.data.code === 200) {
                            let data = res.data.data || []

                            // 整理返回的数据赋值给表单
                            this.ptForm.tableId = data.tableId
                            this.ptForm.tableName = data.tableName
                            this.ptForm.comment = data.comment
                            this.ptForm.procTaskSys = []
                            this.ptForm.procTaskEqp = []

                            // 系统控制项
                            for (let item of data.procTaskSys) {
                                let timeDate = new Date(item.time)
                                let timeDurDate = new Date(item.timeDur)
                                this.ptForm.procTaskSys.push({
                                    time: new Date(1900, 1, 1, timeDate.getHours(), timeDate.getMinutes(), timeDate.getSeconds()),
                                    timeDur: new Date(1900, 1, 1, timeDurDate.getHours(), timeDurDate.getMinutes(), timeDurDate.getSeconds()),
                                    procCode: item.procCode,
                                    cmdNm: item.cmdNm,
                                    isCanRun: item.isCanRun,
                                    id: item.id,
                                    procName: this.getSysAttribute(item.procCode, item.cmdNm, 'procName'),
                                    procType: 's',
                                    equipSetName: '',
                                    equipSetValue: '',
                                    value: '',
                                    linkValue: [this.getSysAttribute(item.procCode, item.cmdNm, 'procName'), item.cmdNm]
                                })
                            }


                            // 设备控制项
                            for (let i = 0; i < data.procTaskEqp.length; i++) {
                                this.$set(this.cascaderPtArr, i, true)
                                let item = data.procTaskEqp[i]
                                let timeDate = new Date(item.time)
                                let timeDurDate = new Date(item.timeDur)
                                this.ptForm.procTaskSys.push({
                                    time: new Date(1900, 1, 1, timeDate.getHours(), timeDate.getMinutes(), timeDate.getSeconds()),
                                    timeDur: new Date(1900, 1, 1, timeDurDate.getHours(), timeDurDate.getMinutes(), timeDurDate.getSeconds()),
                                    equipSetNm: item.equipSetNm,
                                    equipSetValue: [item.equipNo, item.setNo],
                                    equipSetName: item.equipSetNm,
                                    value: String(item.value),
                                    isCanRun: item.isCanRun,
                                    id: item.id,
                                    procType: 'e',
                                    procCode: '',
                                    cmdNm: '',
                                    noEdit: item.noEdit
                                })
                            }
                        } else {
                            this.$message.error(res.data.message, res)
                        }
                    })
                    .catch(err => {
                        console.log(err)
                        this.$message.error(err.data, err)
                    }).finally(() => {
                        this.loading = false
                    })

                // 循环任务
            } else {
                this.xhTaskDetail = true
                this.isNewXhTask = false
                this.$api
                    .GetCycleTaskData({
                        tableId: val
                    })
                    .then(res => {
                        if (res.data.code === 200) {
                            let data = res.data.data

                            this.xhForm.tableId = data.tableId
                            this.xhForm.tableName = data.tableName
                            this.xhForm.comment = data.comment
                            this.xhForm.cycleTaskContent = data.cycleTaskContent

                            // 给设置项加一个唯一键
                            for (let i = 0; i < data.cycleTaskContent.length; i++) {
                                let result = this.xhForm.cycleTaskContent[i];
                                result.nowTime = +new Date() + i
                                result.attribute = result.value ? 'yc' : 'yx'
                                if (result.type == "E") {
                                    result.icon = 'iconfont icon-gw-icon-yijimenu-xitongyunwei'
                                    result.color = '#3875FF'
                                } else if (result.type == "S") {
                                    result.icon = 'iconfont icon-tubiao14_shezhi'
                                    result.color = '#BA38FF'
                                    result.firstLevelDirectory = this.getSysAttribute(result.procCode, result.cmdNm, 'procName')
                                } else {
                                    result.icon = 'iconfont icon-gw-icon-menu-dingshibaobiao'
                                    result.color = '#2EBFDF'
                                    result.day = this.getTime(result.sleepTime, 'd')
                                    result.hour = this.getTime(result.sleepTime, 'h')
                                    result.minutes = this.getTime(result.sleepTime, 'm')
                                    result.second = this.getTime(result.sleepTime, 's')
                                }
                            }

                            let dateBegin = new Date(data.cycleTask.beginTime)
                            let dateEnd = new Date(data.cycleTask.endTime)
                            let currentYear = this.$moment().format("YYYY"),
                            currentMonth = this.$moment().format("MM")-1,
                            currentDay = this.$moment().format("DD")
                            this.xhForm.cycleTask.intervalTime = [
                                new Date(currentYear, currentMonth, currentDay, dateBegin.getHours(), dateBegin.getMinutes(), dateBegin.getSeconds()),
                                new Date(currentYear, currentMonth, currentDay, dateEnd.getHours(), dateEnd.getMinutes(), dateEnd.getSeconds())
                            ]
                            if (data.cycleTask.zhenDianDo) {
                                this.xhForm.cycleTask.implement = '2'
                            } else if (data.cycleTask.zhidingDo) {
                                this.xhForm.cycleTask.implement = '3'
                            } else {
                                this.xhForm.cycleTask.implement = '1'
                            }
                            let dateZhiding = new Date(data.cycleTask.zhidingTime)
                            this.xhForm.cycleTask.zhidingTime = new Date(currentYear, currentMonth, currentDay, dateZhiding.getHours(), dateZhiding.getMinutes(), dateZhiding.getSeconds())
                            this.xhForm.cycleTask.isMaxCycle = data.cycleTask.maxCycleNum !== 0
                            this.xhForm.cycleTask.maxCycleNum = data.cycleTask.maxCycleNum
                            this.xhForm.cycleTask.cycleMustFinish = data.cycleTask.cycleMustFinish

                        } else {
                            this.$message.error(res.data.message, res)
                        }
                    })
                    .catch(err => {
                        console.log(err)
                        this.$message.error(err.data, err)
                    }).finally(() => {
                        this.loading = false
                    })
            }
        },

        ValidatePromisfy (formRef) {
            return new Promise((resolve, reject) => {
                this.$refs[formRef].validate(valid => {
                    if (valid) {
                        resolve(valid)
                    } else {
                        reject(formRef)
                    }
                })
            })
        },

        // 保存普通任务
        savePtTask () {
            this.ptForm.tableName = this.ptForm.tableName.trim()
            this.ptForm.comment = this.ptForm.comment ? this.ptForm.comment.trim() : this.ptForm.comment
            let formsVliadte = ['ptForm', 'procTaskSys'].map(form => this.ValidatePromisfy(form))
            Promise.all(formsVliadte)
                .then(res => {
                    if (res.every(i => i)) {
                        // 获取表单数据重新整理出接口参数
                        let obj = {}
                        obj.tableName = this.ptForm.tableName
                        obj.comment = this.ptForm.comment
                        if (!this.isNewPtTask) {
                            obj.tableId = this.ptForm.tableId
                        }
                        obj.procTaskSys = []
                        obj.procTaskEqp = []

                        // 系统控制项
                        for (let i = 0; i < this.ptForm.procTaskSys.length; i++) {
                            let item = this.ptForm.procTaskSys[i]
                            if (item.procType == 's') {
                                obj.procTaskSys.push({
                                    procCode: item.procCode,
                                    cmdNm: item.cmdNm,
                                    time: this.$moment(item.time).format('YYYY-MM-DD HH:mm:ss'),
                                    timeDur: this.$moment(item.timeDur).format('YYYY-MM-DD HH:mm:ss'),
                                    isCanRun: item.isCanRun,
                                    id: item.id
                                })
                            } else {
                                obj.procTaskEqp.push({
                                    equipNo: item.equipSetValue[0],
                                    setNo: item.equipSetValue[1],
                                    equipSetNm: item.equipSetName,
                                    value: String(item.value),
                                    time: `1900-01-01 ${this.myUtils.addZero(item.time.getHours(), 2)}:${this.myUtils.addZero(item.time.getMinutes(), 2)}:${this.myUtils.addZero(
                                        item.time.getSeconds(),
                                        2
                                    )}`,
                                    timeDur: `1900-01-01 ${this.myUtils.addZero(item.timeDur.getHours(), 2)}:${this.myUtils.addZero(item.timeDur.getMinutes(), 2)}:${this.myUtils.addZero(
                                        item.timeDur.getSeconds(),
                                        2
                                    )}`,
                                    isCanRun: item.isCanRun,
                                    id: item.id
                                })
                            }
                        }
                        if (obj.procTaskSys.length == 0 && obj.procTaskEqp.length == 0) {
                            this.$message({
                                type: 'warning',
                                title: this.$t('taskRepository.message.addCtrlItem')
                            })
                            return
                        }
                        let apiName = this.isNewPtTask ? 'CreateCommonTask' : 'EditCommonTaskData'
                        this.ptLoading = true
                        this.$api[apiName](obj)
                            .then(res => {
                                this.ptLoading = false
                                if (res.data.code === 200) {
                                    this.getRepository()
                                    this.$message({
                                        type: 'success',
                                        title: this.isNewPtTask ? this.$t('taskRepository.publics.tips.addSuccess') : this.$t('taskRepository.publics.tips.editSuccess')
                                    })
                                    this.ptTaskDetail = false
                                } else {
                                    this.$message.error(res.data.message, res)
                                }
                            })
                            .catch(er => {
                                this.ptLoading = false
                                this.$message.error(er.data, er)
                            })
                    }
                })
                .catch(err => {
                    console.log(err)
                })
        },

        // 新增循环任务
        saveXhTask () {
            this.xhForm.tableName = this.xhForm.tableName.trim()
            this.xhForm.comment = this.xhForm.comment ? this.xhForm.comment.trim() : this.xhForm.comment

            let formsVliadte = ['cycleTaskContent', 'xhForm'].map(form => this.ValidatePromisfy(form))
            Promise.all(formsVliadte)
                .then(res => {
                    if (res.every(i => i)) {
                        // 获取表单数据重新整理出接口参数
                        let obj = {}
                        obj.tableName = this.xhForm.tableName
                        obj.comment = this.xhForm.comment
                        if (!this.isNewXhTask) {
                            obj.tableId = this.xhForm.tableId
                        }
                        obj.cycleTask = {}
                        obj.cycleTaskContent = []

                        let cycleTask = this.xhForm.cycleTask

                        obj.cycleTask.beginTime = this.myUtils.dateFormat(cycleTask.intervalTime[0], 'yyyy-MM-dd hh:mm:ss')
                        obj.cycleTask.endTime = this.myUtils.dateFormat(cycleTask.intervalTime[1], 'yyyy-MM-dd hh:mm:ss')
                        obj.cycleTask.zhenDianDo = cycleTask.implement === '2' ? true : false
                        obj.cycleTask.zhidingDo = cycleTask.implement === '3' ? true : false
                        obj.cycleTask.cycleMustFinish = cycleTask.cycleMustFinish

                        obj.cycleTask.zhidingTime = this.myUtils.dateFormat(cycleTask.zhidingTime, 'yyyy-MM-dd hh:mm:ss')
                        obj.cycleTask.maxCycleNum = Number(cycleTask.maxCycleNum)
                        // 过滤空时间
                        this.xhForm.cycleTaskContent = this.xhForm.cycleTaskContent.filter(item => {
                            return !(item.type == "T" && item.sleepTime == 0)
                        })

                        obj.cycleTaskContent = JSON.parse(JSON.stringify(this.xhForm.cycleTaskContent))

                        // 判断是新增还是编辑
                        let apiName = this.isNewXhTask ? 'CreateCycleTask' : 'EditCycleyTaskData'
                        this.xhLoading = true
                        this.$api[apiName](obj)
                            .then(res => {
                                this.xhLoading = false
                                if (res.data.code === 200) {
                                    this.getRepository()
                                    this.$message({
                                        type: 'success',
                                        title: this.isNewXhTask ? this.$t('taskRepository.publics.tips.addSuccess') : this.$t('taskRepository.publics.tips.editSuccess')
                                    })
                                    this.xhTaskDetail = false
                                } else {
                                    this.$message.error(res.data.message, res)
                                }
                            })
                            .catch(er => {
                                this.xhLoading = false
                                this.$message.error(er.data, er)
                            })
                    }
                })
                .catch(err => {
                    console.log(err)
                })
            return
        },

        // 删除任务
        delData (name, val, type) {
            let apiName = type === 'T' ? 'DelCommonData' : 'DelCycleData'

            this.$msgbox({
                title: this.$t('taskRepository.message.tips'),
                message: this.$t('taskRepository.message.delMsg') + name + '?',
                showCancelButton: true,
                confirmButtonText: this.$t('taskRepository.publics.button.confirm'),
                cancelButtonText: this.$t('taskRepository.publics.button.cancel'),
                beforeClose: (action, instance, done) => {
                    if (action === 'confirm') {
                        instance.confirmButtonLoading = true
                        setTimeout(() => {
                            this.taskLoad = true
                            this.$api[apiName]({
                                tableId: val
                            })
                                .then(res => {
                                    let code = res.data.code
                                    if (code === 200) {
                                        this.$message({
                                            type: 'success',
                                            title: this.$t('taskRepository.publics.tips.deleteSuccess')
                                        })
                                        this.getRepository()
                                    } else {
                                        this.$message.error(res.data.message, res)
                                    }
                                    this.taskLoad = false
                                    instance.confirmButtonLoading = false
                                    done()
                                })
                                .catch(err => {
                                    this.$message.error(err.data, err)
                                    console.log(err)
                                    this.taskLoad = false
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
        },

        // 关闭普通任务框回调事件
        closePtDetail () {
            this.isNewPtTask = true
            this.$refs['ptForm'].resetFields()
            this.$refs['procTaskSys'].resetFields()
            this.ptForm = {
                tableId: '',
                tableName: '',
                comment: '',
                procTaskSys: [
                    {
                        time: new Date(1900, 1, 1, new Date().getHours(), new Date().getMinutes(), new Date().getSeconds()),
                        timeDur: new Date(1900, 1, 1, 0, 0, 5),
                        procCode: '',
                        cmdNm: '',
                        procType: 's',
                        equipSetName: '',
                        value: '',
                        isCanRun: true
                    }
                ]
            }
        },

        // 关闭循环任务框回调事件
        closeXhDetail () {
            this.isNewXhTask = true
            this.checkedArr = []
            this.checkAll = false
            this.isIndeterminate = false
            this.$refs['xhForm'].resetFields()
            this.$refs['cycleTaskContent'].resetFields()
            try { this.$refs['xhSysForm'].resetFields() } catch (e) { }
            try { this.$refs['xhEqpForm'].resetFields() } catch (e) { }
            try { this.$refs['xhTimeForm'].resetFields() } catch (e) { }

            this.xhForm = {
                tableId: '',
                tableName: '',
                comment: '',
                cycleTask: {
                    beginTime: '1900-01-01 00:00:00',
                    endTime: '1900-01-01 00:00:00',
                    intervalTime: [new Date(2020, 1, 1, 0, 0, 0), new Date(2020, 1, 1, 23, 59, 59)],
                    zhenDianDo: '0',
                    zhidingDo: '0',
                    implement: '1',
                    cycleMustFinish: false,
                    zhidingTime: new Date(2020, 1, 1, 0, 0, 0),
                    isMaxCycle: false,
                    maxCycleNum: 0
                },
                cycleTaskContent: []
            }

            this.xhSysForm = {
                control: ''
            }
            this.xhEqpForm = {
                control: [],
                value: 0
            }
            this.xhTimeForm = {
                day: undefined,
                hour: undefined,
                min: undefined,
                second: undefined,
                totalSeconds: undefined
            }
        },
        /**
         * @description: 循环任务设备控制值设置
         * @return {*}
         */
        setEquipValue (index) {
            if (this.xhForm.cycleTaskContent && this.xhForm.cycleTaskContent[index] && this.xhForm.cycleTaskContent[index].value == '') {
                this.xhForm.cycleTaskContent[index].value = 0
            }
        },

        /**
         * @description: 自定义校验
         * @return {*}
         */
        validateName (rule, value, callback) {
            let currentValue = typeof value == 'object' ? (value.length > 0) : value
            if (currentValue) { callback() } else {
                callback(new Error('error'))
            }
        },

        /**
         * @description: 设置选择任务状态
         * @return {*}
         */
        setSelectTaskStatus (isCurrentStatus) {
            if (this.ptForm && this.ptForm.procTaskSys)
                for (let j in this.ptForm.procTaskSys) {
                    if (this.sysSelection.includes(this.ptForm.procTaskSys[j])) {
                        this.ptForm.procTaskSys[j].isCanRun = isCurrentStatus
                    }
                }
        },
        /**
         * @description: 移除任务ID
         * @return {*}
         */
        removeId (item) {
            if (this.ptForm && this.ptForm.procTaskSys)
                for (let j in this.ptForm.procTaskSys) {
                    if (item == this.ptForm.procTaskSys[j]) {
                        delete this.ptForm.procTaskSys[j].id
                    }
                }
        }

    },
    watch: {
        searchTreeName: function (newVal) {
            if (!newVal) {
                this.$refs.myTree.filterMethod()
            }
        },
        xhForm: function (newVal) {
            this.$forceUpdate()
        },
        searchName (val, oldValue) {
            if (!val && oldValue.trim()) {
                this.searchList(null, true)
            }
        },
        '$i18n.locale' (val) {
            this.locale = val
            this.$refs['ptForm'] && this.$refs['ptForm'].clearValidate()
            this.$refs['xhForm'] && this.$refs['xhForm'].clearValidate()
        },
        xhTimeForm: {
            deep: true,
            handler: function (newVal, oldVal) {
                let totalSeconds =
                    this.isUndefined(this.xhTimeForm.day) * 24 * 60 * 60 +
                    this.isUndefined(this.xhTimeForm.hour) * 60 * 60 +
                    this.isUndefined(this.xhTimeForm.min) * 60 +
                    this.isUndefined(this.xhTimeForm.second)
                newVal.totalSeconds = totalSeconds || undefined
            }
        }
    }
}
