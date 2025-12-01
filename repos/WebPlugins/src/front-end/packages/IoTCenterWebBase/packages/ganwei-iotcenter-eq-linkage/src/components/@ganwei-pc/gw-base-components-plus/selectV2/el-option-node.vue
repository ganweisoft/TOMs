<template>
    <el-option :key="label + value" :label="source[label]" :value="source[value]">
        <div v-if="source.loading" class="el-option-loading"> <i v-loading="source.loading"
                element-loading-spinner="el-icon-loading" element-loading-background="rgba(0, 0, 0, 0.8)"></i></div>
        <span>{{ source[label] }}</span>
    </el-option>
</template>
<script>
export default {
    name: 'item-component',
    props: {
        index: {
            type: Number
        }, // 每一行的索引
        source: {
            type: Object,
            default () {
                return {}
            }
        }, // 每一行的内容
        label: {
            type: String
        }, // 需要显示的名称
        value: {
            type: String
        } // 绑定的值
    }
}
</script>