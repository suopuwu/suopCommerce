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