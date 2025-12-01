<template>
    <div class="parameters">
        <el-button type="primary" size="small" icon="el-icon-plus" @click="addParameter" :loading="loading"> {{ $tt('common.addParameter') }}</el-button>
        <el-form :model="parameters" label-position="left" ref="customAttribute">
            <div class="parameter" v-for="(item, index) in parameters" :key="index">
                <el-form-item class="inputBox" :label="$tt('common.parameterLabel')" :prop="`${index}.key`" :rules="customAttributeRules.keyRules">
                    <el-input size="small" :placeholder="getPlaceholder('common.parameterLabel', 'input')" v-model="item.key" @blur="inputKeyBlur(item)" clearable></el-input>
                </el-form-item>
                <el-form-item :label="$tt('common.parameterValue')" :prop="`${index}.value`" :rules="customAttributeRules.valueRules">
                    <el-input
                        size="small"
                        :placeholder="getPlaceholder('common.parameterValue', 'input')"
                        @blur="
                            () => {
                                item.value = item.value.trim()
                            }
                        "
                        clearable
                        v-model="item.value"
                    ></el-input>
                </el-form-item>
                <div class="delete" @click="deleteParameter(index)"><i class="el-icon-delete"></i></div>
            </div>
        </el-form>
    </div>
</template>

<script>
export default {
    model: {
        prop: 'parameters',
        event: 'change'
    },
    props: {
        parameters: {
            type: Array,
            default: () => []
        },
        getCustomPropData: {
            type: Object,
            default: () => ({})
        }
    },
    data() {
        return {
            loading: false
        }
    },
    watch: {
        getCustomPropData: {
            immediate: true,
            handler(val) {
                if (val && val.ids && val.ids.length > 0) {
                    this.getCustomProp(val)
                }
            }
        }
    },
    computed: {
        customAttributeRules() {
            return {
                keyRules: [
                    {
                        required: true,
                        trigger: 'blur',
                        validator: (rule, value, callback) => {
                            if (!value) {
                                callback(this.getPlaceholder('common.parameterLabel', 'input'))
                                return
                            }
                            let isHaveCtrl = this.parameters.filter(item => item.key == value).length > 1
                            if (isHaveCtrl) {
                                callback(this.$tt('common.repeatTip'))
                                return
                            }
                            callback()
                        }
                    }
                ],
                valueRules: [{ required: true, message: this.getPlaceholder('common.parameterValue', 'input'), trigger: 'blur' }]
            }
        }
    },
    methods: {
        $tt(string) {
            return this.$t(this.$route.name + '.' + string)
        },
        getPlaceholder(label, type) {
            if (type == 'select') {
                return this.$tt('tips.select') + this.$tt(label)
            }
            return this.$tt('tips.input') + this.$tt(label)
        },
        addParameter() {
            this.$refs.customAttribute.validate(valid => {
                if (valid) {
                    this.parameters.push({ key: '', value: '', repeatTip: '', nullTip: '', id: new Date().getTime() })
                    this.$emit('change', [...this.parameters])
                }
            })
        },
        deleteParameter(index) {
            this.parameters.splice(index, 1)
            this.$emit('change', [...this.parameters])
        },
        inputKeyBlur(item) {
            item.key = item.key.replace(/[^a-zA-Z0-9]/g, '')
            this.parameters.forEach((item, index) => {
                this.$refs.customAttribute.validateField(`${index}.key`, () => {})
            })
        },
        getCustomProp(data) {
            this.loading = true
            this.$api
                .getCustomProp(data)
                .then(res => {
                    if (res.data.code === 200) {
                        this.parameters = Object.entries(res.data.data).map(([key, value]) => {
                            return {
                                key,
                                value,
                                repeatTip: '',
                                nullTip: '',
                                id: new Date().getTime()
                            }
                        })
                        this.$emit('change', this.parameters)
                    } else {
                        this.$message.error(res.data.message, res)
                    }
                })
                .finally(() => {
                    this.loading = false
                })
        }
    }
}
</script>

<style lang="scss" scoped>
.parameters {
    margin-top: 20px;
    width: 100%;

    .el-form {
        padding-bottom: 20px;

        .el-form-item.is-error {
            margin-bottom: 0px !important;
        }

        .el-form-item__error {
            padding-top: 0px !important;
        }
    }

    .el-form-item {
        display: flex;
        flex-direction: row !important;
        justify-content: flex-start !important;
        align-items: center !important;

        .el-form-item__label {
            width: auto !important;
        }

        .el-form-item__content {
            width: 100% !important;
        }
    }

    .parameter {
        display: flex;
        justify-content: flex-start;
        align-items: center;
        margin-top: 12px;

        .label {
            color: var(--con-textColor1);
            line-height: 32px;
            flex-shrink: 0;
        }

        .inputBox {
            width: 40% !important;
            margin-right: 20px;

            .errTip {
                color: #ff4949;
            }
        }

        .valueBox {
            flex: 1;
        }

        .delete {
            height: 32px;
            color: #ff4949;
            border-radius: 6px;
            font-size: 20px;
            cursor: pointer;
        }
    }
}
</style>
