
export default {
    props: {
        loading: {
            type: Boolean,
            default: false
        },
        tabNavNumber: {
            type: Number,
            default: 1
        },
        columnList: {
            type: Array,
            default: () => []
        }
    },
    data () {
        return {
            virtualCheckedCities: [],
            checkedCities: [],
            cities: [],
            contentLoading: false,
            resetLoading: false,
            loading: false,
            columnTableName: '',
            columnListSelected: []
        }
    },
    mounted () {
      this.getPageColumnListByPage()
    },
    watch: {
        columnList: {
            handler (val) {
                this.columnTableName = val.name
                this.cities = val.column
                this.virtualCheckedCities = Object.keys(val.column)
            },
            deep: true,
            immediate: true
        },
        checkedCities: {
            handler (val) {
                this.virtualCheckedCities = []
                this.checkedCities.forEach((item,index) => {
                     if(this.cities.indexOf(item) != -1) {
                        this.virtualCheckedCities.push(this.cities.indexOf(item).toString())
                     }
                });
                // 触发更新逻辑
                this.$bus.emit(this.columnTableName, this.virtualCheckedCities)
            }
        },
        tabNavNumber: {
            handler (val) {
                this.getPageColumnListByPage()
            }
        }
    },
    methods: {
        allSelect () {
            this.resetLoading = true
            this.checkedCities = JSON.parse(JSON.stringify(this.cities));
            setTimeout(()=>{this.addPageColumn()}, 1000)
        },
        getPageColumnListByPage(){
            this.contentLoading = true
            let data = {
                PageCode: this.columnTableName
            }
            this.$api.getPageColumnListByPage(data).then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200 && Array.isArray(data.rows)) {
                    let arry = []
                    arry = data.rows[0]?.columnCode || []
                    this.handlerCheckedCities(arry)
                } else {
                    this.$message.error(message)
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            }).finally(r=>{
                this.contentLoading = false
            })
        },
        addPageColumn(){
            if(!this.resetLoading)
              this.loading = true;
            let data = {
                PageCode: this.columnTableName,
                ColumnCode: JSON.stringify(this.virtualCheckedCities),
                viewState: true
            }
            this.$api.addPageColumn(data).then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    this.$message.success(message)
                } else {
                    this.$message.error(message)
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            }).finally(() => {
                this.loading = false
                this.resetLoading = false
                this.$refs.popover.doClose(); // 关闭弹窗
            });
        },

        handlerCheckedCities (val) {
            let obj = []
            try{obj = JSON.parse(val)}catch(e){}
            if (obj.length == 0) {
                this.checkedCities = JSON.parse(JSON.stringify(this.cities));

            } else {
                let arry = []
                obj.forEach((item,index) => {
                    arry.push(this.cities[item])
                })
                this.checkedCities = arry
            }
        }
    }
}
