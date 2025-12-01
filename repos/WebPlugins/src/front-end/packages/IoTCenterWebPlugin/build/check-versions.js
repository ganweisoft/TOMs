/**
 * @fileOverview 用来检测node和npm等版本
 */

'use strict'
// 下面的插件是chalk插件，他的作用是在控制台中输出不同的颜色的字，
// 大致这样用chalk.blue('Hello world')，这款插件只能改变命令行中的字体颜色
const chalk = require('chalk')
// 下面这个是semver插件，是用来对特定的版本号做判断的，
// 比如 semver.gt('1.2.3','9.8.7') false 1.2.3版本比9.8.7版本低
// semver.satisfies('1.2.3','1.x || >=2.5.0 || 5.0.0 - 7.2.3') true 1.2.3的版本符合后面的规则
const semver = require('semver')
// 下面是导入package.json文件,要使用里面的engines选项，
// 要注意require是直接可以导入json文件的，并且requrie返回的就是json对象
const packageConfig = require('../package.json')
// 下面这个插件是shelljs，作用是用来执行Unix系统命令
const shell = require('shelljs')

function exec(cmd) {
    //脚本可以通过 child_process 模块新建子进程，从而执行 Unix 系统命令
    //下面这段代码实际就是把cmd这个参数传递的值转化成前后没有空格的字符串，也就是版本号
    //require('child_process') node的模块，execSync(cmd)创建同步进程
    return require('child_process').execSync(cmd).toString().trim()
}

const versionRequirements = [
    {
        // node版本的信息
        name: 'node',
        // 使用semver插件吧版本信息转化成规定格式，也就是 ' =v1.2.3 ' -> '1.2.3' 这种功能
        currentVersion: semver.clean(process.version),
        // 这是规定的pakage.json中engines选项的node版本信息 "node":">= 4.0.0"
        versionRequirement: packageConfig.engines.node
    }
]

if (shell.which('npm')) {
    versionRequirements.push({
        name: 'npm',
        // 自动调用npm --version命令，并且把参数返回给exec函数，从而获取纯净的版本号
        currentVersion: exec('npm --version'),
        // 这是规定的pakage.json中engines选项的node版本信息 "npm": ">= 3.0.0"
        versionRequirement: packageConfig.engines.npm
    })
}

module.exports = function () {
    const warnings = []

    for (let i = 0; i < versionRequirements.length; i++) {
        const mod = versionRequirements[i]
        //上面这个判断就是如果版本号不符合package.json文件中指定的版本号，就执行下面的代码
        if (!semver.satisfies(mod.currentVersion, mod.versionRequirement)) {
            // 把当前版本号用红色字体 符合要求的版本号用绿色字体 给用户提示具体合适的版本
            warnings.push(mod.name + ': ' + chalk.red(mod.currentVersion) + ' should be ' + chalk.green(mod.versionRequirement))
        }
    }

    // 提示用户更新版本
    if (warnings.length) {
        console.log('')
        console.log(chalk.yellow('To use this template, you must update following to modules...必须更新以下模块版本:'))
        console.log()

        for (let i = 0; i < warnings.length; i++) {
            const warning = warnings[i]
            console.log('  ' + warning)
        }

        console.log()
        process.exit(1)
    }
}
