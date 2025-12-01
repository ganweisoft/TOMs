<template>
    <el-dialog :title="$t('login.framePro.title.safeMode')" v-model="dialogVisible" center :close-on-click-modal="false"
        @closed="closeDialog" width="600px" class="safetyLevel" top="0vh" append-to="#index">
        <div class="outContainer">
            <div class="optionCard" v-for="(item, index) in safetyLevelList" :key="index"
                :class="{ 'is-active': item.value == safeLevel }" @click="changeType(item)">
                <div class="optionBox">
                    <div class="optionBoxLeft">
                        <img v-if="item.value == 2"
                            :src="`/static/images/index-normalIcon-${theme.value || 'dark'}.svg`" alt="" />
                        <img v-if="item.value == 1" :src="`/static/images/index-alarmIcon-${theme.value || 'dark'}.svg`"
                            alt="" />
                    </div>
                    <div class="optionBoxRight">
                        <div class="label">
                            {{ item.label }}
                        </div>
                        <div class="describe">{{ item.describe }}</div>
                    </div>
                </div>
                <div class="check" v-show="item.value == safeLevel">
                    <el-icon>
                        <Check />
                    </el-icon>
                </div>
            </div>
        </div>
        <template #footer>
            <span class="dialog-footer">
                <el-button type="primary" plain @click.stop.prevent="closeDialog">{{
        $t('login.publics.button.cancel') }}</el-button>
                <el-button type="primary" :loading="saveLoading" @click.stop.prevent="setSafeLevel()">{{
        $t('login.publics.button.confirm') }}
                </el-button>
            </span>
        </template>
    </el-dialog>
</template>

<script>
export default {
    name: 'SafeModeDialog',
    props: {
        showDialog: {
            type: Boolean,
            default: false
        }
    },
    inject: ['theme'],
    data () {
        return {
            dialogVisible: false,
            safeLevel: 1,
            saveLoading: false
        }
    },
    watch: {
        showDialog (val) {
            if (val) {
                this.getCurrentSafeLevel()
            }
            this.dialogVisible = val;
        }
    },
    computed: {
        safetyLevelList () {
            return [
                {
                    label: this.$t('login.framePro.label.unSafeMode'),
                    value: '1',
                    describe: this.$t('login.framePro.tips.unsafeModeTip')
                },
                {
                    label: this.$t('login.framePro.label.safeMode'),
                    value: '2',
                    describe: this.$t('login.framePro.tips.safeModeTip')
                }
            ]
        },
    },
    methods: {
        changeType (item) {
            this.safeLevel = item.value;
        },
        getCurrentSafeLevel () {
            this.$api
                .getSafeLevelByGateway()
                .then(rt => {
                    if (rt?.data?.code == 200) {
                        this.safeLevel = rt?.data?.data || 1
                    }
                })
                .catch(e => {
                    console.log(e)
                })
        },
        setSafeLevel () {
            this.saveLoading = true
            this.$api.setSafeLevel(this.safeLevel).then(res => {
                if (res.data.code === 200) {
                    this.$message.success(res.data.message)
                    this.closeDialog()
                    window.postMessage({ updateSafeMode: true }, '*')
                } else {
                    this.$message.error(res.data.message)
                    this.safeLevel = ''
                }
            }).finally(() => {
                this.saveLoading = false
            })
        },
        closeDialog () {
            this.$emit('closeDialog')
        }
    }
}
</script>

<style lang="scss">
.outContainer {
    display: flex;

    .optionCard {
        &:first-child {
            margin-right: 20px;
        }

        .optionBox {
            display: flex;
            justify-content: flex-start;
            align-items: center;

            img {
                height: 60px;
                width: 60px;
                margin-right: 12px;
            }
        }

        width: 48%;
        padding: 10px;
        border: 1px solid var(--frame-main-border);
        cursor: pointer;

        .label {
            color: #3874f7;
            font-size: 15px;
            line-height: 15px;
            margin-bottom: 5px;
        }

        .describe {
            color: var(--frame-main-color);
            font-size: 12px;
            line-height: 1.5;
            width: 100%;
            white-space: pre-wrap;
        }
    }

    .is-active {
        border-color: #3875ff;
        position: relative;

        .check {
            position: absolute;
            bottom: -1px;
            right: -1px;
            width: 26px;
            height: 26px;
            border: 1px solid #3875ff;
            border-right-width: 0;
            border-bottom-width: 0;
            border-radius: 50px 0 0 0;
            background-color: #3875ff;
            text-align: center;
            line-height: 29px;
            color: #fff;
        }
    }

}

::v-deep .safetyLevel {
    .el-dialog__header {
        display: none !important;
    }

    .el-dialog__body {
        height: 224px;
        padding-top: 0;
        padding-bottom: 0;
    }
}
</style>
