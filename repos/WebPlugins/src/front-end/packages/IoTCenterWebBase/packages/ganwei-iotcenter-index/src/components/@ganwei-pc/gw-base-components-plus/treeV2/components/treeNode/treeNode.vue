<template>
    <div class="el-tree-node" @click="clickFunction" :class="{
        parent_tag: source.isGroup,
        child_tags: !source.isGroup,
        selectedColor: currentSelect === source.key && !source.isGroup,
    }">
        <div class="el-tree-node__content">
            <span class="el-tree__indent" aria-hidden="true"
                :style="{ width: (source.level - 1) * (indent) + 'px' }"></span>

            <span v-if="source.isGroup" :class="[
                {
                    'is-leaf': !source.isGroup,
                    expanded: source.isGroup && source.expand,
                },
                'el-tree-node__expand-icon',
                'el-icon-arrow-right',
            ]"></span>
            <div class="nodeContent">
                <el-checkbox @click.native.stop v-model="source.checked" :indeterminate="source.indeterminate"
                    v-if="showCheckbox" @change="checkedChange"></el-checkbox>
                <span class="circle" v-if="showStatus">
                    <span class="yd" v-if="source.status != 6" :style="{ backgroundColor: getColor(source.status) }"></span>
                    <i v-else :style="{ color: colorConfig.BackUp }" class="iconfont icon-gw-icon-beiji2"></i>
                </span>
                <!-- <span class="label"> {{ source.title }}--{{ source.equipSelectCount }}</span> -->
                <span class="label"> {{ source.title }}</span>
                <span class="equipNumber" v-if="(source.isGroup && !source.isEquip) || (source.isGroup && !source.isEquip)">
                    {{ source.count }}
                </span>
                <!-- 操作按钮：如添加，分组重命名等 -->
                <oparate v-if="source.isGroup && showOperate" :source="source" :groupEditAndNew="groupEditAndNew"
                    :deleteGroup="deleteGroup" />

                <span v-if="source.loading" class="el-tree-node__loading-icon el-icon-loading"> </span>
            </div>
        </div>

    </div>
</template>

<script>
import oparate from '../oparate/oparate.vue'

export default {
    name: 'ElTreeVirtualNode',
    componentName: 'ElTreeVirtualNode',
    props: {
        searchName: {
            type: String,
            default: ''
        },
        expanding: {
            type: Boolean,
            default: false
        },
        aleadyLoadAll: {
            type: Boolean,
            default: false
        },
        groupLoading: {
            type: Boolean,
            default: false
        },
        source: {
            type: Object,
            default () {
                return {};
            }
        },
        showCheckbox: {
            type: Boolean,
            default: false
        },
        showStatus: {
            type: Boolean,
            default: false
        },
        showOperate: {
            type: Boolean,
            default: false,
        },
        currentSelect: {
            type: [Number , String],
            default: -1
        },
        nodeClick: {
            type: Function
        },
        onChecked: {
            type: Function
        },
        groupEditAndNew: {
            type: Function
        },
        deleteGroup: {
            type: Function
        },
        colorConfig: {
            type: Object,
            default: () => { }
        },
        index: {
            type: Number,
            default: -1
        }
    },

    components: {
        oparate
    },
    computed: {
        getColor () {
            return function (state) {
                let color;
                switch (state) {
                    case 0:
                        color = this.colorConfig['noComm']
                        break
                    case 1:
                        color = this.colorConfig['normal']
                        break
                    case 2:
                        color = this.colorConfig['alarm']
                        break
                    case 3:
                        color = this.colorConfig['lsSet']
                        break
                    case 4:
                        color = this.colorConfig['initialize']
                        break
                    case 5:
                        color = this.colorConfig['withdraw']
                        break
                    case 6:
                        color = this.colorConfig['BackUp']
                        break
                    default:
                        color = this.colorConfig['noComm']
                        break
                }
                return color;
            };
        },
    },
    data () {
        return {
            indent: 13
        };
    },
    methods: {
        clickFunction () {
            this.source.expand = !this.source.expand
            this.nodeClick(this.source, this.index, this.source.level, this.source.checked)
        },
        checkedChange (val) {
            this.onChecked(this.source)
        },


    }
};
</script>
