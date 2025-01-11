﻿document.addEventListener("DOMContentLoaded", async function () {
  await loadCurrentWeekAndNextEvents();
});

async function loadCurrentWeekAndNextEvents() {
  const response = await fetch(`/api/calendars/getcalendar`);
  if (!response.ok) {
    alert("Failed to fetch calendar.");
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
      return { date: new Date(date), event };
    });
}

function getCurrentWeekEvents(events, now) {
  const startOfWeek = new Date(now);
  startOfWeek.setDate(now.getDate() - now.getDay() + 1); // Monday
  const endOfWeek = new Date(startOfWeek);
  endOfWeek.setDate(startOfWeek.getDate() + 6); // Sunday

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

function formatDate(date) {
  const day = date.getDate();
  const month = date.getMonth() + 1; // Months are zero-based
  const year = date.getFullYear();
  return `${day}.${month}.${year}`;
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
