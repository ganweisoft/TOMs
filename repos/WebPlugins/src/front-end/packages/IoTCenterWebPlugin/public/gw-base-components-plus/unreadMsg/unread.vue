<template>
    <div class="messageIcon">
        <div class="messageIconMask" @click="toShowDrawer"></div>
        <el-badge :value="totalMessage" class="item" :max="99">
            <img :src="`/static/images/unread-message-${theme.value}.svg`" alt="" />
        </el-badge>
        <el-drawer custom-class="message-el-drawer" :wrapperClosable="false" custom-id="message-el-drawer" :size="400"
            title="" :visible.sync="drawer" direction="rtl" :before-close="handleClose">
            <div class="message-content">
                <header>
                    <span class="item-left"><img :src="`/static/images/unread-message-${theme.value}.svg`" alt="" />{{
                        $t('login.publics.notice')
                        }}
                        <span class="sound" v-if="showSound" @click="closeSpeak"><i
                                class="iconfont icon-shengyin1"></i></span>
                        <span class="sound" v-else @click="openSpeak"><i class="iconfont icon-jingyin"></i></span>
                    </span>
                    <span class="item-right" @click="toRouter">{{ $t('login.publics.more') }}<i
                            class="iconfont icon-tubiao14_shouqi"></i></span>

                </header>
                <virtual-list v-if="messageList.length" ref="virtualList" class="list" data-key="GUID"
                    :data-sources="messageList" :data-component="itemComponent" :keeps="50" :extra-props="{
                        contentItemClick
                    }"></virtual-list>

                <div class="noDataTips" :data-noData="$t('login.publics.noData')" v-else>
                </div>
            </div>
            <widthSetting leftSide="message-el-drawer" side="left" :minWidth="400" />
        </el-drawer>
    </div>
</template>
<script>
    import virtualList from 'vue-virtual-scroll-list'
    import listItem from './listItem.vue'
    import widthSetting from 'gw-base-components-plus/widthSetting/widthSetting.vue'
    export default {
        name: 'unread',
        components: {
            'virtual-list': virtualList,
            widthSetting
        },
        props: {
            messageList: {
                type: Array,
                default: []
            },
            showSound: {
                type: Boolean,
                default: false
            },
            theme: {
                type: Object,
                default: () => ({
                    value: 'dark'
                })
            }
        },
        data () {
            return {
                itemComponent: listItem,
                showDropdown: false,
                drawer: false,
                totalMessage: 0,
                dom: '',
            }
        },

        watch: {
            'messageList' (val) {
                this.totalMessage = val.length
            }
        },
        methods: {
            toShowDrawer () {
                this.drawer = true;
                this.dom = document.getElementsByClassName('message-el-drawer')[0]
                console.log(this.drawer, 'this.drawerthis.drawerthis.drawer')
            },
            contentItemClick (data, index) {
                window.top.snapshotData = JSON.parse(JSON.stringify(data))

                let iframe = document.getElementsByClassName("jumpIframe");
                for (let item of iframe) {
                    item.contentWindow.postMessage({ haveNewSnapshotData: true })
                }
                parent.vm.$router.push({ path: '/Index/jumpIframe/ganwei-iotcenter-system-snapshot/systemSnapshot' });
                this.messageList.splice(index, 1)
                this.drawer = false
            },
            handleClose () {
                this.drawer = false
            },
            toRouter (obj) {
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

    /deep/ .message-el-drawer {
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
                color: var(--con-textColor1);

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
                    color: var(--con-themeColor);

                    .iconfont {
                        font-size: 12px;
                        margin-left: 8px;
                        border-radius: 50%;
                        border: 1px solid;
                        @include border_color('notifytext');
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
            }

            .item-right {
                width: 130px;
                text-align: right;
                color: var(--notifytext);
            }
        }

        .noData {
            width: 100%;
            height: calc(100% - 51px);
            display: flex;
            justify-content: center;
            align-items: center;
            color: var(--con-textColor1);
        }
    }
</style>
