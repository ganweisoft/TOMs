
export default {
    props: {
        editableTabs: {
            default: () => { return {} }
        },
        editableTabsValue: {
            default: () => { return {} }
        }
    },
    
    data() {
        return {
            iEditableTabs: [],
            iEditableTabsValue: ''
        }
    },
    mounted () {
        let jumpIframeArr = document.getElementsByClassName('jumpIframe');
        let iframeItem = jumpIframeArr[jumpIframeArr.length - 1];
        iframeItem.addEventListener('load', () => {
            iframeItem.contentWindow.document.onkeyup = this.keyEvent;
        })
    },
    methods: {
        keyEvent (e) {
            const forArr = (num) => {
                let length = this.editableTabs.length;
                for (let i = 0;i < length;i++) {
                    if (num == -1 && i != 0 && this.editableTabs[i].name == this.editableTabsValue) {
                        this.$router.push(this.editableTabs[i-1].route);
                    } else if (num == 1 && i != length - 1 && this.editableTabs[i].name == this.editableTabsValue) {
                        this.$router.push(this.editableTabs[i+1].route);
                    }
                }
            }

            if (e.altKey && e.keyCode == 188) {
                forArr(-1);
            } else if (e.altKey && e.keyCode == 190) {
                forArr(1);
            }
        }
    }
}