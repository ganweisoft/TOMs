const { execSync } = require('child_process')
const fs = require('fs')
let initConfig = {}

let installUrl = ''

execSync('npm install gw-base-frame-config@latest --registry=http://default/repository/npm-hosted/ --save-optional', { stdio: [0, 1, 2] })

if (!fs.existsSync('./node_modules/gw-base-frame-config/')) {
    console.log('更新失败')
    return;
} else {
    initConfig = fs.readFileSync('./node_modules/gw-base-frame-config/initConfig.json', 'utf-8')
    initConfig = JSON.parse(initConfig)
    Object.keys(initConfig.optionalDependencies).forEach(item => {
        installUrl = installUrl + `${item}@latest `
    })
    installUrl = `npm install ${installUrl} --registry=http://default/repository/npm-hosted/ --save-optional`
    execSync(installUrl, { stdio: [0, 1, 2] })
}
