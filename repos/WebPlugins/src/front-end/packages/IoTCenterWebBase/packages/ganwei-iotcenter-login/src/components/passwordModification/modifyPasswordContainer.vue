<template>
        <!--密码修改 -->
        <div class="modifyPasswordContainer">
            <b>{{$t('login.publics.button.reset')+$t('login.framePro.label.password')}}</b>
            <p>{{emailName}}</p>
            <span>
                <el-form
                    ref="ruleFormRef"
                    style="max-width: 600px"
                    :model="ruleForm"
                    status-icon
                    :rules="rules"
                    label-width="auto"
                    class="demo-ruleForm"
                    label-position="top"
                >
                    <el-form-item :label="$t('login.tips.inputValidatePass')" prop="pass">
                      <el-input v-model="ruleForm.pass" type="password" :placeholder="$t('login.tips.inputValidatePass')" clearable show-password>
                        <template #prefix>
                            <i class="iconfont icon-denglu_mima"></i>
                        </template>
                      </el-input>
                    </el-form-item>
                    <el-form-item :label="$t('login.tips.inputValidatePass2')" prop="checkPass" >
                        <el-input
                            v-model="ruleForm.checkPass"
                            type="password"
                            :placeholder="$t('login.tips.inputValidatePass2')" clearable show-password
                        >
                                                <template #prefix>
                            <i class="iconfont icon-denglu_mima"></i>
                        </template>
                        </el-input>
                    </el-form-item>
                </el-form>
            </span>
            <div class="dialog-footer">
                <el-button size="small" type="primary" @click="submitForm(ruleFormRef)" class="nextStep" :disabled="comfirmed" :loading="comfirmLoading">{{
                    $t('login.publics.button.confirm') }}</el-button>
            </div>
        </div>

</template>

<script lang="ts" setup>
import { getCurrentInstance, reactive, ref, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
const ruleFormRef = ref<FormInstance>()
const modifyPasswordContainer = ref(false)
const comfirmed =ref(true)
const comfirmLoading = ref(false)
const instance = getCurrentInstance()
const emit = defineEmits(['update-end'])
const emailName = ref('')
emailName.value = window.sessionStorage.eamilUrl
const ruleForm = reactive({
  pass: '',
  checkPass: ''
})
const _this = getCurrentInstance().appContext.config.globalProperties
const validatePass = (rule: any, value: any, callback: any) => {
    comfirmed.value = true
  if (value === '') {
    callback(new Error(_this.$t('login.tips.validatePass')))
  } else {
    if (ruleForm.checkPass !== '') {
      if (!ruleFormRef.value) return
      ruleFormRef.value.validateField('checkPass')
    }
    callback()
  }
}
const validatePass2 = (rule: any, value: any, callback: any) => {
    comfirmed.value = true
  if (value === '') {
    callback(new Error(_this.$t('login.tips.validatePass')))
  } else if (value !== ruleForm.pass) {
    callback(new Error(_this.$t('login.tips.validatePass2')))
  } else {
    // 按钮可点击状态
    comfirmed.value = false
    callback()
  }
}

const rules = reactive<FormRules<typeof ruleForm>>({
  pass: [{ validator: validatePass, trigger: 'blur' }],
  checkPass: [{ validator: validatePass2, trigger: 'blur' }]
})


const submitForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate((valid) => {
    if (valid) {
      resetPassword();
      console.log('submit!')
    } else {
      console.log('error submit!')
    }
  })
}

const resetForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.resetFields()
}

function resetPassword(){
   comfirmLoading.value = true
   let data = {
    "email": instance.proxy.$getCode.RSAEncrypt(window.sessionStorage.eamilUrl),
    "password": instance.proxy.$getCode.RSAEncrypt(ruleForm.checkPass)
   }
   instance.proxy.$api
       .RetrievePassword(data)
       .then(res => {
           const { code, data, message } = res?.data || {}
           if (code == 200) {
               // 清除邮箱
               window.sessionStorage.removeItem("eamilUrl");
               // 发送成功进入下一个页面
               comfirmed.value = true
               instance.proxy.$message.success(message)
               emit('update-end');
           } else {
               instance.proxy.$message.error(message)
           }
       })
       .catch(err => {
           instance.proxy.$message.error(err?.data, err)
       }).finally(r=>{
           comfirmLoading.value = false
       })

}

</script>
