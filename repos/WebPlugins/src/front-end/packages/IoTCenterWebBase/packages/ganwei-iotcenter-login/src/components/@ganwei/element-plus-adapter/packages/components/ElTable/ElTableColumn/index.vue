<template>
    <el-table-column :formatter="formatter_wrapper">
        <template v-for="(value, key) in $slots" #[key]="slotProps" :key="key">
            <slot :name="key" v-bind="slotProps"></slot>
        </template>
    </el-table-column>
</template>

<script setup lang="ts">
import { ElTableColumn } from 'element-plus';
const props = defineProps({
    formatter: {
        type: Function,
        default: null
    }
})
function formatter_wrapper(row: any, column: any, cellValue: any, index: number) {
    if(props.formatter) {
        return props.formatter(row, column, cellValue, index)
    }
    return cellValue || '-'
}
</script>
