<template>
    <div>
        <el-tooltip popper-class="topToolTip" :content="$t('login.framePro.tips.fullScreen')" placement="bottom">
            <img v-if="n % 2 == 0" :src="theme.fullScreen" alt="" @click.stop.prevent="setFullCreen()" />
            <img v-else :src="theme.outFullScreeen" @click.stop.prevent="signOutFullCreen()" alt="" />
        </el-tooltip>
    </div>
</template>

<script src="./index.js"></script>
