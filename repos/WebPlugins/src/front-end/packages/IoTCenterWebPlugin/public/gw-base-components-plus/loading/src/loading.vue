<template>
    <transition name="el-loading-fade" @after-leave="handleAfterLeave">
        <div v-show="visible" class="el-loading-mask" :style="{ backgroundColor: background || '' }" :class="[customClass, { 'is-fullscreen': fullscreen }]">
            <div class="el-loading-spinner">
                <div class="container" v-if="!spinner && !src && !componentId">
                    <span v-if="loadingType == 1" class="loader1"></span>
                    <div v-else-if="loadingType == 2" class="loader">
                    </div>
                    <div v-else class="loading-pulse"></div>
                </div>
                <i v-if="spinner" :class="spinner"></i>
                <img v-if="src" :src="src" alt="loading...">
                <p v-if="text" class="el-loading-text">{{ text }}</p>
                <component v-if="componentId" :is="componentId"></component>
            </div>
        </div>
    </transition>
</template>

<script>
  export default {
    data () {
        return {
            text: null,
            spinner: null,
            background: null,
            fullscreen: true,
            visible: false,
            customClass: '',
            src: '',
            loadingType: '',
            componentId: ''
        };
    },
    mounted () {
        // console.log(this.loadingType);
    },

    methods: {
        handleAfterLeave () {
            this.$emit('after-leave');
        },
        setText (text) {
            this.text = text;
        }
    }
};
</script>

<style scoped lang="scss">
.el-loading-spinner {
    transform: translateY(-50%);
    margin-top: 0px;
}

.container {
    display: flex;
    justify-content: center;
}

.loader {
    width: 40px;
    height: 40px;
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);

    &::before,
    &::after {
        content: "";
        width: 100%;
        height: 100%;
        border-radius: 50%;
        @include background-color("loadingBg");
        opacity: 0.6;
        position: absolute;
        top: 0;
        left: 0;
        animation: bounce 2s ease-in-out infinite;
    }

    &::after {
        animation-delay: -1s;
    }
}

@keyframes bounce {
    0%,
    20%,
    53%,
    80%,
    to {
        -webkit-animation-timing-function: cubic-bezier(0.215, 0.61, 0.355, 1);
        animation-timing-function: cubic-bezier(0.215, 0.61, 0.355, 1);
        -webkit-transform: translateZ(0);
        transform: translateZ(0);
    }

    40%,
    43% {
        -webkit-animation-timing-function: cubic-bezier(
            0.755,
            0.05,
            0.855,
            0.06
        );
        animation-timing-function: cubic-bezier(0.755, 0.05, 0.855, 0.06);
        -webkit-transform: translate3d(0, -30px, 0);
        transform: translate3d(0, -30px, 0);
    }

    70% {
        -webkit-animation-timing-function: cubic-bezier(
            0.755,
            0.05,
            0.855,
            0.06
        );
        animation-timing-function: cubic-bezier(0.755, 0.05, 0.855, 0.06);
        -webkit-transform: translate3d(0, -15px, 0);
        transform: translate3d(0, -15px, 0);
    }

    90% {
        -webkit-transform: translate3d(0, -4px, 0);
        transform: translate3d(0, -4px, 0);
    }
}

.loader1 {
    width: 12px;
    height: 12px;
    border-radius: 50%;
    display: inline-block;
    position: relative;
    @include font_color('loadingBg');
    -webkit-animation: animloader1 1s linear infinite;
    animation: animloader1 1s linear infinite;
}
@keyframes animloader1 {
    0% {
        box-shadow: 14px 0 0 -2px, 38px 0 0 -2px, -14px 0 0 -2px, -38px 0 0 -2px;
    }
    25% {
        box-shadow: 14px 0 0 -2px, 38px 0 0 -2px, -14px 0 0 -2px, -38px 0 0 2px;
    }
    50% {
        box-shadow: 14px 0 0 -2px, 38px 0 0 -2px, -14px 0 0 2px, -38px 0 0 -2px;
    }
    75% {
        box-shadow: 14px 0 0 2px, 38px 0 0 -2px, -14px 0 0 -2px, -38px 0 0 -2px;
    }
    100% {
        box-shadow: 14px 0 0 -2px, 38px 0 0 2px, -14px 0 0 -2px, -38px 0 0 -2px;
    }
}

