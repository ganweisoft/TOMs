/**
 * @fileOverview 开发环境的环境变量配置文件
 */

'use strict'
// 引入webpack-merge模块。这个模块用于把多个webpack配置合并成一个配置，后面的配置会覆盖前面的配置。
const merge = require('webpack-merge')
// 导入prod.env.js配置文件，是生产环境的环境变量配置文件
const prodEnv = require('./prod.env')

// 将两个配置对象合并，最终结果是 NODE_ENV: '"development"'
module.exports = merge(prodEnv, {
    NODE_ENV_ALL: '"proAll"'
})
