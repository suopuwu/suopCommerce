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
    if (suopCache.has('product' + id)) return suopCache.get('product' + id)
    let result = fetch(`${window.location.origin}/api/products/${id}`)
        .then((response) => response.text())
        .then((data) => JSON.parse(data))
    suopCache.set('product' + id, result)
    return result
}
//todo make sure the password makes you confirm it and type it twice.
//ensure that the user can change their password through the database
function getImage(id) {
    if (suopCache.has('image' + id)) return suopCache.get('image' + id)

    suopCache.set(
        'image' + id,
        fetch(`${window.location.origin}/api/images/${id}`)
            .then((response) => response.text())
            .then((data) => JSON.parse(data))
    )
    let result = suopCache.get('image' + id)
    return result
}

function confirmAction(text, callback, autoConfirm = false) {
    if (autoConfirm) {
        callback()
        return
    } //todo make it so you can have commas in extras.
    //todo make popup x button work consistently
    var confirmationPopup = new SuopPopup(text, {
        confirm: () => {
            callback()
            confirmationPopup.hideThenDelete()
        },
        cancel: () => confirmationPopup.hideThenDelete(),
        background: 'white',
    })
    confirmationPopup.toggle()
}
//todo Currently, a product whose price is edited will not update prices already in customers' carts. Decide whether this is worth adding or not.
//you could just make cookies expire every week or something to mitigate this issue.
function formatPrice(price) {
    const formatter = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
    })
    return formatter.format(price)
}

let suopCache = new Map()

function lightboxImage(url, description = '') {
    let lightbox = new SuopPopup(`
        <div class="lightbox-wrapper">
            <img src="${url}" class="lightboxed-image">
            <span class="lightbox-description">${description}</span>
        </div>
    `)
    lightbox.showPopup()
}
