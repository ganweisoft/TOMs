export default {
    props: {
        showDialog: {
            type: Boolean,
            default: true
        },
        deleteItem: {
            type: Object,
            default: () => {}
        },
        saveLoading: {
            type: Boolean,
            default: false
        },
        deleteEquip: {
            type: Function,
            default: () => {}
        },
        closeDialog: {
            type: Function,
            default: () => {}
        }
    },
    watch: {
        showDialog: function (val) {
            this.showDeleteEquipDialog = val
        }
    },
    data() {
        return {
            showDeleteEquipDialog: false
        }
    },
    methods: {
        $tt(string) {
            return this.$t(this.$route.name + '.' + string)
        },
    }
}
