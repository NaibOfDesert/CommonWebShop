$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'title', "width": "15%" },
            { data: 'author', "width": "15%" },
            { data: 'price', "width": "10%" },
            { data: 'category.name', "width": "10%" },
            { data: 'description', "width": "30%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                            <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i>Edit
                            </a>
                            <a onClick=Delete("/admin/product/delete?id=${data}") class="btn btn-danger mx-2">
                            <i class="bi bi-x-square"></i> Delete
                            </a>
                            </div>`
                },
                "width": "20%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        color: '#44d9e8',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#44d9e8',  
        cancelButtonColor: '#6f42c1',
        confirmButtonText: 'Yes, delete it!',
        background: '#170229'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}