class SuopPopup {
    static #globalLevel = 100
    static get zIndex() {
        this.#globalLevel++
        return this.#globalLevel
    }
    #visible = false
    level
    id
    _content
    get content() {
        return this._content
    }
    set content(newContent) {
        document
            .getElementById(this.id)
            .querySelector(".suop-popup-content").innerHTML = newContent
        this._content = newContent
    }
    createPopupElement() {
        var element = document.createElement("div")
        element.id = this.id
        element.classList.add("intangible")
        element.classList.add("invisible")
        element.innerHTML = `
			<style>
			.invisible {
				opacity: 0;
			}

			.intangible {
				display: none !important;
			}
			#${this.id} {
				position: fixed;
				background-color: rgba(0, 0, 0, 0.8);
				width: 100%;
				height: 100%;
				top: 0;
                left: 0;
				cursor: pointer;
				transition: 0.2s all;
				display: flex;
				justify-content: center;
				align-items: center;
                z-index: ${this.level}
			}

			#${this.id} > .suop-popup-content {
        cursor: auto;
        @media(orientation: landscape) {
          max-width: 80%;
				  max-height: 80%;
        }
			}

      .suop-popup-close-button {
        position: absolute;
        top: 0;
        left: 0;
        transform: scale(100%);/*not sure why this makes it bigger*/
        background-color: rgba(0, 0, 0, 0.5);
        line-height: 0;
      }
			</style>
      <span class="suop-popup-close-button"><svg height="48px" viewBox="0 0 24 24" width="48px" fill="#aeaeae"><path d="M0 0h24v24H0V0z" fill="none"/><path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12 19 6.41z"/></svg></span>
			<div class="suop-popup-content">${this.content}</div>
	`
        return element
    }
    constructor(content = "", deleteOnClose = true) {
        this.level = SuopPopup.zIndex
        this.id = "suop-popup" + this.level
        this._content = content
        var body = document.querySelector("body")
        body.appendChild(this.createPopupElement())
        document.getElementById(this.id).addEventListener("click", (e) => {
            if (e.target.id == this.id) {
                if (deleteOnClose) {
                    this.hideThenDelete()
                } else {
                    this.killPopup()
                }
            }
        })
    }

    toggle() {
        if (this.#visible) {
            this.hidePopup()
        } else {
            this.showPopup()
        }
    }

    showPopup() {
        this.#visible = true
        var popup = document.getElementById(this.id)
        popup.classList.remove("intangible")
        setTimeout(function () {
            popup.classList.remove("invisible")
        }, 1)
    }

    hidePopup() {
        var popup = document.getElementById(this.id)

        this.#visible = false
        popup.classList.add("invisible")
        setTimeout(function () {
            popup.classList.add("intangible")
        }, 200)
    }

    remove(delay) {
        var popup = document.getElementById(this.id)
        setTimeout(() => popup.remove(), delay)
    }

    hideThenDelete() {
        this.hidePopup()
        this.remove(200)
    }
}
