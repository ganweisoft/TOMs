
<template>
    <!--验证邮箱-->
    <div class="checkEmailContainer" >
        <b>{{$t('login.input.inputEmailCode')}}</b>
        <p>{{$t('login.tips.emailCode')}} {{emailName}}</p>
        <span>
            <el-input v-model="checkEmailCode1" autocomplete="off" maxlength="1" id="elCode1" @input="(value)=>tabInput(value,1)" @focus="focusHandle(checkEmailCode1,1)" @blur="blurHandle(checkEmailCode7,1)"></el-input>
            <el-input v-model="checkEmailCode2" autocomplete="off" maxlength="1" id="elCode2" @input="(value)=>tabInput(value,2)" @focus="focusHandle(checkEmailCode2,2)" @blur="blurHandle(checkEmailCode7,2)"></el-input>
            <el-input v-model="checkEmailCode3" autocomplete="off" maxlength="1" id="elCode3" @input="(value)=>tabInput(value,3)" @focus="focusHandle(checkEmailCode3,3)" @blur="blurHandle(checkEmailCode7,3)"></el-input>
            <el-input v-model="checkEmailCode4" autocomplete="off" maxlength="1" id="elCode4" @input="(value)=>tabInput(value,4)" @focus="focusHandle(checkEmailCode4,4)" @blur="blurHandle(checkEmailCode7,4)"></el-input>
            <el-input v-model="checkEmailCode5" autocomplete="off" maxlength="1" id="elCode5" @input="(value)=>tabInput(value,5)" @focus="focusHandle(checkEmailCode5,5)" @blur="blurHandle(checkEmailCode7,5)"></el-input>
            <el-input v-model="checkEmailCode6" autocomplete="off" maxlength="1" id="elCode6" @input="(value)=>tabInput(value,6)" @focus="focusHandle(checkEmailCode6,6)" @blur="blurHandle(checkEmailCode7,6)"></el-input>
        </span>
        <div class="dialog-footer">
            <el-button size="small" @click="checkPrev" >{{
                $t('login.button.previousStep') }}</el-button>
            <el-button size="small" type="primary" @click="checkNextStep" class="nextStep" :class="fillStatus?'allowStatus':'prohibitStatus'" :Loading="nextLoadingStatus">{{
                $t('login.button.nextStep') }} </el-button>

        </div>
    </div>
</template>

<script lang="ts" setup>
import { getCurrentInstance,defineEmits, ref } from 'vue'
const checkEmailContainer = ref(false)
const checkEmailCode1 = ref('')
const checkEmailCode2 = ref('')
const checkEmailCode3 = ref('')
const checkEmailCode4 = ref('')
const checkEmailCode5 = ref('')
const checkEmailCode6 = ref('')
const checkEmailCode7 = ref('')
const fillStatus = ref(false)
const nextLoadingStatus = ref(false)
const emailName = ref('')
const remainingTime = ref(60)
const countdownInterval = ref('')
const emit = defineEmits(['update-prev','update-next'])
const instance = getCurrentInstance()

emailName.value = window.sessionStorage.eamilUrl
const checkPrev=()=> {
   emit('update-prev');
}

const checkNextStep=()=> {
    if(!fillStatus.value) return;
    // 发送API 逻辑处理
    authEmailCode();
}

const startCountdown=()=>  {
    countdownInterval.value = setInterval(() => {
        remainingTime.value--;
        if (remainingTime.value <= 0) {
            stopCountdown();
        }
    }, 1000);
}

const stopCountdown=()=>  {
    clearInterval(countdownInterval.value);
    countdownInterval.value = null;
}

function focusHandle(val,n){
  if(val) checkEmailCode7.value = val
  switch(n) {
    case 1: checkEmailCode1.value = '';break;
    case 2: checkEmailCode2.value = '';break;
    case 3: checkEmailCode3.value = '';break;
    case 4: checkEmailCode4.value = '';break;
    case 5: checkEmailCode5.value = '';break;
    case 6: checkEmailCode6.value = '';break;
    default: break;
  }
}
function blurHandle(val,n){
  switch(n) {
    case 1: if(checkEmailCode1.value == '') checkEmailCode1.value = val;break;
    case 2: if(checkEmailCode2.value == '') checkEmailCode2.value = val;break;
    case 3: if(checkEmailCode3.value == '') checkEmailCode3.value = val;break;
    case 4: if(checkEmailCode4.value == '') checkEmailCode4.value = val;break;
    case 5: if(checkEmailCode5.value == '') checkEmailCode5.value = val;break;
    case 6: if(checkEmailCode6.value == '') checkEmailCode6.value = val;break;
    default: break;
  }
}

function tabInput(val,n){
     if(val) {
        if(n<6) {
            let dom  = document.getElementById('elCode'+(n+1));
            dom.focus()
        }
        if(checkEmailCode1.value && checkEmailCode2.value && checkEmailCode3.value && checkEmailCode4.value && checkEmailCode5.value&& checkEmailCode6.value) {
            fillStatus.value = true;
        } else {
            fillStatus.value = false;
        }
     }
}

function authEmailCode(){

   nextLoadingStatus.value = true
   let data = {
       "email": instance.proxy.$getCode.RSAEncrypt(window.sessionStorage.eamilUrl),
       "emailCaptcha": checkEmailCode1.value+""+checkEmailCode2.value+""+checkEmailCode3.value+""+checkEmailCode4.value+""+checkEmailCode5.value+""+checkEmailCode6.value
   }
   instance.proxy.$api
       .ValidateEmailCaptcha(data)
       .then(res => {
           const { code, data, message } = res?.data || {}
           if (code == 200) {
               // 发送成功进入下一个页面
               emit('update-next');
           } else {
               instance.proxy.$message.error(message)
           }
       })
       .catch(err => {
           instance.proxy.$message.error(err?.data, err)
       }).finally(r=>{
           nextLoadingStatus.value = false
       })

}

startCountdown();
</script>

