<template>
    <tree :ref="refId" :data="list" v-bind="{ ...$attrs, ...$props }" v-on="$listeners" :buildTree="buildTree" />
</template>
<script>
import tree from './components/virtualList/virtualList.vue'
import utils from './utils/utils.js'
import { mixinWindow } from './utils/windowManager'
import gwEquipCache from '../equipProcessing/gwEquipCache'

export default {
    name: "treeV2",
    components: {
        tree
    },
    mixins: [mixinWindow],
    data () {
        return {
            list: [],
            refId: utils.generateUUID(),
            treeKey: null,
            listKey: null,
            hasBuildTree: false,
        }
    },
    props: {
        treeType: {
            type: String,
            default: ''
        }
    },

    mounted () {
        this.treeKey = `equipGroup${this.treeType}`
        this.listKey = `groupList${this.treeType}`

        if (this.window[this.treeKey]) {
            this.list = utils.formateList(JSON.parse(JSON.stringify(this.window[this.treeKey])))
        } else if (this.window[this.listKey]) {
            this.buildTree()
        }
        if (!window.executeQueue) {
            window.executeQueues = {}
        }
        window.executeQueues[this.refId] = this.destroyTree
        window.addEventListener('message', res => {
            if (res && res.data && res.data.type) {
                this[res.data.type] && this[res.data.type]()
            }
        })

        try {
            if (!this.window.hasIframe) {
                this.selfRequest()
            }
        } catch (error) {
            this.selfRequest()
        }
    },

    methods: {
        selfRequest () {
            let equipCache = new gwEquipCache()
            equipCache.Init()
        },
        GetEquipGroupTreeWidthTreeType () {
            if (this.treeType && this.window[this.listKey]) {
                this.buildTree()
            }
        },

        GetEquipGroupTree () {
            if (!this.treeType && this.window[this.listKey]) {
                this.buildTree()
            }
        },

        buildTree () {
            if (this.window[this.listKey]) {
                let treeData = utils.listToTreeList(JSON.parse(JSON.stringify(this.window[this.listKey])))
                this.list = utils.formateList(JSON.parse(JSON.stringify(treeData)))
                this.window[`equipGroup${this.treeType}`] = treeData
                this.hasBuildTree = true
            }
        },

        filterMethod (searchName) {
            this.$refs[this.refId].filterMethod(searchName)
        },

        resetCheckedStatus () {
            this.$refs[this.refId].resetCheckedStatus()
        },

        getEquipSelectd () {
            return this.$refs[this.refId].getEquipSelectd()
        },

        getControlSelected () {
            return this.$refs[this.refId].getControlSelected()
        },

        destroyTree () {
            this.$refs[this.refId].destroyTree()
        }
    }

}

</script>
