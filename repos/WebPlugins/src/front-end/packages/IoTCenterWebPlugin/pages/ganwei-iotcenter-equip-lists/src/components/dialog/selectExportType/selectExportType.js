
export default {
    props: {
        showDialog:{
            type:Boolean,
            default:false
        }
    },
    data() {
        return {
            exportTypeOption: [
                {
                    label: 'equipListsIot.button.exportByDay',
                    value: false
                },
                {
                    label: 'equipListsIot.button.exportByCombined',
                    value: true
                }
            ],
            isMerge:false
        }
    },
    methods:{
        closeDialog(){
            this.$emit('closeDialog','showExportTypeSelectDialog')
        },
        confirm(){
            this.$emit('confirm',this.isMerge)
        }
    }
}