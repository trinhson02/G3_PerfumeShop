const stat_connect = new signalR.HubConnectionBuilder()
    .withUrl("/statisticsHub")  // Kết nối tới SignalR Hub
    .build();

const chat_connect = new signalR.HubConnectionBuilder()
    .withAutomaticReconnect()
    .withUrl("/chathub")
    .build();

stat_connect.on("ReceiveStatistics", function (newData) {
    // Cập nhật số liệu thống kê mới vào giao diện
    document.getElementById("statisticsDisplay").innerText = newData;
});

stat_connect.start().catch(function (err) {
    return console.error(err.toString());
});

// Phần này có thể gọi từ server để đẩy số liệu mới (khi server cập nhật)
function sendStatisticsUpdate(newData) {
    stat_connect.invoke("UpdateStatistics", newData).catch(function (err) {
        return console.error(err.toString());
    });
}

chat_connect.start().then(function () {
    document.getElementById("chatbox").addEventListener("keyup", function (event) {
        if (event.key === "Enter") {
            chat_connect.invoke("SendMessage", event.target.value);
            event.target.value = "";
        }
    });
}).catch(function (err) {
    return console.error(err.toString());
});

chat_connect.on("ReceiveMessage", function (message) {
    const messages = document.getElementById("messages");
    messages.innerHTML += `<p>${message}</p>`;
    notify('top', 'right', 'glyphicon glyphicon-info-sign', 'info', 'fadeInDown', 'fadeOutUp', 'x', message);
});