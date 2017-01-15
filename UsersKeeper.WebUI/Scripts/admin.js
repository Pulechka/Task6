/// <reference path="jquery-1.9.1.js" />
/// <reference path="jquery-1.9.1.intellisense.js" />

$('.user').on('click', 'button', function (e) {
    var buttonClassName = e.target.className;
    var login = e.target.parentElement.getElementsByClassName("login").item(0);

    if (buttonClassName.includes("button-set-admin")) {
        $.ajax({
            url: "/Views/Service/AdminPanel",
            type: "post",
            data: {
                login: login.innerHTML,
                isAdmin: true,
            }
        })
        .success(function () {
            e.target.innerHTML = "Unset as Admin";
            e.target.classList.remove("button-set-admin");
            e.target.classList.add("button-unset-admin");
        });
    }
    else if (buttonClassName.includes("button-unset-admin")) {
        $.ajax({
            url: "/Views/Service/AdminPanel",
            type: "post",
            data: {
                login: login.innerHTML,
                isAdmin: false,
            }
        })
        .success(function () {
            e.target.innerHTML = "Set as Admin";
            e.target.classList.remove("button-unset-admin");
            e.target.classList.add("button-set-admin");
        });
    }
});