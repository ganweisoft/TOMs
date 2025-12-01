export default class Navigator {
    constructor(chart) {
        this.dragging = false
        this.change = false
        this.startPoint = {}
        this.endPoint = {}
        this.active = null
        this.init(chart);
    }

    init (chart) {
        chart._navigator = this;
        this.chart = chart
        this.background = null
        this.handles = []
        this.outsides = []
        this.navigatorGroup = null
        this.shade = null
        this.renderElements()
        this.addEventListener()
        const chartDestroy = this.chart.destroy;
        this.chart.destroy = () => {
            this.chart._navigator.destroy();
            chartDestroy.apply(this.chart, arguments);
        }
    }

    renderElements () {
        const chart = this.chart
        const renderer = this.chart.renderer

        // 分组
        this.navigatorGroup = renderer.g('navigator')
            .attr({
                zIndex: 8,
                with: chart.plotWidth
            })
            .add();
        // 背景事件兜底
        this.background = renderer.rect(0, 0, chart.spacingBox.width + chart.spacing[1] + chart.spacing[3], chart.spacingBox.height + chart.spacing[0] + chart.spacing[2])
            .addClass('highcharts-navigator-mask-background')
            .css({ 'pointer-events': 'all' })
            .add(this.navigatorGroup)

        // 拖拽部分
        let outside = renderer.rect(chart.plotLeft, chart.plotTop, 0, chart.plotHeight)
            .addClass('highcharts-navigator-mask-outside')
            .add(this.navigatorGroup)
        this.outsides.push(outside)
        this.shade = renderer.rect(chart.plotLeft, chart.plotTop, chart.plotWidth, chart.plotHeight)
            .addClass('highcharts-navigator-mask-inside')
            .add(this.navigatorGroup)
        this.shade.attr({
            fill: 'rgba(102,122,255,0.3)'
        })
        this.shade.css({ cursor: 'ew-resize' });
        outside = renderer.rect(chart.plotLeft + chart.plotWidth, chart.plotTop, 0, chart.plotHeight)
            .addClass('highcharts-navigator-mask-outside')
            .add(this.navigatorGroup)
        this.outsides.push(outside)

        // 拖拽把手
        renderer.symbols['navigator-handle'] = navigatorHandle;
        ;[0, 1].forEach(index => {
            const width = 5, height = 15
            this.handles[index] = renderer.symbol(
                'navigator-handle',
                -width / 2 - 1,
                0,
                width,
                height,
            )
            this.handles[index].attr({ zIndex: 7 - index })
                .addClass(
                    'highcharts-navigator-handle ' +
                    'highcharts-navigator-handle-' +
                    ['left', 'right'][index]
                )
                .add(this.navigatorGroup)
                .attr({
                    fill: '#f2f2f2',
                    stroke: '#999999',
                    stroke: '#cbd5ea',
                    'stroke-width': 2,
                })
                .css({ cursor: 'ew-resize' })
                .attr({
                    translateX: chart.plotLeft - 6 + index * chart.plotWidth,
                    translateY: chart.plotTop
                })
        })
    }

    get shadeEvent () {
        return {
            element: this.shade.element,
            mousedown: (e) => {
                this.active = this.shadeEvent
                e = this.chart.pointer.normalize(e);
                this.dragging = true
                this.startPoint = {
                    start: Number(this.shade.element.attributes.x.value),
                    end: Number(this.shade.element.attributes.x.value) + Number(this.shade.element.attributes.width.value),
                    width: Number(this.shade.element.attributes.width.value),
                    point: e.chartX
                }
            },
            mouseup: () => {
                this.dragging = false
                this.startPoint = {}
                if (this.change) {
                    const xAxis = this.chart.xAxis[0]
                    const min = xAxis.toValue(this.endPoint.start)
                    const max = xAxis.toValue(this.endPoint.end)
                    this.chart._detailsChart.xAxis[0].setExtremes(min, max)
                }
            },
            mousemove: (e) => {
                e.stopPropagation()
                if (this.dragging) {
                    if (this.active?.element === this.shade.element) {
                        e = this.chart.pointer.normalize(e);
                        const delta = e.chartX - this.startPoint.point;
                        const start = this.startPoint.start + delta
                        const end = start + this.startPoint.width
                        if (start <= this.chart.plotLeft || end >= (this.chart.plotWidth + this.chart.plotLeft)) {
                            return
                        }
                        this.change = true
                        this.setShade(this.startPoint.start + delta, this.startPoint.width)
                        this.endPoint = {
                            start: this.startPoint.start + delta,
                            end: this.startPoint.start + delta + this.startPoint.width,
                            width: this.startPoint.width,
                            point: e.chartX
                        }
                    } else {
                        this.active?.mousemove(e)
                    }
                }
            }
        }
    }

    get rightHandleEvent () {
        return {
            element: this.handles[1].element,
            mousedown: (e) => {
                this.active = this.rightHandleEvent
                e = this.chart.pointer.normalize(e);
                this.dragging = true
                this.startPoint = {
                    start: Number(this.shade.element.attributes.x.value),
                    end: Number(this.shade.element.attributes.x.value) + Number(this.shade.element.attributes.width.value),
                    width: Number(this.shade.element.attributes.width.value),
                    point: e.chartX
                }
            },
            mousemove: (e) => {
                e.stopPropagation()
                if (this.dragging) {
                    if (this.active?.element === this.handles[1].element) {
                        e = this.chart.pointer.normalize(e);
                        if (e.chartX <= this.chart.plotLeft || e.chartX >= (this.chart.plotWidth + this.chart.plotLeft)) {
                            return
                        }
                        this.change = true
                        if (e.chartX - this.startPoint.start <= 10) {
                            return
                        }
                        this.setShade(this.startPoint.start, e.chartX - this.startPoint.start)
                        this.endPoint = {
                            start: this.startPoint.start,
                            end: e.chartX,
                            width: e.chartX - this.startPoint.start,
                            point: e.chartX
                        }
                    }
                }
            },
            mouseup: () => {
                this.dragging = false
                this.startPoint = {}
                if (this.change) {
                    const xAxis = this.chart.xAxis[0]
                    const min = xAxis.toValue(this.endPoint.start)
                    const max = xAxis.toValue(this.endPoint.end)
                    this.chart._detailsChart.xAxis[0].setExtremes(min, max)
                }
            },
        }
    }

    get leftHandleEvent () {
        return {
            element: this.handles[0].element,
            mousedown: (e) => {
                this.active = this.leftHandleEvent
                e = this.chart.pointer.normalize(e);
                this.dragging = true
                this.startPoint = {
                    start: Number(this.shade.element.attributes.x.value),
                    end: Number(this.shade.element.attributes.x.value) + Number(this.shade.element.attributes.width.value),
                    width: Number(this.shade.element.attributes.width.value),
                    point: e.chartX
                }
            },
            mouseup: () => {
                this.dragging = false
                this.startPoint = {}
                if (this.change) {
                    const xAxis = this.chart.xAxis[0]
                    const min = xAxis.toValue(this.endPoint.start)
                    const max = xAxis.toValue(this.endPoint.end)
                    this.chart._detailsChart.xAxis[0].setExtremes(min, max)
                }
            },
            mousemove: (e) => {
                e.stopPropagation()
                if (this.dragging) {
                    if (this.active?.element === this.handles[0].element) {
                        e = this.chart.pointer.normalize(e);
                        if (e.chartX <= this.chart.plotLeft || e.chartX >= (this.chart.plotWidth + this.chart.plotLeft)) {
                            return
                        }
                        if (this.startPoint.end - e.chartX <= 10) {
                            return
                        }
                        this.change = true
                        this.setShade(e.chartX, this.startPoint.end - e.chartX)
                        this.endPoint = {
                            start: e.chartX,
                            end: this.startPoint.end,
                            width: this.startPoint.end - e.chartX,
                            point: e.chartX
                        }
                    }

                }
            },
        }
    }

    get backgroundEvent () {
        return {
            mouseup: (e) => {
                this.active = null
                this.change = false
            },
            mousemove: (e) => {
                if (!this.active) return;
                this.active.mousemove(e)
            }
        }
    }

    addEventListener () {
        this.background.element.addEventListener('mouseup', this.backgroundEvent.mouseup)
        this.background.element.addEventListener('mousemove', this.backgroundEvent.mousemove)

        this.shade.element.addEventListener('mousedown', this.shadeEvent.mousedown)
        this.shade.element.addEventListener('mouseup', this.shadeEvent.mouseup)
        this.shade.element.addEventListener('mousemove', this.shadeEvent.mousemove)

        this.handles[1].element.addEventListener('mousedown', this.rightHandleEvent.mousedown)
        this.handles[1].element.addEventListener('mousemove', this.rightHandleEvent.mousemove)
        this.handles[1].element.addEventListener('mouseup', this.rightHandleEvent.mouseup)
        this.handles[0].element.addEventListener('mousedown', this.leftHandleEvent.mousedown)
        this.handles[0].element.addEventListener('mousemove', this.leftHandleEvent.mousemove)
        this.handles[0].element.addEventListener('mouseup', this.leftHandleEvent.mouseup)
    }

    setShade (x, width) {
        this.shade.attr({
            x,
            width
        });
        [0, 1].forEach(index => {
            this.handles[index].attr({
                translateX: x - 6 + index * width,
                translateY: this.chart.plotTop
            })
        })
        this.outsides[0].attr({
            width: x - this.chart.plotLeft,
        })
        this.outsides[1].attr({
            x: x + width,
            width: this.chart.plotLeft + this.chart.plotWidth - x - width,
        })
    }

    destroy () {
        this.background.element.removeEventListener('mouseup', this.backgroundEvent.mouseup)
        this.background.element.removeEventListener('mousemove', this.backgroundEvent.mousemove)

        this.shade.element.removeEventListener('mousedown', this.shadeEvent.mousedown);
        this.shade.element.removeEventListener('mouseup', this.shadeEvent.mouseup);
        this.shade.element.removeEventListener('mousemove', this.shadeEvent.mousemove);

        this.handles[1].element.removeEventListener('mousedown', this.rightHandleEvent.mousedown)
        this.handles[1].element.removeEventListener('mousemove', this.rightHandleEvent.mousemove)
        this.handles[1].element.removeEventListener('mouseup', this.rightHandleEvent.mouseup)
        this.handles[0].element.removeEventListener('mousedown', this.leftHandleEvent.mousedown)
        this.handles[0].element.removeEventListener('mousemove', this.leftHandleEvent.mousemove)
        this.handles[0].element.removeEventListener('mouseup', this.leftHandleEvent.mouseup)
    }
}

function navigatorHandle (
    _x,
    _y,
    width,
    height,
    options = {}
) {
    const halfWidth = options.width ? options.width / 2 : width,
        markerPosition = Math.round(halfWidth / 3) + 0.5;

    height = options.height || height;

    return [
        ['M', 6, 0],
        ['V', 13],
        ['M', 3, 13],
        ['H', 9],
        ['V', 32],
        ['H', 3],
        ['Z'],
        ['M', 6, 32],
        ['V', 45],
        ['M', 2, 0],
        ['H', 10],
        ['M', 2, 45],
        ['H', 10]
    ]
}
