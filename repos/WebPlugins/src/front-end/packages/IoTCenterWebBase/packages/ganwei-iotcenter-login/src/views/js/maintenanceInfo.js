/*
 * @Description:
 * @LastEditTime: 2024-07-24 16:49:14
 */
// import _ from 'lodash';
import sha512 from 'js-sha512'
import showPassword from '@components/@ganwei-pc/gw-base-components-plus/showPassword/indexOption.vue'
export default {
    components: {
        showPassword
    },
    data () {
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
            firstLogin: false, // 首次登录
            secondLogin: false, // 二次登录
            firstLoading: false,
            secondLoading: false,
            url: '',
            homeBgImg: sessionStorage.getItem("bgImg")
        };
    },
    computed: {
        FormRules () {
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
    created () {
        this.IsInitMaintainPwd();
    },
    methods: {
        showBgImg(){
            this.homeBgImg = "/static/images/login-bg-img.png"
        },
        toLogin () {
            this.$router.push('/')
        },

        // 首次登录
        IsInitMaintainPwd () {
            this.$api.IsInitMaintainPwd().then(res => {
                const { code, data, message } = res?.data || {};
                if (code === 200) {
                    const initData = data ? JSON.parse(data) : {};
                    if (
                        initData?.initStatus === 0 ||
                        initData?.initStatus === 1
                    ) {
                        this.firstLogin = true;
                        this.secondLogin = false;
                    } else if (initData?.initStatus === 2) {
                        this.firstLogin = false;
                        this.secondLogin = true;
                    }
                } else {
                    this.$message.warning(message);
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            })
        },

        async confirm (props, DefaultForm) {
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
        InitAdminPwd (formData) {
            this.firstLoading = true;
            this.$api
                .InitAdminPwd(formData)
                .then(res => {
                    const { code, message } = res?.data || {}
                    if (code === 200) {
                        this.firstLogin = false;
                        this.secondLogin = true;
                        this.$message.success(message);
                    } else {
                        this.$message.error(message);
                    }
                    this.firstLoading = false;
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                    this.firstLogin = true;
                    this.secondLogin = false;
                    this.firstLoading = false;
                });
        },

        // 验证管理员密码
        VerifyLogin (formData) {
            this.$api
                .VerifyLogin(formData)
                .then(res => {
                    const { code, message } = res?.data || {}
                    if (code === 200) {
                        this.firstLogin = false;
                        this.secondLogin = false;
                        this.$message.success(message);
                        this.$router.replace('/mainInfo');
                    } else {
                        this.$message.error(message);
                    }
                    this.secondLoading = false;
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                    this.secondLoading = false;
                    this.firstLogin = false;
                    this.secondLogin = true;
                });
        }
    }
};
