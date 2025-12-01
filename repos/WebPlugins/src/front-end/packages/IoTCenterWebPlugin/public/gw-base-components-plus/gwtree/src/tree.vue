<template>
    <div class="gw-tree" v-loading="loading">
        <div v-if="showSelectNum" class="selectNum">
            <!-- <el-checkbox :indeterminate="isIndeterminate" v-model="checkAll" @change="handleCheckAllChange"></el-checkbox> -->
            {{$t('publics.tips.selected')}}{{ selectNum }}{{$t('publics.unit.set')}}
        </div>

        <div class="el-tree" v-loading="searchLoading" :id="id" :key="id" :style="{
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
          aleadyLoadAll,
          expanding,
          searchName,
          groupLoading,
          colorConfig
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
    import { getNodeKey, generateUUID } from './model/util'
    import ElTreeNode from './tree-node.vue'
    import ElVirtualNode from './tree-virtual-node.vue'
    import { t } from 'element-ui/src/locale'
    import emitter from 'element-ui/src/mixins/emitter'

    import api from './request/api.js'

    // 点击加载，包括点击加载设备以及点击加载设备控制项
    /** 
     * clickLoadEquip: 点击加载设备
     * loadSetting: 点击加载控制项
     * **/

    import clickApiLoad from './mixin/clickApiLoad'

    /** signlr数据推送设备：
     * updateEquips: 更新设备数据
     * SignalREquipConnect: 设备状态推送signlr连接
     * **/
    import signlrLoadEquip from './mixin/signlrLoadEquip.js'

    // 设备增删、移动、更新设备名，同时更新缓存
    import OparateUpdateToCache from './mixin/OparateUpdateToCache.js'

    // 设备复选框选择：
    /** 
     * refreshCheckStatus: 更新某个设备或分组选中状态(需传key)
     * resetChecked: 清空选中状态
     * getChecked: 获取所选设备
     * setParentHalf: 设置父级分组半选状态
     * 
     * **/
    import setCheckStatus from './mixin/setCheckStatus.js'

    // 设备列表模块中设备状态、分组状态,如报警、离线、正常等状态
    import setStatus from './mixin/setStatus.js'

    // 深拷贝
    import deepClone from './mixin/deepClone.js'

    // 内存回收：destoryTree、recycling
    import recycling from './mixin/recycling.js'

    // 扁平化设备：formateEquips
    import formateEquips from './mixin/formateEquips.js'

    // 从接口获取设备数量: getEquipTotal
    import getApiEquipNum from './mixin/getApiEquipNum.js'

    // 设备搜索
    import search from './mixin/search.js'

    // 延迟加载: timeoutLoad  otherLoad
    import delayLoad from './mixin/delayLoad.js'

    export default {
        name: 'ElTree',

        mixins: [emitter, OparateUpdateToCache, clickApiLoad, signlrLoadEquip, setCheckStatus, setStatus, deepClone, recycling, formateEquips, search, getApiEquipNum, delayLoad],

        components: {
            VirtualList,
            ElTreeNode,
        },

        data () {
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
                // 记录当前点击选中的设备Key，非checkbox选中
                currentSelect: -1,
                isIndeterminate: false,
                checkAll: false,
                hoverClass: '',
                dialogBodyOuter: null,
                dialogBodyInner: null,
                // 界面展示的节点数量，通过高度计算
                showNumber: 0,
                id: '',
                // 记录扁平化的分组节点
                nodeObject: {},
                // 记录扁平化的设备节点，分组有展开则记录，关闭分组则删除记录
                equipObject: {},

                // 记录搜索状态下设备节点
                searchEquipObject: {},
                signalREquip: null,
                signalrConnectionEquip: null,
                // 设备总数，从后台接口拿
                allEquipNum: 0,
                // 是否已经全部加载
                aleadyLoadAll: false,
                update: false,
                // 搜索状态中
                searchLoading: false,
                // 报警列表
                alarmList: [],
                // 双机热备列表
                backupList: [],
                // 扁平化所有设备节点，指向缓存
                flatternEquipObject: {},
                // 记录设备对应控制项，controllObject.xxx设备Key = [控制项列表]
                controllObject: {},
                destory: false,

                // 记录所有已展开设备号
                isExpendEquip: {},
                // 是否展开中，若展开中，则其它分组不能点击
                expanding: false,
                // 状态列表：当设备还没从signlr推送完，先保存，推完再设置设备及分组状态
                statusList: [],
                // 记录分组所选中的设备：如：equipSelectsByGroup.xxx分组=[设备数组]
                equipSelectsByGroup: {},
                // 记录设备所选中的控制项
                settingSelectByEquip: [],
                // 所选择的设备量
                selectNum: 0,
                // 记录展开的分组
                expandNodes: {},
                // 记录前分组节点，用于内存回收
                preNode: null,
                // 记录设备所存储的设备号格式（如设备号11135，originEquipMap{11135：分组号 + '-' + 11135}）
                originEquipMap: {},
                // 顶级分组loading
                groupLoading: false,
                // 累加已经获取到的数据量
                statisticsNum: 0
            }
        },

        props: {
            colorConfig: {
                type: Object,
                default: () => {
                    return {
                        "noComm": "#a0a0a0",
                        "normal": "#63e03f",
                        "alarm": "#f22433",
                        "lsSet": "#bebcaa",
                        "initialize": "#289ac0",
                        "withdraw": "#ffc0cb",
                        "BackUp": "#f8901c"
                    }
                }
            },
            selectEquips: {  //设备选中列表，用于回显，如权限管理
                type: Array,
                default: () => []
            },
            controllList: {  //控制项选中列表，用于回显，如权限管理
                type: Array,
                default: () => []
            },
            needsSettings: {  //是否需要加载设备控制项，如权限管理
                type: Boolean,
                default: false
            },
            refresh: {
                type: String,
                default: ''
            },
            treeUpdate: {  //整个树形结构更新完
                type: Boolean,
                default: false
            },

            groupUpdate: {  //一个页面存在多个树形结构时，某个分组更新，通知另外一个树形结构更新，如权限管理页面
                type: Boolean,
                default: false
            },

            clickLoad: {  //是否需要点击加载
                type: Boolean,
                default: true
            },
            searchName: {  //设备搜索名字
                type: String,
                default: ''
            },
            needSignalRLoad: {  //是否需要singlr推送加载
                type: Boolean,
                default: true
            },

            lazyCallback: {
                type: Function,
                default: () => null
            },
            loading: {
                type: Boolean,
                default: false
            },
            showSelectNum: {  //是否在树形结构顶部展示已选设备数量，如设备管理
                type: Boolean,
                default () {
                    return false
                },
            },
            needAdd: {  //是否需要添加功能
                type: Boolean,
                default () {
                    return false
                },
            },
            data: {  //分组列表
                type: Array,
            },
            emptyText: { //暂无数据提示语
                type: String,
                default () {
                    return t('el.tree.emptyText')
                },
            },
            showCheckbox: { //是否展示checkbox
                type: Boolean,
                default: false,
            },
            showCount: { // 是否展示分组数量
                type: Boolean,
                default: false,
            },

            showStatus: {   //是否展示设备状态
                type: Boolean,
                default: false,
            },
            draggable: {
                type: Boolean,
                default: false,
            },
            showOperate: {  // 是否展示操作按钮
                type: Boolean,
                default: false,
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

            allowDrag: Function,
            allowDrop: Function,
            props: {
                default () {
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
            // filterNodeMethod: Function,
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
                set (value) {
                    this.data = value
                },
                get () {
                    return this.data
                },
            },

            treeItemArray () {
                return Array.prototype.slice.call(this.treeItems)
            },

            // 是否无数据
            isEmpty () {
                // const { childNodes } = this.root
                let childNodes = this.root ? this.root.childNodes : []
                return (
                    !childNodes ||
                    childNodes.length === 0 ||
                    childNodes.every(({ visible }) => !visible)
                )
            },

            visibleList () {
                return this.flattenTree(this.root.childNodes)
            },

        },

        beforeDestroy () {
            this.clearAll()
            window.clearTree[this.id] = null
            delete window.clearTree[this.id]
        },
        destroyed () {
            for (let item in this.nodeObject) {
                this.nodeObject[item].children.length = 0
            }

        },

        watch: {

            // 是否已经全部加载完
            treeUpdate () {
                this.aleadyLoadAll = this.allEquipNum <= this.statisticsNum
            },
            defaultCheckedKeys (newVal) {
                this.store.setDefaultCheckedKey(newVal)
            },

            expandKeys (newVal) {
                this.store.defaultExpandedKeys = newVal
                this.store.setDefaultExpandedKeys(newVal)
                this.$forceUpdate()
            },

            // 传进来的树形结构
            data (newVal, oldVal) {
                // 设置顶级分组为默认展开
                if (
                    this.data.length > 0 &&
                    !this.expandKeys.includes(this.data[0][this.nodeKey])
                ) {
                    this.expandKeys.push(this.data[0][this.nodeKey])
                }

                this.store.setData(newVal)
                if (!this.update) {
                    // 扁平化分组
                    this.formateEquips(this.data)
                    // 记录前一个节点数据

                    // 记录顶级分组的展开状态为true
                    this.expandNodes[this.data[0].key] = true
                    // 获取设备总数
                    this.getEquipTotal()
                    this.update = true
                } else {
                    this.aleadyLoadAll = this.allEquipNum <= this.statisticsNum
                }
                this.groupLoading = true
                this.preNode = this.store.getNode(this.data[0].key)
            },

            refresh (newVal, oldVal) {
                if (newVal) {
                    this.formateEquips(this.data);
                    this.initData()
                }
            },

            checkStrictly (newVal) {
                this.store.checkStrictly = newVal
            },
        },

        methods: {

            // 初始化分组的展开状态
            initExpandKeys () {
                for (let item in this.expandNodes) {
                    if (this.nodeObject[item] && this.nodeObject[item].isGroup && !this.nodeObject[item].isEquip) {
                        if (item != this.data[0].key) {
                            this.store.nodesMap[item].expanded = false
                        } else {
                            this.store.nodesMap[item].expanded = true
                        }
                    }
                }
            },

            // 获取选中的设备数量
            setSelectNum () {
                this.selectNum = this.getChecked().equips.length
            },

            // 判断当前是否展开若展开则加载设备节点
            // key(Number):节点key值
            expandAndTimeoutLoad (key) {
                // 如果是存在于展开列表中，则延迟加载设备
                if (this.expandKeys.includes(Number(key))) {
                    this.timeoutLoad(key)
                }
            },

            // 记录当前展开分组、回收上一个同级分组的内存
            setExpand (key) {
                if (this.nodeObject[key] && key != this.data[0].key) {
                    if (this.preNode) {
                        if (this.preNode.data.key != key && this.nodeObject[key].parentKey != this.preNode.data.key) {
                            if (this.preNode.data.key != this.nodeObject[key].parentKey) {
                                // 回收内存
                                this.recycling(this.preNode.data.key)
                                // 回收内存后将分组设置为关闭状态
                                this.preNode.expanded = false
                                // 记录当前分组关闭状态
                                this.expandNodes[this.preNode.data.key] = false
                                // 重新获取当前节点
                                this.preNode = this.store.getCurrentNode()
                                // 记录当前分组展开状态为true
                                this.expandNodes[key] = true
                            } else {
                                // 重新获取当前节点
                                this.preNode = this.store.getCurrentNode()
                                // 记录当前分组展开状态为true
                                this.expandNodes[key] = true
                            }
                        } else {
                            this.preNode = this.store.getCurrentNode()
                        }
                    } else {
                        this.preNode = this.store.getCurrentNode()
                    }
                }
            },


            // 重置选择的数量
            resetSelectNum () {
                // 清空分组及设备的选中状态
                this.setCheckedKeys([], false, [])
                // 将分组所记录的选中设备清空
                for (let item in this.equipSelectsByGroup) {
                    this.equipSelectsByGroup[item].length = []
                }
                // 重新获取所选中的设备数量（此时数量为0）
                this.setSelectNum()
            },


            // 初始化分组设备挂载数量
            initEquipNum () {
                for (let item in this.nodeObject) {
                    // 将所有分组节点所记录的设备数量(count)、分组更新状态(groupUpdate)、是否已经计算数量(computedNum)、设备节点是否扁平化(childFormate)、告警状态(status)、告警数量(alarmCounts)、设备节点全部清空
                    this.nodeObject[item].count = 0
                    this.nodeObject[item].groupUpdate = false
                    this.nodeObject[item].computedNum = false
                    this.nodeObject[item].childFormate = false
                    // this.nodeObject[item].status = 1
                    // this.nodeObject[item].alarmCounts = 0
                    let group = this.nodeObject[item].children.filter(item => item.isGroup)
                    this.nodeObject[item].children.length = group.length

                    this.nodeObject[item].children = this.nodeObject[item].children.filter(item => item.isGroup)
                    this.nodeObject[item].children = [...group]
                }
                for (let item in this.searchEquipObject) {
                    // 将搜索状态下所记录的设备信息清空
                    this.searchEquipObject[item] = null
                }
                for (let item in this.equipObject) {
                    // 将非搜索状态下所记录的设备信息清空
                    this.equipObject[item] = null
                }
            },

            // 从缓存中拿设备数据
            initData () {
                // 初始化分组所挂载的设备信息
                this.initEquipNum()
                // 将已加载的设备数量记为0
                this.statisticsNum = 0
                for (let item in this.nodeObject) {
                    if (this.nodeObject[item].isGroup) {
                        // 从缓存中拿设备数据
                        let sessionKey = this.nodeObject[item].title + '-' + item
                        if (window.top[sessionKey]) {
                            try {
                                let data = window.top[sessionKey]
                                // 将设备数据更新到树形结构
                                this.updateEquips(item, data, true)
                            } catch (error) {
                                console.log(error)
                            }
                        }
                    }
                }
            },

            // 计算分组所挂载的设备数量
            setGroupNum (key, num) {
                if (this.nodeObject[key]) {
                    this.nodeObject[key].count = Number(this.nodeObject[key].count) + Number(num)
                    this.setGroupNum(this.nodeObject[key].parentKey, num)
                }
            },

            // 从缓存中更新设备到树形结构(权限管理调用)
            updateEquipsWithCache (key) {
                let cacheName = this.nodeObject[key].title + '-' + key
                let time = setTimeout(() => {
                    // 若缓存中有该分组设备
                    if (this.nodeObject[key] && window.top[cacheName]) {

                        //判断是否展开，展开则延迟加载 
                        this.expandAndTimeoutLoad(key)
                        // 扁平化设备信息，指向缓存
                        this.flatternEquips(window.top[cacheName])

                        let equipNum = window.top[cacheName].length
                        this.statisticsNum = this.statisticsNum + equipNum
                        this.nodeObject[key].computedNum = true
                        this.setGroupNum(key, equipNum)

                        // 记录是否已经全部加载完
                        this.aleadyLoadAll = this.allEquipNum <= this.statisticsNum
                        // 设置分组所挂载的设备数量
                        this.setGroupNum(key, window.top[cacheName].length)
                        if (this.aleadyLoadAll) {
                            // 如果已经加载完，则通知模块设备加载完毕
                            this.$emit('loadAll', true)
                            this.groupLoading = false
                        }
                    }
                    clearTimeout(time)
                    time = null
                }, 200);
            },

            // 扁平化设备，指向的是缓存
            flatternEquips (arr) {
                arr.forEach(item => {
                    this.flatternEquipObject[item.key] = item
                })

            },

            removeExpand (children) {
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

            // 注销signlr连接
            SignalRStop () {
                if (this.signalREquip) {
                    this.signalREquip.stop()
                    this.signalREquip = null;
                }

                if (this.signalrConnectionEquip) {
                    this.signalrConnectionEquip.stop()
                    this.signalrConnectionEquip = null;
                }

            },


            handleCheckAllChange () {
                if (this.isIndeterminate) {
                    this.checkAll = true
                    this.isIndeterminate = false
                }

                this.root.childNodes.forEach((item) => {
                    this.setChecked(item.data.key, this.checkAll, true)
                })
            },

            setCheckAllFalse () {
                this.checkAll = false
                this.isIndeterminate = false
            },

            flattenTree (datas) {
                return datas.reduce((conn, data) => {
                    conn.push(data)
                    if (data.expanded && data.childNodes.length) {
                        conn.push(...this.flattenTree(data.childNodes))
                    }

                    return conn
                }, [])
            },
            filter (value) {
                if (!this.filterNodeMethod)
                    throw new Error('[Tree] filterNodeMethod is required when filter')
                this.store.filter(value)
            },

            getNodeKey (node) {
                // 获取节点key
                return getNodeKey(this.nodeKey, node.data)
            },

            getNodePath (data) {
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

            getCheckedNodes (leafOnly, includeHalfChecked) {
                return this.store.getCheckedNodes(leafOnly, includeHalfChecked)
            },

            getCheckedKeys (leafOnly) {
                return this.store.getCheckedKeys(leafOnly)
            },

            getCurrentNode () {
                const currentNode = this.store.getCurrentNode()
                return currentNode ? currentNode.data : null
            },

            getCurrentKey () {
                if (!this.nodeKey)
                    throw new Error('[Tree] nodeKey is required in getCurrentKey')
                const currentNode = this.getCurrentNode()
                return currentNode ? currentNode[this.nodeKey] : null
            },

            setCheckedNodes (nodes, leafOnly) {
                if (!this.nodeKey)
                    throw new Error('[Tree] nodeKey is required in setCheckedNodes')
                this.store.setCheckedNodes(nodes, leafOnly)
            },

            setCheckedKeys (keys, leafOnly, halfArray) {
                if (!this.nodeKey)
                    throw new Error('[Tree] nodeKey is required in setCheckedKeys')
                this.store.setCheckedKeys(keys, leafOnly, halfArray)
            },

            setChecked (data, checked, deep) {
                this.store.setChecked(data, checked, deep)
            },

            getHalfCheckedNodes () {
                return this.store.getHalfCheckedNodes()
            },

            getHalfCheckedKeys () {
                return this.store.getHalfCheckedKeys()
            },

            setCurrentNode (node) {
                if (!this.nodeKey)
                    throw new Error('[Tree] nodeKey is required in setCurrentNode')
                this.store.setUserCurrentNode(node)
            },

            setCurrentKey (key) {
                if (!this.nodeKey)
                    throw new Error('[Tree] nodeKey is required in setCurrentKey')
                this.currentSelect = key
                this.store.setCurrentNodeKey(key)
            },

            getNode (data) {
                return this.store.getNode(data)
            },

            remove (data) {
                this.store.remove(data)
            },

            append (data, parentNode) {
                this.store.append(data, parentNode)
            },

            insertBefore (data, refNode) {
                this.store.insertBefore(data, refNode)
            },

            insertAfter (data, refNode) {
                this.store.insertAfter(data, refNode)
            },

            handleNodeExpand (nodeData, node, instance) {
                this.broadcast('ElTreeNode', 'tree-node-expand', node)
                this.$emit('node-expand', nodeData, node, instance)
            },

            updateKeyChildren (key, data) {
                if (!this.nodeKey)
                    throw new Error('[Tree] nodeKey is required in updateKeyChild')
                this.store.updateChildren(key, data)
            },

            updateWithEquips (key, data) {
                if (!this.nodeKey)
                    throw new Error('[Tree] nodeKey is required in updateKeyChild')
                this.store.updateWithEquips(key, data)
            },




        },

        created () {
            this.id = generateUUID()

            this.isTree = true

            this.store = null
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


            this.root = this.store.root

        },

        mounted () {

            if (!window.clearTree) {
                window.clearTree = {}
            }
            window.clearTree[this.id] = () => {
                this.clearAll()
            }

            // window.clearTree =  ()=> {
            //   this.clearAll()
            // }

            // 监听展开事件
            this.$on('node-expand', (item) => {
                if (!this.aleadyLoadAll && !window.top[item.data.title + '-' + item.data.key]) {
                    // 如果signlr还没推送完，且缓存中没有该分组的设备数据，则从后台请求设备数据
                    if (item.data.isGroup && !item.expanded && this.clickLoad) {
                        this.clickLoadEquip(item.data)
                    }
                }


                if (!item.expanded) {
                    if (item.data.isEquip && this.needsSettings) {
                        // 如果点击的是设备且需要加载设备控制项，则请求加载设备控制项
                        this.loadSetting(item.data.key, item.checked)
                    } else {
                        if (!item.data.groupUpdate && item.data.isGroup && window.top[item.data.title + '-' + item.data.key]) {
                            // 如果该分组还没加载 且 缓存中有数据，则懒加载
                            this.timeoutLoad(item.data.key)

                        } else {

                            // 否则，记录为当前分组节点，点其它分组时，关闭当前分组
                            this.setExpand(item.data.key)
                        }
                    }
                }

            })


            // 监听关闭事件
            this.$on('node-collapse', (data, item) => {
                if (item.data.isGroup && !item.data.isEquip) {
                    let timeout = setTimeout(() => {
                        // 回收当前关闭分组的内存
                        this.recycling(item.data.key)
                        clearTimeout(timeout)
                        timeout = null
                    }, 100)
                }
            })


            // 计算树形结构占据多大高度，预计展示多少节点
            this.dialogBodyOuter = document.getElementById(this.id)
            let computedHeight = this.dialogBodyOuter.offsetHeight
            if (computedHeight > 0) {
                // 当前高度/一行占据36px,  额外加10条数据
                this.showNumber = parseInt(Number(computedHeight) / 36) + 20
            } else {
                this.showNumber = 40
            }

            // 设置当前选中行
            //if (this.currentNodeKey) {
            // this.setCurrentKey(this.currentNodeKey)
            //}

        },

        updated () {
            this.treeItems = this.$el.querySelectorAll('[role=treeitem]')
            // this.checkboxItems = this.$el.querySelectorAll('input[type=checkbox]')
        },
    }
</script>

<style lang="scss" src="./tree.scss">
</style>