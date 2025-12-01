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
        historyTime: {
            type: Array,
            default: () => []
        },
        historyValue: {
            type: Array,
            default: () => []
        },
        status: {
            type: Boolean,
            default: true
        },
        reflesh: {
            type: Boolean,
            default: true
        },
        refleshExtream: {
            type: Function,
            default: () => { }
        },
        resize: {
            type: Boolean,
            default: false
        }
    },
    watch: {
        status: function (newVal) {
            if (this.historyValue.length > 0) {
                this.onInquire();
            }
        },
        reflesh: function (newVal) {
            this.onInquire()
        },
        resize () {
            if (this.echart) {
                this.echart.resize();
            }
        }
    },

    data () {
        return {
            echart: null,
            echartId: '',
            zoomLock: false
            // reflesh: true
        };
    },
    created () {
        this.echartId = myUtils.generateUUID();
    },
    destroyed () {
        window.removeEventListener('resize', this.echart.resize())
    },
    mounted () {
        window.addEventListener('resize', debounce(() => {
            if (this.echart) {
                this.echart.resize();
            }
        }, 500));

        this.onInquire()
    },
    methods: {

        // 历史曲线
        onInquire () {
            if (this.historyValue.length > 0) {
                setTimeout(() => {
                    this.echart = this.$echart.init(
                        document.getElementById(this.echartId),
                        'light'
                    );

                    let option = {
                        grid: {
                            left: '30px',
                            top: '10%',
                            right: '55px',
                            bottom: '14%',
                            containLabel: true,
                            color: 'red'
                        },
                        tooltip: {
                            style: {
                                color: 'red'
                            }
                        },

                        xAxis: {
                            type: 'category',
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

                            data: this.historyTime
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
                        dataZoom: [
                            {
                                type: 'slider',
                                show: true,
                                xAxisIndex: [0],
                                start: 0,
                                end: 100,
                                yAxisIndex: [3],

                                fillerColor: 'transparent',
                                textStyle: {
                                    color: (window.localStorage.getItem('theme') && window.localStorage.getItem('theme') == 'light') ? '#595959' : '#d3d8e2',
                                    fontSize: '10'
                                },

                                labelFormatter: function (value, time) {
                                    return time.split(' ')[0] + '\n' + time.split(' ')[1];
                                },

                                selectedDataBackground: {
                                    lineStyle: {
                                        color: '#39CC7E',
                                        type: 'solid'
                                    },
                                    areaStyle: {
                                        color: 'transparent'
                                    }
                                },
                                dataBackground: {
                                    lineStyle: {
                                        color: '#184e31'
                                    },
                                    areaStyle: {
                                        color: 'transparent'
                                    }
                                }
                            }
                        ],
                        series: [
                            {
                                data: this.historyValue,
                                type: 'line',
                                smooth: true,
                                sampling: 'average',
                                itemStyle: {
                                    normal: {
                                        color: '#39CC7E', //改变折线点的颜色
                                        lineStyle: {
                                            color: '#39CC7E' //改变折线颜色
                                        }
                                    }
                                },
                                lineStyle: {
                                    width: 2,
                                },
                                emphasis: {
                                    lineStyle: {
                                        width: 2,
                                    },
                                }
                            }
                        ]
                    }

                    this.echart.setOption(option);

                    this.echart.on('dataZoom', (event) => {
                        this.$nextTick(() => {
                            let start = '', end = '';
                            if (event.batch) {
                                start = event.batch[0].start;
                                end = event.batch[0].end;
                            } else {
                                start = event.start;
                                end = event.end;
                            };
                            let data = this.echart.getModel().option;
                            let startIndex = data.dataZoom[0].startValue
                            let endIndex = data.dataZoom[0].endValue
                            this.refleshExtream(startIndex, endIndex)
                            // option.dataZoom[0].start = start;
                            // option.dataZoom[0].end = end;
                            // this.echart.setOption(option);
                            return this.echart;
                        })


                    })
                    this.echart.hideLoading();
                }, 500);
            }

        }
    }
};
