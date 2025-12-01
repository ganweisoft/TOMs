import { ref } from "vue"

export default function useTree() {
    const myTreeRef = ref()
    const searchEquip = ref('')
    const changeSearchEquip = (value: string) => {
        myTreeRef.value.filterMethod(value)
    }

    return {
        myTreeRef, searchEquip, changeSearchEquip
    }
}
