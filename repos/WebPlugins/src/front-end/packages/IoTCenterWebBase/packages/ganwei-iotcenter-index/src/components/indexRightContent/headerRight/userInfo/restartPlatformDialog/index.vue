/*
 * @Description:
 * @LastEditTime: 2024-09-29 16:49:45
 */
<template>
    <el-dialog class="editPassword" @closed="closeDialog" v-model="dialogVisible" width="480px"
        :close-on-click-modal="false" :show-close="false" top="-10vh" center append-to="#index">
        <template #header>
            <div>
                <span class="el-dialog__title" style="line-height: 24px; font-size: 18px">{{
        $t('login.framePro.title.RestartPlatform') }}</span>
                <i style="position: absolute; top: 20px; right: 20px; line-height: 20px; font-size: 14px; color: #909399; cursor: pointer"
                    class="iconfont icon-tubiao24_guanbi" @click.stop.prevent="closeDialog"></i>
            </div>
        </template>
        <el-form :model="form" label-position="top" :rules="rules" ref="form">
            <el-form-item :label="$t('login.framePro.label.password')" prop="password">
                <inputPassword v-model="form.password" :placeholder="$t('login.framePro.tips.inputPasswordTip')" />
            </el-form-item>
            <el-form-item :label="$t('login.framePro.label.confirmPassword')" prop="confirmPassword">
                <inputPassword v-model="form.confirmPassword"
                    :placeholder="$t('login.framePro.tips.enterConfirmPas')" />
            </el-form-item>
        </el-form>

        <template #footer>
            <span class="dialog-footer">
                <el-button type="primary" plain @click.stop.prevent="closeDialog">{{
        $t('login.publics.button.cancel') }}</el-button>
                <el-button type="primary" :loading="loading" @click.stop.prevent="restartPlatform()">{{
        $t('login.publics.button.confirm') }}
                </el-button>
            </span>
        </template>

        <!-- 重启 -->
        <el-dialog :title="$t('login.framePro.tips.restartPlateforming')" width="420px" v-model="isRebootWeb"
            :close-on-click-modal="false" append-to-body :show-close="false" top="-10vh" center class="loadingModal">
            <el-progress :percentage="percentage"></el-progress>
        </el-dialog>
    </el-dialog>
</template>

<script src="./index.js"></script>

<style src="./index.scss"></style>
