<!--
        data.menuOwner : 1:WEB 2:APP 0:WEB&APP
        data.nodeType : 1:目录 2:菜单
        data.backgroundColor : 背景色
        data.icon : 图标
        isCollapse: 是否折叠
        data.name : 菜单名称
        data.route : 路由
        data.icon : 图标
        data.children : 子菜单
        data.level : 1:一级菜单 2:二级菜单 3:三级菜单
    -->
<template>
        <el-sub-menu v-if="data.menuOwner !== 2 && data.nodeType == 1 && data.children?.length > 0 && data.level >= 0" :key="data.resourceId + 'el-sub-menu'" :index="String(data.resourceId)" :class="[menuScaleStatus ? 'menu-list-item-scale': 'menu-list-item']" :title="data.name" >
            <template #title >
                <div class="iconBox" :style="{ backgroundColor1: data.backgroundColor }" >
                    <i class="iconfont" :class="data.icon
                        ? data.icon
                        : data?.children?.length > 0 && 'icon' in data.children[0]
                            ? data.children[0].icon
                            : ''
                        "></i>
                </div>
                <label >{{ data.name }}</label>
            </template>
            <asideMenu v-for="item in data.children" :data="item" :key="item.resourceId" hasParent></asideMenu>
        </el-sub-menu>

        <el-menu-item  v-else-if="data.menuOwner !== 2 && data.nodeType == 2"  :name="data.resourceId" :key="data.resourceId + 'el-menu-item'" :index="data.route"
            @click="changeRoute(data.name, data.route)" :class="[menuScaleStatus ? 'menu-list-item-scale': 'menu-list-item']" :title="data.level == 1?data.name:''">
                <!-- <span v-if="hasParent || !data.icon" class="menuDot"></span>v-else  -->
                <div class="iconBox" :style="{ backgroundColor1: data.backgroundColor }" >
                    <i class="iconfont" :class="data.icon"></i>
                </div>
                <label >{{ data.name }}</label>
        </el-menu-item>

</template>
<script>
export default {
    name: 'asideMenu',
    props: {
        data: {
            type: Object,
            default: () => ({}),
        },
        isCollapse: {
            type: Boolean,
            default: false
        },
        hasParent: {
            type: Boolean,
            default: false
        }
    },
    data () {
        return {
            width: 0,
            showToolTip: true,
            firstMeasure: true,
            menuScaleStatus: true
        }
    },
    methods: {
        changeRoute (name, route) {
            this.$bus.emit('jumpUrl', route)
        }
    },
    watch: {
        isCollapse: {
            handler: function(newVal, oldVal){
                this.menuScaleStatus = newVal;
            },
            immediate: true
        }
    }
}
</script>

<style lang="scss">

// 展开
.maxActive .max{
    .iconBox {
        display: flex;
        justify-content: center;
        align-items: center;
        border-radius: 6px;
        .iconfont {
            width: auto !important;
            margin-right: 0px !important;
        }
    }
    >.el-menu{
        >.el-menu-item,>.el-sub-menu>.el-sub-menu__title{
            >label{font-size: 16px;}
            >.iconBox{
                margin-left: -9px;
                margin-right: 13px;
                >i{
                    font-size: 16px;
                }
            }
        }
        >.el-sub-menu>.el-menu{
            .el-menu-item,.el-sub-menu__title{
                label{
                    font-size: 14px;
                }
                .iconBox{
                    margin-right: 9px;
                    >i{
                        font-size: 14px;
                    }
                }
            }
        }
        
    }
}
.max .el-menu .el-menu-item.is-active{
    >.iconBox{
            i{color: var(--menu-color__active)!important;}
        }
}
.menu-list-item .el-menu-item.is-active {
    background-color: var(--menu-background__active)!important;
    color: var(--menu-color__active)!important;
        >.iconBox{
            i{color: var(--menu-color__active)!important;}
        }
    }
.menu-list-item{
   font-size: 16px;
   border-radius: 8px;
   cursor: pointer;
   min-width: 180px;
   .el-sub-menu__title{
        label{font-size: 16px;}
        border-radius: 8px;
        padding-left: 6!important;
        &:hover{
            background-color: var(--menu-background__hover);
            color: var(--menu-color__hover);
        }
        .el-sub-menu__icon-arrow{
            margin-right: -10px;
        }
   }
   .menuDot{
        display: none!important;
    }
   >label,.el-sub-menu__title>label{
    //    padding-left: 10px;
       cursor: pointer;
   }
   .el-menu-item{
        height: 24px;
        line-height: 24px;
        border-radius: 8px;
        cursor: pointer;
        padding: 0;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        &:hover{
            background-color: var(--menu-background__hover);
            color: var(--menu-color__hover);
        }
    }

    .menu-list-level{
        display: flex;
        width: 100%;
        flex-wrap: wrap;
        gap: 10px;
        padding-left: 35px;
        >p{
            width: 100%;
            display: flex;
            margin: 10px 0 0px -35px;
            label{padding-left: 10px;opacity: 0.3;}
        }
        >li{
           display: inline-block;
           white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }
        .menu-list-level{
            padding-left: 35px;
        }
    }
}
</style>
