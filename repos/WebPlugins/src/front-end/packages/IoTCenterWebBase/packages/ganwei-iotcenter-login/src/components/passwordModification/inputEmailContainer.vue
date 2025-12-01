<template>
    <!-- 输入邮箱 -->
    <div class="inputEmailContainer" >
        <b>{{$t('login.tips.retrievePassword')}}</b>
        <p>{{$t('login.tips.retrievePasswordTips')}}</p>
        <div class="inpuContent">
            <el-input v-model="emailName" @keyup.enter.native="confirmEmail"
                :placeholder="$t('login.input.inputEmail')" autocomplete="off" @input="listenerStatus" clearable>
                <template #prefix>
                    <i class="iconfont icon-gw-icon-youxiang"></i>
                </template>
            </el-input>
            <div class="verificationCode">
                    <el-input v-model="verificationCode" @keyup.enter.native="nextStep" @input="listenerStatus"
                        :placeholder="$t('login.input.inputCode')" autocomplete="off" >
                        <template #prefix>
                            <i class="iconfont icon-denglu_yanzhengma"></i>
                        </template>
                        <template #suffix>
                            <img @click="drawCode()" :src="imgSrc" style="cursor: pointer;" :loading="errorLoading"/>
                        </template>
                    </el-input>
            </div>
        </div>
        <div class="dialog-footer">
            <el-button size="small" type="primary" @click="nextStep" class="nextStep" :class="nextStatus?'allowStatus':'prohibitStatus'" :loading="nextLoadingStatus">{{
                $t('login.button.nextStep') }}</el-button>
        </div>
    </div>
</template>

<script lang="ts" setup>
import { getCurrentInstance, ref,defineEmits  } from 'vue'
const emailName = ref('')
const verificationCode = ref('')
const verificationKey = ref('')
const imgSrc = ref('')
const errorLoading = ref(false)
const nextStatus = ref(false)
const nextLoadingStatus = ref(false)
const emit = defineEmits(['update-message'])
const instance = getCurrentInstance()
const timeout = ref('')

if(window.sessionStorage.eamilUrl) {
    emailName.value = window.sessionStorage.eamilUrl
}

const confirmEmail =()=> {
    nextStep()
}

const nextStep=()=> {
    if(!nextStatus.value) return;
    if(isEmail(emailName.value)) {
        // 发送API 逻辑处理
        validateEmailCode();
    } else{
        instance.proxy.$message.error(instance.proxy.$t('login.tips.correctEmail'))
    }
}

const isEmail=(email)=>  {
    const emailRegExp = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailRegExp.test(email);
}

function listenerStatus(){
    if(emailName.value && verificationCode.value ) {
        nextStatus.value = true
    }
}

function drawCode(){
    errorLoading.value = true
    instance.proxy.$api
        .getImageCaptcha()
        .then(res => {
            const { code, data, message } = res?.data || {}
            if (code == 200) {
                imgSrc.value = data?.verificationCode || ''
                verificationCode.value = ''
                verificationKey.value = data?.verificationKey || ''
                nextStatus.value = false
            } else {
                instance.proxy.$message.error(message)
            }
        })
        .catch(err => {
            instance.proxy.$message.error(err?.data, err)
        }).finally(r=>{
            errorLoading.value = false
        })
}

drawCode();

function validateEmailCode(){
    nextLoadingStatus.value = true
    let data = {
        "verificationKey": verificationKey.value,
        "email": instance.proxy.$getCode.RSAEncrypt(emailName.value),
        "verificationCode": verificationCode.value
    }
    instance.proxy.$api
        .ValidateImageCaptchaAndSendEmail(data)
        .then(res => {
            const { code, data, message } = res?.data || {}
            if (code == 200) {
                // 发送成功进入下一个页面
                emit('update-message');
                window.sessionStorage.eamilUrl = emailName.value
            } else {
                verificationCode.value = ''
                drawCode()
                instance.proxy.$message.error(message)
            }
        })
        .catch(err => {
            verificationCode.value = ''
            drawCode()
            instance.proxy.$message.error(err?.data, err)
        }).finally(r=>{
            nextLoadingStatus.value = false
        })
}

</script>
