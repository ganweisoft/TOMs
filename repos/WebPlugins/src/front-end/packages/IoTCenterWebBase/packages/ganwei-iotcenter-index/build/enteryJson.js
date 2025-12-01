
const pageEntry = [{
    filename: 'ganwei-iotcenter-index', // filename 默认是template文件名，就是index.html
    entry: `src/main.js`,
    output: `../../ganwei-iotcenter-index/dist`,
    template: `index.html`,
    chunks: ['chunk-vendors', 'chunk-common', 'index']
}];
export default pageEntry;
