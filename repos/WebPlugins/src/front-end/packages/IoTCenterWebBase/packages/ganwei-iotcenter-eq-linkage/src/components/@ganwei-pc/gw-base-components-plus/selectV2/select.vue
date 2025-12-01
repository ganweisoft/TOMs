<template>
    <div>
        <el-select :size="size" :clearable="clearable" :value="defaultValue" popper-class="virtualselect" filterable
            :filter-method="filterMethod" @visible-change="visibleChange" v-bind="$attrs" v-on="$listeners"
            :popper-append-to-body="popperAppendToBody">
            <template slot="prefix">
                <slot name="prefix"></slot>
            </template>

            <template slot="suffix">
                <slot name="suffix"></slot>
            </template>
            <virtual-list ref="virtualList" @tobottom="loadmore" class="virtualselect-list" :data-key="dataValue"
                :data-sources="selectArr" :data-component="itemComponent" :keeps="20" :extra-props="{
                    label: dataLabel,
                    value: dataValue
                }"></virtual-list>
        </el-select>
    </div>
</template>
<script>
import virtualList from 'vue-virtual-scroll-list'
import ElOptionNode from './el-option-node'
function debounce (func, wait) {
    let timeout;
    return function (event) {
        let _this = this
        let args = arguments;
        if (timeout) {
            clearTimeout(timeout)
        }
        timeout = setTimeout(() => {
            timeout = null
            func.call(_this, args)
        }, wait)
    }
}
export default {
    components: {
        'virtual-list': virtualList
    },
    model: {
        prop: 'defaultValue',
        event: 'change'
    },
    props: {
        popperAppendToBody: {
            type: Boolean,
            default: false
        },
        defaultValue: {
            type: String,
            default: ''
        }, // 绑定的默认值
        dataLabel: {     // 标签label
            type: String,
            default: 'label'
        },
        dataValue: {     // 值value
            type: String,
            default: 'value'
        },
        data: {         // 数据列表
            type: Array,
            default: () => []
        },
        clearable: {
            type: Boolean,
            default: true
        },
        size: {
            type: String,
            default: 'small'
        }
    },
    mounted () {
        this.init();
    },
    watch: {
        'data' () {
            this.init();
            this.$forceUpdate()

        }
    },
    data () {
        return {
            itemComponent: ElOptionNode,
            selectArr: [],
            keyword: '',
            clearLoadingTime: null,
            loadAll: false
        }
    },
    methods: {
        init () {
            this.selectArr = this.selectArr.filter(item => !item.loading)
            this.selectArr = this.data;
            if (!this.clearLoadingTime) {
                clearInterval(this.clearLoadingTime)
                this.clearLoadingTime = null
            }
            this.loadAll = false
        },

        // 搜索
        filterMethod: debounce(function filter (query) {
            this.keyword = query[0]
            this.$emit('remoteSearch', this.keyword)
            // this.$refs.virtualList.reset();
        }, 200),
        visibleChange (bool) {
            this.$refs.virtualList.reset();
            this.keyword = ''
            if (bool) {
                this.addLoading()
                this.$emit('remoteSearch', '')
            }
        },

        addLoading () {
            if (!this.loadAll) {
                this.selectArr = this.selectArr.filter(item => !item.loading)
                this.selectArr.push({
                    loading: true,
                    [this.dataLabel]: '',
                    [this.dataValue]: new Date().getTime()
                })
                if (!this.clearLoadingTime) {
                    this.clearLoadingTime = setTimeout(() => {
                        this.selectArr = this.selectArr.filter(item => !item.loading)
                        clearInterval(this.clearLoadingTime)
                        this.clearLoadingTime = null
                        this.loadAll = true
                    }, 2000)
                }
            }

        },

        loadmore: debounce(function load () {
            this.addLoading()
            this.$emit('loadmore', this.keyword)
        }, 100)
    }
}
</script>
<style lang="scss" scoped>
/deep/ .el-select {
    position: relative;

    .el-select-dropdown {
        .el-option-loading {
            height: 18px;
            display: flex;
            justify-content: center;
            align-items: center;

            .el-icon-loading {
                font-size: 18px;
            }
        }

        .el-select-dropdown__item {
            height: auto !important;
            white-space: normal !important;
            text-overflow: unset !important;
            overflow: visible !important;
            line-height: 20px;
            margin-bottom: 10px;
            padding: 5px 20px;
        }
    }
}

.virtualselect {

    &-list {
        max-height: 245px;
        overflow-y: auto;
    }

    .el-scrollbar .el-scrollbar__bar.is-vertical {
        width: 0;
    }
}
</style>