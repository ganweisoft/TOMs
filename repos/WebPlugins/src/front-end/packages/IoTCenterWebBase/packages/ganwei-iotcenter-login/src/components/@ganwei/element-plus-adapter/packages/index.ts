import { App } from 'vue'

import { default as ElButton } from './components/ElButton/index.vue'
import { default as ElCascader } from './components/ElCascader/index.vue'
import { default as ElCheckbox } from './components/ElCheckbox/index.vue'
import { default as ElCollapse } from './components/ElCollapse/index.vue'
import { default as ElDatePicker } from './components/ElDatePicker/index.vue'
import { default as ElDialog } from './components/ElDialog/index.vue'
import { default as ElDrawer } from './components/ElDrawer/index.vue'
import { default as ElDropdown } from './components/ElDropdown/index.vue'
import { default as ElForm } from './components/ElForm/index.vue'
import { default as ElImage } from './components/ElImage/index.vue'
import { default as ElInput } from './components/ElInput/ElInput.vue'
// directives
import vLoading from './components/ElLoading/directive'
import { default as ElMenu } from './components/ElMenu/index.vue'
import { default as ElMessageBox } from './components/ElMessageBox/index.js'
import { default as ElNotification } from './components/ElNotification/index.js'
import { default as ElPagination } from './components/ElPagination/index.vue'
import { default as ElPopover } from './components/ElPopover/index.vue'
import { default as ElRadio } from './components/ElRadio/index.vue'
import { default as ElRadioGroup } from './components/ElRadioGroup/index.vue'
import { default as ElSelect } from './components/ElSelect/index.vue'
import { default as ElSwitch } from './components/ElSwitch/index.vue'
import { default as ElTableColumn } from './components/ElTable/ElTableColumn/index.vue'
import { default as ElTable } from './components/ElTable/index.vue'
import { default as ElTimePicker } from './components/ElTimePicker/index.vue'
import { default as ElTransfer } from './components/ElTransfer/index.vue'
import { default as ElTree } from './components/ElTree/index.vue'
import { default as ElUpload } from './components/ElUpload/index.vue'

const Adapter = {
    ElInput,
    ElDropdown,
    ElMenu,
    ElRadioGroup,
    ElTable,
    ElTableColumn,
    ElSelect,
    ElPagination,
    ElButton,
    ElForm,
    ElCascader,
    ElRadio,
    ElCheckbox,
    ElDatePicker,
    ElTimePicker,
    ElDialog,
    ElSwitch,
    ElDrawer,
    ElPopover,
    ElTransfer,
    ElCollapse,
    ElImage,
    ElUpload,
    ElTree
}

export default Adapter

export function adapterInstall(app: App) {
    for (const [key, component] of Object.entries(Adapter)) {
        app.component(key, component)
    }

    adapterLoading(app)
    app.use(ElMessageBox)
    app.use(ElNotification)
}

export function adapterLoading(app: App) {
    app.directive('loading', vLoading)
}
