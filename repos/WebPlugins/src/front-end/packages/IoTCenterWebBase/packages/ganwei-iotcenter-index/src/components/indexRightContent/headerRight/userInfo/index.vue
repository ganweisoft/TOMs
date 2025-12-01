<template>
    <div class="userInfo">
        <el-dropdown class="index-header-right">
            <el-avatar :src="theme.user" shape="square"></el-avatar>
            <i class="iconfont icon-xialasanjiao"></i>
            <template #dropdown>
                <el-dropdown-menu style="padding: 10px">
                    <div class="user">
                        <el-avatar :src="theme.user" shape="square"></el-avatar>
                        <span>{{ loginUsername }}</span>
                    </div>

                    <p @click.stop.prevent="openDialog('showLogoutDialog')" type="default" style="cursor: pointer" v-if="config?.showTopBtn?.logout || config?.showTopBtn?.logout === undefined">
                        <span class="iconBox logout">
                            <i class="iconfont icon-tuichu"></i>
                        </span>
                        <span>
                            {{ $t('login.framePro.button.exitLogin') }}
                        </span>
                    </p>
                </el-dropdown-menu>
            </template>
        </el-dropdown>

        <logoutDialog :showDialog="showLogoutDialog" @closeDialog="closeDialog('showLogoutDialog')" />
        <!-- <PWEditDialog :passwordPolicy="passwordPolicy" :showDialog="showPWEditDialog"
            @closeDialog="closeDialog('showPWEditDialog')" /> -->
    </div>
</template>

<script src="./index.js"></script>

<style src="./index.scss"></style>
