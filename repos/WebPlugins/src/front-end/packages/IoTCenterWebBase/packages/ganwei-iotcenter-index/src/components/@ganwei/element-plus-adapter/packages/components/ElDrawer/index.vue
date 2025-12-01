<template>
    <el-drawer v-bind="attrs" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-drawer>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElDrawer } from 'element-plus';
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)

import useDataTestId from '../../hooks/useDataTestId'
const attrs = useAttrs()
useDataTestId()
</script>

<style lang="scss">
.el-drawer {
    background-color: var(--drawer-background);

    .el-drawer__header {
        color: var(--drawer-color);
        border-color: var(--drawer-border);
    }
    
}
</style>
