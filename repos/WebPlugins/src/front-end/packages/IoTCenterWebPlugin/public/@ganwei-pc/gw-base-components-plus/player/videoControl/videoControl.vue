<template>
    <div class="bottom-container">
        <div class="video-bottom" v-if="isPlay || isPaused && panelControl?.panelControl">
            <div class="left">

            </div>
            <div class="center">
                <!-- <span v-if="panelControl.playControl" class="backward">
                    <i title="倒放" class="iconfont icon-bofangsanjiaoxing" @click="backwardHandler"></i>
                </span> -->
                <span v-if="panelControl.playControl">
                    <i title="慢放" class="iconfont icon-kuaitui" @click="rewindHandler"></i>
                </span>
                <span class="play-btn" v-if="panelControl.playControl">
                    <i title="暂停" class="iconfont icon-zantingtingzhi" v-if="!isPaused" @click="pauseHandler"></i>
                    <i title="播放" class="iconfont icon-bofangsanjiaoxing" v-else @click="resumeHandler"></i>
                </span>
                <span v-if="panelControl.playControl">
                    <i title="快进" class="iconfont icon-kuaijin" @click="forwardHandler"></i>
                </span>
            </div>
            <div class="right">
                <el-select v-if="panelControl.playControl" style="width: 100px;" title="播放倍率" v-model="speed" @change="playSpeedChange">
                    <el-option label="0.25x" :value="0.25"></el-option>
                    <el-option label="0.5x" :value="0.5"></el-option>
                    <el-option label="1x" :value="1"></el-option>
                    <el-option label="2x" :value="2"></el-option>
                    <el-option label="4x" :value="4"></el-option>
                </el-select>
                <span class="play-PTZ" v-if="panelControl?.ptzControl">
                    <i class="iconfont icon-_jiankongqiuji" @click="ptzControlVisible = !ptzControlVisible" title="云台控制"></i>
                </span>
                <!-- 录像 -->
                <span class="play-screenrecording" v-if="panelControl?.record">
                    <i class="iconfont icon-_shishikuaizhao_daixuan" @click="screenHandler" title="录像"></i>
                </span>
                <!-- 截图 -->
                <span class="play-screenshot" v-if="panelControl?.capture">
                    <i class="iconfont icon-gw-icon-tab-tupian" @click="snapHandler" title="截图"></i>
                </span>
                <!-- 音量 -->
                <span class="volume-btn" v-if="panelControl?.volume">
                    <i class="iconfont icon-jingyin" @click="volumeHandler" title="音量" v-if="volumeValue === 0"></i>
                    <i class="iconfont icon-shengyin1" @click="volumeHandler" title="音量" v-else></i>
                </span>
                <!-- 关闭 -->
                <span class="close-btn" v-if="panelControl?.close">
                    <el-icon @click="closeHandler" title="关闭">
                        <SwitchButton />
                    </el-icon>
                </span>
                <!-- 全屏 -->
                <span class="screen-btn" v-if="panelControl?.fullScreen">
                    <i class="iconfont icon-quanping" @click="screenHandler" title="全屏"></i>
                </span>
            </div>
        </div>

        <div class="flow-control">
            <PTZControl v-model="ptzControlVisible" round @ptzControl="ptzControlHandler"></PTZControl>
        </div>
    </div>

</template>
<script setup lang="ts">
// 基础
import { defineEmits, defineProps, ref } from 'vue'

import { PTZCommandEnum } from '../h265webPlayer/Models';
import PTZControl from './PTZControl/index.vue'

export interface IVideoBottomProps {
    isPlay:boolean,
    panelControl: Partial<IPanelControl>,
}

export interface IPanelControl{
    panelControl: boolean, // 控制条
    fullScreen: boolean, // 全屏
    play: boolean, // 播放
    pause: boolean, // 暂停
    stop: boolean, // 停止
    record: boolean, // 录像
    playback: boolean, // 回放
    speed: boolean, // 倍数
    time: boolean, // 时间
    volume: boolean, // 音量
    setting: boolean, // 设置
    share: boolean, // 分享
    more: boolean, // 更多
    close: boolean, // 关闭
    capture: boolean, // 截图
    title: boolean, // 标题
    beforePlayInfo: boolean, // 播放前信息
    streamList: boolean, // 流列表
    ptzControl: boolean,
    playControl: boolean
}

const props = withDefaults(defineProps<IVideoBottomProps>(), {
    isPlay: false,
    panelControl: () => ({}),
})

const speed = ref(1)

const isPaused = ref(false)
const ptzControlVisible = ref(false)

const emit = defineEmits(['ptzControlHandler', 'pauseHandler', 'resumeHandler', 'forwardHandler', 'backwardHandler', 'rewindHandler', 'fullScreenHandler', 'snapshotHandler', 'closeHandler', 'volumeHandler'])

function pauseHandler() {
    isPaused.value = true
    emit('pauseHandler');
}

function resumeHandler() {
    isPaused.value = false
    emit('resumeHandler');
}

function forwardHandler() {
    speed.value = Math.min(4, speed.value * 2)
    emit('forwardHandler', speed.value);
}

function rewindHandler() {
    speed.value = Math.max(0.25, speed.value / 2)
    emit('rewindHandler', speed.value);
}

function backwardHandler() {
    emit('backwardHandler', -1);
}

function playSpeedChange(val: number) {
    emit('rewindHandler', val);
}

// 音量
const volumeValue = ref(0)
function volumeHandler() {
    volumeValue.value = 1 - volumeValue.value
    emit('volumeHandler', volumeValue.value);
}

function ptzControlHandler(cmd: PTZCommandEnum, args: object) {
    emit('ptzControlHandler', cmd, args)
}

// 全屏
function screenHandler() {
    emit('fullScreenHandler');
}

// 关闭
function closeHandler() {
    emit('closeHandler');
}

// 截图
function snapHandler() {
    emit('snapshotHandler');
}

function reset() {
    isPaused.value = false
    ptzControlVisible.value = false
    volumeValue.value = 0
    speed.value = 1
}

defineExpose({
    reset
})
</script>

<style lang="scss" scoped>
.hide {
    display: none !important;
}
.bottom-container {
    position: absolute;
    bottom: 0;
    left: 0;
    width: 100%;
    height: 100%;
}
.video-bottom {
    position: absolute;
    bottom: 0;
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
    height: 44px;
    padding: 0 20px;
    background: rgba(0, 0, 0, 0.3);

    .left, .right, .center {
        display: flex;
        align-items: center;
        gap: 20px;
        flex: 1;
    }

    .center {
        justify-content: center;
    }

    .right {
        justify-content: flex-end;
    }

    span {
        display: inline-block;
        opacity: 0.7;

        i {
            font-size: 20px;
            display: inline-block;
            transition: all .3s ease-in-out;
            cursor: pointer;
        }

        i:hover {
            opacity: 1;
            transform: scale(1.2);
        }

        &.backward {
            transform: rotate(180deg) scale(0.8);
        }
    }
}

.flow-control {
    height: calc(100% - 80px - 44px);
    width: 100%;
    position: absolute;
    top: 80px;
}
</style>
