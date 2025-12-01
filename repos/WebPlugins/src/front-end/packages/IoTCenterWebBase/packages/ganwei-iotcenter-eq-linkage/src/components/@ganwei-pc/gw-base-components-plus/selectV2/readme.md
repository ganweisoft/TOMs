用法：
<selectV2 v-model="stationInfoId" clearable @change="enterSearch" data-label="name" data-value="id" :data="stationList" @remoteSearch="remoteSearch" @loadmore="loadmore" />

change(function):选中某项触发的事件
data(Array)：下拉渲染列表（全局维护）
remoteSearch(function):搜索触发事件
     用法：
    remoteSearch (keyword) {
            //接受输入的关键字
            this.stationList = [] //将下拉列表置空
            this.stationPagination.pageNo = 1;  //分页从1开始
            this.stationPagination.total = 0;   //总数置空
            this.getStationList(keyword)        //重新获取下拉列表
    },
loadmore(function):滚动加载触发事件
   用法：
    loadmore (keyword) {
        //接受输入的关键字
        if (this.stationList.length < this.stationPagination.total) {
                this.stationPagination.pageNo++
                this.getStationList(keyword)
            }
    }
