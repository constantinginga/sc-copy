const nextBtn = document.querySelector('.btn');
const nextBtnContainer = document.querySelector('.continue');
const backBtn = document.querySelector('#back-btn');
const cvrContainer = document.querySelector('.cvr-container');
const ddContainer = document.querySelector('.dd-container');
const yesRadio = document.querySelector('#ja');
const yesLabel = document.querySelector('#ja-label');
const noRadio = document.querySelector('#nej');
const noLabel = document.querySelector('#nej-label');
const cvrP = document.querySelector('#cvr-p');
const steps = document.querySelectorAll('.step');
const tabs = document.querySelectorAll('.tab');
const cvrInput = document.querySelector('#cvr');
const select = document.querySelector('#select');
const emailInput = document.querySelector('#email');
const fornavnInput = document.querySelector('#fornavn');
const efternavnInput = document.querySelector('#efternavn');
const phoneInput = document.querySelector('#phone-number');
const notesInput = document.querySelector('#additional-info');
const dataInfo = document.querySelector('#data-info');
const readMore = document.querySelector('#read-more');
const termsCheckBox = document.querySelector('#accept-terms');
const termsLabel = document.querySelector('#accept-terms-label');
const dataLabel = document.querySelector('#accept-data-label');
const dataCheckBox = document.querySelector('#accept-data');
const placeholders = document.querySelectorAll('.confirmation-container .highlight');
const formular = document.querySelector('.formular');
const endBtn = document.querySelector('.end-page > .btn');
const SELECT_TXT = placeholders[1].parentElement.textContent.split('registeret')[0];
const CVR_TXT = placeholders[1].parentElement.textContent;
const BORDER_COLOR = '#cbccce';

let currentTab = 0;

const showTab = n => {
    tabs[n].classList.remove('is-disabled');
    backBtn.style.visibility = (n == 0) ? 'hidden' : 'visible';
    if (n == (tabs.length - 1)) {
        nextBtn.innerHTML = 'Indsend formular';
        nextBtn.id = 'SumbmitFormFormular';
        dataInfo.classList.add('is-disabled');
        readMore.classList.remove('is-disabled');
    } else {
        nextBtn.innerHTML = 'Næste';
        nextBtn.id = 'NextPageFormFormular';
        dataInfo.classList.remove('is-disabled');
        readMore.classList.add('is-disabled');
    }
    updateProgress(n);
}

const updateProgress = n => {
    // remove active-step from all steps
    steps.forEach(s => {
        s.classList.remove('active-step');
        s.children[1].classList.remove('step-dot__previous');
    });

    // add back active steps
    for (let i = 0; i < n + 1; i++) {
        steps[i].style.opacity = (i != n) ? '.6' : '1';
        if (i != n) {
            steps[i].style.opacity = '.6';
            steps[i].classList.add('previous-step');
            steps[i].children[1].classList.add('step-dot__previous');

        } else {
            steps[i].style.opacity = '1';
            steps[i].classList.remove('previous-step');
            steps[i].classList.remove('step-dot__previous');
        }
        steps[i].classList.add('active-step');
    }
}

const nextPrev = n => {

    // exit function if fields are invalid
    if (n == 1 && !checkPage(currentTab)) return false;

    if (currentTab + n < tabs.length) {

        tabs[currentTab].classList.add('is-disabled');
        currentTab += n;

        if (currentTab == tabs.length - 1) {
            document.querySelector('.permissions').style.marginTop = '19.3em';
            nextBtnContainer.style.marginTop = '1.5em';
        } else if (currentTab == 0) {
            nextBtnContainer.style.marginTop = '24em';
        } else nextBtnContainer.style.marginTop = '2em';

        showTab(currentTab);
    } else {
        // document.getElementsByTagName('form')[0].submit();
        // return false;

        // render final page
        formular.removeChild(formular.querySelector('section.info'));
        formular.removeChild(formular.querySelector('section.main'));
        formular.querySelector('.end-page').classList.remove('is-disabled');
        console.log(formular.querySelector('.end-page'));
        formular.style.height = '55em';
    }
}

const checkPage = n => {
    switch (n) {
        case 0:
            return checkFirstPage();
        case 1:
            return checkSecondPage();
        case 2:
            return checkLastPage();
        default:
            return true;
    }
}

const checkFirstPage = () => {
    if (!yesRadio.checked && !noRadio.checked) {
        yesLabel.style.color = 'red';
        noLabel.style.color = 'red';
        cvrP.style.color = 'red';
        return false;
    }

    if (yesRadio.checked) {
        if (!cvrInput.value || cvrInput.value.length != 8) {
            return triggerError(cvrInput);
        }
    }

    if (noRadio.checked) {
        if (select.options[select.selectedIndex].value == 'default') {
            return triggerError(select);
        }
    }

    return true;
}

