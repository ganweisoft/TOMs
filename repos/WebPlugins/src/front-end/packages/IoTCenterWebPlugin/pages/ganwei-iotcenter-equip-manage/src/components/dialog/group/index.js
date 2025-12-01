export default {
    props: {
        showDialog: {
            type: Boolean,
            default: true
        },
        closeDialog: {
            type: Function,
            default: () => { }
        },
        info: {
            type: Object,
            default: () => { }
        },
        isGroupNew: {
            type: Boolean,
            default: false
        }
    },
    watch: {
        showDialog: function (val) {
            this.form.editGroupName = ''
            this.showGroupDialog = val
            if (this.isGroupNew) {
                this.title = 'equipInfo.poverTips.newChildGroup'
            } else {
                this.title = 'equipInfo.poverTips.rename'
            }
        }
    },
    data () {
        return {
            showGroupDialog: false,
            title: 'equipInfo.poverTips.newChildGroup',
            saveLoading: false,
            form: {
                editGroupName: ''
            }
        }
    },
    computed: {
        rules () {
            return {
                editGroupName: [{ required: true, message: this.$t('equipInfo.tips.inputGroupName'), trigger: 'blur' }]
            }
        }
    },
    mounted () {
        console.log('已经实例化')
    },
    methods: {
        // 新增分组---保存
        saveGroupData () {
            this.$refs.form.validate(valid => {
                if (valid) {
                    this.saveLoading = true
                    if (this.isGroupNew) {
                        let data = {
                            name: this.form.editGroupName,
                            equipNos: [],
                            parentId: this.info.groupId
                        }
                        this.$api
                            .addGroupEquipList(data)
                            .then(res => {
                                const { code, message } = res?.data || {}
                                if (code === 200) {
                                    this.$message.success(this.$t('equipInfo.publics.tips.addSuccess'))
                                    this.closeDialog('showGroupDialog')
                                } else {
                                    this.$message.warning(message)
                                }
                            })
                            .catch(err => {
                                this.$message.warning(err?.data, err)
                            }).finally(() => {
                                this.saveLoading = false
                            })
                    } else {
                        let data = {
                            groupId: this.info.groupId, // 分组id
                            newName: this.form.editGroupName
                        }
                        this.$api
                            .getEquipReName(data)
                            .then(res => {
                                const { code, message } = res?.data || {}
                                if (code === 200) {
                                    this.$message.success(this.$t('equipInfo.publics.tips.editSuccess'))
                                    this.closeDialog('showGroupDialog')
                                } else {
                                    this.$message.error(message)
                                }
                            })
                            .catch(err => {
                                this.$message.error(err?.data, err)
                            }).finally(() => {
                                this.saveLoading = false
                            })
                    }
                }
            })
        }
    }
}
