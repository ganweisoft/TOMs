<template>
    <selectV2 v-bind="$attrs" v-model="defaultValue" :value="value" :dataLabel="props.label" :dataValue="props.value"
        :data="renderList" @remoteSearch="remoteSearch" @loadmore="loadmore" size="small" />
</template>

<script>
import selectV2 from 'gw-base-components-plus/selectV2/select.vue'
export default {
    components: { selectV2 },
    model: {
        prop: 'value',
        event: 'change'
    },
    props: {
        props: {
            type: Object,
            default: () => {
                return {
                    label: 'label',
                    value: 'value'
                }
            }
        },
        url: {
            type: String,
            default: ''
        },
        resolve: {
            type: Function,
            default: () => { }
        },
        currentSelect: {
            type: Array,
            default: () => []
        },
        multi: {
            type: Boolean,
            default: false
        },
        parameters: {
            type: Object,
            default: () => { }
        },
        getAll: {
            type: Boolean,
            default: true
        },
        value: {
            type: String,
            default: ''
        }
    },

    watch: {
        defaultValue (val) {
            this.$emit('change', val)
        },
        currentSelect: {
            handler (val) {
                if (val && val[this.props.value]) {
                    this.defaultValue = val[this.props.value];
                    this.renderList.push({
                        [this.props.label]: val[this.props.label],
                        [this.props.value]: val[this.props.value]
                    })
                }
            },
            deep: true,
            immediate: true
        },
    },

    data () {
        return {
            pagination: {
                pageSize: 20,
                pageNo: 1,
                total: 0
            },
            defaultValue: '',
            isSearched: false,
            list: [],
            renderList: [],
        }
    },
    mounted () {
        // 检测到对应的模块数据有变化
        let _this =  this
        window.addEventListener('message', (e) => {
           if(e.data.pub === 'ganwei-iotcenter-asset-management') {
            this.isSearched = false;
            _this.remoteSearch('')
           }
      });
    },
    methods: {
        remoteSearch (keyword) {
            this.renderList.length = 0
            this.pagination.total = 0
            this.pagination.pageNo = 1
            this.getList(keyword)
        },
        loadmore (keyword) {
            if (!this.getAll) {
                if (this.renderList.length >= this.pagination.total && this.alreadyGetData) {
                    return
                }
                this.pagination.pageNo = this.pagination.pageNo + 1
                this.getList(keyword)
            }
        },
        filterCurrent (list, id) {
            if (id) {
                list.forEach((item, index) => {
                    if (item[this.props.value] == id && index != 0) {
                        list.splice(index, 1)
                    }
                })
            }
        },

        //  获取资产列表
        getList (keyword) {
            if (this.url && !this.isSearched && !keyword) {
                let data = {
                    pageNo: this.pagination.pageNo,
                    pageSize: this.pagination.pageSize,
                    searchName: keyword,
                    ...this.parameters
                }
                this.$api[this.url](data)
                    .then(res => {
                        let resolveData = this.resolve(res, this.pagination);
                        this.renderList = [...this.renderList, ...resolveData];
                        this.list = JSON.parse(JSON.stringify(this.renderList))
                        this.isSearched = true;
                    })
                    .catch(err => {
                        this.$message.error(err?.data, err);
                    });
            } else if (keyword) {
                this.renderList = JSON.parse(JSON.stringify(this.list.filter(item => item[this.props.label].includes(keyword))))

            } else {
                this.renderList = JSON.parse(JSON.stringify(this.list))
            }
        },

    }
}
</script>
