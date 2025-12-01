<template>
    <el-radio v-bind="attrs" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-radio>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElRadio } from 'element-plus'
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)
const attrs = useAttrs()
</script>

<style lang="scss">
.el-radio {
    .el-radio__inner {
        background-color: var(--radio-inner-background);
        border-color: var(--radio-inner-border);

        &::after {
            width: 8px;
            height: 8px;
            background-color: var(--radio-inner-after);
        }
    }

    .el-radio__input.is-checked .el-radio__inner {
        background: var(--radio-inner-border__checked);
        border-color: var(--radio-inner-border__checked);
    }

    .el-radio__input.is-disabled .el-radio__inner {
        background-color: var(--radio-inner-background);
        border-color: var(--radio-inner-border);
    }
}
</style>
