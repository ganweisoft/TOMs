var chalk = require('chalk')

var glob = require('glob')

// 获取所有的moduleList
var moduleList = []

let src = `${process.env.PAGE_ENV}/*`
var moduleSrcArray = glob.sync(src)

for (var x in moduleSrcArray) {
    moduleList.push(moduleSrcArray[x].split('/').pop())
}
// process.env.FILES_ENV = JSON.stringify(moduleList)
console.log('File List', moduleList)
// 检测是否在输入的参数是否在允许的list中
var checkModule = function () {
    var module = process.env.MODULE_ENV
    // 检查moduleList是否有重复
    var hash = {}
    var repeatList = []
    for (var l = 0; l < moduleList.length; l++) {
        if (hash[moduleList[l]]) {
            repeatList.push(moduleList[l])
        }
        hash[moduleList[l]] = true
    }
    if (repeatList.length > 0) {
        console.log(chalk.red('moduleList 有重复：'))
        console.log(chalk.red(repeatList.toString()))
        return false
    }
    let result = true
    let illegalParam = ''
    for (let moduleToBuild of module.split(',')) {
        if (moduleList.indexOf(moduleToBuild) === -1) {
            result = false
            illegalParam = moduleToBuild
            break
        }
    }
    if (result === false) {
        console.log(chalk.red('参数错误，允许的参数为：'))
        console.log(chalk.green(moduleList.toString()))
        console.log(chalk.yellow(`非法参数：${illegalParam}`))
    }
    return result
}

// 获取当前要打包的模块列表
function getModuleToBuild() {
    let moduleToBuild = []
    if (process.env.NODE_ENV === 'production') {
        /* 部署态，构建要打包的模块列表，如果指定了要打包的模块，那么按照指定的模块配置入口
         *  这里有个特性，即使参数未传，那么获取到的undefined也是字符串类型的，不是undefined类型
         * */
        if (process.env.MODULE_ENV !== 'undefined') {
            moduleToBuild = process.env.MODULE_ENV.split(',')
        } else {
            // 如果未指定要打包的模块，那么打包所有模块
            moduleToBuild = moduleList
        }
    } else {
        // 开发态，获取所有的模块列表
        moduleToBuild = moduleList
    }
    return moduleToBuild
}
exports.moduleList = moduleList
exports.checkModule = checkModule
exports.getModuleToBuild = getModuleToBuild
