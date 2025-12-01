<template>
    <el-dialog class="passwordContainer" modal-class="modalContainer" v-model="props.passwordDialogVisible" width="800" destroy-on-close @close="modifyNextStep" :close-on-click-modal="false" :close-on-press-escape="false">
      <inputEmailComponent v-if="inputEmailContainer" @update-message="updateInputEmailContainer"/>
      <checkEmailComponent v-if="checkEmailContainer" @update-prev="updateCheckEmailContainerPrev" @update-next="updateCheckEmailContainerNext"/>
      <modifyPasswordComponent v-if="modifyPasswordContainer" @update-end="updateModifyPasswordContainer"/>
    </el-dialog>
</template>

<script lang="ts" setup>
import { reactive, ref, watch,defineAsyncComponent} from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
const emit = defineEmits(['closePassword'])
const inputEmailContainer = ref(true)
const checkEmailContainer = ref(false)
const modifyPasswordContainer = ref(false)
const props = defineProps({
    passwordDialogVisible: {
        type: Boolean,
        default: false
    }
})
const inputEmailComponent = defineAsyncComponent(() => import('@/components/passwordModification/inputEmailContainer.vue'))
const checkEmailComponent = defineAsyncComponent(() => import('@/components/passwordModification/checkEmailContainer.vue'))
const modifyPasswordComponent = defineAsyncComponent(() => import('@/components/passwordModification/modifyPasswordContainer.vue'))

const updateInputEmailContainer=()=> {
    inputEmailContainer.value = false
    checkEmailContainer.value = true
}

const updateCheckEmailContainerPrev=()=> {
    inputEmailContainer.value = true
    checkEmailContainer.value = false
}
const updateCheckEmailContainerNext=()=> {
    checkEmailContainer.value = false
    modifyPasswordContainer.value = true
}

const updateModifyPasswordContainer=()=> {
     modifyNextStep()
}

const modifyNextStep = ()=>{
    emit('closePassword')
    inputEmailContainer.value = true
    checkEmailContainer.value = false
    modifyPasswordContainer.value = false
}


</script>

<style lang="scss">
    @mixin el-dialog {
        background-color: var(--dialog-bgColor);

        .el-dialog__header {
            height: 64px;
            padding: 23px 20px;
            // @include border_bottom(1px, "bor-default");
            text-align: left;

            .el-dialog__title,
            .dialogTitle span {
                font-size: 16px;
                font-family: "Microsoft YaHei";
            }
        }

        .el-dialog__body {
            font-size: 14px;
            font-family: "Microsoft YaHei";
            padding: 0px 20px !important;
        }

        .el-dialog__footer {
            height: 72px;
            border: none;
            display: flex;
            justify-content: flex-end;
            align-items: center;

            .dialog-footer {
                .el-button {
                    min-width: 92px;
                    height: 32px !important;
                    line-height: 32px;
                    font-size: 14px;
                    font-family: "Microsoft YaHei";
                    border-radius: 2px;
                }
            }
        }
    }
    // 搜索框
    @mixin searchInput {
        .el-input {
            width: 287px;

            .el-input__inner {
                height: 32px;
                background-color: var(--input-bgColor4);

                &::-webkit-input-placeholder {
                    font-size: 14px;
                }

            }
        }
    }
    .modalContainer{
        background-color: transparent;
    }
    .passwordContainer{
        @include el-dialog;
        @include searchInput;
         .el-dialog__header {
            .el-dialog__close{
                color: #333;
            }
         }

         .el-dialog__body>div{
            text-align: center;
            >b{
                width: 128px;
                height: 45px;
                font-family: PingFangSC, PingFang SC;
                font-weight: 500;
                font-size: 32px;
                color: #333333;
                line-height: 45px;
                text-align: left;
                font-style: normal;
            }
            >p{
                width: 100%;
                height: 22px;
                font-family: PingFangSC, PingFang SC;
                font-weight: 400;
                font-size: 16px;
                color: #555963;
                line-height: 22px;
                text-align: center;
                font-style: normal;
                margin: 12px 0px 36px 0;
            }
            .passwordLable{
                font-weight: 400;
                font-size: 16px;
                color: #555963;
                text-align: left;
            }

            .dialog-footer .el-button{
                justify-content: center;
                margin-top: 36px;
                margin-bottom: 130px;
                padding: 0 39px;
                height: 52px !important;
                font-size: 16px;
                border-radius: 26px !important;
                border: 1px solid rgba(151,151,151,0.21);
                backdrop-filter: blur(1px);
                span{
                    color: #2C69F1;
                }
            }
            .dialog-footer .nextStep{
                span{
                    color: white;
                }
            }
            .dialog-footer .prohibitStatus{
                 cursor: not-allowed;
            }
            .dialog-footer .allowStatus{
                 opacity: 1!important;
            }
         }
        .inputEmailContainer .inpuContent{
            >div{
                margin-bottom: 10px;
            }
            .el-input {
                width: 518px;
                height: 52px;
                border-radius: 39px;
                backdrop-filter: blur(1px);
                overflow: hidden;
                background-color: rgba(255,255,255, 1);
                border: 1px solid rgba(151,151,151,0.21);
                .el-input__wrapper,.el-input__inner{
                    text-indent: 10px;
                    border: 0;
                    height: 100%;
                    background-color: rgba(255,255,255, 1);
                    color: #333333 !important;
                    .icon-denglu_yanzhengma{font-size: 20px;}
                }
            }
        }
        .checkEmailContainer>span{
            .el-input {
                width: 52px;
                height: 64px;
                background: #FFFFFF;
                border-radius: 4px;
                border: 1px solid #CDE0EE;
                backdrop-filter: blur(1px);
                overflow: hidden;
                margin: 0 14px;
                .el-input__wrapper,.el-input__inner{
                    text-align: center;
                    border: 0;
                    padding: 0;
                    height: 100%;
                    background-color: rgba(255,255,255, 1);
                    font-size: 24px;
                    color: #333333 !important;
                }
            }
         }
         .modifyPasswordContainer{
            >b{
                margin-bottom: 20px;
            }
            .dialog-footer{
               .el-button{
                   background: #2464F1 !important;
               }
            }
            .el-form-item__error{
                margin-left: 24px;
            }
        }
        .modifyPasswordContainer>span{
            display: flex;
            justify-content: left;
            align-items: center;
            flex-direction: column;
            .el-form{

            }
            .el-input {
                width: 518px;
                height: 52px;
                border-radius: 39px;
                backdrop-filter: blur(1px);
                overflow: hidden;
                background-color: rgba(255,255,255, 1);
                border: 1px solid rgba(151,151,151,0.21);
                .el-input__wrapper,.el-input__inner{
                    text-indent: 10px;
                    border: 0;
                    height: 100%;
                    background-color: rgba(255,255,255, 1);
                    color: #333333 !important;
                }
                .el-input__suffix .el-input__validateIcon{
                   display: none;
                }
            }
            .el-form-item{
                margin-bottom: 25px;
            }
            .el-form-item__label{
                font-size: 18px;
                color: #333333 !important;
            }
        }
        background: linear-gradient( 328deg, rgba(254,255,255,0.25) 0%, rgba(234,239,251,0) 100%);
        border: 2px solid white;
        border-radius: 24px;
        overflow: hideen;
        backdrop-filter: blur(7px);
    }
</style>, watch
