document.addEventListener('DOMContentLoaded', () => {
    function loadFont(url, alias) {
        let link = document.createElement('link');
        link.setAttribute('rel', 'stylesheet');
        link.setAttribute('type', 'text/css');
        link.setAttribute('href', url);
        document.head.appendChild(link);

        let fontDefinition = document.createElement('style')
        fontDefinition.innerHTML = `
            @font-face {
                font-family: ${alias};
                src: url(${url});
            }
        `
        document.head.appendChild(fontDefinition)
    }

    function updatePreview(text, element) {
        let fontName = element.suopFont ?? ''
        element.innerHTML = `
            <span class="label">${fontName}
            </span>
            <span class="preview" style="font-family: ${fontName}">${text}</span>
        `
    }

    const fonts = ['Adrian', 'Alexis', 'Amaya', 'Audrey', 'Boston', 'Bowen', 'Brody', 'Brooke', 'Brooklyn', 'Camila', 'Danielle', 'Delilah', 'Destiny', 'Eleanor', 'Esther', 'Evelyn', 'EVERETT', 'Fiona', 'FORREST', 'Gloria', 'GREYSON', 'Hadley', 'Huntley', 'Isabella', 'Jasmine', 'Juliana', 'Laura', 'Layla', 'LEMON', 'LUCILLE', 'Melissa', 'Monica', 'Morgan', 'RACHEL', 'Randy', 'Rebecca', 'Rylee', 'Selena', 'Serenity', 'SHIRLEY', 'Sienna', 'Simon', 'Sydney', 'Taylor', 'TWANNA', 'Valentina', 'VICTORIA', 'Willow']
    const previews = []

    console.log('test')
    let previewContainer = document.querySelector('.font-previews')
    for (let font of fonts) {
        loadFont(`fonts/a${font}.ttf`, font)

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
