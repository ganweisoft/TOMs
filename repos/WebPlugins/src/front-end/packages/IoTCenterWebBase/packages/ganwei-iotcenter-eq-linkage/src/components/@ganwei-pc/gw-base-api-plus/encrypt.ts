/* eslint-disable */
import JsEncrypt from 'jsencrypt'

class RSAHelper {
    private myKey = ''
    private type = ''
    url = '/IoT/api/v3/auth/getcipher'

    /**
     * @description 获取RSA加密密钥
     */
    getPublic() {
        const ajax = new XMLHttpRequest();
        ajax.onreadystatechange = () => {
            if (ajax.readyState == 4 && ajax.status == 200) {
                let datas = JSON.parse(ajax.responseText);
                if (datas.data.cipher) {
                    this.myKey = decodeURIComponent(datas.data.cipher)
                    this.type = datas.data.padding
                } else {
                    this.myKey = decodeURIComponent(datas.data)
                }
            }
        }
        ajax.open('get', this.url, false);
        ajax.send();
    }

    /**
     * @description RSA加密
     * @param {string} dataString
     * @return {Promise<string>} 加密结果
     */
    async RSAEncrypt(dataString) {
        if (this.myKey === '') {
            this.getPublic()
        }
        let RSAData = ''
        if (!this.type || this.type == 'pkcs1') {
            RSAData = this.RSAEncryptOne(dataString)
        } else {
            RSAData = await this.newEncrypt(dataString)
        }
        return RSAData
    }

    /**
     * @description 默认密钥格式 `pkcs1` 时调用
     * @param {string} dataString
     * @return {string} 加密数据
     */
    RSAEncryptOne(dataString: string) {
        let encrypt = new JsEncrypt();
        encrypt.setPublicKey(this.myKey);
        let data = encrypt.encrypt(dataString) || '';
        return data;
    }

    /**
     * @description 密钥格式为 `OAEP-256` 时调用
     * @param {*} dataString
     * @return {Promise<string>} 加密数据
     */
    async newEncrypt(dataString): Promise<string> {
        let cryptoKey = await this.getCryptoKey(this.myKey)
        let RSAData = await this.cryptoEncrypt(cryptoKey, dataString)
        return RSAData
    }

    /**
     * @description 将密钥字符串转化为 `CryptoKey` 对象
     * @param {*} publicKey
     * @return {CryptoKey} CryptoKey
     */
    async getCryptoKey(publicKey) {
        let crypto = window.crypto
        return crypto.subtle.importKey(
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
            ["encrypt"]
        ).then((publicKey) => {
            return publicKey
        })

    }

    /**
     * @description crypto `RSA-OAEP-256` 加密
     * @param {*} cryptoKey 密钥
     * @param {*} dataString 原文
     * @return {Promise<string>}
     */
    async cryptoEncrypt(cryptoKey, dataString): Promise<string> {
        let crypto = window.crypto || window.webkitCrypto || window.mozCrypto || window.oCrypto || window.msCrypto
        return crypto.subtle.encrypt(
            {
                name: "RSA-OAEP",
            },
            cryptoKey,
            new Uint8Array(this.StringToByte(dataString))
        ).then((encrypted) => {
            return this.uint8ArrayToBase64(new Uint8Array(encrypted))
        })
    }

    // 字符串转换为字节数组
    StringToByte(str) {
        var utf8 = unescape(encodeURIComponent(str));

        var arr = [];
        for (var i = 0; i < utf8.length; i++) {
            arr.push(utf8.charCodeAt(i));
        }
        return arr
    }

    uint8ArrayToBase64(data) {
        return window.btoa(Array.from(data).map((c) => String.fromCharCode(c)).join(''));
    }
}

export default new RSAHelper();
