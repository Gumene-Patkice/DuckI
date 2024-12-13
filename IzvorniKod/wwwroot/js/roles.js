document.getElementById("EducatorReset").addEventListener("click", function(){
    document.getElementById("textarea1").value = "";
    document.getElementById("EducatorReset").disabled = true;
});

document.getElementById("ReviewerReset").addEventListener("click", function(){
    document.getElementById("textarea2").value = "";
    document.getElementById("ReviewerReset").disabled = true;
});

function enableEducatorReset() {
    if (document.getElementById("textarea1").value !== "") {
        document.getElementById("EducatorReset").disabled = false;
    } else {
        document.getElementById("EducatorReset").disabled = true;
    }
}

function enableReviewerReset() {
    if (document.getElementById("textarea2").value !== "") {
        document.getElementById("ReviewerReset").disabled = false;
    } else {
        document.getElementById("ReviewerReset").disabled = true;
    }
}

document.getElementById("textarea1").addEventListener("input", enableEducatorReset);
document.getElementById("textarea2").addEventListener("input", enableReviewerReset);