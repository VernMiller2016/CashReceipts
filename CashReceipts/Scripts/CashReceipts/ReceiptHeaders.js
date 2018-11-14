var selectedRows = undefined,
    currentGrid = null,
    clerksList,
    departmentsList,
    receiptsBodyDataSource,
    receiptsTendersDataSource,
    headersDataSources,
    receiptBodyDiv,
    templatesList,
    accountsList,
    paymentMethods,
    templatesDataSource,
    departmentDataSource;

$(document).ready(function () {
    $.when(
        $.getJSON(clerkListUrl),
        $.getJSON(deptListUrl)
    )
        .done(function (result1, result2) {
            if (result1[1] == "success") clerksList = result1[0].filter(function (item) { return isAdmin || item.value == userClerkId });
            if (result2[1] == "success") departmentsList = result2[0];
            receiptsHeaderGridInit();
        });
});

var dataBoundFirstTime = true;

function BindTopPager() {
    var g = $("#headersGrid"),    //grid object from jquery selector
        grid = g.data('kendoGrid'),
        pager = $('#headersGrid .k-pager-wrap'), //regular bottom pager
        pagerTop = $("#pager_top");      //additional top pager
    if (pagerTop.length == 0) {
        var newPager = $('<div id="pager_top" class="k-pager-wrap pagerTop" />')
            .insertBefore(g.find(".k-grouping-header")); //assumes that groupable is enabled
        g.topPager = new kendo.ui.Pager(newPager, $.extend({}, { 'refresh': true, 'pageSizes': true }, { dataSource: grid.dataSource }));
        g.topPager.refresh();
    }
}

