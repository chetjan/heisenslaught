let userName = prompt("Enter your name: ");
let chat = $.connection.draftHub;
chat.client.messageReceived = function (originatorUser, message) {
    $("#draft").append('<li><strong>' + originatorUser + '</strong>: ' + message);
};

chat.client.getConnectedUsers = function (userList) {
    for (let i = 0; i < userList.length; i++) {
        addUser(userList[i]);
    }
};

chat.client.newUserAdded = function (newUser) {
    addUser(newUser);
}

$("#heroPick").focus();

$("#selectHero").click(function () {
    chat.server.send(userName, $("#heroPick").val());
    $("#heroPick").val("");
    $("#heroPick").focus();
});

function addUser(user) {
    $("#userList").append('<li>' + user + '</li>');
}

$.connection.hub.logging = true;
$.connection.hub.start().done(function () {
    chat.server.connect(userName);
});