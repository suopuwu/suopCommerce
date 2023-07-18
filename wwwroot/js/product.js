
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
        addons: new Set(),
        customization: {},
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
        setPerLetter(text, id, cost) {
            this.customization[id] = text
            this.priceModifiers.set('id', cost * text.length)
            this.refreshPrice()

        },

        refreshPrice() {
            let priceModifierTotal = 0;
            for (let price of this.priceModifiers) {
                priceModifierTotal += price[1]
            }
            priceNode.innerHTML = formatter.format(this.basePrice + priceModifierTotal)
        },
    }

});