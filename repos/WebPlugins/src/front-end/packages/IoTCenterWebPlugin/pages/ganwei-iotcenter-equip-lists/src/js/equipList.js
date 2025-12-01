const myTree = () => import('gw-base-components-plus/treeV2');
import yxyxList from '../components/yxyxList/index.vue';
import exportHistory from 'gw-base-utils-plus/historyExport';
// import gwTab from '../components/gw-tab/gw-tab.vue';
const gwTab = () => import('../components/gw-tab/gw-tab.vue')
import ycTable from '../components/table/ycTable/ycTable.vue';
import yxTable from '../components/table/yxTable/yxTable.vue';
import setList from '../components/table/setList/setList.vue';
import historyCurveDialog from '../components/dialog/historyCurve/historyCurve.vue';
import realtimeCurveDialog from '../components/dialog/realtimeCurve/realtimeCurve.vue';
import exportEquipDialog from '../components/dialog/exportEquip/exportEquip.vue';
import exportCurveDialog from '../components/equipSelects/equipSelect.vue';
import exportTypeSelectDialog from '../components/dialog/selectExportType/selectExportType.vue';
import widthSetting from 'gw-base-components-plus/widthSetting/widthSetting.vue';
import Signalr from 'gw-base-components-plus/equipProcessing/gwSignalr.js';
let exportCurveData = {}

