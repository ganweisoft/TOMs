const fs = require('fs')
const { execSync } = require('child_process')
const baseUrl = '../packages'
const range = ['dependencies', 'devDependencies', 'optionalDependencies']
let updateDependencies = {}
if (fs.existsSync(baseUrl)) {
    const folders = fs.readdirSync(baseUrl);
    folders.forEach(folder => {
        let url = `${baseUrl}/${folder}/package.json`
        if (fs.existsSync(url)) {
            let config = fs.readFileSync(url, 'utf-8')
            config = JSON.parse(config);
            Object.keys(config).forEach(item => {
                if (range.includes(item)) {
                    Object.keys(config[item]).forEach(child => {
                        if (child.includes('@ganwei-pc') && config[item][child].includes('^')) {
                            updateDependencies[child] = config[item][child].split('^').pop().split('.')[0]
                        }
                    })
                }
            })
        }
    })
}
let commands = 'pnpm up'
Object.keys(updateDependencies).forEach(item => {
    commands = `${commands} ${item}@${updateDependencies[item]}`
})
commands = `${commands} --filter *`
execSync(commands, { stdio: [0, 1, 2] })