var receiptsHeaderGridInit = function () {

    departmentDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: deptReadUrl,
                dataType: "json"
            }
        },
        serverPaging: false,
        schema: {
            data: "Data",
            total: "Total",
            model: {
                id: "value",
                fields: {
                    text: { type: "string" },
                    value: { type: "number" }
                }
            },
            errors: "Errors"
        },
        change: function (e) {
        },
        error: function (e) {
            this.cancelChanges();
        }
    });

    headersDataSources = new kendo.data.DataSource({
        transport: {
            read: {
                url: receiptHeaderReadUrl,
                dataType: "json"
            },
            update: {
                url: receiptHeaderUpdateUrl,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                type: "POST"
            },
            destroy: {
                url: receiptHeaderDestroyUrl,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                type: "POST"
            },
            create: {
                url: receiptHeaderCreateUrl,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "POST"
            },
            parameterMap: function (options, operation) {
                if (operation !== "read" && options.models) {
                    return JSON.stringify({ receiptHeaders: options.models });
                }
                //return JSON.stringify(options.models);
            }
        },
        batch: true,
        pageSize: 50,
        serverPaging: false,
        sort: [{ field: "ReceiptNumber", dir: "desc" }],
        schema: {
            data: "Data",
            total: "Total",
            model: {
                id: "ReceiptHeaderID",
                fields: {
                    ReceiptHeaderID: { editable: false, nullable: false, defaultValue: 0 },
                    DepartmentID: {
                        validation: {
                            required: true
                        },
                        type: "number"
                    },
                    ReceiptNumber: {
                        validation: {
                            required: true
                        },
                        type: "number",
                        defaultValue: getNextReceiptNumber()
                    },
                    ReceiptDate: {
                        validation: {
                            required: true
                        },
                        type: "date"
                    },
                    ReceiptTotal: {
                        validation: {
                            required: true
                        },
                        type: "number"
                    },
                    ClerkID: {
                        validation: {
                            required: true
                        },
                        type: "number"
                    },
                    Comments: {
                        type: "string"
                    },
                    ReceivedFor: {
                        type: "string"
                    }
                }
            },
            errors: "Errors"
        },
        error: function (e) {
            if (e.errors && e.errors["_addKey"]) {
                notify.showError(e.errors["_addKey"].errors[0]);
            } else if (e.errors && e.errors["_deleteKey"]) {
                notify.showError(e.errors["_deleteKey"].errors[0]);
            } else if (e.errors && e.errors["_updateKey"]) {
                notify.showError(e.errors["_updateKey"].errors[0]);
            } else {
                notify.showError('An error has been occurred, please contact system admin.');
            }
            this.cancelChanges();
        }
    });

    $("#headersGrid")
        .kendoGrid({
            dataSource: headersDataSources,
            //width: 600,
            selectable: "row",
            resizable: true,
            scrollable: true,
            sortable: true,
            filterable: true,
            groupable: true,
            reorderable: true,
            columnMenu: true,
            pageable: {
                refresh: true,
                input: true,
                numeric: false,
                pageSizes: [20, 50, 100, 500, 1000]
            },
            detailTemplate: kendo.template($("#template").html()),
            detailInit: receiptHeaderDetailInit,
            columns: [
                {
                    field: "ReceiptNumber",
                    title: "Receipt Number",
                    format: "{0}",
                    width: 100
                }, {
                    field: "ReceiptDate",
                    title: "Receipt Date",
                    format: "{0:MM/dd/yyyy}",
                    width: 90
                }, {
                    field: "ReceiptTotal",
                    title: "Receipt Total",
                    editor: oldNumericBoxEditor,
                    format: "{0:n2}",
                    width: 90
                },
                {
                    field: "DepartmentID",
                    title: "Department",
                    editor: departmentsDropDownEditor,
                    values: departmentsList,
                    width: 100
                },
                {
                    field: "ClerkID",
                    title: "Clerk",
                    values: clerksList,
                    width: 100
                },
                {
                    field: "Comments",
                    title: "Received From",
                    width: 110
                },
                {
                    field: "ReceivedFor",
                    title: "Received For",
                    width: 100
                },
                {
                    command: [
                        {
                            name: "edit",
                            visible: function (dataItem) { return !dataItem.IsPosted && hasEditReceiptsPermission && (isAdmin || dataItem.ClerkID == userClerkId) }
                        },
                        {
                            name: "remove", template: "<a role='button' class='k-button k-button-icontext k-grid-delete-header' onclick='deleteReceiptHeader(event, this)'><span class='k-icon k-i-close'></span>Delete</a>"
                        }
                    ],
                    title: "&nbsp;",
                    width: "90px"
                }
            ],
            editable: "inline",
            toolbar: getHeaderToolbarActions(),
            dataBound: function (e) {
                if (!currentGrid) {
                    currentGrid = $("#headersGrid").data("kendoGrid");
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                }
                enableDisableReceiptHeadersButtons(false);
                //receiptHeaderOnDataBound(e);

                if (dataBoundFirstTime) {
                    BindTopPager();
                    dataBoundFirstTime = false;
                }
            },
            edit: function (e) {
                //debugger;
                var editRow = e.container;
                if (e.model.isNew()) {
                    var receipts = this.dataSource.data();
                    var lastReceiptId = Math.max.apply(Math,
                        $(receipts)
                            .map(function (i, r) {
                                if (r.ReceiptNumber !== getNextReceiptNumber())
                                    return r
                                        .ReceiptNumber;
                            }));
                    e.model.set("ReceiptNumber", (lastReceiptId + 1));
                    e.model.ReceiptNumber = (lastReceiptId + 1);
                    //var ds = e.sender.dataSource;
                    //ds.sort(ds.sort());
                }
                editRow.find('td:eq(1) span.k-numeric-wrap span:first').remove();
                editRow.find('td:eq(1) span.k-numeric-wrap').css('padding', '0');
                editRow.find('td:eq(1) :input').attr('readonly', 'readonly');
            },
            change: function (e) {
                var grid = e.sender;
                var selectedItem = grid.dataItem(this.select());
                enableDisableReceiptHeadersButtons(true, selectedItem);
            },
            save: function (e) {
                var updateButton = $(e.container).find('.k-grid-update');
                if (!updateButton.hasClass('k-state-disabled')) {
                    updateButton.addClass('k-state-disabled');
                } else {
                    e.preventDefault();
                }
            },
            destroy: function () {
                enableDisableReceiptHeadersButtons(false);
            }
        });
}


