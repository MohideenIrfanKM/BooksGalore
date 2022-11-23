var datatable;

$(document).ready(function () {
    loaddata();
});

function loaddata() { //below method complex just for ajax api calls
    datatable = $('#myTable3').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetCompanies"
        },
        "columns": [
            { "data": "name" },
            { "data": "streetAddress" },
            { "data": "city" },
            { "data": "state" },
            { "data": "postalCode" },
            { "data": "phoneNumber" },

            {
                "data": "id",
                "render": function (data) {
                    /*if its not rendering please enable js debugging under toold-options-debugging*/
                    return ` 
                            <div class="btn-group">
                            <a  href="/Admin/Company/Upsert?id=${data}" class="btn btn-outline-info" >Edit</a>
                            <a  onClick=Delete('/Admin/Company/Delete?id=${data}') class="btn btn-outline-danger" >Delete</a>
                            </div>
                            `

                },


            }

        ]
    });
}
function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this Company details!!!!",
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










