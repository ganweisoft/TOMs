/**
 * @file 设备列表插件
 */
const treeV2 = () => import('gw-base-components-plus/treeV2');
let ycList = []
let yxList = []

export default {
    components: {
        treeV2
    },
    props: {
        topTilte: {
            // 标题
            type: String,
            default: ''
        },
        subTitle: {
            // 当需要选择设备控制项时，右侧标题，此时showSetParm必须传true
            type: String,
            default: 'equipListsIot.equipSelect.subTitle[0]'
        },
        confirm: {
            // 当点击确定时传所选设备
            type: Function,
            default: () => { }
        },
        selectedMode: {
            // 模式选择，1为单选，0为多选
            type: Number,
            default: 0
        },
        showInput: {
            type: Boolean,
            default: true
        },
        showDatePicker: {
            type: Boolean,
            default: false
        },
        equipSelected: {
            // 已选择的设备
            type: Array,
            default: () => []
        },
        showSetParm: {
            // 是否需要选择设备控制项
            type: Boolean,
            default: false
        },
        confirmButton: {
            type: String,
            default: '' // 确定按钮显示文字（确定，导出、、、等按钮）
        },
        exportLoading: {
            type: Boolean,
            default: false
        },

        treeUpdate: {
            type: String,
            default: '' // 树形结构更新
        }
    },
    beforeDestroy () {
        this.equipList = null;
    },
    data () {
        return {
            equipLoading: false, // 设备加载loading
            searchName: '', // 搜索框
            selectedList: [],
            total: 10,
            pageNo: 1,
            pageSize: 5,
            dialogVisible: true,
            equipNo: -1,
            setParmList: [],
            setParmCurrentPage: 1,
            setParmPageSize: 8,
            setParmTotal: 0,
            setParmCurrentIndex: -1,
            setParmData: {
                setNo: -1,
                setName: ''
            },
            dateTime: [
                new Date(2000, 10, 10, 10, 10),
                new Date(2000, 10, 11, 10, 10)
            ],
            ECStartTime: '', // 导出曲线开始时间
            ECEndTime: '', // 导出曲线结束时间
            ycpSearch: '',
            yxpSearch: '',
            ycpAllSelect: false,
            yxpAllSelect: false,
            equipsList: [],
            ycSelect: [],
            yxSelect: [],
            pickerOptions: {
                disabledDate: time => {
                    return time.getTime() >= this.currentDay();
                }
            },
            getYxYcLoading: false,
            timeSelect: '',
            equipList: [],
            equipNos: [],
            selectEquips: [],
            id: null,
            ycps: [],
            yxps: [],
            allYcps: [],
            allYxps: [],
            filterObject: {},
            showTree: false,
            ycHalfSelect: false,
            yxHalfSelect: false
        };
    },
    mounted () {
        this.ECStartTime = this.$moment()
            .subtract(0, 'days')
            .startOf('day')
            .format('YYYY-MM-DD HH:mm:ss');
        this.ECEndTime = this.$moment()
            .subtract(0, 'days')
            .endOf('day')
            .format('YYYY-MM-DD HH');
        this.ECEndTime = this.ECEndTime + ':59'

        this.timeSelect = [this.ECStartTime, this.ECEndTime];
        this.equipExistsCurveRcord()
    },
    watch: {
        ycSelect () {
            this.updateYcSelect()
        },
        yxSelect () {
            this.updateYxSelect()
        },
        ycpSearch (val) {
            if (!val) {
                this.searchList('ycps')
            }
            this.updateYcSelect()
        },
        yxpSearch (val) {
            if (!val) {
                this.searchList('yxps')
            }
            this.updateYxSelect()
        }
    },
    methods: {
        updateYcSelect () {
            if (this.ycSelect.length) {
                if (this.ycSelect.length == this.allYcps.length) {
                    this.ycpAllSelect = true
                    this.ycHalfSelect = false
                } else {
                    this.ycpAllSelect = false
                    this.ycHalfSelect = true
                }
            } else {
                this.ycpAllSelect = false
                this.ycHalfSelect = false
            }
        },
        updateYxSelect () {
            if (this.yxSelect.length) {
                if (this.yxSelect.length == this.allYxps.length) {
                    this.yxpAllSelect = true
                    this.yxHalfSelect = false
                } else {
                    this.yxpAllSelect = false
                    this.yxHalfSelect = true
                }
            } else {
                this.yxpAllSelect = false
                this.yxHalfSelect = false
            }
        },
        equipExistsCurveRcord () {
            this.$api.equipExistsCurveRcord().then(res => {
                this.filterObject = res.data
            }).finally(() => {
                this.showTree = true
            })
        },
        filterData (arr) {
            return arr.filter(item => this.filterObject[item.id])
        },
        searchList (type) {
            if (type == 'ycps') {
                this.ycps = this.allYcps.filter(item => item.includes(this.ycpSearch))
            } else {
                this.yxps = this.allYxps.filter(item => item.includes(this.yxpSearch))
            }
        },
        getYcps () {
            let names = [];
            for (let item of ycList) {
                if (names.indexOf(item.ycName) == -1) {
                    if (this.ycpSearch) {
                        if (item.ycName.indexOf(this.ycpSearch) != -1) {
                            names.push(item.ycName);
                        }
                    } else {
                        names.push(item.ycName);
                    }
                }
            }

            if (
                this.ycSelect.length > 0 &&
                names.length > 0 &&
                this.isContained(this.ycSelect, names)
            ) {
                this.ycpAllSelect = true;
            } else {
                this.ycpAllSelect = false;
            }

            this.allYcps = this.ycps = names
        },
        getYxps () {
            let names = [];
            for (let item of yxList) {
                if (names.indexOf(item.yxName) == -1) {
                    if (this.yxpSearch) {
                        if (item.yxName.indexOf(this.yxpSearch) != -1) {
                            names.push(item.yxName);
                        }
                    } else {
                        names.push(item.yxName);
                    }
                }
            }

            if (
                this.yxSelect.length > 0 &&
                names.length > 0 &&
                this.isContained(this.yxSelect, names)
            ) {
                this.yxpAllSelect = true;
            } else {
                this.yxpAllSelect = false;
            }
            this.allYxps = this.yxps = names
        },
        currentDay () {
            let date = new Date();
            date.setHours(23, 59, 59);
            return date.getTime();
        },
        timeChange (date) {
            date[1] = date[1].slice(0, date[1].length - 6) + ' 23:59'
            if (date) {
                this.ECStartTime = date[0];
                this.ECEndTime = date[1];
            } else {
                this.ECStartTime = this.ECEndTime = null;
            }
        },
        // 判断是否包含  a是否包含b
        isContained (a, b) {
            if (!(a instanceof Array) || !(b instanceof Array)) return false;
            if (a.length < b.length) return false;
            let aStr = a.toString();
            for (let i = 0, len = b.length; i < len; i++) {
                if (aStr.indexOf(b[i]) == -1) return false;
            }
            return true;
        },

        ycSelectChange (name) {
            let index = this.ycSelect.indexOf(name);
            if (index == -1) {
                this.ycSelect.push(name);
                if (this.ycSelect.length == ycList.length) {
                    this.ycpAllSelect = true;
                }
            } else {
                this.ycSelect.splice(index, 1);
                this.ycpAllSelect = false;
            }
        },

        yxSelectChange (name) {
            let index = this.yxSelect.indexOf(name);
            if (index == -1) {
                this.yxSelect.push(name);
                if (this.yxSelect.length == yxList.length) {
                    this.yxpAllSelect = true;
                }
            } else {
                this.yxSelect.splice(index, 1);
                this.yxpAllSelect = false;
            }
        },

        changeAllSelect (value, type) {
            if (type == 'yc') {
                if (ycList && ycList.length == 0) {
                    this.$message.warning(
                        this.$t('equipListsIot.tips.noSelect')
                    );
                    return;
                }
                if (!this[value]) {
                    this.ycSelect = [];
                } else {
                    this.ycSelect = [];
                    this.ycSelect = [...this.ycps];
                }
            } else {
                if (yxList && yxList.length == 0) {
                    this.$message.warning(
                        this.$t('equipListsIot.tips.noSelect')
                    );
                    return;
                }
                if (!this[value]) {
                    this.yxSelect = [];
                } else {
                    this.yxSelect = [];
                    this.yxSelect = [...this.yxps];
                }
            }
        },

        selectedSetParm (item) {
            this.setParmCurrentIndex = item.setNo;
            this.setParmData.setNo = item.setNo;
            this.setParmData.setName = item.setNm;
        },
        sendData () {
            let ycArr = ycList.filter(item => {
                return this.ycSelect.indexOf(item.ycName) != -1;
            });
            let yxArr = yxList.filter(item => {
                return this.yxSelect.indexOf(item.yxName) != -1;
            });
            // 根据设备号进行分组
            let ycMap = {};
            let res = [];
            let yxMap = {};
            for (
                let index = 0, length = ycArr.length;
                index < length;
                index++
            ) {
                let ai = ycArr[index];
                if (!ycMap[ai.equipNo]) {
                    ycMap[ai.equipNo] = {};
                    ycMap[ai.equipNo]['ycps'] = [ai.ycNo];
                    ycMap[ai.equipNo]['staNo'] = ai.staNo;
                } else {
                    ycMap[ai.equipNo]['ycps'].push(ai.ycNo);
                }
            }

            for (
                let index = 0, length = yxArr.length;
                index < length;
                index++
            ) {
                if (!yxMap[yxArr[index].equipNo]) {
                    yxMap[yxArr[index].equipNo] = {};
                    yxMap[yxArr[index].equipNo].yxps = [];
                }
                yxMap[yxArr[index].equipNo].yxps.push(yxArr[index].yxNo);
            }

            for (let item in ycMap) {
                res.push({
                    equipNo: Number(item),
                    yxps: yxMap[item] ? yxMap[item]['yxps'] : [],
                    staNo: ycMap[item]['staNo'] || 1,
                    ycps: ycMap[item]['ycps'] || []
                });
            }

            for (let item in yxMap) {
                if (!ycMap[item]) {
                    res.push({
                        equipNo: Number(item),
                        yxps: yxMap[item]['yxps'],
                        staNo: yxMap[item]['staNo'] || 1,
                        ycps: []
                    });
                }
            }
            let data = {
                beginTime: this.ECStartTime ? this.myUtils.dateFormat(new Date(this.ECStartTime), 'yyyy-MM-dd hh:mm:ss') : '',
                endTime: this.ECEndTime ? this.myUtils.dateFormat(new Date(this.ECEndTime), 'yyyy-MM-dd hh:mm:ss') : '',
                exportEquips: res,
                equipsList: this.equipNos
            };
            this.confirm(data);
        },

        getChecked () {
            this.ycSelect = [];
            this.yxSelect = [];
            this.equipNos = this.$refs.treeV2.getEquipSelectd();
            if (this.equipNos.length > 0) {
                this.getYxYcLoading = true
                this.$api
                    .getYcpsYxps({ equipNos: this.equipNos })
                    .then(res => {
                        const { code, data, message } = res?.data || {}
                        if (code == 200) {
                            ycList = data?.ycps || []
                            yxList = data?.yxps || []
                            this.getYcps()
                            this.getYxps()
                        } else {
                            this.$message.error(message)
                            yxList = [];
                            ycList = [];
                        }
                    })
                    .catch(err => {
                        this.$message.error(err?.data, err)
                        yxList = [];
                        ycList = [];
                    }).finally(() => {
                        this.getYxYcLoading = false
                    });
            } else {
                this.ycps = []
                this.yxps = []
                ycList = []
                yxList = []
            }
        },
    },
    beforeDestroy () {
        ycList = []
        yxList = []
        this.ycps = []
        this.yxps = []
    }
};
