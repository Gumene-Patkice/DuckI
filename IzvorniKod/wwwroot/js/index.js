document.addEventListener("DOMContentLoaded", async function () {
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
  renderNextEvents(events); // Pass all events to renderNextEvents
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
    dayEventDiv.classList.add("col-5");
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

function renderNextEvents(events) {
  const eventContainer = document.getElementById("nextEvents");
  eventContainer.innerHTML = "";

  // Sort events by date in ascending order
  const sortedEvents = events
    .filter((event) => event.date >= new Date()) // Only future events
    .sort((a, b) => a.date - b.date);

  // Get the next 5 events
  const nextEvents = sortedEvents.slice(0, 5);

  nextEvents.forEach((event) => {
    const eventRow = document.createElement("div");
    eventRow.classList.add("row", "mb-2", "d-flex", "justify-content-between");
    eventRow.id = "event-row";

    const svgCol = document.createElement("div");
    svgCol.classList.add("col-1");

    const svgImg = document.createElement("img");
    svgImg.src = "../images/play-solid.svg";
    svgImg.alt = "Event Icon";
    svgImg.classList.add("icon-svg");
    svgCol.appendChild(svgImg);

    const eventCol = document.createElement("div");
    eventCol.classList.add("col-6");
    eventCol.innerText = event.event; // Event description
    eventCol.id = "event-col";

    const dateCol = document.createElement("div");
    dateCol.classList.add("col-2", "fw-bold");
    dateCol.innerText = formatDate(event.date); // Format the event's date
    dateCol.id = "date-col";

    eventRow.appendChild(svgCol);
    eventRow.appendChild(eventCol);
    eventRow.appendChild(dateCol);
    eventContainer.appendChild(eventRow);
  });

  // If no upcoming events, show a placeholder message
  if (nextEvents.length === 0) {
    const noEventsMessage = document.createElement("div");
    noEventsMessage.classList.add("text-center", "text-muted");
    noEventsMessage.innerText = "No upcoming events.";
    eventContainer.appendChild(noEventsMessage);
  }
}

function formatDate(date) {
  const day = date.getDate();
  const month = date.getMonth() + 1; // Months are zero-based
  const year = date.getFullYear();
  return `${day}.${month}.${year}`;
}
