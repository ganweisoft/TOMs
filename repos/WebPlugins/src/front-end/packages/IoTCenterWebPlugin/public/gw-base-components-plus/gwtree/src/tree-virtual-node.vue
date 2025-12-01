<template>
    <div class="el-tree-node" @click.stop="handleClick" @contextmenu="($event) => this.handleContextMenu($event)"
        v-show="source.visible" :class="{
            'is-expanded': expanded,
            'is-current': source.isCurrent,
            'is-hidden': !source.visible,
            'is-focusable': !source.disabled,
            'is-checked': !source.disabled && source.checked,
            parent_tag: source.data.isGroup,
            child_tags: !source.data.isGroup,
            selectedColor: currentSelect === source.data.key && !source.data.isGroup, //选中样式
            'notAllow': true
        }" role="treeitem" tabindex="-1" :aria-expanded="expanded" :aria-disabled="source.disabled"
        :aria-checked="source.checked" ref="node">

        <!-- <div v-if="source.data.isGroup&&!source.data.isEquip&&(expanding||groupLoading)" class="block" @click.stop>
        </div> -->
        <div class="el-tree-node__content">
            <!-- 判断是否是分组，是分组，则宽度加长 -->
            <span class="el-tree__indent" aria-hidden="true" :style="!source.data.isGroup
                    ? { width: (source.level - 1) * (tree.indent + 8 + 6) + 'px' }
                    : { width: (source.level - 1) * (tree.indent + 7) + 'px' }
                "></span>
            <!-- 内容区域展示 -->
            <div class="nodeContent" :style="source.level - 1 && source.data.isGroup
                    ? { 'margin-left': '8px' }
                    : {}
                ">
                <!-- 如果是分组，则加左侧icon -->
                <span v-if="source.data.isGroup" @click.stop="handleExpandIconClick" :class="[
                    {
                        'is-leaf': !source.data.isGroup,
                        expanded: source.data.isGroup && expanded,
                    },
                    'el-tree-node__expand-icon',
                    tree.iconClass ? tree.iconClass : 'el-icon-caret-right',
                ]"></span>
                <!-- 选框 -->
                <el-checkbox v-if="showCheckbox && aleadyLoadAll" v-model="source.checked"
                    :indeterminate="source.indeterminate" :disabled="!!source.disabled" @click.native.stop
                    @change="handleCheckChange" class="myTreeCheckBox">
                </el-checkbox>

                <!-- 状态点：设备列表用到 -->
                <span class="circle" v-if="showStatus">
                    <!-- <span class="yd" :class="getClass(source.data.status)"></span> -->
                    <svg v-if="source.data.status == 6" t="1681887306865" class="icon" viewBox="0 0 1024 1024" version="1.1"
                        xmlns="http://www.w3.org/2000/svg" p-id="17559" xmlns:xlink="http://www.w3.org/1999/xlink"
                        width="200" height="200">
                        <path
                            d="M993.581176 699.873882v75.113412c-0.240941 137.216-215.883294 248.350118-481.701647 248.350118-265.999059 0-481.460706-111.254588-481.761882-248.350118V699.934118c89.871059 98.725647 269.854118 164.743529 481.761882 164.743529s391.710118-66.017882 481.701647-164.743529zM30.117647 479.292235c23.009882 25.178353 52.103529 48.188235 85.895529 68.668236l3.855059 2.048c12.528941 7.408941 25.901176 14.576941 39.755294 21.202823 6.866824 3.312941 14.155294 6.384941 21.323295 9.517177a643.915294 643.915294 0 0 0 53.910588 20.841411c14.215529 4.818824 28.912941 9.035294 43.971764 13.071059 5.240471 1.385412 10.360471 2.891294 15.721412 4.276706 18.672941 4.577882 38.068706 8.553412 57.825883 11.866353a1032.613647 1032.613647 0 0 0 68.909176 8.794353c6.866824 0.602353 13.793882 1.264941 20.781177 1.807059 22.889412 1.686588 46.200471 2.770824 69.933176 2.770823 23.853176 0 46.983529-1.084235 69.872941-2.770823 7.047529-0.421647 13.974588-1.084235 20.841412-1.807059 16.263529-1.505882 32.105412-3.252706 47.826823-5.601882 7.047529-1.144471 14.095059-1.927529 20.961883-3.192471 19.877647-3.312941 39.454118-7.288471 58.187294-11.866353 5.240471-1.204706 10.059294-2.770824 15.179294-4.096 15.058824-4.035765 29.816471-8.312471 44.152471-13.131294 7.710118-2.590118 15.119059-5.421176 22.588235-8.131765 10.782118-3.975529 21.082353-8.252235 31.322353-12.649412 7.168-3.192471 14.456471-6.264471 21.323294-9.517176a537.298824 537.298824 0 0 0 39.755294-21.263059l3.674353-2.048 16.564706-10.36047a356.171294 356.171294 0 0 0 69.270588-58.307765v75.113412c0 29.936941-10.962824 58.428235-29.81647 85.11247a212.992 212.992 0 0 1-15.299765 19.034353l-1.92753 2.048c-20.841412 22.588235-47.887059 43.128471-80.293647 61.31953l-6.625882 3.734588c-6.445176 3.433412-13.251765 6.746353-19.937882 10.059294-3.734588 1.686588-7.348706 3.493647-11.023059 5.12a521.878588 521.878588 0 0 1-33.792 13.914353c-6.384941 2.409412-12.649412 4.818824-19.275294 7.047529-6.384941 2.228706-13.131294 4.156235-19.757177 6.204236a670.418824 670.418824 0 0 1-52.946823 14.215529c-5.12 1.084235-10.480941 2.048-15.841883 3.132235a613.315765 613.315765 0 0 1-42.044235 7.469177c-8.854588 1.385412-17.648941 2.469647-26.624 3.614117l-16.685176 1.807059c-9.938824 0.903529-19.877647 1.626353-29.936942 2.288941-4.818824 0.301176-9.637647 0.722824-14.45647 0.963765a824.018824 824.018824 0 0 1-90.955294 0c-4.879059-0.240941-9.697882-0.662588-14.516706-0.963765-10.059294-0.662588-20.118588-1.385412-29.936941-2.349176-5.662118-0.542118-11.143529-1.084235-16.685177-1.807059-8.975059-1.084235-17.769412-2.288941-26.624-3.614118a613.376 613.376 0 0 1-42.044235-7.408941 644.638118 644.638118 0 0 1-15.841882-3.132235 822.814118 822.814118 0 0 1-53.007059-14.215529c-6.565647-2.048-13.191529-3.975529-19.636706-6.204236-6.625882-2.228706-13.010824-4.698353-19.275294-7.047529a433.573647 433.573647 0 0 1-33.852236-13.914353 540.190118 540.190118 0 0 1-31.021176-15.179294c-2.349176-1.204706-4.397176-2.469647-6.625882-3.734588-32.346353-18.191059-59.392-38.731294-80.233412-61.31953a214.618353 214.618353 0 0 1-15.420235-18.733176l-1.686589-2.409412C41.020235 613.195294 30.117647 584.523294 30.117647 554.586353zM511.879529 0c266.059294 0 481.701647 111.254588 481.701647 248.591059H993.882353v85.534117c0 29.876706-10.842353 58.428235-29.756235 85.052236-0.602353 0.662588-1.144471 1.505882-1.686589 2.288941a212.992 212.992 0 0 1-13.613176 16.685176l-1.987765 2.108236c-20.781176 22.588235-47.826824 43.128471-80.233412 61.319529-2.168471 1.204706-4.216471 2.529882-6.625882 3.734588-6.445176 3.433412-13.251765 6.746353-19.998118 10.059294l-10.842352 5.12c-6.505412 2.891294-13.010824 5.782588-19.757177 8.553412l-14.034823 5.421177c-6.505412 2.409412-12.830118 4.939294-19.456 7.107764-6.324706 2.048-12.950588 4.035765-19.576471 6.02353-6.023529 1.807059-11.866353 3.794824-18.070588 5.421176l-0.662589 0.120471c-1.807059 0.421647-3.855059 0.843294-5.662117 1.385412-9.396706 2.469647-18.913882 4.999529-28.672 7.228235-2.228706 0.361412-4.276706 0.903529-6.324706 1.325176-1.686588 0.421647-3.312941 0.722824-4.818824 0.963765a480.376471 480.376471 0 0 1-30.057411 5.782588c-2.951529 0.421647-5.842824 1.144471-8.734118 1.566118l-8.432941 1.084235c-8.673882 1.204706-17.468235 2.349176-26.322824 3.433412l-10.601411 1.385412-6.625883 0.602353c-9.637647 0.903529-19.395765 1.626353-29.334588 2.288941-3.855059 0.301176-7.469176 0.662588-11.324235 0.843294-1.204706 0.120471-2.349176 0.120471-3.614118 0.120471a975.570824 975.570824 0 0 1-90.413176 0c-1.204706-0.120471-2.349176-0.120471-3.614118-0.120471-3.855059-0.180706-7.408941-0.602353-11.264-0.843294a974.727529 974.727529 0 0 1-29.394824-2.349177l-6.625882-0.542117-10.601412-1.385412a807.152941 807.152941 0 0 1-26.322823-3.433412c-2.770824-0.421647-5.662118-0.722824-8.432941-1.084235a1175.250824 1175.250824 0 0 1-33.912471-6.324706c-1.626353-0.481882-3.312941-0.722824-4.939294-1.024l-4.577883-0.783059c-2.168471-0.421647-4.216471-1.024-6.324705-1.385411-9.818353-2.228706-19.275294-4.698353-28.672-7.228236a67.764706 67.764706 0 0 1-5.662118-1.325176c-0.301176 0-0.421647-0.180706-0.722824-0.180706-6.204235-1.626353-11.986824-3.614118-18.070588-5.360941a405.865412 405.865412 0 0 1-38.972235-13.251765c-4.698353-1.807059-9.517176-3.433412-14.095059-5.360941-6.746353-2.770824-13.251765-5.662118-19.696941-8.553412a540.069647 540.069647 0 0 1-30.900706-15.179294l-6.625882-3.674353c-32.346353-18.251294-59.392-38.791529-80.233412-61.379765A214.618353 214.618353 0 0 1 61.560471 421.647059l-1.686589-2.409412C41.020235 392.854588 30.117647 364.182588 30.117647 334.245647V248.591059C30.117647 111.254588 245.76 0 511.879529 0z"
                            :fill="colorConfig.BackUp" p-id="17560">
                        </path>
                    </svg>
                    <span v-else class="yd" :style="{ backgroundColor: getColor(source.data.status) }"></span>
                </span>
                <!-- 增加loading效果，如懒加载：点击某个设备，loading加载请求测点 -->
                <span v-if="source.data.loading || (source.data.isGroup && groupLoading && source.level == 1)"
                    class="el-tree-node__loading-icon el-icon-loading"></span>
                <node-content :node="source"></node-content>
                <!-- 操作按钮：如添加，分组重命名等 -->
                <div class="operates" v-if="source.data.isGroup && showOperate && !searchName">
                    <el-popover placement="bottom" trigger="hover" v-if="needAdd">
                        <el-button-group class="new-button-group setModule">
                            <el-button type="primary text" class="elBtn" size="small" @click="newEquip(source)">
                                {{ $t('equipInfo.poverTips.newDevice') }}
                            </el-button>
                            <el-button type="primary text" class="elBtn" size="small" @click="templateNewEquip(source)">
                                {{ $t('equipInfo.poverTips.templateNew') }}
                            </el-button>
                        </el-button-group>
                        <i class="iconfont icon16_tianjia" @click.stop slot="reference"></i>
                    </el-popover>
                    <!-- <i class="iconfont icontianjiafenzu" @click.stop="groupEditAndNew(true, source)" v-if="source.data.isFirst"></i> -->

                    <!-- <svg @click.stop="groupEditAndNew(true, source)" v-if="source.level==1" t="1646382027233" class="icon"
            viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="22011"
            xmlns:xlink="http://www.w3.org/1999/xlink" width="48" height="48">
            <path
              d="M416 106.666667c11.093333 0 21.333333 5.76 27.157333 15.061333l1.472 2.624L499.754667 234.666667H842.666667a96 96 0 0 1 95.893333 91.477333L938.666667 330.666667v490.666666a96 96 0 0 1-91.477334 95.893334L842.666667 917.333333h-661.333334a96 96 0 0 1-95.893333-91.477333L85.333333 821.333333v-618.666666a96 96 0 0 1 91.477334-95.893334L181.333333 106.666667h234.666667z m426.666667 192H149.333333v522.666666a32 32 0 0 0 28.928 31.850667L181.333333 853.333333h661.333334a32 32 0 0 0 31.850666-28.928L874.666667 821.333333v-490.666666a32 32 0 0 0-28.928-31.850667L842.666667 298.666667z m-320 106.666666a32 32 0 0 1 31.850666 28.928L554.666667 437.333333V533.333333h96a32 32 0 0 1 3.072 63.850667L650.666667 597.333333H554.666667v96a32 32 0 0 1-63.850667 3.072L490.666667 693.333333V597.333333h-96a32 32 0 0 1-3.072-63.850666L394.666667 533.333333H490.666667v-96a32 32 0 0 1 32-32zM396.224 170.666667H181.333333a32 32 0 0 0-31.850666 28.928L149.333333 202.666667V234.666667h278.890667l-32-64z"
              p-id="22012" />
          </svg> -->

                    <el-popover placement="bottom" trigger="hover" v-if="source.level == 1">
                        <el-button-group class="new-button-group setModule">
                            <el-button type="text" size="small" class="elBtn" icon="iconfont icon-tubiao20_bianji"
                                @click.stop="groupEditAndNew(false, source)">
                                {{ $t('equipInfo.poverTips.rename') }}
                            </el-button>
                            <el-button type="text" size="small" class="elBtn" icon="iconfont icon-gw-icon-tianjia1"
                                @click="groupEditAndNew(true, source)">
                                {{ $t('equipInfo.poverTips.newChildGroup') }}
                            </el-button>
                            <!-- <el-button type="danger" size="small" @click.stop="deleteGroup(source)"
                icon="el-icon- iconfont icon-tubiao20_shanchu">
                {{ $t('publics.button.deletes') }}
              </el-button> -->
                        </el-button-group>
                        <i class="el-icon-more" @click.stop slot="reference"></i>
                    </el-popover>

                    <el-popover placement="bottom" trigger="hover" v-if="source.level != 1">
                        <el-button-group class="new-button-group setModule">
                            <el-button type="text" size="small" class="elBtn" icon="iconfont icon-tubiao20_bianji"
                                @click.stop="groupEditAndNew(false, source)">
                                {{ $t('equipInfo.poverTips.rename') }}
                            </el-button>
                            <el-button type="text" size="small" class="elBtn" icon="iconfont icon-gw-icon-tianjia1"
                                @click="groupEditAndNew(true, source)">
                                {{ $t('equipInfo.poverTips.newChildGroup') }}
                            </el-button>
                            <el-button type="danger" size="small" @click.stop="deleteGroup(source)"
                                icon="iconfont icon-tubiao20_shanchu">
                                {{ $t('publics.button.deletes') }}
                            </el-button>
                        </el-button-group>
                        <i class="el-icon-more" @click.stop slot="reference"></i>
                    </el-popover>
                </div>
                <!-- 设备数量：仅分组展示 -->
                <span class="equipNumber"
                    v-if="(source.data.isGroup && !source.data.isEquip && showCount) || (source.data.isGroup && !source.data.isEquip && searchName)">
                    {{ source.data.count }}
                </span>
            </div>
        </div>
    </div>
