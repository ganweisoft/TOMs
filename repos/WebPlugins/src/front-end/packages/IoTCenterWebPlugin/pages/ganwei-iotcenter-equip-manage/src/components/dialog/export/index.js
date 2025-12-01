export default {
    data () {
        return {
            isShowDialog: false,
            saveLoading: false,
            checkAll: false,
            checkedList: [],
            list: [],
            isIndeterminate: false,
            exportPage: 1,
            exportSize: 100,
            exportTotal: 0,
            exportSearch: ''

        }
    },
    watch: {
        exportSearch (val, oldValue) {
            if (!val && oldValue.trim()) {
                this.exportSearchFun(null, true)
            }
        },
    },
    methods: {
        closeDialog() {
            this.isShowDialog = false
        },
        openDialog() {
            this.isShowDialog = true
        },
        exportExcel(){
            let obj = this.checkedList;
            this.saveLoading = true;
            this.$api
                .downloadProductList(obj)
                .then((res) => {
                    if (res?.status === 200) {
                        this.myUtils.download(res?.data || '', this.$t('templateManage.title.modelTitle') + '.xlsx')
                    } else {
                        this.$message.error(this.$t('templateManage.publics.tips.exportFail'));
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err);
                }).finally(() => {
                    this.saveLoading = false;
                    this.isShowDialog = false;
                });
        },
        handleCheckAllChange(val) {
            this.allChecked(val);
            this.isIndeterminate = false;
        },
        handleCheckedListChange(value) {
            let equipNo = this.filterListEquipNo()
            // 本页list-this.filterListEquipNo()，是checkedList的子集
            this.checkAll = this.isArraySubset(equipNo,this.checkedList)
            // 全选为false 且 本页list有和checkedList存在交集
            this.isIndeterminate = this.checkAll?false:this.isArrayIntersection(equipNo,this.checkedList)
        },
        getTemplateList (val) {
            let obj = {};
            obj.pageNo = this.exportPage;
            obj.pageSize = this.exportSize;
            obj.equipName = this.exportSearch;
            this.saveLoading = true;
            this.$api
                .getModelEquipTree(obj)
                .then((res) => {
                    const { code, data, message } = res?.data || {}
                    if (code == 200) {
                        this.list = data?.rows || [];
                        if (this.list instanceof Array) {
                            this.list.forEach(item => {
                                item.staNo = item.staN
                                if (item.relatedVideo) {
                                    item.relatedVideo = parseInt(item.relatedVideo.split('|')[1]);
                                }
                            })
                        }
                        this.exportTotal = data?.total || 0;
                        if(val && this.list.length>0) {
                            let equipNo = this.filterListEquipNo()
                             if(this.checkedList.length>0) {
                                this.checkAll = this.isArraySubset(equipNo,this.checkedList)
                             } else {
                                this.checkAll = false
                             }
                             this.isIndeterminate = this.checkAll?false:this.isArrayIntersection(equipNo,this.checkedList)
                        }
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err);
                }).finally(() => {
                    this.saveLoading = false;
                });
        },
        // 页码大小改变时触发事件
        exportSizeChange (pageSize) {
            this.exportSize = pageSize;
            this.exportPage = 1;
            this.getTemplateList()
        },

        // 当前页码改变时触发
        exportCurrentChange (pageNo) {
            this.exportPage = pageNo;
            this.getTemplateList(true)
        },
        // 搜索设备
        exportSearchFun (e, change) {
            this.exportSearch = this.exportSearch.trim()
            if (this.exportSearch || change) {
                this.exportPage = 1;
                this.getTemplateList(true);
            }

        },
        filterListEquipNo(){
           let arry = []
           if(this.list.length>0) {
               this.list.forEach(item=>{
                arry.push(item.equipNo)
               })
           }
           return arry
        },
        allChecked(val){
            let equipNo = this.filterListEquipNo()
            if(val) {
                this.checkedList = this.checkedList.concat(equipNo)
            } else {
                let newArr = this.checkedList.filter(value => !equipNo.includes(value));
                this.checkedList = newArr
            }
        },
        isArraySubset(arr1, arr2) {
            return arr1.every(item => arr2.includes(item));
        },
        isArrayIntersection(arr1, arr2){
            return arr1.some(item => arr2.includes(item));
        }
    },
}
