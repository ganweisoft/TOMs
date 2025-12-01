import { ref } from "vue";

import { IStreamInfo } from "../Models";

export default function useStreamList() {
    const playStreamIdList = ref<IStreamInfo[]>([])  // 流列表
    function setPlayStreamIdList(streamList: Array<IStreamInfo>) {
        if (!streamList.length) {
            console.error('没有流信息')
            return;
        }
        playStreamIdList.value = streamList
    }
    return {
        playStreamIdList, setPlayStreamIdList
    }
}
