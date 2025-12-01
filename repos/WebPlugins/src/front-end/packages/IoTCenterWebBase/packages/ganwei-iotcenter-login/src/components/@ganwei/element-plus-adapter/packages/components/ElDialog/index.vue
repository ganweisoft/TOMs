<template>
    <el-dialog v-bind="$attrs" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-dialog>
</template>
<script setup lang="ts">
import { ElDialog } from 'element-plus';
import useExpose from '../../hooks/useExpose'
import { ref } from 'vue';

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)
</script>

<style lang="scss">
.el-dialog {
    background: var(--dialog-background);
    border-color: var(--dialog-border);
    border-radius: 6px;
    box-shadow: var(--dialog-shadow);

    .el-dialog__header {
        padding: 23px 20px;
    }

    .el-dialog__header, .el-dialog__title {
        font-size: 16px;
        color: var(--dialog-color);
        border-color: var(--dialog-border);
        height: 64px;
        // padding: 23px 20px;
        text-align: left;
    }

    .el-dialog__body {
        color: var(--dialog-color);
        height: calc(100% - 136px);
        padding: 0;
        overflow: hidden;
    }

    .el-dialog__footer {
        box-sizing: border-box;
        height: 72px;
        padding: 16px 20px;
        border: none;
        display: flex;
        justify-content: flex-end;
        align-items: center;

        .el-button {
            min-width: 92px;
            height: 32px;
            line-height: 32px;
            border-radius: 2px;
        }

        .el-button--primary.is-plain {
            color: var(--dialog-footer-button-color__primary);
            border: 1px solid var(--button-primary-color);
            background-color: transparent;
        }

        .el-button--primary {
            background-color: var(--button-primary-color);
            border: 1px solid var(--button-primary-color);
        }
    }

}
</style>
