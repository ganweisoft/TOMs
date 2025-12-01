
export default {

    data () {
        return {
            licenseForm: {
                registrationCode: '',
                file: [],
                licenseStatus: '',
                serviceStatus: '',
                code: ''
            },
            labelWidth: '150px',
            timeInterval: null, // 定时器
            timeOut: null,
            isInitSate: false,
            percentage: 0,
            timeStart: '0',
            loadingDialogVisible: false,
            dialogTitle: 'login.mainInfoDialog.tips.initing',
            tipsMessage: '',
            formData: '',
            tabNav: [
                {
                    name: 'login.button.uploadLisence',
                    val: 0
                }],
            activeTab: 0,
            homeBgImg: sessionStorage.getItem("bgImg")
        };
    },
    async created () {
        await this.GetInitSate();
        await this.GetLicenseInfo();
        await this.GetServiceStatus();
        this.timeInterval = setInterval(async () => {
            await this.GetLicenseInfo();
            await this.GetServiceStatus();
        }, 5000);
    },
    methods: {
        showBgImg(){
            this.homeBgImg = "/static/images/login-bg-img.png"
        },
        toLogin () {
            this.$router.push('/')
        },
        changeTab (val) {
            this.activeTab = val;
        },

        // 获取服务状态
        GetServiceStatus () {
            this.$api
                .GetServiceStatus()
                .then(res => {
                    const { code, message, data } = res?.data || {}
                    if (code === 200) {
                        if (data?.serviceStatus == 2) {
                            this.licenseForm.serviceStatus = this.$t(
                                'login.mainInfoDialog.tips.startUp'
                            );
                        } else {
                            // this.$message.error(message);
                            this.licenseForm.serviceStatus = this.$t(
                                'login.mainInfoDialog.tips.noStartUp'
                            );
                        }
                    }
                })
                .catch(error => {
                    this.$message.error(error?.data, error)
                });
        },

        getStatus () {
            this.$api
                .GetServiceStatus()
                .then(res => {
                    const { code, data } = res?.data || {}
                    if (code === 200) {
                        if (data?.licenseStatus === false) {
                            this.licenseForm.serviceStatus = this.$t(
                                'login.mainInfoDialog.tips.noStartUp'
                            );
                            this.loadingDialogVisible = false;
                            this.$message.error(this.$t('login.mainInfoDialog.tips.lisenceNoFit'));
                            return;
                        }
                        if (data?.serviceStatus == 2) {
                            this.licenseForm.serviceStatus = this.$t(
                                'login.mainInfoDialog.tips.startUp'
                            );
                            this.$message.success(this.tipsMessage);
                            this.RebootWeb();
                            setTimeout(async () => {
                                this.loadingDialogVisible = false;
                                this.percentage = 100;
                                this.$router.push('/')
                            }, 30000);
                        } else if (data?.serviceStatus == 0) {
                            this.licenseForm.serviceStatus = this.$t(
                                'login.mainInfoDialog.tips.noStartUp'
                            );
                            this.loadingDialogVisible = false;
                            setTimeout(async () => {
                                await this.GetLicenseInfo();
                                await this.GetServiceStatus();
                            }, 5000)
                        }
                    }
                })
                .catch(error => {
                    this.loadingDialogVisible = false;
                    this.$message.error(error.data, error)
                    console.log('error:', error);
                })
        },

        // 获取注册码与许可状态
        GetLicenseInfo () {
            this.$api
                .GetLicenseInfo()
                .then(res => {
                    const { code, data, message } = res?.data || {};
                    if (code === 200) {
                        this.licenseForm.registrationCode = data?.authCode || '';
                        this.licenseForm.licenseStatus = data?.licenseState || '';
                    } else {
                        // this.$message.error(message);
                        this.licenseForm.licenseStatus = message;
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                });
        },

        // 获取初始化状态
        GetInitSate () {
            this.$api.GetInitSate().then(res => {
                const { code, message } = res?.data || {};
                if (code === 200) {
                    this.isInitSate = false;
                } else {
                    this.isInitSate = true;
                    // this.$message.error(message);
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            })
        },

        // 选中文件改变事件
        fileListChange (file, fileList) {
            this.licenseForm.file = fileList;
        },

        // 文件删除事件
        fileListremove (file, fileList) {
            this.licenseForm.file = fileList;
        },

        async changeEvents () {
            this.percentage = 0;
            this.formData = new FormData();
            if (this.licenseForm.file.length) {
                this.formData.append('file', this.licenseForm.file[0].raw);
            } else {
                this.$message.warning(
                    this.$t('login.mainInfoDialog.tips.notUpload')
                );
                return;
            }
            clearInterval(this.timeInterval);
            this.timeInterval = null;
            if (this.isInitSate) {
                this.dialogTitle = this.$t(
                    'login.mainInfoDialog.tips.updatingLisens'
                );
                this.tipsMessage = this.$t(
                    'login.mainInfoDialog.tips.updateSuccess'
                );
                await this.getLicenseStatus(this.licenseForm.licenseStatus);
            } else {
                this.loadingDialogVisible = true;
                this.dialogTitle = this.$t('login.mainInfoDialog.tips.initing');
                this.tipsMessage = this.$t(
                    'login.mainInfoDialog.tips.initSuccess'
                );
                this.UploadLicense();
            }
        },

        async getLicenseStatus (licenseStatus) {
            if (licenseStatus === '已生效') {
                this.$confirm(
                    this.$t('login.mainInfoDialog.tips.lisensIsEffectIfNext'),
                    this.$t('login.mainInfoDialog.tips.tip'),
                    {
                        confirmButtonText: this.$t(
                            'login.publics.button.confirm'
                        ),
                        cancelButtonText: this.$t(
                            'login.publics.button.cancel'
                        ),
                        type: 'warning'
                    }
                )
                    .then(async () => {
                        this.loadingDialogVisible = true;
                        await this.UpdateLicense(this.formData);
                    })
                    .catch(err => {
                        console.log(err);
                    });
            } else {
                this.loadingDialogVisible = true;
                await this.UpdateLicense(this.formData);
            }
        },

        // 更新许可
        UpdateLicense (formData) {
            this.$api
                .UpdateLicense(formData)
                .then(res => {
                    const { code, message } = res?.data || {};
                    if (code === 200) {
                        this.$message.success(message);
                        this.InitService();
                    } else {
                        this.loadingDialogVisible = false;
                        this.$message.error(message);
                    }
                })
                .catch(err => {
                    this.loadingDialogVisible = false;
                    this.$message.error(err?.data, err)
                });
        },

        // 上传许可文件
        async UploadLicense () {
            let formData = new FormData();
            this.percentage = 0;
            if (this.licenseForm.file.length) {
                formData.append('file', this.licenseForm.file[0].raw);
            } else {
                this.$message.error(this.$t('login.mainInfoDialog.button.uploadSHDFile'));
                return;
            }
            await this.$api
                .UploadLicense(formData)
                .then(res => {
                    const { code, message } = res?.data || {}
                    if (code === 200) {
                        this.$message.success(message);
                        this.loadingDialogVisible = true;
                        this.InitService();
                    } else {
                        this.$message.error(
                            message ||
                            this.$t('manageLicenses.tips.err')
                        );
                        this.licenseForm.file = [];
                        this.loadingDialogVisible = false;
                    }
                })
                .catch(error => {
                    this.licenseForm.file = [];
                    this.$message.error(error?.data)
                    console.log('error:', error);
                });
        },

        // 初始化
        async InitService () {
            setTimeout(() => {
                this.Reboot();
            }, 5000);
            setTimeout(() => {
                this.$api
                    .InitService()
                    .then(res => {

                    })
                    .catch(error => {
                        this.loadingDialogVisible = false;
                        this.$message.error(error?.data)
                    });
            }, 30000);

            this.timeOut = setTimeout(async () => {
                await this.getStatus();
            }, 35000);
        },

        // 重启网关
        Reboot () {
            this.$api
                .Reboot()
                .then(() => { })
                .catch(error => {
                    this.$message.error(error?.data)
                });
        },

        // 重启网站
        RebootWeb () {
            this.$api
                .RebootWeb()
                .then(() => { })
                .catch(() => { });
        },

        // 下载日志
        DownLoadXlog () {
            let url = this.$api.DownLoadXlog();
            let link = document.createElement('a');
            link.style.display = 'none';
            link.href = url;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },

        increase () {
            this.timeStart = setInterval(() => {
                if (this.percentage < 98) {
                    this.percentage += 2;
                }
                if (this.percentage > 100) {
                    this.percentage = 100;
                }
            }, 1500);
        },
        increaseend () {
            this.percentage = 100;
            clearInterval(this.timeStart);
        }
    },
    watch: {

        // 监听loading状态控制进度条显示
        loadingDialogVisible (value, newValue) {
            if (value) {
                this.increase();
            } else {
                this.increaseend();
            }
        }
    },
    beforeDestroy () {
        clearInterval(this.timeInterval);
        this.timeInterval = null;
        if (this.timeOut) {
            clearTimeout(this.timeOut);
            this.timeOut = null;
        }
    },
    destroyed () {
        clearInterval(this.timeInterval);
        this.timeInterval = null;
        if (this.timeOut) {
            clearTimeout(this.timeOut);
            this.timeOut = null;
        }
    }
};
