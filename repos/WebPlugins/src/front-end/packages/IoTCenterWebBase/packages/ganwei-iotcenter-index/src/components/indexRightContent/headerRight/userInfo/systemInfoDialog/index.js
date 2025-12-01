import * as Echarts from 'echarts'
export default {
    props: {
        showDialog: {
            type: Boolean,
            default: false
        }
    },
    watch: {
        showDialog (val) {
            this.getSystemInfo()
            this.dialogVisible = val;
        }
    },
    data () {
        return {
            systemInfo: {
                "platformInfo": {
                    "grpcVersionInfo": "",
                    "webVersionInfo": ""
                },
            },
            echartsList: [],
            dialogVisible: false
        }
    },
    mounted () {
        this.getSystemInfo()
    },
    computed: {
        platformInfo () {
            return {
                title: this.$t('login.framePro.platformInfo.title'),
                childObject: {
                    grpcVersionInfo: {
                        title: this.$t('login.framePro.platformInfo.ghostVersion.title'),
                        icon: "icon-yunhangpingtai"
                    },
                    webVersionInfo: {
                        title: this.$t('login.framePro.platformInfo.webVersion.title'),
                        icon: "icon-banbenguankong"
                    }
                }
            }
        },
        licensingAndUsage () {
            return {
                title: this.$t('login.framePro.licensingAndUsage.title'),
                childObject: {
                    "liscenseEquipTotal": {
                        title: this.$t('login.framePro.licensingAndUsage.liscenseEquipTotal.title'),
                        toolTip: this.$t('login.framePro.licensingAndUsage.liscenseEquipTotal.toolTip'),
                        icon: "icon-gw-icon-yijimenu-shebeiguanli"
                    },
                    "liscensePointTotal": {
                        title: this.$t('login.framePro.licensingAndUsage.liscensePointTotal.title'),
                        toolTip: this.$t('login.framePro.licensingAndUsage.liscensePointTotal.toolTip'),
                        icon: "icon-jiancedian"
                    },
                    "equipTotal": {
                        title: this.$t('login.framePro.licensingAndUsage.equipTotal.title'),
                        valueColor: "#87c225",
                        icon: "icon-caidan_shujufenxi"
                    },
                    "pointTotal": {
                        title: this.$t('login.framePro.licensingAndUsage.pointTotal.title'),
                        valueColor: "#87c225",
                        icon: "icon-caidan_shujufenxi"
                    },
                    "equipPercentage": {
                        title: this.$t('login.framePro.licensingAndUsage.equipPercentage.title'),
                        showEchart: true,
                        bottomTip: this.$t('login.framePro.licensingAndUsage.equipPercentage.bottomTip'),
                        echartId: "equipPercentage",
                        unit: "%"
                    },
                    "pointPercentage": {
                        title: this.$t('login.framePro.licensingAndUsage.pointPercentage.title'),
                        showEchart: true,
                        bottomTip: this.$t('login.framePro.licensingAndUsage.pointPercentage.bottomTip'),
                        echartId: "pointPercentage",
                        unit: "%"
                    }
                }
            }
        },
        pointUsageDistribute () {
            return {
                title: this.$t('login.framePro.pointUsageDistribute.title'),
                childObject: {
                    "ycpTotal": {
                        title: this.$t('login.framePro.pointUsageDistribute.ycpTotal.title'),
                        icon: "icon-yaotiao"
                    },
                    "yxpTotal": {
                        title: this.$t('login.framePro.pointUsageDistribute.yxpTotal.title'),
                        icon: "icon-yaoxinxinxiliangguanli"
                    },
                    "setparmTotal": {
                        title: this.$t('login.framePro.pointUsageDistribute.setparmTotal.title'),
                        icon: "icon-tubiao14_shezhi"
                    }
                }
            }
        }
    },
    methods: {
        getColor (childItem) {
            if (!childItem.showEchart) {
                return childItem.valueColor || ''
            }
            let proportion = childItem.value.split('%')[0]
            switch (true) {
                case proportion <= 90:
                    return '#0fe90f';
                case proportion > 90 && proportion < 100:
                    return 'orange'
                case proportion >= 100:
                    return 'red'
                default:
                    return 'green'
            }
        },
        showBottomTip (childItem) {
            if (childItem.showEchart) {
                return childItem.value.split('%')[0] >= 100
            }
            return false
        },

        getSystemInfo () {
            this.$api.getSystemInfo().then(res => {
                const { code, data } = res?.data || {}
                if (code == 200 && data) {
                    Object.keys(this.systemInfo).forEach(key => {
                        if (this[key]) {
                            Object.keys(this.systemInfo[key]).forEach(key1 => {
                                if (this[key]['childObject'][key1]) {
                                    this.systemInfo[key]['childObject'] = this.systemInfo[key]['childObject'] || {}
                                    this.systemInfo[key]['childObject'][key1] = {
                                        value: (data[key] ? data[key][key1] : '') + '',
                                        ...this[key]['childObject'][key1]
                                    }
                                    if (this[key]['childObject'][key1].showEchart) {
                                        this.echartsList.push({
                                            echartId: this[key]['childObject'][key1].echartId,
                                            value: (data[key] ? data[key][key1] : '')
                                        })
                                    }
                                }
                            })
                            this.systemInfo[key].title = this[key].title
                        }
                    })
                    this.drawEchart()
                }
            })
        },
        drawEchart () {
            this.echartsList.forEach(item => {
                let dom = document.getElementById(item.echartId)
                if (dom) {
                    dom = Echarts.init(
                        document.getElementById(item.echartId),
                        'light'
                    )
                    let option = {
                        backgroundColor: 'transparent',
                        angleAxis: {
                            max: 100,
                            clockwise: false, // 逆时针
                            // 隐藏刻度线
                            show: false
                        },
                        radiusAxis: {
                            type: 'category',
                            show: true,
                            axisLabel: {
                                show: false
                            },
                            axisLine: {
                                show: false
                            },
                            axisTick: {
                                show: false
                            }
                        },
                        polar: [
                            {
                                center: ['50%', '50%'],
                                radius: '80%'
                            }
                        ],
                        series: [
                            {
                                type: 'bar',
                                data: [item.value],
                                showBackground: true,
                                polarIndex: 0,
                                backgroundStyle: {
                                    color: 'rgba(157, 166, 186, 0.39)',
                                    borderWidth: 2
                                },
                                coordinateSystem: 'polar',
                                roundCap: true,
                                barWidth: 5,
                                itemStyle: {
                                    normal: {
                                        opacity: 1,
                                        color: '#0097fa'
                                    }
                                }
                            }
                        ]
                    };
                    dom.setOption(option)
                }
            })
        },
        getProportion (total, num) {
            return ((num || 0) / (total || 1) * 100).toFixed(2) + "%"
        },
        closeDialog () {
            this.$emit('closeDialog')
        }
    }
}
