import U from 'highcharts/es-modules/Core/Utilities'

const { addEvent, isObject, pick, defined, merge } = U;

const composedClasses = [], defaultOptions = {
    enabled: true,
    sensitivity: 1.1
};

const optionsToObject = (options) => {
    if (!isObject(options)) {
        return merge(defaultOptions, { enabled: defined(options) ? options : true });
    }
    return merge(defaultOptions, options);
};
/**
 * @private
 */
const fitToBox = function (inner, outer) {
    if (inner.x + inner.width > outer.x + outer.width) {
        if (inner.width > outer.width) {
            inner.width = outer.width;
            inner.x = outer.x;
        }
        else {
            inner.x = outer.x + outer.width - inner.width;
        }
    }
    if (inner.width > outer.width) {
        inner.width = outer.width;
    }
    if (inner.x < outer.x) {
        inner.x = outer.x;
    }
    // y and height
    if (inner.y + inner.height > outer.y + outer.height) {
        if (inner.height > outer.height) {
            inner.height = outer.height;
            inner.y = outer.y;
        }
        else {
            inner.y = outer.y + outer.height - inner.height;
        }
    }
    if (inner.height > outer.height) {
        inner.height = outer.height;
    }
    if (inner.y < outer.y) {
        inner.y = outer.y;
    }
    return inner;
};
let wheelTimer, originalOptions;
/**
 * @private
 */
