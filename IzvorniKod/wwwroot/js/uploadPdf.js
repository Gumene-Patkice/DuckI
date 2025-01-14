document.getElementById("file").addEventListener("change", function () {
    var selectedFile = document.getElementById("file").files[0].name;
    document.getElementById("browseFileShown").setAttribute("value", selectedFile.toString());
});

// notification update
async function checkAndSubmitForm(form, isPublic) {
    const formData = new FormData(form);
    const pdfName = formData.get('file').name;
    const response = await fetch(`@Url.Action("FetchPdfByName", "Pdf")?pdfName=${encodeURIComponent(pdfName)}&isPublic=${isPublic}`, {
        method: 'GET'
    });

    const exists = await response.json();

    if (exists) {
        const confirmReplace = confirm("The PDF already exists. Do you want to replace it?");
        if (!confirmReplace) {
            return;
        }
    }

    const uploadResponse = await fetch(form.action, {
        method: 'POST',
        body: formData
    });

    if (uploadResponse.ok) {
        document.getElementById('notification').style.display = 'block';
        document.getElementById('error').style.display = 'none';
    } else {
        const errorText = await uploadResponse.text();
        document.getElementById('error').innerText = `Error: ${errorText}`;
        document.getElementById('error').style.display = 'block';
        document.getElementById('notification').style.display = 'none';
    }
}

document.getElementById('uploadPrivateForm')?.addEventListener('submit', function (event) {
    event.preventDefault();
    checkAndSubmitForm(this, false);
});

document.getElementById('uploadPublicForm')?.addEventListener('submit', function (event) {
    event.preventDefault();
    checkAndSubmitForm(this, true);
});

// hiding the success message (so that it is not constantly visible after one successful upload)
// same for the error message
document.getElementById('file').addEventListener('click', function () {
    document.getElementById('notification').style.display = 'none';
    document.getElementById('error').style.display = 'none';
});

document.getElementById('tagName').addEventListener('click', function () {
    document.getElementById('notification').style.display = 'none';
    document.getElementById('error').style.display = 'none';
});