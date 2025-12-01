<template>
    <div class="gw-tree" :style="{ height: '100%' }">
        <div class="tree" :style="{ height: '100%' }">
            <virtual-list ref="virtualList" class="virtualList" :data-key="'key'" :data-sources="visibleList"
                :data-component="itemComponent" :keeps="40" :extra-props="{
                    currentSelect,
                    nodeClick,
                    showStatus,
                    showSettings,
                    showCheckbox,
                    onChecked,
                    showOperate,
                    groupEditAndNew,
                    deleteGroup,
                    colorConfig
                }" />
        </div>
    </div>
</template>
<script>
import ElVirtualNode from '../treeNode/treeNode.vue'
import VirtualList from 'vue3-virtual-scroll-list'
import utils from '../../utils/utils.js'
import { mixinWindow } from '../../utils/windowManager'

import equipStatusManage from '../../mixin/equipStatusManage'
import cacheManage from '../../mixin/cacheManage'
import searchManage from '../../mixin/searchManage'
import equipNumManage from '../../mixin/equipNumManage'
import requestManage from '../../mixin/requestManage'
import checkStatusManage from '../../mixin/checkStatusManage'

export default {
    components: {
        VirtualList
    },
    mixins: [mixinWindow],
    data () {
        return {
            itemComponent: ElVirtualNode,  //节点组件

            visibleList: [],               //虚拟滚动所需要的列表

            currentSelect: -1,             //当前选中的节点（不包含分组节点）

            equipStatusManage: null,       //设备状态管理工具
            cacheManage: null,             //内存管理工具
            searchManage: null,            //搜索管理工具
            equipNumManage: null,          //设备数量管理工具
            requestManage: null,           //请求管理工具
            checkStatusManage: null,       //请求管理工具

            groupNodeObject: {},           //所有分组扁平化所保存的存储的地方

            nodesMap: {},                  //所有实例化对象映射

            equipCheckObject: {},           //设备选中状态记录 equipCheckObject：{xxx设备号：{indeterminate: false,checked: false,groupId:xxx}}
            equipStatusObject: {},           //记录设备状态,从后台推送的初始化全量状态,增量更新需要实时维护

            controlObject: {},              //展开的控制项,controlObject:{xxx设备号：{groupId:xxx}}用于设置当前展开设置选中，如果依赖与equipControlObject筛选设置选中，性能差

            isSearchStatus: false,         //是否是搜索状态

            equipControllObject: {},        //绑定所有的已选中的设备控制项

            expandGroup: [],               //记录已展开的分组
            updateJob: null

        }
    },
    props: {
        data: {  //分组列表
            type: Array,
        },
        selectEquips: {  //设备选中列表，用于回显，如权限管理
            type: Array,
            default: () => []
        },
        controllList: {  //控制项选中列表，用于回显，如权限管理
            type: Array,
            default: () => []
        },
        showSettings: {  //是否需要加载设备控制项，如权限管理
            type: Boolean,
            default: false
        },
        showSelectNum: {  //是否在树形结构顶部展示已选设备数量，如设备管理
            type: Boolean,
            default () {
                return false
            },
        },

        showCheckbox: { //是否展示checkbox
            type: Boolean,
            default: false,
        },

        showStatus: {   //是否展示设备状态
            type: Boolean,
            default: false,
        },

        showOperate: {  // 是否展示操作按钮
            type: Boolean,
            default: false,
        },

        defaultExpandAll: {
            type: Boolean,
            default: false,
        },
        currentNodeKey: [String, Number],
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
        treeType: {
            type: String,
            default: ''
        },
        buildTree: {
            type: Function,
            default: () => { }
        },
        filterData: {
            type: Function,
            default: () => { }
        },
        alias: {
            type: String,
            default: ''
        }
    },

    watch: {
        'data' (val) {
            if (val && val.length) {
                // 将对象扁平化到this.groupNodeObject
                utils.flattern(this.data, this.groupNodeObject)
                // 增加分组映射
                this.cacheManage.addNodesMap(Object.values(this.groupNodeObject).map(item => item))

                this.init()
            }
        },
        'controllList' (val) {
            this.updateCheckedStatusWithControls()
        },
        'selectEquips' (val) {
            this.updateCheckedStatusWithEquips()
        }
    },
    computed: {
        aliasName () {
            return this.alias ? `-${this.alias}` : ''
        }
    },

    created () {
        // 实例化复选框管理工具
        if (this.showCheckbox) {
            this.checkStatusManage = new checkStatusManage(this.groupNodeObject, this.nodesMap, this.equipControllObject, this.controlObject, this.equipCheckObject, this.aliasName)
        }

        // 实例化缓存管理工具
        this.cacheManage = new cacheManage(this.groupNodeObject, this.nodesMap, this.controlObject, this.equipCheckObject)

        // 实例化搜索管理工具
        this.searchManage = new searchManage(this.groupNodeObject, this.showSettings, this.aliasName)

        // 实例化分组所挂载的设备数量管理工具
        this.equipNumManage = new equipNumManage(this.groupNodeObject, this.aliasName)

        // 实例化数据请求管理工具
        this.requestManage = new requestManage(this.nodesMap, this.equipControllObject)
    },

    mounted () {
        this.updateCheckedStatusWithControls()
        this.updateCheckedStatusWithEquips()
        window.addEventListener('message', (e) => {
            if (e.data.type) {
                this[e.data.type] && this[e.data.type](e.data.data)
            }
        })
    },

    methods: {
        // 初始化
        init () {
            this.data[0].expand = true
            this.updateTreeList()
            setTimeout(() => {
                Object.values(this.groupNodeObject).forEach(group => {
                    if (!group.expand && this.expandGroup.includes(group.key)) {
                        group.expand = true
                    }
                    this.filterWithAlias(group.key)
                    this.updateGroupEquips(group.key, true)
                })
                this.equipNumManage.resetGroupNum(this.isSearchStatus)
                // 实例化设备状态管理工具
                if (this.showStatus && !this.equipStatusManage) {
                    this.equipStatusManage = new equipStatusManage(this.nodesMap, this.equipStatusObject, this.groupNodeObject, this.statusChange, this.aliasName)
                }
            }, 100)
        },

        // 根据条件过滤
        filterWithAlias (groupKey) {
            if (this.aliasName) {
                let arr = this.window[`group-${groupKey}`] || []
                this.window[`group-${groupKey}${this.aliasName}`] = this.filterData(arr)
            }
        },

        // 更新当前选中
        updateCurrentSelect () {
            if (this.window.equipCache && this.window.equipCache[this.currentNodeKey]) {
                this.currentSelect = `${this.window.equipCache[this.currentNodeKey].groupId}-${this.currentNodeKey}`
            }
        },

        // 设备状态变化
        statusChange (groupId, equipNo, status) {
            if (this.currentSelect == `${groupId}-${equipNo}`) {
                this.$emit('statusChange', equipNo, status)
            }
        },

        // 更新checkbox选中状态
        updateCheckWidthJob () {
            if (this.showCheckbox) {
                this.checkStatusManage.resetCheckedStatus()
                this.updateCheckedStatusWithControls()
                this.updateCheckedStatusWithEquips()
            }
        },


        // 通过外框更新设备
        updateGroupEquips (key, isInit) {
            if (!this.isSearchStatus && this.groupNodeObject[key]) {
                let total = this.equipNumManage.getAllEquipsNum()
                this.$emit('getTotal', total)
            }
            if (isInit) {
                let arr = this.window[`group-${key}${this.aliasName}`] || []
                this.equipNumManage.setGroupNum(key, arr.length)
            }
            if (this.groupNodeObject[key].expand) {
                this.updateList(key, this.groupNodeObject[key].level, null, false)
            }
        },


        // 展开的时候从缓存中拿设备数据到equips中并更新视图
        updateList (key) {

            // 当前为分组
            if (this.groupNodeObject[key]) {
                let arr = []
                if (this.isSearchStatus) {
                    arr = this.window[`group-${key}-search`]
                    this.groupNodeObject[key].equips = [...utils.deepClone(arr, Number(this.groupNodeObject[key].level) + 1, this.showSettings, key, this.equipCheckObject, this.equipStatusObject)]
                } else {
                    arr = this.window[`group-${key}${this.aliasName}`]
                    this.groupNodeObject[key].equips = [...utils.deepClone(arr, Number(this.groupNodeObject[key].level) + 1, this.showSettings, key, this.equipCheckObject, this.equipStatusObject)]
                }

                // 增加设备节点映射
                this.cacheManage.addNodesMap(this.groupNodeObject[key].equips)

                // 关闭兄弟节点
                this.cacheManage.closeBrotherNode(key)
                this.visibleList = []
                this.updateTreeList(this.data)

                if (this.showStatus && this.equipStatusManage) {
                    this.equipStatusManage.updateGroupStatus(key)
                }
                this.updateCurrentSelect()
            }

        },

        // 更新整个树形结构
        updateTreeList (data) {
            if (data) {
                data.forEach(item => {
                    this.visibleList.push(item)
                    if (item.expand) {
                        if (item.isEquip) {
                            if (!item.children) {
                                item.children = []
                            }
                            if (item.settings) {
                                item.children = [...(item.settings || [])]
                            }

                        } else {
                            item.children = [...(item.equips || []), ...(item.groups || [])]
                        }

                    } else {
                        item.children = []
                    }
                    this.updateTreeList(item.children || [])
                })
            }
        },

        // 节点点击事件
        nodeClick (node, nodeIndex, level, checked) {
            this.$emit('node-click', { ...node, key: node.isEquip ? node.equipNo : node.key })
            if (node.isGroup) {
                if (!node.isEquip) {
                    this.groupClick(node, nodeIndex, checked)
                } else {
                    this.equipClick(node, nodeIndex, checked)
                }
            } else {
                this.currentSelect = node.key;
            }
        },

        groupClick (node, nodeIndex, checked) {
            if (node.expand) {
                this.updateList(node.key, node.level, nodeIndex, checked)
            } else {
                this.cacheManage.recycleGroupCache(node.key)
                this.visibleList = []
                this.updateTreeList(this.data)
            }
        },

        async equipClick (node, nodeIndex, checked) {
            if (this.showSettings) {
                if (node.expand) {
                    node.loading = true
                    await this.requestManage.getSetting(node.key, node.title, node.level, checked)
                    this.controlObject[node.equipNo] = { groupId: node.groupId }
                    node.loading = false
                    this.visibleList.splice(nodeIndex + 1, 0, ...this.nodesMap[node.key].settings)
                    this.cacheManage.addNodesMap(this.nodesMap[node.key].settings)
                } else {
                    this.visibleList.splice(nodeIndex + 1, this.nodesMap[node.key].settings.length)
                    this.cacheManage.removeNodesMap(this.nodesMap[node.key].settings)
                    this.nodesMap[node.key].settings = []
                    delete this.controlObject[node.equipNo]
                }
            }
        },

        onChecked (node) {
            this.checkStatusManage && this.checkStatusManage.onChecked(node, this.isSearchStatus)
            this.$emit('onCheck')
        },

        groupEditAndNew (isGroupNew, node) {
            this.$emit('groupEditAndNew', { isGroupNew, node })
        },

        deleteGroup (node) {
            this.$emit('deleteGroup', node)
        },

        updateCheckedStatusWithControls () {
            if (this.controllList && this.controllList.length && this.checkStatusManage) {
                this.checkStatusManage && this.checkStatusManage.updateCheckedStatusWithControls(this.controllList)
            }
        },
        updateCheckedStatusWithEquips () {
            if (this.selectEquips && this.selectEquips.length && this.checkStatusManage) {
                this.checkStatusManage && this.checkStatusManage.updateCheckedStatusWithEquips(this.selectEquips)
            }
        },

        resetCheckedStatus () {
            this.checkStatusManage && this.checkStatusManage.resetCheckedStatus()
        },

        /**
         * @description 获取选择的设备
         *  @return Array
         */
        getEquipSelectd () {
            return this.checkStatusManage && this.checkStatusManage.getEquipSelectd()
        },

        /**
         * @description 获取选择的设备控制项
         *  @return Array
         */
        getControlSelected () {
            return this.checkStatusManage && this.checkStatusManage.getControlSelected()
        },

        /**
         * @description 记录已展开的分组
         *  @return 无
         */
        recordExpandGroup () {
            this.expandGroup = []
            Object.values(this.groupNodeObject).forEach(group => {
                if (group.expand) {
                    this.expandGroup.push(group.key)
                }
            })
        },

        /**
         * @description 清除已展开的分组
         *  @return
         */
        removeExpandGroup () {
            this.$nextTick(() => {
                this.expandGroup = []
            })
        },

        /**
         * @description 搜索设备
         *    1：清除所有节点缓存
              2: 设置搜索状态为true,搜索状态下所有操作全部不更新
              3: 非搜索状态，需要清除搜索缓存
         * @param {searchName}  搜索名称
         *  @return 无返回值
         */
        filterMethod (searchName) {
            this.visibleList = []
            this.cacheManage.recycleAllNodeCache()
            if (searchName) {
                this.isSearchStatus = true
                this.searchManage.filterMethod(searchName)
            } else {
                this.isSearchStatus = false
            }
            this.init()
            this.$nextTick(() => {
                this.$refs.virtualList.scrollToIndex(0)
            })

            this.equipNumManage.resetGroupNum(this.isSearchStatus)

            if (this.showCheckbox) {
                this.checkStatusManage && this.checkStatusManage.reComputedCheckNum(this.isSearchStatus)
                this.checkStatusManage && this.checkStatusManage.updateGroupCheckStatus()
            }
        },

        // 重新构建树
        rebuildTree () {
            this.recordExpandGroup()
            this.checkStatusManage && this.checkStatusManage.resetCheckedStatus()
            this.cacheManage.recycleAllNodeCache(true)
            this.$nextTick(() => {
                this.buildTree()
                this.removeExpandGroup()
            })
        },

        // 树形结构销毁
        destroyTree () {
            this.cacheManage.recycleAllNodeCache(true)
            this.visibleList = []
            this.updateTreeList(this.data)
        },

        // 通过外框事件更新树形结构数据--start

        /**
         * @description 获取分组设备
         * @param {data}  {分组Id}
         *  @return 无返回值
         */
        GetGroupEquips (data) {
            if (data.groupId && this.groupNodeObject[data.groupId] && !this.isSearchStatus) {
                this.updateGroupEquips(data.groupId, true)
                if (this.updateJob) {
                    clearTimeout(this.updateJob)
                }
                this.updateJob = setTimeout(() => { this.updateCheckWidthJob() }, 500)
            }
        },

        /**
          * @description 删除设备
          * @param {data}  {groupId,equips}
          *  @return 无返回值
        */
        DeleteEquip (data) {
            const { groupId, equips } = data || {}
            if (groupId && this.groupNodeObject[groupId]) {
                this.updateGroupEquips(data.groupId)
                this.equipNumManage.resetGroupNum()
                this.checkStatusManage && this.checkStatusManage.resetCheckedStatus()
            }
            if (equips.length) {
                equips.forEach(item => {
                    let node = this.nodesMap[`${groupId}-${item.id}`]
                    if (node) {
                        if (this.currentSelect.toString().includes(item.id)) {
                            this.$emit('currentDelete')
                        }
                        if (this.showStatus) {
                            if (node.status == 2 || node.status == 0) {
                                this.equipStatusManage.setGroupStatus(groupId, false, 2)
                            } else if (node.status == 6) {
                                this.equipStatusManage.setGroupStatus(groupId, false, 6)
                            }
                        }
                        delete this.nodesMap[`${groupId}-${item.id}`]
                        delete this.equipCheckObject[item.id]
                    }
                    delete this.equipStatusObject[item.id]
                })
            }
        },

        /**
          * @description 移动设备
          * @param {data} {updateGroups} 需要更新的分组列表
          *  @return 无返回值
        */
        moveEquips (data) {
            const { updateGroups, buildTree } = data || {}
            if (!buildTree && updateGroups) {
                updateGroups.forEach(groupId => {
                    if (groupId && this.groupNodeObject[groupId]) {
                        this.updateGroupEquips(groupId)
                        this.equipNumManage.resetGroupNum()
                        this.checkStatusManage && this.checkStatusManage.resetCheckedStatus()
                    }
                })
            } else if (buildTree) {
                this.rebuildTree()
            }
        },

        /**
           * @description 删除分组
           * @param {data}  {groupId,parentGroupId}
           * @return 无返回值
        */
        DeleteEquipGroup (data) {
            data.forEach(group => {
                if (group.groupId && this.nodesMap[group.groupId]) {
                    if (!this.isSearchStatus) {
                        this.rebuildTree()
                    }
                }
            })
        },

        /**
           * @description 新增分组
           * @param {data}  {groupId,parentGroupId}
           * @return 无返回值
        */
        AddEquipGroup (data) {
            const { parentGroupId, groupId } = data || {}
            if (groupId && this.nodesMap[parentGroupId] && this.treeType) {
                if (!this.isSearchStatus) {
                    this.rebuildTree()
                }
            }
        },

        /** 编辑分组
           * @description 新增分组
           * @param {data}  {groupId,parentGroupId}
           * @return 无返回值
        */
        EditEquipGroup (data) {
            const { groupId, groupName } = data
            if (this.nodesMap[groupId]) {
                this.nodesMap[groupId].title = groupName
            }
        },

        /**
         * @description 新增分组设备
         * @param {data}  {分组Id,设备}
         *  @return 无返回值
         */
        AddEquip (data) {
            const { groupId } = data || {}
            if (groupId) {
                if (!this.isSearchStatus) {
                    this.rebuildTree()
                }
            }
        },
        /**
            * @description 新增分组设备
            * @param {data}  {分组Id,设备}
            * @return 无返回值
        */
        EditEquip (data) {

            const { equipNo, groupId, equipName } = data || {}
            if (this.nodesMap[`${groupId}-${equipNo}`]) {
                if (!this.isSearchStatus) {
                    this.nodesMap[`${groupId}-${equipNo}`].title = equipName
                }
            }
        },
        // 通过外框事件更新树形结构数据--end
    }
}
</script>
