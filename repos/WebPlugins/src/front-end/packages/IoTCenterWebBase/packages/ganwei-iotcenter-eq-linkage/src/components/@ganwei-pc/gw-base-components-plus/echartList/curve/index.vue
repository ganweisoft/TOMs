<template>
    <div class="solidChart" :style="[styleObject]">
        <div class="name">{{name}}</div>
        <div class="chartBox" :id='echartId'></div>
    </div>
</template>

<script>
export default {
    props: {
        name: {
            type: String,
            default: ''
        },
        realTime: {
            type: Boolean,
            default: false
        },
        xData: {
            type: Array,
            default: () => []
        },
        yData: {
            type: Array,
            default: () => []
        },
        unit: {
            type: String,
            default: ''
        },
        width: {
            type: String,
            default: '100%'
        },
        height: {
            type: String,
            default: '100%'
        },
        showLegend: {
            type: Boolean,
            default: false
        },
        layoutOptions: {
            type: Object,
            default: () => {}
        },
        seriesOptions: {
            type: Array,
            default: () => []
        }
    },
    computed: {
        styleObject () {
            return {
                width: this.width,
                height: this.height
            }
        }
    },
    watch: {
        xData (val) {
            this.option.xAxis.data = val;
            this.myChart.setOption(this.option)
        },
        yData (val) {
            val.forEach((item, index) => {
                let temp = Object.assign({}, this.seriesBasic)
                temp.data = item;
                this.option.series[index].data = item;
            })
            this.myChart.setOption(this.option)
        }
    },
    data () {
        return {
            echartId: '',
            myChart: null,
            option: {
                color: ['#5470c6', '#91cc75', '#fac858', '#ee6666', '#73c0de', '#3ba272', '#fc8452', '#9a60b4', '#ea7ccc'],
                grid: {
                    left: '30px',
                    top: '10%',
                    right: '55px',
                    bottom: '11%',
                    containLabel: true,
                    color: 'red'
                },
                tooltip: {
                    style: {
                        color: 'red'
                    }
                },
                xAxis: {
                    boundaryGap: false,
                    splitLine: {
                        show: false
                    },
                    axisLine: {
                        lineStyle: {
                            color: (window.localStorage.getItem('theme') && window.localStorage.getItem('theme') == 'light') ? '#e6e6e6' : '#999fa8',
                            width: 1 // 这里是为了突出显示加上的
                        }
                    },
                    axisLabel: {
                        margin: 12,
                        textStyle: {
                            color: (window.localStorage.getItem('theme') && window.localStorage.getItem('theme') == 'light') ? '#595959' : '#d3d8e2',
                            fontSize: '13'
                        }
                    },
                    labels: {
                        style: {
                            color: '#d3d8e2'
                        }
                    },
                    style: {
                        color: '#d3d8e2'
                    },

                    data: [1, 2, 3, 4, 5, 6]
                },
                yAxis: {
                    gridLineColor: '#3b4357',
                    gridLineWidth: 1,
                    type: 'value',
                    axisLine: {
                        show: false
                    },
                    axisTick: {
                        show: false
                    },
                    axisLabel: {
                        margin: 12,
                        textStyle: {
                            color: (window.localStorage.getItem('theme') && window.localStorage.getItem('theme') == 'light') ? '#595959' : '#d3d8e2',
                            fontSize: '13'
                        }
                    },
                    style: {
                        color: '#d3d8e2',
                        fontWeight: 'bold',
                        fontSize: '12px',
                        fontFamily: 'Trebuchet MS, Verdana, sans-serif'
                    },

                    title: {
                        style: {
                            color: '#d3d8e2',
                            fontWeight: 'bold',
                            fontSize: '12px',
                            fontFamily: 'Trebuchet MS, Verdana, sans-serif'
                        }
                    },
                    labels: {
                        style: {
                            color: '#d3d8e2'
                        }
                    }
                },
                legend: {
                    orient: 'horizontal',
                    textStyle: {
                        color: '#A1E2FF'
                    },
                    show: this.showLegend,
                    icon: 'circle'
                },
                series: []
            },
            seriesBasic: {
                type: 'line',
                stack: 'Total',
                emphasis: {
                    lineStyle: {
                        width: 2
                    }
                },
                smooth: true,
                sampling: 'average',
                lineStyle: {
                    width: 2
                }
            }
        }
    },
    created () {
        this.echartId = this.myUtils.generateUUID()
    },
    mounted () {
        this.drawEchart()
    },
    methods: {
        drawEchart() {
            let nc = document.getElementById(this.echartId);
            this.myChart = this.$echart.init(nc);
            this.option = Object.assign({}, this.option, this.layoutOptions)
            this.yData.forEach((item, index) => {
                let customSeries = this.seriesOptions[index] ? this.seriesOptions[index] : {};
                let temp = this.merge(Object.assign({}, this.seriesBasic), customSeries)
                temp.data = item;
                this.option.series.push(temp)
            })
            this.myChart.setOption(this.option);
        },
        merge(defaultOp, customOp) {
            if (typeof customOp === 'object' && !Array.isArray(customOp)) {
                for (let key of Object.keys(customOp)) {
                    if (!defaultOp[key] || (typeof defaultOp[key] !== 'object') || Array.isArray(customOp[key])) {
                        defaultOp[key] = customOp[key];
                    } else {
                        this.merge(defaultOp[key], customOp[key])
                    }
                }
            }
            return defaultOp;
        }
    }
};
</script>
<style lang='scss' scoped>
.solidChart {
    color: white;
    .name {
        height: 40px;
        font-size: 18px;
    }
    .chartBox {
        width: 100%;
        height: calc(100% - 40px);
        div,
        canvas {
            width: 100%;
            height: 100% !important;
        }
    }
}
</style>