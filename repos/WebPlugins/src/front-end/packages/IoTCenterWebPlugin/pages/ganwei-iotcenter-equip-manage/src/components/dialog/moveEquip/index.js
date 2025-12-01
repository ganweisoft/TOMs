export default {
    props: {
        showDialog: {
            type: Boolean,
            default: false,
        },
        closeDialog: {
            type: Function,
            default: () => { },
        },
        saveData: {
            type: Function,
            default: () => { }
        },
        saveLoading: {
            type: Boolean,
            default: false,
        }
    },
    watch: {
        showDialog: function (val) {
            this.showMoveGroupsDialog = val;
            this.moveGroupId = ''
        }
    },
    inject: ['getMoveGroups'],
    computed: {
        moveGroups () {
            return this.getMoveGroups()
        }
    },
    data () {
        return {
            showMoveGroupsDialog: false,
            moveGroupId: ''
        }
    },
    methods: {
        saveMoveData () {
            if (!this.moveGroupId) {
                this.$message.warning(this.$t('equipInfo.tips.selectGroupName'));
                return
            }
            this.saveData(this.moveGroupId)
        }
    }
}