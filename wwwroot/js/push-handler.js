self.addEventListener('push', function (event) {
    if (!(self.Notification && self.Notification.permission === 'granted')) {
        return;
    }

    let data = {};
    if (event.data) {
        data = event.data.json();
    }

    let title = data.title;
    let message = data.message;
    let url = data.url;
    let icon = "/apple-touch-icon.png";

    // https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorkerRegistration/showNotification
    event.waitUntil(self.registration.showNotification(title, {
        body: message,
        icon: icon,
        data: { url: url }
    }));
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();
});