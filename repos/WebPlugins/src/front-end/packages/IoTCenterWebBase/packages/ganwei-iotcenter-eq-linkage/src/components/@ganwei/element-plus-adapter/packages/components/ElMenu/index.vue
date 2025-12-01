<template>
    <el-menu v-bind="attrs">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-menu>
</template>

<script setup lang="ts">
import { useAttrs } from 'vue'
import { ElMenu } from 'element-plus';

import useDataTestId from '../../hooks/useDataTestId'
const attrs = useAttrs()
useDataTestId()
</script>

<style lang="scss">
.el-menu {
    background-color: transparent;

    &::-webkit-scrollbar {
        width: 8px;
        height: 8px;
        margin: 50px;

        background-color: var(--scrollbar-background);
        border-radius: 5px;
    }

    .el-menu-item {
        display: flex;
        align-items: center;

        height: 40px;

        line-height: 40px;
        color: var(--menu-color);

        transition: none;

        &:hover {
            color: var(--menu-color__hover);
            background-color: var(--menu-background__hover);
        }

        &.is-active {
            color: var(--menu-color__active);
            background-color: var(--menu-background__active);
        }
    }

    .el-sub-menu {
        .el-sub-menu__title {
            color: var(--menu-color);
        }

        .el-sub-menu__title:hover {
            color: var(--menu-color__hover);
            background-color: var(--menu-background__hover);
        }
    }
}

</style>
