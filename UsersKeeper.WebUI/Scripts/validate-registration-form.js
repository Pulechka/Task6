$().ready(function () {
    $("#registationForm").validate({
        rules: {
            login: {
                required: true,
                minlength: 3
            },
            password: {
                required: true,
                minlength: 6
            },
            repeatPassword: {
                required: true,
                minlength: 6,
                equalTo: "#password"
            },
        },
        messages: {
            login: {
                required: "Please enter your login",
                minlength: "Your login must consist of at least 3 characters"
            },
            password: {
                required: "Please provide your password",
                minlength: "Your password must consist of at least 6 characters"
            },
            repeatPassword: {
                required: "Please provide your password",
                minlength: "Your password must consist of at least 6 characters",
                equalTo: "Please enter the same password as above"
            },
        }
    });
});