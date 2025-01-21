let savedQuestions = []; // Praćenje spremljenih pitanja

function groupFlashcardsByTitle(flashcards) {
  return flashcards.reduce((groups, card) => {
    if (!groups[card.title]) {
      groups[card.title] = [];
    }
    groups[card.title].push(card);
    return groups;
  }, {});
}

function createFlashcard(card) {
  const flashcard = document.createElement("div");
  flashcard.className = "flashcard";

  const front = document.createElement("div");
  front.className = "front";
  front.innerHTML = `
    <p>${card.question}</p>
  `;

  const back = document.createElement("div");
  back.className = "back";
  back.innerHTML = `
    <p>${card.answer}</p>
  `;

  const starIcon = document.createElement("div");
  starIcon.className = "star-icon";
  const isSaved = savedQuestions.some(
    (savedCard) =>
      savedCard.title === card.title &&
      savedCard.question === card.question &&
      savedCard.answer === card.answer,
  );

  const regularStarSVG = `
    <svg fill="#FFE052" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" width="42" height="42">
      <path d="M287.9 0c9.2 0 17.6 5.2 21.6 13.5l68.6 141.3 153.2 22.6c9 1.3 16.5 7.6 19.3 16.3s.5 18.1-5.9 24.5L433.6 328.4l26.2 155.6c1.5 9-2.2 18.1-9.7 23.5s-17.3 6-25.3 1.7l-137-73.2L151 509.1c-8.1 4.3-17.9 3.7-25.3-1.7s-11.2-14.5-9.7-23.5l26.2-155.6L31.1 218.2c-6.5-6.4-8.7-15.9-5.9-24.5s10.3-14.9 19.3-16.3l153.2-22.6L266.3 13.5C270.4 5.2 278.7 0 287.9 0zm0 79L235.4 187.2c-3.5 7.1-10.2 12.1-18.1 13.3L99 217.9 184.9 303c5.5 5.5 8.1 13.3 6.8 21L171.4 443.7l105.2-56.2c7.1-3.8 15.6-3.8 22.6 0l105.2 56.2L384.2 324.1c-1.3-7.7 1.2-15.5 6.8-21l85.9-85.1L358.6 200.5c-7.8-1.2-14.6-6.1-18.1-13.3L287.9 79z"/>
    </svg>
  `;

  const solidStarSVG = `
    <svg fill="#FFE052" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" width="42" height="42">
      <path d="M316.9 18C311.6 7 300.4 0 288.1 0s-23.4 7-28.8 18L195 150.3 51.4 171.5c-12 1.8-22 10.2-25.7 21.7s-.7 24.2 7.9 32.7L137.8 329 113.2 474.7c-2 12 3 24.2 12.9 31.3s23 8 33.8 2.3l128.3-68.5 128.3 68.5c10.8 5.7 23.9 4.9 33.8-2.3s14.9-19.3 12.9-31.3L438.5 329 542.7 225.9c8.6-8.5 11.7-21.2 7.9-32.7s-13.7-19.9-25.7-21.7L381.2 150.3 316.9 18z"/>
    </svg>
  `;

  starIcon.innerHTML = isSaved ? solidStarSVG : regularStarSVG;

  starIcon.addEventListener("click", async function (event) {
    event.stopPropagation();

    const isSaved = savedQuestions.some(
      (savedCard) =>
        savedCard.title === card.title &&
        savedCard.question === card.question &&
        savedCard.answer === card.answer,
    );

    if (!isSaved) {
      try {
        await SaveFlashcard(card.title, card.question, card.answer);
        savedQuestions.push(card);
        starIcon.innerHTML = solidStarSVG;

        const flashcards = await getAllFlashcards();
        const groupedFlashcards = groupFlashcardsByTitle(flashcards);
        const allTasks = document.getElementById("alltasks");
        renderGroupedFlashcards(groupedFlashcards, allTasks);
      } catch (error) {
        console.error("Failed to save flashcard:", error);
      }
    } else {
      try {
        await DeleteFlashcard(card.title, card.question, card.answer);
        savedQuestions = savedQuestions.filter(
          (savedCard) =>
            savedCard.title !== card.title ||
            savedCard.question !== card.question ||
            savedCard.answer !== card.answer,
        );
        starIcon.innerHTML = regularStarSVG;

        const flashcards = await getAllFlashcards();
        const groupedFlashcards = groupFlashcardsByTitle(flashcards);
        const allTasks = document.getElementById("alltasks");
        renderGroupedFlashcards(groupedFlashcards, allTasks);
      } catch (error) {
        console.error("Failed to delete flashcard:", error);
      }
    }
  });

  front.appendChild(starIcon);
  flashcard.appendChild(front);
  flashcard.appendChild(back);

  let isFlipping = false;

  flashcard.addEventListener("click", (event) => {
    if (
      event.target.classList.contains("star-icon") ||
      event.target.closest(".star-icon")
    ) {
      return;
    }

    if (isFlipping) return;

    isFlipping = true;

    flashcard.classList.toggle("flipped");

    flashcard.addEventListener(
      "transitionend",
      () => {
        isFlipping = false;
      },
      { once: true },
    );
  });

  return flashcard;
}

