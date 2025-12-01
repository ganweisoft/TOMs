
/**
*
* 权限指令
* 使用： v-permission="'add'"
*/

// 权限初始化
export function initHasPermission(options) {
    const permissionList = options;
    return (permission) => { // 内部返回function，用于判断权限
        if(!Array.isArray(permissionList) || permissionList.length == 0) {
            return false;
        }
        if (permission.auth) {
            let value = permissionList.includes(permission.auth);
            return value;
        }
        return false;
    }
}

// 权限指令
export function createPermissionDirective(arry) {
    return {
        async mounted(el, binding) { // vue3
            // 元素插入父节点时执行的逻辑
            el.style.display = 'none';
            const hasPermissions = initHasPermission(arry);
            checkPermission(el, binding, hasPermissions);
        },
        async bind(el, binding, vnode) { // vue2
            // 元素插入父节点时执行的逻辑
            el.style.display = 'none';
            const hasPermissions = initHasPermission(arry);
            checkPermission(el, binding, hasPermissions);
        }
    };
}

// 权限检查并执行相应操作
function checkPermission(el, binding, hasPermissions) {
    setTimeout(()=>{
        const {value} = binding;
        if (value) {
            el.style.display = '';
          let options = {auth: value};
          if (hasPermissions(options)) {
              el.parentNode
          } else {
              el.parentNode && el.parentNode.removeChild(el)
          }
        }
    },200)
  }
