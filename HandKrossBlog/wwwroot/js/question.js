function closeModalMakeQuestion() {
    document.getElementById('btnCloseModalMakeQuestion').click()
}

function sendQuestion() {

    let question = document.getElementById('inputWriteQuestion')

    // Valido que el campo contenga texto
    if (question.value.trim() === '') {
        showAlert('#alert-warning-question', 3000)
        return
    }

    let formData = new FormData()
    formData.append("Question", question.value)

    // Envio la pregunta
    getApiPost('Questions/SaveQuestion', formData)
        .then(function (result) {
            // Limpio el campo
            question.value = ''
            closeModalMakeQuestion()
        })
        .catch(function (err) { alert(err) })
}
















