/**
 * 定时任务
 */

const TimeTask = {

    // 获取任务列表
    tkGetRepository (data) {
        return this.post('/IoT/api/v3/TimeTask/GetAllTaskDataList', data);
    },

    // 任务流程图
    // 查看一个任务项(普通任务或循环任务)
    tkCheckTask (data) {
        return this.post('/IoT/api/v3/TimeTask/GetTaskData', data);
    },

    // 添加一个任务项
    tkAddTask (data) {
        return this.post('/IoT/api/v3/TimeTask/AddTaskData', data);
    },

    // 删除一个任务项
    tkDelTask (data) {
        return this.post('/IoT/api/v3/TimeTask/DelTaskData', data);
    },

    // 更新一个任务项
    tkEditTask (data) {
        return this.post('/IoT/api/v3/TimeTask/EditTaskData', data);
    },

    // 获取一条普通任务
    GetCommonTaskData (data) {
        return this.get('/IoT/api/v3/TimeTask/GetCommonTaskData', data);
    },

    // 获取一条循环任务
    GetCycleTaskData (data) {
        return this.get('/IoT/api/v3/TimeTask/GetCycleTaskData', data);
    },

    // 创建普通任务
    CreateCommonTask (data) {
        return this.post('/IoT/api/v3/TimeTask/CreateCommonTask', data);
    },

    // 创建循环任务
    CreateCycleTask (data) {
        return this.post('/IoT/api/v3/TimeTask/CreateCycleTask', data);
    },

    // 修改一条普通任务
    EditCommonTaskData (data) {
        return this.post('/IoT/api/v3/TimeTask/EditCommonTaskData', data);
    },

    // 修改一条循环任务
    EditCycleyTaskData (data) {
        return this.post('/IoT/api/v3/TimeTask/EditCycleyTaskData', data);
    },

    // 删除普通任务
    DelCommonData (data) {
        return this.delete('/IoT/api/v3/TimeTask/DelCommonData', data);
    },

    // 删除循环任务
    DelCycleData (data) {
        return this.delete('/IoT/api/v3/TimeTask/DelCycleData', data);
    },

    // 特殊日期 --- 查
    tkSpGet (data) {
        return this.post('/IoT/api/v3/TimeTask/GetProcTaskSpecDataList', data);
    },

    // 特殊日期 --- 增
    tkSpAdd (data) {
        return this.post('/IoT/api/v3/TimeTask/AddProcTaskSpecData', data);
    },

    // 特殊日期 --- 删
    tkSpDel (set) {
        return this.postUrl('/IoT/api/v3/TimeTask/DelProcTaskSpecData', set);
    },

    // 特殊日期 --- 改
    tkSpEdit (data) {
        return this.post('/IoT/api/v3/TimeTask/EditProcTaskSpecData', data);
    },

    // 特殊日期 --- 获取月份任务数列表
    tkSpMonthSum (data) {
        return this.get('/IoT/api/v3/TimeTask/GetProcTaskSpecMonthData', data);
    },

    // 每周任务安排
    // 任务列表获取
    tkWeekList (data) {
        return this.post('/IoT/api/v3/TimeTask/GetProcTaskWeekDataList', data);
    },

    // 修改每周任务安排
    tkWeekEdit (data) {
        return this.post('/IoT/api/v3/TimeTask/EditProcTaskWeekData', data);
    },
    // /IoT/api/v3/CommonTable/GetExProcCmdData
    evtSysControl () {
        return this.get("/IoT/api/v3/TimeTask/GetExProcCmdData");
    },
    // 设备控制项--树状结构
    GetEquipSetParmTreeList (data) {
        return this.post('/IoT/api/v3/EquipList/GetEquipSetParmTreeList', data);
    },

    // 获取设备分组
    getGroupEquip (data) {
        return this.get('/IoT/api/v3/BA/GetGroupEquip', data)
    }
}

export default TimeTask;
