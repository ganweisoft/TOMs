import selectV2 from 'gw-base-components-plus/selectV2/select.vue'
export default {
    components: {
        selectV2
    },
    props: {
        showDialog: { type: Boolean, default: false },
        groupList: {
            type: Array,
            default: () => []
        }
    },
    data () {
        return {
            saveLoading: false,

            // 模板增加设备表单
            tempEquipForm: {
                equipNo: '',
                num: 1,
                groupId: ''
            },
            templatePagination: {
                pageSize: 20,
                pageNo: 1,
                total: 0
            },
            tempList: [],

        }
    },

    computed: {

        rules () {
            return {
                equipNo: [
                    {
                        required: true,
                        message: this.$t('equipInfo.tips.pleaseSelectTemplate')
                    }
                ],
                groupId: [
                    {
                        required: true,
                        message: this.$t('equipInfo.addFormRules.addToGroup'),
                    }
                ]
            }
        }
    },

    methods: {
        // 确定通过模板新增设备
        saveTempAddEquip () {
            this.$refs.form.validate(valid => {
                if (valid) {
                    this.saveLoading = true
                    this.$api
                        .addEquipFromModel({
                            iEquipNo: this.tempEquipForm.equipNo,
                            equipNum: this.tempEquipForm.num,
                            groupId: this.tempEquipForm.groupId
                        })
                        .then(res => {
                            const { code, data, message } = res?.data || {}
                            if (code == 200) {
                                this.$message.success(this.$t('equipInfo.publics.tips.addSuccess'))
                                this.closeDialog('showModelNewDialog')
                            } else {
                                this.$message.error(message)
                            }
                        })
                        .catch(err => {
                            this.$message.error(err?.data, err)
                            console.log(err)

                        }).finally(() => {
                            this.saveLoading = false
                        })
                }
            })
        },

        remoteSearch (keyword) {
            this.tempList = []
            this.templatePagination.pageNo = 1
            this.templatePagination.total = 0
            this.getModelList(keyword)
        },
        loadmore (keyword) {
            if (this.tempList.length >= this.templatePagination.total) {
                return;
            }
            this.templatePagination.pageNo = this.templatePagination.pageNo + 1;
            this.getModelList(keyword)
        },

        // 获取全部模板
        getModelList (keyword) {
            let data = {
                pageNo: this.templatePagination.pageNo,
                pageSize: this.templatePagination.pageSize,
                equipName: keyword
            }
            this.$api
                .getModelEquipTree(data)
                .then(res => {
                    const { code, data, message } = res?.data || {}
                    if (code == 200) {
                        this.tempList = [...this.tempList, ...(data?.rows || [])];
                        this.templatePagination.total = data?.total || 0
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err);
                });
        },

        closeDialog () {
            this.$refs.form.resetFields()
            this.$emit('closeDialog')
        }
    }
}
