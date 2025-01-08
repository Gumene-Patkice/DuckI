let currentDate = new Date(); // Initialize with the current date

document
  .getElementById("prevMonthButton")
  .addEventListener("click", () => changeMonth(-1));
document
  .getElementById("nextMonthButton")
  .addEventListener("click", () => changeMonth(1));

async function renderCalendar() {
  const year = currentDate.getFullYear();
  const month = currentDate.getMonth();

  updateMonthLabel();

  const response = await fetch(`/api/calendars/getcalendar`);

  if (response.ok) {
    document.getElementById("calendarContainer").style.display = "block";
    document.getElementById("calendarGrid").innerHTML = "";

    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const firstDayOfMonth = new Date(year, month, 1).getDay();
    const offset = firstDayOfMonth === 0 ? 6 : firstDayOfMonth - 1;

    let weekRow = document.createElement("div");
    weekRow.classList.add("row", "g-0");
    document.getElementById("calendarGrid").appendChild(weekRow);

    for (let i = 0; i < offset; i++) {
      weekRow.appendChild(createEmptyDayCell());
    }

    for (let day = 1; day <= daysInMonth; day++) {
      if ((offset + day - 1) % 7 === 0 && day !== 1) {
        weekRow = document.createElement("div");
        weekRow.classList.add("row", "g-0");
        document.getElementById("calendarGrid").appendChild(weekRow);
      }
      const dayCell = createDayCell(day);
      weekRow.appendChild(dayCell);
    }

    const remainingCells = 7 - weekRow.children.length;
    for (let i = 0; i < remainingCells; i++) {
      weekRow.appendChild(createEmptyDayCell());
    }

    const csvContent = await response.text();
    const eventData = parseCSV(csvContent);

    // Petlja za dodavanje događaja i Delete dugmeta
    eventData.forEach((event) => {
      const eventDate = new Date(event.date);
      if (eventDate.getFullYear() === year && eventDate.getMonth() === month) {
        const eventDay = eventDate.getDate();
        const dayCell = document.querySelector(
          `#calendarGrid .day-cell[data-day="${eventDay}"]`,
        );
        if (dayCell) {
          dayCell.classList.add("has-event");
          const eventDiv = document.createElement("div");
          eventDiv.classList.add("event-text");
          eventDiv.innerText = event.event;

          dayCell.querySelector(".event-container").appendChild(eventDiv);
          addDeleteEventButton(dayCell, event.date, event.event); // Dodavanje Delete dugmeta
        }
      }
    });

    // Petlja za dodavanje Add Event dugmeta na ćelije bez događaja
    document.querySelectorAll("#calendarGrid .day-cell").forEach((dayCell) => {
      if (
        !dayCell.classList.contains("has-event") &&
        !dayCell.classList.contains("empty-day-cell")
      ) {
        const day = dayCell.getAttribute("data-day");
        addAddEventButton(dayCell, day);
      }
    });
  } else {
    alert("Failed to fetch calendar.");
  }
}

// Helper function to add "Add Event" button to a day cell
function addAddEventButton(dayCell, day) {
  const addButton = document.createElement("button");
  addButton.innerText = "Add Event";
  addButton.classList.add("btn", "btn-success", "btn-sm", "add-event-btn");
  addButton.style.position = "absolute";
  addButton.style.bottom = "5px";
  addButton.style.right = "5px";
  addButton.style.display = "none";

  addButton.addEventListener("click", () => {
    const month = currentDate.getMonth() + 1;
    const year = currentDate.getFullYear();

    document.getElementById("eventDate").value = `${year}-${String(
      month,
    ).padStart(2, "0")}-${String(day).padStart(2, "0")}`;

    document.getElementById("eventForm").style.display = "block";
  });

  dayCell.appendChild(addButton);

  dayCell.addEventListener("mouseenter", () => {
    addButton.style.display = "block";
  });
  dayCell.addEventListener("mouseleave", () => {
    addButton.style.display = "none";
  });
}

