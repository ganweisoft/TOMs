const fs = require('fs')
const path = require('path')
const versionOptions = require('./config/versionOptions.json')
const { execSync } = require('child_process')
let framePackageConfig = require('../../package.json')
const plugin = require('./config/plugins.json')

module.exports = async function (type) {
    preLoadPlugin()
    if (versionOptions.versions && versionOptions.versions.length) {
        const prompts = require('prompts')
        if (versionOptions.versions.length > 1) {
            const response = await prompts({
                type: 'select',
                name: 'version',
                message: "请选择版本：",
                choices: versionOptions.versions
            })
            if (response.version) {
                downAndMove(type, response.version)
            }
        } else if (versionOptions.versions.length == 1) {
            downAndMove(type, versionOptions.versions[0].value)
        }
    }
}

/**
 * @description: 提前加载自身需要用到的插件
 * @return 无返回值
 */
function preLoadPlugin () {
    let dependenciesUrl = ''
    Object.keys(plugin).forEach(item => {
        dependenciesUrl = `${dependenciesUrl} ${item}`
    })
    dependenciesUrl = `npm install ${dependenciesUrl}`
    execSync(dependenciesUrl, { stdio: [0, 1, 2] })
}

/**
 * @description: 根据用户选择版本，下载对应依赖包、更新对应文件、更新对应脚本
 * @param type String 类型（最新版lat、稳定版fix）
 * @param version String 版本（多版本管理情况下，如后续有6.1，7.1等版本）
 * @return 无返回值
 */
function downAndMove (type, version) {
    if (fs.existsSync(path.resolve(`./node_modules/gw-base-frame-version/versions/${version}.json`))) {
        let versionConfig = fs.readFileSync(path.resolve(`./node_modules/gw-base-frame-version/versions/${version}.json`), 'utf-8')
        versionConfig = JSON.parse(versionConfig)
        if (versionConfig[type]) {
            downPackages(versionConfig[type])
            moveFolder(process.env.npm_config_force, versionConfig[type].excludeFile)
            updateFrameScript(versionConfig[type].scripts)
        }
    } else {
        console.log('版本文件不存在！')
    }
}

/**
 * @description: 下载对应依赖包
 * @param config Object 对应版本文件中的配置
 * @return 无返回值
 */
function downPackages (config) {

    // 卸载第三方依赖包（版本冲突）
    unInstallDependencies(config.unInstallDependencies)

    // 更新第三方依赖包
    downDependencies(config.dependencies, 'dependencies', '', '')
    downDependencies(config.devDependencies, 'devDependencies', '--dev', '')

    // 更新自身依赖包
    downDependencies(config.optionalDependencies, 'optionalDependencies', '--save-optional', '--registry=http://default/repository/npm-hosted/')

}

/**
 * @description: 删除某些因版本冲突相关依赖包
 * @param dependencies Object 对应版本文件中的配置
 * @return 无返回值
 */
function unInstallDependencies (dependencies) {
    if (dependencies) {
        let dependenciesUrl = ''
        Object.keys(dependencies).forEach(item => {
            dependenciesUrl = `${dependenciesUrl} ${item}`
        })
        dependenciesUrl = `npm uninstall ${dependenciesUrl}`
        execSync(dependenciesUrl, { stdio: [0, 1, 2] })
    }
}


/**
 * @description: 下载对应依赖包
 * @param dependencies Object 对应版本文件中的配置（某个配置对象）
 * @param obj String 外框中的配置（某个配置对象）
 * @param type String 下载类型，是--dev 还是--save-optional
 * @param registry String 指定镜像
 * @return 无返回值
 */
function downDependencies (dependencies, obj, type, registry) {
    if (dependencies) {
        let dependenciesUrl = ''
        Object.keys(dependencies).forEach(item => {

            if (!framePackageConfig[obj]) {
                framePackageConfig[obj] = {}
            }
            if (!framePackageConfig[obj][item] || framePackageConfig[obj][item] != dependencies[item]) {
                dependenciesUrl = dependenciesUrl + `${item}@${dependencies[item].replace('^', '')} `
            }
        })
        dependenciesUrl = `npm install ${dependenciesUrl} ${type} ${registry}`
        execSync(dependenciesUrl, { stdio: [0, 1, 2] })
    }
}


/**
 * @description: 更新对应文件
 * @param force boolean 是否强制更新
 * @param excludeFile Array 排除哪些文件不更新
 * @return 无返回值
 */
function moveFolder (force, excludeFile) {
    let configFiles = fs.readdirSync(path.resolve('./node_modules/gw-base-frame-config'))
    configFiles.forEach(child => {
        if (!excludeFile.includes(child) && (force || isFileExistence(child))) {
            let isDirectory = fs.statSync(path.resolve(`./node_modules/gw-base-frame-config/${child}`)).isDirectory()
            move(`./node_modules/gw-base-frame-config/${child}`, `./${child}`, isDirectory)
        }
    })

}
/**
 * @description: 移动更新对应文件
 * @param fromFilePath String 源文件夹
 * @param toFilePath String 目标文件夹
 * @param isDirectory Boolean 是否是文件夹
 * @return 无返回值
 */
function move (fromFilePath, toFilePath, isDirectory) {
    if (!fs.existsSync(toFilePath) && isDirectory) {
        fs.mkdir(toFilePath, err => {
            // console.log(err)
        })
    }
    if (isDirectory) {
        //读取newFile文件夹下的文件
        fs.readdir(fromFilePath, { withFileTypes: true }, (err, files) => {
            if (!files) {
                return;
            }
            for (let file of files) {
                //判断是否是文件夹，不是则直接复制文件到newFile中
                if (!file.isDirectory()) {
                    //获取旧文件夹中要复制的文件
                    const OriginFile = path.resolve(fromFilePath, file.name)
                    //获取新文件夹中复制的地方
                    const CopyFile = path.resolve(toFilePath, file.name)
                    //将文件从旧文件夹复制到新文件夹中
                    fs.copyFileSync(OriginFile, CopyFile)
                } else { //如果是文件夹就递归变量把最新的文件夹路径传过去
                    const OriginDirPath = path.resolve(fromFilePath, file.name)
                    const CopyDirPath = path.resolve(toFilePath, file.name)
                    fs.mkdir(CopyDirPath, (err) => {

                    })
                    move(OriginDirPath, CopyDirPath, true)
                }
            }
        })
    } else {
        fs.copyFileSync(fromFilePath, toFilePath)
    }
}


/**
 * @description: 检测目标地址是否含有该文件,目标含有则不覆盖
 * @return {目标文件不存在则返回true,否则false}
 */
function isFileExistence (foldName) {
    let configFiles = fs.readdirSync('./')
    return !configFiles.includes(foldName)
}


/**
 * @description: 更新对应脚本
 * @param scripts 从版本文件中获取的脚本
 * @return 无返回值
 */
function updateFrameScript (scripts) {
    let config = fs.readFileSync(path.resolve(`./package.json`), 'utf-8')
    config = JSON.parse(config)
    Object.keys(scripts).forEach(item => {
        if (!config.scripts[item] || config.scripts[item] != scripts[item]) {
            config.scripts[item] = scripts[item]
        }
    })

    // 更新根目录package.json
    fs.writeFileSync('./package.json', JSON.stringify(config, null, 4), 'utf-8')
}