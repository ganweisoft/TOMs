// import _ from 'lodash';
export default {
    data() {
        return {

            // 上传文件表单
            uploadForm: {
                systemInfo: '',
                serviceLog: '',
                file: []
            },
            loading: false,
            labelWidth: '150px',
            DefaultForm: {
                DefaultPassword: '',
                InputPassword: '',
                confirmAdminPwd: ''
            }, // 默认密码数据
            testForm: {
                password: ''
            }, //
            firstLogin: Boolean, // 首次登录
            secondLogin: false, // 二次登录
            firstLoading: false,
            secondLoading: false,
            url: ''
        };
    },
    computed: {
        FormRules() {
            return {
                DefaultPassword: [
                    { required: true, message: this.$t('login.maintainDialog.tips.inputPassword'), trigger: 'blur' }
                ],
                InputPassword: [
                {
                    required: true,
                    validator: (rule, value, callback) => {
                        if (value === '') {
                            callback(new Error(this.$t('login.maintainDialog.tips.inputAdminPWD')));
                        } else if (value === this.DefaultForm.DefaultPassword) {
                            callback(new Error(this.$t('login.maintainDialog.tips.adminPWDFitDefaultPWD')));
                        } else {
                            callback();
                        }
                    },
                    trigger: 'blur'
                }],
                confirmAdminPwd: [
                {
                    required: true,
                    validator: (rule, value, callback) => {
                        if (value === '') {
                            callback(new Error(this.$t('login.maintainDialog.tips.inputAdminPWDAgain')));
                        } else if (value !== this.DefaultForm.InputPassword) {
                            callback(new Error(this.$t('login.maintainDialog.tips.twoPWDNotFit')));
                        } else {
                            callback();
                        }
                    },
                    trigger: 'blur'
                }],
                password: [
                {
                    required: true,
                    message: this.$t('login.maintainDialog.tips.inputAdminPWD'),
                    trigger: 'blur'
                }]
            }
        }
    },
    created() {
        this.IsInitMaintainPwd();
    },
    methods: {
        toLogin() {
            this.$router.push('/Login')
        },

        // 首次登录
        IsInitMaintainPwd() {
            this.$api.IsInitMaintainPwd().then(res => {
                const { code, data, message } = res.data;
                if (code === 200) {
                    const initData = JSON.parse(data);
                    if (
                        initData.initStatus === 0 ||
                        initData.initStatus === 1
                    ) {
                        this.firstLogin = true;
                        this.secondLogin = false;
                    } else if (initData.initStatus === 2) {
                        this.firstLogin = false;
                        this.secondLogin = true;
                    }
                } else {
                    this.$message.warning(message, res);
                }
            }).catch(err => {
                this.$message.error(err.data, err)
            })
        },

        async confirm(props, DefaultForm) {
            window.sessionStorage.clear();
            let sha512 = require('js-sha512');
            if (props === 'firstLogin') {
                this.firstLogin = true;
                this.secondLogin = false;
                let firstFormData = {
                    DefaultPassword: await this.$getCode.RSAEncrypt(
                        sha512(this.DefaultForm.DefaultPassword)
                    ),
                    InputPassword: await this.$getCode.RSAEncrypt(sha512(this.DefaultForm.InputPassword))
                };
                this.$refs[DefaultForm].validate(valid => {
                    if (valid) {
                        this.InitAdminPwd(firstFormData);
                    } else {
                        console.log('error submit!!');
                        return false;
                    }
                });
            }
            if (props === 'secondLogin') {
                this.secondLoading = true;
                this.firstLogin = false;
                this.secondLogin = true;
                this.$refs[DefaultForm].validate(async valid => {
                    if (valid) {
                        this.VerifyLogin({
                            InputPassword: await this.$getCode.RSAEncrypt(sha512(this.testForm.password))
                        });
                    } else {
                        console.log('error submit!!');
                        this.secondLoading = false;
                        return false;
                    }
                });
            }
        },

        // 初始化管理员密码
        InitAdminPwd(formData) {
            this.firstLoading = true;
            this.$api
                .InitAdminPwd(formData)
                .then(res => {
                    if (res.data.code === 200) {
                        this.firstLogin = false;
                        this.secondLogin = true;
                        this.$message.success(res.data.message);
                    } else {
                        this.$message.error(res.data.message, res);
                    }
                    this.firstLoading = false;
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                    console.log(err, 'err');
                    this.firstLogin = true;
                    this.secondLogin = false;
                    // this.$message.success(err.data.message);
                    this.firstLoading = false;
                });
        },

        // 验证管理员密码
        VerifyLogin(formData) {
            this.$api
                .VerifyLogin(formData)
                .then(res => {
                    if (res.data.code === 200) {
                        this.firstLogin = false;
                        this.secondLogin = false;
                        this.$message.success(res.data.message);
                        window.sessionStorage.clear();
                        this.$router.replace('/mainInfo');
                    } else {

                        this.$message.error(res.data.message, res);
                    }
                    this.secondLoading = false;
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                    console.log(err);
                    this.secondLoading = false;
                    this.firstLogin = false;
                    this.secondLogin = true;
                    // this.$message.success(err.data.message);
                });
        }
    }
};