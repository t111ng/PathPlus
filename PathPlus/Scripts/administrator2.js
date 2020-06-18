
$('.CaptchaButton').click(function () {
    CaptchaForm.Captcha.src = "/AdministratorLogin/getCaptcha?" + Math.random();
});

inactivityTime();
function inactivityTime() {
    var t;
    window.onload = resetTimer;
    window.onmousemove = resetTimer;
    window.onkeypress = resetTimer;
    window.onmousedown = resetTimer;
    window.onclick = resetTimer;
    window.onscroll = resetTimer;

    function logout() {
        alert("畫面逾時, 請重新登入")
        window.location = "/AdministratorHome/Autologout";
    }

    function resetTimer() {
        clearTimeout(t);
        var sessionTimeoutWarning = 30;
        var sTimeout = parseInt(sessionTimeoutWarning) * 60 * 1000;
        t = setTimeout(logout, sTimeout);
    }
};