const checkSecondPage = () => {
    if (checkName() && checkPhone() && checkEmail()) {
        setPlaceholders();
        return true;
    }

    return false;
}

const checkLastPage = () => {
    if (!dataCheckBox.checked) {
        dataLabel.style.color = 'red';
        return false;
    }
    if (!termsCheckBox.checked) {
        termsLabel.style.color = 'red';
        return false;
    }

    dataLabel.style.color = '#000';
    termsLabel.style.color = '#000';

    //$('#SumbmitFormFormular').click(function () {
    var name = fornavnInput.value + " " + efternavnInput.value;

    var dataForm =
    {
        CVRNumber: cvrInput.value,
        Industry: select.value,
        Email: emailInput.value,
        Name: name,
        PhoneNumber: phoneInput.value,
        Notes: notesInput.value
    };

    $.ajax({
        url: '/umbraco/api/FormularLogger/Add',
        type: 'post',
        data: dataForm,
        success: function (response, status) {
            if (response) {
             
            }
        }
    });
   // });

    return true;
}

const checkName = () => {
    if (!fornavnInput.value) {
        return triggerError(fornavnInput);
    }
    if (!efternavnInput.value) {
        return triggerError(efternavnInput);
    }

    return true;
}

const checkEmail = () => {
    if (!emailInput.value || !(/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(emailInput.value))) {
        return triggerError(emailInput);
    }

    return true;
}

const checkPhone = () => {
    if (!phoneInput.value || !(/^[-+]?[0-9]+$/).test(phoneInput.value) || phoneInput.value.length != 8 ) {
        return triggerError(phoneInput);
    }

    return true;
}

// set spans for last page
const setPlaceholders = () => {
    const temp = placeholders[1].parentElement;
    placeholders[0].textContent = fornavnInput.value.trim() + ' ' + efternavnInput.value.trim();
    if (cvrInput.value) {
        placeholders[1].textContent = cvrInput.value.trim();
        temp.textContent = CVR_TXT;
    } else {
        placeholders[1].textContent = select.options[select.selectedIndex].text;
        temp.textContent = SELECT_TXT;
    }
    temp.appendChild(placeholders[1]);
    placeholders[1].parentElement.replaceWith(temp);
    placeholders[2].innerHTML = emailInput.value.trim();
    placeholders[3].innerHTML = phoneInput.value.trim();
}

const triggerError = input => {
    input.style.borderColor = 'red';
    return false;
}

const disableError = input => {
    if (input.style.borderColor == 'red') input.style.borderColor = BORDER_COLOR;
}

yesRadio.addEventListener('click', () => {
    if (yesLabel.style.color == 'red') {
        yesLabel.style.color = '#000';
        noLabel.style.color = '#000';
        cvrP.style.color = '#000';
    }
    select.style.borderColor = BORDER_COLOR;
    cvrContainer.classList.remove('is-hidden', 'is-disabled');
    select.selectedIndex = 0;
    ddContainer.classList.add('is-disabled');
});

noRadio.addEventListener('click', () => {
    if (yesLabel.style.color == 'red') {
        yesLabel.style.color = '#000';
        noLabel.style.color = '#000';
        cvrP.style.color = '#000';
    }

    cvrInput.style.borderColor = BORDER_COLOR;
    cvrContainer.classList.add('is-disabled');
    cvrInput.value = '';
    ddContainer.classList.remove('is-disabled');
});

nextBtn.addEventListener('click', () => {
    nextPrev(1);
});

backBtn.addEventListener('click', () => {
    nextPrev(-1);
});

cvrInput.addEventListener('keyup', () => {
    disableError(cvrInput);
});

fornavnInput.addEventListener('keyup', () => {
    disableError(fornavnInput);
});

efternavnInput.addEventListener('keyup', () => {
    disableError(efternavnInput);
});

emailInput.addEventListener('keyup', () => {
    disableError(emailInput);
});

phoneInput.addEventListener('keyup', () => {
    disableError(phoneInput);
});

select.addEventListener('change', () => {
    disableError(select);
});

termsCheckBox.addEventListener('change', () => {
    if (termsLabel.style.color == 'red') termsLabel.style.color = '#000';
});

dataCheckBox.addEventListener('change', () => {
    if (dataLabel.style.color == 'red') dataLabel.style.color = '#000';
});

endBtn.addEventListener('click', () => {
    window.location.replace('https://www.startupcentral.dk/');
});