function renderFlashcards(flashcards, container) {
  container.innerHTML = "";
  flashcards.forEach((card) => {
    const flashcard = createFlashcard(card);
    container.appendChild(flashcard);
  });
}

function renderGroupedFlashcards(groups, container) {
  const currentTasks = document.getElementById("currenttasks");

  container.innerHTML = "";
  Object.keys(groups).forEach((title) => {
    const groupDiv = document.createElement("div");
    groupDiv.className = "tasks-group";
    groupDiv.textContent = title;
    groupDiv.addEventListener("click", () => {
      renderFlashcards(groups[title], currentTasks);
    });
    container.appendChild(groupDiv);
  });
}

document.addEventListener("DOMContentLoaded", function () {
  const currentTasks = document.getElementById("currenttasks");
  const allTasks = document.getElementById("alltasks");

  getAllFlashcards()
    .then((flashcards) => {
      if (flashcards && flashcards.length > 0) {
        const groupedFlashcards = groupFlashcardsByTitle(flashcards);
        renderGroupedFlashcards(groupedFlashcards, allTasks);
      }

      var response =
        typeof viewBagResponse !== "undefined" ? viewBagResponse : null;

      if (response && response.Flashcards.flashcards) {
        if (
          response &&
          response.Flashcards &&
          response.Flashcards.flashcards &&
          response.Flashcards.flashcards.length > 0
        ) {
          renderFlashcards(response.Flashcards.flashcards, currentTasks);
        } else {
          currentTasks.innerHTML = "No flashcards selected or generated.";
        }
      } else {
        if (response && response.Flashcards && response.Flashcards.length > 0) {
          renderFlashcards(response.Flashcards, currentTasks);
        } else {
          currentTasks.innerHTML = "No flashcards selected or generated.";
        }
      }
    })
    .catch((error) => {
      console.error("Error fetching flashcards:", error);
    });
});

async function SaveFlashcard(title, question, answer) {
  return new Promise((resolve, reject) => {
    var xhr = new XMLHttpRequest();
    var flashcard = {
      title: title,
      question: question,
      answer: answer,
    };
    xhr.open("POST", "/api/flashcards", true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
      if (xhr.readyState === 4 && xhr.status === 200) {
        var response = JSON.parse(xhr.responseText);
        if (response && response.success) {
          resolve(response.message);
        } else {
          reject(response.message || "Failed to save flashcard");
        }
      } else if (xhr.readyState === 4) {
        reject("Failed to save flashcard");
      }
    };
    xhr.send(JSON.stringify(flashcard));
  });
}

async function DeleteFlashcard(title, question, answer) {
  return new Promise((resolve, reject) => {
    var xhr = new XMLHttpRequest();
    var flashcard = {
      title: title,
      question: question,
      answer: answer,
    };
    xhr.open("DELETE", "/api/flashcards", true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
      if (xhr.readyState === 4 && xhr.status === 200) {
        var response = JSON.parse(xhr.responseText);
        if (response && response.success) {
          resolve(response.message);
        } else {
          reject(response.message || "Failed to delete flashcard");
        }
      } else if (xhr.readyState === 4) {
        reject("Failed to delete flashcard");
      }
    };
    xhr.send(JSON.stringify(flashcard));
  });
}

async function getAllFlashcards() {
  return new Promise((resolve, reject) => {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "/api/flashcards", true);
    xhr.onreadystatechange = function () {
      if (xhr.readyState === 4 && xhr.status === 200) {
        var response = JSON.parse(xhr.responseText);
        if (response && response.flashcards) {
          savedQuestions = response.flashcards.map((card) => ({
            title: card.title,
            question: card.question,
            answer: card.answer,
          }));
          resolve(response.flashcards);
        } else {
          resolve([]);
        }
      } else if (xhr.readyState === 4) {
        reject("Failed to fetch flashcards");
      }
    };
    xhr.send();
  });
}
