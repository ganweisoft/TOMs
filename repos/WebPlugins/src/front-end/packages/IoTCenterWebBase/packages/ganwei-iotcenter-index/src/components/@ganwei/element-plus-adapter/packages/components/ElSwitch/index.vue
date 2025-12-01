<template>
    <el-switch v-bind="attrs" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-switch>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElSwitch } from 'element-plus';

import useDataTestId from '../../hooks/useDataTestId'
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)
const attrs = useAttrs()
useDataTestId()
</script>

<style lang="scss">
.el-switch {
    .el-switch__core {
        width: 56px;
        height: 26px;

        background-color: var(--switch-core-background);
        border: 1px solid;
        border-color: var(--switch-core-border);
        border-radius: 13px;

        .el-switch__action {
            width: 16px;
            height: 16px;
            background-color: var(--switch-core-after-background) !important;
        }
    }

    .el-switch__label {
        color: transparent;

        &--left {
            transform: translateX(55px);
            z-index: 2;
            &.is-active {
                color: var(--switch-label-color);
            }
        }

        &--right {
            transform: translateX(-55px);

            &.is-active {
                color: var(--switch-label-color);
            }
        }
    }

    &.is-checked  {
        .el-switch__core {
            background-color: var(--switch-core-border__active);
            border-color: var(--switch-core-background__active);
        }
    }

}
</style>
