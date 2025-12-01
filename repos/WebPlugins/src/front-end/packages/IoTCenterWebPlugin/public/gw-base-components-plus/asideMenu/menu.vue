<template>
    <div v-if="data.menuOwner !== 2">
        <el-submenu v-if="data.nodeType == 1" :key="data.resourceId" :index="$t(data.name)" >
            <template slot="title">
                <i class="iconfont" :class="
            data.icon
            ? data.icon
            : data.children.length > 0 && 'icon' in data.children[0]
            ? data.children[0].icon
            : ''
        " v-show="showIcon"></i>
                <el-tooltip :disabled="showToolTip" class="item" effect="dark" ref="tooltip" :content="$t(data.name)"
                    placement="right" popper-class="menuOverflowPoper">
                    <span @mouseenter="handleMouseEnter">{{ $t(data.name) }}</span>
                </el-tooltip>

                <i class="iconfont icon-caidan_xiala"></i>
            </template>
            <asideMenu v-for="item in data.children" :data="item" :key="item.resourceId" @func="setTableTabs"
                :showIcon="showIcon"></asideMenu>
        </el-submenu>
        <el-menu-item :name="$t(data.name)" :key="data.resourceId" :index="data.route" @click="changeRoute(data.name)"
            v-else-if="data.nodeType == 2" >
            <template v-if="isCollapse">
                <el-tooltip class="item" effect="dark" ref="tooltip" :content="$t(data.name)" placement="right"
                    popper-class="menuOverflowPoper">
                    <span class="singleCode">{{ $t(data.name).slice(0,1) }}</span>
                </el-tooltip>

            </template>
            <template v-else>
                <!-- <span v-show="showIcon"></span> -->
                <i class="iconfont" :class="data.icon" style="margin-right: 16px;"></i>
                <el-tooltip :disabled="showToolTip" class="item" effect="dark" ref="tooltip" :content="$t(data.name)"
                    placement="right" popper-class="menuOverflowPoper">
                    <span @mouseenter="handleMouseEnter">{{ $t(data.name) }}</span>
                </el-tooltip>
            </template>

        </el-menu-item>
    </div>
</template>
<script>
    export default {
        name: 'asideMenu',
        props: {
            data: {
                type: Object,
                default: () => { },
            },
            showIcon: {
                type: Boolean,
                default: true,
            },
            isCollapse: {
                type: Boolean,
                default: false
            }
        },
        data () {
            return {
                width: 0,
                showToolTip: true,
                firstMeasure: true
            }
        },
        mounted () {
            // this.$store.commit('setRouteList', this.data);
        },
        methods: {
            changeRoute (name, route) {
                this.$emit('func', name)
            },
            setTableTabs (name) {
                this.$emit('func', name)
            },
            handleMouseEnter () {
                if (this.firstMeasure) {
                    const text = this.$el.querySelector('.el-tooltip.item');
                    const range = document.createRange()
                    range.setStart(text, 0);
                    range.setEnd(text, text.childNodes.length);
                    let RealWidth = range.getBoundingClientRect().width;

                    if (text.getBoundingClientRect().width < RealWidth) {
                        this.showToolTip = false;
                    }
                    this.firstMeasure = false;
                }
            }
        },
    }
</script>

<style lang="scss" scoped>
    .singleCode {
        width: 25px !important;
        height: 25px !important;
        flex-shrink: 1 !important;
        border-radius: 50%;
        background-color: transparent !important;
        margin-right: 0px !important;
        line-height: 21px !important;
        /* background: var(--img-color-1); */
        /* border: 1px solid; */
        /* border-color: var(--input-iconColor1); */
        text-align: center;
        /* color: #181C29 !important; */
    }
</style>
