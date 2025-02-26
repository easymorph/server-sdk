using System.ComponentModel;

namespace Morph.Server.Sdk.Model
{
    public enum ParameterType
    {
        [Description("Text or number")]
        Text,
        [Description("File name")]
        FilePath,
        [Description("Date")]
        Date,
        [Description("Calculated")]
        Calculated,
        [Description("Folder path")]
        FolderPath,
        [Description("Checkbox")]
        Checkbox,
        [Description("Fixed list")]
        FixedList,
        [Description("Multiple choice")]
        MultipleChoice,
        [Description("File Uploads")]
        UploadsParameter

    }


    

}
