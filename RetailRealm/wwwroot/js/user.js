var dataTable;


$(document).ready(function () {
    loadDataTable();
});



function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'company.name', "width": "15%" },
            { data: 'role', "width": "15%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                            <div class="text-center d-flex" style="gap: 10px;"> 
                              <a onclick="LockUnlock('${data.id}')" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                              <i class="bi bi-lock-fill"></i> Lock
                              </a>

                              <a href="/Admin/User/RoleManagement?id=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:180px;">
                              <i class="bi bi-pencil-square"></i> Permission
                              </a>

                            </div>
                            `;
                    } else {
                        return `
                           <div class="text-center d-flex" style="gap: 10px;">
                             <a onclick="LockUnlock('${data.id}')" class="btn btn-success text-white" style="cursor:pointer; width:150px;">
                              <i class="bi bi-unlock-fill"></i> Unlock
                              </a>
      
                               <a href="/Admin/User/RoleManagement?id=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:180px;">
                              <i class="bi bi-pencil-square"></i> Permission
                              </a>

                            </div>
                            `;
                    }



                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> Edit</a>            
                    </div>`
                }
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