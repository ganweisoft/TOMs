export default {
    props: {
        showDialog: { type: Boolean, default: false },
        closeDialog: {
            type: Function,
            default: () => { }
        },
        info: {
            type: Object,
            default: () => { }
        },
        callback: {
            type: Function,
            default: () => { }
        }
    },
    watch: {
        showDialog: function (val) {
            this.showDeleteGroup = val
        }
    },
    data () {
        return {
            showDeleteGroup: false,
            loading: false
        }
    },
    methods: {

        // 确认删除分组
        confirmDeleteGroup () {
            this.loading = true;
            this.$api
                .deleteGroup({
                    groupId: this.info.groupId,
                    // deleteEquip: this.info.deleteChild,   // deleteEquip(Boolean):是否删除分组下面的设备
                    deleteEquip: true,   // deleteEquip(Boolean):是否删除分组下面的设备
                })
                .then((res) => {
                    const { code, message } = res?.data || {}
                    if (code === 200) {
                        this.$message.success(this.$t('equipInfo.publics.tips.deleteSuccess'));
                        this.closeDialog('showDeleteGroup')
                        this.callback(true)
                    } else {
                        this.$message.error(message);
                    }

                }).catch((err) => {
                    this.$message.error(err?.data, err);
                    console.log(err)
                }).finally(() => {
                    this.loading = false;
                });
        }
    }
}