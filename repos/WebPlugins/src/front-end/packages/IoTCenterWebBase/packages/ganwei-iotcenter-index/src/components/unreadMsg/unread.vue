<template>
    <div class="messageIcon">
        <div class="messageIconMask" @click="toShowDrawer"></div>
        <el-badge :value="totalMessage" class="item" :max="99">
            <img :src="`/static/images/unread-message-${theme.value}.svg`" alt="" />
        </el-badge>
        <el-drawer class="message-el-drawer" :wrapperClosable="false" custom-id="message-el-drawer" :size="400" title=""
            v-model="drawer" direction="rtl" :before-close="handleClose" append-to="#index">
            <div class="message-content" v-if="drawer">
                <header>
                    <span class="item-left"><img :src="`/static/images/unread-message-${theme.value}.svg`" alt="" />
                        {{ $t('login.publics.notice') }}
                        <span class="sound" v-if="showSound" @click="closeSpeak"><i
                                class="iconfont icon-shengyin1"></i></span>
                        <span class="sound" v-else @click="openSpeak"><i class="iconfont icon-jingyin"></i></span>
                    </span>
                    <span class="item-right" @click="toRouter">{{ $t('login.publics.more') }}<i
                            class="iconfont icon-tubiao14_shouqi"></i></span>
                </header>
                <div v-if="msgList.length" class="list">
                    <div class="message-content-item" :style="{ background: source.backgroundColor }"
                        @click="contentItemClick(source, index)" v-for="(source, index) in msgList" :key="index">
                        <div class="item-bottom">
                            <div class="item-label item-left" :style="{ color: source.color }">
                                <img :src="source.iconImage" alt="">
                                {{ source.snapshotName }}
                            </div>
                            <div class="item-time item-right">
                                {{ source.Time }}
                            </div>
                        </div>
                        <p class="item-top">{{ source.eventMsg }}</p>
                        <!-- <div class="message-content-line"></div> -->
                    </div>
                </div>
                <div class="noDataTips" v-else :data-noData="$t('login.publics.noData')">
                </div>
            </div>
            <widthSetting leftSide="message-el-drawer" side="left" :minWidth="400" />
        </el-drawer>
    </div>
</template>

<script>
import widthSetting from '@components/@ganwei-pc/gw-base-components-plus/widthSetting/widthSetting.vue'
export default {
    name: 'unread',
    components: {
        // VirtualList,
        widthSetting
    },
    props: {
        messageList: {
            type: Array,
            default: () => []
        },
        theme: {
            type: Object,
            default: () => ({
                value: 'dark'
            })
        },
        showSound: {
            type: Boolean,
            default: false
        }
    },
    data () {
        return {
            // itemComponent: listItem,
            showDropdown: false,
            drawer: false,
            totalMessage: 0,
            msgList: [],
            timer: null
        }
    },

    watch: {
        'messageList.length' (val) {
            this.totalMessage = val
            // this.msgList = JSON.parse(JSON.stringify(this.messageList))
        }
    },

    mounted () {
        this.msgList = JSON.parse(JSON.stringify(this.messageList));
        this.timer = setInterval(() => {
            this.msgList = JSON.parse(JSON.stringify(this.messageList));
        }, 1000)
    },

    methods: {
        toShowDrawer () {
            this.drawer = true;
        },
        contentItemClick (data, index) {
            window.top.snapshotData = JSON.parse(JSON.stringify(data))
            let iframe = document.getElementsByTagName("iframe");
            for (let item of iframe) {
                item.contentWindow.postMessage({ haveNewSnapshotData: true })
            }
            this.$router.push({ path: '/Index/jumpIframe/ganwei-iotcenter-system-snapshot/systemSnapshot' });
            this.$emit('updateMsgList', index)
            this.drawer = false
        },
        handleClose () {
            this.drawer = false
        },
        toRouter () {
            this.drawer = false
            this.$emit('jump');
        },
        closeSpeak () {
            this.$emit('closeSpeak')
        },
        openSpeak () {
            this.$emit('openSpeak')
        }
    }
}
</script>
<style lang="scss" scoped>
.list {
    margin-top: -20px;
}

.list-item-dynamic {
    &:not(:first-child) {
        margin-top: 20px;
    }
}

.message-content-item {
    width: 100%;
    line-height: 38px;
    margin-top: 20px;
    color: var(--frame-main-color);
    cursor: pointer;
    border-radius: 4px;
    padding: 10px;
    background: var(--frame-main-background-3);

    .item-bottom {
        display: flex;
        justify-content: space-between;

        .item-left {
            img {
                width: 14px;
                height: 14px;
                transform: translateY(1px);
            }
        }

        .item-right {
            flex: 1;
            text-align: right;
            font-size: 12px;
        }

    }

    &:hover {

        .item-left,
        .item-top,
        .item-right {
            color: var(--gw-color-primary) !important;
            // @include font_color("con-themeColor");
        }
    }
}

.messageIcon {
    position: relative;
    user-select: none;

    .messageIconMask {
        width: 100%;
        height: 100%;
        position: absolute;
        top: 0px;
        left: 0px;
        z-index: 100;
    }
}

::v-deep .el-badge {
    line-height: 1;

    .el-badge__content.is-fixed {
        top: 5px;
        right: 13px;
    }

    .el-badge__content {
        background-color: #ff0024;
        border-color: #ff0024;
    }
}

.message-el-drawer {
    white-space: normal !important;
    overflow: visible !important;

    .el-drawer__header {
        margin-bottom: 0px;
    }

    .el-drawer__body {
        height: calc(100% - 43px);
        padding-bottom: 10px;

        header {
            line-height: 50px;
            margin-bottom: 8px;
            display: flex;
            justify-content: space-between;

            img {
                width: 24px;
                height: 24px;
                margin-right: 4px;
            }

            .item-left {
                font-size: 16px;
                display: flex;
                align-items: center;

                .iconfont {
                    font-size: 36px;
                }

                .sound {
                    margin-left: 12px;
                    margin-top: 4px;

                    i {
                        font-size: 20px;
                    }
                }
            }

            .item-right {
                font-size: 14px;
                cursor: pointer;
                color: var(--gw-color-primary);

                .iconfont {
                    font-size: 12px;
                    margin-left: 8px;
                    border-radius: 50%;
                    border: 1px solid;
                    // @include border_color('notifytext');
                }
            }
        }
    }

    .message-content {
        padding: 0 16px 28px !important;
        height: 100%;
    }

    .list {
        height: calc(100% - 51px);
        overflow: auto;
        font-size: 14px;

        .item-top {
            // color: var(--notifytext);
            text-align: left;
        }

        .item-left {
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis;
            color: var(--con-textColor1);
            font-size: 16px;
        }

        .item-right {
            width: 130px;
            text-align: right;
        }
    }

    .noData {
        width: 100%;
        height: calc(100% - 51px);
        display: flex;
        justify-content: center;
        align-items: center;
    }
}
</style>
