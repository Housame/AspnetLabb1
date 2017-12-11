$(document).ready(function () {

    var firstNameInput = $("#addForm [name=FirstName]");
    var lastNameInput = $("#addForm [name=LastName]");
    var emailInput = $("#addForm [name=Email]");
    var ageInput = $("#addForm [name=Age]");
    var genderInput = $("#inputgender option:selected");
    var formBtn = $("#submit-customer");
    var uploadCSV = $("#file-uploading");
    var initData = $("#init-data");

    UpdateTable();

    //-------------------------------------------------------------------------------------

    $(document).on('click', '.addNew', function () {
        $.ajax({
            url: '/api/customer/',
            method: 'POST',
            data: {
                "FirstName": firstNameInput.val(),
                "LastName": lastNameInput.val(),
                "Email": emailInput.val(),
                "Age": ageInput.val(),
                "Gender": genderInput.text().toString()
            }

        })
            .done(function (result) {

                console.log("Success!", result);
                UpdateTable();

            })

            .fail(function (xhr, status, error) {

                alert(`Fail!`);
                console.log("Error", xhr, status, error);

            });
    });

    $(document).on('click', '.remove', function () {
        var id = $(this).attr('data-removeId');
        console.log(id);
        $.ajax({
            url: "/api/customer",
            type: 'Delete',
            data: { 'id': id }
        })
            .done(function (result) {
                alert(id + " deleted");
                UpdateTable();
            });
    });

    $(document).on('click', '.uppgrade', function () {
        var id = $(this).attr('data-uppgradeId');
        $.ajax({
            url: '/api/customer/',
            type: 'Put',
            data: { 'id': id }
        }).done(function (result) {
            console.log(result);
            firstNameInput.val(result.firstName);
            lastNameInput.val(result.lastName);
            emailInput.val(result.email);
            genderInput.val(result.gender);
            ageInput.val(result.age);
            switchBtnFunc(0, result.id);
            console.log(result.id);
        });
    });

    $(document).on('click', '.uppgradeDB', function () {
        var id = formBtn.data('id');
        console.log(id);
        $.ajax({
            url: "/api/customer",
            type: 'Patch',
            data: {
                'id': id,
                'firstName': firstNameInput.val(),
                'lastName': lastNameInput.val(),
                'email': emailInput.val(),
                'age': ageInput.val(),
                'gender': genderInput.text().toString()
            }
        })
            .done(function (result) {
                $("#upgradeCustomerDiv").css('display', 'none');
                alert(" uppgraded !");
                UpdateTable();
                switchBtnFunc(1);
            });
    });

    uploadCSV.on('click', function () {
        $.ajax({
            url: '/api/customer/csv',
            type: 'Get'
        }).done(function (result) {
            console.log("Success");
            UpdateTable();
        }).fail(function (xhr, status, error) {
            alert("fail");
        });

    });

    initData.on('click', function () {
        if (confirm("Are you sure? All data will be lost!") === true) {
            $.ajax({
                url: '/api/customer/removeAll',
                type: 'Get'
            }).done(function (result) {
                console.log("Success");
                UpdateTable();
            }).fail(function (xhr, status, error) {
                alert("fail");
            });
        }
    });

    function UpdateTable() {
        $('#customerTable').empty();
        $.ajax({
            url: '/api/customer/',
            method: 'Get'

        })
            .done(function (result) {
                console.log("Success!", result);
                $.each(result, function (i, item) {
                    var $tr = $('<tr>').append(
                        $('<th scope="row">').text(i + 1),
                        $('<td>').text(item.firstName),
                        $('<td>').text(item.lastName),
                        $('<td>').text(item.email),
                        $('<td>').text(item.age),
                        $('<td>').text(item.gender),
                        $('<td>').text(splitDateTime(item.dateOfCreation)),
                        $('<td>').text(splitDateTime(item.dateOfEdit)),
                        $('<td>').append('<button class="btn btn-danger remove" data-removeId="' + item.id + '"> Ta bort </button> <button class="btn btn-success uppgrade" data-uppgradeId="' + item.id + '"> Uppgradera </button>')
                    ).appendTo('#customerTable');
                });

            })

            .fail(function (xhr, status, error) {
                console.log("Error", xhr, status, error);
            });
    }

    function splitDateTime(date) {

        var year = date.substring(10, 0);
        var time = date.substring(19, 11);
        var timeToReturn = "";
        if (year === "0001-01-01") {
            timeToReturn = "Never edited";
        }
        else timeToReturn = year + " " + time;

        return timeToReturn;
    }

    function switchBtnFunc(value, id) {
        if (value === 0) {
            formBtn.removeClass("btn-primary addNew");
            formBtn.addClass("btn-success uppgradeDB");
            formBtn.text("save changes");
            formBtn.data('id', id);
        }
        else {
            formBtn.removeClass("btn-success uppgradeDB");
            formBtn.addClass("btn-primary addNew");
            formBtn.text("submit");
            firstNameInput.val("");
            lastNameInput.val("");
            emailInput.val("");
            ageInput.val("");
            genderInput.val("Man");
        }
    }
});



