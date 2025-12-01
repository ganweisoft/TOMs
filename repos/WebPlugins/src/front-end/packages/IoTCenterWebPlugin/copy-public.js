const fs = require('fs-extra');
const path = require('path');

// 配置源目录和目标目录
const sourceDir = path.resolve(__dirname, 'public/');
const targetDir = path.resolve(__dirname, 'node_modules');

// 递归拷贝函数
async function copyPublicFiles() {
  try {
    // 确保目标目录存在
    await fs.ensureDir(targetDir);

    // 递归拷贝整个目录
    await fs.copy(sourceDir, targetDir, {
      overwrite: true, // 覆盖已存在的文件
      dereference: true, // 解引用符号链接
      preserveTimestamps: true, // 保留时间戳
      filter: (src) => {
        // 可选：过滤不想拷贝的文件
        const excludeFiles = ['.DS_Store', 'Thumbs.db', '.gitkeep'];
        return !excludeFiles.some(exclude => src.endsWith(exclude));
      }
    });
  } catch (err) {
    process.exit(1); // 出错时退出并返回错误码
  }
}

// 执行拷贝
copyPublicFiles();