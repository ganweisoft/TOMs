<template>
    <el-select v-bind="attrs" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-select>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElSelect } from 'element-plus';

import useDataTestId from '../../hooks/useDataTestId'
import useExpose from '../../hooks/useExpose'
const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)
const attrs = useAttrs()
useDataTestId()
</script>

<style lang="scss">
.el-select {
    .el-select__wrapper {
        background-color: var(--input-background);
        border: 1px solid var(--input-border);
        box-shadow: none;

        &.is-focused, &.is-hovering {
            border: 1px solid var(--input-border__focus);
            box-shadow: none;
        }

        &.is-disabled {
            border: 1px solid var(--input-border);
        }

        .el-select__selected-item {
            .el-tag {
                color: var(--select-tag-color);
                background: var(--select-tag-background);
            }
        }

        .el-select__input, .el-select__placeholder {
            color: var(--input-color);
        }
    }
}

.el-form .el-select .el-select__wrapper {
    background-color: var(--form-input-background);
}

.el-popper.el-select__popper {
    background: var(--select-dropdown-background);
    border: 1px solid var(--select-dropdown-border);

    .el-popper__arrow::before {
        border-color: var(--select-dropdown-border);
    }

    .el-select-dropdown {
        .el-select-dropdown__item {
            color: var(--select-dropdown-color);

            &.is-hovering,
            &:hover{
                color: var(--select-dropdown-color__selected);
                background-color: var(--select-dropdown-item-background__hover);
            }
        }
    }
}
</style>
