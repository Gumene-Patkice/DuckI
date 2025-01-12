document.addEventListener('DOMContentLoaded', function () {
    var response = typeof viewBagResponse !== 'undefined' ? viewBagResponse : null;


    if (response && response.Flashcards && response.Flashcards.length > 0) {
            var currentTasks = document.getElementById('currenttasks');
            for (var i = 0; i < response.Flashcards.length; i++) {
                var flashcard = response.Flashcards[i];
                var flashcardDiv = document.createElement('div');
                flashcardDiv.className = 'task';
                flashcardDiv.innerHTML = flashcard.question + ' - ' + flashcard.answer;
                currentTasks.appendChild(flashcardDiv);
                var saveButton = document.createElement('button');
                saveButton.innerHTML = 'Save';
                (function(flashcard) {
                    saveButton.addEventListener("click", function() {
                        SaveFlashcard(flashcard.title, flashcard.question, flashcard.answer);
                    });
                })(flashcard);
                currentTasks.appendChild(saveButton);
            }
        } else {
            document.getElementById('currenttasks').innerHTML = 'No flashcards available';
        }

        var allTasks = document.getElementById('alltasks');
        getAllFlashcards().then(flashcards => {
            if (flashcards && flashcards.length > 0) {
                for (var i = 0; i < flashcards.length; i++) {
                    var flashcard = flashcards[i];
                    var flashcardDiv = document.createElement('div');
                    flashcardDiv.className = 'task';
                    flashcardDiv.innerHTML = flashcard.question + ' - ' + flashcard.answer;
                    allTasks.appendChild(flashcardDiv);

                    var deleteButton = document.createElement('button');
                    deleteButton.innerHTML = 'Delete';
                    (function(flashcard) {
                        deleteButton.addEventListener("click", function() {
                            DeleteFlashcard(flashcard.title, flashcard.question, flashcard.answer);
                        });
                    })(flashcard);
                    allTasks.appendChild(deleteButton);
                }
            } else {
                allTasks.innerHTML = 'No flashcards available';
            }
        }).catch(error => {
            console.error('Error fetching flashcards:', error);
            allTasks.innerHTML = 'Failed to load flashcards';
        });
});


async function SaveFlashcard(title, question, answer) {
    var xhr = new XMLHttpRequest();
    var flashcard = {
        title: title,
        question: question,
        answer: answer
    };
    xhr.open('POST', '/api/flashcards', true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var response = JSON.parse(xhr.responseText);
            if (response && response.success) {
                alert(response.message);
            } else {
                alert('Failed to save flashcard - ' + response.message);
            }
        }
    };
    xhr.send(JSON.stringify(flashcard));
}

async function DeleteFlashcard(title, question, answer){
    var xhr = new XMLHttpRequest();
    var flashcard = {
        title: title,
        question: question,
        answer: answer
    };
    xhr.open('DELETE', '/api/flashcards', true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var response = JSON.parse(xhr.responseText);
            if (response && response.success) {
                alert(response.message);
            } else {
                alert('Failed to delete flashcard - ' + response.message);
            }
        }
    };
    xhr.send(JSON.stringify(flashcard));
}

async function getAllFlashcards() {
    return new Promise((resolve, reject) => {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/api/flashcards', true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                if (response && response.flashcards) {
                    resolve(response.flashcards);
                } else {
                    resolve([]);
                }
            } else if (xhr.readyState === 4) {
                reject('Failed to fetch flashcards');
            }
        };
        xhr.send();
    });
}