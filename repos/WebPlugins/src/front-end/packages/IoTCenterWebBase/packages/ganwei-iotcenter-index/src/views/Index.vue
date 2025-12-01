
<template>
    <el-container id="index" :class="{onlyContent: onlyContent,isHideMenu: isHideMenu, isHideHead: isHideHead}">
        <div class="topIndexHeader"></div>
        <!-- 左侧区域 -->
        <indexLeftContent :navTopActive="navTopActive" :loading="loading" :allMenus="allMenus" />

        <!-- 右侧区域 -->
        <indexRightContent v-model="navTopActive" :allMenus="allMenus" />

    </el-container>
</template>

<script>
import index from "./index.js"
export default index;
</script>
<style lang="scss" src="@/assets/css/index.scss" scoped></style>
