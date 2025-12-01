import Element from 'element-ui'
let duration = 1200
let message = function (data) {
    return Element.Message({
        message: data.message,
        type: data.type || 'success',
        duration: duration
    })
}

message.warning = function (msg) {
    return Element.Message({
        message: msg,
        type: 'warning',
        duration: duration
    })
}

message.success = function (msg) {
    return Element.Message({
        message: msg,
        type: 'success',
        duration: duration
    })
}

message.error = function (msg) {
    return Element.Message({
        message: msg,
        type: 'error',
        duration: duration
    })
}
message.closeAll = function () {
    return Element.Message.closeAll()
}

export default message