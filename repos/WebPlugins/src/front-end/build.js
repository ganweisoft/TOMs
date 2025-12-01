// 说明：需要安装nvm ，并且使用nvm安装两个版本的node，14.21.3 和 23.1.0，其中23.1.0需要安装pnpm
// 执行指令：nvm use 14.21.3;node build.js;nvm use 23.1.0;node build.js;
// 执行完成后，在IoTCenterWebPlugin和IoTCenterWebBase目录下会生成dist文件夹，里面就是构建好的包


const { exec } = require('child_process');
const path = require('path');
const fs = require('fs');

// 兼容的彩色日志
const colors = {
  green: text => `\x1b[32m${text}\x1b[0m`,
  yellow: text => `\x1b[33m${text}\x1b[0m`,
  red: text => `\x1b[31m${text}\x1b[0m`,
  blue: text => `\x1b[34m${text}\x1b[0m`
};

if(!process || !process.versions || !process.versions.node) {console.log(colors.yellow('请使用NVM安装node环境!'));return;}

const versionNumber = process.versions.node;
console.log("当前使用node版本为:",colors.blue(versionNumber),"\n")

// 兼容不同平台的 npm 命令
const currentNodeVersion = "14.21.3";
const targetOutputDir = "/Release/IoTCenterWeb/publish/wwwroot"
const npmCmd =  versionNumber == currentNodeVersion?(process.platform ==='win32' ? 'npm.cmd' : 'npm'):(process.platform ==='win32' ? 'pnpm.cmd' : 'pnpm');
const targetDirUrl =  versionNumber == currentNodeVersion?"/packages/IoTCenterWebPlugin": "/packages/IoTCenterWebBase";
const targetInstallCommand = `${npmCmd} install`;
const targetexecCommand = versionNumber == currentNodeVersion?`${npmCmd} run prestart&${npmCmd} run build-all`:`${npmCmd} build`;

// 检测依赖是否已经安装
const targetDir = path.join(__dirname+targetDirUrl, 'node_modules');

if (!fs.existsSync(targetDir)) {
    console.log(colors.green('正在安装依赖...'));
    execProcessFunction(targetInstallCommand,"✅ 依赖安装完成", 1);
} else {
    console.log(colors.green('正在构建包...'));
    execProcessFunction(targetexecCommand,"✅ 包构建完成", 0);
}

function execProcessFunction(exeCommand,tips, index){
    const installProcess = exec(exeCommand, { cwd: __dirname +targetDirUrl});
    installProcess.on('exit', (code) => {
        if (code === 0) {
            console.log(colors.green(tips));
            if(index == 1) {
                try {
                    console.log(colors.green('正在构建包...'));
                    execProcessFunction(targetexecCommand,"✅ 包构建完成", 0);
                } catch (error) {
                    console.error(colors.red(`安装后任务出错: ${error.message}`));
                    process.exit(1);
                }
            } else { 
                if(versionNumber == currentNodeVersion) {
                    // 删除文件夹
                    deleteFolder(path.join(__dirname+targetDirUrl, 'dist/static'));
                } else {
                    copyFolderRecursive(path.resolve(__dirname + targetDirUrl, 'dist/'), process.cwd()+ targetOutputDir);
                }
            }
        } else {
            console.log(colors.red('❌ 执行指令失败:'+ exeCommand));
            process.exit(code);
        }
    });
}


// 递归拷贝函数
function copyFolderRecursive(source, target) {
    // 创建目标文件夹
    if (!fs.existsSync(target)) {
      fs.mkdirSync(target, { recursive: true });
    }
  
    // 读取源文件夹
    const files = fs.readdirSync(source);
  
    for (const file of files) {
      const sourcePath = path.join(source, file);
      const targetPath = path.join(target, file);
  
      // 获取文件状态
      const stat = fs.statSync(sourcePath);
  
      if (stat.isDirectory()) {
        // 如果是文件夹，递归拷贝
        copyFolderRecursive(sourcePath, targetPath);
      } else {
        // 如果是文件，直接拷贝
        fs.copyFileSync(sourcePath, targetPath);
      }
    }
  }

  // 删除文件夹
  function rimraf(dir) {
    return new Promise((resolve, reject) => {
      fs.rmdir(dir, { recursive: true }, err => {
        if (err && err.code !== 'ENOENT') return reject(err);
        resolve();
      });
    });
  }
  
  async function deleteFolder(dir) {
    try {
      await rimraf(dir);
      copyFolderRecursive(path.resolve(__dirname + targetDirUrl, 'dist/'), process.cwd()+ targetOutputDir);
    } catch (err) {
      console.error('删除失败:', err);
    }
  }
