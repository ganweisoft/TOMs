const autoPlay = {
    equip_item_state (equip_no, ycp_no) {
        return this.post('/api/real/equip_item_state', {
            equip_no: equip_no,
            ycp_no: ycp_no
        });
    },
    equip_yxp_state (equipno, yxno) { // 旧版本遥信
        return this.post('/api/real/equip_yxp_state', {
            equip_no: equipno,
            yxp_no: yxno
        });
    },
    setIssue (data) {
        return this.post('/api/real/setup_service', data)
    },

    // 下发命令
    IssueOrder(data) {
        return this.post('/IoT/api/v3/AutomaticExplanation/IssueOrder', data);
    }
}
export default autoPlay;