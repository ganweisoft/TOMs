<template>
    <el-form v-bind="attrs" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-form>
</template>

<script setup lang="ts">
import {onMounted, ref, useAttrs} from 'vue'
import {ElForm} from 'element-plus'

import useExpose from '../../hooks/useExpose'
const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)
const attrs = useAttrs()

</script>

<style lang="scss">
.el-form {
    .el-form-item, .el-form-item__label {
        color: var(--form-label-color);
    }

    .el-input {
        .el-input__wrapper, .el-input__inner {
            background-color: var(--form-input-background);
        }

        input:-webkit-autofill,
        textarea:-webkit-autofill {
            -webkit-box-shadow: 0 0 0px 1000px var(--form-input-background) inset !important;
        }
    }

    .el-textarea {
        .el-textarea__inner  {
            background-color: var(--form-input-background);
        }

        .el-input__count {
            background-color: transparent;
        }
    }
}

.el-popper {
    border: 1px solid var(--frame-main-border);

    &.is-light .el-popper__arrow::before {
        background-color: var(--select-dropdown-background);
        border-color: var(--poper-border);
    }
}

.el-dropdown-menu {
    padding: 9px 0;
    background-color: var(--dropdown-menu-background);
    border-radius: 0;

    >p:hover {
        background: var(--dropdown-menu-background__hover);
    }
}
</style>
