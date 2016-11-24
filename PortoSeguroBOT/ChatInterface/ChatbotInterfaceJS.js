var conversationId = "";
var conversationToken = "";
var watermark = 0;
$(function () {
    $(".chatInput").keydown(function (e) {
        if (e.keyCode == "13") {
            manageSubmitText();
            e.preventDefault();
        }
    })

    $("#chatInputBtn").click(function () {
        manageSubmitText();
        $(".chatInput").focus();
    })

    getTokens();
})

function getTokens() {
    var xhr = $.ajax({
        url: "../rest/getToken",
        type: "POST",
        data: "",
        timeout: 5000,
        contentType: "application/json",
        success: function(data) {
            conversationId = data.ConversationId;
            conversationToken = data.ConversationToken;
            watermark = data.WaterMark;
            window.setTimeout(function () { getBotMessage(); }, 2000);
        },
        error: function(data) { }
    });
}

function getBotMessage() {
    var xhr = $.ajax({
        url: "../rest/botToUser/" + conversationId + "/" + watermark,
        type: "POST",
        data: "",
        timeout: 10000,
        contentType: "application/json",
        success: function (data) {
            for (var msg = 0; msg < data.Messages.length; msg++) {
                generateBotMsg(data.Messages[msg].Text);
                watermark++;
            }
        },
        error: function (data) { }
    });
}

function sendMessageToBot(txt) {
    var xhr = $.ajax({
        url: "../rest/userToBot/" + conversationId,
        type: "POST",
        data: "{\"text\":\""+txt+"\"}",
        timeout: 10000,
        contentType: "application/json",
        success: function (data) {
            watermark++;
            getBotMessage();
        },
        error: function (data) {
            console.log(data);
        }
    });
}

function manageSubmitText() {
    var val = $.trim($(".chatInput").val());
    if (val.length > 0) {
        var msgWrapper = $("<div class='chatMessageWrapper chatMessageSenderWrapper'>");
        var msgDiv = $("<div class='chatMessageDiv chatMessageSender'>");
        var msg = $("<span>").text(val);
        $(msg).appendTo(msgDiv);
        $(msgDiv).appendTo(msgWrapper);
        $(msgWrapper).appendTo(".chatContentMsgWrapper");
        $(".chatContentMsgWrapper")[0].scrollTop = $(".chatContentMsgWrapper")[0].scrollHeight;
        $(".chatInput").val("");
        sendMessageToBot(val);
    } else {
        $(".chatInput").val("");
    }
}

function generateBotMsg(txt) {
    if (txt.length > 0) {
        var msgWrapper = $("<div class='chatMessageWrapper chatMessageReceiverWrapper'>");
        var msgDiv = $("<div class='chatMessageDiv chatMessageReceiver'>");
        var msg = $("<span>").text(txt);
        $(msg).appendTo(msgDiv);
        $(msgDiv).appendTo(msgWrapper);
        $(msgWrapper).appendTo(".chatContentMsgWrapper");
        $(".chatContentMsgWrapper")[0].scrollTop = $(".chatContentMsgWrapper")[0].scrollHeight;
    }
}