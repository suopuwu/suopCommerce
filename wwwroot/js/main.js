document.addEventListener('DOMContentLoaded', () => {
    window.suopCart = new Cart()
})

function storeCookie(key, value) {
    Cookies.set(key, JSON.stringify(value))
}

function retrieveCookie(key) {
    try {
        return JSON.parse(Cookies.get(key) ?? null)
    } catch {
        return null
    }   
}


function getProduct(id) {
    return fetch(`${window.location.origin}/api/products/${id}`)
        .then((response) => response.text())
        .then(data => JSON.parse(data))
}

function getImage(id) {
    return fetch(`${window.location.origin}/api/images/${id}`)
        .then((response) => response.text())
        .then(data => JSON.parse(data))
}

function confirmAction(text, callback, autoConfirm = false) {
    if (autoConfirm) {
        callback()
        return
    }
    var confirmationPopup = new SuopPopup(text, {
        confirm: () => {
            callback();
            confirmationPopup.hideThenDelete()
        },
        cancel: () => confirmationPopup.hideThenDelete(),
        background: 'white'
    })
    confirmationPopup.toggle()
}