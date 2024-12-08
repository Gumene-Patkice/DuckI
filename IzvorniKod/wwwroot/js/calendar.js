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

  // Update the displayed month name
  updateMonthLabel();

  const response = await fetch(`/api/calendars/getcalendar`);

  if (response.ok) {
    // Show the calendar controls (buttons, headers, etc.)
    document.getElementById("calendarContainer").style.display = "block";

    // Clear the calendar grid
    document.getElementById("calendarGrid").innerHTML = "";

    // Get the days in the month and the starting day
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const firstDayOfMonth = new Date(year, month, 1).getDay();
    const offset = firstDayOfMonth === 0 ? 6 : firstDayOfMonth - 1;

    let weekRow = document.createElement("div");
    weekRow.classList.add("row", "g-0"); // Create the first row for the initial offset cells
    document.getElementById("calendarGrid").appendChild(weekRow);

    // Fill in empty cells for days before the start of the month
    for (let i = 0; i < offset; i++) {
      weekRow.appendChild(createEmptyDayCell());
    }

    // Fill in the actual days of the month, creating new rows for each week
    for (let day = 1; day <= daysInMonth; day++) {
      if ((offset + day - 1) % 7 === 0 && day !== 1) {
        // Start a new row every 7 cells
        weekRow = document.createElement("div");
        weekRow.classList.add("row", "g-0");
        document.getElementById("calendarGrid").appendChild(weekRow);
      }
      weekRow.appendChild(createDayCell(day));
    }

    // Add empty cells to complete the last row, if needed
    const remainingCells = 7 - weekRow.children.length;
    for (let i = 0; i < remainingCells; i++) {
      weekRow.appendChild(createEmptyDayCell());
    }

    // Parse CSV and populate events
    const csvContent = await response.text();
    const eventData = parseCSV(csvContent);
    eventData.forEach((event) => {
      const eventDate = new Date(event.date);
      if (eventDate.getFullYear() === year && eventDate.getMonth() === month) {
        const eventDay = eventDate.getDate();
        const dayCell = document.querySelector(
          `#calendarGrid .day-cell[data-day="${eventDay}"]`,
        );
        if (dayCell) {
          const eventDiv = document.createElement("div");
          eventDiv.classList.add("event-text");
          eventDiv.innerText = event.event;
          dayCell.querySelector(".event-container").appendChild(eventDiv);
        }
      }
    });
  } else {
    alert("Failed to fetch calendar.");
  }
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

  // Create the day number element and append it
  const dayNumber = document.createElement("strong");
  dayNumber.innerText = day;
  dayCell.appendChild(dayNumber);

  // Create the event container (initially empty)
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
});