var deleteReceiptHeader = function (e, elem) {
    e.preventDefault();
    var grid = $("#headersGrid").data("kendoGrid");
    var uid = $(elem).parents('tr:first').data('uid');
    var receiptHeaderId = grid.dataSource.getByUid(uid).ReceiptHeaderID;
    $('#deletedreceiptheaderid').val(receiptHeaderId);
    deleteReceiptHeaderWnd.center().open();
};

var deleteReceiptHeaderWnd = $("#deleteReceiptHeaderWnd")
    .kendoWindow({
        title: "Receipt Header Delete Confirmation",
        modal: true,
        visible: false,
        resizable: true,
        width: 400,
        close: function (e) {

        }
    }).data("kendoWindow");

function confirmReceiptHeaderDelete() {
    var reason = $('#deletereason').val();
    if (!reason) {
        alert('Please enter a delete reason.');
        return;
    }

    $.ajax({
        type: "POST",
        url: receiptHeaderDeleteUrl,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ Id: $('#deletedreceiptheaderid').val(), Reason: reason })
    }).done(function (data) {
        if (!data.Success) {
            alert(data.Message);
        }
        deleteReceiptHeaderWnd.close();
        //refresh the original grid
        var grid = $("#headersGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
            grid.refresh();
        }

        //clear modal values
        $('#deletedreceiptheaderid').val('');
        $('#deletereason').val('');

    });
}

function receiptHeaderOnDataBound(e) {
    var grid = $("#headersGrid").data("kendoGrid");
    var gridData = grid.dataSource.view();

    for (var i = 0; i < gridData.length; i++) {
        var currentUid = gridData[i].uid;
        if (gridData[i].IsPosted) {
            var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
            var editButton = $(currenRow).find(".k-grid-edit");
            editButton.hide();
            var deleteButton = $(currenRow).find(".k-grid-delete");
            deleteButton.hide();
        }
    }
}

function getNextReceiptNumber() {
    return Number.MAX_SAFE_INTEGER || 9007199254740991;
}

function departmentsDropDownEditor(container, options) {
    $('<input required data-text-field="text" data-value-field="value" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            //autoBind: false,
            //optionLabel: "Select Department",
            dataTextField: "text",
            dataValueField: "value",
            filter: "contains",
            dataSource: departmentDataSource
        });
}

function enableDisableReceiptHeadersButtons(isEnable, selectedItem) {
    if (!isEnable) {
        $('.k-grid-PrintReceipt').addClass("k-state-disabled").unbind("click");
        $('.k-grid-PostCheck').addClass("k-state-disabled").unbind("click");
    } else {
        $(".k-grid-PrintReceipt")
            .removeClass("k-state-disabled")
            .unbind("click")
            .bind("click",
                function () {
                    var recordUid = currentGrid.tbody.find(".k-state-selected").data('uid');
                    var receipt = headersDataSources.getByUid(recordUid);
                    if (receipt) {
                        $("#receiptHeaderId").val(receipt.ReceiptHeaderID);
                        $("#exportReceiptPDF").submit();
                    }
                });

        if (!selectedItem.IsPosted) {
            $(".k-grid-PostCheck")
                .removeClass("k-state-disabled")
                .unbind("click")
                .bind("click",
                    function () {
                        var recordUid = currentGrid.tbody.find(".k-state-selected").data('uid');
                        var receipt = headersDataSources.getByUid(recordUid);
                        if (receipt && hasPostPermission) {
                            $.post(checkReciptHeaderTotalsUrl + receipt.ReceiptHeaderID,
                                {
                                    ReceiptHeaderID: receipt.ReceiptHeaderID,
                                })
                                .done(function (data) {
                                    if (data) {
                                        if (data.Result) {
                                            if (hasLockPermission) {
                                                notify.postConfirm(data.Message, lockReceipt(receipt));
                                            }
                                            else {
                                                notify.showSuccess(data.Message);
                                            }
                                        } else {
                                            notify.showError(data.Message);
                                        }
                                    }
                                });
                        }
                    });
        } else {
            $('.k-grid-PostCheck').addClass("k-state-disabled").unbind("click");
        }
    }
}

