<template>
    <el-table :height="height">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-table>
</template>

<script setup lang="ts">
import { ElTable } from 'element-plus'

import useDataTestId from '../../hooks/useDataTestId'
import useResizeTable from './useResizeTable'

const props = defineProps({
    height: {
        type: Number,
        default: null
    }
})

const height = useResizeTable()

useDataTestId()
</script>

<style lang="scss">
@import "./ElTableMixin";

@include el-table;
</style>
