namespace Pie.Freetype.Native;

public enum FT_Error
{
    Ok,
    CannotOpenResource,
    UnknownFileFormat,
    InvalidFileFormat,
    InvalidVersion,
    LowerModuleVersion,
    InvalidArgument,
    UnimplementedFeature,
    InvalidTable,
    InvalidOffset,
    ArrayTooLarge,
    MissingModule,
    MissingProperty
    
    // TODO: the rest of the errors https://freetype.org/freetype2/docs/reference/ft2-error_code_values.html
    // ... there are a lot of them
}