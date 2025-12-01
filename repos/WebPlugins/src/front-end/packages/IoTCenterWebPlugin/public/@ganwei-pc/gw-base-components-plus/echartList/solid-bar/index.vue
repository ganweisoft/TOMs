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
            default: () => { return {} }
        },
        seriesOptions: {
            type: Object,
            default: () => { return {} }
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
    data () {
        return {
            echartId: '',
            myChart: null,
            option: {
                backgroundColor: 'transparent',
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {
                        type: 'shadow'
                    },
                    formatter: function (params, ticket, callback) {
                        const item = params[1];
                        return item.name + ' : ' + item.value;
                    }
                },
                grid: {
                    left: '2%',
                    right: '10%',
                    top: '20%',
                    bottom: '5%',
                    containLabel: true,
                },
                xAxis: {
                    type: 'category',
                    data: this.xData,
                    axisLine: {
                        show: true,
                        lineStyle: {
                            width: 2,
                            color: '#2B7BD6',
                        },
                    },
                    axisTick: {
                        show: false,
                    },
                    axisLabel: {
                        fontSize: 14,
                    },
                },
                yAxis: {
                    type: 'value',
                    axisLine: {
                        show: true,
                        lineStyle: {
                            width: 2,
                            color: '#2B7BD6',
                        },
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: '#153D7D',
                        },
                    },
                    axisTick: {
                        show: false,
                    },
                    axisLabel: {
                        fontSize: 14,
                    },
                    // boundaryGap: ['20%', '20%'],
                },
                legend: {
                    orient: 'horizontal',
                    textStyle: {
                        color: '#A1E2FF'
                    },
                    show: this.showLegend,
                    icon: 'circle'
                }
            },
            customSeries: {
                type: 'custom',
                renderItem: (params, api) => {
                    const location = api.coord([api.value(0), api.value(1)]);
                    return {
                        type: 'group',
                        children: [
                            {
                                type: 'CubeLeft',
                                shape: {
                                    api,
                                    xValue: api.value(0),
                                    yValue: api.value(1),
                                    x: location[0],
                                    y: location[1],
                                    xAxisPoint: api.coord([api.value(0), 0]),
                                },
                                style: {
                                    fill: (this.seriesOptions['color'] && this.seriesOptions['color']['CubeLeft']) || new this.$echart.graphic.LinearGradient(0, 0, 0, 1, [
                                        {
                                            offset: 0,
                                            color: '#33BCEB',
                                        },
                                        {
                                            offset: 1,
                                            color: '#337CEB',
                                        },
                                    ]),
                                },
                            },
                            {
                                type: 'CubeRight',
                                shape: {
                                    api,
                                    xValue: api.value(0),
                                    yValue: api.value(1),
                                    x: location[0],
                                    y: location[1],
                                    xAxisPoint: api.coord([api.value(0), 0]),
                                },
                                style: {
                                    fill: (this.seriesOptions['color'] && this.seriesOptions['color']['CubeRight']) || new this.$echart.graphic.LinearGradient(0, 0, 0, 1, [
                                        {
                                            offset: 0,
                                            color: '#28A2CE',
                                        },
                                        {
                                            offset: 1,
                                            color: '#1A57B7',
                                        },
                                    ]),
                                },
                            },
                            {
                                type: 'CubeTop',
                                shape: {
                                    api,
                                    xValue: api.value(0),
                                    yValue: api.value(1),
                                    x: location[0],
                                    y: location[1],
                                    xAxisPoint: api.coord([api.value(0), 0]),
                                },
                                style: {
                                    fill: (this.seriesOptions['color'] && this.seriesOptions['color']['CubeTop']) || new this.$echart.graphic.LinearGradient(0, 0, 0, 1, [
                                        {
                                            offset: 0,
                                            color: '#43C4F1',
                                        },
                                        {
                                            offset: 1,
                                            color: '#28A2CE',
                                        },
                                    ]),
                                },
                            },
                        ],
                    };
                },
                data: this.yData
            },
            labelSeries: {
                type: 'bar',
                label: {
                    normal: {
                        show: true,
                        position: 'top',
                        formatter: (e) => {
                            return e.value + this.unit;
                        },
                        fontSize: 16,
                        color: '#43C4F1',
                        offset: [0, -25]
                    }
                },
                itemStyle: {
                    color: 'transparent'
                },
                tooltip: {},
                data: this.yData
            }
        }
    },
    created () {
        this.echartId = this.myUtils.generateUUID()
    },
    mounted () {
        const offsetX = 20;
        const offsetY = 10;
        // 绘制左侧面
        const CubeLeft = this.$echart.graphic.extendShape({
            shape: {
                x: 0,
                y: 0,
            },
            buildPath: function (ctx, shape) {
                // 会canvas的应该都能看得懂，shape是从custom传入的
                const xAxisPoint = shape.xAxisPoint;
                const c0 = [shape.x, shape.y];
                const c1 = [shape.x - offsetX, shape.y - offsetY];
                const c2 = [xAxisPoint[0] - offsetX, xAxisPoint[1] - offsetY];
                const c3 = [xAxisPoint[0], xAxisPoint[1]];
                ctx.moveTo(c0[0], c0[1]).lineTo(c1[0], c1[1]).lineTo(c2[0], c2[1]).lineTo(c3[0], c3[1]).closePath();
            },
        });
        // 绘制右侧面
        const CubeRight = this.$echart.graphic.extendShape({
            shape: {
                x: 0,
                y: 0,
            },
            buildPath: function (ctx, shape) {
                const xAxisPoint = shape.xAxisPoint;
                const c1 = [shape.x, shape.y];
                const c2 = [xAxisPoint[0], xAxisPoint[1]];
                const c3 = [xAxisPoint[0] + offsetX, xAxisPoint[1] - offsetY];
                const c4 = [shape.x + offsetX, shape.y - offsetY];
                ctx.moveTo(c1[0], c1[1]).lineTo(c2[0], c2[1]).lineTo(c3[0], c3[1]).lineTo(c4[0], c4[1]).closePath();
            },
        });
        // 绘制顶面
        const CubeTop = this.$echart.graphic.extendShape({
            shape: {
                x: 0,
                y: 0,
            },
            buildPath: function (ctx, shape) {
                const c1 = [shape.x, shape.y];
                const c2 = [shape.x + offsetX, shape.y - offsetY]; //右点
                const c3 = [shape.x, shape.y - offsetX];
                const c4 = [shape.x - offsetX, shape.y - offsetY];
                ctx.moveTo(c1[0], c1[1]).lineTo(c2[0], c2[1]).lineTo(c3[0], c3[1]).lineTo(c4[0], c4[1]).closePath();
            },
        });
        // 注册三个面图形
        this.$echart.graphic.registerShape('CubeLeft', CubeLeft);
        this.$echart.graphic.registerShape('CubeRight', CubeRight);
        this.$echart.graphic.registerShape('CubeTop', CubeTop)

        this.drawEchart()
    },
    methods: {
        drawEchart () {
            let nc = document.getElementById(this.echartId);
            this.myChart = this.$echart.init(nc);

            this.option = Object.assign({}, this.option, this.layoutOptions)

            this.merge(this.labelSeries.label, this.seriesOptions['label'] || {})

            this.option.series = [this.customSeries, this.labelSeries];

            // 使用刚指定的配置项和数据显示图表。
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
}
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