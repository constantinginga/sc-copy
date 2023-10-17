//var table = document.querySelector('.opgaver-table');
//var newRows = document.querySelectorAll('.opgaver-table__new-row');

//if (newRows) {
//    newRows.forEach(row => {
//        row.addEventListener('click', () => {
//            row.classList.remove('opgaver-table__new-row');
//        });
//    });
//}

// sorting functionality
for (let i of Array.from(table.children)) {
    for (let row of Array.from(i.children)) {
        for (let th of Array.from(row.children)) {
            if (th.tagName === 'TH')
                th.addEventListener('click', function (e) {
                    sortTable(e.target.cellIndex);
                    th.classList.contains('sort-down') ? th.classList.replace('sort-down', 'sort-up') : th.classList.replace('sort-up', 'sort-down');
                });
        }
    }
}

// sorting functionality
function sortTable(n) {

    let rows, switching, i, x, y, xNew, yNew, shouldSwitch, dir, switchcount = 0;
    switching = true;
    dir = 'desc';

    while (switching) {

        switching = false;
        rows = table.rows;

        for (i = 1; i < (rows.length - 1); i++) {

            shouldSwitch = false;

            x = rows[i].getElementsByTagName('TD')[n];
            y = rows[i + 1].getElementsByTagName('TD')[n];

            if (n === 0) {
                xNew = new Date(...formatDate(x.innerHTML)).getTime();
                yNew = new Date(...formatDate(y.innerHTML)).getTime();
            } else if (n === 1 && (!x.children.length || !y.children.length)) {
                // compare cvr correctly
                yNew = (x.children.length) ? x.children[0].textContent.toLowerCase() : x.innerHTML.toLowerCase();
                xNew = (y.children.length) ? y.children[0].textContent.toLowerCase() : y.innerHTML.toLowerCase();
            } else if (n === 5) {
                xNew = Number(x.innerHTML);
                yNew = Number(y.innerHTML);
            } else {
                xNew = x.innerHTML.toLowerCase();
                yNew = y.innerHTML.toLowerCase();
            }

            if (dir == 'asc') {
                if (xNew > yNew) {
                    shouldSwitch = true;
                    break;
                }
            } else if (dir == 'desc') {
                if (xNew < yNew) {
                    shouldSwitch = true;
                    break;
                }
            }
        }

        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            switchcount++;
        } else {
            if (switchcount == 0 && dir == 'desc') {
                dir = 'asc';
                switching = true;
            }
        }
    }
}

function formatDate(d) {
    [d, m, y] = d.split('-');
    return [y, m - 1, d];
}