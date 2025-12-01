/**
 * @file 统计图表
 */
const GwEcharts = {
    getChartsValue() {
        return this.get('/IoT/api/v3/RealTime/GetChartValue');
    }
};
export default GwEcharts;
