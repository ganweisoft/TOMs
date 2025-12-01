import { getNodeKey } from '../model/util';

export default {
    methods: {
        creator (parent, nodeTag) {
            const node = this[nodeTag];

            if (parent.isTree) {
                this.tree = parent;
            } else {
                this.tree = parent.tree;
            }

            const tree = this.tree;
            if (!tree) {
                console.warn('Can not find node\'s tree.');
            }

            const props = tree.props || {};
            const childrenKey = props['children'] || 'children';

            this.$watch(`${nodeTag}.data.${childrenKey}`, () => {
                node.updateChildren();
            });

            if (node.expanded) {
                this.expanded = true;
                this.childNodeRendered = true;
            }

            if (this.tree.accordion) {
                this.$on('tree-node-expand', currentNode => {
                    if (node !== currentNode) {
                        node.collapse();
                    }
                });
            }
        },

        getNodeKey (node) {
            return getNodeKey(this.tree.nodeKey, node.data);
        },

        handleSelectChange (checked, indeterminate) {
            const node = this.node || this.source;

            if (this.oldChecked !== checked && this.oldIndeterminate !== indeterminate) {
                this.tree.$emit('check-change', node.data, checked, indeterminate);
            }
            this.oldChecked = checked;
            this.indeterminate = indeterminate;
        },

        handleClick () {
            const node = this.node || this.source;
            const store = this.tree.store;
            store.setCurrentNode(node);
            this.tree.$emit('current-change', store.currentNode ? store.currentNode : null, store.currentNode);
            this.tree.currentNode = this;
            if (this.tree.expandOnClickNode) {
                this.handleExpandIconClick();
            }

            if (node.data && !node.data.isGroup) {
                this.tree.currentSelect = node.data.key;
            }

            this.tree.$emit('saveOpenStatus', node);
            this.tree.$emit('node-click', node.data, node, this);
        },

        handleContextMenu (event) {
            const node = this.node || this.source;

            if (this.tree._events['node-contextmenu'] && this.tree._events['node-contextmenu'].length > 0) {
                event.stopPropagation();
                event.preventDefault();
            }
            this.tree.$emit('node-contextmenu', event, node.data, node, this);
        },

        handleExpandIconClick () {
            const node = this.node || this.source;
            const store = this.tree.store;
            store.setCurrentNode(node);
            // if (node.isLeaf && !node.data.isEquip) return;
            if (this.expanded) {
                this.tree.$emit('node-collapse', node.data, node, this);
                node.collapse();
            } else {
                this.tree.$emit('node-expand', store.currentNode ? store.currentNode : null, store.currentNode)
                node.expand();
                // this.$emit('node-expand', node.data, node, this);
            }
        },

        handleCheckChange (_, ev) {
            const node = this.node || this.source;

            node.setChecked(ev.target.checked, !this.tree.checkStrictly);
            this.$nextTick(() => {
                const store = this.tree.store;
                this.tree.$emit('check', node, {
                    checkedNodes: store.getCheckedNodes(),
                    checkedKeys: store.getCheckedKeys(),
                    halfCheckedNodes: store.getHalfCheckedNodes(),
                    halfCheckedKeys: store.getHalfCheckedKeys()
                });
            });
        },

        handleChildNodeExpand (nodeData, node, instance) {
            this.broadcast('ElTreeNode', 'tree-node-expand', node);
            this.tree.$emit('node-expand', nodeData, node, instance);
        },

        groupEditAndNew (isGroupNew, node) {
            this.tree.$emit('groupEditAndNew', { isGroupNew, node })
        },
        templateNewEquip (node) {
            this.tree.$emit('templateNewEquip', node)
        },
        newEquip (node) {
            this.tree.$emit('newEquip', node)
        },
        deleteGroup (node) {
            this.tree.$emit('deleteGroup', node)
        }

    }
};
