
import filterColumn from '../filterColumn/index.vue'
export default {
    components: {
        filterColumn
    },
    props: {
        loading: {
            type: Boolean,
            default: false
        },
        tabNames: {
            type: Array,
            default: () => []
        }
    },
    data () {
        return {
            tabNavNumber: 1,
            // ycp column
            columnList: [ {
                name: 'equipListsIot-ycp',
                column: [
                    this.$t('equipListsIot.table.listTitleYc.ycYxNo'),
                    this.$t('equipListsIot.table.listTitleYc.alarmState'),
                    this.$t('equipListsIot.table.listTitleYc.ycYxName'),
                    this.$t('equipListsIot.table.listTitleYc.value'),
                    // this.$t('equipListsIot.table.listTitleYc.quantity'),
                    this.$t('equipListsIot.table.listTitleYc.curve'),
                    this.$t('equipListsIot.table.listTitleYc.suggestion')
                ]
            },
            {
                name: 'equipListsIot-yxp',
                column: [
                    this.$t('equipListsIot.table.listTitleYx.ycYxNo'),
                    this.$t('equipListsIot.table.listTitleYx.alarmState'),
                    this.$t('equipListsIot.table.listTitleYx.ycYxName'),
                    this.$t('equipListsIot.table.listTitleYx.value'),
                    this.$t('equipListsIot.table.listTitleYx.curve'),
                    this.$t('equipListsIot.table.listTitleYx.suggestion')
                ]
            },{
                name: 'equipListsIot-set',
                column: []
            }]
        }
    },
    methods: {
        onTabNav (index) {
            this.tabNavNumber = index;
            this.$emit('change', index)
        }
    }
}
