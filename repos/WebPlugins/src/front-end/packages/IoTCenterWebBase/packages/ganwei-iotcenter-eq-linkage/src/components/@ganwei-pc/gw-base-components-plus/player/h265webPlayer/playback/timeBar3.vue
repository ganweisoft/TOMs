<template>
    <div class="playback-timebar">
        <div class="range" ref="containerRef">
            <div class="range-container">
                <div class="range-layout" ref="rangeRef">
                    <div class="time-tick" v-for="(i, index) in ticks" :key="i" :style="{left: index / hours * scale * 100 + '%'}">
                        {{ i }}
                    </div>
                    <div class="enable-range" v-for="r in _ranges" :key="r.left" :style="{left: r.left + '%', width: r.width + '%'}">
                    </div>
                </div>
            </div>
        </div>
        <div class="left-handle handle" ref="dragRef">
            <span class="time">{{ time }}</span>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import dayjs from 'dayjs'

import { useMessage } from '@components/@ganwei-pc/gw-base-utils-plus/notification';

import { useHandleDraggable } from '../js/useDraggable';
import { TimeRange } from './index'

export interface ITimeBarProps {
    ranges: TimeRange[],
    date: Date
}

const $message = useMessage()
const props = defineProps<ITimeBarProps>()
const emits = defineEmits<{
    timeChange: [nptTime: number]
}>()

const _ranges = computed(() => {
    resetPosition()
    return mergeRange(props.ranges || [])
})

const rangeRef = ref<HTMLElement>()
const containerRef = ref<HTMLElement>()
const time = ref('')
const scale = 2
const hours = 24
const total = hours * 60 * 60
const ticks = Array.from({ length: hours + 1 }, (_, index) => {
    return index >= 10 ? `${index}:00` : `0${index}:00`
})
const { resetPosition, setOffset } = useHandleDraggable(containerRef, rangeRef, scale, onSelectChange)

function onSelectChange(context:{
    left: number,
    leftPercent: number
}, type: string) {
    const fixPosition = getTimeRangeFromPosition(context.leftPercent)
    if(fixPosition) {
        // 长度被scale放大，计算时间需要缩小回来
        const nptTime = Math.floor((context.leftPercent - _ranges.value[0].left) * total / scale / 100)
        const StartOfDay = _ranges.value[0].startTime
        time.value = StartOfDay.add(nptTime, 'second').format('YYYY-MM-DD HH:mm:ss')
        if(type === 'up') {
            emits('timeChange', nptTime)
        }
    } else {
        $message.closeAll()
        $message.warning('播放时间超出范围')
    }
}

function mergeRange(range: TimeRange[]) {
    const _r = range.reduce((acc, cur) => {
        if (acc.length === 0) {
            acc.push(JSON.parse(JSON.stringify(cur)));
            return acc
        }
        const last = acc[acc.length - 1]
        if (last.endTime === cur.startTime) {
            last.endTime = cur.endTime
        } else {
            acc.push(JSON.parse(JSON.stringify(cur)))
        }
        return acc
    }, [] as TimeRange[])

    const StartOfDay = dayjs(props.date).startOf('day')
    const EndOfDay = dayjs(props.date).endOf('day')

    const r = _r.map(r => {
        const startTime = dayjs(r.startTime).isBefore(StartOfDay) ? StartOfDay : dayjs(r.startTime)
        const endTime = dayjs(r.endTime).isAfter(EndOfDay) ? EndOfDay : dayjs(r.endTime)
        const duration = endTime.diff(startTime, 'second')
        return {
            startTime,
            endTime,
            duration,
            left: Math.max(0, startTime.diff(StartOfDay, 'second')) / total * scale * 100,
            width: Math.min(100 * scale, duration / total * scale * 100)
        }
    })
    time.value = r[0].startTime.format('YYYY-MM-DD HH:mm:ss')
    setOffset(r[0].left, '%')
    return r
}

function getTimeRangeFromPosition(left: number) {
    return _ranges.value.find(i => {
        if(left >= i.left && left - (i.left + i.width) <= 0.01){
            return i
        }
    })
}

</script>

<style lang="scss" scoped>
.playback-timebar {
    height: 50px;
    margin: 10px 0;
    position: relative;
    padding-top: 20px;
    user-select: none;
    overflow: hidden;
    .range {
        height: 30px;
        width: 100%;
        background: var(--frame-main-background-3);
        cursor: crosshair;
        position: relative;

        .mask {
            position: absolute;
            display: inline-block;
            height: 100%;
            width: 0;
            background-color:violet;
        }

        .range-container, .range-layout {
            width: 100%;
            height: 100%;
        }
        .range-container {
            transform: translate(50%);
            cursor: e-resize;
        }

        .enable-range {
            position: absolute;
            left: 0%;
            height: 10px;
            bottom: 0;
            background-color: #3B436F;
        }

        .time-tick {
            position: absolute;
            left: 0;
            transform: translateX(-50%);

            &::after {
                content: "";
                position: absolute;
                left: 50%;
                top: 100%;
                height: 100%;
                border: 1px solid var(--frame-main-border);
            }
        }
    }

    .handle {
        position: absolute;
        top: 20px;
        border-left: 1px solid #a1a8c2;
        height: 30px;

        .time {
            position: absolute;
            left: 0;
            top: -12px;
            width: 150px;
            transform: translate(-50%, -50%);
        }
    }

    .left-handle {
        left: 50%;
        &::after {
            content: "";
            position: absolute;
            left: -3px;
            top: 5px;
            display: inline-block;
            width: 4px;
            height: 20px;
            background: #353450;
            border: 1px solid #a1a8c2;
        }
    }

}

</style>
