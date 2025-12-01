/**
 * @file 工具类
 */

export default class equipUtils {

    /**
    * @description 将多个数值拼接转化成10进制数
    * @param {Object} obj 需要转换成十进制的二进制数
    * @return {Number} 返回生成的十进制数
    */
    static toDecimalSystem (obj) {
        let str = '';
        for (let i in obj) {
            str += obj[i];
        }
        return parseInt(str, 2);
    }

    /**
    * @description 验证安全时段
    * @param {String} time 需要进行校验的时间
    * @return {Object} 返回验证信息
    */
    static verificationSateTime (time) {
        let data = {
            pass: true,
            warning: '验证通过'
        }
        if (time !== '') {
            let safeTimeReg = new RegExp(
                '^(((([2][0-3])|([0-1][0-9])):[0-5][0-9]:[0-5][0-9])-((([2][0-3])|([0-1][0-9])):[0-5][0-9]:[0-5][0-9]))(([+](([2][0-3])|([0-1][0-9])):[0-5][0-9]:[0-5][0-9])-((([2][0-3])|([0-1][0-9])):[0-5][0-9]:[0-5][0-9]))*$'
            );
            if (!safeTimeReg.test(time)) {
                data.pass = false;
                data.warning = 'equipInfo.tips.safeTimeErr'
                return data;
            }

            let timeArr = time.split('+');
            for (let i = 0; i < timeArr.length; i++) {
                let timeArrItem = timeArr[i].split('-');

                let startItem = timeArrItem[0].split(':');
                let endItem = timeArrItem[1].split(':');

                let startTimeNum =
                    Number(startItem[0]) * 3600 +
                    Number(startItem[1]) * 60 +
                    Number(startItem[2]);
                let endTimeNum =
                    Number(endItem[0]) * 3600 + Number(endItem[1]) * 60 + Number(endItem[2]);

                if (startTimeNum >= endTimeNum) {
                    data.pass = false;
                    data.warning = 'equipInfo.tips.saveStartNotMoreEnd'
                    return data;
                }
            }
        }
        return data;
    }

    /**
     * @description 匹配视频名称
     * @param {Array} list 选中的修改项列表
     * @return {Object} 返回转换后的数据
     */
    static getVideoName (vidoeList, key) {
        let name = '';
        vidoeList.forEach((item) => {
            if (item.id === key) {
                name = item.channelName;
            }
        });
        return name;
    }

}