<template>
    <el-button v-bind="{ type: 'primary', loading, ...attrs, onClick: handleClick}">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-button>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElButton } from 'element-plus';

export interface IButtonOverride {
    onClick: (evt: MouseEvent) => any
}

defineOptions({
  inheritAttrs: false,
})

const attrs = useAttrs() as unknown as IButtonOverride

const loading = ref(false)
const handleClick = async (evt: MouseEvent) => {
    const res = attrs.onClick(evt)
    if(res instanceof Promise) {
        loading.value = true
        res.finally(() => {
            loading.value = false
        })
    }
    return res
}
</script>

<style lang="scss">
.el-button {
    &.el-button--primary {
        background-color: var(--button-primary-background);
        border-color: var(--button-primary-border-color);

        &.is-plain, &.is-plain:hover {
            color: var(--button-primary-color);
            background-color: transparent;
            border: 1px solid var(--button-primary-border-color);
        }
    }
}
</style>
