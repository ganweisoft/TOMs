<template>
    <el-time-picker v-bind="attrs" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-time-picker>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElTimePicker } from 'element-plus'
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)
const attrs = useAttrs()
</script>

<style lang="scss">

</style>
