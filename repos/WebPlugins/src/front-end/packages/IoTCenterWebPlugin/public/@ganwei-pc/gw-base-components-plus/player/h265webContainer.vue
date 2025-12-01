<template>
    <div class="container">
        <H265web :panelControl="props?.panelControl" :id="100" :selected="100"></H265web>
    </div>
</template>

<script lang="ts" setup>
import { defineProps, getCurrentInstance, onMounted, watch } from 'vue'

import H265web, { IH265WebProps } from './h265webPlayer/index.vue'

export interface Props {
    streamId: number,
    url: string,
    panelControl: IH265WebProps['panelControl']
}

const {proxy} = getCurrentInstance() as any
const props = defineProps<Props>();
onMounted(() => {
    sendInit()
})
function sendInit(){
    let obj = [{id: -1, url: '', name: "mp4", value: "", title: ""}]
    props?.streamId ? obj[0].id = props.streamId : obj[0].url = props?.url
    proxy.$bus.emit('H265web100', obj)
}
watch(() => props?.streamId, (val) => {
    if(!val) return
    sendInit()
})
watch(() => props?.url, (val) => {
    if(!val) return
    sendInit()
})
</script>

<style lang="scss" scoped>
    .container{
        height: 500px;
    }
</style>
