<template>
    <div class="normalPage">
        <div class="breadcrumb">
            <el-breadcrumb separator-class="el-icon-arrow-right">
                <el-breadcrumb-item>{{ $t('login.framePro.label.home') }}</el-breadcrumb-item>
                <el-breadcrumb-item v-for="(item, i) in breadcrumbList" :key="i">{{
                    $t(item.title || '') }}</el-breadcrumb-item>
            </el-breadcrumb>
        </div>
        <el-main class="container-main noLabelPage" id="pane-normalPage">
            <transition name="slide-fade" class="wh100">
                <Suspense>
                    <router-view :key="$route.fullPath"></router-view>
                </Suspense>
            </transition>
        </el-main>
    </div>
</template>

<script src="./index.js"></script>
<style src="./index.scss" scoped></style>