function lockReceipt(receipt) {
    return function () {
        $.post(lockReceiptUrl + receipt.ReceiptHeaderID,
            {
                ReceiptHeaderID: receipt.ReceiptHeaderID
            })
            .done(function (data) {
                if (data) {
                    if (data.Result) {
                        notify.showSuccess(data.Message);
                        refreshHeadersGrid();
                    } else {
                        notify.showError(data.Message);
                    }
                }
            });
    }
}

function refreshHeadersGrid() {
    $("#headersGrid").data("kendoGrid").dataSource.read();
    $("#headersGrid").data("kendoGrid").refresh();
}

function receiptHeaderDetailInit(e) {
    var detailRow = e.detailRow;

    detailRow.find(".tabstrip")
        .kendoTabStrip({
            animation: {
                open: { effects: "fadeIn" }
            }
        });

    $.getJSON(templatesListUrl + '?includeAccounts=false', function (data) {
        templatesList = data;
        accountsList = $(data)
            .map(function (i, item) {
                return { value: item.value, text: item.AccountNumber };
            });
        receiptBodyDiv = detailRow.find(".receiptsBodyGrid");
        initReceiptsBodyGrid(receiptBodyDiv, e);
    });

    $.getJSON(paymentMethodsUrl, function (data) {
        paymentMethods = data;
        initTendersGrid(detailRow.find(".tendersGrid"), e);
    });
}

