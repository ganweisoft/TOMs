<template>
    <transition name="el-fade-in" @after-leave="$emit('destroy')">
        <div class="gw-toast" v-show="visible" role="alert"
            style="position: fixed; z-index: 99999; top: 50%; color: white; transform: translateY(-50%);">
            <div class="gw-toast-inner" :class="{ 'big-msg': isBig, 'has-icon': icon }">
                <div class="message">
                    <el-result :icon="type" :title="message">
                    </el-result>
                </div>
            </div>
        </div>
    </transition>
</template>

<script>
import { ElResult } from 'element-plus'
export default {
    components: { ElResult },
    name: 'gwToast',
    data () {
        return {
            visible: false,
            message: '',
            duration: 1200,
            type: '',
            customClass: '',
            iconClass: '',
            onClose: null,
            closed: false,
            timer: null,
            isBig: false,
            icon: ''
        };
    },

    watch: {
        closed (newVal) {
            if (newVal) {
                this.visible = false;
            }
        }
    },

    methods: {
        close () {
            this.closed = true;
            if (typeof this.onClose === 'function') {
                this.onClose();
            }
        },

        clearTimer () {
            clearTimeout(this.timer);
        },

        startTimer () {
            if (this.duration > 0) {
                this.timer = setTimeout(() => {
                    if (!this.closed) {
                        this.close();
                    }
                }, this.duration);
            }
        }
    },

    mounted () {
        if (this.duration > 0) {
            this.timer = setTimeout(() => {
                if (!this.closed) {
                    this.close();
                }
            }, this.duration);
        }
    }
};
</script>

<style lang="scss">
.gw-toast {
    // width: 170px;
    // height: 156px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-between;
    left: 0;
    right: 0;
    margin: 0 auto;

    .icon .iconfont {
        font-size: 61px;
        // height: 61px;
    }
}

.gw-toast-inner {
    max-width: 280px;
    background: rgba(0, 0, 0, 0.8);
    padding: 12px;
    border-radius: 6px;
}

.message {
    font-size: 15px;
    display: flex;
    align-items: center;

    .iconfont,
    .el-icon-circle-close {
        margin-right: 8px;
    }
}

.big-msg {
    padding: 25px 55px;

    &.has-icon {
        padding: 25px 25px;

        .icon {
            text-align: center;
            margin-bottom: 8px;
        }

        .iconfont {
            font-size: 24px;
            color: #3874f7;
        }
    }

    .icon {
        margin-bottom: 26px;
    }
}
</style>
