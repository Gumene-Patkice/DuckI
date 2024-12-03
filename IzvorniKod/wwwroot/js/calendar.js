let currentDate = new Date(); // Initialize with the current date

document.getElementById('fetchCalendarButton').addEventListener('click', () => renderCalendar());
document.getElementById('prevMonthButton').addEventListener('click', () => changeMonth(-1));
document.getElementById('nextMonthButton').addEventListener('click', () => changeMonth(1));

async function renderCalendar() {
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();

    updateMonthLabel();

    const response = await fetch(`/api/calendars/getcalendar`);

    if (response.ok) {
        document.getElementById('calendarContainer').style.display = 'block';
        document.getElementById('calendarGrid').innerHTML = ''; // Clear calendar grid
        document.getElementById('eventList').innerHTML = ''; // Clear event list

        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const firstDayOfMonth = new Date(year, month, 1).getDay();
        const offset = (firstDayOfMonth === 0) ? 6 : firstDayOfMonth - 1;

        let weekRow = document.createElement('div');
        weekRow.classList.add('row', 'g-0');
        document.getElementById('calendarGrid').appendChild(weekRow);

        for (let i = 0; i < offset; i++) {
            weekRow.appendChild(createEmptyDayCell());
        }

        const csvContent = await response.text();
        const eventData = parseCSV(csvContent);

        for (let day = 1; day <= daysInMonth; day++) {
            if ((offset + day - 1) % 7 === 0 && day !== 1) {
                weekRow = document.createElement('div');
                weekRow.classList.add('row', 'g-0');
                document.getElementById('calendarGrid').appendChild(weekRow);
            }
            const dayCell = createDayCell(day);
            const eventsForDay = eventData.filter(event => {
                const eventDate = new Date(event.date);
                return (
                    eventDate.getFullYear() === year &&
                    eventDate.getMonth() === month &&
                    eventDate.getDate() === day
                );
            });

            if (eventsForDay.length > 0) {
                const dot = document.createElement('span');
                dot.classList.add('event-dot');
                dayCell.appendChild(dot); // Add the dot to the cell

                // Add events to the event list
                eventsForDay.forEach(event => {
                    const eventDiv = document.createElement('div');
                    eventDiv.classList.add('event-item');

                    // Format the date to dd.mm.yyyy
                    const eventDate = new Date(event.date);
                    const formattedDate = eventDate.toLocaleDateString('en-GB');

                    eventDiv.innerText = `${formattedDate}: ${event.event}`;
                    document.getElementById('eventList').appendChild(eventDiv);
                });
            }

            weekRow.appendChild(dayCell);
        }

        const remainingCells = 7 - weekRow.children.length;
        for (let i = 0; i < remainingCells; i++) {
            weekRow.appendChild(createEmptyDayCell());
        }
    } else {
        alert('Failed to fetch calendar.');
    }
}

function changeMonth(delta) {
    currentDate.setMonth(currentDate.getMonth() + delta);
    renderCalendar();
}

function parseCSV(data) {
    return data.split('\n').filter(line => line.trim() !== '').map(line => {
        const [date, event] = line.split(',');
        return { date, event };
    });
}

function createDayCell(day) {
    const dayCell = document.createElement('div');
    dayCell.classList.add('col', 'border', 'd-flex', 'flex-column', 'align-items-center', 'justify-content-start', 'p-2', 'day-cell');
    dayCell.setAttribute('data-day', day);

    const dayNumber = document.createElement('strong');
    dayNumber.innerText = day;
    dayCell.appendChild(dayNumber);

    return dayCell;
}

function createEmptyDayCell() {
    const emptyCell = document.createElement('div');
    emptyCell.classList.add('col', 'border', 'd-flex', 'flex-column', 'align-items-center', 'justify-content-start', 'p-2', 'day-cell');

    const placeholder = document.createElement('div');
    placeholder.classList.add('day-placeholder');
    emptyCell.appendChild(placeholder);

    return emptyCell;
}

function updateMonthLabel() {
    const monthNames = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    const month = currentDate.getMonth();
    const year = currentDate.getFullYear();
    const monthLabel = document.getElementById('monthLabel');
    monthLabel.innerText = `${monthNames[month]} ${year}`;
}