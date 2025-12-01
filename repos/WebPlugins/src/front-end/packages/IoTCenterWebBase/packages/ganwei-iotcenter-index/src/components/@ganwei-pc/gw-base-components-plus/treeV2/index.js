import Tree from './treev2.vue';
import './tree.scss'

/* istanbul ignore next */
Tree.install = function (Vue) {
    Vue.component(Tree.name, Tree);
};

export default Tree;
