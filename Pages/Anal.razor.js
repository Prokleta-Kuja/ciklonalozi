export function showCharts(data) {
    console.log(data)
    const monthCount = document.getElementById("month-count");
    const yearCount = document.getElementById("year-count");
    const monthPrice = document.getElementById("month-price");
    const yearPrice = document.getElementById("year-price");
    const monthRemoved = document.getElementById("month-removed");
    const yearRemoved = document.getElementById("year-removed");


    new Chart(monthCount, {
        type: 'line',
        data: {
            datasets: Object.keys(data.monthCount).map(name => ({
                label: name,
                data: data.monthCount[name]
            }))
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Mjesečno naloga'
                }
            }
        }
    });
    new Chart(yearCount, {
        type: 'bar',
        data: {
            datasets: [{ data: data.yearCount }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    display: false,
                },
                title: {
                    display: true,
                    text: 'Godišnje naloga'
                }
            }
        }
    });

    new Chart(monthPrice, {
        type: 'line',
        data: {
            datasets: Object.keys(data.monthPrice).map(name => ({
                label: name,
                data: data.monthPrice[name]
            }))
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Ukupno mjesečno €'
                }
            }
        }
    });
    new Chart(yearPrice, {
        type: 'bar',
        data: {
            datasets: [{ data: data.yearPrice }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    display: false,
                },
                title: {
                    display: true,
                    text: 'Ukupno godišnje €'
                }
            }
        }
    });

    new Chart(monthRemoved, {
        type: 'line',
        data: {
            datasets: Object.keys(data.monthRemoved).map(name => ({
                label: name,
                data: data.monthRemoved[name]
            }))
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Mjesečno odustanaka'
                }
            }
        }
    });
    new Chart(yearRemoved, {
        type: 'bar',
        data: {
            datasets: [{ data: data.yearRemoved }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    display: false,
                },
                title: {
                    display: true,
                    text: 'Godišnje odustanaka'
                }
            }
        }
    });
}