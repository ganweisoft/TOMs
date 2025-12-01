<template>
    <div class="upgradeBox">
        <div @click="openElDrawer()" class="upgradeBox-btn" >
            <img style="width: 28px;" :src="`/static/images/index-upgrade.svg`" />
            <span>{{$t('login.tips.upgrade') }}</span>
        </div>
        <el-drawer v-model="drawer" :size="424" class="upgradeBox-drawer" append-to-body>
            <template #header>
                <label class="titleClass">
                    <span>{{$t('login.tips.pluginUpgrade') }}</span> 
                    <el-button type="primary" @click="upgradePlugin()" :class="{'is-loading': upgradeLoading}">{{$t('login.tips.onetouchUpgrade') }}</el-button>
                </label>
            </template>
            <ul class="upgradeBox-item">
                <li v-for="item in upgradeList" :key="item.pluginId">
                    <h4>
                        <span>{{ formatName(item.product) }}</span>
                        <label>{{ item.product }}[{{ item.packageVersion }}]</label>
                    </h4>
                    <p>
                        <span>{{item.memo}}</span>
                        <el-button v-if="!upgradeLoading" type="primary" @click="upgradePlugin(item)" :class="{'is-loading': item.id == upgradeBtnLoading}">{{$t('login.tips.upgrade') }}</el-button>
                    </p>
                </li>
            </ul>
        </el-drawer>
    </div>
</template>
<script src="./index.js"></script>
<style src="./index.scss"></style>
