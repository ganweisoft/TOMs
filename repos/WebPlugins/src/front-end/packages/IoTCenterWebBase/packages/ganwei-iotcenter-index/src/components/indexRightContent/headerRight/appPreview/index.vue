<template>
    <div v-if="showApp" class="appPreview">
        <el-tooltip popper-class="topToolTip" :content="$t('login.framePro.tips.appPreview')" placement="bottom">
            <img :src="theme.appPreview" alt="" @click="showPreview = true" />
        </el-tooltip>
        <el-dialog v-model="showPreview" className="el-dialog dialogContainer" width="750px" draggable :close-on-click-modal="false" append-to="#index">
            <div class="appPreview">
                <div class="tabs-wrapper">
                    <div class="tabs-header">
                        <div class="tabs-header-item" :class="{ active: activeName === 'preview' }"
                            @click="changeActiveName('preview')">
                            <span class="label">{{ $t('login.framePro.label.preview') }}</span>
                        </div>
                        <div class="tabs-header-item" :class="{ active: activeName === 'download' }"
                            @click="changeActiveName('download')">
                            <span class="label">{{ $t('login.framePro.label.download') }}</span>
                        </div>
                    </div>
                    <div class="tabs-body">
                        <div class="tabs-body-item" v-show="activeName === 'preview'">
                            <div class="QRcode">
                                <div class="image">
                                    <img :src="QRcode.app" alt="">
                                </div>
                                <el-button type="primary" @click="doCopy(AppUrl)">{{
        $t('login.framePro.button.copyLink') }}</el-button>
                            </div>
                            <iframe class="AppFrame" :src="AppUrl"
                                v-show="activeName === 'preview' || finishTour"></iframe>
                        </div>
                        <div class="tabs-body-item" v-show="activeName === 'download'">
                            <div class="QRcode">
                                <div class="image">
                                    <img :src="QRcode.download" alt="">
                                </div>
                                <el-button type="primary" @click="doCopy(connectUrl)">{{
        $t('login.framePro.button.copyLink') }}</el-button>
                                <el-button type="primary" @click="openTour">{{ $t('login.framePro.button.tour')
                                    }}</el-button>
                            </div>
                            <div class="AppFrame AppConnect">
                                <span class="tour">
                                    <img src="./images/appconnect.jpg" alt="">
                                    <div ref="language" class="language-target"></div>
                                    <div ref="url" class="url-target"></div>
                                    <div ref="button" class="button-target"></div>
                                    <el-tour v-model="open" @finish="onFinishTour" v-if="open">
                                        <el-tour-step :target="$refs.language"
                                            :title="$t('login.framePro.title.changeLanguage')">
                                            <div>{{ $t('login.framePro.tips.changeLanguage') }}</div>
                                        </el-tour-step>
                                        <el-tour-step :target="$refs.url" :title="$t('login.framePro.title.serverUrl')"
                                            :description="$t('login.framePro.tips.serverUrl')" />
                                        <el-tour-step :target="$refs.button"
                                            :title="$t('login.framePro.title.connectServer')"
                                            :description="$t('login.framePro.tips.connectServer')" />
                                    </el-tour>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </el-dialog>
    </div>
</template>

<script setup>
import { inject, nextTick, onMounted, reactive, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import QRCodeLib from 'qrcode'

import api from '@/request/api'

const { t } = useI18n()
const theme = inject('theme')
const $message = inject('$message')

const showApp = ref(false)
const QRcode = reactive({
    app: '',
    download: ''
})
const showPreview = ref(false)

const activeName = ref('preview')
const open = ref(false)
const finishTour = ref(false)

const AppUrl = window.location.origin + '/APP/index.html?languageType=' + sessionStorage.getItem('languageType')
const downloadUrl = ''
const connectUrl = window.location.host

function changeActiveName (name) {
    activeName.value = name
    if (name === 'download' && !finishTour.value) {
        openTour()
    }
}

function openTour () {
    finishTour.value = false
    setTimeout(() => {
        open.value = true
    }, 200)
}

function onFinishTour () {
    finishTour.value = true
}

function doCopy (str) {
    const textarea = document.createElement('textarea');
    textarea.style.position = 'fixed';
    textarea.style.opacity = 0;
    textarea.value = str;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    $message.success(t('login.framePro.tips.copySuccess'))
}

onMounted(() => {

    QRCodeLib.toDataURL(AppUrl).then(url => {
        QRcode.app = url
    })
    QRCodeLib.toDataURL(downloadUrl).then(url => {
        QRcode.download = url
    })
})
</script>

<style lang="scss" scoped>
.appPreview,
.tabs-wrapper,
.tabs-body-item,
.AppFrame {
    height: 100%;
}

.appPreview {
    .tabs-wrapper {
        .tabs-header {
            display: flex;
            align-items: center;
            justify-content: flex-start;
            margin-bottom: 20px;
            cursor: auto;
        }

        .tabs-header-item {
            color: var(--tab-color);
            border-bottom: 1px solid transparent;
            height: 32px;
            cursor: pointer;

            &+.tabs-header-item {
                margin-left: 60px;
            }

            .label {
                line-height: 32px;
                font-size: 18px;
            }

            &.active {
                color: var(--tab-color__active);
                border-bottom: 2px solid var(--tab-background__active);
            }
        }

        .tabs-body {
            height: calc(100% - 52px);
        }

        .tabs-body-item {
            display: flex;
            justify-content: space-around;
            align-items: center;
            cursor: auto;
        }
    }

}

.AppFrame {
    width: 360px;

    .tour {
        display: inline-block;
        height: 100%;
        position: relative;

        img {
            height: 100%;
        }
    }
}

.AppConnect {
    cursor: auto;

    .language-target {
        position: absolute;
        top: 4.6%;
        right: 10.5%;
        width: 20%;
        height: 4.6%;
        cursor: pointer;
    }

    .url-target {
        position: absolute;
        top: 24%;
        right: 0;
        width: 100%;
        height: 9.6%;
        cursor: pointer;
    }

    .button-target {
        position: absolute;
        top: 35%;
        right: 0;
        width: 100%;
        height: 9.6%;
        cursor: pointer;
    }
}

.QRcode {
    .image {
        margin-bottom: 20px;
    }
}
.el-tour {
    --el-tour-width: 300px;
}
</style>

<style lang="scss">
.dialogContainer {
    height: 90%;
    margin: 0;

    .el-dialog__header {
        height: 32px !important;
        padding: 0 !important;
    }

    .el-dialog__body {
        height: calc(100% - 32px - 32px);
    }
}
.el-tour {
    --tour-background: var(--gw-color-Neutrals27);
    --tour-border: var(--gw-color-Neutrals22);
    --el-tour-width: 350px;

    .el-tour__content {
        background: var(--tour-background);
        border: 1px solid var(--tour-border);
    }

    .el-tour__arrow {
        background: var(--tour-background);
    }

    .el-tour__content[data-side^=bottom] .el-tour__arrow {
        border-top: 1px solid var(--tour-border);
        border-left: 1px solid var(--tour-border);
    }
}
</style>