function initTendersGrid(receiptTendersDiv, headersGridRef) {
    receiptsTendersDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: tendersReadUrl,
                //contentType: "application/json; charset=utf-8",
                dataType: "json"
            },
            update: {
                url: tendersUpdateUrl,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                type: "POST"
            },
            destroy: {
                url: tendersDestroyUrl,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                type: "POST"
            },
            create: {
                url: tendersCreateUrl,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "POST"
            },
            parameterMap: function (options, operation) {
                if (operation !== "read" && options.models) {
                    if (operation == "create")
                        return (JSON.stringify({ receiptHeaderId: headersGridRef.data.ReceiptHeaderID, receiptTenders: options.models }));
                    return JSON.stringify({ receiptTenders: options.models });
                }
            }
        },
        batch: true,
        pageSize: 10,
        filter: { field: "ReceiptHeaderID", operator: "eq", value: headersGridRef.data.ReceiptHeaderID },
        dataBound: function () {
        },
        serverPaging: false,
        aggregate: [
            { field: "Amount", aggregate: "sum" }
        ],
        schema: {
            data: "Data",
            total: "Total",
            model: {
                id: "TenderID",
                fields: {
                    TenderID: { type: "number", editable: false, nullable: false, defaultValue: 0 },
                    ReceiptHeaderID: { type: "number", editable: false, nullable: false, defaultValue: headersGridRef.data.ReceiptHeaderID },
                    Description: { validation: { required: false }, type: "string" },
                    Amount: { validation: { required: true }, type: "number" },
                    PaymentMethodId: { validation: { required: true }, type: "number", nullable: false, defaultValue: 2 /*Check*/ },
                    IsReceiptPosted: { type: "boolean" }
                }
            },
            errors: "Errors"
        },
        change: function (e) {

        },
        error: function (e) {
            if (e && e.errors) {
                if (e.errors["_deleteKey"]) {
                    notify.showError(e.errors["_deleteKey"].errors[0]);
                } else if (e.errors["_updateKey"]) {
                    notify.showError(e.errors["_updateKey"].errors[0]);
                } else {
                    notify.showError('An error has been occurred, please contact system admin.');
                }
            }
            this.cancelChanges();
        }
    });

    receiptTendersDiv.kendoGrid({
        dataSource: receiptsTendersDataSource,
        selectable: "row",
        resizable: true,
        scrollable: true,
        sortable: true,
        filterable: true,
        groupable: true,
        reorderable: true,
        columnMenu: true,
        navigatable: true,
        pageable: {
            refresh: true,
            input: true,
            numeric: false,
            pageSizes: [10, 20, 30, 50, 75, 100]
        },
        columns: [
            {
                field: "PaymentMethodId",
                title: "Payment Method",
                values: paymentMethods,
                width: 140
            },
            {
                field: "Description",
                title: "Description",
                width: 140
            },
            {
                field: "Amount",
                title: "Amount",
                editor: oldNumericBoxEditor,
                format: "{0:n2}",
                footerTemplate: "Total: #=kendo.toString(sum, 'C')#",
                width: 69
            },
            {
                command: [{
                    name: "destroy", visible: function (dataItem) { return !dataItem.IsReceiptPosted && hasDeleteTenderItemsPermission && (isAdmin || dataItem.ReceiptClerkId == userClerkId); }
                }],
                title: "&nbsp;", width: "140px"
            }
        ],
        editable: !headersGridRef.data.IsPosted,
        toolbar: getTenderToolbarActions(headersGridRef.data),
        change: function () {
            //selectedRows = this.select();
        },
        edit: function (e) {
            //var popupWindow = $(e.container.data('kendoWindow'));
            //e.container.data('kendoWindow').bind('activate', function (e) {
            //    $(popupWindow).attr('element').find('input:first').focus();
            //});
        },
        save: function (e) {
            //if (!e.values.Amount)
            //    return;
            //var oldModel = e.model,
            //    receiptsTenderTotal = 0,
            //    currentDataSource = this.dataSource,
            //    oldModelAmount = currentDataSource.get(oldModel.TenderID).Amount;

            //$(currentDataSource.data())
            //    .map(function (i, item) {
            //        if (item.ReceiptHeaderID == e.model.ReceiptHeaderID)
            //            if (!e.values.Amount || item.TenderID != e.model.TenderID)
            //                receiptsTenderTotal += item.Amount;
            //            else {
            //                receiptsTenderTotal += e.values.Amount;
            //            }
            //    });
            //var receiptHeaderTotal = headersDataSources.get(e.model.ReceiptHeaderID).ReceiptTotal;
            //if (receiptsTenderTotal > receiptHeaderTotal) {
            //    notify.confirm("Tenders total amount(" +
            //        receiptsTenderTotal + ") is greater than receipt total(" +
            //        receiptHeaderTotal + ")",
            //        "Are you sure you want to continue?",
            //        function (isConfirmed) {
            //            if (!isConfirmed) {
            //                currentDataSource.get(oldModel.TenderID).set("Amount", oldModelAmount);
            //            }
            //        });
            //}
        }
    });
}

