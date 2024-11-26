document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('sendButton').addEventListener('click', (event) => SubmitPrompt(event));
});

function SubmitPrompt(event) {
    event.preventDefault();
    var prompt = document.getElementById('prompt').value;
    document.getElementById('prompt').value = '';
    if (prompt !== '') {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/api/chat/prompt?p=' + encodeURIComponent(prompt), true);
        xhr.send();
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                document.getElementById('response').innerText = response.Text;
            }
        }
        xhr.onerror = function () {
            console.error('Request failed');
        };
    }
}