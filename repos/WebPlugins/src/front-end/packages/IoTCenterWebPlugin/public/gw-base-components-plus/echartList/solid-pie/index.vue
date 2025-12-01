<template>
    <div class="solidChart" :style="[styleObject]">
        <div class="name">{{name}}</div>
        <div class="chartBox" :id='echartId'></div>
    </div>
</template>

<script>
export default {
    name: "cityGreenLand",
    components: {},
    props: {
        data: {
            type: Array,
            required: true
        },
        //图表名称
        name: {
            type: String,
            default: ''
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
            type: Object,
            default: () => {}
        }
    },
    data () {
        return {
            echartId: '',
            myChart: null,
            optionData: [{
                name: '林地面积统计',
                value: 10000,
                itemStyle: {
                    color: '#22c4ff',
                }
            }, {
                name: '草地面积统计',
                value: 12116,
                itemStyle: {
                    color: '#aaff00'
                }
            }, {
                name: '耕地地面积统计',
                value: 16616,
                itemStyle: {
                    color: '#ffaaff'
                }
            }],
            pieSeries: {
                name: 'pie2d',
                type: 'pie',
                labelLine: {
                    length: 10,
                    length2: 10
                },
                label: {
                    color: 'red',
                    show: true
                },
                startAngle: -20, //起始角度，支持范围[0, 360]。
                clockwise: false,//饼图的扇区是否是顺时针排布。上述这两项配置主要是为了对齐3d的样式
                radius: ['20%', '72%'],
                center: ['48%', '55%'],
                itemStyle: {
                    opacity: 1,
                }
            }
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
    created () {
        this.echartId = this.myUtils.generateUUID()
    },
    mounted () {
        this.init();
    },
    methods: {
        init () {
            this.data.sort((a, b) => {
                return (b.value - a.value);
            })
            //构建3d饼状图
            this.myChart = this.$echart.init(document.getElementById(this.echartId));
            // 传入数据生成 option
            this.option = this.getPie3D(this.data, 0);
            this.option = this.merge(this.option, this.layoutOptions)
            //是否需要label指引线，如果要就添加一个透明的2d饼状图并调整角度使得labelLine和3d的饼状图对齐，并再次setOption
            this.pieSeries.data = JSON.parse(JSON.stringify(this.data)).map(item => { item.itemStyle.color = 'transparent'; return item; })
            this.option.series.push(this.merge(this.pieSeries, (this.seriesOptions && this.seriesOptions.label) || {}));

            this.myChart.setOption(this.option);
            this.bindListen(this.myChart);

            this.myChart.on('legendselectchanged', (params) => {
                let copyData = JSON.parse(JSON.stringify(this.data));
                copyData.forEach(item => {
                    if (!params.selected[item.name]) item.value = 0;
                })
                this.option = this.getPie3D(copyData, 0);
                this.option = this.merge(this.option, this.layoutOptions)
                this.pieSeries.data = JSON.parse(JSON.stringify(copyData)).map(item => { item.itemStyle.color = 'transparent'; return item; })
                this.option.series.push(this.merge(this.pieSeries, (this.seriesOptions && this.seriesOptions.label) || {}));

                this.myChart.setOption(this.option);
            })
        },

        getPie3D (pieData, internalDiameterRatio) {
            //internalDiameterRatio:透明的空心占比
            let that = this;
            let series = [];
            let sumValue = 0;
            let startValue = 0;
            let endValue = 0;
            let legendData = [];
            let k = 1 - internalDiameterRatio;
            // 为每一个饼图数据，生成一个 series-surface 配置
            for (let i = 0; i < pieData.length; i++) {
                sumValue += pieData[i].value;
                let seriesItem = {
                    name: typeof pieData[i].name === 'undefined' ? `series${i}` : pieData[i].name,
                    type: 'surface',
                    parametric: true,
                    wireframe: {
                        show: false
                    },
                    pieData: pieData[i],
                    pieStatus: {
                        selected: false,
                        hovered: false,
                        k: k
                    },
                    center: ['10%', '50%']
                };
                this.merge(seriesItem, (this.seriesOptions && this.seriesOptions.surface) || {});

                if (typeof pieData[i].itemStyle != 'undefined') {
                    let itemStyle = {};
                    typeof pieData[i].itemStyle.color != 'undefined' ? itemStyle.color = pieData[i].itemStyle.color : null;
                    typeof pieData[i].itemStyle.opacity != 'undefined' ? itemStyle.opacity = pieData[i].itemStyle.opacity : null;
                    seriesItem.itemStyle = itemStyle;
                }
                series.push(seriesItem);
            }

            // 使用上一次遍历时，计算出的数据和 sumValue，调用 getParametricEquation 函数，
            // 向每个 series-surface 传入不同的参数方程 series-surface.parametricEquation，也就是实现每一个扇形。
            legendData = [];
            for (let i = 0; i < series.length; i++) {
                endValue = startValue + series[i].pieData.value;
                series[i].pieData.startRatio = startValue / sumValue;
                series[i].pieData.endRatio = endValue / sumValue;
                series[i].parametricEquation = this.getParametricEquation(series[i].pieData.startRatio, series[i].pieData.endRatio,
                    false, false, k, series[i].pieData.value);
                startValue = endValue;
                let bfb = that.fomatFloat(series[i].pieData.value / sumValue, 4);
                legendData.push({
                    name: series[i].name,
                    value: bfb
                });
            }
            let boxHeight = this.getHeight3D(series, 26);//通过传参设定3d饼/环的高度，26代表26px
            // 准备待返回的配置项，把准备好的 legendData、series 传入。
            let option = {
                legend: {
                    data: legendData,
                    orient: 'horizontal',
                    left: 10,
                    top: 10,
                    itemGap: 10,
                    textStyle: {
                        color: '#A1E2FF',
                    },
                    show: this.showLegend,
                    icon: "circle",
                    formatter: function (param) {
                        let item = legendData.filter(item => item.name == param)[0];
                        let bfs = that.fomatFloat(item.value * 100, 2) + "%";
                        return `${item.name}  ${bfs}`;
                    }
                },
                labelLine: {
                    show: true,
                    lineStyle: {
                        color: '#7BC0CB'
                    }
                },
                label: {
                    show: true,
                    position: 'outside',
                    rich: {
                        b: {
                            color: '#7BC0CB',
                            fontSize: 12,
                            lineHeight: 20
                        },
                        c: {
                            fontSize: 16,
                        },
                    },
                    formatter: '{b|{b} \n}{c|{c}}{b|  亩}',

                },
                tooltip: {
                    formatter: params => {
                        if (params.seriesName !== 'mouseoutSeries' && params.seriesName !== 'pie2d') {
                            let bfb = ((option.series[params.seriesIndex].pieData.endRatio - option.series[params.seriesIndex].pieData.startRatio) *
                                100).toFixed(2);
                            return `${params.seriesName}<br/>` +
                                `<span style="display:inline-block;margin-right:5px;border-radius:10px;width:10px;height:10px;background-color:${params.color};"></span>` +
                                `${bfb}%`;
                        }
                    }
                },
                xAxis3D: {
                    min: -1,
                    max: 1
                },
                yAxis3D: {
                    min: -1,
                    max: 1
                },
                zAxis3D: {
                    min: -1,
                    max: 1
                },
                grid3D: {
                    show: false,
                    boxHeight: boxHeight, //圆环的高度
                    viewControl: { //3d效果可以放大、旋转等，请自己去查看官方配置
                        alpha: 40, //角度
                        distance: 300,//调整视角到主体的距离，类似调整zoom
                        rotateSensitivity: 0, //设置为0无法旋转
                        zoomSensitivity: 0, //设置为0无法缩放
                        panSensitivity: 0, //设置为0无法平移
                        autoRotate: false //自动旋转
                    }
                },
                series: series
            };
            return option;
        },

        //获取3d丙图的最高扇区的高度
        getHeight3D (series, height) {
            let max = 0;
            for (let item of series) {
                max = item.pieData.value > max ? item.pieData.value : max;
            }
            return height * 25 / max;
        },

        // 生成扇形的曲面参数方程，用于 series-surface.parametricEquation
        getParametricEquation (startRatio, endRatio, isSelected, isHovered, k, h) {
            // 计算
            let midRatio = (startRatio + endRatio) / 2;
            let startRadian = startRatio * Math.PI * 2;
            let endRadian = endRatio * Math.PI * 2;
            let midRadian = midRatio * Math.PI * 2;
            // 如果只有一个扇形，则不实现选中效果。
            if (startRatio === 0 && endRatio === 1) {
                isSelected = false;
            }
            // 通过扇形内径/外径的值，换算出辅助参数 k（默认值 1/3）
            k = typeof k !== 'undefined' ? k : 1 / 3;
            // 计算选中效果分别在 x 轴、y 轴方向上的位移（未选中，则位移均为 0）
            let offsetX = isSelected ? Math.cos(midRadian) * 0.1 : 0;
            let offsetY = isSelected ? Math.sin(midRadian) * 0.1 : 0;
            // 计算高亮效果的放大比例（未高亮，则比例为 1）
            let hoverRate = isHovered ? 1.05 : 1;
            // 返回曲面参数方程
            return {
                u: {
                    min: -Math.PI,
                    max: Math.PI * 3,
                    step: Math.PI / 32
                },
                v: {
                    min: 0,
                    max: Math.PI * 2,
                    step: Math.PI / 20
                },
                x: function (u, v) {
                    if (u < startRadian) {
                        return offsetX + Math.cos(startRadian) * (1 + Math.cos(v) * k) * hoverRate;
                    }
                    if (u > endRadian) {
                        return offsetX + Math.cos(endRadian) * (1 + Math.cos(v) * k) * hoverRate;
                    }
                    return offsetX + Math.cos(u) * (1 + Math.cos(v) * k) * hoverRate;
                },
                y: function (u, v) {
                    if (u < startRadian) {
                        return offsetY + Math.sin(startRadian) * (1 + Math.cos(v) * k) * hoverRate;
                    }
                    if (u > endRadian) {
                        return offsetY + Math.sin(endRadian) * (1 + Math.cos(v) * k) * hoverRate;
                    }
                    return offsetY + Math.sin(u) * (1 + Math.cos(v) * k) * hoverRate;
                },
                z: function (u, v) {
                    if (u < -Math.PI * 0.5) {
                        return Math.sin(u);
                    }
                    if (u > Math.PI * 2.5) {
                        return Math.sin(u) * h * .1;
                    }
                    return Math.sin(v) > 0 ? 1 * h * .1 : -1;
                }
            };
        },

        fomatFloat (num, n) {
            var f = parseFloat(num);
            if (isNaN(f)) {
                return false;
            }
            f = Math.round(num * Math.pow(10, n)) / Math.pow(10, n); // n 幂   
            var s = f.toString();
            var rs = s.indexOf('.');
            //判定如果是整数，增加小数点再补0
            if (rs < 0) {
                rs = s.length;
                s += '.';
            }
            while (s.length <= rs + n) {
                s += '0';
            }
            return s;
        },

        bindListen (myChart) {
            // 监听鼠标事件，实现饼图选中效果（单选），近似实现高亮（放大）效果。
            let that = this;
            let selectedIndex = '';
            let hoveredIndex = '';
            myChart.on('mouseover', function (params) {
                // 准备重新渲染扇形所需的参数
                let isSelected;
                let isHovered;
                let startRatio;
                let endRatio;
                let k;
                // 如果触发 mouseover 的扇形当前已高亮，则不做操作
                if (hoveredIndex === params.seriesIndex) {
                    return;
                    // 否则进行高亮及必要的取消高亮操作
                } else {
                    // 如果当前有高亮的扇形，取消其高亮状态（对 option 更新）
                    if (hoveredIndex !== '') {
                        // 从 option.series 中读取重新渲染扇形所需的参数，将是否高亮设置为 false。
                        isSelected = that.option.series[hoveredIndex].pieStatus.selected;
                        isHovered = false;
                        startRatio = that.option.series[hoveredIndex].pieData.startRatio;
                        endRatio = that.option.series[hoveredIndex].pieData.endRatio;
                        k = that.option.series[hoveredIndex].pieStatus.k;
                        // 对当前点击的扇形，执行取消高亮操作（对 option 更新）
                        that.option.series[hoveredIndex].parametricEquation = that.getParametricEquation(startRatio, endRatio,
                            isSelected,
                            isHovered, k, that.option.series[hoveredIndex].pieData.value);
                        that.option.series[hoveredIndex].pieStatus.hovered = isHovered;
                        // 将此前记录的上次选中的扇形对应的系列号 seriesIndex 清空
                        hoveredIndex = '';
                    }
                    // 如果触发 mouseover 的扇形不是透明圆环，将其高亮（对 option 更新）
                    if (params.seriesName !== 'mouseoutSeries' && params.seriesName !== 'pie2d') {
                        // 从 option.series 中读取重新渲染扇形所需的参数，将是否高亮设置为 true。
                        isSelected = that.option.series[params.seriesIndex].pieStatus.selected;
                        isHovered = true;
                        startRatio = that.option.series[params.seriesIndex].pieData.startRatio;
                        endRatio = that.option.series[params.seriesIndex].pieData.endRatio;
                        k = that.option.series[params.seriesIndex].pieStatus.k;
                        // 对当前点击的扇形，执行高亮操作（对 option 更新）
                        that.option.series[params.seriesIndex].parametricEquation = that.getParametricEquation(startRatio, endRatio,
                            isSelected, isHovered, k, that.option.series[params.seriesIndex].pieData.value + 5);
                        that.option.series[params.seriesIndex].pieStatus.hovered = isHovered;
                        // 记录上次高亮的扇形对应的系列号 seriesIndex
                        hoveredIndex = params.seriesIndex;
                    }
                    // 使用更新后的 option，渲染图表
                    myChart.setOption(that.option);
                }
            });
            // 修正取消高亮失败的 bug
            myChart.on('globalout', function () {
                // 准备重新渲染扇形所需的参数
                let isSelected;
                let isHovered;
                let startRatio;
                let endRatio;
                let k;
                if (hoveredIndex !== '') {
                    // 从 option.series 中读取重新渲染扇形所需的参数，将是否高亮设置为 true。
                    isSelected = that.option.series[hoveredIndex].pieStatus.selected;
                    isHovered = false;
                    k = that.option.series[hoveredIndex].pieStatus.k;
                    startRatio = that.option.series[hoveredIndex].pieData.startRatio;
                    endRatio = that.option.series[hoveredIndex].pieData.endRatio;
                    // 对当前点击的扇形，执行取消高亮操作（对 option 更新）
                    that.option.series[hoveredIndex].parametricEquation = that.getParametricEquation(startRatio, endRatio,
                        isSelected,
                        isHovered, k, that.option.series[hoveredIndex].pieData.value);
                    that.option.series[hoveredIndex].pieStatus.hovered = isHovered;
                    // 将此前记录的上次选中的扇形对应的系列号 seriesIndex 清空
                    hoveredIndex = '';
                }
                // 使用更新后的 option，渲染图表
                myChart.setOption(that.option);
            });
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