<template>
    <el-main class="container-main">
        <el-tabs @contextmenu.prevent="openContextMenu($event)" v-model="editableTabsValue" type="card" closable
            @tab-remove="removeTab" @tab-click="tabClick" class="tabs-page-container">
            <el-tab-pane v-for="(item, index) in editableTabs" :key="item.route" :label="item.title"
                :disabled="item.disabled" :name="item.name">
                <template v-slot:label>
                    <i :class="'iconfont ' + item.icon"></i>
                    <span ref="tab-title" :res="index">{{ item.title }}</span>
                </template>
                <Suspense>
                     <!-- 通过/Index/jumpIframe/  **  跳转 frame -->
                    <router-view v-slot="{ Component }" :key="item.route" :editableTabs="editableTabs" :editableTabsValue="editableTabsValue">
                        <component :ref="item.route" :is="Component" />
                    </router-view>
                </Suspense>
            </el-tab-pane>
        </el-tabs>
        <span class="tabs-refresh">
            <el-tooltip
                        class="box-item"
                        effect="dark"
                        :content="$t('login.contextMenu.reload')"
                        placement="bottom"
                    >
                    <el-icon  @click.stop.prevent="refresh"
                class="active" >
                <RefreshRight />
            </el-icon>
            </el-tooltip>
        </span>
    </el-main>
</template>

<script src="./index.js"></script>
<style src="./index.scss" scoped></style>
