namespace PvWay.LoggerService.MethodResultWrapper.nc6;

public enum DsoHttpResultMutationEnum
{
    None,
    Create,
    Update,
    Delete
}

public class EnumDsoHttpResultMutation
{
    private const string None = "N";
    private const string Create = "C";
    private const string Update = "U";
    private const string Delete = "D";

    public static string GetCode(DsoHttpResultMutationEnum value)
    {
        return value switch
        {
            DsoHttpResultMutationEnum.None => None,
            DsoHttpResultMutationEnum.Create => Create,
            DsoHttpResultMutationEnum.Update => Update,
            DsoHttpResultMutationEnum.Delete => Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

}