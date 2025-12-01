<template>
    <div class="playback">
        <!-- 播放器 -->
        <div class="player-container" v-loading="loading">
            <div :id="_id" class="glplayer" :class="{transparent: isPlay}"></div>
        </div>
        <div class="control-container">
            <videoControl ref="control" :isPlay :panelControl="panelControl" @closeHandler="destroyAll" @pauseHandler="pause" @resumeHandler="resume" @forwardHandler="speedPlay" @rewindHandler="speedPlay" @backwardHandler="speedPlay" @fullScreenHandler="context.player?.fullScreen()"></videoControl>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed, inject, onUnmounted, ref, watch } from 'vue'

import { AbstractPlayer, createPlayer, SupportPlayerEnum } from "../../classDefintion/Player"
import videoControl, { IPanelControl } from '../../videoControl/videoControl.vue'
import { asyncOperationKey } from '../js/useRecordPlayService';

export interface IPlayBackPlayerProps {
    id: string,
    url: string,
    change: number,
    panelControl?: Partial<IPanelControl>
}
const props = withDefaults(defineProps<IPlayBackPlayerProps>(), {
    panelControl: () => ({close: true, volume: true, panelControl: true, fullScreen: true, playControl: true})
})

const asyncOperation = inject(asyncOperationKey)
const emit = defineEmits<{
    performance: [number]
}>()

const _id = computed(() => {
    return `glplayer-${props.id}`
})

const control = ref()
const loading = ref(false) // 缓冲
const isPlay = ref(false) // 是否播放
const context: {
    player: AbstractPlayer | null,
} = {
    player: null,
}

watch(() => props.change, () => {
    context.player?.destroy()
    if(props.url) {
        play(props.url)
    }
})
function play(url: string) {
    loading.value = true;
    context.player = createPlayer(SupportPlayerEnum.H265WebPlayer, {
        player: _id.value,
        width: 860,
        height: 500
    })
    try {
        context.player.init(url);
        context.player.eventsOn(getEventListenerFunction())
        context.player.start();
        context.player.play();
    } catch (e) {
        console.log(e);
    }
}

function getEventListenerFunction(){
    return {
        onPlayState: (state: boolean) => {
            isPlay.value = state
        },
        onPlayFinish: ()=> {
            console.log('finished')
            destroyAll()
        },
        onPerformance: (time: number) => {
            loading.value = false  // 取消缓冲效果
            isPlay.value = true  // 修改为播放状态
            emit('performance', time)
        },
        onLoadFinish: () => {
            context.player?.setVoice(0)
            let mediaInfo = context.player?.instance.mediaInfo()
            let codecName = "h265";
            if (mediaInfo?.meta?.isHEVC === false) {
                console.log("\r\nonLoadFinish is Not HEVC/H.265");
                codecName = "h264";
            } else {
                console.log("\r\nonLoadFinish is HEVC/H.265");
            }
        }
    }
}

function pause() {
    asyncOperation?.recordPlayPause?.()
}

function resume() {
    asyncOperation?.recordPlayResume?.()
}

function speedPlay(speed:number) {
    asyncOperation?.recordSpeedPlay?.(speed)
}

function destroyAll(outer = false) {
    control.value?.reset()
    isPlay.value = false
    !outer && asyncOperation?.recordStopPlay?.()
    context.player?.destroy()
    context.player = null
}

onUnmounted(()=>{
    if(context.player) {
        destroyAll()
    }
})

defineExpose({
    getPlayer() {
        return context.player?.instance
    },
    destroy: () => {
        destroyAll(true)
    }
})
</script>

<style lang="scss" scoped>
.player-container {
    min-height: 500px;
    background: black;

    .glplayer {
        width: 100% !important;
    }
}

.control-container {
    position: absolute;
    height: 44px;
    bottom: 0;
    width: 100%;
    .bottom-container {
        display: none;
    }
}

.playback {
    &:hover {
        .control-container .bottom-container {
            display: block;
        }
    }
}
</style>
