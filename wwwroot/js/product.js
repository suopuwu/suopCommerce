
addEventListener('DOMContentLoaded', () => {
    let priceNode = document.getElementById('final-price')
    let counter = 0
    window.suopProduct = {
        priceModifiers: new Map(),
        basePrice: 0,
        id: -1,
        addons: new Set(),
        customization: new Map(),
        toggleAddon(id, price) {
            //onclick events on labels trigger twice
            counter++

            if (counter % 2 != 0) {
                return;
            }
            if (this.addons.has(id)) {
                this.priceModifiers.delete(id)
                this.addons.delete(id)
            } else {

                this.priceModifiers.set(id, price)
                this.addons.add(id)
            }
            this.refreshPrice()
        },
        //todo make selected addons more obvious
        setPerLetter(text, id, cost, freeLetters) {
            console.log(freeLetters)
            this.customization.set(id, text)
            this.priceModifiers.set(id, cost * Math.max(text.length - freeLetters, 0))
            console.log(Math.max(text.length - freeLetters, 0))
            this.refreshPrice()

        },

        setRadioOption(id, value, cost) {
            this.customization.set(id, value)
            this.priceModifiers.set(id, cost)
            this.refreshPrice()
        },

        getPriceTotal() {
            let priceModifierTotal = 0;
            for (let price of this.priceModifiers) {
                priceModifierTotal += price[1]
            }
            return this.basePrice + priceModifierTotal
        },

        refreshPrice() {
            priceNode.innerHTML = formatPrice(this.getPriceTotal())
        },

        addToCart() {
            //changes the product as it is currently configured into a more json readable form, then adds it to the cart.
            let cartItem = new CartItem(this.id)
            for (let field of this.customization) {
                cartItem.customization.push(field)
            }

            for (let addonId of this.addons) {
                cartItem.children.push(new CartItem(addonId, 1))
            }
            //this truly is just a display price. Modifying it will not change the price stripe charges.
            cartItem.displayPrice = this.getPriceTotal()
            window.suopCart.add(cartItem)
        }
    }

});