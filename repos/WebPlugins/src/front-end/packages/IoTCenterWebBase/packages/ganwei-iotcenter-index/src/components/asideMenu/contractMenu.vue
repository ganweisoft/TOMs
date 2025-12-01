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
        <el-sub-menu v-if="data.menuOwner !== 2 && data.nodeType == 1 && data.children?.length > 0 && data.level === menuLevel" :key="data.resourceId + 'el-sub-menu'" :index="String(data.resourceId)" class="menu-list-item-scale" :title="data.name" >
            <template #title >
                <div class="iconBox" :style="{ backgroundColor: data.backgroundColor }" >
                    <i class="iconfont" :class="data.icon
                        ? data.icon
                        : data?.children?.length > 0 && 'icon' in data.children[0]
                            ? data.children[0].icon
                            : ''
                        "></i>
                </div>
            </template>
            <contractMenu v-for="item in data.children" :data="item" :key="item.resourceId" hasParent></contractMenu>
        </el-sub-menu>

        <!-- 2-3-N级菜单 -->
        <div v-else-if="data.menuOwner !== 2 && data.nodeType == 1 && data.level > menuLevel" class="menu-list-level">
            <p>
                <span class="iconBox" :style="{ backgroundColor: data.backgroundColor }" >
                        <i class="iconfont" :class="data.icon
                            ? data.icon
                            : data?.children?.length > 0 && 'icon' in data.children[0]
                                ? data.children[0].icon
                                : ''
                            "></i>
                </span>
                <label>{{ data.name }}</label>
            </p>
            <contractMenu v-for="item in data.children" :data="item" :key="item.resourceId" hasParent></contractMenu>
        </div>

        <el-menu-item  v-else-if="data.menuOwner !== 2 && data.nodeType == 2"  :name="data.resourceId" :key="data.resourceId + 'el-menu-item'" :index="data.route"
            @click="changeRoute(data.name, data.route)" class="menu-list-item-scale" :title="data.level == 1?data.name:''">
                <span v-if="hasParent || !data.icon" class="menuDot"></span>
                <div v-else class="iconBox" :style="{ backgroundColor: data.backgroundColor }" >
                    <i class="iconfont" :class="data.icon"></i>
                </div>
                <label>{{ data.name }}</label>
        </el-menu-item>

</template>
<script>
export default {
    name: 'contractMenu',
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
            menuLevel: 1
        }
    },
    mounted () {
        try{
            let configData = JSON.parse(sessionStorage.configInfoData)
            this.menuLevel = configData?.showTopNav ? 1 : 0
        } catch (error) {
            console.log(error)
        }
    },
    methods: {
        changeRoute (name, route) {
            this.$bus.emit('jumpUrl', route)
        }
    }
}
</script>

<style lang="scss">

// 缩起
.minActive {
.el-menu--collapse{
    .menu-list-item-scale {
        position: relative;
        label{
        padding-left: 0px!important;
        }
        &:hover{
            background-color: transparent!important;
        }
        label{
            display: none;
        }
        &.is-active{
            background-color: transparent !important;
            &::after{
                content: "";
                position: absolute;
                right: -20px;
                top: 0;
                width: 1px;
                height: 100%;
                border-right: 3px solid var(--menu-background__active);
            }
        }
        .iconBox{
            min-width: 32px;
            width: 32px !important;
            height: 32px !important;
            border-radius: 8px;
            display: flex;
            justify-content: center;
            align-items: center;
            &:hover .iconfont{
                font-size: 18px!important;
            }
            .iconfont {
                color: white !important;
                font-size: 16px;
                margin: 0;
            }
        }
    }
    >li{
        margin-bottom: 10px;
    }
}
}

.newMenuMinActive{
    padding: 20px;
    .menuDot{
        display: none!important;
    }
    .el-menu--popup{
        min-width: 50px!important;
        .menu-list-item-scale{
            padding-right: 10px;
            height: 24px;
            line-height: 24px;
            border-radius: 8px;
            background-color: transparent!important;
            cursor: pointer;
            padding: 0;
            label{
                width: auto !important;
                cursor: pointer;
            }
        }
    }
    .el-menu{
        box-shadow: none !important;
        display: flex;
        gap: 10px;
        flex-wrap: wrap;
    }

    .el-menu-item:hover{
        color: var(--menu-background__active);
        background-color: transparent;
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
        .iconBox{
            display: flex;
            width: 24px;
            height: 24px;
            text-align: center;
            justify-content: center;
            align-items: center;
            border-radius: 6px;
            color: white;
        }
    }
    .is-active label{
        color: var(--menu-background__active);
    }
}

</style>
