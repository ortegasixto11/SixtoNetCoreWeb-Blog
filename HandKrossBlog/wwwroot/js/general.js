
function showAlert(element, duration_ms) {
    document.querySelector(element).classList.remove("d-none")
    setTimeout(function () { document.querySelector(element).classList.add("d-none") }, duration_ms)
}

function ajaxPost(url, formData) {
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

function showElementById(elementId) {
    document.getElementById(elementId).classList.remove('d-none')
}

function hideElementById(elementId) {
    document.getElementById(elementId).classList.add('d-none')
}

function clearInputById(inputId) {
    document.getElementById(inputId).value = ''
}

function setValueElementById(elementId, value) {
    document.getElementById(elementId).value = value
}

function getValueElementById(elementId) {
    return document.getElementById(elementId).value
}

function setHtmlElementById(elementId, html) {
    document.getElementById(elementId).innerHTML = html
}

function appendHtmlElementById(elementId, html) {
    document.getElementById(elementId).innerHTML += html
}

function clearChildsElementById(elementId) {
    let element = document.getElementById(elementId)
    while (element.firstChild) element.removeChild(element.firstChild)
    element = null
}

function clearAndLoadChildsElementById(elementId, viewChilds) {
    clearChildsElementById(elementId)
    document.getElementById(elementId).innerHTML = viewChilds
}

function clickElementById(elementId) {
    document.getElementById(elementId).click()
}