</template>

<script type="text/jsx">
import ElCheckbox from 'element-ui/packages/checkbox';
import emitter from 'element-ui/src/mixins/emitter';
import mixinNode from './mixin/node';
import { getNodeKey } from './model/util';

export default {
    name: 'ElTreeVirtualNode',
    componentName: 'ElTreeVirtualNode',

    mixins: [emitter, mixinNode],

    props: {
        colorConfig: {
            type: Object,
            default: () => { }
        },
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
        needAdd: {
            type: Boolean,
            default () {
                return false;
            }
        },
        source: {
            default () {
                return {};
            }
        },
        renderContent: Function,
        showCheckbox: {
            type: Boolean,
            default: false
        },
        showCount: {
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
            type: [Number, String],
            default: ''
        }
    },

    components: {
        ElCheckbox,
        NodeContent: {
            props: {
                node: {
                    required: true
                }
            },
            render (h) {
                const parent = this.$parent;
                const tree = parent.tree;
                const node = this.node;
                const { data, store } = node;
                return (
                    parent.renderContent
                        ? parent.renderContent.call(parent._renderProxy, h, { _self: tree.$vnode.context, node, data, store })
                        : tree.$scopedSlots.default
                            ? tree.$scopedSlots.default({ node, data })
                            : <span class="el-tree-node__label" > {this.$t(node.label)} </span>
                );
            }
        }
    },
    computed: {
        // 设备状态
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
            tree: null,
            expanded: false,
            childNodeRendered: false,
            oldChecked: null,
            oldIndeterminate: null
        };
    },

    watch: {
        'source.indeterminate' (val) {
            this.handleSelectChange(this.source.checked, val);
        },
        'source.checked' (val) {
            this.handleSelectChange(val, this.source.indeterminate);
        },

        'source.expanded' (val) {
            this.$nextTick(() => this.expanded = val);
            if (val) {
                this.childNodeRendered = true;
            }
        }
    },

    created () {
        const parent = this.$parent.$parent.$parent;
        this.creator(parent, 'source');
    }
};
</script>