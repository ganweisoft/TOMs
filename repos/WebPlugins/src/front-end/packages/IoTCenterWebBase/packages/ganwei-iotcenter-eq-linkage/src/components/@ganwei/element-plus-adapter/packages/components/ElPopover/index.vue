<template>
     <el-popover>
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-popover>
</template>

<script setup lang="ts">
import { ElPopover } from 'element-plus';
</script>

<style lang="scss">
.el-popper.el-popover {
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