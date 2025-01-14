document.getElementById("sortButton").addEventListener("click", function () {
    var table, rows, switching, i, x, y, shouldSwitch;
    table = document.getElementById("pdfTableBody");
    switching = true;
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 0; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName("TD")[0].querySelector('form:nth-child(1)').querySelector("button").innerText.trim().toLowerCase();
            y = rows[i + 1].getElementsByTagName("TD")[0].querySelector('form:nth-child(1)').querySelector("button").innerText.trim().toLowerCase();
            if (x > y) {
                shouldSwitch = true;
                break;
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
        }
    }
})

document.addEventListener('DOMContentLoaded', async function() {
    const response = await fetch('/Pdf/GetAllTags', {
        method: 'GET'
    });
    const tags = await response.json();
    const tagFilter = document.getElementById('tagFilter');
    tags.forEach(tag => {
        const option = document.createElement('option');
        option.value = tag.tagName;
        option.textContent = tag.tagName;
        tagFilter.appendChild(option);
    });
});

document.getElementById('filterButton').addEventListener('click', function() {
    if (document.getElementById("no-pdf")) {
        document.getElementById("no-pdf").remove();
    }

    const selectedTag = document.getElementById('tagFilter').value.toLowerCase().trim();
    const rows = document.querySelectorAll('#pdfTableBody tr');

    rows.forEach(row => {
        const tagName = row.querySelector('td').querySelector('div:nth-child(4)').textContent.toLowerCase().trim();
        if (selectedTag === "" || tagName === selectedTag) {
            row.style.display = "";
        } else {
            row.style.display = "none";
        }
    });
});

document.querySelectorAll('.flag-form').forEach(form => {
    form.addEventListener('submit', async function(event) {
        event.preventDefault();
        const formData = new FormData(this);
        const response = await fetch('/Pdf/FlagPdf', {
            method: 'POST',
            body: formData
        });

        const element =  document.getElementById('flag-message');
        if (response.ok) {
            if (typeof(element) != 'undefined' && element != null)
            {
                element.remove()
            }
            const msg = document.createElement("span")
            msg.setAttribute("id", "flag-message")
            msg.setAttribute("class", "text-center flag-message fw-bold msg-animation")
            msg.style.color = "green";
            msg.style.display = "block";
            msg.innerHTML = "Successfully added!"
            document.getElementById("nextToMsg").appendChild(msg)

        } else if (response.status >= 400 && response.status < 500) {
            if (typeof(element) != 'undefined' && element != null)
            {
                element.remove()
            }
            const msg = document.createElement("span")
            msg.setAttribute("id", "flag-message")
            msg.setAttribute("class", "text-center flag-message fw-bold msg-animation")
            msg.style.color = "orangered";
            msg.style.display = "block";
            msg.innerHTML = "The selected PDF has already been flagged!"
            document.getElementById("nextToMsg").appendChild(msg)

        } else {
            if (typeof(element) != 'undefined' && element != null)
            {
                element.remove()
            }
            const msg = document.createElement("span")
            msg.setAttribute("id", "flag-message")
            msg.setAttribute("class", "text-center flag-message fw-bold msg-animation")
            msg.style.color = "orangered";
            msg.style.display = "block";
            msg.innerHTML = "Failed to flag the PDF."
            document.getElementById("nextToMsg").appendChild(msg)
        }
    });
});

document.getElementById('filterButton').addEventListener('click', function() {
    var a = document.querySelectorAll("#pdfTableBody tr");
    let count = 0;
    for (let b of a) {
        if (b.style.display !== "none")
            count++;
    }
    if (count === 0) {
        const msg = document.createElement("h2");
        msg.setAttribute("class", "text-primary");
        msg.setAttribute("id", "no-pdf");
        msg.innerHTML = "No PDFs available";
        document.getElementById("pdfTableBody").appendChild(msg);
    }
});

if(document.getElementById("pdfTableBody").childElementCount === 0) {
    const msg = document.createElement("h2");
    msg.setAttribute("class", "text-primary");
    msg.setAttribute("id", "no-pdf");
    msg.innerHTML = "No PDFs available";
    document.getElementById("pdfTableBody").appendChild(msg);
}