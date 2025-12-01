<template>
    <el-dropdown v-bind="attrs" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-dropdown>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import {ElDropdown} from 'element-plus'
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)

import useDataTestId from '../../hooks/useDataTestId'
const attrs = useAttrs()
useDataTestId()
</script>

<style lang="scss">
.el-popper {
    border: 1px solid var(--frame-main-border);

    &.is-light .el-popper__arrow::before {
        background-color: var(--popper-arrow-background);
        border-color: var(--poper-border);
    }
}

.el-dropdown-menu {
    padding: 9px 0;
    background-color: var(--dropdown-menu-background);
    border-radius: 0;

    >p:hover {
        background: var(--dropdown-menu-background__hover);
    }
}
</style>
