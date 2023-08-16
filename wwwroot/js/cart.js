//the only instance of this class must be window.suopCart
class Cart {
    //paths to not show hover carts on
    #hiddenPaths = ['/checkout']

    #validateCartItemsAndRender() {
        let items = this.items
        let invalidIds = new Set()
        let itemCount = items.length
        let validatedItems = 0
        for (let item of items) {
            getProduct(item.id).then((itemInfo) => {
                validatedItems++
                if (itemInfo.Name == null) {
                    invalidIds.add(item.id)
                }
                if (validatedItems == itemCount) {
                    for (let id of invalidIds) {
                        this.items = items.filter(
                            (filterItem) => filterItem.id != id
                        )
                    }
                    this.#save()
                    this.#updateCartNumbers()
                }
            })
        }
    }

    #setHoverCarts() {
        let cart = this
        function showHoverCart(node) {
            if (
                cart.items.length == 0 ||
                cart.#hiddenPaths.includes(window.location.pathname)
            )
                return
            node.suopCartShown = true
            //create the cart
            let hoverCart = new SuopPopup('', {
                floating: true,
                background: 'white',
                x: node.offsetLeft + node.offsetWidth,
                y: node.offsetTop + node.offsetHeight,
            })
            //add items to the cart
            fillHoverCart(hoverCart)
            hoverCart.node.style.translate = '-100%'
            //set the cart to disappear on mouse leave
            hoverCart.node.addEventListener('mouseleave', () => {
                node.suopCartShown = false
                node.mouseEnteredHoverCart = false
                hoverCart.hideThenDelete()
            })
            hoverCart.node.addEventListener('mouseenter', () => {
                node.mouseEnteredHoverCart = true
            })
            hoverCart.toggle()
            return hoverCart
        }
        async function fillHoverCart(popup) {
            let contentContainer = popup.node.querySelector(
                '.suop-popup-content'
            )
            if (contentContainer == null) return
            for (let index = 0; index < cart.items.length; index++) {
                let item = cart.items[index]
                //immediately add the element to preserve the order in which the products were added
                let itemNode = document.createElement('span')
                itemNode.classList.add('shopping-popup-item')
                contentContainer.append(itemNode)

                //add data to the cart items
                getProduct(item.id).then(async (itemInfo) => {
                    itemNode.id = 'cartItem' + index
                    itemNode.innerHTML = `
                        ${await (async () => {
                            let html = ''
                            //add each image, waiting for each one to load consecutively
                            for (let image of itemInfo.Images) {
                                let tempImage = await getImage(image)
                                html += `<span class="cart-item-image" style="background-image: url('${tempImage.Url}')"></span>`
                                break //todo make this just grab the first image instead of breaking after one loop
                            }
                            return html
                        })()}
                        <div>
                            <div>
                            ${itemInfo.Name}
                            </div>
                            <div>${formatPrice(item.displayPrice)}</div>
                        </div>
                        <div class="flex-spacer"></div>
                        <button onclick="
                            window.suopCart.tryRemove(${index});
                            document.getElementById('${itemNode.id}').remove()
                        " class="rounded-square-button">delete</button>
                    `
                })
            }
            let checkoutButton = document.createElement('button')
            contentContainer.append(checkoutButton)
            let cartTotal = 0
            for (let item of cart.items) {
                cartTotal += item.displayPrice
            }
            checkoutButton.outerHTML = `
                <div>
                    <button id="checkout-button" class="rounded-square-button" onclick="window.suopCart.submitCart()">Checkout</button>
                    <span>${formatPrice(cartTotal)}</span>
                </div>
            `
            //todo make the back button take you to whatever prior page.
        }

        document.querySelectorAll('.cart-hover-target').forEach((node) => {
            let hoverCart
            node.addEventListener('mouseenter', () => {
                if (!node.suopCartShown) {
                    hoverCart = showHoverCart(node)
                }
            })
            node.addEventListener('mouseleave', () => {
                if (hoverCart) {
                    setTimeout(() => {
                        if (!node.mouseEnteredHoverCart) {
                            hoverCart.hideThenDelete()
                            hoverCart = null
                            node.suopCartShown = false
                        }
                    }, 50)
                }
            })
        })
    }

    #save() {
        storeCookie('cart', this.items)
    }

    #updateCartNumbers() {
        document.querySelectorAll('.cart-product-number').forEach((node) => {
            node.innerHTML = this.items.length
        })
    }

    #cacheCartItems() {
        for (let item of this.items) {
            getProduct(item.id).then((data) => {
                for (let imageId of data.Images) {
                    getImage(imageId)
                }
            })
        }
    }

    items
    constructor() {
        try {
            this.items = retrieveCookie('cart') ?? []
        } catch {
            this.items = []
            this.#save()
        }
        this.#validateCartItemsAndRender()
        this.#setHoverCarts()
        this.#cacheCartItems()
    }

    clear() {
        this.items = []
        this.#updateCartNumbers()
        this.#save()
    }

    add(cartItem) {
        this.items.push(cartItem)
        this.#updateCartNumbers()
        this.#save()
    }

    tryRemove(index) {
        if (index >= this.items.length) return false

        this.items.splice(index, 1)
        this.#updateCartNumbers()
        this.#save()
        return true
    }

    submitCart() {
        let stripeLoading = SuopSnack.add(
            'Bringing you to stripe...',
            Infinity,
            null,
            true
        )
        fetch(window.location.origin + '/api/checkout', {
            method: 'post',
            headers: {
                cancelUrl: window.location,
                successUrl: window.location.origin + '/success',
            },
            body: JSON.stringify(this.items),
        })
            .then((response) => response.text())
            .then((data) => {
                console.log(data)
                let parsedData = JSON.parse(data)
                if (!parsedData.success) {
                    throw new Error(parsedData.message)
                }
                window.location = JSON.parse(data)['Location']
            })
            .catch((error) => {
                console.log(error)
                stripeLoading.text = '<i style="color:red">Error:</i> ' + error
            })
    }
}

class CartItem {
    id = 0
    quantity = 1
    children = []
    customization = []
    displayPrice = 0
    constructor(id = 0, quantity = 1, children = [], customization = []) {
        this.id = id
        this.quantity = quantity
        this.children = children
        this.customization = customization
    }
}
