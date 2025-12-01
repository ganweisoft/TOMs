<template>
    <el-dialog class="addGroup" :title="$t(title)" :visible.sync="showGroupDialog" :close-on-click-modal="false"
        @close="closeDialog('showGroupDialog')" center>
        <div class="addEquipBox">
            <div class="addEquipAndFenZu">
                <div class="addEquipAndFenZu_main">
                    <el-form ref="form" :inline="true" :model="form" :rules="rules">
                        <div class="nmEquip_link" v-if="isGroupNew" @click.stop="">
                            <span>{{ $t('equipInfo.dialog.branch') }}</span>
                            <el-input clearable v-model="info.groupName" maxlength="255" :disabled="true"></el-input>

                            <p>{{ $t('equipInfo.dialog.groupName') }}</p>
                            <el-form-item prop="editGroupName">
                                <el-input clearable v-model="form.editGroupName" maxlength="255" @blur="() => {
                                    form.editGroupName = form.editGroupName.trim()
                                }
                                    " :placeholder="$t('equipInfo.dialog.inputGroupName')"></el-input>
                            </el-form-item>
                        </div>

                        <div class="nmEquip_link" v-else>
                            <p>{{ $t('equipInfo.dialog.currentGroup') }}</p>
                            <el-input clearable v-model="info.groupName" @blur="() => {
                                info.groupName = info.groupName.trim()
                            }
                                " maxlength="255" readonly></el-input>

                            <p>{{ $t('equipInfo.poverTips.rename') }}</p>
                            <el-form-item prop="editGroupName">
                                <el-input clearable v-model="form.editGroupName" @blur="() => {
                                    form.editGroupName = form.editGroupName.trim()
                                }
                                    " maxlength="255" :placeholder="$t('equipInfo.dialog.inputGroupName')"></el-input>
                            </el-form-item>
                        </div>
                    </el-form>
                </div>
            </div>
        </div>
        <span class="dialog-footer" slot="footer">
            <el-button type="primary" plain size="small" @click="closeDialog('showGroupDialog')"> {{
                $t('equipInfo.publics.button.cancel') }}</el-button>
            <el-button type="primary" size="small" @click="saveGroupData" :loading="saveLoading"> {{
                $t('equipInfo.publics.button.confirm') }}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
