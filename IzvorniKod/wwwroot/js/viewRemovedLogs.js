if (!(document.getElementById("pdfTableBody").children.length > 1)) {
    const msg = document.createElement("p");
    document.getElementById("tableId").appendChild(msg)
    document.getElementById("tableHead").setAttribute("class", "d-none")
    msg.setAttribute("class", "text-primary mx-3")
    msg.innerHTML = "No PDFs available"
}