<template>
    <el-table :height="tableHeight" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps" :key="key">
            <slot :name="key" v-bind="slotProps"></slot>
        </template>
    </el-table>
</template>

<script setup lang="ts">
import { computed, ref, useAttrs } from 'vue';
import { ElTable } from 'element-plus'

import useDataTestId from '../../hooks/useDataTestId'
import useResizeTable from './useResizeTable'
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)

const props = defineProps({
    height: {
        type: Number,
        default: null
    }
})

const tableHeight = useResizeTable()
useDataTestId()
</script>

<style lang="scss">
@import "./ElTableMixin";

@include el-table;
</style>
