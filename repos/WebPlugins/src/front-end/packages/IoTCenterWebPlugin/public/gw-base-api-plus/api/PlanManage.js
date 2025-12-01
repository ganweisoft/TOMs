/**
 * 预案管理
 */
const planManage = {

  // 获取预案列表
  getPlanList(data) {
    return this.post('/IoT/api/v3/ReservePlan/GetPlanList', data);
  },

  // 添加预案
  createPlan(data) {
    return this.post('/IoT/api/v3/ReservePlan/CreatePlan', data);
  },

  // 编辑预案
  editPlan(data) {
    return this.post('/IoT/api/v3/ReservePlan/SetPlan', data);
  },

  // 删除预案
  deletePlan(data) {
    return this.post('/IoT/api/v3/ReservePlan/DeletePlan?planId=' + data)
  },

  // 上传附件
  uploadPlanFile(data) {
    return this.post('/IoT/api/v3/ReservePlan/UploadAttach', data);
  },

  // 删除附件
  deletePlanFile(data) {
    return this.post('/IoT/api/v3/ReservePlan/DeleteAttach?attachmenId=' + data);
  },

  // 获取预案详情
  getPlanDetail(data) {
    return this.post('/IoT/api/v3/ReservePlan/GetPlanDetail?planId=' + data);
  },

  // 下载单个附件（暂未使用）
  downloadPlanFile(data) {
    return this.get('/IoT/api/v3/ReservePlan/getDownLoadAttach?attachmenId=' + data);
  }
}

export default planManage;