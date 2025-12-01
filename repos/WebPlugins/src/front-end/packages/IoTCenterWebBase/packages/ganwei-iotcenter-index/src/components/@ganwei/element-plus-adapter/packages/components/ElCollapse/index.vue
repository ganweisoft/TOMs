<template>
    <el-collapse ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-collapse>
</template>

<script setup lang="ts">
import { ElCollapse } from 'element-plus'
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)
</script>

<style lang="scss">
.el-collapse {
    border: 0;

    .el-collapse-item {
        .el-collapse-item__header {
            margin-bottom: 10px;
            color: var(--collapse-color);
            background-color: var(--collapse-header-background);
            border-bottom: 0;
        }

        .el-collapse-item__wrap {
            background-color: transparent;
            border-bottom: 0;

            .el-collapse-item__content {
                color: var(--collapse-color);
            }
        }
    }
}
</style>