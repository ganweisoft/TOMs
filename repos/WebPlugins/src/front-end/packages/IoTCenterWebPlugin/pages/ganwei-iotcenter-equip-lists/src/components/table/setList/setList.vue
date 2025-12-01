<template>
    <ul class="setList">
        <li @click="onSetOperate(0, item)" class="link_set" :title="item.setNm" v-for="(item, i) in tableData" :key="'item.setNo' + i">
            <span>{{ item.setNm }}</span>
        </li>
        <div class="noDataTips" :data-noData="$t('equipListsIot.publics.noData')" v-if="tableData.length == 0 && !loading"></div>
    </ul>
</template>
<style lang="scss" src="./setList.scss">
</style>
<script src="./setList.js"></script>
