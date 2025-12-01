<template>
    <transition name="el-fade-in">
        <div @mouseup="this.close" v-show="visible" class="ContextMenu-wrapper">
            <ul :class="['ContextMenu', customClass]" :style="`width: ${width}px;left: ${left}px;top: ${top}px;`">
                <li @mouseup="handleClick($event, item)" class="ContextMenu-item" :class="{ 'is-disabled': item.disabled }"
                    v-for="item in commands" :key="item">
                    <i :class="['iconfont', iconClass, item.icon]"></i>
                    <span>{{ item.label }}</span>
                </li>
            </ul>
        </div>
    </transition>
</template>

<script type="text/babel">
export default {
    data () {
        return {
            visible: false,
            customClass: '',
            iconClass: '',
            onClose: null,
            closed: false,
            commands: [],
            left: -9999,
            top: -9999,
            width: 100
        };
    },

    watch: {
        closed (newVal) {
            if (newVal) {
                this.visible = false;
                // this.$el.addEventListener('transitionend', this.destroyElement);
            }
        }
    },
    methods: {
        destroyElement () {
            this.$el.removeEventListener('transitionend', this.destroyElement);
            this.$destroy(true);
            this.$el.parentNode.removeChild(this.$el);
        },

        close () {
            this.closed = true;
            if (typeof this.onClose === 'function') {
                this.onClose();
            }
        },

        handleClick (e, item) {
            if (item.disabled) {
                e.stopPropagation();
                return;
            }
            item.onclick && item.onclick();
        }
    },

    // mounted () {
    // }
};
</script>

<style lang="scss" scoped>
.ContextMenu-wrapper {
    position: fixed;
    width: 100%;
    height: 100%;
    top: 0;
    left: 0;
}

.ContextMenu {
    position: fixed;
    background-color: var(--contextmenu-background);
    border-radius: 6px;
    margin: 4px;
    padding: 6px 0;
    display: flex;
    flex-direction: column;
    border: 1px solid rgba(240, 244, 255, 0.16);
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
    user-select: none;

    &-item {
        cursor: pointer;
        padding: 10px;
        font-size: 14px;
        color: var(--contextmenu-color);
        display: flex;
        align-items: center;

        i {
            display: inline-block;
            min-width: 16px;
            margin-right: 5px;
            font-size: 16px;
        }

        &.is-disabled {
            cursor: not-allowed;
            color: var(--contextmenu-color__disabled);
        }

        &:not(.is-disabled):hover {
            background-color: var(--contextmenu-background__hover);
        }
    }
}
</style>
