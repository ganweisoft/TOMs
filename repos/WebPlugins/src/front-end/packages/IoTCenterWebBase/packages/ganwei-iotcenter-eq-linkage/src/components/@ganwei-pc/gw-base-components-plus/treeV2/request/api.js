const treeRequest = {
    getSetParm (data) {
        if (window.AxiosBuilder.axios) {
            return window.AxiosBuilder.axios({
                method: 'post',
                url: '/IoT/api/v3/EquipList/GetFullSetParmByEquipNo',
                params: data,
                data: data,
                headers: {
                    'Content-Type': 'application/json;charset=UTF-8',
                    'Accept-Language': window.sessionStorage.languageType || 'zh-CN'
                }
            })
        }
    }
}

const api = Object.assign(
    {},
    treeRequest
);

export default api;
