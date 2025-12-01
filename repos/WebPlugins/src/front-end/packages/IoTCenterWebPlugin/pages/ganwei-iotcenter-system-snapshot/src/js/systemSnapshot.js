
export default {
    name: 'systemSnapshot',
    // mixins: [Speaktts],
    components: {
        // GWVideoPlaybackCom
    },
    data () {
        return {
            tableHeight: null,
            timeInterval: null,

            // 顶部类型导航切换
            confirmedNo: false,
            confirmedOk: false,

            allSnapshoot: [], // 所有快照记录-渲染页面

            // 弹出层信息
            popAdmin: '',
            popType: '', // 事件类型
            popTime: '', // 时间
            popIncident: '', // 事件名
            popConfirmremark: '', // 处理意见
            acknowledgingTime: '', // 已确认事件的确认时间
            timeId: '', // 事件id
            handlingSuggestion: '', // 处理意见--监听输入内容

            // 分页组件
            confirm: 0, // 0：全部、1:已确认、2：待确认
            total: '',
            pageSize: 20, // 每页条数
            pageNo: 1, // 页码

            // 加载loading
            loading: true,
            state: '',
            numberStrAll: '',
            tabTopList: [],
            defaultTopNavImages: [
                {
                    img: '/static/images/systemsnapshot-guzhang.png'
                },
                {
                    img: '/static/images/systemsnapshot-jinggao.png'
                },
                {
                    img: '/static/images/systemsnapshot-xinxi.png'
                },
                {
                    img: '/static/images/systemsnapshot-shezhi.png'
                },
                {
                    img: '/static/images/systemsnapshot-zichan.png'
                }],

            // 确认快照时是否为误报
            radio1: 0,
            kzTitle: '',
            attrCurrentList: null,
            wuBao: '',
            wuBaoShow: false,
            onAffirmShow: true,
            setIntervalOk: 0,
            typeList: [],
            tabId: -1,

            listType: [
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.error'),
                    title: '故障',
                    text: '故障',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: ''
                },
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.warning'),
                    title: '警告',
                    text: '警告',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: ''
                },
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.info'),
                    title: '信息',
                    text: '信息',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: ''
                },
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.setting'),
                    title: '设置',
                    text: '设置',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: ''
                },
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.asset'),
                    title: '资产',
                    text: '资产',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: ''
                }],
            activeNm: [],

            // 录像回放
            videoShow: false,
            videoValue: {},
            videoName: '',

            // 浏览器宽高
            clientWidth: 0,
            clientHeight: 0,

            drawer: false,
            form: {
                popType: '',
                popTime: '',
                popIncident: '',
                popConfirmremark: ''
            },

            formReadOnly: false,

            // 导入导出
            exportType: 1,
            exportLoading: false,
            exportDialog: false,
            showExportSnapshots: false,
            showTopology: false,
            topology: '',
            topologyName: '',

            // 播音定时器
            speakttsRange: [],
            speakttsArray: [],
            speakttsIs: true,
            speakttsTime: null,
            speakttsText: true,

            // 当页数据最下面的事件, 往后查
            LastRecordId: null,

            // 当页数据最上面的事件，往回查
            MaxRecordId: null,

            // 终端选项
            terminalOptions: [],
            terminalType: null
        }
    },
    computed: {
        isLargeScreen: function () {
            return this.clientWidth > 992
        },
        languageSet: function () {
            return this.$i18n.locale
        }
    },

    // 监听中英文切换
    watch: {
        languageSet (value) {
            this.listType = [
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.error'),
                    title: '故障',
                    text: '故障',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: require('../images/systemsnapshot-guzhang.png')
                },
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.warning'),
                    title: '警告',
                    text: '警告',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: require('../images/systemsnapshot-jinggao.png')
                },
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.info'),
                    title: '信息',
                    text: '信息',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: require('../images/systemsnapshot-xinxi.png')
                },
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.setting'),
                    title: '设置',
                    text: '设置',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: require('../images/systemsnapshot-shezhi.png')
                },
                {
                    check: false,
                    name: this.$t('systemSnapshot.typeLst.asset'),
                    title: '资产',
                    text: '资产',
                    tabTopId: 0,
                    level: 0,
                    value: 0,
                    img: require('../images/systemsnapshot-zichan.png')
                }]
            this.getRealTimeEventTypeConfig()
            this.popType = this.popType.split(' ')[0] + ' ' + this.$t('systemSnapshot.tips.details')
        },
        pageNo (val) {
            if (val == 1 && !this.timeInterval) {
                this.timeInterval = setInterval(() => {
                    this.getRealTimeEventCount()
                }, 5000)
            } else {
                if (this.timeInterval) {
                    clearInterval(this.timeInterval)
                    this.timeInterval = null
                }
            }
        }
    },
    async mounted () {
        let that = this
        window.onresize = function windowResize () {
            that.clientWidth = document.body.clientWidth
            that.clientHeight = document.body.clientHeight
            that.tableHeight = document.getElementById('systemSnapshot').offsetHeight - document.getElementById('containerHeader').offsetHeight - 64 - 68 - 20
        }
        let query = this.$route.query
        const { id } = query
        if (id) {
            this.tabId = parseInt(id, 10)
        }
        this.getRealTimeEventTypeConfig()

        await this.myUtils.configInfoData(this).then(webConfig => {
            this.showExportSnapshots = webConfig.showExportSnapshots
            this.speakttsRange = webConfig.speaktts || []
        })

        this.getWindowData()
        window.onmessage = e => {
            if (e.data.haveNewSnapshotData) {
                this.getWindowData()
            }
        }
    },

    methods: {
        getDefaultTypeImage (index) {
            return this.defaultTopNavImages[index] ? this.defaultTopNavImages[index].img : this.defaultTopNavImages[0].img
        },
        getWindowData () {
            if (window.top.snapshotData) {
                this.onTBC(window.top.snapshotData)
            }
        },


        // 查看组态
        goLocation (row, route) {
            let equipInfo = {
                id: row.relatedPic,
                eno: row.equipno

                // yc: '',
                // yx: '',
                // sno: ''
            }
            this.topologyName = row.equipName
            if (row.type.toLowerCase() == 'c') {
                equipInfo.yc = row.ycyxno
            } else if (row.type.toLowerCase() == 'x') {
                equipInfo.yx = row.ycyxno
            }
            let url = route + '?'
            Object.keys(equipInfo).forEach(item => {
                url += `${item}=${equipInfo[item]}&`
            })
            this.showTopology = true
            this.$nextTick(() => {
                this.topology = url
            })
        },

        // 预案跳转
        goPlan (planNo) {
            window.top.location.hash = `#/Index/jumpIframe/ganwei-iotcenter-modules-planmanage/planmanagement`
            sessionStorage.setItem('planId', planNo)
        },

        // 组件关闭录像回放窗口
        onBtnClose () {
            this.videoShow = false
        },

        // 资产点击事件跳转到资产清单
        goToZiChan (id, name) {
            if (name == null) {
                return
            }
            if (name.length > 0) {
                this.$router.push({
                    path: '/Index/assetManagement',
                    query: {
                        id,
                        name
                    }
                })
            }
        },

        // 查找录像回放列表
        getPlayback (token, start, end) {
            this.videoShow = true
            let docData = []
            if (!window.localStorage.ac_session) {
                this.$message({
                    title: this.$t('systemSnapshot.messageType.connectError'),
                    type: 'warning'
                })
                return
            }
            this.$api
                .getVideoSearch({
                    hostUrl: '/api/v1/Search',
                    session: window.localStorage.ac_session,
                    token,
                    type: 'record', // 类型为视频回放
                    start,
                    end
                })
                .then(res => {
                    let result = JSON.parse(res?.data?.data ?? {})
                    if (!result.record) {
                        this.$message({
                            title: this.$t('systemSnapshot.messageType.noPlayBackInTime'),
                            type: 'warning'
                        })
                        return
                    }
                    let list = result.record
                    if (!list) {
                        this.$message({
                            title: this.$t('systemSnapshot.messageType.noPlayBackInTime'),
                            type: 'warning'
                        })
                        return
                    }
                    if (list.length > 0) {
                        list.forEach((item, i) => {
                            let obj = {}
                            let strPathTo = []
                            if (this.searchType === 'record') {
                                obj.info = this.timeFormat(item.strStartTime) + ' - ' + this.timeFormat(item.strEndTime)
                            } else {
                                obj.info = this.timeFormat(item.strEndTime)
                            }

                            strPathTo = item.strPath.split('/')
                            item.strPathTo = strPathTo[strPathTo.length - 1]
                            item.downUrl = this.$store.state.serverUrl + item.strPath
                            item.active = ''
                            obj.data = item
                            docData.push(obj)
                        })
                    }
                    this.videoValue = docData[0]
                })
                .catch(err => {
                    this.$message({
                        title: this.$t('systemSnapshot.messageType.searchFailed'),
                        type: 'warning'
                    })
                })
        },

        onVideo (item) {
            if (!window.localStorage.ac_session) {
                // h5流媒体视频
                this.myUtils.streamLogin()
            }

            this.videoName = ''
            if (item.relatedVideo === '') {
                this.$message({
                    title: this.$t('systemSnapshot.messageType.noPlayBack'),
                    type: 'warning'
                })
                return
            }
            let date = new Date(item.time).getTime() / 1000

            let dateS = this.myUtils.getTimeStampToTime((date - 300) * 1000) // 前五分钟
            let dateE = this.myUtils.getTimeStampToTime((date + 300) * 1000) // 后五分钟

            this.$api.getEquipNumCatVideoId(String(item.relatedVideo)).then(res => {
                if (res?.data?.code === 200) {
                    if (res?.data?.data) {
                        let videoObj = res.data.data[0]
                        this.videoName = videoObj.path.slice(3)
                        this.getPlayback(videoObj.channelNum, dateS, dateE)
                    } else {
                        this.$message({
                            title: this.$t('systemSnapshot.messageType.noPlayBack'),
                            type: 'warning'
                        })
                    }
                } else {
                    this.$message({
                        title: this.$t('systemSnapshot.messageType.noPlayBack'),
                        type: 'warning'
                    })
                }
            })
        },

        // 关闭视频回放
        onCloseVideo () {
            this.videoShow = false
        },

        // 后五分钟
        addThreeS (dateStr) {
            // dateStr格式为yyyy-mm-dd hh:mm:ss
            let dt = new Date(dateStr.replace(/-/, '/')) // 将传入的日期格式的字符串转换为date对象 兼容ie
            // var dt=new Date(dateStr);//将传入的日期格式的字符串转换为date对象 非ie
            let ndt = new Date(dt.getTime() + 30000000) // 将转换之后的时间减去两秒
            let result = {
                year: parseInt(ndt.getFullYear()),
                month: parseInt(ndt.getMonth() + 1),
                day: parseInt(ndt.getDay()),
                hour: parseInt(ndt.getHours()),
                minute: parseInt(ndt.getMinutes()),
                second: parseInt(ndt.getSeconds())
            }
            return result
        },

        // 前五分钟
        reduceTwoS (dateStr) {
            // dateStr格式为yyyy-mm-dd hh:mm:ss
            let dt = new Date(dateStr.replace(/-/, '/')) // 将传入的日期格式的字符串转换为date对象 兼容ie
            // var dt=new Date(dateStr);//将传入的日期格式的字符串转换为date对象 非ie
            let ndt = new Date(dt.getTime() - 30000000) // 将转换之后的时间减去两秒
            let result = {
                year: parseInt(ndt.getFullYear()),
                month: parseInt(ndt.getMonth() + 1),
                day: parseInt(ndt.getDay()),
                hour: parseInt(ndt.getHours()),
                minute: parseInt(ndt.getMinutes()),
                second: parseInt(ndt.getSeconds())
            }
            return result
        },

        radioVla (no) {
            console.log('no-', no)
        },
        generateNumberString (min, max) {
            return Array.from({ length: max - min + 1 }, (v, k) => k + min).join(',')
        },

        // 获取实时快照类别配置
        getRealTimeEventTypeConfig () {
            if (this.timeInterval) {
                clearInterval(this.timeInterval)
            }
            this.loading = true
            this.Axios.get('/static/json/snapshot.json').then(res => {
                let data = res.data || {};
                if (Object.entries(data).length == 0) {
                    this.$message.warning(this.$t('systemSnapshot.messageType.snapshotConfig'))
                }
                this.tabTopList = Object.entries(data).map(([levelRange, value]) => {
                    const [min, max] = levelRange.split('-')
                    const level = this.generateNumberString(parseInt(min), parseInt(max));
                    return {
                        key: levelRange,
                        check: true,
                        img: value.icon,
                        title: value[sessionStorage.languageType],
                        key: value['zh-CN'],
                        level,
                        value: 0
                    }
                })
                // 激活项，所有激活项level
                this.activeNm = this.tabTopList.map(i => i.level);
                this.eventTypeList = this.activeNm.join(',');
                // 所有类型level
                this.numberStrAll = this.tabTopList.map(i => i.level).join(';')

                this.getRealTimeEventCount()
                this.timeInterval = setInterval(() => {
                    this.getRealTimeEventCount()
                }, 5000)
                this.$nextTick(() => {
                    this.clientWidth = document.body.clientWidth
                    this.clientHeight = document.body.clientHeight
                    this.tableHeight = document.getElementById('systemSnapshot').offsetHeight - document.getElementById('containerHeader').offsetHeight - 64 - 68 - 20
                })
            }).catch(err => {
                this.$message.warning(this.$t('systemSnapshot.messageType.snapshotConfig'))
                console.log(err)
            }).finally(() => {
                this.loading = false
            })
        },

        // 获取实时快照事件总数---支持动态配置实时快照项
        getRealTimeEventCount () {
            this.$api
                .getRealTimeEventCount(this.numberStrAll)
                .then(rt => {
                    if (rt?.data?.code === 200) {
                        let rtList = rt?.data?.data ?? []
                        if (rtList.length > 0) {
                            rtList.forEach((item, i) => {
                                this.tabTopList.forEach(itemJ => {
                                    if (item.name === itemJ.key) {
                                        itemJ.value = item.value
                                    }
                                })
                            })
                        }
                        if (this.tabId !== -1) {
                            this.onTabTop(this.tabId, this.tabTopList[this.tabId])
                        }
                        this.getRealTimeEvent()
                    } else {
                        this.$message.error(rt?.data?.message, rt)
                    }

                    // this.loading = false;
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                    // this.loading = false;
                    console.log('err---', err)
                })
        },

        // 获取当前系统报警实时事件--数据列表
        getRealTimeEvent () {
            let data = {
                pageNo: this.pageNo,
                pageSize: this.pageSize,
                eventType: this.eventTypeList,
                eventName: this.state,
                LastRecordId: this.LastRecordId,
                MaxRecordId: this.MaxRecordId,
                // TerminalTypeId: this.terminalType
            }
            this.$api
                .getRealTimeEvent(data)
                .then(rt => {
                    if (rt?.data?.code === 200) {
                        let allSnapshoot = rt?.data?.data?.list ?? []
                        this.total = rt?.data?.data?.totalCount ?? 0
                        if (allSnapshoot.length > 0) {
                            allSnapshoot.forEach((item, i) => {
                                item.guid = item.guid
                                item.time = item.time.replace(/T/, ' ')
                                item.typeIcon = this.judgeSnapshotType(item.level).typeIcon
                                item.typeSz = this.judgeSnapshotType(item.level).typeSz
                                if (item.relatedVideo) {
                                    item.relatedVideoState = true
                                } else {
                                    item.relatedVideoState = false
                                }
                                item.spanTextColor = this.judgeSnapshotType(item.level).spanTextColor
                                item.typeIndex = this.judgeSnapshotType(item.level).index

                                // 塞入语音报警序列
                                if (this.speakttsIs && !item.userConfirm && item.wavefile != 0) {
                                    this.speakttsArray.push({
                                        [item.typeSz]: item.eventMsg
                                    })
                                }
                            })

                            if (this.speakttsIs) {
                                this.speakttsSetPlay()
                            }
                        }
                        this.allSnapshoot = allSnapshoot
                    } else {
                        this.$message.error(rt.data.message, rt)
                    }

                    this.loading = false
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                    this.loading = false
                    console.log('err---', err)
                })
        },

        // 语音播报实现
        speakttsSetPlay () {
            let that = this,
                isCurrentSpeak = false
            that.speakttsIs = false

            // 先确认that.speakttsRange播放项目，在that.speakttsArray是否存在
            if (that.speakttsArray.length > 0 && that.speakttsRange.length > 0) {
                that.speakttsRange.forEach((itemParent, indexParent) => {
                    that.speakttsArray.forEach((item, index) => {
                        if (item[itemParent]) {
                            isCurrentSpeak = true
                        }
                    })
                })
            }

            if (isCurrentSpeak) {
                that.speakttsTime = setTimeout(() => {
                    if (that.speakttsArray.length > 0 && that.speakttsRange.length > 0) {
                        that.speechPlay('语音播报开始', false)
                        that.speakttsRange.forEach((itemParent, indexParent) => {
                            that.speechPlay('现在进行' + itemParent + '播报', false)
                            that.speakttsArray.forEach((item, index) => {
                                if (item[itemParent]) {
                                    that.speechPlay(item[itemParent] + `正在发生${itemParent}提示，请处理`, false)
                                }
                            })
                        })
                        that.speechPlay('语音播报结束，将进行新一轮循环播报', true)
                    }
                    that.speakttsArray = []
                    clearTimeout(that.speakttsTime)
                    that.speakttsTime = null
                }, 1000)
            } else {
                that.speakttsIs = true
            }
        },

        // 判断快照类型
        judgeSnapshotType (no) {
            let type = {
                typeIcon: '',
                spanTextColor: '',
                typeSz: ''
            }
            let obj = this.tabTopList.filter(item => item.level.split(',').includes(String(no)))
            let index = this.tabTopList.findIndex(item => item.level.split(',').includes(String(no)))
            if (obj[0]) {
                type = {
                    typeIcon: obj[0].img,
                    typeSz: obj[0].title,
                    spanTextColor: '',
                    index
                }
            }
            return type
        },

        // 快照确认事件
        onAffirm () {
            let wuBao = 0
            let attrCurrentList = this.attrCurrentList

            if (this.handlingSuggestion.length > 255) {
                this.$message({
                    title: this.$t('systemSnapshot.messageType.overflow'),
                    type: 'warning'
                })
                return
            }
            if (Number(this.radio1)) {
                wuBao = 1
            }
            let guid = attrCurrentList.guid
            let data = {
                GUID: guid,
                eventMsg: this.popIncident,
                time: this.myUtils.setDate(this.popTime),
                procMsg: this.handlingSuggestion,
                userName: sessionStorage.getItem('userName'),
                wuBao: wuBao
            }
            this.onAffirmShow = false
            this.$api
                .getConfirmedRealTimeEvent(data)
                .then(rt => {
                    if (rt?.data?.code === 200) {
                        this.$message({
                            title: this.$t('systemSnapshot.messageType.confirmed'),
                            type: 'success'
                        })
                        this.onAffirmShow = true
                        this.confirmedNo = false
                        this.confirmedOk = false
                        this.handlingSuggestion = ''
                        this.getRealTimeEventCount()
                    } else {
                        this.onAffirmShow = true
                        this.$message.error(rt?.data?.message, rt)
                        this.confirmedNo = false
                        this.confirmedOk = false
                        this.handlingSuggestion = ''
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                    console.log('err---', err)
                    this.onAffirmShow = true
                })
        },

        // 打开事件快照-事件详情
        onTBC (item) {
            this.radio1 = null
            this.wuBaoShow = false
            this.attrCurrentList = item
            if (item.ycyxno > 0 && item.type != null) {
                this.wuBaoShow = true
            }
            if (item.bConfirmed) {
                this.confirmedNo = false
                this.confirmedOk = true
                this.acknowledgingTime = item.dtConfirm.slice(0, 19).replace(/T/, ' ')
                window.top.snapshotData = null
            } else {
                this.confirmedOk = false
                this.confirmedNo = true
            }
            this.popAdmin = item.userConfirm // 确认人
            this.popType = item.typeSz || item.Type + ' ' + this.$t('systemSnapshot.tips.details')
            this.popTime = item.time || item.Time
            this.popIncident = item.eventMsg || item.EventMsg
            this.popConfirmremark = item.procAdviceMsg || item.ProcAdviceMsg
        },

        // 关闭事件快照-遮罩层
        closeEquipDialog () {
            this.confirmedNo = false
            this.confirmedOk = false
            this.handlingSuggestion = ''
            window.top.snapshotData = null
        },

        // 判断tab高亮
        tabActive (level, data) {
            // 最少保留一个类型
            if (this.activeNm.length === 1 && level === this.activeNm[0]) {
                return
            }

            const clickItem = this.tabTopList.find(item => item.level === level);
            clickItem && (clickItem.check = !clickItem.check)
            this.activeNm = this.tabTopList.filter(item => item.check).map(item => item.level)
            this.eventTypeList = this.activeNm.join(',')
        },

        // 顶部tab切换
        onTabTop (no, item) {
            this.tabActive(item.level, item)

            // 重复点击最后一个禁止请求
            if (this.activeNm.length === 1 && item.level === this.activeNm[0]) {
                return
            }

            this.setIntervalOk = 1
            this.pageNo = 1
            if (this.total >= 1) {
                this.$refs.pagination.internalCurrentPage = 1
            }
            this.loading = true
            this.getRealTimeEvent()
        },

        // 页码大小改变时触发事件-每页条数
        handleSizeChange (pageSize) {
            this.pageSize = pageSize
            this.pageNo = 1

            // 改变每页条数后将从第一页查起
            this.$refs.pagination.internalCurrentPage = 1
            this.loading = true
            this.getRealTimeEvent()
        },

        // 页码改变事件
        handleCurrentChange (currentPage) {
            if (currentPage == 1) {
                // 获取最新
                this.MaxRecordId = null
                this.LastRecordId = null
            } else {
                // 往后查
                if (currentPage > this.pageNo) {
                    this.LastRecordId = String(this.allSnapshoot[this.allSnapshoot.length - 1].timeID)
                    this.MaxRecordId = null
                } else {
                    // 往回查
                    this.MaxRecordId = String(this.allSnapshoot[0].timeID)
                    this.LastRecordId = null
                }
            }

            this.pageNo = currentPage
            this.loading = true
            this.getRealTimeEvent()
        },

        showDetail (val) {
            this.drawer = true
            this.formReadOnly = val.bConfirmed
            this.attrCurrentList = val

            this.form = {
                popType: val.typeSz,
                popTime: val.time,
                popIncident: val.eventMsg,
                popConfirmremark: val.procAdviceMsg ? val.procAdviceMsg : ''
            }
        },

        onAffirmDraw () {
            if (this.formReadOnly) {
                return
            }

            let wuBao = 0
            let data = {
                GUID: this.attrCurrentList.guid,
                eventMsg: this.form.popIncident,
                time: this.myUtils.setDate(this.form.popTime),
                procMsg: this.form.popConfirmremark,
                userName: sessionStorage.getItem('userName'),
                wuBao: wuBao
            }
            this.$api
                .getConfirmedRealTimeEvent(data)
                .then(rt => {
                    if (rt?.data?.code === 200) {
                        this.drawer = false
                        this.$message({
                            title: this.$t('systemSnapshot.messageType.confirmed'),
                            type: 'success'
                        })
                        this.getRealTimeEventCount()
                    } else {
                        this.$message.error(rt?.data?.message, rt)
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                    console.log('err---', err)
                })
        },

        closedDrawer () {
            this.form = {
                popType: '',
                popTime: '',
                popIncident: '',
                popConfirmremark: ''
            }
        },

        // 导出导入确认
        exportInfo () {
            this.exportDialog = true
        },
        confirmExport () {
            let that = this
            this.$api
                .exportRealTimeRecord(this.eventTypeList)
                .then(res => {
                    if (res.status === 200) {
                        let url = window.URL.createObjectURL(new Blob([res?.data ?? []]))
                        let link = document.createElement('a')
                        link.style.display = 'none'
                        link.href = url
                        let date = new Date(),
                            excelName = ''
                        let dateStr = `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`

                        document.querySelectorAll('.aside_top_nav.aside_top_nav_active').forEach(item => {
                            excelName += item.querySelector('.tab_btn_title .title').innerHTML
                        })

                        // let excelName = $(".aside_top_nav_active>.tab_btn_title>.title").html();//this.$t('systemSnapshot.excelName');
                        link.setAttribute('download', dateStr + '-' + excelName + '.xlsx')
                        document.body.appendChild(link)
                        link.click()
                        this.exportDialog = false
                    } else {
                        this.$message.error(this.$t('systemSnapshot.publics.tips.exportFail'), res)
                    }
                })
                .catch(err => {
                    this.exportDialog = false
                    let reader = new FileReader()
                    reader.readAsText(err?.data, 'utf-8')
                    reader.onload = function (e) {
                        let content = JSON.parse(reader.result)
                        that.$message.error(content.message, err)
                    }
                })
        }
    },
    beforeRouteUpdate () {
        if (this.timeInterval) {
            clearInterval(this.timeInterval)
        }
    },
    beforeDestroy () {
        if (this.timeInterval) {
            clearInterval(this.timeInterval)
        }
        this.onBtnClose()
    },

    destroyed () {
        if (this.timeInterval) {
            clearInterval(this.timeInterval)
        }
        this.onBtnClose()
    }
}
