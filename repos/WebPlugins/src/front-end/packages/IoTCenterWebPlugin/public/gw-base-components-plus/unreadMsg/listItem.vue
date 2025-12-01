<template>
    <div class="message-content-item" @click="contentItemClick(source, index)">
        <p class="item-top">{{ source.EventMsg }}</p>
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
            default: () => { }
        }
    }
};
</script>
<style lang="scss" scoped>
.xinxi {
    color: rgb(79, 160, 242) !important;
}

.shezhi {
    color: rgb(51, 191, 126) !important;
}

.zichan {
    color: rgb(51, 191, 126) !important;
}

.guzhang {
    color: rgb(240, 63, 63) !important;
}

.jinggao {
    color: rgb(255, 142, 83) !important;
}

.message-content-item {
    width: 100%;
    line-height: 38px;
    color: var(--con-textColor1);
    cursor: pointer;

    padding: 0;

    .message-content-line {
        width: 100%;
        height: 1px;
        background: var(--notifytext);
        transform: scale(1, 0.5);
    }

    .item-bottom {
        display: flex;
        justify-content: space-between;
        color: var(--notifytext);

        .item-left {
            img {
                width: 14px;
                height: 14px;
                transform: translateY(1px);
            }
        }

        .item-right {
            flex: 1;
            text-align: right;
            font-size: 12px;
        }

    }

    &:hover {
        background-color: transparent !important;

        .item-left,
        .item-top,
        .item-right {
            color: var(--con-themeColor) !important;
            // @include font_color("con-themeColor");
        }
    }
}
</style>
