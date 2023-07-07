function retrieveSetting(name) {
    switch (name) {
        case 'confirm-deletion':
            return document.getElementById('confirm-deletion-checkbox').checked
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
        fetch(`${window.location.origin}/api/products/${productId}`,
            { method: 'delete' })
            .then((data) => data.text())
            .then((data) => {
                if (data == 'deleted') {
                    removeElement(`product_${productId}`)
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
            { method: 'delete', headers: { 'id': id } })
            .then((data) => data.text())
            .then((data) => {
                if (data.split(' ')[0] == 'deleted') {
                    removeElement(`image_${id}`)
                    deletionIndicator.close()
                } else {
                    deletionIndicator.text = 'error: ' + data
                }
            })

    }, !retrieveSetting('confirm-deletion'))
}

function createProductForm() {
    var createProductPopup = new SuopPopup(`
        <style>
            .create-product-form {
                padding: 10px;
                border-radius: 10px;
                background-color: white;
            }
            
        </style>

        <form enctype="multipart/form-data" method="post" class="create-product-form">
            <div><h1>Add product</h1></div>
            <span class="text-input">
                Name
                <input type="text" name="Name" value="temp" required />
            </span>
            <span class="text-input">
                Description
                <input type="text" name="Description" value="temp" required>
            </span>
            <span class="text-input">
                Category Id
                <input type="text" name="CategoryId"/>
            </span>
            <span class="text-input">
                Price
                <input type="number" name="Price" value="0.5" step="0.01" min="0.5" max="99999999"  required />
            </span>
            <span class="text-input">
                Tags
                <input type="text" name="Tags"/>
            </span>
            <span class="text-input">
                Extras
                <input type="text" name="Extras"/>
            </span>
            <span class="text-input">
                Add ons
                <input type="text" name="Addons"/>
            </span>
            <span class="text-input">
                Images
                <input type="file" name="Images" multiple="multiple" accept=".png, .jpg, .jpeg, .webp"/>
                <i class="warning">Note: make sure only trusted users have access to this page, as uploads are not validated beyond extension.</i>
            </span>
            <button type="submit" class="btn btn-default">Register</button>
        </form>
    `)

    const form = document.querySelector('.create-product-form')
    const submitUrl = window.location.origin + '/api/products'

    form.addEventListener('submit', e => {
        e.preventDefault() // Prevent the form from submitting normally

        const formData = new FormData(form); // Get the form data
        var uploadIndicator = SuopSnack.add('Uploading product...', Infinity)

        fetch(submitUrl, { // Use the defined URL
            method: form.method,
            body: formData
        })
            .then(response => response.text())
            .then(data => {
                if (data == 'success') {

                    uploadIndicator.text = 'Successfully added new product. Refresh to see changes.'
                    uploadIndicator.setAction(new SuopSnack.Action('Refresh', () => window.location = window.location))
                } else {
                    uploadIndicator.text = 'Error: ' + data
                }

            })
            .catch(error => {
                uploadIndicator.text = 'Error: ' + error
            })
        createProductPopup.hideThenDelete()

    })

    createProductPopup.showPopup()
}

function editProductForm(id, name, description, categoryId, price, tags, extras, addons) {
    var editProductPopup = new SuopPopup(`
        <style>
            .update-product-form {
                padding: 10px;
                border-radius: 10px;
                background-color: white;
                width: 700px;
            }
            
        </style>

        <form enctype="multipart/form-data" method="post" class="update-product-form">
            <div><h1>Edit product</h1></div>

            <span class="text-input">
                Id
                <input type="text" name="Id" value="${id}" required/>
            </span>
            <span class="text-input">
                Name
                <input type="text" name="Name" value="${name}" required />
            </span>
            <span class="text-input">
                Description
                <input type="text" name="Description" value="${description}" required>
            </span>
            <span class="text-input">
                Category Id
                <input type="text" name="CategoryId" value="${categoryId ?? ''}"/>
            </span>
            <span class="text-input">
                Price
                <input type="number" name="Price" value="${price}" step="0.01" min="0.5" max="99999999"  required />
            </span>
            <span class="text-input">
                Tags
                <input type="text" name="Tags" value="${tags}"/>
            </span>
            <span class="text-input">
                Extras
                <input type="text" name="Extras" value="${extras}"/>
            </span>
            <span class="text-input">
                Add ons
                <input type="text" name="Addons" value="${addons}"/>
            </span>
            <button type="submit" class="btn btn-default">Save Changes</button>
        </form>
    `)

    const form = document.querySelector('.update-product-form')
    const submitUrl = window.location.origin + '/api/products/' + id

    form.addEventListener('submit', e => {//todo refactor to reduce reusage of code
        e.preventDefault()

        const formData = new FormData(form);
        var uploadIndicator = SuopSnack.add('updating product...', Infinity)

        fetch(submitUrl, {
            method: form.method,
            body: formData
        })
            .then(response => response.text())
            .then(data => {
                if (data == 'success') {

                    uploadIndicator.text = 'Successfully updated product. Refresh to see changes.'
                    uploadIndicator.setAction(new SuopSnack.Action('Refresh', () => window.location = window.location))
                } else {
                    uploadIndicator.text = 'Error: ' + data
                }

            })
            .catch(error => {
                uploadIndicator.text = 'Error: ' + error
                console.error(error)
            })
        editProductPopup.hideThenDelete()

    })

    editProductPopup.showPopup()
}

function imageForm() {
    var formPopup = createBaseFormPopup()

    //make the form with input boxes
    const form = document.createElement('form')
    form.innerHTML = `
        <div><h1>Upload Images</h1></div>
        <span class="text-input">
            <input type="file" name="Images" multiple="multiple" accept=".png, .jpg, .jpeg, .webp" required/>
            <i class="warning">Note: make sure only trusted users have access to this page, as uploads are not validated beyond extension.</i>
        </span>
        <button type="submit" class="btn btn-default">Register</button>
    `
    form.enctype = 'multipart/form-data'
    form.classList.add('create-product-form')
    form.method = 'post'
    formPopup.contentNode.append(form)


    const submitUrl = window.location.origin + '/api/images'

    //handle form submission
    form.addEventListener('submit', e => {
        e.preventDefault() // Prevent the form from submitting normally

        const formData = new FormData(form); // Get the form data
        var uploadIndicator = SuopSnack.add('Uploading image...', Infinity)

        fetch(submitUrl, {
            method: form.method,//todo refactor to reuse this function for all forms
            body: formData
        })
            .then(response => response.text())
            .then(data => {
                if (JSON.parse(data).length > 0) {

                    uploadIndicator.text = `Successfully uploaded ${JSON.parse(data).length} image${JSON.parse(data).length == 1 ? '' : 's'}. Refresh to see changes.`
                    uploadIndicator.setAction(new SuopSnack.Action('Refresh', () => window.location = window.location))
                } else {
                    uploadIndicator.text = 'Error: ' + data
                }

            })
            .catch(error => {
                uploadIndicator.text = 'Error: ' + error
            })
        formPopup.hideThenDelete()

    })

    formPopup.showPopup()
}

function createBaseFormPopup() {
    return new SuopPopup(`
        <style>
            .create-product-form {
                padding: 10px;
                border-radius: 10px;
                background-color: white;
            }
            
        </style>
    `)
}