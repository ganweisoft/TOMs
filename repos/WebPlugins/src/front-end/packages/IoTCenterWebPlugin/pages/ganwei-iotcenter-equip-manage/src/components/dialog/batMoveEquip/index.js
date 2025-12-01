export default {
    props: {
        showDialog: {
            type: Boolean,
            default: false
        },
        closeDialog: {
            type: Function,
            default: () => { }
        },
        info: {
            type: Array,
            default: []
        },
        parentSeleceed: {
            type: Array,
            default: []
        }
    },
    watch: {
        showDialog: function (val) {
            this.showSelectGroupDialog = val
            this.form.newGroupId = ''
        }
    },

    data () {
        return {
            form: {
                newGroupId: ''
            },
            showSelectGroupDialog: false,
            btnLoading: false,

        }
    },
    computed: {
        rules () {
            return {
                newGroupId: [
                    {
                        required: true,
                        message: this.$t('equipInfo.tips.selectGroupName')
                    }
                ]
            }
        }
    },

    methods: {
        ConfirmMove () {
            this.$refs.form.validate(valid => {
                console.log(valid)
                if (valid) {
                    this.moveToNewGroup()
                }
            })
        },

        // 移动设备至其他分组
        moveToNewGroup () {
            let data = {
                equipNoList: this.info,
                newGroupId: this.form.newGroupId
            }
            this.btnLoading = true
            this.$api
                .moveToNewGroup(data)
                .then(res => {
                    const { code, message } = res?.data || {}
                    if (code === 200) {
                        this.$message.success(this.$t('equipInfo.tips.moveSuccess'))
                        this.batSetEquipDialog = false
                        this.showSelectGroupDialog = false
                    } else {
                        this.$message.error(message)
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                }).finally(() => {
                    this.btnLoading = false
                })
        },
        resetAndClose () {
            this.$refs.form.clearValidate()
            this.closeDialog('showSelectGroupDialog')
        }
    }
}
