class Cart {
    constructor() {


        this.items = retrieveCookie('cart') ?? []
        

        this.updateCartNumbers()
        this.setHoverCarts()
    }

    save() {
        storeCookie('cart', this.items)
    }

    updateCartNumbers() {
        document.querySelectorAll('.cart-product-number').forEach((node) => {
            node.innerHTML = this.items.length
        })
    }

    setHoverCarts() {
        function showHoverCart(node) {
            node.suopCartShown = true
            //create the cart
            var hoverCart = new SuopPopup('', {
                floating: true, background: 'white',
                x: node.offsetLeft + node.offsetWidth, y: node.offsetTop + node.offsetHeight
            })
            //add items to the cart
            fillHoverCart(hoverCart)
            hoverCart.node.style.translate = "-100%"
            //set the cart to disappear on mouse leave
            hoverCart.node.addEventListener('mouseleave', () => {
                node.suopCartShown = false
                hoverCart.hideThenDelete()
            })
            hoverCart.toggle()
        }
        function fillHoverCart(popup) {
            for (let item of window.suopCart.items) {
                getProduct(item.id).then((itemInfo) => {
                    console.log(itemInfo)
                    popup.content += `
                    <span class="shopping-popup-item">
                        ${(() => {
                        var html = ''
                        for (let image of itemInfo.Images) {
                            html += `<img src="${image}">`
                        }
                        return html
                        })()}
                        ${itemInfo.Name}
                    </span>
                `
                })
                
            }
        }

        document.querySelectorAll('.cart-hover-target').forEach((node) => {
            node.addEventListener('mouseenter', () => {
                if (!node.suopCartShown) {
                    showHoverCart(node)
                }

            })
        })
    }

    add(cartItem) {
        this.items.push(cartItem)
        this.updateCartNumbers()
        this.save()
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