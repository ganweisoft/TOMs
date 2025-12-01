import type { Plugin } from 'vite'
import fs from 'fs'
import path from 'path'
import child_process from 'child_process'
const packageJson = require('../package.json')
const distUrl = path.resolve(__dirname, '../../../dist')

export default function viteAddInfoHtml(): Plugin {
    return {
        // 插件名称
        name: 'viteAddInfoHtml',
        closeBundle() {
            try {
                const info = htmlInfo()
                let url = ''
                if (packageJson.name == 'ganwei-iotcenter-index') {
                    url = `${distUrl}/index.html`
                } else {
                    url = `${distUrl}/${packageJson.name}/index.html`
                }
                let html = fs.readFileSync(url, 'utf-8')
                fs.writeFileSync(url, `<!-- ${JSON.stringify(info)} -->${html}`)
            } catch (error) {
                console.log(error, '添加版本信息失败--不影响最终包输出')
            }

        }
    }
};

function htmlInfo() {
    const nowDate = new Date()
    const version = child_process.execSync('git tag --sort=-taggerdate').toString().trim().split('\n')[0]
    const buildUserName = child_process.execSync('git config user.name').toString().trim()
    const buildDate = `${nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + ' ' + nowDate.getHours() + ':' + nowDate.getMinutes()}`
    return { "版本号": version, "发布人员": buildUserName, "发布日期": buildDate }
}

