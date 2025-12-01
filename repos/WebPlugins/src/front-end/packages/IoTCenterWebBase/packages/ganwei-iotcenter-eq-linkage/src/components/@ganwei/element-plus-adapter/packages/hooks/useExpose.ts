import { onMounted, Ref } from 'vue'
export default function (proxyElement: Ref<any>) {
    const expose = {}

    onMounted(() => {
        Object.assign(expose, proxyElement.value);
    });
    return expose
}