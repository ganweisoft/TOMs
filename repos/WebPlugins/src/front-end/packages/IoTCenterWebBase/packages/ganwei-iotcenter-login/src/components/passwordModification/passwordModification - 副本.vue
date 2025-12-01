
<el-button size="small" type="primary" @click="checkNextStep" class="nextStep" :class="remainingTime == 0?'allowStatus':'prohibitStatus'">{{
    $t('login.tips.retrieveAgainCode') }} | {{remainingTime}}s</el-button>

