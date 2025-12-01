import { Ref } from 'vue'
export default function (proxyElement: Ref<any>) {
    const expose = new Proxy({}, {
        get(target, key) {
            return proxyElement.value?.[key]
        },
        has(target, key) {
            return key in proxyElement.value
        },
    })
    return expose
}