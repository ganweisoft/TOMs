import apiFunction from 'gw-base-api-plus/apiFunction';

const treeRequest={
    getEquip(data){
        return this.get('/IoT/api/v3/BA/GroupListNew',data)
    },
    getEquipCount(){
        return this.get('/IoT/api/v3/BA/GroupListCount')
    },
    getSetParm(data){
        return this.post('/IoT/api/v3/EquipList/GetSetParmByEquipNo',data)
    }
}

const api = Object.assign(
    {},
    apiFunction,
    treeRequest
);

export default api;