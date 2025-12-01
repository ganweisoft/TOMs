<template>
    <el-image fit="contain" lazy :src="_src" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps" :key="key"></slot>
        </template>
        <template #error v-if="!$slots.error">
            <div class="error-slot">
                <el-icon><icon-picture /></el-icon>
            </div>
        </template>
    </el-image>
</template>

<script setup lang="ts">

import { ElImage } from 'element-plus'
import { Picture as IconPicture } from '@element-plus/icons-vue'
import { watch, ref } from 'vue';

import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)

const props = withDefaults(defineProps<{
    src: string,
    request: (data: {id: string}) => Promise<{data: Blob}>
}>(), {
    src: '',
})

const _src = ref('')

watch(() => props.src, async (newVal) => {
    if(!newVal) {
        return;
    }
    // 文件路径
    if(newVal.indexOf('/') > -1) {
        _src.value = newVal
        return
    } else {
        if(!props.request) {
            _src.value = newVal
            return
        }
        // id
        props.request({id: props.src}).then(res => {
            const blob = res.data;
            try {
                URL.revokeObjectURL(_src.value)
            } catch (e) {
                
            }
            _src.value = URL.createObjectURL(blob)
        })
    }

}, {
    immediate: true
})

</script>

<style lang="scss">
.el-image {
    .error-slot {
        color: var(--frame-main-color);
    }
    .el-image__placeholder {
        background-color: transparent
    }
}
</style>