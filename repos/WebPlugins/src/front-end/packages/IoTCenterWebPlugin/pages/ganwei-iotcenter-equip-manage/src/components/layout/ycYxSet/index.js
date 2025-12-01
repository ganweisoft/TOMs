import equipUtils from '../../../utils/util.js';
import yc from '../../dialog/yc/index.vue';
import yx from '../../dialog/yx/index.vue';
import set from '../../dialog/set/index.vue';
import batEditYc from '../../dialog/batEditYc/index';
import batEditYx from '../../dialog/batEditYx/index';
import batEditSet from '../../dialog/batEditSet/index';

function debounce (func, wait = 1000) {
    let timeout;
    return function (event) {
        clearTimeout(timeout);
        timeout = setTimeout(() => {
            func.call(this, event);
        }, wait);
    };
}
export default {
    components: { yc, yx, set, batEditYc, batEditYx, batEditSet },
    props: {
        selecteItem: {
            type: Object,
            default: () => { }
        }
    },
    inject: ['getVideoDropdown', 'getZcDropdown'],

    watch: {
        'selecteItem.equipNo':{
            immediate: true,
            handler(val, oldVal) {
                if (val && val !== 0) {
                    this.clearData()
                    this.getYcYxSetNumByEquipNo(val)
                } else {
                    this.ycTable = [];
                    this.yxTable = [];
                    this.setTable = [];
                    this.ycNum = '0';
                    this.yxNum = '0';
                    this.setNum = '0';
                }
            }
            
        },
        searchTableEquip (val, oldValue) {
            if (!val && oldValue.trim()) {
                this.changeTableList(null, true)
            }
        }
    },
    computed: {
        videoDropdown () {
            return this.getVideoDropdown()
        },
        zcDropdown () {
            return this.getZcDropdown()
        }
    },
    data () {
        return {
            setTypeOptions: [
                {
                    label: 'setTypeOptions.parameterSettings',
                    value: 'V'
                },
                {
                    label: 'setTypeOptions.signalingSettings',
                    value: 'X'
                },
                {
                    label: 'setTypeOptions.telemetrySettings',
                    value: 'C'
                },
                {
                    label: 'setTypeOptions.systemSettings',
                    value: 'S'
                },
                {
                    label: 'setTypeOptions.sceneSettings',
                    value: 'J'
                },
            ],
            searchActiveTabName: 'input.searchYc',
            activeTabName: 'tab.yc',
            dataObj: {},
            pagination: {
                pageSize: 20,
                pageNo: 1,
                totalPage: 0
            },
            ycPagination: {
                pageSize: 20,
                pageNo: 1,
                totalPage: 0
            },
            yxPagination: {
                pageSize: 20,
                pageNo: 1,
                totalPage: 0
            },
            setPagination: {
                pageSize: 20,
                pageNo: 1,
                totalPage: 0
            },
            DataLoading: false,
            searchTableEquip: '',

            // 选中设备ycyxset数量
            ycName: 'tab.yc',
            yxName: 'tab.yx',
            setName: 'tab.set',
            ycNum: '0',
            yxNum: '0',
            setNum: '0',

            loading: false,
            // 选中的标签页
            activeYcYxSet: 'first',

            // 表格数据
            ycTable: [],
            yxTable: [],
            setTable: [],
            showYcDialog: false,
            showYxDialog: false,
            showSetDialog: false,
            showBatYcDialog: false,
            showBatYxDialog: false,
            showBatSetDialog: false,
            saveLoading: false,

            // yc表单
            ycInfo: {
                parameters: [],
                expression: '',
                equipNo: '',
                ycNo: 1,
                ycNm: '',
                unit: '',
                valTrait: 0,
                ziChanId: '',
                ziChanName: '',
                planNo: '',
                relatedPic: '',
                relatedVideo: '',
                relatedVideoName: '',
                valMax: 999999,
                valMin: 0,
                physicMin: 0,
                physicMax: 999999,
                restoreMax: 999999,
                restoreMin: 0,
                ycMin: 0,
                ycMax: 999999,
                mainInstruction: '',
                minorInstruction: '',
                outminEvt: '',
                outmaxEvt: '',
                mapping: false,
                curveRcd: false,
                curveLimit: '',
                lvlLevel: 3,
                restoreAcceptableTime: '',
                alarmAcceptableTime: '',
                alarmRepeatTime: '',
                alarmScheme: 0,
                procAdvice: '',
                waveFile: '',
                alarmShield: '',
                alarmRiseCycle: '',
                safeBgn: '',
                safeEnd: '',
                safeTime: '',
                staNo: 1,
                isGenerateWO: false,
                ycCode: '',
                dataType: null,
            },
            yxInfo: {
                parameters: [],
                expression: '',
                equipNo: '',
                yxNo: 1,
                yxNm: '',
                ziChanId: '',
                ziChanName: '',
                planNo: '',
                relatedPic: '',
                relatedVideo: '',
                relatedVideoName: '',
                valTrait: 0,
                procAdviceR: '',
                procAdviceD: '',
                curveRcd: false,
                levelR: 2,
                levelD: 3,
                evt01: '',
                evt10: '',
                mainInstruction: '',
                minorInstruction: '',
                inversion: false,
                initval: 0,
                safeBgn: '',
                safeEnd: '',
                safeTime: '',
                alarmAcceptableTime: '',
                alarmRepeatTime: '',
                restoreAcceptableTime: '',
                alarmScheme: 0,
                procAdvice: '',
                waveFile: '',
                alarmRiseCycle: '',
                alarmShield: '',
                staNo: 1,
                isGenerateWO: false,
                yxCode: '',
                dataType: null
            },
            setInfo: {
                parameters: [],
                equipNo: '',
                setNo: 1,
                setNm: '',
                setType: '',
                mainInstruction: '',
                minorInstruction: '',
                record: false,
                action: '',
                value: '',
                voiceKeys: '',
                enableVoice: false,
                canexecution: 0,
                staNo: 1,
                setCode: '',
                setOffline: false,
                enableSetParm: false
            },
            isYcNew: false,
            isYxNew: false,
            isSetNew: false,
            copyObject: {},
            deleteOperate: '',
            language: 'zh-CN',
            searchStatus: false,
            tableHeight: null
        }
    },


    mounted () {
        this.language = sessionStorage.languageSet

        // 表格自适应
        let that = this;
        let eqTable = document.getElementById('eqTable');
        if (eqTable) {
            that.tableHeight = eqTable.offsetHeight - 15;
        }
        window.onresize = function windowResize () {
            eqTable = document.getElementById('eqTable');
            if (eqTable) {
                that.tableHeight = eqTable.offsetHeight - 15;
            }
        };
    },

    methods: {
        $tt(string) {
            return this.$t(this.$route.name + '.' + string)
        },
        getSetTypeName (value) {
            let name = ''
            this.setTypeOptions.forEach(item => {
                if (item.value == value) {
                    name = item.label
                }
            })
            return name
        },
        clearData () {
            this.ycPagination = {
                pageSize: 20,
                pageNo: 1,
                totalPage: 0
            }
            this.yxPagination = {
                pageSize: 20,
                pageNo: 1,
                totalPage: 0
            }
            this.setPagination = {
                pageSize: 20,
                pageNo: 1,
                totalPage: 0
            }
            this.pagination = {
                pageSize: 20,
                pageNo: 1,
                totalPage: 0
            }
            this.searchTableEquip = '';
            this.searchStatus = false;
        },
        tabClick (index) {
            this.searchTableEquip = '';
            this.searchStatus = false;
            if (index == 1) {
                this.activeYcYxSet = 'first'
            }
            if (index == 2) {
                this.activeYcYxSet = 'second'
            }
            if (index == 3) {
                this.activeYcYxSet = 'third'
            }
            if (this.activeYcYxSet == 'first') {
                this.activeTabName = 'tab.yc';
                this.searchActiveTabName = 'input.searchYc';
            } else if (this.activeYcYxSet == 'second') {
                this.activeTabName = 'tab.yx'
                this.searchActiveTabName = 'input.searchYx'
            } else if (this.activeYcYxSet == 'third') {
                this.activeTabName = 'tab.set'
                this.searchActiveTabName = 'input.searchSet';
            }
            this.pagination.pageNo = 1;
            this.getItemData();
        },
        getYcYxSetNumByEquipNo (data) {
            this.$api.getYcYxSetNumByEquipNo(data).then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    const { setNum, ycNum, yxNum } = data || {}
                    this.setNum = setNum || 0;
                    this.ycNum = ycNum || 0;
                    this.yxNum = yxNum || 0;
                    this.setPagination.totalPage = this.setNum;
                    this.ycPagination.totalPage = this.ycNum;
                    this.yxPagination.totalPage = this.yxNum;
                    this.getItemData()
                } else {
                    this.$message.error(message)
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            })
        },

        // 获取单个遥测遥信设置数据
        getItemData () {
            if (this.isCheckEq()) return;
            let data = {
                equipNo: this.selecteItem.equipNo,
                pageSize: this.pagination.pageSize,
                pageNo: this.pagination.pageNo,
                searchName: this.searchTableEquip
            };
            this.dataObj = JSON.parse(JSON.stringify(data));
            if (this.activeYcYxSet == 'first') {
                if (!this.searchStatus) {
                    this.pagination.totalPage = this.ycPagination.totalPage;
                }
                this.getYcYxList('ycNum', 'ycTable', 'getEquipYCPConf');
            } else if (this.activeYcYxSet == 'second') {
                if (!this.searchStatus) {
                    this.pagination.totalPage = this.yxPagination.totalPage;
                }
                this.getYcYxList('yxNum', 'yxTable', 'getEquipYXPConf')
            } else if (this.activeYcYxSet == 'third') {
                if (!this.searchStatus) {
                    this.pagination.totalPage = this.setPagination.totalPage;
                }
                this.getSetList()
            }
        },

        // 获取遥测遥信列表
        getYcYxList (tabName, tableName, url) {
            this.loading = true
            this.DataLoading = true;
            this.$api[url](this.dataObj).then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    let rows = data?.rows || [];
                    for (let i = 0; i < rows.length; i++) {
                        rows[i].staNo = rows[i].staN
                        if (rows[i].relatedVideo) {
                            rows[i].relatedVideoName = equipUtils.getVideoName(this.videoDropdown,
                                parseInt(rows[i].relatedVideo.split('|')[1])
                            )
                            rows[i].relatedVideo = parseInt(rows[i].relatedVideo.split('|')[1]);
                        }

                        if (rows[i].curveRcd) {
                            rows[i].curveRcdStr = 'dialog.selectYes';
                        } else {
                            rows[i].curveRcdStr = 'dialog.selectNo';
                        }
                    }
                    this[tableName] = rows;
                    if (this.searchStatus) {
                        this.pagination.totalPage = data?.total || 0
                    }
                } else {
                    this.$message.error(message)
                    this[tableName] = [];
                    this.pagination.totalPage = 0;
                }
            }).catch((err) => {
                this.$message.error(err?.data, err)
            }).finally(() => {
                this.loading = false
                this.DataLoading = false;
            });
        },

        // 获取设置列表---独立开防止添加保存时多传字段
        getSetList () {
            this.DataLoading = true
            this.$api
                .getEquipSetParmConf(this.dataObj)
                .then((res) => {
                    const { code, data, message } = res?.data || {}
                    if (code == 200) {
                        let rows = data?.rows || [];
                        if (rows.length > 0) {
                            rows.forEach((item, i) => {
                                item.staNo = item.staN
                                if (item.record) {
                                    item.recordStr = 'dialog.selectYes';
                                } else {
                                    item.recordStr = 'dialog.selectNo';
                                }
                            });
                        }
                        this.setTable = rows;
                        if (this.searchStatus) {
                            this.pagination.totalPage = data?.total || 0
                        }

                    } else {
                        this.$message.error(message)
                        this.setTable = [];
                        this.pagination.totalPage = 0;
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err)
                }).finally(() => {
                    this.setLoading = false;
                    this.DataLoading = false
                });
        },

        // 搜索ycyx设置
        changeTableList (e, change) {
            this.searchTableEquip = this.searchTableEquip.trim();
            this.searchStatus = !!this.searchTableEquip;
            if (this.searchStatus || change) {
                this.pagination.pageNo = 1
                this.ycYxSetApi();
            }

        },

        ycYxSetApi: debounce(function (e) {
            this.getItemData();
        }, 10),

        // 页码大小改变时触发事件
        handleSizeChange (pageSize) {
            this.pagination.pageSize = pageSize;
            this.pagination.pageNo = 1;
            this.getItemData();
        },

        // 当前页码改变时触发
        handleCurrentChange (pageNo) {
            this.pagination.pageNo = pageNo;
            this.getItemData();
        },

        newYc () {
            if (this.isCheckEq()) {
                return;
            }

            this.isYcNew = true;
            this.copyObject = JSON.parse(JSON.stringify(this.ycInfo))
            this.copyObject.equipNo = this.selecteItem.equipNo;
            this.copyObject.staNo = this.selecteItem.staNo;
            this.showYcDialog = true;
        },

        newYx () {
            if (this.isCheckEq()) return;

            this.isYxNew = true;
            this.copyObject = JSON.parse(JSON.stringify(this.yxInfo))
            this.copyObject.equipNo = this.selecteItem.equipNo;
            this.copyObject.staNo = this.selecteItem.staNo;
            this.showYxDialog = true;
        },

        newSet () {
            if (this.isCheckEq()) return;
            this.isSetNew = true;
            this.copyObject = JSON.parse(JSON.stringify(this.setInfo))
            this.copyObject.equipNo = this.selecteItem.equipNo;
            this.copyObject.staNo = this.selecteItem.staNo;
            this.showSetDialog = true;
        },

        // 编辑遥测
        editYc (row) {
            this.isYcNew = false;
            this.copyObject = JSON.parse(JSON.stringify(this.ycInfo))
            for (let i in this.copyObject) {
                if (row[i] != null && row[i] != undefined) {
                    this.copyObject[i] = row[i];
                }
            }
            this.showYcDialog = true;
        },

        // 编辑遥信
        editYx (row) {

            this.isYxNew = false;
            this.copyObject = JSON.parse(JSON.stringify(this.yxInfo))
            for (let i in this.copyObject) {
                if (row[i] != null && row[i] != undefined) {
                    this.copyObject[i] = row[i];
                }
            }
            this.showYxDialog = true;
        },

        // 编辑设置
        editSet (row) {
            this.isSetNew = false;
            this.copyObject = JSON.parse(JSON.stringify(this.setInfo))

            for (let i in this.copyObject) {
                if (row[i] != null && row[i] != undefined) {
                    this.copyObject[i] = row[i];
                }
            }
            this.showSetDialog = true
        },

        // 保存修改信息到接口
        saveYcYxSet (formName, apiName, dialogName) {
            this.saveLoading = true;
            let data = JSON.stringify(formName);

            // apiName为接口名称
            this.$api[apiName](data)
                .then((res) => {
                    const { code, message } = res?.data || {}
                    if (code == 200) {
                        this.$message.success(this.$t('publics.tips.saveSuccess'));
                        this[dialogName] = false;
                        this.getYcYxSetNumByEquipNo(this.selecteItem.equipNo);
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err);
                }).finally(() => {
                    this.saveLoading = false;
                });
        },

        closeDialog (dialog) {
            this[dialog] = false;
        },

        // 判断是否选择设备
        isCheckEq () {
            if (this.selecteItem.equipNo == 0 || this.selecteItem.equipNo === undefined) {
                this.$message.warning(this.$tt('tips.selectADevice'));
            }
            return this.selecteItem.equipNo == 0 || this.selecteItem.equipNo === undefined;
        },

        deleteYc (row) {
            this.deleteOperate = 'yc';
            this.selectEquipDeleteNm = row.ycNm;
            this.copyObject = JSON.parse(JSON.stringify(this.ycInfo))
            for (let i in this.copyObject) {
                if (row[i]) {
                    this.copyObject[i] = row[i];
                }
            }
            this.deleteDialog();
        },

        deleteYx (row) {
            this.deleteOperate = 'yx';
            this.selectEquipDeleteNm = row.yxNm;
            this.copyObject = JSON.parse(JSON.stringify(this.yxInfo))
            for (let i in this.copyObject) {
                if (row[i]) {
                    this.copyObject[i] = row[i];
                }
            }
            this.deleteDialog();
        },

        deleteSet (row) {
            this.deleteOperate = 'set';
            this.selectEquipDeleteNm = row.setNm;
            this.copyObject = JSON.parse(JSON.stringify(this.setInfo))
            for (let i in this.copyObject) {
                if (row[i]) {
                    this.copyObject[i] = row[i];
                }
            }
            this.deleteDialog();
        },

        // 删除弹窗
        deleteDialog () {
            this.$msgbox({
                title: this.$tt('tips.tip'),
                message: this.$tt('tips.sureToDelete') + `：${this.selectEquipDeleteNm}`,
                showCancelButton: true,
                type: 'warning',
                confirmButtonText: this.$t('publics.button.confirm'),
                cancelButtonText: this.$t('publics.button.cancel'),
                beforeClose: (action, instance, done) => {
                    if (action === 'confirm') {
                        instance.confirmButtonLoading = true;
                        setTimeout(() => {
                            this.confirmDelete(() => {
                                instance.confirmButtonLoading = false;
                                done();
                            });
                        }, 120);
                    } else { done() }
                }
            }).then().catch(() => {
            })
        },

        // 确定删除
        confirmDelete (cb) {
            console.log(this.deleteOperate, 66999)
            if (this.deleteOperate == 'yc') {
                let length = this.ycTable.length
                this.$api
                    .deleteYc({
                        staNo: this.copyObject.staNo,
                        equipNo: this.selecteItem.equipNo,
                        ycYxSetNo: this.copyObject.ycNo
                    })
                    .then((res) => {
                        const { code, message } = res?.data || {}
                        if (code == 200) {
                            this.$message.success(this.$t('publics.tips.deleteSuccess'))
                            this.deleteEquipDialog = false;
                            if (length == 1 && this.pagination.pageNo > 1) {
                                this.pagination.pageNo = this.pagination.pageNo - 1
                            }
                            this.getYcYxSetNumByEquipNo(this.selecteItem.equipNo);
                        } else {
                            this.$message.error(message)
                        }
                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err)
                    }).finally(() => {
                        cb()
                        this.delEquipLoading = false;
                    });
            } else if (this.deleteOperate == 'yx') {
                let length = this.yxTable.length
                this.$api
                    .deleteYx({
                        staNo: this.copyObject.staNo,
                        equipNo: this.selecteItem.equipNo,
                        ycYxSetNo: this.copyObject.yxNo
                    })
                    .then((res) => {
                        const { code, message } = res?.data || {}
                        if (code == 200) {
                            this.$message.success(this.$t('publics.tips.deleteSuccess'))
                            this.deleteEquipDialog = false;
                            if (length == 1 && this.pagination.pageNo > 1) {
                                this.pagination.pageNo = this.pagination.pageNo - 1
                            }
                            this.getYcYxSetNumByEquipNo(this.selecteItem.equipNo);
                        } else {
                            this.$message.error(message)
                        }
                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err)

                    }).finally(() => {
                        this.delEquipLoading = false;
                        cb()
                    });
            } else if (this.deleteOperate == 'set') {
                let length = this.setTable.length
                this.$api
                    .deleteSet({
                        staNo: this.copyObject.staNo,
                        equipNo: this.selecteItem.equipNo,
                        ycYxSetNo: this.copyObject.setNo
                    })
                    .then((res) => {
                        const { code, message } = res?.data || {}
                        if (code == 200) {
                            this.$message.success(this.$t('publics.tips.deleteSuccess'))
                            this.deleteEquipDialog = false;
                            if (length == 1 && this.pagination.pageNo > 1) {
                                this.pagination.pageNo = this.pagination.pageNo - 1
                            }
                            this.getYcYxSetNumByEquipNo(this.selecteItem.equipNo);
                        } else {
                            this.$message.error(message)
                        }

                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err)

                    }).finally(() => {
                        this.delEquipLoading = false;
                        cb()
                    });
            }
        },

        // 编辑设备
        editEquip () {
            this.$parent.editEquip()
        },

        // 移动设备
        moveEquip () {
            this.$parent.moveEquip()
        },

        // 删除设备
        deleteEquip () {
            this.$parent.deleteEquip()
        }

    }
}