const zoomBy = function (chart, howMuch, centerXArg, centerYArg, mouseX, mouseY, options) {
    let xAxis = chart.xAxis[0], yAxis = chart.yAxis[0], type = pick(options.type, chart.options.chart.zooming.type, 'x'), zoomX = /x/.test(type), zoomY = /y/.test(type);
    if (defined(xAxis.max) && defined(xAxis.min) &&
        defined(xAxis.dataMax) && defined(xAxis.dataMin)) {
        if (!defined(yAxis.min) || !defined(yAxis.max) || !defined(yAxis.dataMin) || !defined(yAxis.dataMax)) {
            yAxis.min = 0
            yAxis.max = 100
            yAxis.dataMin = 0
            yAxis.dataMax = 100
            yAxis.update({
                min: 0, max: 100
            })
        } else {
            yAxis.update({
                min: undefined, max: undefined
            })
        }

        if (zoomY) {
            // Options interfering with yAxis zoom by setExtremes() returning
            // integers by default.
            if (defined(wheelTimer)) {
                clearTimeout(wheelTimer);
            }
            const { startOnTick, endOnTick } = yAxis.options;
            if (!originalOptions) {
                originalOptions = { startOnTick, endOnTick };
            }
            if (startOnTick || endOnTick) {
                yAxis.setOptions({ startOnTick: false, endOnTick: false });
            }
            wheelTimer = setTimeout(() => {
                if (originalOptions) {
                    yAxis.setOptions(originalOptions);
                    // Set the extremes to the same as they already are, but now
                    // with the original startOnTick and endOnTick. We need
                    // `forceRedraw` otherwise it will detect that the values
                    // haven't changed. We do not use a simple yAxis.update()
                    // because it will destroy the ticks and prevent animation.
                    const { min, max } = yAxis.getExtremes();
                    yAxis.forceRedraw = true;
                    yAxis.setExtremes(min, max);
                    originalOptions = void 0;
                }
            }, 400);
        }
        if (chart.inverted) {
            const emulateRoof = yAxis.pos + yAxis.len;
            // Get the correct values
            centerXArg = xAxis.toValue(mouseY);
            centerYArg = yAxis.toValue(mouseX);
            // Swapping x and y for simplicity when chart is inverted.
            const tmp = mouseX;
            mouseX = mouseY;
            mouseY = emulateRoof - tmp + yAxis.pos;
        }
        let fixToX = mouseX ? ((mouseX - xAxis.pos) / xAxis.len) : 0.5;
        if (xAxis.reversed && !chart.inverted ||
            chart.inverted && !xAxis.reversed) {
            // We are taking into account that xAxis automatically gets
            // reversed when chart.inverted
            fixToX = 1 - fixToX;
        }
        let fixToY = 1 - (mouseY ? ((mouseY - yAxis.pos) / yAxis.len) : 0.5);
        if (yAxis.reversed) {
            fixToY = 1 - fixToY;
        }
        const xRange = xAxis.max - xAxis.min, centerX = pick(centerXArg, xAxis.min + xRange / 2), newXRange = xRange * howMuch, yRange = yAxis.max - yAxis.min, centerY = pick(centerYArg, yAxis.min + yRange / 2), newYRange = yRange * howMuch, newXMin = centerX - newXRange * fixToX, newYMin = centerY - newYRange * fixToY, dataRangeX = xAxis.dataMax - xAxis.dataMin, dataRangeY = yAxis.dataMax - yAxis.dataMin, outerX = xAxis.dataMin - dataRangeX * xAxis.options.minPadding, outerWidth = dataRangeX + dataRangeX * xAxis.options.minPadding +
            dataRangeX * xAxis.options.maxPadding, outerY = yAxis.dataMin - dataRangeY * yAxis.options.minPadding, outerHeight = dataRangeY + dataRangeY * yAxis.options.minPadding +
                dataRangeY * yAxis.options.maxPadding, newExt = fitToBox({
                    x: newXMin,
                    y: newYMin,
                    width: newXRange,
                    height: newYRange
                }, {
                    x: outerX,
                    y: outerY,
                    width: outerWidth,
                    height: outerHeight
                }), zoomOut = (newExt.x <= outerX &&
                    newExt.width >=
                    outerWidth &&
                    newExt.y <= outerY &&
                    newExt.height >= outerHeight);
        // Zoom
        if (defined(howMuch) && !zoomOut) {
            if (zoomX) {
                xAxis.setExtremes(newExt.x, newExt.x + newExt.width, false);
            }
            if (zoomY) {
                yAxis.setExtremes(newExt.y, newExt.y + newExt.height, false);
            }
            if (!chart.resetZoomButton) {
                chart.showResetZoom();
            }
            // Reset zoom
        }
        else {
            if (zoomX) {
                xAxis.setExtremes(void 0, void 0, false);
            }
            if (zoomY) {
                yAxis.setExtremes(void 0, void 0, false);
            }
            chart.zoomOut()
        }
        chart.redraw(false);
    }
};
/**
 * @private
 */
function onAfterGetContainer () {
    const chart = this, wheelZoomOptions = optionsToObject(chart.options.chart.zooming.mouseWheel);
    if (wheelZoomOptions.enabled) {
        addEvent(this.container, 'wheel', (e) => {
            e = this.pointer.normalize(e);
            // Firefox uses e.detail, WebKit and IE uses deltaX, deltaY, deltaZ.
            if (chart.isInsidePlot(e.chartX - chart.plotLeft, e.chartY - chart.plotTop)) {
                const wheelSensitivity = pick(wheelZoomOptions.sensitivity, 1.1), delta = e.detail || ((e.deltaY || 0) / 120);
                zoomBy(chart, Math.pow(wheelSensitivity, delta), chart.xAxis[0].toValue(e.chartX), chart.yAxis[0].toValue(e.chartY), e.chartX, e.chartY, wheelZoomOptions);
            }
            // prevent page scroll
            if (e.preventDefault) {
                e.preventDefault();
            }
        });
    }
}
/**
 * @private
 */
function compose (ChartClass) {
    if (composedClasses.indexOf(ChartClass) === -1) {
        composedClasses.push(ChartClass);
        addEvent(ChartClass, 'afterGetContainer', onAfterGetContainer);
    }
}

export default function install (Highcharts) {
    compose(Highcharts.Chart)
};
