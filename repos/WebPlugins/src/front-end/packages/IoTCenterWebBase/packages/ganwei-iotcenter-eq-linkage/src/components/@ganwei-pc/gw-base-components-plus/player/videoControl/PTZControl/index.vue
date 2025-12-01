<template>
    <div class="PTZ-operator" v-if="visible" ref="container">
        <div class="PTZ-operator-header" ref="drag">
            <span class="close-btn">
                <i class="iconfont icon-tubiao24_guanbi" @click="visible = false"></i>
            </span>
        </div>
        <div class="PTZ-operator-content">
            <div class="PTZ-panel " :class="{round: round}">
                <div class="direction ptz-left-up" title="左上">
                    <i class="iconfont icon-_fangxiangjian" @mousedown="ptzControlHandler(PTZCommandEnum.LeftUp)"></i>
                </div>
                <div class="direction ptz-up" title="上">
                    <i class="iconfont icon-_fangxiangjian" @mousedown="ptzControlHandler(PTZCommandEnum.Up)"></i>
                </div>
                <div class="direction ptz-right-up" title="右上">
                    <i class="iconfont icon-_fangxiangjian"  @mousedown="ptzControlHandler(PTZCommandEnum.UpRight)"></i>
                </div>
                <div class="direction ptz-left" title="左">
                    <i class="iconfont icon-_fangxiangjian" @mousedown="ptzControlHandler(PTZCommandEnum.Left)"></i>
                </div>
                <div class="direction ptz-auto">
                    <!-- <i class="iconfont icon-16_zhongzhi"></i> -->
                </div>
                <div class="direction ptz-right" title="右">
                    <i class="iconfont icon-_fangxiangjian" @mousedown="ptzControlHandler(PTZCommandEnum.Right)"></i>
                </div>
                <div class="direction  ptz-left-down" title="左下">
                    <i class="iconfont icon-_fangxiangjian" @mousedown="ptzControlHandler(PTZCommandEnum.DownLeft)"></i>
                </div>
                <div class="direction  ptz-down" title="下">
                    <i class="iconfont icon-_fangxiangjian" @mousedown="ptzControlHandler(PTZCommandEnum.Down)"></i>
                </div>
                <div class="direction ptz-right-down" title="右下">
                    <i class="iconfont icon-_fangxiangjian" @mousedown="ptzControlHandler(PTZCommandEnum.RightDown)"></i>
                </div>
            </div>
            <div class="ptz-slider">
                <el-row>
                    <el-col :span="6">
                        <span class="label">速度：</span>
                    </el-col>
                    <el-col :span="18">
                        <el-slider v-model="step" :min="1" :max="254" show-input size="small"></el-slider>
                    </el-col>
                </el-row>
            </div>
            <div class="zoom">
                <div class="zoomIn zoomButton" title="变焦放大">
                    <i class="iconfont icon-_gongnengjian_fangda" @mousedown="ptzControlHandler(PTZCommandEnum.ZoomIn)"></i>
                </div>

                <div class="zoomOut zoomButton" title="变焦缩小">
                    <i class="iconfont icon-_gongnengjian_suoxiao" @mousedown="ptzControlHandler(PTZCommandEnum.ZoomOut)"></i>
                </div>
            </div>
            <div class="preset-list">
                <div class="preset-item" :class="{'is-disabled': !item.enable}" v-for="(item) in presetList" :key="item.presetId">
                    <span class="index">{{ item.presetId }}</span>
                    <span class="name">
                        {{ item.presetName }}
                    </span>
                    <span class="operation">
                        <el-icon title="调用" v-if="item.enable" @click="ptzControlHandler(PTZCommandEnum.GetPreset, { speed: item.presetId})"><Location /></el-icon>
                        <el-icon title="设置" @click="ptzControlHandler(PTZCommandEnum.SetPreset, { speed: item.presetId})"><Setting /></el-icon>
                        <el-icon title="删除" v-if="item.enable" class="danger" @click="ptzControlHandler(PTZCommandEnum.RemovePreset, { speed: item.presetId})"><Delete /></el-icon>
                    </span>
                </div>
            </div>
        </div>
    </div>

