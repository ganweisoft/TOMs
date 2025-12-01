<template>
    <div class="safeModeBox" v-if="hasGetData">
        <div :class="'safeModeContainer'+safeMode">
            <span class="safeMode" v-if="safeTipsText && safeMode == 2"><label :class="'safeMode' + safeMode">{{
                $t(safeTipsText) }}</label>
            </span>
            <el-tooltip popper-class="topToolTip" :content="$t('login.framePro.tips.runMode')" placement="bottom">
                <div>
                    <img style="width: 28px;" v-if="safeMode == 2" :src="`/static/images/index-normalIcon-${theme.value || 'dark'}.svg`" alt=""
                        @click.stop.prevent="setSafeMode()" />
                    <img style="width: 20px;" v-if="safeMode == 1" :src="`/static/images/index-alarmIcon-${theme.value || 'dark'}.svg`" alt=""
                        @click.stop.prevent="setSafeMode()" />
                </div>
            </el-tooltip>
            <span class="safeMode" v-if="safeTipsText && safeMode == 1"><label :class="'safeMode' + safeMode">{{
                $t(safeTipsText) }}</label>
            </span>
        </div>
    </div>
</template>
<script src="./index.js"></script>
<style src="./index.scss"></style>
