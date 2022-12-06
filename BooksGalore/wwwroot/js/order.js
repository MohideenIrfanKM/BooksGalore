var datatable;

$(document).ready(function () {
    var url = window.location.search; //to get url in javascript
    if (url.includes("inprocess")) {
        loaddata("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loaddata("completed");
        }
        else {
            if (url.includes("pending")) {
                loaddata("pending");
            }
            else {
                if (url.includes("approved")) {
                    loaddata("approved");
                }
                else {
                    loaddata("all");
                }
            }
        }
    }
});

function loaddata(status) { //below method complex just for ajax api calls
    datatable = $('#myTable2').DataTable({
        "ajax": {
            "url": "/Admin/Orders/GetAll?status=" + status,
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "phoneNumber" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "orderTotal" },


            {
                "data": "id",
                "render": function (data) {
                    /*if its not rendering please enable js debugging under toold-options-debugging*/
                    return ` 
                            <div class="btn-group">
                            <a  href="/Admin/Orders/Details?id=${data}" class="btn btn-outline-info" >Details</a>`
                           

                },


            }

        ]
    });
}