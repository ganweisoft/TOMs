<template>
    <el-date-picker v-bind="attrs" :default-time="defaultTime" :value-format="valueFormat" ref="proxyElement">
        <template v-for="(value, key) in $slots" #[key]="slotProps">
            <slot :name="key" v-bind="slotProps"  :key="key"></slot>
        </template>
    </el-date-picker>
</template>

<script setup lang="ts">
import { reactive, ref, useAttrs } from 'vue'
import { ElDatePicker } from 'element-plus'
import useExpose from '../../hooks/useExpose'

const proxyElement = ref()
const expose = useExpose(proxyElement)
defineExpose(expose)

type DateType = string | Date

interface IDatePickerProps {
    'defaultTime': DateType | [DateType, DateType],
    'valueFormat': string,
}

const props = defineProps<IDatePickerProps>()
const attrs = useAttrs() as unknown as IDatePickerProps

const defaultTime = parseDate(props.defaultTime)
const valueFormat = props.valueFormat || 'YYYY-MM-DD HH:mm:ss'

function parseDate(defaultTime: DateType): Date;
function parseDate(defaultTime: [DateType, DateType]): [ Date, Date ];
function parseDate(defaultTime: DateType | [DateType, DateType]): Date | [ Date, Date ];
function parseDate(defaultTime: DateType | [DateType, DateType]): Date | [ Date, Date ] | undefined {
    if(!defaultTime) {
        return undefined
    }
    if(Array.isArray(defaultTime)) {
        if(defaultTime[0] && defaultTime[1]) {
            return [parseDate(defaultTime[0]), parseDate(defaultTime[1])]
        }
        return [new Date(2000, 1, 1, 0, 0, 0), new Date(2000, 1, 1, 23, 59, 59)]
    }

    if(defaultTime instanceof Date) {
        return defaultTime
    }
        const [h = 0, m = 0, s = 0] = defaultTime.split(':')
        return new Date(2000, 1, 1, Number(h), Number(m), Number(s))

}
</script>

<style lang="scss">
.el-popper.el-picker__popper {
    background-color: var(--datepicker-background);
    border-color: var(--datepicker-border);

    &.is-light .el-popper__arrow::before {
        background-color: var(--datepicker-background);
    }

    .el-picker-panel__body-wrapper,
    .el-picker-panel__footer,
    .el-picker-panel__sidebar {
        color: var(--datepicker-color);
        background-color: var(--datepicker-background);
        border-color: var(--datepicker-border);
    }

    .el-picker-panel__shortcut, .el-date-picker__header-label,.el-icon {
        color: var(--datepicker-color);
    }

    .el-date-table th {
        font-size: 16px;
        font-weight: bold;
        color: var(--datepicker-color);

        background-color: var(--datepicker--table-header-color);
        border-bottom-color: transparent;

        &:first-child {
            border-top-left-radius: 3px;
            border-bottom-left-radius: 3px;
        }

        &:last-child {
            border-top-right-radius: 3px;
            border-bottom-right-radius: 3px;
        }
    }

    .el-date-table td {
        &.disabled .el-date-table-cell {
            background: var(--datepicker-month-background__disabled);
        }

        &.prev-month, &.next-month, &.disabled {
            opacity: .32;
        }

        &.current:not(.disabled) .el-date-table-cell__text {
            border-radius: 3px;
        }

        .el-date-table-cell__text {
            font-size: 16px;
            font-weight: bold;
        }
    }

    .el-picker-panel__footer .el-picker-panel__link-btn, .el-time-panel__btn {
        color: var(--datepicker-color);
        background: transparent;
        border: unset;

        &:last-child {
            color: var(--datepicker-dooter-button-color__primary);
        }
    }

    .el-date-picker__time-header {
        border-bottom-color: var(--datepicker-border);
    }

    .el-time-panel {
        background-color: var(--datepicker-background);
        border-color: var(--datepicker-border);

        .el-time-spinner__item, .el-time-spinner__item.is-active:not(.is-disabled) {
            color: var(--datepicker-color);

            &:hover {
                background-color: var(--datepicker-color__hover);
            }
        }

        .el-time-panel__content.has-seconds::before {
            border-color: var(--datepicker-border);
        }

        .el-time-panel__footer {
            border-top-color: var(--datepicker-border);
        }
    }
}
.el-date-editor {
    &.el-input__wrapper {
        box-shadow: 0 0 0 1px var(--input-border) inset;

        .el-range-input {
            color: var(--input-color);
        }
    }

    .el-range-separator {
        color: var(--input-color);
    }
}
</style>
