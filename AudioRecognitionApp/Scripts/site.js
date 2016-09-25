$(document).ready(function () {
    
    $("#selectedFile").change(function () {
        var files = this.files;
        var fileName = "";
        for (var i = 0; i < this.files.length; i++) {
            fileName = fileName + "/" + this.files[i].name;
        }
        $("#file-path").val(fileName);

    });
});