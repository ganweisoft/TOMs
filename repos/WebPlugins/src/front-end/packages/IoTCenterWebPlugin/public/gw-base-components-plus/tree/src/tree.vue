<template>
  <div class="gw-tree">
    <div v-if="showSelectNum" class="selectNum">
      <el-checkbox :indeterminate="isIndeterminate" v-model="checkAll" @change="handleCheckAllChange"></el-checkbox>
      {{$t('publics.tips.selected')}}{{ selectNum }}{{$t('publics.unit.set')}}
    </div>

    <div class="el-tree" v-loading="loading" :id="id" :class="{
      'el-tree--highlight-current': highlightCurrent,
      'is-dragging': !!dragState.draggingNode,
      'is-drop-not-allow': !dragState.allowDrop,
      'is-drop-inner': dragState.dropType === 'inner',
    }" :style="{
  height: showSelectNum ? 'calc(100% - 36px)' : height + '%',
  'overflow-y': 'auto',
  'overflow-x': 'hidden',
}" role="tree">
      <virtual-list v-if="!isEmpty" class="virtualList" :class="hoverClass" :data-key="getNodeKey"
        :data-sources="visibleList" :data-component="itemComponent" :keeps="showNumber" :extra-props="{
          renderAfterExpand,
          showCheckbox,
          renderContent,
          onNodeExpand: handleNodeExpand,
          showCount,
          showStatus,
          showOperate,
          currentSelect,
          needAdd,
        }" />

      <!--暂无数据展示  -->
      <div class="el-tree__empty-block" v-if="isEmpty && !loading">
        <span class="el-tree__empty-text">{{ emptyText }}</span>
      </div>
    </div>
  </div>
</template>

<script>
import TreeStore from './model/tree-store'
import VirtualList from 'vue-virtual-scroll-list'
import { getNodeKey, findNearestComponent } from './model/util'
import ElTreeNode from './tree-node.vue'
import ElVirtualNode from './tree-virtual-node.vue'
import { t } from 'element-ui/src/locale'
import emitter from 'element-ui/src/mixins/emitter'
import { addClass, removeClass } from 'element-ui/src/utils/dom'

