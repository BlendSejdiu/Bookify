var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "state", "width": "10%" },
            { "data": "city", "width": "10%" },
            { "data": "role", "width": "10%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                        <div class="text-center btn-group">
                            <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white mx-2" style="cursor:pointer; width:100px;">
                            <i class="bi bi-unlock-fill"></i>  UnLock
                                </a>
                            <a onclick=Delete('/admin/user/delete/${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;"><i class="bi bi-trash-fill"></i> Delete</a>
                        </div>
                    `
                    }
                    else {
                        return `
                        <div class="text-center btn-group">
                             <a onclick=LockUnlock('${data.id}') class="btn btn-primary text-white mx-2" style="cursor:pointer; width:100px;">
                                <i class="bi bi-lock-fill"></i>  Lock
                             </a> 
                             <a onclick=Delete('/admin/user/delete/${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;"><i class="bi bi-trash-fill"></i> Delete</a>
                        </div>
                    `
                    }
                },
                "width": "15%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax(
                {
                    url: url,
                    type: 'DELETE',
                    success: function (data) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }

                }
            )
        }
    });
}