function initReceiptsBodyGrid(receiptBodyDiv, headersGridRef) {

    var parentReceipt = headersGridRef;

    templatesDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: templatesReadUrl,
                dataType: "json"
            }
        },
        serverPaging: false,
        schema: {
            data: "Data",
            total: "Total",
            model: {
                id: "TemplateID",
                fields: {
                    Description: { type: "string" },
                    TemplateID: { type: "number" }
                }
            },
            errors: "Errors"
        },
        change: function (e) {

        },
        error: function (e) {
            this.cancelChanges();
        }
    });

    receiptsBodyDataSource = new kendo.data.DataSource({
        serverPaging: false,
        serverFiltering: true,
        serverSorting: false,
        transport: {
            read: {
                url: bodyReadUrl,
                //contentType: "application/json; charset=utf-8",
                dataType: "json",
            },
            update: {
                url: bodyUpdateUrl,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                type: "POST"
            },
            destroy: {
                url: bodyDestroyUrl,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                type: "POST"
            },
            create: {
                url: bodyCreateUrl,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "POST"
            },
            parameterMap: function (options, operation) {
                if (operation !== "read" && options.models) {
                    if (operation == "create")
                        return (JSON.stringify({ receiptHeaderId: headersGridRef.data.ReceiptHeaderID, receiptBodies: options.models }));
                    return JSON.stringify({ receiptBodies: options.models });
                }
                else if (operation == "read") {
                    return { receiptHeaderId: headersGridRef.data.ReceiptHeaderID };
                }
            }
        },
        sort: [{ field: "TemplateOrder", dir: "asc" }, { field: "AccountNumber", dir: "asc" }],
        batch: true,
        pageSize: 500,
        filter: { field: "ReceiptHeaderID", operator: "eq", value: headersGridRef.data.ReceiptHeaderID },
        dataBound: function () {
        },
        aggregate: [
            { field: "LineTotal", aggregate: "sum" }
        ],
        schema: {
            data: "Data",
            total: "Total",
            model: {
                id: "ReceiptBodyID",
                fields: {
                    ReceiptBodyID: { type: "number", editable: false, nullable: false, defaultValue: 0 },
                    ReceiptHeaderID: { type: "number", editable: false, nullable: false, defaultValue: headersGridRef.data.ReceiptHeaderID },
                    AccountNumber: { type: "string" },
                    LineTotal: { validation: { required: true }, type: "number", spinners: false },
                    AccountDescription: { validation: { required: true }, type: "string" },
                    TemplateOrder: { validation: { required: false }, type: "number" },
                    IsReceiptPosted: { type: "boolean" }
                }
            },
            errors: "Errors"
        },
        change: function (e) {
        },
        error: function (e) {
            if (e && e.errors) {
                if (e.errors["_deleteKey"]) {
                    notify.showError(e.errors["_deleteKey"].errors[0]);
                } else if (e.errors["_updateKey"]) {
                    notify.showError(e.errors["_updateKey"].errors[0]);
                } else {
                    notify.showError('An error has been occurred, please contact system admin.');
                }
            }
            this.cancelChanges();
        }
    });

    receiptBodyDiv.kendoGrid({
        dataSource: receiptsBodyDataSource,
        navigatable: true,
        selectable: "cell",
        resizable: true,
        scrollable: true,
        sortable: true,
        filterable: true,
        groupable: true,
        reorderable: true,
        columnMenu: true,
        pageable: {
            refresh: true,
            input: true,
            numeric: false,
            pageSizes: [500, 1000, 5000, 10000]
        },
        columns: [
            {
                field: "AccountNumber",
                title: "Account Number",
                editor: templateAutocompleteEditor,
                width: 100
            },
            {
                field: "LineTotal",
                title: "Line Total",
                editor: oldNumericBoxEditor,
                format: "{0:n2}",
                headerAttributes: { style: "text-align:right" },
                attributes: { class: "text-right editable-cell highlight-cell-text" },
                footerTemplate: "Total: #=kendo.toString(sum, 'C')#",
                width: 40
            },
            {
                field: "AccountDescription",
                title: "Template",
                editor: templateAutocompleteEditor,
                width: 100
            },
            {
                command: [{
                    name: "remove",
                    click: function (e) {
                        var tr = $(e.target).closest("tr");
                        this.removeRow(tr);
                        this.saveChanges();
                    },
                    text: "Delete",
                    className: "btn-destroy",
                    visible: function (dataItem) { return !dataItem.IsReceiptPosted && hasDeleteReceiptsBodyPermission && (isAdmin || dataItem.ReceiptClerkId == userClerkId); }
                }], title: "&nbsp;", width: "60px"
            }
        ],
        editable: !headersGridRef.data.IsPosted,
        toolbar: getBodyToolbarActions(headersGridRef.data),
        dataBound: function () {
            //highlight total cell if selected, so client would be able to overwrite values
            $(this.tbody.find(".highlight-cell-text")).on('focus', '.k-input', function () {
                var input = $(this);
                setTimeout(function () { input.select(); });
            });
            updateLineTotalsSum(this.dataSource, headersGridRef.data.ReceiptHeaderID);
        },
        change: function () {
            updateLineTotalsSum(this.dataSource, headersGridRef.data.ReceiptHeaderID);
            //selectedRows = this.select();
        },
        edit: function (e) {

        },
        save: function (e) {
            //if (!e.values.LineTotal)
            //    return;
            //var oldModel = e.model,
            //    receiptsLineTotal = 0,
            //    currentDataSource = this.dataSource,
            //    oldModelAmount = currentDataSource.get(oldModel.ReceiptBodyID).LineTotal;
            //$(currentDataSource.data())
            //    .map(function (i, item) {
            //        if (item.ReceiptHeaderID == e.model.ReceiptHeaderID)
            //            if (!e.values.LineTotal || item.ReceiptBodyID != e.model.ReceiptBodyID)
            //                receiptsLineTotal += item.LineTotal;
            //            else {
            //                receiptsLineTotal += e.values.LineTotal;
            //            }
            //    });
            //var receiptHeaderTotal = headersDataSources.get(e.model.ReceiptHeaderID).ReceiptTotal;
            //if (receiptsLineTotal > receiptHeaderTotal) {
            //    notify.confirm("Receipt body total line sum(" +
            //        receiptsLineTotal + ") is greater than receipt total(" +
            //        receiptHeaderTotal + ")",
            //        "Are you sure you want to continue?",
            //        function (isConfirmed) {
            //            if (!isConfirmed) {
            //                currentDataSource.get(oldModel.ReceiptBodyID).set("LineTotal", oldModelAmount);
            //            }
            //        });
            //}
        }
    })
        .find("table").on("keydown", onBodyGridKeydown);
}

