<template>
    <el-upload class="upload_wrapper" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-upload>
</template>

<script setup lang="ts">
import { ElUpload } from 'element-plus'
import useExpose from '../../hooks/useExpose'
import { ref } from 'vue';

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)
</script>

<style lang="scss">
.upload_wrapper {
    color: var(--upload-tips-color);

    .el-upload__tip {
        color: var(--upload-tips-color);
    }

    .el-upload-list--picture .el-upload-list__item {
        background-color: var(--frame-main-background-1);
        border-color: var(--frame-main-border);
    }

    .el-upload-list--text .el-upload-list__item {
        &:hover {
            background-color: unset;
        }
    }
}
</style>