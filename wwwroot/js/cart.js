class Cart {
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
                        this.items = items.filter(filterItem => (filterItem.id != id))
                    }
                    this.#save()
                    this.#updateCartNumbers()
                }
            })

        }
    }

    #setHoverCarts() {
        function showHoverCart(node) {
            node.suopCartShown = true
            //create the cart
            let hoverCart = new SuopPopup('', {
                floating: true, background: 'white',
                x: node.offsetLeft + node.offsetWidth, y: node.offsetTop + node.offsetHeight
            })
            //add items to the cart
            fillHoverCart(hoverCart)
            hoverCart.node.style.translate = "-100%"
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
        function fillHoverCart(popup) {
            let contentContainer = popup.node.querySelector('.suop-popup-content')
            if (contentContainer == null) return;
            for (let item of window.suopCart.items) {
                let itemNode = document.createElement('span')
                itemNode.classList.add('shopping-popup-item')
                contentContainer.append(itemNode)
                getProduct(item.id).then((itemInfo) => {
                    itemNode.innerHTML = `
                        ${(() => {
                            let html = ''
                            for (let image of itemInfo.Images) {
                                html += `<img src="${image}">`
                            }
                            return html
                        })()}
                        ${itemInfo.Name}
                    `
                })

            }
            let checkoutButton = document.createElement('button')
            contentContainer.append(checkoutButton)
            checkoutButton.outerHTML = '<button id="checkout-button" onclick="window.suopCart.submitCart()">Checkout</button>'
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

    submitCart() {
        let stripeLoading = SuopSnack.add('Bringing you to stripe...', Infinity, null, true)
        fetch(window.location.origin + '/api/checkout', {
            method: 'post',
            headers: {
                'cancelUrl': window.location,
                'successUrl': window.location.origin + '/success'
            },
            body: JSON.stringify(this.items)
        })
        .then(response => response.text())
        .then(data => {

            window.location = JSON.parse(data)['Location']
        })
            .catch(error => {
                stripeLoading.text = '<i style="color:red">Error:</i> ' + error
            console.log(error)
        })
    }
}

class CartItem {
    id = ''
    quantity = 1
    children = []
    constructor(id = '', quantity = 1, children = []) {
        this.id = id
        this.quantity = quantity
        this.children = children
    }
}