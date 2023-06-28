// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function removeElement(id) {
    document.getElementById(id).remove()
}
function deleteProduct(productId) {
    confirmAction('Are you sure you want to delete this product? This cannot be undone.', () => {
        fetch(`${window.location.origin}/api/products/${productId}`,
            { method: 'DELETE' })
            .then((data) => data.text())
            .then((data) => {
                if (data == 'deleted') {
                    removeElement(productId)
                }
            })

    })
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
                Id
                <input type="text" name="Id" value="${crypto.randomUUID()}" required/>
            </span>
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

function editProductForm(id, name, description, categoryId, price, tags) {
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
                    uploadIndicator.setAction(new SuopSnackbar.Action('Refresh', () => window.location = window.location))
                } else {
                    uploadIndicator.text = 'Error: ' + data
                }

                })
            .catch(error => {
                uploadIndicator.text = 'Error: ' + error
            })
        editProductPopup.hideThenDelete()

    })

    editProductPopup.showPopup()
}