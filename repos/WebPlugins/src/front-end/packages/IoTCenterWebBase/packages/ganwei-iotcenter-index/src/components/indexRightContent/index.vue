<template>
    <el-container id="contentRef">
        <!-- 顶部内容块 -->
        <el-header class="indexHeader">
            <top-nav ref="TopNav" v-model="navTopActive" @select="topNavSelect" v-if="config.showTopNav"
                :options="navTopOpt">
            </top-nav>
            <headerRight />
        </el-header>

        <!-- 底部内容块 -->
        <div class="indexBottomContent">
            <normalPage :allMenus="allMenus" v-if="!config.labelPage" />
            <labelPage :allMenus="allMenus" v-else />
        </div>
    </el-container>
</template>

<script src="./index.js"></script>
<style src="./index.scss" scoped></style>
