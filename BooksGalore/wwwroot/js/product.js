var datatable;

$(document).ready(function () {
    loaddata();
});

function loaddata() { //below method complex just for ajax api calls
    datatable=$('#myTable').DataTable({
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
                            <a  onClick=Delete('/Admin/Product/Delete?id=${data}') class="btn btn-outline-danger" >Delete</a>
                            </div>
                            `
            
                },
            
                
                }

        ]
    });
}
function Delete(url)
{
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this product details!!!!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        datatable.ajax.reload();
                        toastr.success(data.msg);
                        //as we have to access toastr now directly in this request we are doing this. else use normal tempdata method as we have toastr.cshtml file
                    }
                    else
                        toastr.error(data.msg);

                }
            })
        });
}










