/** @file 日志预览 **/
export default {

    data () {
        return {
            pagination: {
                pageSize: 20,
                pageNo: 1,
                total: 0
            },
            pageLoading: false,
            logOptions: [
                {
                    value: 0,
                    label: 'logPreview.logType.serverLog'
                },
                {
                    value: 1,
                    label: 'logPreview.logType.webLog'
                }
            ],

            logType: 0,
            logDataList: [],
            downLoading: false,
            dialogVisible: false,
            defaultProps: {
                children: 'childs',
                label: 'name'
            },
            dialogTitle: '',
            previewLoading: false,
            flatterObj: {},
            down: 'publics.button.download',
            tableHeight: null
        };
    },
    mounted () {
        let tableBox = document.getElementsByClassName('logPreviewMain')[0];
        if (tableBox) {
            this.tableHeight = tableBox.offsetHeight - 30;
        }
        window.onresize = () => {
            tableBox = document.getElementsByClassName('logPreviewMain')[0];
            if (tableBox) {
                this.tableHeight = tableBox.offsetHeight - 30;
            }
        };
        this.getLog();

    },
    methods: {
        handleCurrentChange (val) {
            this.pagination.pageNo = val;
            this.getLog()
        },
        handleSizeChange (val) {
            this.pagination.pageSize = val;
            this.getLog()
        },

        previewLog (node) {
            this.dialogTitle = node.name
            this.dialogVisible = true
            this.previewLoading = true;
            let that = this;
            this.$api
                .downLoadLog({
                    logType: this.logType,
                    relativePath: node.fullPath
                })
                .then(res => {
                    if (res.data) {
                        try {
                            const reader = new FileReader();
                            reader.onload = (ev) => {
                                const content = ev.target.result
                                that.$refs.logContent.innerHTML = content;
                            }
                            reader.readAsText(res.data);
                        } catch (error) {
                            console.log(error)
                        }

                    }
                    this.previewLoading = false;
                })
                .catch(err => {
                    this.$message.error(err.data, err);
                    this.previewLoading = false;
                    console.log(err);
                });

        },
        generateUUID () {
            let d = new Date().getTime();
            if (window.performance && typeof window.performance.now === 'function') {
                d += performance.now(); // use high-precision timer if available
            }
            let uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                let r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c === 'x' ? r : (r & 0x3) | 0x8).toString(16);
            });
            return uuid;
        },
        addLoadings (data) {
            return data.map(res => {
                this.$set(res, 'loading', false)
                this.$set(res, 'key', this.generateUUID())
                if (res.childs) {
                    res.childs = this.addLoadings(res.childs)
                }
                return res
            })
        },
        getLog () {
            let data = {
                logType: this.logType,
                pageSize: this.pagination.pageSize,
                pageNo: this.pagination.pageNo
            };
            this.pageLoading = true

            this.$api.getLog(data)
                .then(res => {
                    const { rows, totalCount } = res?.data || {}
                    if (rows) {
                        this.logDataList = this.addLoadings(rows)
                        this.pagination.total = totalCount
                    } else {
                        this.logDataList = []
                        this.pagination.total = 0
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err);

                }).finally(() => {
                    this.pageLoading = false;
                });
        },
        getFile (node) {
            node.loading = true
            setTimeout(() => { node.loading = false }, 1000)
            let link = document.createElement('a')
            link.style.display = 'none'
            link.target = 'blank'
            link.href = `/IoT/api/v3/LogPreview/DownLoadLog?logType=${this.logType}&relativePath=${node.fullPath}`
            document.body.appendChild(link)
            link.setAttribute('download', node.name)
            link.click();
        },
        selectChange (val, app) {
            this.pagination.pageNo = 1
            this.getLog()
        }

    }
};
