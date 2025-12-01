export default {
    data () {
        return {
            key: 0,
            observer: null,
        }
    },
    watch: {
        passwordPolicy () {
            this.disObserveDOM()
            if (this.passwordPolicy == 1) {
                setTimeout(() => {
                    this.observeDOM()
                }, 500)
            }
        }
    },
    beforeDestroy () {
        this.disObserveDOM()
    },
    methods: {
        observeDOM () {
            const id = '#index .el-overlay.editPassword'
            const targetElement = document.querySelector(id);
            this.observer = new MutationObserver((mutations) => {
                mutations.forEach(mutation => {
                    // 被删除
                    if (Array.from(mutation.removedNodes).findIndex(i => i.contains(targetElement)) > -1) {
                        this.reRender()
                    }
                    // 被隐藏
                    if (mutation.target === targetElement && !this.isVisible(targetElement)) {
                        this.reRender()
                    }
                });
            })
            this.observer.observe(document.getElementById('index'), { childList: true, subtree: true, attributes: true }); // 开始观察目标元素
        },
        disObserveDOM () {
            if (this.observer) {
                this.observer.disconnect()
                this.observer = null
            }
        },
        reRender () {
            this.disObserveDOM()
            this.key = new Date().getTime()
            setTimeout(() => {
                this.observeDOM()
            }, 500)
        },
        isVisible (elem) {
            if (!(elem instanceof Element)) throw Error('DomUtil: elem is not an element.');
            const style = getComputedStyle(elem);
            if (style.display === 'none') return false;
            if (style.visibility !== 'visible') return false;
            if (style.opacity < 0.1) return false;
            if (elem.offsetWidth + elem.offsetHeight + elem.getBoundingClientRect().height +
                elem.getBoundingClientRect().width === 0) {
                return false;
            }
            const elemCenter = {
                x: elem.getBoundingClientRect().left + elem.offsetWidth / 2,
                y: elem.getBoundingClientRect().top + elem.offsetHeight / 2
            };
            if (elemCenter.x < 0) return false;
            if (elemCenter.x > (document.documentElement.clientWidth || window.innerWidth)) return false;
            if (elemCenter.y < 0) return false;
            if (elemCenter.y > (document.documentElement.clientHeight || window.innerHeight)) return false;
            let pointContainer = document.elementFromPoint(elemCenter.x, elemCenter.y);

            while (pointContainer && pointContainer !== document.body) {
                if (pointContainer === elem) return true;
                pointContainer = pointContainer.parentNode;
            }
            return false;
        }
    }
}
