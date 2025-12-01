import { useI18n } from 'vue-i18n';
import { ElMessageBox } from 'element-plus';
import { ElNotification } from 'element-plus';

import { useAxios } from '@components/@ganwei-pc/gw-base-api-plus/useAxios/useAxios';

import { LinkSettingService } from '@/request/api';

import { SceneRepositoryData } from './GetSceneSettings';
export default function (refresh: () => void) {
    const $t = useI18n().t;
    const { execute: delScene } = useAxios(LinkSettingService.delScene)

    function confirmDelete(row: SceneRepositoryData) {
        ElMessageBox({
            title: $t('sceneSetting.tips.delTitle1'),
            message: $t('sceneSetting.label.delPrompt') + row.sceneName + '?',
            showCancelButton: true,
            confirmButtonText: $t('sceneSetting.button.comfirmed'),
            cancelButtonText: $t('sceneSetting.button.cancel'),
            beforeClose: (action, instance, done) => {
                if (action === 'confirm') {
                    instance.confirmButtonLoading = true
                    delScene({ equipno: row.equipNo, setNo: row.sceneNo }).then(res => {
                        ElNotification({
                            title: $t('sceneSetting.tips.delTitle1'),
                            message: res.data.message,
                            type: 'success',
                          })
                        refresh()
                    }).finally(() => {
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
        confirmDelete
    }
}
