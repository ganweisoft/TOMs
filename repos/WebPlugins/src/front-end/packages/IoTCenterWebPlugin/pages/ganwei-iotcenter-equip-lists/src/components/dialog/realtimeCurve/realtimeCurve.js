/*
 * @Author: Copyright (c) 2020-2025 Shenzhen Ganwei Software Technology Co., Ltd
 * @Date: 2022-08-16 11:21:54
 * @FilePath: ganwei-iotcenter-equip-lists\src\components\dialog\realtimeCurve\realtimeCurve.js
 * @Description: 实时曲线
 */
export default {
    props: {
        showDialog: {
            type: Boolean,
            default: false
        },
        currentSelect: {
            type: Object,
            default: () => { }
        },
        realTimeData: {
            type: Array,
            default: () => []
        }
    },
    data () {
        return {
            realTimeEchart: null,
            maxValues: [],
            minValues: [],
            xAxisValues: [],
            yAxisValues: [],
            currentMaxValue: '',
            currentminValue: ''
        }
    },
    watch: {
        realTimeData (newVal) {
            if (!this.realTimeEchart) {
                return
            }
            let itemData = newVal[newVal.length - 1];
            let minValue = 0
            let maxValue = 0
            this.xAxisValues.push(itemData.time);
            this.yAxisValues.push(itemData.value);

            this.maxValues.push(itemData.valMax);
            this.minValues.push(itemData.valMin);

            // 设置y轴最大值最小值
            if (itemData.value > itemData.valMax) {
                minValue = this.currentSelect.ycyxType === 'C' ? Math.ceil(itemData.valMin * 0.8) : 2
                maxValue = this.currentSelect.ycyxType === 'C' ? Math.ceil(itemData.value * 1.08) : 0
            } else if (itemData.value < itemData.valMin) {
                if (itemData.value >= 0) {
                    minValue = this.currentSelect.ycyxType === 'C' ? Math.ceil(itemData.value * 0.8) : 2
                } else {
                    minValue = this.currentSelect.ycyxType === 'C' ? Math.ceil(itemData.value * 1.2) : 2
                }
                maxValue = this.currentSelect.ycyxType === 'C' ? Math.ceil(itemData.valMax * 1.08) : 0
            } else {
                minValue = this.currentSelect.ycyxType === 'C' ? Math.ceil(itemData.valMin * 0.8) : 2
                maxValue = this.currentSelect.ycyxType === 'C' ? Math.ceil(itemData.valMax * 1.08) : 0
            }
            this.currentMaxValue = maxValue
            this.currentminValue = minValue

            // 取20个实时数据进行展示
            if (this.maxValues.length > 21) {
                this.maxValues.splice(0, 1)
                this.minValues.splice(0, 1)
                this.xAxisValues.splice(0, 1)
                this.yAxisValues.splice(0, 1)
            }

            this.$nextTick(() => {
                // this.realTimeEchart.clear()
                // this.realTimeEchart = ''
                this.drawEchart()
            })
        }
    },
    mounted () {
        this.$nextTick(() => {
            this.initEchart()
            this.drawEchart(true)
        })
    },
    methods: {
        initEchart () {
            this.realTimeEchart = this.$echart.init(
                document.getElementById('containerE'),
                'light'
            )
            let legendData = [];

            let textColor = '#f4f4f4'
            let lineColor = '#343a4c'
            if (
                window.localStorage.getItem('theme') && (
                window.localStorage.getItem('theme') == 'light' || window.localStorage.getItem('theme') == 'green')
            ) {
                textColor = '#595959'
                lineColor = '#e6e6e6'
            }

            if (this.currentSelect.ycyxType == 'C') {
                legendData = [
                    this.legendTitleRight(this.$t('equipListsIot.echart.legend.realTimeValue'), textColor),
                    this.legendTitleRight(this.$t('equipListsIot.echart.legend.upperLimitValue'), textColor),
                    this.legendTitleRight(this.$t('equipListsIot.echart.legend.lowerLimitValue'), textColor)
                ]
            } else {
                legendData = [this.legendTitleRight(this.$t('equipListsIot.echart.legend.realTimeValue'), textColor)]
            }
            this.realTimeEchart.setOption({
                backgroundColor: 'transparent',
                grid: {
                    left: '20px',
                    top: '10%',
                    right: '40px',
                    bottom: '20px',
                    containLabel: true
                },
                tooltip: {
                    trigger: 'axis',
                    formatter: (params) => {
                        params = params[0]
                        return (
                            `<span style="display:inline-block;min-width: 56px;text-align: right;">${this.$t('equipListsIot.echart.toolTip.time')}：</span>` +
                            params.axisValue +
                            `<br/>` +
                            `<span style="display:inline-block;min-width: 56px;text-align: right;">${this.$t('equipListsIot.echart.toolTip.currentValue')}：</span>` +
                            '<span style="color:#3875ff">' +
                            params.value +
                            '</span>'
                        )
                    },
                },
                legend: {
                    show: true,
                    orient: 'horizontal', // 'vertical'
                    x: 'right',
                    // y: 'top',
                    right: '20',
                    padding: [10, 20, 0, 0],
                    top: '7',
                    data: legendData,
                    textStyle: {
                        color: 'red'
                    }
                },
                xAxis: {
                    lineColor: '#999fa8',
                    // type: 'time',
                    boundaryGap: false,
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: lineColor,
                            width: 1
                        }
                    },
                    labels: {
                        style: {
                            color: '#595959'
                        }
                    },
                    axisLine: {
                        lineStyle: {
                            type: 'solid',
                            color: '#9197a0', // 左边线的颜色
                            width: '1' // 坐标线的宽度
                        }
                    },
                    axisLabel: {
                        show: true,
                        onZero: false,
                        textStyle: {
                            color: textColor // 更改坐标轴文字颜色
                        }
                    },
                    data: this.xAxisValues,
                    animation: false,
                },
                yAxis: {
                    type: 'value',
                    boundaryGap: false,
                    scale: true,
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: lineColor,
                            width: 1
                        }
                    },
                    axisLine: {
                        show: true,
                        onZero: false,
                        lineStyle: {
                            type: 'solid',
                            color: '#9197a0', // 左边线的颜色
                            width: '1' // 坐标线的宽度
                        }
                    },
                    axisLabel: {
                        show: true,
                        textStyle: {
                            color: textColor // 更改坐标轴文字颜色
                        }
                    },
                    axisTick: {
                        show: true
                    },
                    animation: false,
                },
            })
        },
        drawEchart (init) {
            if (init) {
                this.initData()
            }
            this.initEchart()

            let seriesList = [
                {
                    type: 'line',
                    name: this.$t('equipListsIot.echart.legend.realTimeValue'),
                    color: '#3875ff',
                    itemStyle: {
                        normal: {
                            lineStyle: {
                                width: 1//设置线条粗细
                            }
                        }
                    },
                    smooth: true,
                    style: {
                        color: '#d3d8e2'
                    },
                    animation: false,
                    showSymbol: true,
                    symbolSize: 10,
                    data: this.getRenderData(this.yAxisValues),
                }
            ]

            if (this.currentSelect.ycyxType === 'C') {
                seriesList.push(
                    {
                        name: this.$t('equipListsIot.echart.legend.upperLimitValue'),
                        type: 'line',
                        // smooth: true,
                        color: 'rgba(242,36,51,0.6)',
                        style: {
                            color: '#d3d8e2'
                        },
                        showSymbol: false,
                        data: this.maxValues,
                        itemStyle: {
                            normal: {
                                lineStyle: {
                                    width: 2,
                                    type: 'dotted' // 'dotted'虚线 'solid'实线
                                }
                            }
                        },
                        animationDuration: 100
                    },
                    {
                        name: this.$t('equipListsIot.echart.legend.lowerLimitValue'),
                        type: 'line',
                        // smooth: true,
                        color: 'rgba(245,187,54,0.6)',
                        style: {
                            color: '#d3d8e2'
                        },
                        showSymbol: false,
                        // hoverAnimation: true,
                        data: this.minValues,
                        itemStyle: {
                            normal: {
                                lineStyle: {
                                    width: 2,
                                    type: 'dotted' // 'dotted'虚线 'solid'实线
                                }
                            }
                        },
                        animationDuration: 100

                    }
                )
            }
            this.realTimeEchart.setOption({
                xAxis: {
                    data: this.xAxisValues
                },
                series: seriesList,
            })
        },

        processBreakData () {

        },
        getRenderData (data) {
            const renderData = []
            let preData = null
            for (let i = 0; i < data.length; i++) {
                renderData.push({
                    value: data[i],
                    symbol: preData !== data[i] || data[i] !== data[i + 1] ? 'circle' : 'none',
                })
                preData = data[i]
            }
            return renderData
        },

        legendTitleRight (title, textColor) {
            return {
                name: title,
                textStyle: {
                    fontSize: 12,
                    color: textColor
                }
            }

        },


        initData () {
            if (this.realTimeData.length > 0) {
                let itemData = this.realTimeData[this.realTimeData.length - 1];
                this.maxValues.push(itemData.valMax);
                this.minValues.push(itemData.valMin);
                this.xAxisValues.push(itemData.time);
                this.yAxisValues.push(itemData.value);
            }
        },

        closeDialog () {
            this.$emit('closeDialog', 'showRealtimeCurveDialog')
        }
    }
}