.spinner {
    -webkit-animation: rotator 1.4s linear infinite;
    animation: rotator 1.4s linear infinite;
}

@-webkit-keyframes rotator {
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(270deg);
    }
}

@keyframes rotator {
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(270deg);
    }
}
.path {
    stroke-dasharray: 187;
    stroke-dashoffset: 0;
    transform-origin: center;
    -webkit-animation: dash 1.4s ease-in-out infinite,
        colors 5.6s ease-in-out infinite;
    animation: dash 1.4s ease-in-out infinite, colors 5.6s ease-in-out infinite;
}

@keyframes colors {
    0% {
        stroke: #4285f4;
    }
    25% {
        stroke: #de3e35;
    }
    50% {
        stroke: #f7c223;
    }
    75% {
        stroke: #1b9a59;
    }
    100% {
        stroke: #4285f4;
    }
}

@keyframes dash {
    0% {
        stroke-dashoffset: 187;
    }
    50% {
        stroke-dashoffset: 46.75;
        transform: rotate(135deg);
    }
    100% {
        stroke-dashoffset: 187;
        transform: rotate(450deg);
    }
}
/* 
i {
  height: 16px;
  width: 16px;
  border-radius: 100%;
  background: #fff;
  display: block;
  position: absolute;
  left: 50%;
  animation: spin 2s ease infinite;
}
i:before, i:after {
  content: '';
  display: block;
  position: absolute;
  height: inherit;
  width: inherit;
  background: inherit;
  border-radius: inherit;
  animation: spin 2s ease infinite;
}
i:before {
  left: -2.3em;
}
i:after {
  left: 2.3em;
}

@keyframes spin {
  0% {
    top: 0;
    transform: rotate(0deg);
  }
  50% {
    top: -4em;
    transform: rotate(-180deg);
  }
  100% {
    top: 0;
    transform: rotate(-360deg);
  }
} */

.spinner {
    -webkit-animation: rotator 1.4s linear infinite;
    animation: rotator 1.4s linear infinite;
}

@keyframes rotator {
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(270deg);
    }
}

.path {
    stroke-dasharray: 187;
    stroke-dashoffset: 0;
    transform-origin: center;
    -webkit-animation: dash 1.4s ease-in-out infinite,
        colors 5.6s ease-in-out infinite;
    animation: dash 1.4s ease-in-out infinite, colors 5.6s ease-in-out infinite;
}

@keyframes colors {
    0% {
        stroke: #4285f4;
    }
    25% {
        stroke: #de3e35;
    }
    50% {
        stroke: #f7c223;
    }
    75% {
        stroke: #1b9a59;
    }
    100% {
        stroke: #4285f4;
    }
}

@keyframes dash {
    0% {
        stroke-dashoffset: 187;
    }
    50% {
        stroke-dashoffset: 46.75;
        transform: rotate(135deg);
    }
    100% {
        stroke-dashoffset: 187;
        transform: rotate(450deg);
    }
}

.loading-pulse {
    position: relative;
    width: 6px;
    height: 24px;
    background: rgba(255, 255, 255, 0.2);
    -webkit-animation: pulse 750ms infinite;
    animation: pulse 750ms infinite;
    -webkit-animation-delay: 250ms;
    animation-delay: 250ms;
}
.loading-pulse:before,
.loading-pulse:after {
    content: "";
    position: absolute;
    display: block;
    height: 16px;
    width: 6px;
    background: rgba(255, 255, 255, 0.2);
    top: 50%;
    transform: translateY(-50%);
    -webkit-animation: pulse 750ms infinite;
    animation: pulse 750ms infinite;
}
.loading-pulse:before {
    left: -12px;
}
.loading-pulse:after {
    left: 12px;
    -webkit-animation-delay: 500ms;
    animation-delay: 500ms;
}
@keyframes pulse {
    50% {
        background: white;
    }
}
html,
body {
    height: 100%;
}
</style>
