const equipList = {
    "title": {
        "leftHeaderTitle": "Equipments",
        "exportType": "Export Classification",
        "typeSelect": "Classification Select",
        "exportCurve": "Curve Export",
        "selectDevice": "Select Equipment",
        "selectYc": "Select Telemetry",
        "selectYx": "Select Teleindication",
        "ycData": "Telemetry Data",
        "yxData": "Teleindication Data"
    },
    "input": {
        "inputSearchEquip": "Enter Equipment Name",
        "searchYc": "Enter Telemetry Name",
        "searchYx": "Enter Teleindication Name",
        "searchSet": "Enter Setting Name",
        "selectStartTime": "Select Begin Time",
        "selectEndTime": "Start Time",
        "startTime": "Start Time",
        "endTime": "End Time"
    },
    "tips": {
        "exportTips": "Export All Current Failed And Offline Equipments In Batches",
        "execute": "Execute Or Not?",
        "titleTip": "Tips",
        "timeHorizon": "Select The Time Range For The Query!",
        "timeRange": "Select A Reasonable Time Range.",
        "timeDays": "The Query Days Range Cannot Be Greater Than 7 Days",
        "queryHistory": "The Query History Curve Is Empty!",
        "curveValue": "No Real Time Value For The Current Measurement Point Offline!",
        "selectDevice": "Select Equipment",
        "success": "Operate Succeed",
        "noRelatedVideo": "There Is No Video Associated With The Current Measurement Point",
        "videoConnectError": "Streaming Media Service Connection Failed",
        "lessOnePoint": "Select At Least One Measurement Point To Export",
        "exportByDayTip": "A Log File Is Generated For Each Equipment For Each Day",
        "exportByCombinedTip": "Only One Log File Will Be Generated Per Equipment",
        "noSelect": "No Option At This Time",
        "deviceNoConnect": "Device No Connect"
    },
    "tabs": {
        "ycNm": "Telemetry",
        "yxNm": "Teleindication",
        "setNm": "Set"
    },
    "table": {
        "listTitleYc": {
            "equipNo": "Equip Number",
            "ycYxNo": "Number",
            "alarmState": "Alarm Status",
            "ycYxName": "Name",
            "value": "Real Time Value",
            "quantity": "Number Of All Alarms",
            "curve": "Curve",
            "location": "Location",
            "video": "Video",
            "asset": "Asset",
            "suggestion": "Handling Comments"
        },
        "listTitleYx": {
            "ycYxNo": "Number",
            "alarmState": "Alarm Status",
            "ycYxName": "Name",
            "value": "Real Time Status",
            "quantity": "Number Of All Alarms",
            "curve": "Curve",
            "location": "Location",
            "video": "Video",
            "asset": "Asset",
            "suggestion": "Handling Comments"
        }
    },
    "label": {
        "inputValue": "Input Value:",
        "date": "Date",
        "value": "Value",
        "list": "Display List"
    },
    "state": [
        "Normal",
        "Alarm",
        "Offline"
    ],
    "button": {
        "confirm": "Confirm",
        "cancel": "Cancel",
        "curve": "Real Time Curve",
        "history": "Historical Curve",
        "list": "History List",
        "exportDevice": "Export Equipment",
        "exportCurve": "Export Curve",
        "exportByDay": "Export By Day",
        "exportByCombined": "Export Together",
        "export": "Export",
        "search": "Search",
        "RealTimeValue": "RealTime Value"
    },
    "echart": {
        "legend": {
            "realTimeValue": "Real Time Value",
            "upperLimitValue": "Upper Limit Value",
            "lowerLimitValue": "Lower Limit Value"
        },
        "toolTip": {
            "time": "Time",
            "currentValue": "Value"
        }
    },
    "excelName": {
        "alarmDevice": "Alarm Equipment",
        "outLineDevice": "Offline Equipment",
        "normalDevice": "Normal Equipment",
        "allDevice": "All Equipments"
    },
    "publics": {
        "button": {
            "confirm": "Confirm",
            "cancel": "Cancel",
            "deletes": "Delete",
            "edit": "Edit",
            "reset": "Reset",
            "empty": "Empty",
            "download": "Download",
            "exports": "Export",
            "imports": "Import",
            "filter": "Filter",
            "selectAll": "Select All",
            "add": "Add",
            "save": "Save",
            "search": "Search",
            "upload": "Upload",
            "upgrade": "Upgrade"
        },
        "tips": {
            "addSuccess": "Add Succeed",
            "addFail": "Add Failed",
            "saveSuccess": "Save Succeed",
            "saveFail": "Save Failed",
            "editSuccess": "Edit Succeed",
            "editFail": "Edit Failed",
            "importSuccess": "Import Succeeded",
            "importFail": "Import Failed",
            "exportSuccess": "Export Succeed",
            "exportFail": "Export Failed",
            "deleteSuccess": "Delete Succeed",
            "deleteFail": "Delete Failed",
            "serverErr": "Service Exception",
            "uploadSuccess": "Upload Was Succeed",
            "uploadFail": "Upload Failed",
            "upgradeSuccess": "Upgrade Succeed",
            "upgradeFail": "Upgrade Failed",
            "setIssueSuccess": "Instruction Sent Succeed",
            "setIssueError": "Instruction Sent Failed"
        },
        "noData": "No Content",
        "platform": "Integrated Management Platform",
        "warnings": {
            "STTimeCantGreaterEndTime": "Begin Time Cannot Be Greater Than End Time",
            "timeCantMoreThanNinetyDay": "The Export Interval Must Not Exceed 90 Days!",
            "processing": "During Data Processing, You Can Close The Pop-up Window To Operate Other Pages!",
            "selectStartTime": "Select The Begin Time Of Export",
            "selectEndTime": "Select The End Time Of The Export",
            "readyToExport": "History Curve Data Is Processed And Will Be Exported Soon"
        }
    }
}

export default equipList