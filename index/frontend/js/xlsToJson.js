function fileReader(oEvent, callback) 
{
    var oFile = oEvent.target.files[0];
    var sFilename = oFile.name;

    var reader = new FileReader();
    var result = {};

    reader.onload = function (e) {
        var data = e.target.result;
        data = new Uint8Array(data);
        var workbook = XLSX.read(data, {type: 'array'});
        //console.log(workbook);
        var result = {};
        workbook.SheetNames.forEach(function (sheetName) {
            var roa = XLSX.utils.sheet_to_json(workbook.Sheets[sheetName], {header: 1});
            if (roa.length) result["dados"] = roa;
        });
        // see the result, caution: it works after reader event is done.
        //console.log(result);
        callback(result);
    };
    reader.readAsArrayBuffer(oFile);
}