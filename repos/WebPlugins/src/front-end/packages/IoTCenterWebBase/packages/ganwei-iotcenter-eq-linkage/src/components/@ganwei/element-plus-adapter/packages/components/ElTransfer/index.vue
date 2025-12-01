<template>
    <el-transfer></el-transfer>
</template>

<script setup lang="ts">
import { ElTransfer } from 'element-plus';
</script>

<style lang="scss">
.el-transfer {
    .el-transfer-panel {
        background-color: var(--transfer-background);

        .el-transfer-panel__header {
            background-color: var(--transfer-header-background);
            border-color: var(--transfer-border);

            .el-checkbox .el-checkbox__label {
                color: var(--checkbox-label-color);
            }
        }

        .el-transfer-panel__body {
            border-color: var(--transfer-border);

            .el-input  {
                .el-input__wrapper, .el-input__inner {
                    background: var(--form-input-background);
                }
            }
        }
    }
}
</style>