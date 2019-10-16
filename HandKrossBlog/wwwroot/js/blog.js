// Inicializo estos valores
localStorage.setItem('DeleteComment_PostId', '')
localStorage.setItem('DeleteComment_CommentId', '')
localStorage.setItem('EditComment_PostId', '')
localStorage.setItem('EditComment_CommentId', '')

document.getElementById('btnSaveComment').addEventListener('click', function () {
    saveComment()
})

function formatDate(date) {
    var dateJs = new Date(Date.parse(date))
    var responseDate = [("0" + dateJs.getDate()).slice(-2), ("0" + (dateJs.getMonth() + 1)).slice(-2), dateJs.getFullYear()].join('/')
    responseDate += ", " + [("0" + dateJs.getHours()).slice(-2), ("0" + dateJs.getMinutes()).slice(-2)].join(':')
    return responseDate
}

function prepareCommentToDelete(post_id, comment_id) {
    localStorage.setItem('DeleteComment_PostId', post_id)
    localStorage.setItem('DeleteComment_CommentId', comment_id)
}

function prepareCommentToEdit(post_id, comment_id, comment) {
    localStorage.setItem('EditComment_PostId', post_id)
    localStorage.setItem('EditComment_CommentId', comment_id)
    setValueElementById('inputEditComment', comment)
}

function saveComment() {
    var comment = getValueElementById('inputComment')
    var user = getValueElementById('inputUser')
    var post_id = getValueElementById('postId')

    if (comment.trim() !== "") {

        var formData = new FormData();
        formData.append("Comment", comment)
        formData.append("User", user)
        formData.append("PostId", post_id)

        ajaxPost("/BlogPosts/SaveComment", formData)
        .then(function (result) {
            if (result.response) {
                // Limpio los valores que fueron introducidos
                document.querySelector('#inputComment').value = ""
                document.querySelector('#inputUser').value = ""
                // Muestro el mensaje de exito
                showAlert("#alert-success", 3000)
                // Cargo los comentarios
                loadComments(result.viewComments)
                // Aca cambio el numero total de comentarios en el sidebar menu
                changeNumberOfCommentsSidebarMenu(post_id, result.numberOfComments)
            }
        })

    } else {
        showAlert("#alert-warning", 3000)
    }
}

function deleteComment() {
    var comment_id = localStorage.getItem('DeleteComment_CommentId') || ''
    var post_id = localStorage.getItem('DeleteComment_PostId') || ''

    if (comment_id === '' || post_id === '') {
        alert('No se puede eliminar el comentario')
        return
    }

    var formData = new FormData()
    formData.append("CommentId", comment_id)
    formData.append("PostId", post_id)

    ajaxPost("/BlogPosts/DeleteComment", formData)
    .then(function (result) {
        if (result.response) {
            // Cargo los comentarios
            loadComments(result.viewComments)
            // Aca cambio el numero total de comentarios en el sidebar menu
            changeNumberOfCommentsSidebarMenu(post_id, result.numberOfComments)
        }
    })

    // Para que se cierre el modal
    clickElementById('btn-close-modal-delete-comment')

    // Limpio las variables
    localStorage.setItem('DeleteComment_PostId', '')
    localStorage.setItem('DeleteComment_CommentId', '')
}

function editComment() {
    var comment_id = localStorage.getItem('EditComment_CommentId') || ''
    var post_id = localStorage.getItem('EditComment_PostId') || ''

    if (comment_id === '' || post_id === '') {
        alert('No se puede editar el comentario')
        return
    }

    var comment = document.getElementById('inputEditComment').value;
    if (comment === '') {
        showAlert("#alert-warning-edit-comment", 2000)
        return
    }

    var formData = new FormData()
    formData.append("CommentId", comment_id)
    formData.append("PostId", post_id)
    formData.append("Comment", comment)

    ajaxPost("/BlogPosts/EditComment", formData)
    .then(function (result) {
        if (result.response) {
            // Cargo los comentarios
            loadComments(result.viewComments)
            // Aca cambio el numero total de comentarios en el sidebar menu
            changeNumberOfCommentsSidebarMenu(post_id, result.numberOfComments)
        }
    })

    // Para que se cierre el modal
    clickElementById('btn-close-modal-edit-comment')
    // Limpio las variables
    localStorage.setItem('EditComment_PostId', '')
    localStorage.setItem('EditComment_CommentId', '')
    // Limpio el campo del comentario
    clearInputById('inputEditComment')
}

function loadComments(viewComments) {
    clearChildsElement("#section-comments")
    document.getElementById('section-comments').innerHTML = viewComments
}

function clearChildsElement(element_id) {
    var section_comments = document.querySelector(element_id)
    while (section_comments.firstChild) section_comments.removeChild(section_comments.firstChild)
    section_comments = null
}

function changeNumberOfCommentsSidebarMenu(post_id, numberOfComments) {
    document.getElementById('span_comment_number_post_id_' + post_id).innerHTML = numberOfComments
}









