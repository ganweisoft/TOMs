import i18n from '../main';
const regTel = /^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$/;
const regName = /^([A-Za-z0-9_]|[\u4e00-\u9fa5]){1,10}$/;

function validateTel(rule, value, callback) {
    if (value === '') {
        callback(new Error(i18n.t('formValidate.empty_phoneNum')));
    } else if (!regTel.test(value)) {
        callback(new Error(i18n.t('formValidate.error_phoneNum')));
    } else {
        callback();
    }
}

function validateName(rule, value, callback) {
    if (value === '') {
        callback(new Error(i18n.t('formValidate.empty_username')));
    } else if (value !== '' && !regName.test(value)) {
        callback(new Error(i18n.t('formValidate.length_username')));
    } else {
        callback();
    }
}

export {validateTel, validateName}
