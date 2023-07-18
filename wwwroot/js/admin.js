let suopCounter = function() {
    _counter = 0;
    return function () {
        _counter++
        return _counter
    }
}()

function retrieveSetting(name) {
    switch (name) {
        case 'confirm-deletion':
            return document.getElementById('confirm-deletion-checkbox').checked
            break
        case 'delete-from-product':
            return document.getElementById('delete-images-from-products-checkbox').checked
            break
        case 'delete-product-images':
            return document.getElementById('delete-product-images-checkbox').checked
            break
        default:
            return false            
    }
}

function removeElement(id) {
    document.getElementById(id).remove()
}
function deleteProduct(productId) {
    confirmAction('Are you sure you want to delete this product? This cannot be undone.', () => {
        let deletionIndicator = SuopSnack.add('Deleting...', Infinity, null, true)
        fetch(`${window.location.origin}/api/products/${productId}`,
            { method: 'delete', headers: { 'delete-images': retrieveSetting('delete-product-images') } })
            .then((data) => data.text())
            .then((data) => {
                if (data == 'deleted') {
                    removeElement(`product_${productId}`)
                    deletionIndicator.text = 'Successfully deleted a product.'
                    deletionIndicator.close(1000)

                } else {
                    deletionIndicator.text = 'error: ' + data

                }
            })

    }, !retrieveSetting('confirm-deletion')
        //todo make images work again
    )
}//todo make suopPopup not right aligned for the content within it

function deleteImage(id) {
    confirmAction('Are you sure you want to delete this image? This cannot be undone. Deleting images that are in use will cause broken pages.', () => {
        let deletionIndicator = SuopSnack.add('Deleting...', Infinity, null, true)
        fetch(`${window.location.origin}/api/images`,
            { method: 'delete', headers: { 'id': id, 'delete-from-product': retrieveSetting('delete-from-product') } })
            .then((data) => data.text())
            .then((data) => {
                if (data.split(' ')[0] == 'deleted') {
                    removeElement(`image_${id}`)
                    deletionIndicator.close(1000)
                } else {
                    deletionIndicator.text = 'error: ' + data
                }
            })

    }, !retrieveSetting('confirm-deletion'))
}

