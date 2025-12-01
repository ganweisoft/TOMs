<template>
    <div class="playback-container">
        <div class="playback-player">
            <PlayBackPlayer @performance="onPerformance" ref="player" :id :url :change="_id"></PlayBackPlayer>
            <div class="replay-container"  v-if="isReplay && !loading">
                <i class="iconfont icon-_bofang replay" @click="recordReplay"></i>
                <span>重新播放</span>
            </div>
        </div>
        <TimeBar v-if="ranges.length" :ranges :date="date" @timeChange="doRandomDragDropPlay"></TimeBar>
        <videoPlayInfo :playInfo="playInfo" :isPlayInfo></videoPlayInfo>
    </div>
</template>

<script setup lang="ts">
import { onBeforeUnmount, provide, Ref, ref, watch } from 'vue'
import { dayjs } from 'element-plus';

import { useMessage } from '@ganwei-pc/gw-base-utils-plus/notification';

import { SuspenseTask } from '../../classDefintion/SuspenseTask';
import videoPlayInfo from '../../videoPlayInfo/videoPlayInfo.vue'
import { DefaultRecordPlayService, IRecordPlayTaskResponse } from '../js/AsyncRecordPlaySignalRController';
import { asyncOperationKey, RecordPlayServiceKey } from '../js/useRecordPlayService'
import { TimeRange } from './index';
import PlayBackPlayer from './player.vue'
import TimeBar from './timeBar3.vue'

const props = defineProps<{
    id: string,
    date: Date,
    deviceId: number,
    nvrChannelId: number
}>()
const $message = useMessage()
const player = ref()
const loading = defineModel('loading', {
    type: Boolean,
    default: false
})

let playDuration = 0
const playInfo = ref<string[]>([])
const isPlayInfo = ref(false)
let timeout = -1;
function ShowLog (res: IRecordPlayTaskResponse) {
    if(!res.response || res.playStatus == 90) {
        // 失败？
    }

    if((res.playStatus == 60) || res.playStatus == 100){
        // 播放？
    }
    // 记录总耗时
    if(playDuration < res.duration) {
        playDuration = res.duration
    }
    if(res.responseMessage && res.duration) {
        playInfo.value.push(res.responseMessage + "(" + res.duration + "ms)")
    }
}

const recordPlayService = new DefaultRecordPlayService(props.deviceId, props.nvrChannelId)
provide(RecordPlayServiceKey, recordPlayService)
provide(asyncOperationKey, {
    recordStopPlay,
    recordSpeedPlay,
    recordPlayResume,
    recordPlayPause
})
let streamId = ''
const ranges = ref<TimeRange[]>([]) as Ref<TimeRange[]>
const url = ref('')
const _id = ref(0)
const isReplay = ref(false)

watch(() => props.date, (day) => {
    startPlayback(day)
})

async function startPlayback(day: Date) {
    loading.value = true
    const startTime = dayjs(day).format('YYYY-MM-DD 00:00:00')
    const endTime = dayjs(day).format('YYYY-MM-DD 23:59:59')
    try {
        await recordStopPlay()
        let hasFile = await sendDeviceRecordsQuery(startTime, endTime)
        if(!hasFile) return;

        await recordPlay(startTime, endTime)
    } catch (e) {
        if(!SuspenseTask.isCanceled(e)) {
            $message.error(e)
        }
    } finally {
        loading.value = false
    }
}

async function sendDeviceRecordsQuery(startTime: string, endTime: string) {
    isReplay.value = false
    let res = await recordPlayService.doSendDeviceRecordsQuery({
        startTime,
        endTime
    })
    ranges.value = res.map(item => {
        return {
            startTime: dayjs(item.startTime).isBefore(startTime) ? startTime : item.startTime,
            endTime: dayjs(item.endTime).isAfter(endTime) ? endTime : item.endTime
        }
    })
    if(!res || res.length === 0) {
        $message.warning('该时间段内没有录像')
        return false
    }
    return true
}

async function recordPlay(startTime: string, endTime: string) {
    playInfo.value = []
    isPlayInfo.value = true
    let res = await recordPlayService.doRecordPlay({
        startTime: startTime,
        endTime: endTime
    }, ShowLog)
    _id.value += 1
    url.value = res.responseData.playInfos?.[0].url || ''

    streamId = res.responseData.streamId
}

async function doRandomDragDropPlay(nptTime: number) {
    try {
        player.value.getPlayer().player.myPlayer.currentTime = nptTime
    } catch(e) {
        console.error(e);
    }
    return recordPlayService.doRandomDragDropPlay({time: nptTime}).catch(reason => {
        !SuspenseTask.isCanceled(reason) && $message.error(reason)
    })
}

function onPerformance(time: number) {
    playInfo.value.push("播放器加载完成时间: " + time.toFixed(4) + "ms")
    playInfo.value.push("播放总耗时: " + (playDuration + time).toFixed(4) + "ms")
    if(timeout === -1) {
        timeout = window.setTimeout(() => {
            isPlayInfo.value = false;
            timeout = -1
        }, 5000)
    }
}

function recordStopPlay(_streamId?: string) {
    player.value?.destroy()
    ranges.value = []
    isPlayInfo.value = false
    isReplay.value = true
    if(timeout !== -1) {
        clearTimeout(timeout)
        timeout = -1
    }
    if(_streamId || streamId) {
        return recordPlayService.doRecordStopPlay({
            streamId: _streamId || streamId
        })
    }
    return Promise.resolve(true)
}

function recordReplay() {
    isReplay.value = false
    startPlayback(props.date)
}

function recordSpeedPlay(speed: number) {
    return recordPlayService.doRecordSpeedPlay({speed})
}

function recordPlayResume() {
    return recordPlayService.doRecordPlayResume()
}

function recordPlayPause() {
    return recordPlayService.doRecordPlayPause()
}

onBeforeUnmount(() => {
    recordStopPlay()
    recordPlayService.dispose()
})

defineExpose({
    sendDeviceRecordsQuery,
    recordPlay,
    recordStopPlay,
    startPlayback
})
</script>

<style lang="scss" scoped>
.playback-container {
    position: relative;
    margin-bottom: 20px;

    .playback-player {
        position: relative;

        .replay-container {
            position: absolute;
            top: 0;
            height: 100%;
            width: 100%;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            gap: 20px;
        }
        .replay {
            width: 60px;
            height: 60px;
            line-height: 60px;
            text-align: center;
            font-size: 40px;
            background: rgba(255, 255, 255, .1);
            cursor: pointer;
            border-radius: 50%;

            &:hover {
                transform: scale(1.2);
            }
        }
    }
    :deep() .video-info {
        bottom: 70px;
    }
}
</style>
