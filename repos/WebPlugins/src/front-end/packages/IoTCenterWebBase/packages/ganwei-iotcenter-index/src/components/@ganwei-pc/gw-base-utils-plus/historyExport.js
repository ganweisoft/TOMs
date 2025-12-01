import * as signalR from '@aspnet/signalr';
export default {
    data () {
        return {
            exportSignalrConnection: null,
            haveDataDwonload: false,
            exportTimer: null,
            exportTimes: 0,
            alreadyExport: true,
            getExportCommand: false
        }
    },
    methods: {
        getCommand (existIframe) {
            if (existIframe) {
                this.getExportCommand = true;
                if (document.getElementById('jumpIframe')) {
                    document.getElementById('jumpIframe').contentWindow.postMessage({ getExportCommand: true }, '*')
                } else {
                    if (window.top.opener) {
                        window.top.opener.postMessage({ getExportCommand: true }, '*')
                    }
                    let arr = Array.from(document.getElementsByTagName('iframe'));
                    arr.forEach(item => {
                        item.contentWindow.postMessage({ getExportCommand: true }, '*')
                    })
                }
            }
        },

        // existIframe:是否有外框
        curveSignalR (e, existIframe) {
            if (e.data.openCurveLink) {
                this.getCommand(existIframe);
                if (!this.haveDataDwonload) {
                    if (this.exportSignalrConnection) {
                        this.exportSignalrConnection.stop()
                    }
                    this.exportSignalrConnection = null;
                    this.exportSignalrConnection = new signalR.HubConnectionBuilder()
                        .withUrl('/downFileNotify', {})
                        .build();
                    this.exportSignalrConnection.start().then(() => { console.log('链接成功'); }).catch(function (ex) {
                        console.log('connectHub 连接失败' + ex);
                    });
                    window.clearInterval(this.exportTimer);
                    this.exportTimer = null;

                    // 判断连接状态
                    this.exportSignalrConnection.off('connectionSucceeded');
                    this.exportSignalrConnection.on('connectionSucceeded', (res) => {
                        if (res) {
                            this.exportCurves(window.top.exportData);
                            this.alreadyExport = false
                            this.haveDataDwonload = true;
                            if (this.exportTimer) {
                                window.clearInterval(this.exportTimer);
                                this.exportTimer = null;
                            }
                            this.exportTimer = setInterval(() => {
                                this.exportTimes = Number(this.exportTimes) + 1;
                                if (this.exportTimes == 5 && this.$route.path.indexOf('equipListsIot') != -1 && !this.alreadyExport) {
                                    this.$message.warning(this.$t('publics.warnings.processing'));
                                }
                            }, 1000);
                        }
                    })

                    this.exportSignalrConnection.off('downloadUrl');
                    this.exportSignalrConnection.on('downloadUrl', res => {
                        if (res) {
                            sessionStorage.getDownloadStatus = 'true'
                            if (this.exportTimes && Number(this.exportTimes) > 7) {
                                this.$message.warning(this.$t('publics.warnings.readyToExport'));
                                setTimeout(() => {
                                    let link = document.createElement('a');
                                    link.style.display = 'none';
                                    link.href = res;
                                    link.target = 'blank'
                                    document.body.appendChild(link);
                                    link.click();
                                    this.haveDataDwonload = false;
                                    if (this.exportSignalrConnection) {

                                        this.exportSignalrConnection.stop()
                                    }
                                    this.exportSignalrConnection = null
                                    this.getExportCommand = false

                                }, 2000);
                                this.alreadyExport = true
                            } else {
                                let link = document.createElement('a');
                                link.style.display = 'none';
                                link.target = 'blank'
                                link.href = res;
                                document.body.appendChild(link);
                                link.click();
                                this.haveDataDwonload = false
                                this.alreadyExport = true
                                if (this.exportSignalrConnection) {

                                    this.exportSignalrConnection.stop()
                                }
                                this.exportSignalrConnection = null
                                this.getExportCommand = false
                            }

                            if (!existIframe) {
                                this.exportCurveLoading = false
                            } else {
                                if (document.getElementById('jumpIframe')) {
                                    document.getElementById('jumpIframe').contentWindow.postMessage({ exportYes: true }, '*')
                                } else {
                                    if (window.top.opener) {
                                        window.top.opener.postMessage({ exportYes: true }, '*')
                                    }
                                    let arr = Array.from(document.getElementsByTagName('iframe'));
                                    arr.forEach(item => {
                                        item.contentWindow.postMessage({ exportYes: true }, '*')
                                    })
                                }
                            }
                            window.clearInterval(this.exportTimer);
                            this.exportTimer = null;
                            this.exportTimes = 0;
                        }
                    });

                    this.exportSignalrConnection.off('downloadError');
                    this.exportSignalrConnection.on('downloadError', res => {
                        if (res) {
                            this.$message.warning(res);
                            sessionStorage.getDownloadStatus = 'error'
                        }
                    });
                }
            }
        },
        exportCurves (data) {
            this.$api.exportCurve(data).then(res => { 
                console.log(res)
            }).catch(err => {
                this.$message.error(err.data, err)
                console.log(err);
            })
        }
    },
}
