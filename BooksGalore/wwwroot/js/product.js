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
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="btn-group">
                            <a  href="/Admin/Product/Upsert?id=${data}" class="btn btn-outline-info" >Edit</a>
                            <a  class="btn btn-outline-danger" >Delete</a>
                            </div>
                            `
            
                },
            
                
                }

        ]
    });
}