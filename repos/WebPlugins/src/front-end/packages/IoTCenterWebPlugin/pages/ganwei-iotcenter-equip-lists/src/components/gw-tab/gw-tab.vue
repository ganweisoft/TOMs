
<template>
    <div class="gw-tab">
        <div class="tab_btn">
            <span class="tab" :class="{ onTabNavClass: tabNavNumber == index + 1 }" v-for="(item, index) in tabNames" :key="index" @click="onTabNav(index + 1)">
                <span class="leftBox"></span>
                <span class="content">{{ item }}</span>
                <span class="rightBox"></span>
            </span>
            <!-- <filterColumn :columnList="columnList[tabNavNumber - 1]" :tabNavNumber="tabNavNumber"></filterColumn> -->
        </div>
        <div class="tab_content" v-loading="loading">
            <slot v-for="(item, index) in tabNames" v-if="tabNavNumber == index + 1"  :name="index + 1"></slot>
        </div>
    </div>
</template>
<style lang="scss" src="./gw-tab.scss"></style>
<script src="./gw-tab.js"></script>
