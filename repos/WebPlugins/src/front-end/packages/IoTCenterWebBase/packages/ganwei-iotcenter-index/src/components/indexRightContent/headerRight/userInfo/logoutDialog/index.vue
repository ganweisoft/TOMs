<template>
    <el-dialog id="quit" :title="$t('login.framePro.tips.sureToLogOut')" v-model="dialogVisible" width="350px" top="30vh"
        center append-to="#index">
        <template #footer>
            <span class="dialog-footer">
                <el-button type="primary" plain @click="closeDialog">{{
        $t('login.publics.button.cancel') }}</el-button>
                <el-button @click.stop.prevent="quitLogin" type="danger" :loading="submitLoading"
                    style="color: #ffffff"> {{
        $t('login.publics.button.confirm') }}</el-button>
            </span>
        </template>
    </el-dialog>
</template>

<script src="./index.js"></script>
