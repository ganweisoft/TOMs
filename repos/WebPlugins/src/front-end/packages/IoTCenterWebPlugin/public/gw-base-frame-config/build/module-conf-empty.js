/* 
先把入口路径，先缓存起来，然后置空
*/

const fs = require('fs')
const util = require('util')
const outputFile = util.promisify(fs.writeFile)
const baseWebpackConfig = require('./webpack.base.conf')
const baseWebpackConfigEntry = baseWebpackConfig.entry()
const pages = {}

async function main() {
    const tasks = []
    if (!fs.existsSync('./build/dev-entries')) {
        fs.mkdirSync('./build/dev-entries')
    }
    Object.keys(baseWebpackConfigEntry).forEach(key => {
        pages[key] = {}
        const entry = `./build/dev-entries/${key}.js`
        pages[key].tempEntry = `../${process.env.PAGES_ENV}/${key}/index.js` // 暂存真正的入口文件地址

        pages[key].entry = entry
        tasks.push(outputFile(entry, ''))
    })
    await Promise.all(tasks)
}
if (process.env.NODE_ENV_ALL === 'debugAll') {
    main()
}
module.exports = pages
