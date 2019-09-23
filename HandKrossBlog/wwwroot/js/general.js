
function showAlert(element, duration_ms) {
    document.querySelector(element).classList.remove("d-none")
    setTimeout(function () { document.querySelector(element).classList.add("d-none") }, duration_ms)
}

function getApiPost(url, formData) {
    var request = {
        method: 'POST',
        body: formData
    }

    return fetch(url, request)
        .then(function (res) {
            return res.json()
        })
        .then(function (result) {
            return result
        })
        .catch(function (err) {
            alert(err)
        })
}