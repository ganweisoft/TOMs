import { h } from "vue"
import { useI18n } from "vue-i18n"
import { ElMessageBox } from "element-plus"
import { useAxios } from "@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios"
import { LinkSettingService } from "@/request/api"

/**
 * 删除联动确认
 *
 * @export
 * @param {() => void} refresh
 * @returns
 */
export default function (refresh: () => void) {
    const $t = useI18n().t
    const { deleteLinkSetting } = DeleteLinkSetting()
    const { ifDeleteLinkSetting } = IfDeleteLinkSetting()
    function delData(equipName: string, equipNo: number, id: number, typeValue: boolean) {
        ElMessageBox({
            title: $t('linkSetting.tips.delTitle1'),
            message: h('div', null, [
                h(
                    'div',
                    {
                        class: {
                            'el-message-box__status el-icon-warning': true
                        }
                    },
                    []
                ),
                h('div', { class: { 'el-message-box__message': true } }, $t('linkSetting.label.delPrompt') + equipName + '?')
            ]),
            showCancelButton: true,
            confirmButtonText: $t('linkSetting.button.comfirmed'),
            cancelButtonText: $t('linkSetting.button.cancel'),
            beforeClose: (action, instance, done) => {
                if (action === 'confirm') {
                    instance.confirmButtonLoading = true
                    let dt = typeValue? ifDeleteLinkSetting({'id': id }):deleteLinkSetting({ type: 'id', data: id })
                    dt.finally(() => {
                        refresh()
                        instance.confirmButtonLoading = false
                        done()
                    })
                } else {
                    done()
                }
            }
        })
    }
    return {
        delData
    }
}

function DeleteLinkSetting() {
    const { execute: deleteLinkSetting } = useAxios(LinkSettingService.linkDel)
    return {
        deleteLinkSetting
    }
}

function IfDeleteLinkSetting() {
    const { execute: ifDeleteLinkSetting } = useAxios(LinkSettingService.IfLinkDel)
    return {
        ifDeleteLinkSetting
    }
}
