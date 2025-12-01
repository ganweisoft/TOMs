/**
 * @file eqEvent事件查询
 */
const myTree = () => import('gw-base-components-plus/treeV2');
import widthSetting from 'gw-base-components-plus/widthSetting/widthSetting.vue';
export default {
    components: { myTree, widthSetting },
    data () {
        return {
            equipNumber: 0,
            tableHeight: 0,
            tableHeightApp: 300,

            light: {},
            searchTime: [],
            eventType: 'E',
            eventTypeList: [
                {
                    type: 'E',
                    name: ('eqEvent.typeJs.equip')
                },
                {
                    type: 'C',
                    name: ('eqEvent.typeJs.yc')
                },
                {
                    type: 'X',
                    name: ('eqEvent.typeJs.yx')
                },
                {
                    type: 'S',
                    name: ('eqEvent.typeJs.setting')
                }],
            tbLoad: false,
            eqData: [],
            evtSum: 0,
            nodeLoad: false,
            beginTime: this.$moment()
                .startOf('month')
                .format('YYYY-MM-DD HH:mm:ss'),
            endTime: this.$moment()
                .endOf('month')
                .format('YYYY-MM-DD HH:mm:ss'),
            scout: '',
            eqSum: 0,
            defaultProps: {
                children: 'children',
                label: 'label'
            },
            recordPage: {
                pageSize: 100,
                pageNo: 1,
                total: 0
            },
            eqPage: {
                pageNo: 1,
                pageSize: 25,
                total: 0
            },
            tableData: [],
            allStatus: {},
            eqChoice: 'all',
            isScout: false,
            equipList: [],
            searchArray: [],
            eventName: '',


            // app
            drawerAppHeader: false,
            mainAppShow: false,
            appStartTimeValue: '',
            appEndTimeValue: '',
            searchEquipName: '',
            disabled: true
        }
    },
    computed: {
        pickerOptions () {
            return {
                shortcuts: [{
                    text: this.$t('eqEvent.tips.recentWeek'),
                    onClick (picker) {
                        const end = new Date();
                        const start = new Date();
                        start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
                        picker.$emit('pick', [start, end]);
                    }
                }, {
                    text: this.$t('eqEvent.tips.recentMonth'),
                    onClick (picker) {
                        const end = new Date();
                        const start = new Date();
                        start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
                        picker.$emit('pick', [start, end]);
                    }
                }, {
                    text: this.$t('eqEvent.tips.recentThreeMonth'),
                    onClick (picker) {
                        const end = new Date();
                        const start = new Date();
                        start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
                        picker.$emit('pick', [start, end]);
                    }
                }]
            }
        },
    },
    watch: {
        scout (val, oldValue) {
            if (!val) {
                // this.searchEquipName = val
                this.$refs.myTree.filterMethod()
                // this.scoutEq(null,true)
            }
        },
        '$i18n.locale' (val) {
            this.pickerOptions = {
                shortcuts: [{
                    text: this.$t('eqEvent.tips.recentWeek'),
                    onClick (picker) {
                        const end = new Date();
                        const start = new Date();
                        start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
                        picker.$emit('pick', [start, end]);
                    }
                }, {
                    text: this.$t('eqEvent.tips.recentMonth'),
                    onClick (picker) {
                        const end = new Date();
                        const start = new Date();
                        start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
                        picker.$emit('pick', [start, end]);
                    }
                }, {
                    text: this.$t('eqEvent.tips.recentThreeMonth'),
                    onClick (picker) {
                        const end = new Date();
                        const start = new Date();
                        start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
                        picker.$emit('pick', [start, end]);
                    }
                }]
            }
        }
    },
    mounted () {
        this.beginTime = this.myUtils.getCurrentDate(1, '-') + ' 00:00:00';
        this.endTime = this.myUtils.getCurrentDate(1, '-') + ' 23:59:59';
        this.searchTime.push(this.myUtils.getCurrentDate(1, '-') + ' 00:00:00');
        this.searchTime.push(this.myUtils.getCurrentDate(1, '-') + ' 23:59:59');

        this.appStartTimeValue = this.myUtils.getCurrentDate(1, '-') + ' 00:00:00';
        this.appEndTimeValue = this.myUtils.getCurrentDate(1, '-') + ' 23:59:59';

        // 表格自适应
        let thisOne = this;
        let eqTable = document.getElementById('eqTable');
        let eqTableApp = document.getElementById('eqTableAppHeight');
        window.onresize = function windowResize () {
            eqTable = document.getElementById('eqTable');
            eqTableApp = document.getElementById('eqTableAppHeight');
            if (eqTable) {
                thisOne.tableHeight = eqTable.offsetHeight;
            }
            if (eqTableApp) {
                thisOne.tableHeightApp = eqTableApp.offsetHeight - 15;
            }
        };
    },
    updated () {

        // 表格自适应
        let eqTable = document.getElementById('eqTable');
        this.tableHeight = eqTable.offsetHeight - 15;
    },
    methods: {
        getTotal (equipNumber) {
            this.equipNumber = equipNumber
        },
        loadAll () {
            this.disabled = false
        },

        // app
        getBack () {
            this.mainAppShow = false;
        },
        onQuery () {
            this.mainAppShow = true;
            if (this.getSelected().length == 0) {
                this.$message({
                    title: this.$t('eqEvent.tips.selectEquip'),
                    type: 'warning'
                });
                return;
            }
            this.drawerAppHeader = true;
        },
        closeDrawerAppHeader () {
            this.drawerAppHeader = false;
        },
        appStartTime () {
            this.searchTime[0] = this.appStartTimeValue;
            this.beginTime = this.appStartTimeValue;
        },
        appEndTime () {
            this.endTime = this.appEndTimeValue;
            this.searchTime[1] = this.appEndTimeValue;
        },

        // 选择事件类型
        changeTypeList (val) {
            this.eventType = val;
            this.recordPage.pageNo = 1;
            this.recordPage.total = 0;
            this.getList(true);
        },

        // 选择时间跨度
        checkTime (searchTime) {
            if (searchTime === null) {
                this.beginTime = '';
                this.endTime = '';
            } else if (searchTime.length === 2) {
                this.beginTime = this.$moment(searchTime[0]).format('YYYY-MM-DD HH:mm:ss');
                this.endTime = this.$moment(searchTime[1]).format('YYYY-MM-DD HH:mm:ss');
                this.appStartTimeValue = this.$moment(searchTime[0]).format('YYYY-MM-DD HH:mm:ss');
                this.appEndTimeValue = this.$moment(searchTime[1]).format('YYYY-MM-DD HH:mm:ss');
            }
        },

        // 搜索设备
        scoutEq () {
            this.scout = this.scout.trim()
            this.$refs.myTree.filterMethod(this.scout)
            // this.searchEquipName = this.scout
        },

        getSelected () {
            return this.$refs.myTree.getEquipSelectd();
        },

        // 获取设备事件列表
        getList (isButton) {
            this.eqChoice = '';
            let equips = this.getSelected();

            if (equips.length == 0) {
                return this.$message.error(this.$t('eqEvent.tips.selectEquip'));
            }

            this.eqChoice = equips;
            let requestData = {
                pageNo: this.recordPage.pageNo,
                pageSize: this.recordPage.pageSize,
                beginTime: this.$moment(this.beginTime).format('YYYY-MM-DD HH:mm:ss'),
                endTime: this.$moment(this.endTime).format('YYYY-MM-DD HH:mm:ss'),
                equipNos: this.eqChoice.join(','),
                eventType: this.eventType,
                sort: 'desc',
                eventName: this.eventName
            };
            this.tbLoad = true;
            this.tableData = [];
            this.evtSum = 0;

            if (isButton) {
                this.getEventList(requestData);
            } else {
                this.getEventList(requestData);
            }
        },

        getEventList (data) {
            if (!window.executeQueues) {
                window.executeQueues = {}
            }
            window.executeQueues.deleteQueryCache = () => {
                let delelteData = {
                    beginTime: data.beginTime,
                    endTime: data.endTime,
                    equipNos: data.equipNos,
                    eventType: data.eventType
                }
                this.$api.deleteQueryCache(delelteData).then(res => {
                   // console.log(res)
                }).catch(err => {
                    console.log(err)
                })
            }
            this.tbLoad = true;
            this.$api
                .evtList(data)
                .then((res) => {
                    const { code, data, message } = res?.data || {}
                    if (code !== 200) {
                        this.$message.error(message);
                        return;
                    }
                    let evtList = data?.rows || [];
                    this.recordPage.total = data?.total || 0;
                    this.evtSum = data?.total || 0;
                    if (evtList && evtList.length > 0) {
                        evtList.forEach((item) => {
                            item.event = item.event || '-';
                            item.location = '-';
                            item.relatedVideo = item.relatedVideo || '-';
                            item.ziChanName = '-';
                            item.confirmName = item.confirmName || '-';
                            item.confirmRemark = item.confirmRemark || '-';
                            item.planNo = item.planNo || '-';
                            if (item.confirmTime == '1970-01-01T00:00:00') {
                                item.confirmTime = '-';
                            } else {
                                item.confirmTime = item.confirmTime ? this.$moment(item.confirmTime).format('YYYY-MM-DD HH:mm:ss') : '-';
                            }
                        })
                    }
                    this.tableData = evtList;
                })
                .catch((err) => {
                    this.$message.error(err?.data, err);
                }).finally(() => {
                    this.tbLoad = false;
                });
        },

        // 根据日期查找设备事件
        searchEvt (type) {
            this.drawerAppHeader = false;
            this.recordPage.pageNo = 1;
            this.recordPage.total = 0;
            if (type) {
                this.mainAppShow = false;
            } else {
                this.mainAppShow = true;
                let eqTableApp = document.getElementById('eqTableAppHeight');
                this.tableHeightApp = eqTableApp.offsetHeight - 50;
            }
            this.getList(true);
        },


        // 页码大小改变时触发事件
        handleSizeChange2 (pageSize) {
            this.recordPage.pageSize = pageSize;
            this.recordPage.pageNo = 1;
            this.getList();
        },

        // 当前页码改变时触发
        handleCurrentChange2 (pageNo) {
            this.recordPage.pageNo = pageNo;
            this.getList();
        },

        // 父菜单点击事件
        showToggle (dt) {
            let that = $(dt);
            if (that.parent().hasClass('active')) {
                that.parent().removeClass('active');
            } else {
                that.parent().addClass('active');
            }
        }
    }
};
