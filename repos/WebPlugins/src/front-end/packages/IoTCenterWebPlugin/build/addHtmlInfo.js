const fs = require('fs')
const writeFile = async function (url) {
    let html = fs.readFileSync(url, 'utf-8')
    if (!html.includes('htmlWebpackPlugin.options.buildInfo')) {
        const fileHandle = await fs.promises.open(url, "r+")
        const buffer = await fileHandle.readFile()
        const txt = Buffer.from('<!--<%= htmlWebpackPlugin.options.buildInfo %>-->\r\n')
        const content = Buffer.concat([txt, buffer])
        fileHandle.write(content, 0, content.length, 0)
    }
}
module.exports = writeFile