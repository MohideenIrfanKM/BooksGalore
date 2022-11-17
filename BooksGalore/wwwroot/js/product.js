var datatable;

$(document).ready(function () {
    loaddata();
});

function loaddata() { //below method complex just for ajax api calls
    $('#myTable').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetProducts"
        },
        "columns": [
            { "data": "name" },
            { "data": "isbn" },
            { "data": "price" },
            { "data": "author" },
            { "data": "category.name" },





        ]
    });
}