export default {
  name: 'ElTree',

  mixins: [emitter],

  components: {
    VirtualList,
    ElTreeNode,
  },

  data() {
    return {
      store: null,
      root: null,
      currentNode: null,
      treeItems: null,
      checkboxItems: [],
      dragState: {
        showDropIndicator: false,
        draggingNode: null,
        dropNode: null,
        allowDrop: true,
      },
      itemComponent: ElVirtualNode,
      expandKeys: [],
      currentSelect: -1,
      equipObject: {},
      nodeObject: {},
      alarmList: [],
      isIndeterminate: false,
      checkAll: false,

      hoverClass: '',

      dialogBodyOuter: null,
      dialogBodyInner: null,
      showNumber: 0,
      id: ''
    }
  },

  props: {
    loading: {
      type: Boolean,
      defualt: false
    },
    showSelectNum: {
      type: Boolean,
      default() {
        return false
      },
    },
    needAdd: {
      type: Boolean,
      default() {
        return false
      },
    },
    data: {
      type: Array,
    },
    emptyText: {
      type: String,
      default() {
        return t('el.tree.emptyText')
      },
    },
    renderAfterExpand: {
      type: Boolean,
      default: true,
    },
    nodeKey: String,
    checkStrictly: { type: Boolean, default: false },
    defaultExpandAll: {
      type: Boolean,
      default: false,
    },
    expandOnClickNode: {
      type: Boolean,
      default: true,
    },
    checkOnClickNode: Boolean,
    checkDescendants: {
      type: Boolean,
      default: false,
    },
    autoExpandParent: {
      type: Boolean,
      default: true,
    },
    defaultCheckedKeys: Array,
    defaultExpandedKeys: Array,
    currentNodeKey: [String, Number],
    renderContent: Function,
    showCheckbox: {
      type: Boolean,
      default: false,
    },
    showCount: {
      // 是否展示分组数量
      type: Boolean,
      default: false,
    },
    showStatus: {
      //是否展示设备状态
      type: Boolean,
      default: false,
    },
    draggable: {
      type: Boolean,
      default: false,
    },
    showOperate: {
      // 是否展示操作按钮
      type: Boolean,
      default: false,
    },
    allowDrag: Function,
    allowDrop: Function,
    props: {
      default() {
        return {
          children: 'children',
          label: 'title',
          disabled: 'disabled',
        }
      },
    },
    lazy: {
      type: Boolean,
      default: false,
    },
    highlightCurrent: Boolean,
    load: Function,
    filterNodeMethod: Function,
    accordion: Boolean,
    indent: {
      type: Number,
      default: 25,
    },
    iconClass: {
      type: String,
      default: 'el-icon-arrow-right',
    },
    height: {
      type: Number,
      default: 100,
    },
    extraLine: {
      type: Number,
      default: 8,
    },
  },

  computed: {
    children: {
      set(value) {
        this.data = value
      },
      get() {
        return this.data
      },
    },

    treeItemArray() {
      return Array.prototype.slice.call(this.treeItems)
    },

    isEmpty() {
      const { childNodes } = this.root
      return (
        !childNodes ||
        childNodes.length === 0 ||
        childNodes.every(({ visible }) => !visible)
      )
    },

    visibleList() {
      return this.flattenTree(this.root.childNodes)
    },
    selectNum() {
      return this.getCheckedNodes(true).reduce((pre, item) => {
        if (!item.isGroup) {
          pre++
        }
        return pre
      }, 0)
    },
  },

  watch: {
    defaultCheckedKeys(newVal) {
      this.store.setDefaultCheckedKey(newVal)
    },

    expandKeys(newVal) {
      this.store.defaultExpandedKeys = newVal
      this.store.setDefaultExpandedKeys(newVal)
    },

    data(newVal, oldVal) {
      if (
        this.data.length > 0 &&
        !this.expandKeys.includes(this.data[0][this.nodeKey])
      ) {
        this.expandKeys.push(this.data[0][this.nodeKey])
      }
      this.formateEquips(this.data)
      this.store.setData(newVal)
    },

    checkboxItems(val) {
      Array.prototype.forEach.call(val, (checkbox) => {
        checkbox.setAttribute('tabindex', -1)
      })
    },

    checkStrictly(newVal) {
      this.store.checkStrictly = newVal
    },
  },

  methods: {
    // 生成唯一Id
    generateUUID() {
      let d = new Date().getTime();
      if (window.performance && typeof window.performance.now === 'function') {
        d += performance.now(); // use high-precision timer if available
      }
      let uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        let r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c === 'x' ? r : (r & 0x3) | 0x8).toString(16);
      });
      return uuid;
    },
    // 设置样式自适应
    setClass() {
      this.dialogBodyOuter = document.getElementsByClassName('el-tree')[0]
      this.dialogBodyInner = document.getElementsByClassName('virtualList')[0]
      console.log(
        this.dialogBodyOuter.offsetHeight,
        this.dialogBodyInner.offsetHeight,
      )
      if (this.dialogBodyOuter && this.dialogBodyInner) {
        if (
          this.dialogBodyOuter.offsetHeight < this.dialogBodyInner.offsetHeight
        ) {
          this.hoverClass = 'hover-add-width'
        } else {
          this.hoverClass = ''
        }
      }
    },
    handleCheckAllChange() {
      if (this.isIndeterminate) {
        this.checkAll = true
        this.isIndeterminate = false
      }

      this.root.childNodes.forEach((item) => {
        this.setChecked(item.data.key, this.checkAll, true)
      })
    },

    setCheckAllFalse() {
      this.checkAll = false
      this.isIndeterminate = false
    },

    formateEquips(list, parent) {
      for (const item of list) {
        if (!item.children) {
          this.equipObject[item.key] = {}
          this.equipObject[item.key] = item
          this.equipObject[item.key].parent = parent
        }
        if (item.children) {
          this.nodeObject[item.key] = {}
          this.nodeObject[item.key] = item
          this.nodeObject[item.key].children = item.children
          if (parent) {
            this.nodeObject[item.key].parent = parent
          }
          this.formateEquips(item.children, item)
        }
      }
    },
    flattenTree(datas) {
      return datas.reduce((conn, data) => {
        conn.push(data)
        if (data.expanded && data.childNodes.length) {
          conn.push(...this.flattenTree(data.childNodes))
        }

        return conn
      }, [])
    },
    filter(value) {
      if (!this.filterNodeMethod)
        throw new Error('[Tree] filterNodeMethod is required when filter')
      this.store.filter(value)
    },

    getNodeKey(node) {
      return getNodeKey(this.nodeKey, node.data)
    },

    getNodePath(data) {
      if (!this.nodeKey)
        throw new Error('[Tree] nodeKey is required in getNodePath')
      const node = this.store.getNode(data)
      if (!node) return []
      const path = [node.data]
      let parent = node.parent
      while (parent && parent !== this.root) {
        path.push(parent.data)
        parent = parent.parent
      }
      return path.reverse()
    },

    getCheckedNodes(leafOnly, includeHalfChecked) {
      return this.store.getCheckedNodes(leafOnly, includeHalfChecked)
    },

    getCheckedKeys(leafOnly) {
      return this.store.getCheckedKeys(leafOnly)
    },

    getCurrentNode() {
      const currentNode = this.store.getCurrentNode()
      return currentNode ? currentNode.data : null
    },

    getCurrentKey() {
      if (!this.nodeKey)
        throw new Error('[Tree] nodeKey is required in getCurrentKey')
      const currentNode = this.getCurrentNode()
      return currentNode ? currentNode[this.nodeKey] : null
    },

    setCheckedNodes(nodes, leafOnly) {
      if (!this.nodeKey)
        throw new Error('[Tree] nodeKey is required in setCheckedNodes')
      this.store.setCheckedNodes(nodes, leafOnly)
    },

    setCheckedKeys(keys, leafOnly, halfArray) {
      if (!this.nodeKey)
        throw new Error('[Tree] nodeKey is required in setCheckedKeys')
      this.store.setCheckedKeys(keys, leafOnly, halfArray)
    },

    setChecked(data, checked, deep) {
      this.store.setChecked(data, checked, deep)
    },

    getHalfCheckedNodes() {
      return this.store.getHalfCheckedNodes()
    },

    getHalfCheckedKeys() {
      return this.store.getHalfCheckedKeys()
    },

    setCurrentNode(node) {
      if (!this.nodeKey)
        throw new Error('[Tree] nodeKey is required in setCurrentNode')
      this.store.setUserCurrentNode(node)
    },

    setCurrentKey(key) {
      if (!this.nodeKey)
        throw new Error('[Tree] nodeKey is required in setCurrentKey')
      this.currentSelect = Number(key)
      this.store.setCurrentNodeKey(key)
    },

    getNode(data) {
      return this.store.getNode(data)
    },

    remove(data) {
      this.store.remove(data)
    },

    append(data, parentNode) {
      this.store.append(data, parentNode)
    },

    insertBefore(data, refNode) {
      this.store.insertBefore(data, refNode)
    },

    insertAfter(data, refNode) {
      this.store.insertAfter(data, refNode)
    },

    handleNodeExpand(nodeData, node, instance) {
      this.broadcast('ElTreeNode', 'tree-node-expand', node)
      this.$emit('node-expand', nodeData, node, instance)
    },

    updateKeyChildren(key, data) {
      if (!this.nodeKey)
        throw new Error('[Tree] nodeKey is required in updateKeyChild')
      this.store.updateChildren(key, data)
    },

    initTabIndex() {
      this.treeItems = this.$el.querySelectorAll('.is-focusable[role=treeitem]')
      this.checkboxItems = this.$el.querySelectorAll('input[type=checkbox]')
      const checkedItem = this.$el.querySelectorAll(
        '.is-checked[role=treeitem]',
      )
      if (checkedItem.length) {
        checkedItem[0].setAttribute('tabindex', 0)
        return
      }
      this.treeItems[0] && this.treeItems[0].setAttribute('tabindex', 0)
    },

    handleKeydown(ev) {
      const currentItem = ev.target
      if (currentItem.className.indexOf('el-tree-node') === -1) return
      const keyCode = ev.keyCode
      this.treeItems = this.$el.querySelectorAll('.is-focusable[role=treeitem]')
      const currentIndex = this.treeItemArray.indexOf(currentItem)
      let nextIndex
      if ([38, 40].indexOf(keyCode) > -1) {
        // up、down
        ev.preventDefault()
        if (keyCode === 38) {
          // up
          nextIndex = currentIndex !== 0 ? currentIndex - 1 : 0
        } else {
          nextIndex =
            currentIndex < this.treeItemArray.length - 1 ? currentIndex + 1 : 0
        }
        this.treeItemArray[nextIndex].focus() // 选中
      }
      if ([37, 39].indexOf(keyCode) > -1) {
        // left、right 展开
        ev.preventDefault()
        currentItem.click() // 选中
      }
      const hasInput = currentItem.querySelector('[type="checkbox"]')
      if ([13, 32].indexOf(keyCode) > -1 && hasInput) {
        // space enter选中checkbox
        ev.preventDefault()
        hasInput.click()
      }
    },
    removeExpand(children) {
      children.forEach((item) => {
        let index = this.expandKeys.indexOf(item.key)
        if (index != -1) {
          this.expandKeys.splice(index, 1)
        }
        if (item.children) {
          this.removeExpand(item.children)
        }
      })
    },
  },

  created() {
    // 给属性结构生成唯一id，当页面用多个树形结构时，用来获取高度，计算展示节点数量
    this.id = this.generateUUID()
    this.$on('check', () => {
      let indeterminate = 0
      let checked = 0
      this.root.childNodes.forEach((node) => {
        node.checked && checked++
        node.indeterminate && indeterminate++
      })
      if (checked == 0 && indeterminate == 0) {
        this.checkAll = false
        this.isIndeterminate = false
      } else {
        if (checked === this.root.childNodes.length) {
          this.isIndeterminate = false
          this.checkAll = true
        } else {
          this.isIndeterminate = true
        }
      }
    })
    this.isTree = true

    this.store = new TreeStore({
      key: this.nodeKey,
      data: this.data,
      lazy: this.lazy,
      props: this.props,
      load: this.load,
      currentNodeKey: this.currentNodeKey,
      checkStrictly: this.checkStrictly,
      checkDescendants: this.checkDescendants,
      defaultCheckedKeys: this.defaultCheckedKeys,
      defaultExpandedKeys: this.expandKeys,
      autoExpandParent: this.autoExpandParent,
      defaultExpandAll: this.defaultExpandAll,
      filterNodeMethod: this.filterNodeMethod,
    })

    this.$on('saveOpenStatus', (node) => {
      if (node.data.isGroup) {
        if (node.expanded) {
          this.expandKeys.push(node.data.key)
        } else {
          let index = this.expandKeys.indexOf(node.data.key)
          if (index != -1) {
            this.expandKeys.splice(index, 1)
          }
          this.removeExpand(this.nodeObject[node.data.key].children)
        }
      } else {
        this.currentSelect = Number(node.data.key)
      }
      this.$nextTick(() => {
        this.setClass()
      })
    })

    this.root = this.store.root

    let dragState = this.dragState
    this.$on('tree-node-drag-start', (event, treeNode) => {
      if (
        typeof this.allowDrag === 'function' &&
        !this.allowDrag(treeNode.node)
      ) {
        event.preventDefault()
        return false
      }
      event.dataTransfer.effectAllowed = 'move'

      // wrap in try catch to address IE's error when first param is 'text/plain'
      try {
        // setData is required for draggable to work in FireFox
        // the content has to be '' so dragging a node out of the tree won't open a new tab in FireFox
        event.dataTransfer.setData('text/plain', '')
      } catch (e) { }
      dragState.draggingNode = treeNode
      this.$emit('node-drag-start', treeNode.node, event)
    })

    this.$on('tree-node-drag-over', (event, treeNode) => {
      const dropNode = findNearestComponent(event.target, 'ElTreeNode')
      const oldDropNode = dragState.dropNode
      if (oldDropNode && oldDropNode !== dropNode) {
        removeClass(oldDropNode.$el, 'is-drop-inner')
      }
      const draggingNode = dragState.draggingNode
      if (!draggingNode || !dropNode) return

      let dropPrev = true
      let dropInner = true
      let dropNext = true
      let userAllowDropInner = true
      if (typeof this.allowDrop === 'function') {
        dropPrev = this.allowDrop(draggingNode.node, dropNode.node, 'prev')
        userAllowDropInner = dropInner = this.allowDrop(
          draggingNode.node,
          dropNode.node,
          'inner',
        )
        dropNext = this.allowDrop(draggingNode.node, dropNode.node, 'next')
      }
      event.dataTransfer.dropEffect = dropInner ? 'move' : 'none'
      if ((dropPrev || dropInner || dropNext) && oldDropNode !== dropNode) {
        if (oldDropNode) {
          this.$emit(
            'node-drag-leave',
            draggingNode.node,
            oldDropNode.node,
            event,
          )
        }
        this.$emit('node-drag-enter', draggingNode.node, dropNode.node, event)
      }

      if (dropPrev || dropInner || dropNext) {
        dragState.dropNode = dropNode
      }

      if (dropNode.node.nextSibling === draggingNode.node) {
        dropNext = false
      }
      if (dropNode.node.previousSibling === draggingNode.node) {
        dropPrev = false
      }
      if (dropNode.node.contains(draggingNode.node, false)) {
        dropInner = false
      }
      if (
        draggingNode.node === dropNode.node ||
        draggingNode.node.contains(dropNode.node)
      ) {
        dropPrev = false
        dropInner = false
        dropNext = false
      }

      const targetPosition = dropNode.$el.getBoundingClientRect()
      const treePosition = this.$el.getBoundingClientRect()

      let dropType
      const prevPercent = dropPrev
        ? dropInner
          ? 0.25
          : dropNext
            ? 0.45
            : 1
        : -1
      const nextPercent = dropNext
        ? dropInner
          ? 0.75
          : dropPrev
            ? 0.55
            : 0
        : 1

      let indicatorTop = -9999
      const distance = event.clientY - targetPosition.top
      if (distance < targetPosition.height * prevPercent) {
        dropType = 'before'
      } else if (distance > targetPosition.height * nextPercent) {
        dropType = 'after'
      } else if (dropInner) {
        dropType = 'inner'
      } else {
        dropType = 'none'
      }

      const iconPosition = dropNode.$el
        .querySelector('.el-tree-node__expand-icon')
        .getBoundingClientRect()
      const dropIndicator = this.$refs.dropIndicator
      if (dropType === 'before') {
        indicatorTop = iconPosition.top - treePosition.top
      } else if (dropType === 'after') {
        indicatorTop = iconPosition.bottom - treePosition.top
      }
      dropIndicator.style.top = indicatorTop + 'px'
      dropIndicator.style.left = iconPosition.right - treePosition.left + 'px'

      if (dropType === 'inner') {
        addClass(dropNode.$el, 'is-drop-inner')
      } else {
        removeClass(dropNode.$el, 'is-drop-inner')
      }

      dragState.showDropIndicator =
        dropType === 'before' || dropType === 'after'
      dragState.allowDrop = dragState.showDropIndicator || userAllowDropInner
      dragState.dropType = dropType
      this.$emit('node-drag-over', draggingNode.node, dropNode.node, event)
    })

    this.$on('tree-node-drag-end', (event) => {
      const { draggingNode, dropType, dropNode } = dragState
      event.preventDefault()
      event.dataTransfer.dropEffect = 'move'

      if (draggingNode && dropNode) {
        const draggingNodeCopy = { data: draggingNode.node.data }
        if (dropType !== 'none') {
          draggingNode.node.remove()
        }
        if (dropType === 'before') {
          dropNode.node.parent.insertBefore(draggingNodeCopy, dropNode.node)
        } else if (dropType === 'after') {
          dropNode.node.parent.insertAfter(draggingNodeCopy, dropNode.node)
        } else if (dropType === 'inner') {
          dropNode.node.insertChild(draggingNodeCopy)
        }
        if (dropType !== 'none') {
          this.store.registerNode(draggingNodeCopy)
        }

        removeClass(dropNode.$el, 'is-drop-inner')

        this.$emit(
          'node-drag-end',
          draggingNode.node,
          dropNode.node,
          dropType,
          event,
        )
        if (dropType !== 'none') {
          this.$emit(
            'node-drop',
            draggingNode.node,
            dropNode.node,
            dropType,
            event,
          )
        }
      }
      if (draggingNode && !dropNode) {
        this.$emit('node-drag-end', draggingNode.node, null, dropType, event)
      }

      dragState.showDropIndicator = false
      dragState.draggingNode = null
      dragState.dropNode = null
      dragState.allowDrop = true
    })
  },

  mounted() {
    // 记录展开的父级key
    if (
      this.data.length > 0 &&
      !this.expandKeys.includes(this.data[0][this.nodeKey])
    ) {
      this.expandKeys.push(this.data[0][this.nodeKey])
    }

    // 计算树形结构占据多大高度，预计展示多少节点
    this.dialogBodyOuter = document.getElementById(this.id)
    let computedHeight = this.dialogBodyOuter.offsetHeight
    if (computedHeight > 0) {
      // 当前高度--除--一行占据36px--加10条数据
      this.showNumber = parseInt(Number(computedHeight) / 36) + 20
    }else{
      this.showNumber=40
    }

    // 设置当前选中行
    if (this.currentNodeKey) {
      this.setCurrentKey(this.currentNodeKey)
    }

    this.formateEquips(this.data)
    this.initTabIndex()
    this.$el.addEventListener('keydown', this.handleKeydown)
  },

  updated() {
    this.treeItems = this.$el.querySelectorAll('[role=treeitem]')
    this.checkboxItems = this.$el.querySelectorAll('input[type=checkbox]')
  },
}
</script>

<style lang="scss" src="./tree.scss">
</style>
