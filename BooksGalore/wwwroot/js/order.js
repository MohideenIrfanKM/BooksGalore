var datatable;

$(document).ready(function () {
    loaddata();
});

function loaddata() { //below method complex just for ajax api calls
    datatable = $('#myTable2').DataTable({
        "ajax": {
            "url": "/Admin/Orders/GetAll"
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
                            <a  href="#" class="btn btn-outline-info" >Details</a>`
                           

                },


            }

        ]
    });
}