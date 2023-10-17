const notificationContainer = document.getElementsByClassName('sc-logged');

const NOTIFICATION_TYPES = {
	INFO: 'Info',
	SUCCESS: 'Succes',
	WARNING: 'Advarsel',
	DANGER: 'Fare'
}

function addNotification(type, text) {
	const newNotification = document.createElement('div');
	newNotification.classList.add(`snackbar`, `snackbar--has-action`, `snackbar--${type}`);

	const innerNotification = `<span class="snackbar__message"><strong>${type}:</strong> ${text}</span>`;

	newNotification.innerHTML = innerNotification;
	notificationContainer[0].appendChild(newNotification);

	return newNotification;
}

function removeNotification(notification) {
	notification.classList.add('hide');

	setTimeout(() => {
		notificationContainer[0].removeChild(notification);
	}, 1000);
}

//testing example
//const info = addNotification(NOTIFICATION_TYPES.INFO, 'Info text added');
//setTimeout(() => {
//	removeNotification(info);
//}, 5000);