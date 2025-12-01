import Highcharts, { chart } from 'highcharts'
import highchartTheme from './highchartTheme'
import boost from 'highcharts/modules/boost';
boost(Highcharts)
import mouseZoom from './mousewheelZoom'
mouseZoom(Highcharts)
import Navigator from './renderZoom'


Highcharts.setOptions(highchartTheme);

Highcharts.setOptions({
    global: {
        useUTC: false
    }
})
let timeValue = []
let historyValue = []
let historyTime = []
let timeZoomChart = null
let detailsChart = null

export default {
    props: {
        closeDialog: {
            type: Function,
            default: () => { }
        },
        showDialog: {
            type: Boolean,
            default: false
        },
        currentSelect: {
            type: Object,
            default: () => { }
        },
        theme: {
            type: String,
            default: ''
        }
    },
    data () {
        return {
            pickerOptions: {
                disabledDate: (time) => {
                    let hmTime =
                        this.myUtils.getCurrentDate(0, '-').substring(0, 10) + ' 24:00:00'
                    let date = new Date(hmTime)
                    let thatDay = date.getTime()
                    return time.getTime() >= thatDay
                }
            },
            defaultTime: ['00:00:00', '23:59:59'],
            chartHistoryTime: [],
            loading: false,
            splitIndex: [],
            dataHistory: {},
            dataZoomChange: false,
            historyLength: 0
        }
    },
    watch: {
        theme (val) {
            this.drawEchart();
        }
    },
    mounted () {
        if (sessionStorage.languageType === 'zh-CN') {
            Highcharts.setOptions({
                lang: {
                    resetZoom: '重置缩放'
                }
            })
        }
        this.dataHistory = {
            type: this.currentSelect.ycyxType,
            beginTime: '',
            endTime: '',
            staNo: this.currentSelect.staNo,
            equipNo: this.currentSelect.equipNo,
            // ycpNo
        }
        this.currentSelect.ycyxType === 'C' ? this.dataHistory.ycpNo = this.currentSelect.ycyxNo : this.dataHistory.yxpNo = this.currentSelect.ycyxNo
        this.chartHistoryTime.push(
            this.myUtils.getCurrentDate(1, '-') + ' 00:00:00'
        )
        this.chartHistoryTime.push(
            this.myUtils.getCurrentDate(1, '-') + ' 23:59:59'
        )
        this.dataHistory.beginTime =
            this.myUtils.getCurrentDate(1, '-') + ' 00:00:00'
        this.dataHistory.endTime =
            this.myUtils.getCurrentDate(1, '-') + ' 23:59:59'
    },

    methods: {
        // 历史曲线---开始查询
        onInquire () {
            if (!this.checkTime()) {
                return;
            }

            this.equipHistory = []
            this.loading = true
            historyTime = []
            historyValue = []
            this.hisChart = null

            let url = this.dataHistory.ycpNo ? 'getEquipGetCurData' : 'getEquipGetYxpCurData'
            this.$api[url](this.dataHistory)
                .then((res) => {
                    this.loading = false
                    const { code, data, message } = res?.data || {}
                    if (code === 200) {
                        let times = data?.times || []
                        let values = data?.values || []
                        if (!times.length || !values.length) {
                            this.$message.warning(this.$t('equipListsIot.tips.queryHistory'))
                            this.equipHistory = []
                            historyValue = []
                            this.splitIndex = []
                            if (this.hisChart) {
                                this.hisChart.clear()
                                this.hisChart = null
                            }
                            return
                        }
                        this.historyLength = times.length
                        historyTime = times
                        historyValue = values
                        setTimeout(() => {
                            this.drawEchart()
                        }, 500)
                    } else {
                        this.$message.error(message)
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err)
                    console.log('err---', err)
                    this.loading = false
                })
        },

        getMarkFill () {
            return this.theme === 'dark' ? '#34415d' : 'rgba(0,0,255,0.2)';
        },

        drawEchart () {
            const maskFill = this.getMarkFill()
            const data = timeValue = historyValue.map((value, index) => [new Date(historyTime[index]).getTime(), value === '' ? null : value]);
            if (timeZoomChart) {
                timeZoomChart.destroy()
            }
            timeZoomChart = Highcharts.chart(
                'historyData-timezoom',
                {
                    chart: {
                        backgroundColor: 'transparent',
                        reflow: false,
                        marginLeft: 50,
                        marginRight: 20,
                        // zoomType: 'x',
                        zooming: {
                            mouseWheel: {
                                enabled: false
                            }
                        },
                        events: {
                            selection: function (event) {
                                const extremesObject = event.xAxis[0],
                                    min = extremesObject.min,
                                    max = extremesObject.max,
                                    xAxis = this.xAxis[0];

                                xAxis.removePlotBand('mask-before');
                                xAxis.addPlotBand({
                                    id: 'mask-before',
                                    from: min,
                                    to: max,
                                    color: maskFill
                                });

                                detailsChart.xAxis[0].setExtremes(min, max);
                                if (!detailsChart.resetZoomButton) {
                                    detailsChart.showResetZoom();
                                }
                                return false;
                            },
                            load () {
                                new Navigator(this)
                            }
                        }
                    },
                    credits: {
                        enabled: false
                    },
                    title: {
                        text: null
                    },
                    tooltip: {
                        enabled: false
                    },
                    xAxis: {
                        type: 'datetime',
                        title: {
                            text: null
                        },
                        labels: {
                            enabled: false
                        },
                        tickWidth: 0
                    },
                    yAxis: {
                        maxPadding: 0.2,
                        visible: false,
                        gridLineWidth: 0,
                        labels: {
                            enabled: false
                        },
                        title: {
                            text: null
                        },
                        showFirstLabel: false,
                        tickWidth: 0
                    },
                    legend: {
                        enabled: false
                    },
                    plotOptions: {
                        series: {
                            marker: {
                                enabled: false,
                                states: {
                                    hover: {
                                        enabled: false,
                                    }
                                }
                            }
                        }
                    },
                    series: [{
                        data: data,
                        color: '#37a2da'
                    }]

                },
                (timeZoomChart) => {
                    this.drawDetails(timeZoomChart);
                }
            );
            timeZoomChart.container.addEventListener('wheel', (e) => {
                e = timeZoomChart.pointer.normalize(e)
                if (timeZoomChart.isInsidePlot(
                    e.chartX - timeZoomChart.plotLeft,
                    e.chartY - timeZoomChart.plotTop
                )) {
                    const wheelSensitivity = 1.1,
                        delta = e.detail || ((e.deltaY || 0) / 120),
                        xAxis = timeZoomChart.xAxis[0]
                    const howMuch = Math.pow(
                        wheelSensitivity,
                        delta
                    )
                    const { from, to } = xAxis.plotLinesAndBands[0].options
                    const xRange = (from - to) * (howMuch - 1);
                    const newFrom = from + xRange
                    const newTo = to + xRange
                    if (newFrom < xAxis.min || newTo > xAxis.max) {
                        return
                    }
                }

            })
        },

        drawDetails (timeZoomChart) {
            const data = timeValue;
            const detailData = [], detailStart = data[0][0];
            timeZoomChart.series[0].xData.forEach((x, index) => {
                if (x >= detailStart) {
                    detailData.push([x, timeZoomChart.series[0].yData[index]]);
                }
            })
            const maskFill = this.getMarkFill()
            let textColor = '#f4f4f4'
            let lineColor = '#3d4a63'
            if (
                this.theme &&
                (this.theme == 'light' || this.theme == 'green')
            ) {
                textColor = '#595959'
                lineColor = '#e6e6e6'
            }
            if (detailsChart) {
                detailsChart.destroy();
            }
            detailsChart = Highcharts.chart('historyData-details', {
                chart: {
                    backgroundColor: 'transparent',
                    reflow: false,
                    marginTop: 20,
                    marginBottom: 60,
                    marginLeft: 50,
                    marginRight: 20,
                    style: {
                        position: 'absolute'
                    },
                    zoomType: 'x',
                    events: {
                        redraw: function (event) {
                            const xAxis = event.target.xAxis[0];
                            const { min, max } = xAxis
                            const zoomXAxis = timeZoomChart.xAxis[0]

                            const left = zoomXAxis.toPixels(min)
                            const right = zoomXAxis.toPixels(max)
                            timeZoomChart._navigator.setShade(left, right - left);
                        }
                    },
                    zooming: {
                        mouseWheel: {
                            enabled: true
                        },
                        resetButton: {
                            position: {
                                align: 'right',
                                x: 0,
                                verticalAlign: 'top',
                                y: 0
                            },
                            relativeTo: 'chart'
                        }
                    }
                },
                credits: {
                    enabled: false
                },
                title: {
                    text: null
                },
                xAxis: {
                    type: 'datetime',
                    labels: {
                        formatter: function (param) {
                            return Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', param.value).replace(' ', '<br />');
                        },
                        format: '{value:%Y-%m-%d %H:%M:%S}',
                        style: {
                            color: textColor
                        },
                    },
                    lineColor: lineColor,
                    tickColor: lineColor,
                },
                yAxis: {
                    title: {
                        text: null
                    },
                    gridLineColor: lineColor,
                    tickColor: lineColor,
                    labels: {
                        style: {
                            color: textColor
                        }
                    },
                    maxZoom: 0.1
                },
                tooltip: {
                    dateTimeLabelFormats: {
                        millisecond: '%Y-%m-%d %H:%M:%S.%L',
                        second: '%Y-%m-%d %H:%M:%S.%L'
                    }
                },
                plotOptions: {
                    series: {
                        marker: {
                            enabled: false,
                            states: {
                                hover: {
                                    enabled: true,
                                    radius: 3
                                }
                            }
                        }
                    }
                },
                legend: {
                    enabled: false
                },
                series: [{
                    name: this.currentSelect.ycyxName,
                    pointStart: detailStart,
                    data: detailData,
                    color: '#37a2da'
                }],
            })
            timeZoomChart._detailsChart = detailsChart
        },

        // 历时曲线---确定时间---获取开始、结束时间戳
        checkTime () {
            if (!this.chartHistoryTime || this.chartHistoryTime.length == 0) {
                this.dataHistory.beginTime = ''
                this.dataHistory.endTime = ''
                this.$message({
                    title: this.$t('equipListsIot.tips.timeHorizon'),
                    type: 'warning'
                })
                return false
            }
            this.dataHistory.beginTime = this.chartHistoryTime[0]
            this.dataHistory.endTime = this.chartHistoryTime[1]
            return true
        },
        closeDialog () {
            this.$emit('closeDialog', 'showHistoryDialog')
        }

    }

}
