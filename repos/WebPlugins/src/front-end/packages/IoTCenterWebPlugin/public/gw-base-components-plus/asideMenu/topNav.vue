<template>
    <div class="top-nav-warpper" ref="top-nav-warpper">
        <span class="arrow arrow-left iconfont icon-tubiao24_you" v-if="showLeftArrow" @click="arrowClikc('left')"></span>
        <div class="scrollView" ref="top-nav-scrollView">
            <div class="top-nav-container" ref="top-nav-container">
                <div class="top-nav-item" :class="{ active: activeItem === item.index }" :id="item.id + item.name"
                    v-for="item in options" :key="item.id" @click="itemClick(item)">{{ $t(item.name) }}</div>
            </div>
        </div>
        <span class="arrow arrow-right iconfont icon-tubiao24_you" @click="arrowClikc('right')"
            v-if="showRightArrow"></span>
    </div>
</template>

<script>
export default {
    name: 'TopNav',
    model: {
        prop: 'activeItem',
        event: 'itemClick'
    },
    props: {
        options: {
            type: Array,
            default: () => []
        },
        activeItem: {
            type: Number,
            default: 0
        }
    },
    data () {
        return {
            showLeftArrow: false,
            showRightArrow: false
        }
    },
    mounted () {
        this.updateArrayShow()
    },
    methods: {
        resize () {
            this.updateArrayShow()
        },
        updateArrayShow () {
            setTimeout(() => {
                if (this.$refs['top-nav-warpper'].clientWidth < this.$refs['top-nav-container'].clientWidth) {
                    this.showLeftArrow = true;
                    this.showRightArrow = true
                }
            }, 200)
        },
        itemClick (item) {
            this.$emit('itemClick', item.index);
            this.$emit('click', item.index);
            let dom = document.getElementById(`${item.id}${item.name}`)
            dom.scrollIntoView({ behavior: "smooth" })
        },
        arrowClikc (type) {
            switch (type) {
                case 'left':
                    this.$refs['top-nav-scrollView'].scrollTo({ left: this.$refs['top-nav-scrollView'].scrollLeft - 200, behavior: "smooth" })
                    break;
                case 'right':
                    this.$refs['top-nav-scrollView'].scrollTo({ left: this.$refs['top-nav-scrollView'].scrollLeft + 200, behavior: "smooth" })
                    break;
                default:
                    break;
            }
        }
    }
}
</script>

<style lang="scss" scoped>
.top-nav-warpper {
    display: flex;
    width: calc(100% - 295px);
    min-width: 140px;
    max-width: 840px;
    position: relative;
    overflow: hidden;
    line-height: 60px;
    user-select: none;

    .arrow {
        display: none;
        font-size: 24px;
        color: var(--top-menu-fc);
        position: absolute;
        cursor: pointer;
        z-index: 2;
    }

    .scrollView {
        display: flex;
        justify-content: flex-start;
        overflow: auto;
        width: 100%;
    }

    &:hover .arrow {
        display: block;
    }

    .arrow-right {
        right: 0px;
    }

    .arrow-left {
        left: 0;
        transform: rotateY(180deg);
    }
}

.top-nav-container {
    display: inline-block;
    line-height: 60px;
    transition: transform 0.3s ease-in-out;

    .top-nav-item {
        display: inline-block;
        font-weight: 600;
        color: var(--top-menu-fc);
        position: relative;
        cursor: pointer;
        padding: 0 20px;

        &:hover {
            background: var(--top-menu-bg);

            &::after {
                content: '';
                height: 6px;
                width: 100%;
                position: absolute;
                bottom: 0px;
                left: 0px;
                background-color: #3875FF;
            }
        }

        &.active {
            /* background: var(--top-menu-bg); */

            &::after {
                content: '';
                height: 6px;
                width: 100%;
                position: absolute;
                bottom: 0px;
                left: 0px;
                background-color: #3875FF;
            }
        }
    }
}
</style>