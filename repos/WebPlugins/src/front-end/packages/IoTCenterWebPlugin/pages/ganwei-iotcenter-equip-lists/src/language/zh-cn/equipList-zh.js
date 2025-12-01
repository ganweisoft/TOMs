const equipList = {
    "title": {
        "leftHeaderTitle": "全部设备",
        "exportType": "导出类型",
        "typeSelect": "类型选择",
        "exportCurve": "曲线导出",
        "selectDevice": "选择设备",
        "selectYc": "选择遥测",
        "selectYx": "选择遥信",
        "ycData": "遥测数据",
        "yxData": "遥信数据"
    },
    "input": {
        "inputSearchEquip": "搜索设备名称",
        "searchYc": "搜索遥测名称",
        "searchYx": "搜索遥信名称",
        "searchSet": "搜索设置名称",
        "selectStartTime": "选择开始时间",
        "selectEndTime": "选择结束时间",
        "startTime": "开始时间",
        "endTime": "结束时间"
    },
    "tips": {
        "exportTips": "批量导出当前全部故障设备和离线设备",
        "execute": " 是否执行？",
        "titleTip": "提示",
        "timeHorizon": "请选择查询的时间范围!",
        "timeRange": "请选择合理的时间范围",
        "timeDays": "查询天数范围不能大于7天",
        "queryHistory": "查询历史曲线为空!",
        "curveValue": "当前测点离线无实时值！",
        "selectDevice": "请选择设备",
        "success": "操作成功",
        "noRelatedVideo": "当前测点没有关联视频",
        "videoConnectError": "流媒体服务连接失败",
        "lessOnePoint": "至少选一个测点导出",
        "exportByDayTip": "每一个设备每一天都会生成一个记录文件",
        "exportByCombinedTip": "每个设备仅会生成一个记录文件",
        "noSelect": "暂无可选项",
        "deviceNoConnect": "设备未连接！"
    },
    "tabs": {
        "ycNm": "遥测量",
        "yxNm": "遥信量",
        "setNm": "设置"
    },
    "table": {
        "listTitleYc": {
            "equipNo": "设备号",
            "ycYxNo": "遥测编号",
            "alarmState": "报警状态",
            "ycYxName": "遥测名称",
            "value": "实时值",
            "quantity": "报警合并数量",
            "curve": "曲线",
            "location": "定位",
            "video": "视频",
            "asset": "资产",
            "suggestion": "处理意见"
        },
        "listTitleYx": {
            "equipNo": "设备号",
            "ycYxNo": "遥信编号",
            "alarmState": "报警状态",
            "ycYxName": "遥信名称",
            "value": "实时状态",
            "quantity": "报警合并数量",
            "curve": "曲线",
            "location": "定位",
            "video": "视频",
            "asset": "资产",
            "suggestion": "处理意见"
        }
    },
    "label": {
        "inputValue": "输入值：",
        "date": "日期",
        "value": "数值",
        "list": "展示列表"
    },
    "state": [
        "正常",
        "报警",
        "离线"
    ],
    "button": {
        "confirm": "确认",
        "cancel": "取消",
        "curve": "实时曲线",
        "history": "历史曲线",
        "list": "历史列表",
        "exportDevice": "导出设备",
        "exportCurve": "导出曲线",
        "exportByDay": "分天导出",
        "exportByCombined": "合并导出",
        "export": "导出",
        "search": "查询",
        "RealTimeValue": "实时值"
    },
    "echart": {
        "legend": {
            "realTimeValue": "实时值",
            "upperLimitValue": "上限值",
            "lowerLimitValue": "下限值"
        },
        "toolTip": {
            "time": "时间",
            "currentValue": "当前值"
        }
    },
    "excelName": {
        "alarmDevice": "告警设备",
        "outLineDevice": "离线设备",
        "normalDevice": "正常设备",
        "allDevice": "全部设备"
    },
    "publics": {
        "button": {
            "confirm": "确认",
            "cancel": "取消",
            "deletes": "删除",
            "edit": "编辑",
            "reset": "重置",
            "empty": "清空",
            "download": "下载",
            "exports": "导出",
            "imports": "导入",
            "filter": "筛选",
            "selectAll": "全选",
            "add": "新增",
            "save": "保存",
            "search": "查询",
            "upload": "上传",
            "upgrade": "升级"
        },
        "tips": {
            "addSuccess": "新增成功",
            "addFail": "新增失败",
            "saveSuccess": "保存成功",
            "saveFail": "保存失败",
            "editSuccess": "编辑成功",
            "editFail": "编辑失败",
            "importSuccess": "导入成功",
            "importFail": "导入失败",
            "exportSuccess": "导出成功",
            "exportFail": "导出失败",
            "deleteSuccess": "删除成功",
            "deleteFail": "删除失败",
            "serverErr": "服务异常",
            "uploadSuccess": "上传成功",
            "uploadFail": "上传失败",
            "upgradeSuccess": "升级成功",
            "upgradeFail": "升级失败",
            "setIssueSuccess": "指令下发成功",
            "setIssueError": "指令下发失败"
        },
        "noData": "暂无相关内容",
        "platform": "综合管理平台",
        "warnings": {
            "STTimeCantGreaterEndTime": "开始时间不能大于结束时间",
            "timeCantMoreThanNinetyDay": "导出时间间隔不能超过90天！",
            "processing": "数据处理中，可关闭弹窗操作其他页面！",
            "selectStartTime": "请选择导出开始时间",
            "selectEndTime": "请选择导出结束时间",
            "readyToExport": "历史曲线数据处理完毕，即将导出"
        }
    }
}

export default equipList