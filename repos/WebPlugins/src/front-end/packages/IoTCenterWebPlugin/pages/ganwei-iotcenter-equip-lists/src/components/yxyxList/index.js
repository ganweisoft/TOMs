export default {
    props: {
        currentSelect: {
            type: Object,
            default: () => { }
        },
        closeDialog: {
            type: Function,
            default: () => { }
        },
        showDialog: {
            type: Boolean,
            default: false
        },
        showTimeSelect: {
            type: Boolean,
            default: true
        },
        realTimeData: {
            type: Array,
            default: () => []
        }
    },
    watch: {
        realTimeData (newVal) {
            if (!this.showTimeSelect) {
                this.currentList = newVal
            }
        }
    },
    data () {
        return {
            timeSelect: [],
            pickerOptions: {
                disabledDate: (time) => {
                    return time.getTime() >= this.currentDay();
                }
            },
            startTime: '',
            endTime: '',
            pageNo: 1,
            pageSize: 100,
            total: 0,
            list: [],
            currentList: [],
            loading: false,
            tableHeight: this.showTimeSelect ? 'calc(100% - 44px)' : 'calc(100% + 30px)',
            defaultTime: ['00:00:00', '23:59:59']
        }
    },
    mounted () {
        this.timeSelect.push(
            this.myUtils.getCurrentDate(1, '-') + ' 00:00:00'
        )

        this.timeSelect.push(
            this.myUtils.getCurrentDate(1, '-') + ' 23:59:59'
        );

        if (this.showTimeSelect) {
            this.getData()
        }

    },
    methods: {
        currentDay () {
            let date = new Date()
            date.setHours(23, 59, 59)
            return date.getTime()
        },
        close () {
            this.closeDialog()
        },
        formatTime (time) {
            return this.$moment(new Date(time)).format('YYYY-MM-DD HH:mm:ss')
        },
        timeChange (date) {
            if (date) {
                this.startTime = this.formatTime(date[0])
                this.endTime = this.formatTime(date[1])
            } else {
                this.startTime = this.endTime = null
            }
        },
        handleCurrentChange (val) {
            this.pageNo = val
            this.currentList = this.list.slice((val - 1) * this.pageSize, val * this.pageSize)
        },
        btnGetData () {
            this.getData()
        },
        getData () {
            this.timeChange(this.timeSelect);
            if (!this.startTime || !this.endTime) {
                this.$message.warning(this.$t('equipListsIot.tips.timeHorizon'));
                this.loading = false
                return;
            }
            this.loading = true;
            let url = this.currentSelect.ycyxType == 'C' ? 'getEquipGetCurData' : 'getEquipGetYxpCurData';
            let dataHistory = {
                beginTime: this.startTime,
                endTime: this.endTime,
                equipNo: this.currentSelect.equipNo,
                staNo: this.currentSelect.staNo,
                type: this.currentSelect.ycyxType
            }
            this.currentSelect.ycyxType === 'C' ? dataHistory.ycpNo = this.currentSelect.ycyxNo : dataHistory.yxpNo = this.currentSelect.ycyxNo
            this.$api[url](dataHistory).then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    this.list = [];
                    this.currentList = []
                    this.pageNo = 1
                    let times = data?.times || [];
                    let values = data?.values || [];

                    this.total = values.length;
                    times.forEach((item, index) => {
                        this.list.push({
                            time: item,
                            value: JSON.stringify(values[index]).replace('[', '(').replace(']', ')').replaceAll('\"', "").replaceAll('\\', "").replaceAll('\'', "")
                        })
                    })
                    this.currentList = this.list.slice(0, this.pageSize)
                } else {
                    this.$message.error(message)
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            }).finally(() => {
                this.loading = false
            })
        }
    }
}
