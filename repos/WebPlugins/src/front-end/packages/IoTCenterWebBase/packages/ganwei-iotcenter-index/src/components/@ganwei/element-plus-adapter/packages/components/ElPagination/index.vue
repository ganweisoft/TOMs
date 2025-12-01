<template>
    <el-pagination v-bind="{...defaultAttrs, ...attrs}" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-pagination>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElPagination } from 'element-plus';

import useDataTestId from '../../hooks/useDataTestId'
const attrs = useAttrs()
useDataTestId()

import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)

const defaultAttrs = {
    pageSizes: [20, 50, 100],
    layout: "sizes,prev, pager, next,total"
}
</script>

<style lang="scss">
.el-pagination.is-background {
    .el-select .el-select__wrapper {
        background-color: var(--page-background);
        border: none;

        .el-input__inner {
            color: var(--page-color);
            background-color: var(--page-background);
            border: 1px solid var(--page-border);
        }
    }
    .el-pagination__sizes  {
        margin-right: 10px;
    }

    .btn-prev,
    .btn-prev:disabled,
    .btn-next,
    .btn-next:disabled {
        margin: 0 5px;
        color: var(--page-color);
        background-color: var(--page-background);
        border: 1px solid var(--page-border);
    }

    .el-pager li.is-active{
        margin: 0 5px;
        color: var(--page-color__active);
        background-color: var(--page-background__active);
        border-color: var(--page-background__active);
    }

    .el-pagination__jump {
        .el-input  {
            .el-input__wrapper, .el-input__inner {
                background-color: var(--page-background);
            }
        }
    }

}
</style>
