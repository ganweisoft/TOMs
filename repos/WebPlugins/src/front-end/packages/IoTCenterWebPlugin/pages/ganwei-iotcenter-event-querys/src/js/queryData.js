function debounce (func, wait) {
    let timer;
    return function () {
        if (timer) {
            clearTimeout(timer);
            timer = null
        }
        timer = setTimeout(() => {
            func.apply(this, arguments)
        }, wait);
    }
}

function deepClone1 (obj) {
    //判断拷贝的要进行深拷贝的是数组还是对象，是数组的话进行数组拷贝，对象的话进行对象拷贝
    var objClone = Array.isArray(obj) ? [] : {};
    //进行深拷贝的不能为空，并且是对象或者是
    if (obj && typeof obj === "object") {
        for (let key in obj) {
            if (obj.hasOwnProperty(key)) {
                if (obj[key] && typeof obj[key] === "object") {
                    objClone[key] = deepClone1(obj[key]);
                } else {
                    objClone[key] = obj[key];
                }
            }
        }
    }
    return objClone;
}
import dayjs from 'dayjs'
export default {
    data () {
        return {
            dataBaseOptions: [],
            dataBaseSelect: '',
            baseItemOptions: [],
            baseItemSelect: [],
            editBaseItems: [], //获取到需要编辑的选项
            tableData: [],
            multipleSelection: [],
            input: '',
            dataObject: {},
            pageLoading: false,

            times: [],
            timeRange: [],
            typeConnectSelect: 0,
            typeConnectOptions: [
                {
                    label: this.$t('queryData.typeConnectOptions.all'),
                    value: 0
                },
                {
                    label: this.$t('queryData.typeConnectOptions.any'),
                    value: 1
                },
                {
                    label: this.$t('queryData.typeConnectOptions.custom'),
                    value: 2
                }
            ],
            editBaseCheckBox: [],
            showEditBaseCheckPopper: false,
            showDefineDialog: false,
            popperSearchOptions: [],
            popperSearchContent: '',
            noMatch: true,
            relationSelect: 0,

            //高级查询
            seniorDataBaseSelect: '',
            seniorQuery: '',
            editSeniorQuery: '',
            seniorObject: {},
            seniorDaseItemSelect: '',
            searchType: 0, //查询类别,0为普通查询，1为自定义查询
            tabelHeader: [],
            pagination: {
                pageSize: 20,
                pageNo: 1,
                total: 0
            },
            exportLoading: false,
            show: true,
            temUse: [],
            myRefs: [],
            existIndex: '',
            tableHeaderTemUse: []
        }
    },
    computed: {
        temporaryUse () {
            return this.tableHeaderTemUse.slice(1, this.tableHeaderTemUse.length)
        },
        boolOparationOptions () {
            return [
                {
                    label: this.$t('queryData.boolOparationOptions.isOrNot'),
                    value: 1,
                    inputType: 'bool',
                    oparate: 'bool'
                }
            ]
        },
        boolNoNullOparationOptions () {
            return [
                {
                    label: this.$t('queryData.boolOparationOptions.isOrNot'),
                    value: 1,
                    inputType: 'bool',
                    oparate: 'bool'
                }
            ]
        },
        boolOptions () {
            return [
                {
                    label: this.$t('queryData.boolOptions.true'),
                    value: true
                },
                {
                    label: this.$t('queryData.boolOptions.false'),
                    value: false
                }
            ]
        },
        stringOparateOptions () {
            return [
                {
                    label: this.$t('queryData.analysisHeader.like'),
                    value: 0,
                    inputType: 'string',
                    oparate: 'like'
                },
                {
                    label: this.$t('queryData.stringOparateOptions.is'),
                    value: 2,
                    inputType: 'string',
                    oparate: 'equal'
                },
                {
                    label: this.$t('queryData.stringOparateOptions.not'),
                    value: 3,
                    inputType: 'string',
                    oparate: 'noEqual'
                },
                {
                    label: this.$t('queryData.analysisHeader.in'),    //用el-select，回车创建新label
                    value: 4,
                    inputType: 'multiString',
                    oparate: 'in'
                }
            ]
        },
        stringNoNullOparateOptions () {
            return [
                {
                    label: this.$t('queryData.analysisHeader.like'),
                    value: 0,
                    inputType: 'string',
                    oparate: 'like'
                },
                {
                    label: this.$t('queryData.stringOparateOptions.is'),
                    value: 2,
                    inputType: 'string',
                    oparate: 'equal'
                },
                {
                    label: this.$t('queryData.stringOparateOptions.not'),
                    value: 3,
                    inputType: 'string',
                    oparate: 'noEqual'
                },
                {
                    label: this.$t('queryData.analysisHeader.in'),    //用el-select，回车创建新label
                    value: 4,
                    inputType: 'multiString',
                    oparate: 'in'
                }
            ]
        },
        numberOparateOptions () {
            return [
                {
                    label: this.$t('queryData.numberOparateOptions.equal'),
                    value: 0,
                    inputType: 'number',
                    oparate: 'equal'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.notEqual'),
                    value: 1,
                    inputType: 'number',
                    oparate: 'noEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.bigger'),
                    value: 2,
                    inputType: 'number',
                    oparate: 'more'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.smaller'),
                    value: 3,
                    inputType: 'number',
                    oparate: 'less'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.biggerAndEqual'),
                    value: 4,
                    inputType: 'number',
                    oparate: 'moreEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.smallerAndEqual'),
                    value: 5,
                    inputType: 'number',
                    oparate: 'lessEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.inRange'),
                    value: 6,
                    inputType: 'numberRange',
                    oparate: 'range'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.outOfRange'),
                    value: 7,
                    inputType: 'numberRange',
                    oparate: 'outRange'
                },
                {
                    label: this.$t('queryData.analysisHeader.in'),    //用el-select，回车创建新label
                    value: 8,
                    inputType: 'multiString',
                    oparate: 'in'
                }
            ]
        },
        numberNoNullOparateOptions () {
            return [
                {
                    label: this.$t('queryData.numberOparateOptions.equal'),
                    value: 0,
                    inputType: 'number',
                    oparate: 'equal'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.notEqual'),
                    value: 1,
                    inputType: 'number',
                    oparate: 'noEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.bigger'),
                    value: 2,
                    inputType: 'number',
                    oparate: 'more'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.smaller'),
                    value: 3,
                    inputType: 'number',
                    oparate: 'less'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.biggerAndEqual'),
                    value: 4,
                    inputType: 'number',
                    oparate: 'moreEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.smallerAndEqual'),
                    value: 5,
                    inputType: 'number',
                    oparate: 'lessEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.inRange'),
                    value: 6,
                    inputType: 'numberRange',
                    oparate: 'range'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.outOfRange'),
                    value: 7,
                    inputType: 'numberRange',
                    oparate: 'outRange'
                },
                {
                    label: this.$t('queryData.analysisHeader.in'),    //用el-select，回车创建新label
                    value: 8,
                    inputType: 'multiString',
                    oparate: 'in'
                }
            ]
        },
        dateOparateOptions () {
            return [
                {
                    label: this.$t('queryData.numberOparateOptions.equal'),
                    value: 0,
                    inputType: 'dateTime',
                    oparate: 'equal'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.notEqual'),
                    value: 1,
                    inputType: 'dateTime',
                    oparate: 'noEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.bigger'),
                    value: 2,
                    inputType: 'dateTime',
                    oparate: 'more'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.smaller'),
                    value: 3,
                    inputType: 'dateTime',
                    oparate: 'less'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.biggerAndEqual'),
                    value: 4,
                    inputType: 'dateTime',
                    oparate: 'moreEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.smallerAndEqual'),
                    value: 5,
                    inputType: 'dateTime',
                    oparate: 'lessEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.inRange'),
                    value: 6,
                    inputType: 'dateRange',
                    oparate: 'range'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.outOfRange'),
                    value: 7,
                    inputType: 'dateRange',
                    oparate: 'outRange'
                }
            ]
        },
        dateNoNullOparateOptions () {
            return [
                {
                    label: this.$t('queryData.numberOparateOptions.equal'),
                    value: 0,
                    inputType: 'dateTime',
                    oparate: 'equal'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.notEqual'),
                    value: 1,
                    inputType: 'dateTime',
                    oparate: 'noEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.bigger'),
                    value: 2,
                    inputType: 'dateTime',
                    oparate: 'more'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.smaller'),
                    value: 3,
                    inputType: 'dateTime',
                    oparate: 'less'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.biggerAndEqual'),
                    value: 4,
                    inputType: 'dateTime',
                    oparate: 'moreEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.smallerAndEqual'),
                    value: 5,
                    inputType: 'dateTime',
                    oparate: 'lessEqual'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.inRange'),
                    value: 6,
                    inputType: 'dateRange',
                    oparate: 'range'
                },
                {
                    label: this.$t('queryData.numberOparateOptions.outOfRange'),
                    value: 7,
                    inputType: 'dateRange',
                    oparate: 'outRange'
                }
            ]
        },
        relationOptions () {
            return [
                {
                    label: this.$t('queryData.relationOptions.or'),
                    value: 0
                },
                {
                    label: this.$t('queryData.relationOptions.and'),
                    value: 1
                }
            ]
        }

    },
    watch: {
        '$i18n.locale' (val) {
            this.typeConnectOptions = [
                {
                    label: this.$t('queryData.typeConnectOptions.all'),
                    value: 0
                },
                {
                    label: this.$t('queryData.typeConnectOptions.any'),
                    value: 1
                },
                {
                    label: this.$t('queryData.typeConnectOptions.custom'),
                    value: 2
                }
            ]
        }
    },
    mounted () {
        this.$api.getTabels().then(res => {
            const { code, data, message } = res?.data || {}
            if (code == 200) {
                this.dataBaseOptions = data || [];
                this.dataBaseSelect = this.dataBaseOptions[0];
                this.seniorDataBaseSelect = this.dataBaseOptions[0];
                this.getColumns(this.dataBaseSelect.id);
                this.dataObject.id = this.dataBaseSelect.id;

            } else {
                this.$message.error(message);
            }
        }).catch(err => {
            this.$message.error(err?.data, err);
            console.log(err);
        })

    },
    methods: {
        popperSearchChange: debounce(function (queryString, cb) {
            let arr = []
            this.popperSearchOptions = []
            for (let index = 0, length = this.baseItemOptions.length; index < length; index++) {
                if (this.baseItemOptions[index].name.indexOf(queryString) > -1 && !this.editBaseCheckBox.includes(this.baseItemOptions[index].name)) {

                    arr.push({ value: this.baseItemOptions[index].name })
                }
            }
            cb(arr)
            console.log(arr);

        }, 500),

        handleSelect (item) {

            this.editBaseCheckBox.push(item.value)
            this.editBaseItemsChange(this.editBaseCheckBox)
        },
        openDefineDialog () {
            this.showDefineDialog = true;
        },
        closeDialog (dialog) {
            this[dialog] = false
        },
        formateColumn () {
            this.baseItemOptions.forEach((item, index) => {
                item.typeOption = {}
                if (item.type == 'string') {
                    item.typeOption = {
                        label: this.$t('queryData.analysisHeader.like'),
                        value: 0,
                        inputType: 'string',
                        oparate: 'like'
                    }

                } else if (item.type == 'number') {
                    item.typeOption = {
                        label: this.$t('queryData.numberOparateOptions.equal'),
                        value: 0,
                        inputType: 'number',
                        oparate: 'equal'
                    }
                } else if (item.type == 'dateTime') {
                    item.typeOption = {
                        label: this.$t('queryData.numberOparateOptions.equal'),
                        value: 0,
                        inputType: 'dateTime',
                        oparate: 'equal'
                    }
                } else {
                    item.typeOption = {
                        label: this.$t('queryData.boolOparationOptions.isOrNot'),
                        value: 1,
                        inputType: 'bool',
                        oparate: 'bool'
                    }
                }
                this.times[index] = ''
                this.timeRange[index] = ''
                item.inputValue1 = ''  // 用户输入绑定
                item.inputValue2 = ''; // 当用户选择的是范围查询时，需填写两个字段
                item.tags = [];  // 当是字符串时，使用in查询，需要显示的标签
                item.temUse = ''  // 用于临时使用
                item.link = 0   //用于自定义查询之间链接
            })
        },

        getColumns (id) {
            this.$api.getColumns({ id: id }).then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    this.baseItemOptions = []
                    this.baseItemOptions = data ? JSON.parse(data) : [];
                    this.formateColumn();
                    this.seniorQuery = JSON.parse(JSON.stringify(this.baseItemOptions));
                    this.editBaseItems = this.baseItemOptions.slice(0, 2);
                    this.editBaseCheckBox = []
                    this.editBaseItems.forEach((item, index) => {
                        this.editBaseCheckBox[index] = item.name
                    });
                    this.editSeniorQuery = this.seniorQuery.slice(0, 2);
                } else {
                    this.$message.error(message);
                }
            }).catch(err => {
                this.$message.error(err?.data, err);
            })
        },

        forceUpdate (val, type) {
            if (type == 'multi') {
                val.forEach((item, index) => {
                    if (item.trim() == '') {
                        val.splice(index, 1)
                    }
                })

            }
            this.$forceUpdate();
        },
        typeOptionChange (val, data, index) {
            data.inputValue1 = ''
            data.inputValue1 = ''
            data.tags = [];
            data.temUse = '';
            this.times[index] = '';
            this.timeRange[index] = '';
            this.forceUpdate();
        },
        toggleSelection (rows) {
            if (rows) {
                rows.forEach(row => {
                    this.$refs.multipleTable.toggleRowSelection(row);
                });
            } else {
                this.$refs.multipleTable.clearSelection();
            }
        },
        handleCurrentChange (val) {
            this.pagination.pageNo = val;
            this.getData();
        },

        handleCurrentChanges (val) {
            this.pagination.pageSize = val;
            this.getData();
        },

        dataBaseSelectChange (val) {
            this.dataObject.id = val.id;
            this.baseItemSelect = [];
            this.tableData = [];
            this.popperSearchContent = '';
            this.pagination.total = 0;
            this.pagination.pageNo = 1
            this.getColumns(val.id)
        },

        editBaseItemsChange: debounce(function (val, item) {
            let copyEditBaseItems = deepClone1(this.editBaseItems)
            let arrName = [];
            copyEditBaseItems.forEach(item => {
                arrName.push(item.name)
            })
            this.editBaseItems = [];
            let data;
            val.forEach(item => {

                let index = arrName.indexOf(item);
                if (index == -1) {
                    data = this.getEditBaseItem(item);
                    this.editBaseItems.push(deepClone1(data));
                } else {
                    data = copyEditBaseItems[index];
                    this.editBaseItems.push(deepClone1(data));
                }

            })
            copyEditBaseItems.forEach((item, index) => {
                if (item.name != val[index]) {
                    if (item.typeOption.inputType == 'dateTime') {
                        this.times[index] = ''
                    } else if (item.typeOption.inputType == 'dateRange') {
                        this.timeRange[index] = ''
                    }
                }
            })
        }, 600),

        editCheckboxChange (val, data) {
            data.inputValue1 = ''
            data.inputValue2 = ''
        },


        getEditBaseItem (name) {
            let data = {}
            for (let index = 0, length = this.baseItemOptions.length; index < length; index++) {
                if (this.baseItemOptions[index].name == name) {
                    data = this.baseItemOptions[index]
                }
            }
            return data;
        },
        openEditBaseCheckPopper () {
            this.showEditBaseCheckPopper = !this.showEditBaseCheckPopper;
        },
        equalOprate (optType, oparate, value, index) {
            if (optType == 'number') {
                if (oparate == 1) {
                    return value ? Number(value) : ''
                } else {
                    return { "$ne": value ? Number(value) : '' }
                }
            } else {
                if (oparate == 1) {
                    return (value)
                } else {
                    return { "$ne": value }
                }
            }

        },
        likeOparate (oparate, value) {
            if (oparate == 1) {
                return { "$like": value ? `%${value}%` : '' }
            } else {
                return { "$not LIKE": value ? `%${value}%` : '' }
            }
        },

        emptyOparate (oparate, value) {
            if (oparate == 1) {
                return ''
            } else {
                return { "$ne": '' }
            }
        },
        nullOparate (oparate, value) {
            if (oparate == 1) {
                return ''
            } else {
                return { "$ne": '' }
            }
        },

        inOparate (type, oparate, value1, value2) {
            if (oparate == 1) {
                return value1
            } else {
                return {
                    // $nin
                    '$nin': value1
                }
            }
        },

        moreAndLessOparate (type, oparate, value, index) {   // 大于 小于
            if (type == 'number') {
                if (oparate == 1) {
                    //   more
                    return {
                        '$gt': value ? Number(value) : ''
                    }
                } else {
                    // less
                    return {
                        '$lt': value ? Number(value) : ''
                    }
                }
            } else {
                //时间格式
                if (oparate == 1) {
                    //   more
                    return {
                        '$gt': value
                    }
                } else {
                    // less
                    return {
                        '$lt': value
                    }
                }
            }
        },

        moreAndLessEqualOparate (type, oparate, value, index) {   //大于等于，小于等于
            if (type == 'number') {
                if (oparate == 1) {
                    //   more
                    return {
                        '$gte': value ? Number(value) : ''
                    }
                } else {
                    // less
                    return {
                        '$lte': value ? Number(value) : ''
                    }
                }
            } else {
                //时间格式
                if (oparate == 1) {
                    //   more
                    return {
                        '$gte': value
                    }
                } else {
                    // less
                    return {
                        '$lte': value
                    }
                }
            }
        },

        rangeOparate (type, oparate, value1, value2, index) {   //范围之内
            if (type == 'number') {
                if (oparate == 1) {
                    return {
                        '$gte': value1 ? Number(value1) : '',
                        '$lte': value2 ? Number(value2) : ''
                    }
                } else {
                    return {
                        '$gte': value2 ? Number(value2) : '',
                        '$lte': value1 ? Number(value1) : ''
                    }
                }
            } else {
                let time = this.timeRange[index] ? this.timeRange[index] : ''
                //时间格式
                if (oparate == 1) {
                    //   more
                    return {
                        '$gte': time ? dayjs(time[0]).format('YYYY-MM-DD HH:mm:ss') : '',
                        '$lte': time ? dayjs(time[1]).format('YYYY-MM-DD HH:mm:ss') : ''
                    }
                } else {
                    // less
                    return {
                        '$gte': time ? dayjs(time[1]).format('YYYY-MM-DD HH:mm:ss') : '',
                        '$lte': time ? dayjs(time[0]).format('YYYY-MM-DD HH:mm:ss') : ''
                    }
                }
            }
        },

        boolOpatate (val) {
            return val
        },
        timeSelectChange (val, item, index) {
            if (val) {
                item.inputValue1 = dayjs(val).format('YYYY-MM-DD HH:mm:ss');
                this.forceUpdate()
            } else {
                item.inputValue1 = ''
                this.times[index] = ''
            }
        },
        dateRangeChange (val, item, index) {
            if (val) {
                item.inputValue1 = dayjs(val[0]).format('YYYY-MM-DD HH:mm:ss');
                item.inputValue2 = dayjs(val[1]).format('YYYY-MM-DD HH:mm:ss');
                this.forceUpdate()
            } else {
                item.inputValue1 = '';
                item.inputValue2 = '';
                this.timeRange[index] = ''

            }
        },
        searchData () {

            if (this.dataBaseOptions && this.dataBaseOptions.length == 0) {
                this.$message.warning(this.$t('queryData.warnings.noData'));
                return;
            }
            this.searchType = 0;
            this.dataObject.filters = {};
            let data = {};
            let link = ''
            let messageArr = []

            this.editBaseItems.forEach((item, index) => {
                if (item.inputValue1 == '' && typeof (item.inputValue1) == 'string') {
                    messageArr.push(item.name)
                }
                if (this.typeConnectSelect == 0) {
                    this.dataObject.filters[item.code] = ''
                    this.dataObject.filters[item.code] = this.switchData(item, index);
                } else if (this.typeConnectSelect == 1) {
                    if (!this.dataObject.filters['$or']) {
                        this.dataObject.filters['$or'] = {};
                    }
                    this.dataObject.filters['$or'][item.code] = this.switchData(item, index);
                } else {
                    if (this.editBaseItems.length > 1) {   //两个以上
                        if (index > 0) {
                            if (index == 1) {
                                if (item.link == 0) {
                                    data['$or'] = {};
                                    data['$or'][this.editBaseItems[index - 1].code] = ''
                                    data['$or'][this.editBaseItems[index - 1].code] = this.switchData(this.editBaseItems[index - 1], index - 1);
                                    data['$or'][this.editBaseItems[index].code] = ''
                                    data['$or'][this.editBaseItems[index].code] = this.switchData(this.editBaseItems[index], index);
                                    link = '$or'
                                } else {
                                    data['$and'] = {}
                                    data['$and'][this.editBaseItems[index - 1].code] = ''
                                    data['$and'][this.editBaseItems[index - 1].code] = this.switchData(this.editBaseItems[index - 1], index - 1);
                                    data['$and'][this.editBaseItems[index].code] = ''
                                    data['$and'][this.editBaseItems[index].code] = this.switchData(this.editBaseItems[index], index);
                                    link = '$and'
                                }

                            } else {

                                let dataItem = {}
                                if (item.link == 0) {
                                    let myLink = link
                                    link = '$or'
                                    if (data['$or']) {
                                        dataItem = deepClone1(data);
                                        dataItem['$or'][this.editBaseItems[index].code] = ''
                                        dataItem['$or'][this.editBaseItems[index].code] = this.switchData(this.editBaseItems[index], index);
                                    }
                                    else {
                                        dataItem['$or'] = {}
                                        dataItem['$or'][this.editBaseItems[index].code] = ''
                                        dataItem['$or'][this.editBaseItems[index].code] = this.switchData(this.editBaseItems[index], index);
                                        dataItem['$or'][myLink] = deepClone1(data['$and'])
                                    }

                                }
                                else {
                                    let myLink = link
                                    link = '$and'

                                    if (data['$and']) {
                                        dataItem = deepClone1(data);
                                        dataItem['$and'][this.editBaseItems[index].code] = ''
                                        dataItem['$and'][this.editBaseItems[index].code] = this.switchData(this.editBaseItems[index], index);
                                    } else {
                                        dataItem['$and'] = {}
                                        dataItem['$and'][this.editBaseItems[index].code] = ''
                                        dataItem['$and'][this.editBaseItems[index].code] = this.switchData(this.editBaseItems[index], index);
                                        dataItem['$and'][myLink] = deepClone1(data['$or'])
                                    }

                                }
                                data = deepClone1(dataItem)
                            }
                        }

                    } else {
                        this.dataObject.filters[item.code] = this.switchData(item);
                    }

                    this.dataObject.filters = deepClone1(data)
                }

            })
            if (messageArr.length > 0) {
                this.$message.warning(this.$t('queryData.tips1.pleaseInput') + messageArr[0]);
                messageArr = []
                return;
            }
            this.pagination.pageNo = 1;

            this.getData();
        },



        switchData (item, index) {
            switch (item.typeOption.oparate) {
                case 'equal':
                    return this.equalOprate(item.type, 1, item.inputValue1, index)
                case 'noEqual':
                    return this.equalOprate(item.type, 2, item.inputValue1, index)

                case 'more':
                    return this.moreAndLessOparate(item.type, 1, item.inputValue1, index)

                case 'less':
                    return this.moreAndLessOparate(item.type, 2, item.inputValue1, index)

                case 'moreEqual':
                    return this.moreAndLessEqualOparate(item.type, 1, item.inputValue1, index)

                case 'lessEqual':
                    return this.moreAndLessEqualOparate(item.type, 2, item.inputValue1, index)

                case 'empty':
                    return this.emptyOparate(1, item.inputValue1)

                case 'noEmpty':
                    return this.emptyOparate(2, item.inputValue1)

                case 'null':
                    return this.nullOparate(1, item.inputValue1)

                case 'noNull':
                    return this.nullOparate(2, item.inputValue1)

                case 'range':
                    return this.rangeOparate(item.type, 1, item.inputValue1, item.inputValue2, index)

                case 'outRange':
                    return this.rangeOparate(item.type, 2, item.inputValue1, item.inputValue2, index)

                case 'in':
                    return this.inOparate(item.type, 1, item.inputValue1)

                case 'noIn':
                    return this.inOparate(item.type, 2, item.inputValue1)

                case 'like':
                    return this.likeOparate(1, item.inputValue1)

                case 'noLike':
                    return this.likeOparate(2, item.inputValue1)
                case 'bool':
                    return this.boolOpatate(item.inputValue1)

                default:
                    return;
            }
        },
        getData () {
            if (this.baseItemSelect.length == 0) {
                this.$message.warning(this.$t('queryData.warnings.selectHeader'));
                return;
            }

            let data = deepClone1(this.dataObject);
            data.pageNo = this.pagination.pageNo;
            data.pageSize = this.pagination.pageSize
            this.pageLoading = true
            this.$api.getSearchResult(data).then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    if (data?.rows && data.rows instanceof Array) {
                        this.tableData = data.rows;
                        this.$nextTick(() => { //在数据加载完，重新渲染表格
                            this.$refs['table'].doLayout();
                        })
                        this.pagination.total = data?.totalCount || 0;
                        this.show = false;
                        this.tableHeaderTemUse = deepClone1(this.baseItemSelect);
                    } else {
                        this.tableHeaderTemUse = []
                        this.tableData = [];
                        this.pagination.pageNo = 1
                        this.pagination.total = 0;
                        this.$message.warning(this.$t('queryData.publics.noData'))
                    }

                } else {
                    this.$message.error(message);
                }
                this.pageLoading = false
            }).catch(err => {
                this.$message.error(err.data, err);
                this.pageLoading = false
                console.log(err);
            })
        },


        baseItemSelectChange (val) {
            this.dataObject.fields = []
            val.forEach(item => {
                this.dataObject.fields.push(item.code)
            })
        },
        seniorDaseItemSelectChange (val) {
            this.dataObject.fields = []
            val.forEach(item => {
                this.dataObject.fields.push(item.code)
            })
        },
        exportData () {
            if (this.tableData && this.tableData.length > 0) {
                this.exportLoading = true;
                this.$api.exportTableData(this.dataObject).then(res => {
                    if (res.status === 200) {
                        let url = window.URL.createObjectURL(new Blob([res.data]));
                        let link = document.createElement('a');
                        link.style.display = 'none';
                        link.href = url;
                        let excelName = this.dataBaseSelect.label;
                        link.setAttribute('download', excelName + '.xlsx');
                        document.body.appendChild(link);
                        link.click();
                    } else {
                        this.$message.error(this.$t('queryData.publics.tips.exportFail'), res);
                    }
                    this.exportLoading = false
                }).catch((err) => {
                    this.$message.error(err.data, err);
                    this.exportLoading = false
                })
            } else {
                this.$message.warning(this.$t('queryData.tips1.noData'));
            }
        },
        toBack () {
            this.show = true
        },
        mySelectChange (val, item, itemIndex) {
            let childIndex = item.tags.indexOf(val)

            if (childIndex != -1) {  //数据重复，提示
                this.existIndex = itemIndex.toString() + childIndex
                setTimeout(() => {
                    this.existIndex = '';
                    this.forceUpdate()
                }, 1000);
            } else {
                if (val.trim() != '') {
                    if (item.type == 'number') {
                        if (Number(val)) {
                            item.tags.push(Number(val))
                        }
                    } else {
                        item.tags.push(val)
                    }
                }
            }

            if (item.tags.length == 0) {
                let ref = 'myRefs' + itemIndex
                this.$refs[ref][0].$el.style.width = '106px';
            }

            item.inputValue1 = deepClone1(item.tags);

            this.temUse[itemIndex] = ''
            this.forceUpdate()
        },
        deleteChildItem (childIndex, item, index) {
            item.tags.splice(childIndex, 1);
            if (item.tags.length == 0) {
                let ref = 'myRefs' + index
                this.$refs[ref][0].$el.style.width = '106px';
            }
            this.forceUpdate()
        },
        focusOnMySelect (event, index) {
            let ref = 'myRefs' + index
            this.$refs[ref][0].focus();
        },
        mySelectInput (val, index) {
            let ref = 'myRefs' + index
            this.$refs[ref][0].$el.style.width = (val.length + 1) * 12 + 'px';
        }

    }
}