</template>

<script setup lang="ts">
import { onBeforeUnmount, ref, computed } from 'vue';

import { useDraggable } from 'element-plus'

import { useMessage } from "@components/@ganwei-pc/gw-base-utils-plus/notification";

import { IPresetData } from '../../classDefintion/DefaultPTZControlService';
import usePlayerContext from '../../h265webPlayer/js/usePlayerContext';
import { PTZCommandEnum, PTZControlTypeEnum } from '../../h265webPlayer/Models/index';

export interface IDisplayPresetData extends IPresetData {
    enable: boolean
}

const props = withDefaults(defineProps<{
    round: boolean,
    maxPresetCount: number,
}>(), {
    round: false,
    maxPresetCount: 255
})
const visible = defineModel<boolean>({
    default: true
})
const step = ref(60)
const presetList = ref<IDisplayPresetData[]>([])

const $message = useMessage()
const playerContext = usePlayerContext()
if(playerContext && playerContext.deviceId && playerContext.ptzControl) {
    playerContext.player?.enablePTZ(playerContext.deviceId, playerContext.nvrChannelId, PTZControlTypeEnum.GB28181)
    playerContext?.player?.doPresetQuery().then(res => {
        let data = res.rows || []
        presetList.value = Array.from({length: props.maxPresetCount}, (_, index) => {
            const id = index + 1
            let findItem = data.find(item => item.presetId === id)
            if(findItem) {
                return {
                    ...findItem,
                    enable: true
                }
            }
            return {
                presetId: id,
                presetName: '预置点' + id,
                enable: false
            }
        })
    })
}

const container = ref()
const drag = ref()
useDraggable(container, drag, computed(()=>true))

let isPTZControlClick = false

function ptzControlHandler(cmd: number, arg: Record<string, any> = {}) {
    if(cmd >= PTZCommandEnum.StopPTZ && cmd <= PTZCommandEnum.ZoomOut) {
        isPTZControlClick = true
    }
    playerContext?.player?.ptzControl(cmd, { speed: step.value, ...arg}).then(res => {
        $message.success('命令下发成功，请稍等')
        if(cmd === PTZCommandEnum.SetPreset) {
            presetList.value.find(item => item.presetId === arg.speed)!.enable = true
        }
        if(cmd === PTZCommandEnum.RemovePreset) {
            presetList.value.find(item => item.presetId === arg.speed)!.enable = false
        }
    }).catch(err => {
        $message.error(err)
    })
}

function StopPTZControl() {
    if(isPTZControlClick) {
        playerContext?.player?.ptzControl(PTZCommandEnum.StopPTZ, { speed: 0})
        isPTZControlClick = false
    }
}

window.addEventListener('mouseup', StopPTZControl)
onBeforeUnmount(() => {
    window.removeEventListener('mouseup', StopPTZControl)
})
</script>

<style lang="scss" scoped>
.PTZ-operator {
    --button-size: 40px;
    --size: calc((var(--button-size) + 4px) * 3);
    --offset: calc(var(--button-size) * 1.5 - var(--size) / (2 * 1.414) );
    --n-offset: calc(-1 * var(--offset));
    width: 300px;
    min-height: 220px;
    display: flex;
    flex-direction: column;
    align-items: center;
    position: absolute;
    bottom: 10%;
    background: var(--frame-main-background-2);
    border-radius: 6px;
}

