const manage = () => import('../components/manage/manage.vue')
export default {
    components: { manage },
    data() {
        return {
            selecteItem: {}
        }
    },

    mounted() {
        this.getInvocing()
    },

    created() {},

    watch: {},

    methods: {
        async getInvocing() {
            // await window.top.getLanguage('ganwei-iotcenter-equip-manage', 'manageFram', 'Ganweisoft.IoTCenter.Module.EquipConfig', this)
            await this.i18n.getLanguage('Ganweisoft.IoTCenter.Module.EquipConfig', 'ganwei-iotcenter-equip-manage', 'manageFram', this)
            this.selecteItem = {
                equipNo: Number(this.$route.query.equipNo),
                equipName: this.$route.query.equipName,
                groupName: this.$route.query.groupName,
                groupId: Number(this.$route.query.groupId),
                staNo: Number(this.$route.query.staNo)
            }
        }
    }
}