export default {
    mixins: [exportHistory],
    components: {
        exportCurveDialog,
        myTree,
        yxyxList,
        gwTab,
        ycTable,
        yxTable,
        setList,
        realtimeCurveDialog,
        historyCurveDialog,
        exportEquipDialog,
        exportTypeSelectDialog,
        widthSetting
    },
    data () {
        return {
            defaultProps: {
                children: 'children',
                label: 'title'
            },
            signalrConnection: null,
            exportSignalrConnection: null,
            signalrConnectionEquip: null,

            equipNumber: 0, // 设备总数

            // 搜索输入框内容
            searchEquipSideCon: '',
            mainSearchCon: '',

            // 选中的设备名\设备状态
            equipConState: '',

            // 表格数据
            tableDataYc: [],
            tableDataYx: [],
            tableDataSet: [],

            // 当前设备
            nowEquipStaNo: 1,
            nowEquipNo: '',

            // 记录当前tab
            tabNavType: 1, // [遥测量:1],[遥信量:2],[设置:3]

            loading: false,

            // 分页-右侧设备列表
            pageSizeRight: 20,
            pageNoRight: 1, // 初始化第一页
            totalRight: 0,

            // 实时值参数
            equipDataYc: [],
            equipDataYx: [],
            ycData: [],
            yxData: [],

            // 测点数量
            stationNumber: {
                setNum: 0,
                ycNum: 0,
                yxNum: 0
            },

            // 历史曲线数组
            equipHistory: [],
            loadingHistory: false,

            equipNoList: [],

            nodeLoad: false,

            // 定时器
            clearTimeoutVariable: null,
            firstEquipFlag: true, // 首次开启数据推送

            exportLoading: false,
            exportCurveLoading: false,
            showExportCurveDialog: false,
            showExportTypeSelectDialog: false,

            // 曲线 设备参数
            showYcyxListDialog: false,
            searchActiveTabName: 'equipListsIot.input.searchYc',

            // 判断是否存在外框
            existIframe: false,

            // 颜色配置，在json.config中获取
            colorConfig: {
                "noComm": "#a0a0a0",
                "normal": "#63e03f",
                "alarm": "#f22433",
                "lsSet": "#bebcaa",
                "initialize": "#289ac0",
                "withdraw": "#ffc0cb",
                "BackUp": "#f8901c"
            },

            // 实时数据数组
            realTimeData: [],

            // 当前测点值
            curentValue: null,

            // 定时器，定时获取当前测点值
            realTimeValueInterval: '',

            ycTabName: 'equipListsIot.tabs.ycNm',
            yxTabName: 'equipListsIot.tabs.yxNm',
            setTabName: 'equipListsIot.tabs.setNm',

            // 展示实时曲线弹窗
            showRealtimeCurveDialog: false,

            // 当前选中（包含设备、遥测遥信等信息）
            currentSelect: {},

            // 展示历史数据图表弹窗
            showHistoryDialog: false,

            // 设备导出
            showExportEquipDialog: false,
            signalR: null,
            tableDataYcObj: {},
            tableDataYxObj: {},
            theme: localStorage.getItem('theme')
        };
    },
    provide () {
        return {
            getColor: this.getColor
        };
    },
    watch: {
        searchEquipSideCon (newVal) {
            if (!newVal) {
                this.$refs.mytree.filterMethod(newVal)
            }
        },
        mainSearchCon (newVal) {
            if (!newVal && this.nowEquipNo) {
                this.equipYcAndYxAndSet(this.nowEquipStaNo, this.nowEquipNo);
            }
        }
    },
    computed: {
        tabNames () {
            return () => {
                let ycName = `${this.$t(this.ycTabName)}(${this.stationNumber.ycNum
                    })`;
                let yxName = `${this.$t(this.yxTabName)}(${this.stationNumber.yxNum
                    })`;
                let setName = `${this.$t(this.setTabName)}(${this.stationNumber.setNum
                    })`;
                return [ycName, yxName, setName];
            }
        },

        returnStatus () {
            return status => {
                let className = this.getColor('dotNormal');
                switch (parseInt(status)) {
                    case 0:
                        className = this.getColor('dotNoComm');
                        break;
                    case 1:
                        className = this.getColor('dotNormal');

                        //  'dotNormal'
                        break;
                    case 2:
                        className = this.getColor('dotAlarm');
                        break;
                    case 3:
                        className = this.getColor('dotLsSet');
                        break;
                    case 4:
                        className = this.getColor('dotInitialize');
                        break;
                    case 5:
                        className = this.getColor('dotWithdraw');
                        break;
                    case 6:
                        className = this.getColor('dotBackUp');
                        break;
                    default:
                        className = this.getColor('dotNoComm');
                        break;
                }
                return className;
            };
        }
    },

    async created () {
        await this.myUtils.configInfoData(this).then(webConfig => {
            this.colorConfig = webConfig.equipStatus || ''
        })
    },

    mounted () {
        let that = this;
        window.onmessage = function (e) {
            if (e.data.getExportCommand) {
                that.existIframe = true;
            }
            if (e.data.exportYes) {
                that.exportCurveLoading = false;
            }
            if (e.data && (e.data.theme)) {
                that.theme = e.data.theme
            }
        };
        this.signalR = new Signalr('/pointStatusMonitor', '', '');
        this.signalR.openConnect().then(rt => {
            this.signalrConnection = rt
        })
    },

    // 销毁前
    beforeDestroy () {
        this.clearTimer();
        if (this.signalrConnection) {
            this.signalrConnection.stop();
        }
        if (this.signalrConnectionEquip) {
            this.signalrConnectionEquip.stop();
        }
    },

    methods: {

        getTotal (equipNumber) {
            this.equipNumber = equipNumber
        },

        getColor (type) {
            let color = '';
            if (type) {
                for (const item in this.colorConfig) {
                    if (type.toLowerCase().includes(item.toLowerCase())) {
                        color = this.colorConfig[item];
                        break;
                    }
                }
            }
            return color;
        },

        // type(类型：遥测或遥信)，title（弹窗标题展示）,ycyxNum(遥测遥信号),showTimeSelect(是否展示时间选择)
        showYcYxList (type, title, itemData, showTimeSelect) {
            this.showTimeSelect = showTimeSelect;
            this.setYcYxInfo(itemData);
            this.currentSelect.ycyxType = type;
            this.currentSelect.ycyxCultureName = title;

            this.showYcyxListDialog = true;
            if (!this.showTimeSelect) {
                this.getRealTimeData(true);
            }
        },

        // 获取实时数据
        getRealTimeData (noCurve) {
            if (this.realTimeValueInterval) {
                clearInterval(this.realTimeValueInterval);
                this.realTimeValueInterval = null;
            }
            this.realTimeData = [];

            // 第一遍从遥测或遥信table数据中获取
            this.realTimeData.push({
                time: this.$moment(new Date()).format('HH:mm:ss.SSS'),
                value: this.getEquipCurve(this.currentSelect.ycyxNo),
                valMax: this.currentSelect.ycyxMax,
                valMin: this.currentSelect.ycyxMin
            });

            this.realTimeValueInterval = setInterval(() => {
                if (!noCurve) {
                    this.showRealtimeCurveDialog = true;
                }

                // 当存在数据类型时，展示实时曲线，否则展示实时列表（倒序刷新）
                let curentValue = this.getEquipCurve(this.currentSelect.ycyxNo)
                if (!this.currentSelect.ycyxDataType) {
                    this.realTimeData.push({
                        time: this.$moment(new Date()).format(
                            'HH:mm:ss.SSS'
                        ),
                        value: curentValue,
                        valMax: this.currentSelect.ycyxMax,
                        valMin: this.currentSelect.ycyxMin
                    });
                    if (this.realTimeData.length > 20) {
                        this.realTimeData.splice(0, 1);
                    }
                } else {
                    this.realTimeData.unshift({
                        time: this.$moment(new Date()).format(
                            'HH:mm:ss.SSS'
                        ),
                        value: curentValue,
                        valMax: this.currentSelect.ycyxMax,
                        valMin: this.currentSelect.ycyxMin
                    });
                    if (this.realTimeData.length > 100) {
                        this.realTimeData.splice(100, 200);
                    }
                }
            }, 100);
        },

        // 展示实时曲线:type(String):类型（C,X）,item(Object):获取到的遥测或遥信对象
        showRealTimeCurve (type, item) {
            if (
                item.isAlarm === 'dotNoComm' ||
                item.equipState === 'dotNoComm'
            ) {
                this.$message({
                    title: this.$t('equipListsIot.tips.curveValue'),
                    type: 'warning'
                });
                return;
            }
            this.setYcYxInfo(item);
            this.currentSelect.ycyxType = type;

            this.getRealTimeData();

        },

        // 展示历史曲线:type(String):类型（yc,yx）,item(Object):获取到的遥测或遥信对象
        showHistoryCurve (type, item) {
            this.setYcYxInfo(item);
            this.currentSelect.ycyxType = type;
            this.showHistoryDialog = true;
        },

        // 保存当前选中的遥测遥信数据
        setYcYxInfo (item) {
            this.currentSelect.ycyxName = item.ycNm ? item.ycNm : item.yxNm;
            this.currentSelect.ycyxNo = item.ycNo ? item.ycNo : item.yxNo;
            this.currentSelect.ycyxMin = item.valMin;
            this.currentSelect.ycyxMax = item.valMax;
            this.currentSelect.ycyxValue = item.value;
            this.currentSelect.ycyxDataType = item.dataType;
        },

        // 清除保存的数据
        clearYcYxInfo () {
            this.currentSelect.ycyxName = '';
            this.currentSelect.ycyxNo = '';
            this.currentSelect.ycyxMin = '';
            this.currentSelect.ycyxMax = '';
            this.currentSelect.ycyxValue = '';
            this.currentSelect.ycyxDataType = '';
            this.curentValue = null;
        },

        // 关闭弹窗
        closeDialog (dialog) {
            if (dialog) {
                this[dialog] = false;
            }
            this.clearYcYxInfo();
            this.showYcyxListDialog = false;
            clearInterval(this.realTimeValueInterval);
            this.realTimeValueInterval = null;
        },

        exportCurve () {
            this.showExportCurveDialog = true;
        },
        confirmExportCurve (isMerge) {
            this.exportCurveLoading = true;
            delete exportCurveData.equipsList;
            exportCurveData.isMerge = isMerge;
            window.top.exportData = exportCurveData
            window.parent.postMessage({ openCurveLink: true },
                '*'
            );
            setTimeout(() => {
                if (!this.existIframe) {
                    this.curveSignalR(
                        {
                            data: {
                                openCurveLink: true
                            }
                        },
                        false
                    );
                }
            }, 500);
            this.showExportTypeSelectDialog = false;
        },

        toExportCurve (data) {
            if (data) {
                if (!data.beginTime) {
                    this.$message.warning(
                        this.$t(
                            'equipListsIot.publics.warnings.selectStartTime'
                        )
                    );
                    return;
                }
                if (!data.endTime) {
                    this.$message.warning(
                        this.$t('equipListsIot.publics.warnings.selectEndTime')
                    );
                    return;
                }
                if (new Date(data.beginTime) > new Date(data.endTime)) {
                    this.$message.warning(
                        this.$t(
                            'equipListsIot.publics.warnings.STTimeCantGreaterEndTime'
                        )
                    );
                    return;
                }
                if (
                    parseInt(
                        Math.abs(
                            new Date(data.beginTime).getTime() -
                            new Date(data.endTime).getTime()
                        ) /
                        1000 /
                        60 /
                        60 /
                        24
                    ) > 90
                ) {
                    this.$message.warning(
                        this.$t(
                            'equipListsIot.publics.warnings.timeCantMoreThanNinetyDay'
                        )
                    );
                    return;
                }
                if (data.equipsList && data.equipsList.length > 0) {
                    if (data.exportEquips && data.exportEquips.length == 0) {
                        this.$message.warning(
                            this.$t('equipListsIot.tips.lessOnePoint')
                        );
                        return;
                    }
                } else {
                    this.$message.warning(
                        this.$t('equipListsIot.tips.selectDevice')
                    );
                    return;
                }
                exportCurveData = data;
                this.showExportTypeSelectDialog = true;
            } else {
                this.showExportCurveDialog = false;
                this.exportCurveLoading = false;
                window.top.exportData = {}
                exportCurveData = {}
            }
        },
        exportEquip () {
            this.showExportEquipDialog = true;
        },

        setRealTimeValue (ycyxNo, curentValue) {
            if (this.showRealtimeCurveDialog && this.currentSelect.ycyxNo == ycyxNo) {
                if (!this.currentSelect.ycyxDataType) {
                    this.realTimeData.push({
                        time: this.$moment(new Date()).format(
                            'YYYY-MM-DD HH:mm:ss'
                        ),
                        value: curentValue,
                        valMax: this.currentSelect.ycyxMax,
                        valMin: this.currentSelect.ycyxMin
                    });
                    if (this.realTimeData.length > 100) {
                        this.realTimeData.splice(0, 1);
                    }
                } else {
                    this.realTimeData.unshift({
                        time: this.$moment(new Date()).format(
                            'YYYY-MM-DD HH:mm:ss'
                        ),
                        value: curentValue,
                        valMax: this.currentSelect.ycyxMax,
                        valMin: this.currentSelect.ycyxMin
                    });
                    if (this.realTimeData.length > 100) {
                        this.realTimeData.splice(100, 200);
                    }
                }
            }
        },

        // 测点数据推送
        connectHub (equipNo) {
            if (this.signalrConnection) {
                this.signalrConnection.invoke('ConnectEquipNo', equipNo);
                // 开始连接this.signalrConnectionr
                this.signalrConnection.off('ycStatusChange');
                this.signalrConnection.on('ycStatusChange', res => {
                    if (res && res.isSuccess) {
                        let data = res.data || []
                        data.forEach((item) => {
                            if (
                                this.tableDataYcObj[item.ycNo]
                            ) {
                                this.tableDataYcObj[item.ycNo].value = item.value;
                                this.tableDataYcObj[item.ycNo].state = item.isAlarm;
                                // 设备离线或者实时值不是number类型都为离线置灰色equipState
                                if (!this.equipConState) {
                                    this.tableDataYcObj[item.ycNo].isAlarm = 'dotNoComm';
                                    this.tableDataYcObj[item.ycNo].txIsAlarm = 'noComm';
                                    this.tableDataYcObj[item.ycNo].equipState = 'dotNoComm';
                                } else {
                                    this.tableDataYcObj[item.ycNo].isAlarm = item.isAlarm
                                        ? 'dotAlarm'
                                        : 'dotNormal';
                                    this.tableDataYcObj[item.ycNo].txIsAlarm = item.isAlarm
                                        ? 'alarm'
                                        : 'normal';
                                    this.tableDataYcObj[item.ycNo].equipState = item.isAlarm
                                        ? 'dotAlarm'
                                        : 'dotNormal';
                                }
                            }
                            // this.setRealTimeValue(item.ycNo, item.value)
                        });
                    }
                });

                this.signalrConnection.off('yxStatusChange');
                this.signalrConnection.on('yxStatusChange', res => {
                    if (res && res.isSuccess) {
                        let data = res.data || []
                        data.forEach((item) => {
                            if (
                                this.tableDataYxObj[item.yxNo]
                            ) {
                                this.tableDataYxObj[item.yxNo].value = item.value;
                                this.tableDataYxObj[item.yxNo].state = item.isAlarm;
                                if (!this.equipConState) {
                                    this.tableDataYxObj[item.yxNo].isAlarm = 'dotNoComm';
                                    this.tableDataYxObj[item.yxNo].txIsAlarm = 'noComm';
                                    this.tableDataYxObj[item.yxNo].equipState = 'dotNoComm';
                                } else {
                                    this.tableDataYxObj[item.yxNo].isAlarm = item.isAlarm
                                        ? 'dotAlarm'
                                        : 'dotNormal';
                                    this.tableDataYxObj[item.yxNo].txIsAlarm = item.isAlarm
                                        ? 'alarm'
                                        : 'normal';
                                    this.tableDataYxObj[item.yxNo].equipState = item.isAlarm
                                        ? 'dotAlarm'
                                        : 'dotNormal';
                                }
                            }
                            // this.setRealTimeValue(item.ycNo, item.value)
                        });

                    }
                });
                this.signalrConnection.onclose(() => {
                    this.signalR.openConnect().then(rt => {
                        this.signalrConnection = rt
                        this.connectHub(this.currentSelect.equipNo)
                    })
                })
            }
        },


        // 右侧---页码大小改变时触发事件
        handleSizeChangeRight (pageSize) {
            this.$refs.pagination.internalCurrentPage = 1;
            this.pageSizeRight = pageSize;
            this.pageNoRight = 1;
            this.equipYcAndYxAndSet(this.nowEquipStaNo, this.nowEquipNo);
        },

        // 右侧---当前页码改变时触发
        handleCurrentChangeRight (pageNo) {
            this.pageNoRight = pageNo;
            this.equipYcAndYxAndSet(this.nowEquipStaNo, this.nowEquipNo);
        },

        // 获取当前设备遥测/遥信/设置---列表
        equipYcAndYxAndSet (staNo, equipNo) {
            if (equipNo == '') {
                this.$message.warning(
                    this.$t('equipListsIot.tips.selectDevice')
                );
                return;
            }
            this.tableDataYc = [];
            this.tableDataYx = [];
            this.tableDataSet = [];
            this.tableDataYcObj = {}
            this.tableDataYxObj = {}


            //	this.tabNavType【1：遥测、2：遥信、3：设置】
            this.loading = true;
            let data = {
                staN: staNo || 1,
                equipNo: equipNo,
                pageNo: this.pageNoRight,
                pageSize: this.pageSizeRight,
                searchName: this.mainSearchCon
            };
            if (this.tabNavType === 1) {
                this.getEquipRealTimeData(0, data);
            } else if (this.tabNavType === 2) {
                this.getEquipRealTimeData(1, data);
            } else if (this.tabNavType === 3) {
                this.$api
                    .getEquipControlTable(data)
                    .then(res => {
                        const { code, data, message } = res?.data || {}
                        if (code === 200) {
                            this.tableDataSet = data?.rows || [];
                            this.totalRight = data?.total || 0;
                        } else {
                            this.$message.error(message)
                        }
                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err)
                    }).finally(() => {
                        this.loading = false;
                    });
            }
        },


        getEquipRealTimeData (typeNo, paremeter) {
            let url = typeNo == 0 ? 'getYcpByEquipNo' : 'getYxpByEquipNo'
            let table = typeNo == 0 ? 'tableDataYc' : 'tableDataYx'
            this.$api[url](paremeter).then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    this[table] = data?.rows || []
                    let equipConState = this.equipConState;
                    this[table].forEach(item => {
                        if (!equipConState) {
                            item.isAlarm =
                                'dotNoComm';
                            item.txIsAlarm =
                                'noComm';
                        } else if (equipConState == 6) {
                            item.isAlarm =
                                'dotBackUp';
                            item.txIsAlarm =
                                'BackUp';
                        } else {
                            item.isAlarm = item.state
                                ? 'dotAlarm'
                                : 'dotNormal';
                            item.txIsAlarm = item.state
                                ? 'alarm'
                                : 'normal';
                        }
                    })
                    this.totalRight = data?.total || 0;
                    this.formatObject(table)
                } else {
                    this.$message.error(message)
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            }).finally(() => {
                this.loading = false;
            });
        },

        formatObject (table) {
            let obj = `${table}Obj`
            this[table].forEach(item => {
                if (table.toLowerCase().includes('yc')) {
                    this[obj][item.ycNo] = item
                } else {
                    this[obj][item.yxNo] = item
                }
            })
        },

        // 曲线图-返回当前遥测遥信实时值
        getEquipCurve (no) {
            let equipCurve = [];
            let num = 0;

            // 遥测实时值
            if (this.currentSelect.ycyxType == 'C') {
                equipCurve = this.tableDataYc;
                equipCurve.forEach((item, i) => {
                    if (no == item.ycNo) {
                        if (!this.currentSelect.ycyxDataType) {
                            num = item.value;
                        } else {
                            num = JSON.stringify(item.value).replaceAll('\"', "").replaceAll('\'', "");
                        }
                    }
                });
            } else {

                // 遥信实时值【0、1】
                equipCurve = this.tableDataYx;
                equipCurve.forEach((item, i) => {
                    if (no === item.yxNo) {
                        if (!this.currentSelect.ycyxDataType) {
                            num = item.isAlarm.toLowerCase().includes('alarm') ? 0 : 1
                        } else {
                            num = JSON.stringify(item.value)
                        }
                    }
                });
            }

            return num;
        },

        // tabNab切换
        onTabNav (no) {
            this.loading = false;
            this.mainSearchCon = '';
            this.tabNavType = no;
            this.$refs.pagination.internalCurrentPage = 1;
            this.pageNoRight = 1;
            this.tableDataYcObj = {}
            this.tableDataYxObj = {}
            this.equipYcAndYxAndSet(this.nowEquipStaNo, this.nowEquipNo);

            if (no === 1) {
                this.searchActiveTabName = 'equipListsIot.input.searchYc';
            } else if (no === 2) {
                this.searchActiveTabName = 'equipListsIot.input.searchYx';
            } else {
                this.searchActiveTabName = 'equipListsIot.input.searchSet';
            }
        },

        // 获取当前设备的测点数量
        getStationNumber (equipNo) {
            this.$api.getYcYxSetNumByEquipNo(equipNo).then(res => {
                const { code, data, message } = res?.data || {}
                if (code === 200) {
                    this.stationNumber = data || '';
                } else {
                    this.$message.error(message)
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            });
        },

        currentDelete () {
            this.tableDataYc = []
            this.tableDataYx = []
            this.tableDataSet = []
            this.totalRight = 0
            this.pageNoRight = 1
            this.stationNumber.ycNum = 0
            this.stationNumber.yxNum = 0
            this.stationNumber.setNum = 0
            this.currentSelect.equipName = ''
            this.currentSelect.equipNo = ''
        },

        // 子菜单点击事件
        getItem (data) {
            if (!data.isGroup) {
                const { key, status, staNo, title } = data;

                // this.tabNavType = 1
                this.pageNoRight = 1;
                this.loading = true;

                this.mainSearchCon = '';
                this.$refs.pagination.internalCurrentPage = 1;

                if (key) {
                    this.connectHub(key);
                }
                this.nowEquipStaNo = staNo;
                this.nowEquipNo = key;

                // 保存当前选中的设备信息
                this.currentSelect.equipNo = key;
                this.currentSelect.equipName = title;
                this.currentSelect.staNo = staNo || 1;

                this.equipConState = status || 0; // 选中设备状态

                this.tableDataYcObj = {}
                this.tableDataYxObj = {}
                this.equipYcAndYxAndSet(staNo, key);

                this.getStationNumber(key);
            }
        },

        statusChange (equipNo, status) {
            if (Number(equipNo) == Number(this.currentSelect.equipNo)) {
                this.equipConState = status;
            }
        },


        // 左侧设备搜索---回车事件
        onSearchSide () {
            this.searchEquipSideCon = this.searchEquipSideCon.trim()
            this.$refs.mytree.filterMethod(this.searchEquipSideCon)
        },

        // 遥信遥测回车
        onSearchHeader () {
            if (this.mainSearchCon) {
                this.pageNoRight = 1;
                this.$refs.pagination.internalCurrentPage = 1;
                this.equipYcAndYxAndSet(this.nowEquipStaNo, this.nowEquipNo);
            }
        },

        // 取消定时器
        clearTimer () {
            if (this.getModelValueTime) {
                clearInterval(this.getModelValueTime);
            }
            if (this.timeChart) {
                clearInterval(this.timeChart);
            }
            this.getModelValueTime = '';
            this.timeChart = '';
        }
    }
};
