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
      selectedColor: currentSelect === source.data.key&&!source.data.isGroup, //选中样式
    }" role="treeitem" tabindex="-1" :aria-expanded="expanded" :aria-disabled="source.disabled"
    :aria-checked="source.checked" ref="node">
    <div class="el-tree-node__content">
      <!-- 判断是否是分组，是分组，则宽度加长 -->
      <span class="el-tree__indent" aria-hidden="true" :style="
          !source.data.isGroup
            ? { width: (source.level - 1) * (tree.indent + 8 + 6) + 'px' }
            : { width: (source.level - 1) * (tree.indent + 7) + 'px' }
        "></span>
        <!-- 内容区域展示 -->
      <div class="nodeContent" :style="
          source.level - 1 && source.data.isGroup
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
        <el-checkbox v-if="showCheckbox" v-model="source.checked" :indeterminate="source.indeterminate"
          :disabled="!!source.disabled" @click.native.stop @change="handleCheckChange" class="myTreeCheckBox">
        </el-checkbox>
        <!-- <i v-if="showStatus" class="iconfont icondian" :class="getClass(source.data.status)"></i> -->
        <!-- 状态点：设备列表用到 -->
        <span class="circle" v-if="showStatus">
          <span class="yd" :class="getClass(source.data.status)"></span>
        </span>
        <!-- 增加loading效果，如懒加载：点击某个设备，loading加载请求测点 -->
        <span v-if="source.data.loading" class="el-tree-node__loading-icon el-icon-loading"></span>
        <node-content :node="source"></node-content>
        <!-- 操作按钮：如添加，分组重命名等 -->
        <div class="operates" v-if="source.data.isGroup && showOperate">
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

          <svg @click.stop="groupEditAndNew(true, source)" v-if="source.data.isFirst" t="1646382027233" class="icon"
            viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="22011"
            xmlns:xlink="http://www.w3.org/1999/xlink" width="48" height="48">
            <path
              d="M416 106.666667c11.093333 0 21.333333 5.76 27.157333 15.061333l1.472 2.624L499.754667 234.666667H842.666667a96 96 0 0 1 95.893333 91.477333L938.666667 330.666667v490.666666a96 96 0 0 1-91.477334 95.893334L842.666667 917.333333h-661.333334a96 96 0 0 1-95.893333-91.477333L85.333333 821.333333v-618.666666a96 96 0 0 1 91.477334-95.893334L181.333333 106.666667h234.666667z m426.666667 192H149.333333v522.666666a32 32 0 0 0 28.928 31.850667L181.333333 853.333333h661.333334a32 32 0 0 0 31.850666-28.928L874.666667 821.333333v-490.666666a32 32 0 0 0-28.928-31.850667L842.666667 298.666667z m-320 106.666666a32 32 0 0 1 31.850666 28.928L554.666667 437.333333V533.333333h96a32 32 0 0 1 3.072 63.850667L650.666667 597.333333H554.666667v96a32 32 0 0 1-63.850667 3.072L490.666667 693.333333V597.333333h-96a32 32 0 0 1-3.072-63.850666L394.666667 533.333333H490.666667v-96a32 32 0 0 1 32-32zM396.224 170.666667H181.333333a32 32 0 0 0-31.850666 28.928L149.333333 202.666667V234.666667h278.890667l-32-64z"
              p-id="22012" />
          </svg>

          <el-popover placement="bottom" trigger="hover" v-if="!source.data.isFirst">
            <el-button-group class="new-button-group setModule">
              <el-button type="text" size="small" class="elBtn" icon="el-icon- iconfont icon-tubiao20_bianji"
                @click.stop="groupEditAndNew(false, source)">
                {{ $t('equipInfo.poverTips.rename') }}
              </el-button>
              <el-button type="text" size="small" class="elBtn" icon="el-icon- iconfont icon-gw-icon-tianjia1"
                @click="groupEditAndNew(true, source)">
                {{ $t('equipInfo.poverTips.newChildGroup') }}
              </el-button>
              <el-button type="danger" size="small" @click.stop="deleteGroup(source)"
                icon="el-icon- iconfont icon-tubiao20_shanchu">
                {{ $t('publics.button.deletes') }}
              </el-button>
            </el-button-group>
            <i class="el-icon-more" @click.stop slot="reference"></i>
          </el-popover>
        </div>
        <!-- 设备数量：仅分组展示 -->
        <span class="equipNumber" v-if="source.data.isGroup && showCount">
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
            type: Number,
            default: -1
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
                            : <span class= "el-tree-node__label" > { this.$t(node.label) } < /span>
        );
            }
        }
    },
    computed: {
      // 设备状态
        getClass () {
            return function (state) {
                let className;
                switch (state) {
                    case 0:
                        className = 'dotNoComm'
                        break
                    case 1:
                        className = 'dotNormal'
                        break
                    case 2:
                        className = 'dotAlarm'
                        break
                    case 3:
                        className = 'dotLsSet'
                        break
                    case 4:
                        className = 'dotInitialize'
                        break
                    case 5:
                        className = 'dotWithdraw'
                        break
                    case 6:
                        className = 'BackUp'
                        break
                    default:
                        className = 'dotNoComm'
                        break
                }
                return className;
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