function getBodyToolbarActions(receipt) {
    var actions = [];
    if (!receipt.IsPosted && (isAdmin || receipt.ClerkID == userClerkId)) {
        if (hasCreateReceiptsBodyPermission)
            actions.push("create");
        if (hasEditReceiptsBodyPermission)
            actions.push("save", "cancel");
    }
    actions.push({
        text: "",
        template: "<a class='k-button k-grid-showtotal'></a>"
    });
    return actions;
}

function getTenderToolbarActions(receipt) {
    var actions = [];
    if (!receipt.IsPosted && (isAdmin || receipt.ClerkID == userClerkId)) {
        if (hasAddTenderItemsPermission)
            actions.push("create");
        if (hasEditTenderItemsPermission)
            actions.push("save", "cancel");
    }
    return actions;
}

function getHeaderToolbarActions() {
    var actions = [];
    if (hasCreateReceiptItemsPermission)
        actions.push("create");

    actions.push({
        text: "",
        template:
            "<a class='k-button k-grid-PrintReceipt'><span class='k-icon k-i-pdf'></span>Download Receipt</a>"
    });

    if (hasPostPermission)
        actions.push({
            text: "",
            template: "<a class='k-button k-grid-PostCheck'><span class='k-icon k-i-group'></span>Post</a>"
        });
    return actions;
}

function updateLineTotalsSum(currentDataSource, receiptHeaderId) {
    var receiptsLineTotal = 0;
    $(currentDataSource.data())
        .map(function (i, item) {
            if (item.ReceiptHeaderID == receiptHeaderId)
                receiptsLineTotal += item.LineTotal;
        });
    $('.k-grid-showtotal').text('Line Items Total: ' + kendo.toString(receiptsLineTotal, 'C'));
}

function templateAutocompleteEditor(container, options) {
    var model = options.model;
    var parentContainer = container.parent();
    $('<input data-bind="value:' + options.field + '" />')
        .appendTo(container)
        .kendoAutoComplete({
            placeholder: "Enter value ...",
            //suggest: true,
            dataTextField: 'Description',
            filter: "contains",
            minLength: 2,
            dataSource: {
                type: "json",
                serverFiltering: true,
                serverPaging: true,
                transport: {
                    read: {
                        url: gcAccountsDetailsUrl,
                        //type: "POST",
                        dataType: "json",
                        data: function (param) {
                            var searchParam = param.filter.filters[0];
                            var field = options.field === "AccountNumber" ? "Account" : "Description";
                            //return searchParam;
                            return { value: searchParam.value, operator: "contains", field: field, ignoreCase: true };
                        }
                    }
                }
            },
            select: function (e) {
                autoCompleteOnSelect(this, parentContainer, options.field, model, e);
            }
        });
}

