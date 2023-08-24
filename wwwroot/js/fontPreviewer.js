document.addEventListener('DOMContentLoaded', () => {
    function loadFont(url) {
        var link = document.createElement('link');
        link.setAttribute('rel', 'stylesheet');
        link.setAttribute('type', 'text/css');
        link.setAttribute('href', url);
        document.head.appendChild(link);
    }

    function updatePreview(text, element) {
        let fontName = element.suopFont ?? ''
        element.innerHTML = `
            <span class="label">${fontName}</span>
            <span class="preview" style="font-family: ${fontName}">${text}</span>
        `
    }

    const urls = ['https://fonts.googleapis.com/css2?family=Montserrat&display=swap']
    const fonts = ['consolas', 'arial', 'Montserrat']
    const previews = []

    for (let url of urls) {
        loadFont(url)
    }


    console.log('test')
    let previewContainer = document.querySelector('.font-previews')
    for (let font of fonts) {
        let preview = document.createElement('span')
        preview.classList.add('font-preview')
        preview.suopFont = font

        updatePreview(font, preview)
        previews.push(preview)
        previewContainer.append(preview)
    }

    document.querySelector('#font-previewer-input').addEventListener('input', (e) => {
        console.log()
        for (let preview of previews) {
            updatePreview(e.target.value, preview)
        }
    })
}) 
