<template>
     <el-popover ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-popover>
</template>

<script setup lang="ts">
import { ElPopover } from 'element-plus';
import useExpose from '../../hooks/useExpose'
import { ref } from 'vue';

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)
</script>

<style lang="scss">
.el-popper.el-popover {
    min-width: 50px;
    color: var(--frame-main-color);
    background: var(--popper-background);
    border-color: var(--poper-border);

    .el-popover__title {
        color: var(--frame-main-color);
    }

    .el-popper__arrow::before {
        background-color: var(--popper-background);
    }
}
</style>
