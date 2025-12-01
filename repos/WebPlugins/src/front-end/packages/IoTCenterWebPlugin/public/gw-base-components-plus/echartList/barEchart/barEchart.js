/** @file 组件 **/
import myUtils from 'gw-base-utils-plus/myUtils';
function debounce (func, wait) {
    let timeout;
    return function (event) {
        let _this = this
        let args = arguments;
        if (timeout) {
            clearTimeout(timeout)
        }
        timeout = setTimeout(() => {
            timeout = null
            func.call(_this, args)
        }, wait)
    }
}
export default {
    props: {
        status: {
            type: Boolean,
            default: true
        },
        echartData: {
            type: Object,
            default: () => { }
        },
        yUnit: {
            type: String,
            default: '℃'
        },
        colorList: {
            type: Array,
            default: () => ['#00f0ff', '#008af5']
        }
    },
    watch: {
        'echartData': function (newVal) {
            if (this.echartData.length > 0) {
                this.setEchartData()
                this.drawEchart()
            }
        },
        status: function (newVal) {
            this.loading = newVal;
            this.setEchartData()
            this.drawEchart()
        }
    },
    data () {
        return {
            loading: true,
            echartId: -1,
            xData: [],
            echart: '',
            reflesh: true
        }
    },
    created () {
        this.echartId = myUtils.generateUUID();
    },
    mounted () {
        window.addEventListener('resize', debounce(() => {
            this.echart.resize();
        }, 500));
        this.setEchartData()
        this.drawEchart()
        this.loading = this.status
    },
    methods: {
        setEchartData () {
            this.echartData.series.forEach((item, index) => {
                item.type = 'bar';
                item.barWidth = '50%';
                item.label = {
                    show: true,
                    position: 'top',
                    color: this.colorList[0]
                }
            });
        },
        drawEchart () {
            let option = {
                color: this.colorList,
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {
                        type: 'shadow'
                    }
                },
                legend: {
                    show: false
                },
                xAxis: {
                    type: 'category',
                    data: this.echartData.xData,
                    axisLabel: {
                        color: (window.localStorage.getItem('theme') && window.localStorage.getItem('theme') == 'light') ? '#595959' : '#d3d8e2',
                        fontSize: myUtils.fontSize(12, this.echartId)
                    },
                    axisLine: {
                        lineStyle: {
                            color: '#8c9097'
                        }
                    },
                    axisTick: {
                        show: false
                    }
                },
                yAxis: {

                    name: '(' + this.yUnit + ')',
                    nameTextStyle: {
                        color: '#999fa8',
                        padding: [0, 0, 5, -30]
                    },
                    axisLabel: {
                        fontSize: myUtils.fontSize(12, this.echartId)
                    },
                    axisLine: { show: false }
                },

                series: this.echartData.series,
                grid: {
                    x: '8%',
                    y: '20%',
                    x2: '4%',
                    y2: '15%'
                },
                backgroundColor: 'transparent'
            };
            this.echart = this.$echart.init(document.getElementById(this.echartId));
            this.echart.setOption(option);
        }
    }
}