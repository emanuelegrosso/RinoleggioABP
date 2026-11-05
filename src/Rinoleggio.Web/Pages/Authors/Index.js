$(function () {
    var l = abp.localization.getResource('Rinoleggio');
    var authorService = rinoleggio.authors.author;
    var createModal = new abp.ModalManager(abp.appPath + 'Authors/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Authors/EditModal');
    var dataTable = $('#AuthorsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(authorService.getList),
            columnDefs: [
                { title: l('Actions'), rowAction: { items: [
                    { text: l('Edit'), visible: abp.auth.isGranted('Rinoleggio.Authors.Edit'), action: function (data) { editModal.open({ id: data.record.id }); } },
                    { text: l('Delete'), visible: abp.auth.isGranted('Rinoleggio.Authors.Delete'), confirmMessage: function (data) { return l('AuthorDeletionConfirmationMessage', data.record.name); }, action: function (data) { authorService.delete(data.record.id).then(function() { abp.notify.success(l('DeletedSuccessfully')); dataTable.ajax.reload(); }); } }
                ] } },
                { title: l('Name'), data: "name" },
                { title: l('BirthDate'), data: "birthDate", dataFormat: "datetime" },
                { title: l('ShortBio'), data: "shortBio" },
                { title: l('CreationTime'), data: "creationTime", dataFormat: "datetime" }
            ]
        })
    );
    createModal.onResult(function () { abp.notify.success(l('CreatedSuccessfully')); dataTable.ajax.reload(); });
    editModal.onResult(function () { abp.notify.success(l('SavedSuccessfully')); dataTable.ajax.reload(); });
    $('#NewAuthorButton').click(function (e) { e.preventDefault(); createModal.open(); });
    $('#ExportAuthorsToExcelButton').click(function (e) {
        e.preventDefault();
        authorService.getDownloadToken().then(function(result) {
            window.location.href = abp.appPath + 'api/app/author/as-excel-file?downloadToken=' + encodeURIComponent(result.token);
        });
    });
});
