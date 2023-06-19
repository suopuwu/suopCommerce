// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function removeElement(id) {
    document.getElementById(id).remove()
}
function deleteProduct(productId) {
    fetch(`${window.location.origin}/products/${productId}`,
        { method: 'DELETE' })
        .then((data) => data.text())
        .then((data) => {
            if (data == 'deleted') {
                removeElement(productId)
            }
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
            <h1>Add product</h1>

            <span class="text-input">
                Id
                <input type="text" name="Id" value="${crypto.randomUUID()}" required/>
            </span>
            <span class="text-input">
        
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
                <input type="number" name="Price" value="0" required />
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
    const submitUrl = window.location.origin + '/products' // Define the URL here

    form.addEventListener('submit', e => {
        e.preventDefault() // Prevent the form from submitting normally

        const formData = new FormData(form); // Get the form data
        var uploadIndicator = suopSnackbar.add('Uploading product...', Infinity)

        fetch(submitUrl, { // Use the defined URL
            method: form.method,
            body: formData
        })
            .then(response => response.text())
            .then(data => {
                if (data == 'success') {

                    uploadIndicator.text = 'Successfully added new product. Refresh to see changes.'
                    uploadIndicator.setAction(new SuopSnackbarAction('refresh', () => window.location = window.location))
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