.PTZ-operator-header {
    height: 40px;
    width: 100%;
    .close-btn {
        position: absolute;
        top: 15px;
        right: 15px;
    }
}
.PTZ-operator-content {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
    padding: 0 20px;
}
.PTZ-panel {
    width: var(--size);
    height: var(--size);
    margin-bottom: 20px;
    background: var(--frame-main-background);
    overflow: hidden;

    .direction {
        width: var(--button-size);
        height: var(--button-size);
        margin: 0 4px 4px 0;
        float: left;
        display: flex;
        align-items: center;
        justify-content: center;

        &:active {
            color: var(--gw-color-primary);
        }

        i {
            font-size: calc(var(--button-size) / 2);
        }

        &:not(.round) {
            background: var(--frame-main-background);
        }
    }

    .ptz-left-up i {
        transform: rotate(-45deg);
    }

    .ptz-right-up i {
        transform: rotate(45deg);
    }

    .ptz-left i {
        transform: rotate(-90deg);
    }

    .ptz-auto i {
    }

    .ptz-right i {
        transform: rotate(90deg);
    }

    .ptz-left-down i {
        transform: rotate(-135deg);
    }

    .ptz-down i {
        transform: rotate(180deg);
    }

    .ptz-right-down i {
        transform: rotate(135deg);
    }

    &.round {
        border-radius: 50%;
        border: 10px solid var(--frame-main-background-3);
        box-sizing: content-box;
        i {
            transform: none;
        }
        .ptz-left-up {
            transform: translate(var(--offset), var(--offset)) rotate(-45deg);
        }

        .ptz-right-up {
            transform: translate(var(--n-offset), var(--offset)) rotate(45deg);
        }

        .ptz-left {
            transform: rotate(-90deg);
        }

        .ptz-right {
            transform: rotate(90deg);
        }

        .ptz-left-down {
            transform: translate(var(--offset), var(--n-offset)) rotate(-135deg);
        }

        .ptz-down {
            transform: rotate(180deg);
        }

        .ptz-right-down {
            transform: translate(var(--n-offset), var(--n-offset)) rotate(135deg);
        }
    }
}
.ptz-slider {
    padding-left: 20px;
    width: 100%;
    margin-bottom: 20px;

    .label {
        font-size: 12px;
    }
    :deep(.el-slider) {
        .el-slider__runway.show-input {
            margin-right: 20px;
        }
        .el-slider__input {
            width: 40px;
        }
        .el-input-number .el-input-number__decrease, .el-input-number .el-input-number__increase {
            display: none;
        }
    }
}
.zoom {
    margin-bottom: 20px;
    .zoomButton {
        display: inline-block;
        width: calc(var(--button-size) + 4px);
        height: var(--button-size);
        line-height: var(--button-size);
        text-align: center;
        border: 1px solid var(--frame-main-border);

        &:active {
            color: var(--gw-color-primary);
        }
    }

    .zoomIn {
        border-right: 1px dashed var(--frame-main-border);
        border-radius: 50% 0 0 50%;
    }

    .zoomOut {
        border-left: none;
        border-radius: 0 50% 50% 0;
    }
}
.preset-list {
    max-height: 250px;
    overflow-y: auto;
    width: 100%;

    .preset-item {
        font-size: 14px;
        line-height: 40px;
        height: 38px;
        margin-bottom: 8px;
        padding: 0;
        display: flex;
        justify-content: space-between;
        align-items: center;
        border-radius: 3px;
        background-color: var(--tree-node-background);
        border: 1px solid var(--tree-node-border);
        color: var(--tree-node-color);

        .index {
            flex-shrink: 0;
            width: 36px;
            height: 36px;
            text-align: center;
            background-color: var(--tree-list-index-background);
        }

        .name {
            width: calc(100% - 38px);
            height: 38px;
            flex: 1;
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 0 10px;
            font-size: 12px;
        }

        .operation {
            display: none;
            gap: 5px;
            padding-right: 10px;
            color: var(--gw-color-primary);
        }

        &.is-disabled {
            color: var(--frame-main-color-1);
        }

        &:hover {
            .operation {
                display: flex;

                .danger {
                    color: var(--gw-color-danger);
                }
            }
        }
    }
}
</style>
