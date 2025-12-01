<template>
    <div class="maintain">
        <div class="loginBg">
            <img :src="require('./images/login-bg.jpg')" alt />
        </div>

        <div class="default">
            <div class="bgImage"></div>
            <el-form ref="DefaultForm" :model="DefaultForm" :rules="FormRules" v-show="firstLogin" @submit.native.prevent>
                <el-form-item>
                    <h3>{{ $t('login.maintainDialog.title.inputDefaultPWD') }}</h3>
                </el-form-item>
                <el-form-item :label="$t('login.maintainDialog.labels.defaultPWD')" prop="DefaultPassword"
                    :label-width="labelWidth">
                    <el-input v-model.trim="DefaultForm.DefaultPassword" onkeyup="this.value=this.value.replace(/\s+/g,'')"
                        autocomplete="off" type="password" show-password>
                    </el-input>
                </el-form-item>
                <el-form-item :label="$t('login.maintainDialog.labels.adminPWD')" prop="InputPassword"
                    :label-width="labelWidth">
                    <el-input v-model.trim="DefaultForm.InputPassword" onkeyup="this.value=this.value.replace(/\s+/g,'')"
                        autocomplete="off" type="password" show-password>
                    </el-input>
                </el-form-item>
                <el-form-item :label="$t('login.maintainDialog.labels.confirmAdminPWD')" prop="confirmAdminPwd"
                    :label-width="labelWidth">
                    <el-input v-model.trim="DefaultForm.confirmAdminPwd" onkeyup="this.value=this.value.replace(/\s+/g,'')"
                        autocomplete="off" type="password" @keyup.enter.native="confirm('firstLogin', 'DefaultForm')"
                        show-password></el-input>
                </el-form-item>
                <el-form-item style="text-align: center;">
                    <el-button type="primary" size="small" @click.stop="confirm('firstLogin', 'DefaultForm')"
                        :loading="firstLoading">{{ $t('login.publics.button.confirm') }}</el-button>
                </el-form-item>
                <div class="toLogin" @click="toLogin">
                    &gt;&gt;&gt;{{ $t('login.button.toLogin') }}&lt;&lt;&lt;
                </div>
            </el-form>

            <el-form ref="testForm" :model="testForm" :rules="FormRules" v-show="secondLogin" @submit.native.prevent>
                <el-form-item>
                    <h3>{{ $t('login.maintainDialog.title.inputAdminPWD') }}</h3>
                </el-form-item>
                <el-form-item :label="$t('login.maintainDialog.labels.adminPWD')" prop="password" :label-width="labelWidth">
                    <el-input v-model.trim="testForm.password" onkeyup="this.value=this.value.replace(/\s+/g,'')"
                        autocomplete="off" type="password" @keyup.enter.native="confirm('secondLogin', 'testForm')"
                        show-password>
                    </el-input>
                </el-form-item>
                <el-form-item style="text-align: center;">
                    <el-button type="primary" size="small" @click.stop="confirm('secondLogin', 'testForm')"
                        :loading="secondLoading">{{ $t('login.publics.button.confirm') }}</el-button>
                </el-form-item>
                <div class="toLogin" @click="toLogin">
                    &gt;&gt;&gt;{{ $t('login.button.toLogin') }}&lt;&lt;&lt;
                </div>
            </el-form>

        </div>
    </div>
</template>
<script>
import multipleAccess from './js/maintenanceInfo.js';
export default multipleAccess;
</script>
<style scoped lang="scss" src="./css/maintenanceInfo.scss"></style>