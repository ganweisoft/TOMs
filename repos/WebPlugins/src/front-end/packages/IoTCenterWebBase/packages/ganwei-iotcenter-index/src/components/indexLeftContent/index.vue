<template>
    <div :class="{ isCollapse: isCollapse, noIsCollapse: !isCollapse, transition: transition }">
        <!-- 全量菜单 -->
        <div class="maxW maxActive" :class="{ transition: transition, showOrganizationalStructure: showOrganizationalStructure}" id="menuRef">

            <div class="aside-header">
                <div class="aside-header-box">
                    <a style="cursor: pointer" @click="logoJump()">
                        <img :src="theme.logo" class="header-top-img" alt="" v-if="theme.logo" @error="showImg" />
                    </a>
                </div>
            </div>

            <div class="organizationalStructure" v-if="showOrganizationalStructure" :title="organizationName">
                {{organizationName}}
            </div>

            <div class="left-nav" @click.stop>
                <el-row class="list">
                    <div class="max">
                        <el-input v-model="menuSearch" @keyup.enter="searchMenu"
                            :placeholder="$t('login.framePro.tips.inputMenuName')" clearable>
                            <template #prefix>
                                <i class="iconfont icon-sousuoL"></i>
                            </template>
                        </el-input>
                        <!-- UL -->
                        <el-menu ref="menu" :router="false" :default-active="menuActive" :collapse="false"
                            :collapse-transition="false" unique-opened popper-class="newMenu newMenuMaxActive" >
                            <loading v-if="loading"></loading>
                            <asideMenu :isCollapse="isCollapse" v-else v-for="item in renderMenu" :data="item" :key="item.resourceId">
                            </asideMenu>
                        </el-menu>
                    </div>
                </el-row>
                <div class="fold">
                    <div @click.stop.prevent="onAsideListShow()">
                        <el-button>
                            <i :class="isCollapse ? 'iconfont icon-caidan_zhankai cacelmargin' : 'iconfont icon-caidan_zhankai isopen'"></i>
                        </el-button>
                        <span>{{ $t('login.menuJson.closeNavigationBar') }}</span>
                    </div>
                </div>
            </div>

            <widthSetting custom @resizeEnd="resizeEnd" v-if="!isCollapse" leftSide="menuRef" rightSide="contentRef">
            </widthSetting>
        </div>

        <!-- 缩放菜单 -->
        <div class="maxW minActive" :class="{ transition: transition }" >
            <div class="topIndexHeader"></div>
            <div class="minActiveContent">
                <div class="aside-header">
                    <div class="aside-header-box">
                        <a style="cursor: pointer">
                            <img class="min-img" :src="config.img.indexSmallImg" alt />
                        </a>
                    </div>
                </div>
                <div class="left-nav" @click.stop>
                    <el-row class="list">
                        <div class="max">
                            <el-menu ref="menu" :router="false" :default-active="menuActive" :collapse="true"
                                :collapse-transition="false" unique-opened popper-class="newMenu newMenuMinActive" teleported="false">
                                <contractMenu :isCollapse="isCollapse" v-for="item in renderMenu" :data="item"
                                    :key="item.resourceId"> </contractMenu>
                            </el-menu>
                        </div>
                    </el-row>
                    <div class="fold">
                        <div @click.stop.prevent="onAsideListShow()">
                            <el-button>
                                <i :class="isCollapse ? 'iconfont icon-caidan_zhankai cacelmargin' : 'iconfont icon-caidan_zhankai isopen'"></i>
                            </el-button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script src="./index"></script>

<style src="./index.scss" scoped></style>

<style lang="scss">
    // 新版菜单
    .newMenuMaxActive{
        left: 236px !important;
        .el-menu--popup{
            min-width: auto!important;
        }
    }
    .newMenuMinActive{
        left: 75px!important;
    }
    .newMenu{
        max-width: 597px;
        max-height: 600px;
        background: var(--menu-popup-background)!important;
        box-shadow: 0px 4px 14px 0px var(--menu-popup-boxshadow);
        border-radius: 10px;
        backdrop-filter: blur(6px);

        .el-menu--vertical{
            overflow-y: auto;
            overflow-x: hidden;
        }
    }
</style>
