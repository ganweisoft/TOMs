<template>
    <el-dialog :key="key" ref="editPasswordDialog" class="editPassword" @closed="closeDialog" modal-class="editPassword" v-model="dialogVisible" width="480px" top="0vh"
        :close-on-click-modal="false" :show-close="false" center :close-on-press-escape="false" append-to="#index" >
        <template #header>
            <div>
                <span class="el-dialog__title" style="line-height: 24px; font-size: 18px">{{
        $t('login.framePro.title.dialogChangePasswordTitle') }}</span>
                <i v-show="passwordPolicy != 1"
                    style="position: absolute; top: 20px; right: 20px; line-height: 20px; font-size: 14px; color: #909399; cursor: pointer"
                    class="iconfont icon-tubiao24_guanbi" @click.stop.prevent="closeDialog"></i>
            </div>
        </template>
        <el-form :model="changePwd" label-position="top" :rules="rules" ref="changePasswordForm">
            <el-form-item :label="$t('login.framePro.label.originalPassword')" prop="oldPwd">
                <inputPassword v-model="changePwd.oldPwd" :placeholder="$t('login.framePro.tips.enterOriginPas')" />
            </el-form-item>
            <el-form-item :label="$t('login.framePro.label.newPassword')" prop="newPwd">
                <inputPassword v-model="changePwd.newPwd" :placeholder="$t('login.framePro.tips.inputNewPassword')"
                    @change="pwdInputChange" />
            </el-form-item>
            <el-form-item :label="$t('login.framePro.label.confirmPassword')" prop="confPwd">
                <inputPassword v-model="changePwd.confPwd" :placeholder="$t('login.framePro.tips.enterConfirmPas')" />
            </el-form-item>
        </el-form>

        <template #footer>
            <span class="dialog-footer">
                <el-button type="primary" v-if="passwordPolicy != 1" plain @click.stop.prevent="closeDialog">{{
        $t('login.publics.button.cancel') }}</el-button>
                <el-button type="primary" @click.stop.prevent="changeCode()">{{ $t('login.publics.button.confirm') }}
                </el-button>
            </span>
        </template>
    </el-dialog>
</template>

<script src="./index.js"></script>

<style src="./index.scss"></style>
