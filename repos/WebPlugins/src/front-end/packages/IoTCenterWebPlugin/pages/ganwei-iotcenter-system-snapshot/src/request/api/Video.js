/**
 * 视频监控
 */

const Video = {

    // 云台控制
    canVideoOperator (data) {
        return this.post('/IoT/api/v3/video/videoh5streamoperator', data);
    },

    // 快照
    getVideoSnapshot (data) {
        return this.post('/IoT/api/v3/video/videoh5streamsnapshot', data);
    },

    // 录像
    getVideoManualRecord (data) {
        return this.post('/IoT/api/v3/video/videoh5streamManualRecord', data);
    },

    // 回放与快照
    getVideoDownResource (data) {
        return this.post('/IoT/api/v3/video/videoh5streamDownResource', data);
    },

    // 查找回放记录
    getVideoSearch (data) {
        return this.post('/IoT/api/v3/video/videoh5streamSearch', data);
    },

    // 登录H5Stream平台
    loginVideo (data) {
        return this.post('/IoT/api/v3/video/videoh5streamlogin', data);
    },

    // 获取所有视频配置
    getVideoConfig (data) {
        return this.post('/IoT/api/v3/video/videoconfig', data);
    },

    // 获取所有视频信息
    getVideoAllInfo (data) {
        return this.get('/IoT/api/v3/video/videoinfor', data);
    },

    // 视频平台
    getRecordList (params) {
        return this.get(`/api/VideoInfo/GetRecordListClient`, params);
    },

    StartVideotape (params) {
        return this.get('/api/VideoInfo/StartVideotape', params)
    }
}

export default Video;
