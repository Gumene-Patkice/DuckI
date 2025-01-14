document.addEventListener("DOMContentLoaded", async function () {
  await loadCurrentWeekAndNextEvents();
});

async function loadCurrentWeekAndNextEvents() {
  const response = await fetch(`/api/calendars/getcalendar`);
  if (!response.ok) {

    const element = document.getElementById("flag-message");
    if (typeof(element) != 'undefined' && element != null)
    {
      element.remove()
    }

    const msg = document.createElement("h5")
    msg.setAttribute("id", "flag-message")
    msg.setAttribute("class", "text-center flag-message fw-bold msg-animation")
    msg.style.color = "orangered";
    msg.style.display = "block";
    msg.innerHTML = "Failed to load calendar."
    document.getElementById("weekEvents").appendChild(msg)
    
    return;
  }

  const csvContent = await response.text();
  const events = parseCSV(csvContent);

  const now = new Date();
  const currentWeekEvents = getCurrentWeekEvents(events, now);

  renderCurrentWeek(currentWeekEvents);
}

function parseCSV(data) {
  return data
    .split("\n")
    .filter((line) => line.trim() !== "")
    .map((line) => {
      const [date, event] = line.split(",");
      const parsedDate = new Date(date.trim());
      if (isNaN(parsedDate)) {
        console.error(`Invalid date format: ${date}`);
        return null;
      }
      return { date: normalizeDate(parsedDate), event: event.trim() };
    })
    .filter((event) => event !== null);
}

function normalizeDate(date) {
  const normalized = new Date(date);
  normalized.setHours(0, 0, 0, 0);
  return normalized;
}

function getCurrentWeekEvents(events, now) {
  const startOfWeek = normalizeDate(new Date(now));

  if (startOfWeek.getDay() === 0) {
    startOfWeek.setDate(startOfWeek.getDate() - 6);
  } else {
    startOfWeek.setDate(startOfWeek.getDate() - startOfWeek.getDay() + 1);
  }

  const endOfWeek = new Date(startOfWeek);
  endOfWeek.setDate(startOfWeek.getDate() + 6);
  endOfWeek.setHours(23, 59, 59, 999);

  return events.filter(
    (event) => event.date >= startOfWeek && event.date <= endOfWeek,
  );
}

function renderCurrentWeek(events) {
  const weekContainer = document.getElementById("weekEvents");
  weekContainer.innerHTML = "";

  const daysOfWeek = ["MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"];
  const startOfWeek = new Date();
  startOfWeek.setDate(new Date().getDate() - new Date().getDay() + 1); // Monday

  daysOfWeek.forEach((day, index) => {
    const dayDate = new Date(startOfWeek);
    dayDate.setDate(startOfWeek.getDate() + index);

    const dayEvent = events.find(
      (event) => event.date.toDateString() === dayDate.toDateString(),
    );

    const dayRow = document.createElement("div");
    dayRow.classList.add(
      "row",
      "mb-2",
      "d-flex",
      "justify-content-start",
      "gap-1",
    );
    dayRow.id = "day-row";

    const dayHeader = document.createElement("div");
    dayHeader.classList.add("col-1", "fw-bold", "text-center", "text-light");
    dayHeader.id = "day-header";
    dayHeader.innerText = `${day}`;

    const dayEventDiv = document.createElement("div");
    dayEventDiv.classList.add("col-10");
    if (dayEvent) {
      dayEventDiv.innerText = dayEvent.event;
    } else {
      dayEventDiv.innerText = "";
    }
    dayEventDiv.id = "day-event";

    dayRow.appendChild(dayHeader);
    dayRow.appendChild(dayEventDiv);
    weekContainer.appendChild(dayRow);
  });
}

document.addEventListener("DOMContentLoaded", function () {
  let currentlyOpenRow = null; // Track the currently open row

  const rows = document.querySelectorAll("#pdfTableBody tr");

  rows.forEach((row) => {
    row.addEventListener("click", function (event) {
      // Close the previously opened row
      if (currentlyOpenRow && currentlyOpenRow !== this) {
        const previousDivs = currentlyOpenRow.querySelectorAll(
          "td > div:not(:first-of-type)",
        );
        previousDivs.forEach((div) => {
          div.style.display = "none";
        });
      }

      // Toggle the visibility of the current row
      const childDivs = this.querySelectorAll("td > div:not(:first-of-type)");
      const isAlreadyOpen = Array.from(childDivs).some(
        (div) => div.style.display === "flex",
      );

      if (isAlreadyOpen) {
        childDivs.forEach((div) => {
          div.style.display = "none";
        });
        currentlyOpenRow = null; // Reset currently open row
      } else {
        childDivs.forEach((div) => {
          div.style.display = "flex";
        });
        currentlyOpenRow = this; // Update currently open row
      }
    });
  });
});
