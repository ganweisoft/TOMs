<template>
    <el-checkbox v-bind="attrs" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-checkbox>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElCheckbox } from 'element-plus'
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)

const attrs = useAttrs()
</script>

<style lang="scss">
.el-checkbox {
    .el-checkbox__inner {
        background: var(--checkbox-inner-background);
        border-color: var(--checkbox-inner-border);
    }

    .el-checkbox__input.is-checked .el-checkbox__inner {
        background-color: var(--checkbox-inner-border__checked);
        border-color: var(--checkbox-inner-border__checked);
    }

    .el-checkbox__input.is-indeterminate .el-checkbox__inner {
        background-color: var(--checkbox-inner-border__checked);
        border-color: var(--checkbox-inner-border__checked);
    }

    .el-checkbox__input.is-disabled .el-checkbox__inner {
        background: var(--checkbox-inner-background);
        border-color: var(--checkbox-inner-border);
    }

    .el-checkbox__input.is-checked+.el-checkbox__label {
        color: var(--checkbox-label-color__checked);
    }

    .el-checkbox__label {
        color: var(--checkbox-label-color);
    }
}
</style>
