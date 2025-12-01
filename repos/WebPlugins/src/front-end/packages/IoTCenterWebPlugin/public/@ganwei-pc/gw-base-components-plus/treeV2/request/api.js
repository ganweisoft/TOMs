const treeRequest = {
    getSetParm (data) {
        if (window.axios) {
            return window.axios({
                method: 'post',
                url: '/IoT/api/v3/EquipList/GetFullSetParmByEquipNo',
                params: data,
                data: data,
            })
        }
    }
}

const api = Object.assign(
    {},
    treeRequest
);

export default api;
