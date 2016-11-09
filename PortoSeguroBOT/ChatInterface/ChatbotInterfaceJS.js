function manageSubmitText(txt) {
    var val = $.trim($(".chatInput").val());
    if(val.length > 0) {
        var msgWrapper = $("<div class='chatMessageWrapper chatMessageSenderWrapper'>");
        var msgDiv = $("<div class='chatMessageDiv chatMessageSender'>");
        var msg = $("<span>").text(val);
        $(msg).appendTo(msgDiv);
        $(msgDiv).appendTo(msgWrapper);
        $(msgWrapper).appendTo(".chatContentMsgWrapper");
        $(".chatContentMsgWrapper")[0].scrollTop = $(".chatContentMsgWrapper")[0].scrollHeight;
        $(".chatInput").val("");
    } else {
        $(".chatInput").val("");
    }
}

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
})