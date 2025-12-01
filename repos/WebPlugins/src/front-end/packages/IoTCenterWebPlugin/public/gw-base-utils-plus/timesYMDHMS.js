export default function timesYMDHMS() {
    let date = new Date();
    let seperator1 = '-';
    let seperator2 = ':';

    // 外国的月份都是从0开始的，所以+1
    let month = date.getMonth() + 1;
    let strDate = date.getDate();

    // 1-9月用0补位
    if (month >= 1 && month <= 9) {
        month = '0' + month;
    }

    // 1-9日用0补位
    if (strDate >= 0 && strDate <= 9) {
        strDate = '0' + strDate;
    }

    // 获取当前时间 yyyy-MM-dd HH:mm:ss
    let currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate + ' ' + date.getHours() + seperator2 + date.getMinutes() + seperator2 + date.getSeconds();
    return currentdate;
}

export {
    timesYMDHMS
}