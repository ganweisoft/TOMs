<template>
    <div class="message-content-item" @click="contentItemClick(source, index)">
        <p class="item-top">{{ source.eventMsg }}</p>
        <div class="item-bottom">
            <div class="item-label item-left" :class="source.className">
                <img :src="source.iconRes" alt="">
                {{ source.snapshotName }}
            </div>
            <div class="item-time item-right">
                {{ source.Time }}
            </div>
        </div>
        <div class="message-content-line"></div>
    </div>
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
                return {};
            }
        }, // 每一行的内容
        contentItemClick: {
            type: Object,
            default: () => ({})
        }
    }
};
</script>
<style lang="scss" scoped></style>
