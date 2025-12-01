<template>
    <commonSelect v-bind="$attrs" v-model="ziChanId" :value="value" :props="ziChanProps" :resolve="resolveZiChanList"
        :currentSelect='currentSelect' url="getZiChan">
    </commonSelect>
</template>

<script>
import commonSelect from '../commonSelect/index.vue'
export default {
    model: {
        prop: 'value',
        event: 'change'
    },
    components: {
        commonSelect
    },
    props: {
        currentSelect: {
            type: Object,
            default: () => { }
        }
    },
    data () {
        return {
            ziChanProps: {
                label: 'ziChanName',
                value: 'ziChanId'
            },
            ziChanId: ''
        }
    },
    watch: {
        ziChanId (val) {
            this.$emit('change', val)
        },
    },
    methods: {
        resolveZiChanList (res, pagination) {
            const { code, data, message } = res?.data || {}
            let list = []
            if (code == 200) {
                list = data || []
                // pagination.total = totalCount
            } else {
                this.$message.warning(message)
            }

            return list
        },
    }
}
</script>
