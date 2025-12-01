export default function timestamp(type, datetime) {
    let date = new Date(datetime); // 时间戳为10位需*1000，时间戳为13位的话不需乘1000
    let year = date.getFullYear(),
        month = ('0' + (date.getMonth() + 1)).slice(-2),
        sdate = ('0' + date.getDate()).slice(-2),
        hour = ('0' + date.getHours()).slice(-2),
        minute = ('0' + date.getMinutes()).slice(-2),
        second = ('0' + date.getSeconds()).slice(-2);

    // 拼接
    let result = year + '-' + month + '-' + sdate + ' ' + hour + ':' + minute + ':' + second;

    // 返回
    if (type == 'YTD') {
        return result;
    } else if (type == 'HMS') {
        result = result.substring(result.length - 8);
        return result;
    }
}
export {
    timestamp
}