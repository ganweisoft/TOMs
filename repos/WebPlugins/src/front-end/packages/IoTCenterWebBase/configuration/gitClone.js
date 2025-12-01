
import CHILDPROCESS from 'child_process';
import fs from 'fs';
import path from 'path';
import {
    fileURLToPath
} from 'url';

const __FILENAME = fileURLToPath(
    import.meta.url);
const __DIRNAME = path.dirname(__FILENAME)
const MODULEPATH = path.join(__DIRNAME, "/")
const PACKAGEPATH = path.join(__DIRNAME, "../packages")

let thePackage = process.argv[2]

fs.readFile(MODULEPATH + 'moduleConfiguration.json', 'utf-8', (err, data) => {
    let obj = JSON.parse(data)
    if (obj.applicationPath && typeof (obj.applicationPath) == 'object') {
        if (thePackage == 'All') {
            Object.keys(obj.applicationPath).forEach(key => {
                try {
                    CHILDPROCESS.execSync(`git clone ${obj.applicationPath[key].url} --branch ${obj.applicationPath[key].branch}`, {
                        cwd: PACKAGEPATH
                    })
                } catch (e) {
                    
                }
            })
        } else {
            Object.keys(obj.applicationPath).forEach(key => {
                if (key == thePackage) {
                    try {
                        CHILDPROCESS.execSync(`git clone ${obj.applicationPath[key].url} --branch ${obj.applicationPath[key].branch}`, {
                            cwd: PACKAGEPATH
                        })
                    } catch (e) {
                        
                    }
                }
            })
        }
    }

})
