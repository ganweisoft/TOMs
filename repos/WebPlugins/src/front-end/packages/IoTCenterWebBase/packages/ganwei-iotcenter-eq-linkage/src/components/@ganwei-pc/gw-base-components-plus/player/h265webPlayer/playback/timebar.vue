<template>
    <div class="playback-timebar" ref="containerRef">
        <div class="range" ref="rangeRef">
            <div class="mask" ref="selectMask"></div>
            <div class="enable-range" v-for="r in _ranges" :style="{left: r.left + '%', width: r.width + '%'}">
            </div>
        </div>
        <div class="top-handle" ref="topRef"></div>
        <div class="left-handle handle" ref="dragLeftRef"></div>
        <div class="right-handle handle" ref="dragRightRef"></div>
    </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import dayjs from 'dayjs'

import { useMessage } from '@components/@ganwei-pc/gw-base-utils-plus/notification';

import { SuspenseTask } from '../../classDefintion/SuspenseTask';
import { useDraggable, useSelectDraggable } from '../js/useDraggable';
import useRecordPlayService from '../js/useRecordPlayService';
import { TimeRange } from './index'

export interface ITimeBarProps {
    ranges: TimeRange[],
    date: Date
}
const recordPlayService = useRecordPlayService()!
const $message = useMessage()

const props = defineProps<ITimeBarProps>()

const _ranges = computed(() => {
    return mergeRange(props.ranges || [])
})

const containerRef = ref<HTMLElement>()
const dragLeftRef = ref<HTMLElement>()
const dragRightRef = ref<HTMLElement>()
const topRef = ref<HTMLElement>()
const _draggable = computed(() => {
    return true
})

const { setRightHandleOffsetX, setLeftHandleOffsetX } = useDraggable(containerRef, dragLeftRef, dragRightRef, topRef, _draggable, onSelectChange)
watch(_ranges, (val) => {
    const r = val[0]
    if(r) {
        setLeftHandleOffsetX(r.left, '%')
        setRightHandleOffsetX(100 - (r.left + r.width), '%')
    }
})
onMounted(() => {
    const r = _ranges.value[0]
    if(r) {
        setLeftHandleOffsetX(r.left, '%')
        setRightHandleOffsetX(100 - (r.left + r.width), '%')
    }
})

const selectMask = ref<HTMLElement>()
const rangeRef = ref<HTMLElement>()

useSelectDraggable(rangeRef, selectMask, onSelectChange)

function onSelectChange(context:{
    leftPercent: number
    widthPercent: number
}) {
    const fixPosition = getTimeRangeFromPosition(context.leftPercent, context.widthPercent)

    if(fixPosition) {
        const total = 24 * 60 * 60
        const nptTime = Math.floor((fixPosition.left - _ranges.value[0].left) * total / 100)

        recordPlayService.doRandomDragDropPlay({time: nptTime}).catch(reason => {
            !SuspenseTask.isCanceled(reason) && $message.error(reason)
        })
        setLeftHandleOffsetX(fixPosition.left, '%')
        setRightHandleOffsetX(100 - fixPosition.left - fixPosition.width, '%')
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
    const total = 24 * 60 * 60

    return _r.map(r => {
        const startTime = dayjs(r.startTime).isBefore(StartOfDay) ? StartOfDay : dayjs(r.startTime)
        const endTime = dayjs(r.endTime).isAfter(EndOfDay) ? EndOfDay : dayjs(r.endTime)
        const duration = endTime.diff(startTime, 'second')
        return {
            startTime,
            endTime,
            duration,
            left: Math.max(0, startTime.diff(StartOfDay, 'second')) / total * 100,
            width: Math.min(100, duration / total * 100)
        }
    })
}

function getTimeRangeFromPosition(left: number, width: number) {
    const _r = _ranges.value.find(i => {
        if(i.left + i.width < left || i.left > left + width) {
            return false
        }
        return i
    })
    if(_r) {
        // 包含时间段
        // |------|
        //   |--| _r
        if(_r.left >= left && _r.left + _r.width <= left + width) {
            return {
                left: _r.left,
                width: _r.width
            }
        }
        // 被时间段包含
        //   |--|
        // |------| _r
        if(_r.left <= left && _r.left + _r.width >= left + width) {
            return {
                left,
                width
            }
        }
        // 选中时间段的左边
        // |------|
        //   |------| _r
        if(_r.left >= left && _r.left <= left + width && _r.left + _r.width >= left + width) {
            return {
                left: _r.left,
                width: left + width - _r.left
            }
        }
        // 选中时间段的右边
        // |------| _r
        //   |------|
        if(_r.left <= left && _r.left + _r.width >= left && _r.left + _r.width <= left + width) {
            return {
                left,
                width: _r.left + _r.width - left
            }
        }
    }
}

</script>

<style lang="scss" scoped>
.playback-timebar {
    height: 40px;
    position: relative;
    padding-top: 10px;
    user-select: none;
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

        .enable-range {
            position: absolute;
            left: 0%;
            height: 20px;
            height: 10px;
            bottom: 0;
            background-color: #3B436F;
        }
    }

    .handle {
        position: absolute;
        top: 10px;
        border-left: 1px solid #a1a8c2;
        height: 30px;
        cursor: e-resize;
    }

    .left-handle {
        left: 0;
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

    .right-handle {
        right: 0;
        &::after {
            content: "";
            position: absolute;
            right: -2px;
            top: 5px;
            display: inline-block;
            width: 4px;
            height: 20px;
            background: #353450;
            border: 1px solid #a1a8c2;
        }
    }

    .top-handle {
        height: 5px;
        background-color: #403f58;
        position: absolute;
        left: 0;
        right: 0;
        top: 5px;
        cursor: move;
    }

}

</style>
