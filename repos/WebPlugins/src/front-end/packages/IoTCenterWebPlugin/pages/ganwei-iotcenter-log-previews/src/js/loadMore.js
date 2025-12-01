export default {}.install = (Vue, options = {}) => {
    Vue.directive('loadmore', {
        inserted (el, binding) {

            // 获取element-ui定义好的scroll盒子
            const SELECTDOWN_DOM = el.querySelector('.logPreviewMain')
            SELECTDOWN_DOM.addEventListener('scroll', function () {
                binding.value.setData(true)
                const CONDITION = this.scrollHeight - this.scrollTop <= this.clientHeight
                if (CONDITION) {
                    binding.value.getData()
                }
            })
        }
    })
}