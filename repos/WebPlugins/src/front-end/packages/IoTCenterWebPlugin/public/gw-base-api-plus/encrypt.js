/* eslint-disable */
import JsEncrypt from 'jsencrypt/bin/jsencrypt.min.js'
let myKey = ''
let type = ''
// let isFirst = false;

const encrypt = {

    // 字符串转换为字节数组
    StringToByte (str) {
        var utf8 = unescape(encodeURIComponent(str));

        var arr = [];
        for (var i = 0; i < utf8.length; i++) {
            arr.push(utf8.charCodeAt(i));
        }
        return arr
    },

    uint8ArrayToBase64 (data) {
        return window.btoa(Array.from(data).map((c) => String.fromCharCode(c)).join(''));
    },

    async newEncrypt (data) {
        let dataStr = await this.setEncrypt(myKey)
        let RSAData = await this.setOther(dataStr, data)
        return RSAData
    },

    RSAEncrypt (data) {
        if (!myKey) {
            this.getPublic()
        }
        let RSAData = ''
        if (!type || type == 'pkcs1') {
            RSAData = this.RSAEncryptOne(data)
        } else {
            RSAData = this.newEncrypt(data)
        }
        return RSAData
    },

    getPublic () {
        var ajax = new XMLHttpRequest();
        ajax.onreadystatechange = function () {
            if (ajax.readyState == 4 && ajax.status == 200) {
                let datas = JSON.parse(ajax.responseText);
                if (datas.data.cipher) {
                    myKey = decodeURIComponent(datas.data.cipher)
                    type = datas.data.padding
                } else {
                    myKey = decodeURIComponent(datas.data)
                }
            }
        }
        ajax.open('get', '/IoT/api/v3/auth/GetString', false);
        ajax.send();
    },


    async setEncrypt (publicKey) {
        return new Promise((resolve, reject) => {
            let crypto = window.crypto
            crypto.subtle.importKey(
                "jwk",
                {
                    kty: "RSA",
                    e: "AQAB",
                    n: publicKey,
                    alg: "RSA-OAEP-256",
                    ext: true,
                },
                {
                    name: "RSA-OAEP",
                    hash: { name: "SHA-256" },
                },
                false,
                ["encrypt"])
                .then((publicKey) => {
                    resolve(publicKey)
                })
                .catch(function (err) {
                    console.error(err);
                });
        })

    },

    async setOther (publicKey, data) {
        return new Promise((resolve, reject) => {
            let crypto = window.crypto || window.webkitCrypto || window.mozCrypto || window.oCrypto || window.msCrypto
            crypto.subtle.encrypt(
                {
                    name: "RSA-OAEP",
                },
                publicKey,
                new Uint8Array(this.StringToByte(data))
            ).then((encrypted) => {
                // RSAData.data = encodeURIComponent(this.uint8ArrayToBase64(new Uint8Array(encrypted)))
                resolve(this.uint8ArrayToBase64(new Uint8Array(encrypted)))
                // return encodeURIComponent(this.uint8ArrayToBase64(new Uint8Array(encrypted)))
            }).catch(function (err) {
                console.error(err);
            });
        })
    },


    RSAEncryptOne (strPassword) {
        let pubkey = myKey;
        let encrypt = new JsEncrypt();
        encrypt.setPublicKey(pubkey);
        let data = encrypt.encrypt(strPassword);
        return data;
    },

    clearMyKey () {
        myKey = '';
    }
}

export default encrypt;
