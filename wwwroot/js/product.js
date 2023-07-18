
addEventListener('DOMContentLoaded', () => {
    let priceNode = document.getElementById('final-price')
    const formatter = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
    });
    let counter = 0
    window.suopProduct = {
        displayPrice: 0,
        addons: new Set(),
        customization: {},
        toggleAddon(id, price) {
            //onclick events on labels trigger twice
            counter++

            if (counter % 2 != 0) {
                return;
            }
            console.log(this.displayPrice)
            if (this.addons.has(id)) {
                this.displayPrice -= price
                this.addons.delete(id)
            } else {
                this.displayPrice += price
                this.addons.add(id)
            }
            console.log(this.displayPrice)

            this.refreshPrice()
        },

        refreshPrice() {
            priceNode.innerHTML = formatter.format(this.displayPrice)
        }
    }

});