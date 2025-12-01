<!-- 左侧列表\右侧内容区 ref -->
<template>
    <div class="resizeTransform" :class="{showResize:showResize}" :id="id" :style="{ [side]: '0px' }">
        <div class="bar"></div>
    </div>
</template>

<script>
    export default {
        props: {
            leftSide: {
                type: String,
                default: ''
            },
            rightSide: {
                type: String,
                default: ''
            },
            side: {
                type: String,
                default: 'right'
            },
            minWidth: {
                type: Number,
                default: 220
            },
            custom: {
                type: Boolean,
                default: false
            }
        },

        data () {
            return {
                startLeft: 0,
                endLeft: 0,
                x: 0,
                y: 0,
                mask: null,
                id: null,
                maskId: null,
                move: 0,
                currentWidth: 0,
                showResize: false

            }
        },
        created () {
            this.id = this.generateUUID()
            this.maskId = this.generateUUID()
            this.mask = document.createElement('div')
            this.mask.id = this.maskId
            this.mask.style.position = 'fixed'
            this.mask.style.top = '0px'
            this.mask.style.left = '0px'
            this.mask.style.width = '100%'
            this.mask.style.height = '100%'
            this.mask.style.zIndex = '10000'
            this.mask.style.cursor = 'e-resize';

        },
        watch: {
            currentWidth (val) {
                if (val < this.minWidth) {
                    this.currentWidth = this.minWidth
                }
            }
        },
        mounted () {
            this.currentWidth = this.getDom(this.leftSide).offsetWidth
            if (this.leftSide) {
                setTimeout(() => {
                    let letDom = document.getElementById(this.leftSide);
                    if (!letDom) {
                        letDom = document.getElementsByClassName(this.leftSide)[0]
                    }
                    letDom.classList.add("hoverActive")

                }, 1000)
            }
            this.dragControl()
        },
        methods: {
            generateUUID () {
                let d = new Date().getTime();
                if (window.performance && typeof window.performance.now === 'function') {
                    d += performance.now(); // use high-precision timer if available
                }
                let uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                    let r = (d + Math.random() * 16) % 16 | 0;
                    d = Math.floor(d / 16);
                    return (c === 'x' ? r : (r & 0x3) | 0x8).toString(16);
                });
                return uuid;
            },
            getDom (el) {
                let dom = document.getElementById(el);
                if (!dom) {
                    dom = document.getElementsByClassName(el)[0]
                }
                return dom;
            },
            setLeftWidth (width) {
                let letDom = this.getDom(this.leftSide);
                let finallyWidth = Number(width) + Number(this.currentWidth)
                if (letDom) {
                    if (finallyWidth < this.minWidth) {
                        letDom.style.width = Number(this.minWidth) + "px";
                    } else {
                        letDom.style.width = finallyWidth + "px";
                    }
                }
            },
            setRightWidth (width) {
                let rightDom = this.getDom(this.rightSide);
                let finallyWidth = Number(width) + Number(this.currentWidth)
                if (rightDom) {
                    if (finallyWidth < this.minWidth) {
                        rightDom.style.width = "calc(100% - " + this.minWidth + "px)";
                    } else {
                        rightDom.style.width = "calc(100% - " + finallyWidth + "px)";
                    }
                }
            },
            setData (e) {
                document.body.removeChild(document.getElementById(this.maskId))
                this.showResize = false
                e = e || window.event;
                let end = 0
                this.currentWidth = this.getDom(this.leftSide).offsetWidth;

                if (this.custom) {
                    if (window.top == window) {
                        if (e.clientX > this.minWidth) {
                            document.documentElement.style.setProperty('--maxActiveWidth', this.currentWidth)
                            end = e.clientX;
                        } else {
                            document.documentElement.style.setProperty('--maxActiveWidth', this.minWidth)
                            end = this.minWidth;
                        }
                    } else {
                        end = e.clientX;
                    }
                    this.$emit('resizeEnd', end - this.x)
                }

                this.mask.onmousemove = null
                this.mask.onmouseup = null
                this.mask.onmouseout = null
            },
            dragControl () {
                let drag = document.getElementById(this.id)
                drag.onmousedown = (e) => {
                    this.x = e.clientX;
                    let start = e.clientX;
                    document.body.appendChild(this.mask)
                    this.showResize = true

                    this.mask.onmousemove = (e) => {
                        let end = e.clientX;
                        if (this.side == 'right') {
                            this.move = end - start
                        } else {
                            this.move = start - end
                        }

                        e = e || window.event;
                        this.setLeftWidth(this.move)
                        this.setRightWidth(this.move)

                    }
                    this.mask.onmouseup = (e) => {
                        this.setData(e)

                    }
                    this.mask.onmouseout = (e) => {
                        this.setData(e)
                    }

                }
            }

        }
    }
</script>

<style lang="scss" scoped>
    .resizeTransform {
        width: 8px;
        height: 100%;
        position: absolute;
        cursor: e-resize;
        top: 0%;
        right: 0;
        font-size: 30px;
        color: var(--bor-default);
        background-color: transparent;
        transition: all .1s ease-in-out;
        display: flex;
        justify-content: flex-end;

        .bar {
            width: 2px;
            height: 100%;
            /* background-color: #3875ff; */
        }

        svg {
            position: absolute;
            top: 300px;
            left: 0px;
            display: none;
            transform: translateX(-50%);
        }
    }

    .hoverActive:hover .resizeTransform {
        transition: all 1s ease-in-out;
    }

    .showResize {
        cursor: e-resize;

        svg {
            display: block;
        }

    }
</style>
