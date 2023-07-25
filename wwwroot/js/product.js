
addEventListener('DOMContentLoaded', () => {
    let priceNode = document.getElementById('final-price')
    const formatter = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
    });
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
        setPerLetter(text, id, cost) {
            this.customization.set(id, text)
            this.priceModifiers.set(id, cost * text.length)
            this.refreshPrice()

        },

        setRadioOption(id, value, cost) {
            this.customization.set(id, value)
            this.priceModifiers.set(id, cost)
            this.refreshPrice()
        },

        refreshPrice() {
            let priceModifierTotal = 0;
            for (let price of this.priceModifiers) {
                priceModifierTotal += price[1]
            }
            priceNode.innerHTML = formatter.format(this.basePrice + priceModifierTotal)
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
            window.suopCart.add(cartItem)
        }
    }

});