<template>
    <el-time-picker v-bind="attrs">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-time-picker>
</template>

<script setup lang="ts">
import { useAttrs } from 'vue'
import { ElTimePicker } from 'element-plus'
const attrs = useAttrs()
</script>

<style lang="scss">

</style>
