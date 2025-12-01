<template>
    <div>
        <div class="themeImage" style="margin-right: 0px">
            <img :src="theme.skin" alt="" @click="showDialog"/>
        </div>
        <el-dialog :title="$t('login.framePro.theme.themeTitle')" v-model="dialogVisible" center :close-on-click-modal="false"
         width="420px" class="selectedTheme" top="25vh" append-to="#index" >
            <div class="themeContainer">
                <div class="themeOptionCard" v-for="(item, index) in themeList" :key="index"
                    :class="{ 'is-active': item.value == selectedTheme, 'noPadding': (index + 1) % 3 == 0 }"  @click="changeTheme(item.value)">
                    <div class="optionBox" >
                        <i :style="{background: item.color}"></i>
                        <p>{{  $t('login.framePro.theme.supportThemes.' + item.value) }}</p>
                    </div>
                    <span></span>
                    <label>
                        ✔
                    </label>
                </div>
            </div>
            <template #footer>
                <span class="dialog-footer">
                    <el-button type="primary" plain @click.stop.prevent="closeDialog">{{
            $t('login.publics.button.cancel') }}</el-button>
                    <el-button type="primary" :loading="saveLoading" @click.stop.prevent="saveTheme()">{{
            $t('login.publics.button.confirm') }}
                    </el-button>
                </span>
            </template>
    </el-dialog>
  </div>
</template>

<script src="./index.js"></script>
<style src="./index.scss"></style>
