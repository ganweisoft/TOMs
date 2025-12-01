<template>
    <el-radio-group v-bind="attrs" :class="{round: round}" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-radio-group>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElRadioGroup } from 'element-plus'
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)

defineProps({
    round: {
        type: Boolean,
        default: false
    }
})

import useDataTestId from '../../hooks/useDataTestId'
const attrs = useAttrs()
useDataTestId()
</script>

<style lang="scss">
.el-radio-group {
    .el-radio .el-radio__label{
        color: var(--frame-main-color);
    }
}

.el-radio-group.round {
    height: 40px;
    margin: 0 10px;
    padding: 3px;

    border: 1px solid var(--dropdown-menu-background__hover);
    border-radius: 17px;

    .el-radio-button {
        &:last-child .el-radio-button__inner,
        &:first-child .el-radio-button__inner,
        .el-radio-button__inner {
            color: var(--frame-main-color);
            background-color: transparent;
            border: none;
            border-radius: 19px;
        }

        &:not(:first-child)::before {
            content: " ";

            position: absolute;
            top: 50%;
            left: 0;
            transform: translate(0, -50%);

            display: block;

            width: 1px;
            height: 20px;

            background-color: #595959;
        }

        &.is-active .el-radio-button__inner {
            background-color: var(--tab-background__active);
        }

        &.is-active::before,
        &.is-active+.el-radio-button::before {
            background-color: transparent;
        }

        &:hover {
            color: var(--tab-background__active);
        }
    }
}
</style>
