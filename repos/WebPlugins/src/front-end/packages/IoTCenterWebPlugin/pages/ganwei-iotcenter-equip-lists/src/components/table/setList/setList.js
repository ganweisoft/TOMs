import { watch } from "vue"

export default {
    props: {
        tableDataSet: {
            type: Array,
            default: () => []
        },
        equipConState: {
            type: Number,
            default: 0
        },
        loading: {
            type: Boolean,
            default: false
        },
    },
    watch: {
        tableDataSet: {
            handler: function (val, oldVal) {
                this.tableData = val
            },
            deep: true
        }
    },
    data () {
        return {
            tableData: []
        }
    },
    methods: {
        // 设备发送控制操作命令
        onSetOperate (type, item) {
            if (!this.equipConState && !item?.mainInstruction?.toUpperCase() == 'ENABLEEQUIP') {
                this.$message.warning(this.$t('equipListsIot.tips.deviceNoConnect'));
                return;
            }
            let data = {
                equipNo: item.equipNo,
                setNo: item.setNo,
                setType: item.setType,
                value: item.value,
                userName: sessionStorage.getItem('userName')
            }
            if (item.setType !== 'V') {
                this.$msgbox({
                    title: this.$t('equipListsIot.tips.titleTip'),
                    message: item.setNm + this.$t('equipListsIot.tips.execute'),
                    showCancelButton: true,
                    confirmButtonText: this.$t('equipListsIot.button.confirm'),
                    cancelButtonText: this.$t('equipListsIot.button.cancel'),
                    beforeClose: (action, instance, done) => {
                        if (action === 'confirm') {
                            instance.confirmButtonLoading = true;
                            setTimeout(() => {
                                this.setCommandBySetNo(data, () => {
                                    instance.confirmButtonLoading = false;
                                    done()
                                })
                            }, 120);
                        } else {
                            done()
                        }
                    }
                }).then(() => { }).catch(() => { })
            } else {
                this.$prompt(this.$t('equipListsIot.label.inputValue'), item.setNm, {
                    confirmButtonText: this.$t('equipListsIot.button.confirm'),
                    cancelButtonText: this.$t('equipListsIot.button.cancel'),
                    beforeClose: (action, instance, done) => {
                        if (action === 'confirm') {
                            instance.confirmButtonLoading = true;
                            setTimeout(() => {
                                data.value = instance.inputValue;
                                this.setCommandBySetNo(data, () => {
                                    instance.confirmButtonLoading = false;
                                    done()
                                })
                            }, 120);
                        } else {
                            done()
                        }
                    }
                }).then(({ value }) => { }).catch(() => { })
            }
        },
        setCommandBySetNo (data, cb) {
            this.$api
                .getSetCommandBySetNo(data)
                .then((res) => {
                    const { code, message } = res?.data || {}
                    if (code === 200) {
                        this.$message({
                            title: this.$t('equipListsIot.tips.success'),
                            type: 'success'
                        })
                    } else {
                        this.$message.error(message)
                    }
                    if (cb) {
                        cb()
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err)
                    if (cb) {
                        cb()
                    }
                })
        },
    }
}
