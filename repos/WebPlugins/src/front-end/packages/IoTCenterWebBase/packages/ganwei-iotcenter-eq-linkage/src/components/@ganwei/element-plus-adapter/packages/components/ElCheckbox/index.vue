<template>
    <el-checkbox v-bind="attrs">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-checkbox>
</template>

<script setup lang="ts">
import { useAttrs } from 'vue'
import { ElCheckbox } from 'element-plus'
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

    .el-checkbox__input.is-disabled .el-checkbox__inner {
        background: var(--checkbox-inner-background);
        border-color: var(--checkbox-inner-border);
    }

    .el-checkbox__input.is-checked+.el-checkbox__label {
        color: var(--checkbox-label-color);
    }

    .el-checkbox__label {
        color: var(--checkbox-label-color);
    }
}
</style>
