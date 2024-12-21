// Function to check if the upload size is greater then the given number.

function VerifyUploadSize(UploadFieldID, MaxSizeInBytes)
{
    var ActualSizeInBytes = 0;
    var field = document.getElementById(UploadFieldID);
    var files = field.files
    for (let i = 0; i < files.length; ++i)
    {
        ActualSizeInBytes += files[i].size;
    }
    if (ActualSizeInBytes > MaxSizeInBytes)
    {
        alert("Max upload size is " + parseInt(MaxSizeInBytes/1024/1024) + "MB,"
            + " but got " + parseInt(ActualSizeInBytes/1024/1024) + "MB");
        return false;
    }
    return true;
}
