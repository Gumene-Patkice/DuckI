if (!(document.getElementById("tableId").children.length > 0)) {
    const msg = document.createElement("h3");
    document.getElementById("tableId").appendChild(msg)
    msg.setAttribute("class", "text-primary")
    msg.innerHTML = "No users available"
}