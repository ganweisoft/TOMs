<template>
    <el-cascader v-bind="attrs">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-cascader>
</template>

<script setup lang="ts">
import { useAttrs } from 'vue'
import { ElCascader } from 'element-plus'
const attrs = useAttrs()
</script>

<style lang="scss">
.el-cascader {
    .el-cascader__tags .el-tag {
        color: var(--select-tag-color);
        background: var(--select-tag-background);
    }
}

.el-popper.el-cascader__dropdown {
    background: var(--select-dropdown-background);
    border: 1px solid var(--select-dropdown-border);

    .el-popper__arrow::before {
        border-color: var(--select-dropdown-border);
    }

    .el-cascader-menu {
        border: 1px solid var(--select-dropdown-border);
    }

    .el-cascader-node {
        color: var(--select-dropdown-color);

        &.is-hovering,
        &:hover{
            color: var(--select-dropdown-color__selected);
            background-color: var(--select-dropdown-item-background__hover);
        }

        &.is-active, &.in-active-path {
            color: var(--select-dropdown-color__selected);
        }
    }

    .el-cascader-node:not(.is-disabled):focus, .el-cascader-node:not(.is-disabled):hover {
        color: var(--select-dropdown-color__selected);
        background-color: var(--select-dropdown-item-background__hover);
    }
}
</style>
