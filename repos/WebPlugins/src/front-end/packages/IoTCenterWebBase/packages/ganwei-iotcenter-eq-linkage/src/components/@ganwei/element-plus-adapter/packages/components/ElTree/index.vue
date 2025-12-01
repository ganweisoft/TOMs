<template>
    <el-tree ref="tree">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-tree>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElTree } from 'element-plus'

import useExpose from '../../hooks/useExpose'
const tree = ref()
const expose = useExpose(tree)
defineExpose(expose)
</script>

<style lang="scss">
.el-tree {
    background-color: transparent;

    .el-tree-node:focus>.el-tree-node__content {
        background-color: var(--tree-node-background_hover);
    }

    .el-tree-node__content {
        position: relative;
        height: 36px;
        color: var(--frame-main-color);
        background-color: var(--tree-node-background_transparent);
        &:hover {
            background-color: var(--tree-node-background_hover);
            &::before {
                content: '';
                width: 4px;
                height: 100%;
                position: absolute;
                top: 0;
                left: 0;
                background: var(--tree-node-border__select);
            }
        }
    }
}
.el-tree--highlight-current .el-tree-node.is-current>.el-tree-node__content {
    background-color: var(--tree-node-background_hover);

    &::before {
        content: '';
        width: 4px;
        height: 100%;
        position: absolute;
        top: 0;
        left: 0;
        background: var(--tree-node-border__select);
    }
}
</style>
