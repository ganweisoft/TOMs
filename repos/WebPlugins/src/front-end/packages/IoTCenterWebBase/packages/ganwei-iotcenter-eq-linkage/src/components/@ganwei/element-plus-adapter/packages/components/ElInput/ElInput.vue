<template>
    <el-input ref="input" v-model="model" v-bind="attrs" @input="inputHandler" @change="changeHandler">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps" :key="key"></slot>
        </template>
    </el-input>
</template>

<script setup lang="ts">
import { ref, useAttrs } from 'vue'
import { ElInput } from 'element-plus';

import useDataTestId from '../../hooks/useDataTestId'

const model = defineModel<string>({
    default: ''
})

let preChangeVal = ''

const emits = defineEmits(['change'])

function changeHandler(val: string) {
    if(preChangeVal === val) return
    preChangeVal = val
    emits('change', val)
}

const input = ref<InstanceType<typeof ElInput>>()
function inputHandler(val: string) {
    if(model.value !== '' && val.trim() === '') {
        changeHandler('')
    }
    model.value = val.trim()
}

const attrs = useAttrs()
useDataTestId()
</script>

<style lang="scss">
@import './ElInputMixin';

.el-input__wrapper {
    color: var(--input-color);
    background-color: var(--input-background);
    box-shadow: 0 0 0 1px var(--input-border) inset;

    input{
        color: var(--input-color);
    }
}

@include el-input;
@include el-input-number;
@include el-textarea;
</style>