function addDeleteEventButton(dayCell, eventDate, eventDescription) {
  const deleteButton = document.createElement("button");
  deleteButton.classList.add("btn", "btn-danger", "btn-sm", "delete-btn");
  deleteButton.innerText = "Delete Event";
  deleteButton.style.position = "absolute";
  deleteButton.style.bottom = "5px";
  deleteButton.style.right = "5px";
  deleteButton.style.display = "none";

  deleteButton.addEventListener("click", async () => {
    const response = await fetch(
      `/api/calendars/deleteevent?eventDate=${encodeURIComponent(
        eventDate,
      )}&eventDescription=${encodeURIComponent(eventDescription.trim())}`,
      { method: "DELETE" },
    );
    if (response.ok) {
      alert("Event deleted successfully.");
      dayCell.classList.remove("has-event");
      renderCalendar();
    } else {
      alert("Failed to delete event.");
    }
  });

  dayCell.appendChild(deleteButton);

  dayCell.addEventListener("mouseenter", () => {
    deleteButton.style.display = "block";
  });
  dayCell.addEventListener("mouseleave", () => {
    deleteButton.style.display = "none";
  });
}

function changeMonth(delta) {
  currentDate.setMonth(currentDate.getMonth() + delta);
  renderCalendar();
}

function parseCSV(data) {
  return data
    .split("\n")
    .filter((line) => line.trim() !== "")
    .map((line) => {
      const [date, event] = line.split(",");
      return { date, event };
    });
}

function createEmptyDayCell() {
  const emptyCell = document.createElement("div");
  emptyCell.classList.add(
    "col",
    "d-flex",
    "flex-column",
    "align-items-center",
    "justify-content-start",
    "p-2",
    "day-cell",
    "empty-day-cell",
  );

  // Add a placeholder element to maintain consistent layout
  const placeholder = document.createElement("div");
  placeholder.classList.add("day-placeholder");
  emptyCell.appendChild(placeholder);

  return emptyCell;
}

function createDayCell(day) {
  const dayCell = document.createElement("div");
  dayCell.classList.add(
    "col",
    "d-flex",
    "flex-column",
    "align-items-center",
    "justify-content-start",
    "p-2",
    "day-cell",
  );
  dayCell.setAttribute("data-day", day);

  // Dodajte broj dana
  const dayNumber = document.createElement("strong");
  dayNumber.innerText = day;
  dayCell.appendChild(dayNumber);

  // Dodajte kontejner za događaje
  const eventContainer = document.createElement("div");
  eventContainer.classList.add(
    "event-container",
    "d-flex",
    "flex-column",
    "align-items-center",
    "w-100",
  );
  dayCell.appendChild(eventContainer);

  return dayCell;
}

function updateMonthLabel() {
  const monthNames = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
  ];
  const month = currentDate.getMonth();
  const year = currentDate.getFullYear();
  const monthLabel = document.getElementById("monthLabel");
  monthLabel.innerText = `${monthNames[month]} ${year}`;
}

// Render the calendar as soon as the DOM content is loaded
document.addEventListener("DOMContentLoaded", () => {
  renderCalendar();

  document.addEventListener("click", (e) => {
    const dayCell = e.target.closest(".day-cell:not(.has-event)");
    if (dayCell) {
      const day = dayCell.getAttribute("data-day");
      const month = currentDate.getMonth() + 1;
      const year = currentDate.getFullYear();
      document.getElementById("eventDate").value = `${year}-${String(
        month,
      ).padStart(2, "0")}-${String(day).padStart(2, "0")}`;
    }
  });
});

document.addEventListener("DOMContentLoaded", () => {
  const eventForm = document.getElementById("eventForm");
  const createEventForm = document.getElementById("createEventForm");

  createEventForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    const eventDate = document.getElementById("eventDate").value;
    const eventDescription = document.getElementById("eventDescription").value;

    if (eventDate && eventDescription) {
      const response = await fetch(
        `/api/calendars/addevent?eventDate=${encodeURIComponent(
          eventDate,
        )}&eventDescription=${encodeURIComponent(eventDescription)}`,
        {
          method: "POST",
        },
      );
      if (response.ok) {
        alert("Event created successfully!");
        renderCalendar();
        eventForm.style.display = "none";
      } else {
        alert("Failed to create event.");
      }
    } else {
      alert("Both date and description are required.");
    }
  });

  // Dodajte zatvaranje forme
  document.getElementById("cancelEventForm").addEventListener("click", () => {
    eventForm.style.display = "none";
  });
});