const formModes = {
    uploadImage: 0,
    editImage: 1,
    createProduct: 2,
    editProduct: 3
}
function popupForm(mode, data = {}) {
    let idNumber = suopCounter();
    if (mode == null) {
        SuopSnack.add('Error: popup form made without a mode specified.')
        return
    }
    var formPopup = new SuopPopup('', {background: 'white'})
    var infoSnackbar
    //create the form element, changing depending on the mode
    function createFormNode() {
        const form = document.createElement('form')
        formPopup.contentNode.append(form)


        //creates the node containing the title and input elements. Returns the input node.
        function createInputNode(text, name, value = '', required = true, type = 'text') {
            let wrapperNode = document.createElement('span')
            form.append(wrapperNode)
            wrapperNode.innerHTML = text
            wrapperNode.classList.add('text-input')

            let inputNode = document.createElement('input')
            wrapperNode.append(inputNode)
            inputNode.id = name + idNumber;

            inputNode.type = type
            if (type == 'file') {
                inputNode.multiple = 'multiple'
                inputNode.accept = '.png, .jpg, .jpeg, .webp'
            } else if (type == 'number') {
                inputNode.step = '0.01'
                inputNode.min = '0.5'
                inputNode.max = '99999999'
            }
            inputNode.name = name
            inputNode.required = required
            if (type != 'file') {
                inputNode.setAttribute('value', value)
            }

            return inputNode
        }

        switch (mode) {
            case formModes.uploadImage:
                form.innerHTML = `<div><h1>Upload Images</h1></div>`
                createInputNode('Upload', 'Images', '',  true, 'file')
                form.method = 'post'
                form.submitUrl = '/api/images'
                break
            case formModes.editImage:
                form.innerHTML = `<div><h1>Edit Image</h1></div>`
                createInputNode('Description', 'Description', data.description)
                form.method = 'post'
                form.submitUrl = '/api/images/' + data.id
                break
            case formModes.createProduct:
            case formModes.editProduct:
                form.innerHTML = `<div><h1>${mode == formModes.createProduct ? 'Create' : 'Edit'} Product</h1></div>`
                createInputNode('Name', 'Name', data.name ?? 'Example name')
                descriptionNode = createInputNode('Description', 'Description', data.description ?? 'Example description')
                createInputNode('Category', 'Category', data.category ?? '', false)
                createInputNode('Price', 'Price', data.price ?? '0.5', true, 'number')
                createInputNode('Tags', 'Tags', data.tags ?? '', false)
                createInputNode('Addons(only numbers, spaces, and commas)', 'Addons', data.addons ?? '', false)
                createInputNode('Extras', 'Extras', data.extras ?? '', false)
                createInputNode('Images', 'Images', data.images ?? '', false)
                form.method = 'post'
                form.submitUrl = '/api/products'
                if (mode == formModes.editProduct) {
                    form.submitUrl = '/api/products/' + data.id
                }
                break
        }
        form.innerHTML += '<button type="submit" class="rounded-square-button">Submit</button>'
        form.style.width = '700px'
        form.enctype = 'multipart/form-data'
        form.classList.add('create-product-form')
        return form
    }//todo make ui to choose products or images
    function submitStart() {
        switch (mode) {
            case formModes.uploadImage:
                infoSnackbar = SuopSnack.add('Uploading image...', Infinity)
                break
            case formModes.editImage:
                infoSnackbar = SuopSnack.add('Editing image...', Infinity)
                break
            case formModes.createProduct:
                infoSnackbar = SuopSnack.add('Adding product...', Infinity)
                break
            case formModes.editProduct:
                infoSnackbar = SuopSnack.add('Editing product...', Infinity)
                break
        }
    }//todo move the rest of the api to a proper restful api

    function handleResponse(response) {

        switch (mode) {
            case formModes.uploadImage:
                infoSnackbar.text = `Successfully uploaded ${response.data.length} image${response.data.length == 1 ? '' : 's'}. Refresh to see changes.`
                infoSnackbar.setAction(new SuopSnack.Action('Refresh', () => window.location = window.location))
                break
            case formModes.createProduct:
                infoSnackbar.text = `Successfully created product. Refresh to see changes.`
                infoSnackbar.setAction(new SuopSnack.Action('Refresh', () => window.location = window.location))
                break
            case formModes.editProduct:
                infoSnackbar.text = `Successfully edited product. Refresh to see changes.`
                infoSnackbar.setAction(new SuopSnack.Action('Refresh', () => window.location = window.location))
                break
            case formModes.editImage:
                infoSnackbar.close(1000)
                infoSnackbar.text = 'Successfully edited the description.'
                document.querySelector('#image_' + data.id)
                    .querySelector('.admin-description')
                    .innerHTML = response.newDescription
                break
        }
     }
    function handleFailure(error) {
        switch (mode) {
            default:
                if (infoSnackbar == undefined)
                    infoSnackbar = SuopSnack.add()
                infoSnackbar.text = '<i style="color: #FF5252;">Error</i>: ' + error

                break
        }
    }
    const form = createFormNode()
    //change the description field to a markdown editor, then replace <br>s with newlines
    let editor = new EasyMDE({
        element: document.getElementById('Description' + idNumber),
        forceSync: true,
        toolbar: ['bold', 'italic', 'heading-bigger', 'heading-smaller', 'strikethrough', '|', 'quote', 'code', 'unordered-list', 'ordered-list', '|', 'undo', 'redo'],
        initialValue: data.description,
    })

    //todo make it so the frontend can handle ` characters

    //handle form submission
    form.addEventListener('submit', e => {
        e.preventDefault() // Prevent the form from submitting normally

        const formData = new FormData(form); // Get the form data

        //reformat form
        if (formData.has('Description')) {
            formData.set('Description', editor.value())
        }

        //on submisison, depending on the mode
        submitStart()

        fetch(form.submitUrl, {
            method: form.method,//todo refactor to reuse this function for all forms
            body: formData
        })
            .then(response => response.text())
            .then(data => {
                //handle response, depending on the mode
                let parsedData = JSON.parse(data)
                console.log(parsedData)
                if (parsedData.success) {
                    handleResponse(parsedData)
                } else {
                    handleFailure(parsedData)
                }


            })
            .catch(error => {
                //handle failure, depending on the mode
                handleFailure(error)
            })
        formPopup.hideThenDelete()

    })

    formPopup.showPopup()
}