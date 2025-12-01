/**
 * 日志预览
 */

const LogPreview = {

    // ------日志预览------
    // 查询服务日志
    getLog(data) {
        return this.get(`/IoT/api/v3/LogPreview/GetLogFileTree`, data);
    },

    // 服务日志导出
    downLoadLog(data) {
        return this.postBlob(`/IoT/api/v3/LogPreview/DownLoadLog?logType=${data.logType}&relativePath=${data.relativePath}`);
    }

}

export default LogPreview;