function autoCompleteOnSelect(that, parentContainer, fieldName, model, e) {
    var dataItem = that.dataItem(e.item.index());
    var accountNumberElem = parentContainer.find('[data-bind="value:AccountNumber"]'),
        descElem = parentContainer.find('[data-bind="value:AccountDescription"]');

    var acctNumber = dataItem.Fund +
        '.' +
        dataItem.Dept +
        '.' +
        dataItem.Program +
        '.' +
        dataItem.Project +
        '.' +
        dataItem.BaseElementObjectDetail;
    var description = dataItem.Description.split(']')[1].trim();

    accountNumberElem.val(acctNumber);
    model.set("AccountNumber", acctNumber);
    descElem.val(description);
    model.set("AccountDescription", description);
    model.set("TemplateID", dataItem.TemplateID);
    model.set("AccountDataSource", dataItem.DataSource);
    model.set("IsRemote", true);
    e.preventDefault();
}


//var ignoreKey = false;
//var handler = function (e) {
//    if (ignoreKey) {
//        e.preventDefault();
//        return;
//    }
//    if (e.keyCode == 38 || e.keyCode == 40) {
//        var pos = this.selectionStart;
//        this.value = (e.keyCode == 38 ? 1 : -1) + parseInt(this.value, 10);
//        this.selectionStart = pos; this.selectionEnd = pos;

//        ignoreKey = true; setTimeout(function () { ignoreKey = false }, 1);
//        e.preventDefault();
//    }
//};
//input.addEventListener('keydown', handler, false);
//input.addEventListener('keypress', handler, false);


function onBodyGridKeydown(e) {
    if (e.keyCode === kendo.keys.TAB) {
        var grid = $(this).closest("[data-role=grid]").data("kendoGrid");
        var current = grid.current();
        if (!current.hasClass("editable-cell")) {
            //search the next row
            var row = current.parent();
            var cell;

            if (!e.shiftKey)
                cell = row.next().children(".editable-cell:first");
            else
                cell = row.prev().children(".editable-cell:first");
            grid.current(cell);
            grid.editCell(cell[0]);
            setTimeout(function () { $(cell[0]).find(':input.k-input:first').select(); }, 100);
        }
    }
};

function oldNumericBoxEditor(container, options) {
    $('<input data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            spinners: false
        }).off("keydown");
}

function templateDropDownEditor(container, options) {
    var rowContainer = container, fieldName = options.field;
    $('<input required data-text-field="text" data-value-field="value" data-bind="value:' + fieldName + '" />')
        .appendTo(container)
        .kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            filter: "contains",
            height: 400,
            dataSource: {
                transport: {
                    dataType: "json",
                    read: templatesListUrl
                },
                batch: false,
                pageSize: 1000,
                group: { field: "DepartmentName" },
                schema: {
                    model: {
                        id: "value",
                        fields: {
                            value: { editable: false, nullable: false },
                            text: { validation: { required: true } },
                            DepartmentName: { validation: { required: true } },
                        }
                    }
                }
            },
            select: function (e) {
                //debugger;
                if (fieldName == 'TemplateID') {
                    var accountNumber = e.item.text().split("|")[1].trim();
                    $(rowContainer).parents('tr:first').find('td:first').text(accountNumber);
                } else {
                    var grid = receiptBodyDiv.data("kendoGrid");
                    var model = grid.dataItem($(rowContainer).parents('tr:first'));
                    model.set("TemplateID", e.sender.dataItem(e.item).value);
                }
            }
        });
}

function isBlank(str) {
    return (!str || /^\s*$/.test(str));
}

function lessThanOrEqual(val, charLength) {
    return val.length <= charLength;
}
