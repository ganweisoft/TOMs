<template>
    <div class="mainInfo">
        <!-- <video autoplay loop muted>
            <source :src="url" type="video/mp4" />
        </video> -->
        <div class="loginBg">
            <img :src="require('./images/login-bg.jpg')" alt />
        </div>

        <div class="bgImage"></div>
        <el-form ref="licenseForm" :model="licenseForm" class="license" label-position="top">

            <el-form-item :label="$t('login.mainInfoDialog.labels.registryCode')" prop="registrationCode"
                :label-width="labelWidth">
                <el-input v-model="licenseForm.registrationCode" autocomplete="off" disabled></el-input>
            </el-form-item>
            <el-form-item :label="$t('login.mainInfoDialog.labels.lisensStatus')" prop="licenseStatus"
                :label-width="labelWidth">
                <el-input v-model="licenseForm.licenseStatus" autocomplete="off" disabled></el-input>
            </el-form-item>
            <div class="tabs">
                <div class="tabs__nav">
                    <div class="tabs__nav--item" v-for="item in tabNav" :key="item.val"
                        :class="{ active: item.val == activeTab }" @click="changeTab(item.val)">{{ $t(item.name) }}</div>
                </div>
                <div class="tabs__pane">
                    <el-form-item label="" prop="file" :label-width="labelWidth" v-if="activeTab == 0">
                        <el-upload class="upload-demo" ref="upload" action="" :limit="1" accept=".shd"
                            :file-list="licenseForm.file" :on-change="fileListChange" :on-remove="fileListremove"
                            :auto-upload="false">
                            <el-button type="text" slot="trigger" size="small" icon="el-icon-upload2">{{
                                $t('login.mainInfoDialog.button.uploadSHDFile') }}</el-button>
                        </el-upload>
                    </el-form-item>
                    <el-form-item label="" prop="code" :label-width="labelWidth" v-else>
                        <el-input type="textarea" resize="none" :autosize="{ minRows: 3, maxRows: 3 }"
                            :placeholder="$t('login.input.inputLisenceCode')" v-model="licenseForm.code"></el-input>
                    </el-form-item>
                </div>
            </div>
            <!-- <el-form-item :label="$t('login.mainInfoDialog.labels.uploadLisens')" prop="file" :label-width="labelWidth">
                <el-upload class="upload-demo" ref="upload" action="" :limit="1" accept=".shd" :file-list="licenseForm.file" :on-change="fileListChange" :on-remove="fileListremove" :auto-upload="false">
                    <el-button slot="trigger" size="small" icon="el-icon-upload2">{{$t('login.mainInfoDialog.button.uploadSHDFile')}}</el-button>
                </el-upload>
            </el-form-item> -->

            <el-form-item :label="$t('login.mainInfoDialog.labels.serviceStatus')" prop="serviceStatus"
                :label-width="labelWidth">
                <el-input v-model="licenseForm.serviceStatus" autocomplete="off" disabled></el-input>
            </el-form-item>
            <el-form-item style="text-align: center;">
                <el-button size="small" type="primary" @click="DownLoadXlog">{{
                    $t('login.mainInfoDialog.button.downLoadLogs') }}</el-button>
                <el-button size="small" type="primary" @click="changeEvents" v-if="!isInitSate">{{
                    $t('login.mainInfoDialog.button.init') }}</el-button>
                <el-button size="small" type="primary" @click="changeEvents" v-else>{{
                    $t('login.mainInfoDialog.button.updateLisens') }}</el-button>
            </el-form-item>

            <div class="toLogin" @click="toLogin">
                &gt;&gt;&gt;{{ $t('login.button.toLogin') }}&lt;&lt;&lt;
            </div>
        </el-form>

        <el-dialog :title='$t(dialogTitle)' :visible.sync="loadingDialogVisible" center :show-close="false"
            :close-on-click-modal="false" class="loadingModal">
            <el-progress :percentage="percentage"></el-progress>
        </el-dialog>
    </div>
</template>

<script>
import mainInfo from './js/mainInfo.js';
export default mainInfo;
</script>
<style scoped lang="scss" src="./css/mainInfo.scss"></style>