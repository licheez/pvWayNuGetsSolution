namespace PvWay.LoggerService.MethodResultWrapper.nc6;

public enum DsoHttpResultMutationEnu
{
    None,
    Create,
    Update,
    Delete
}

public static class EnumDsoHttpResultMutation
{
    private const string None = "N";
    private const string Create = "C";
    private const string Update = "U";
    private const string Delete = "D";

    public static string GetCode(DsoHttpResultMutationEnu value)
    {
        return value switch
        {
            DsoHttpResultMutationEnu.None => None,
            DsoHttpResultMutationEnu.Create => Create,
            DsoHttpResultMutationEnu.Update => Update,
            DsoHttpResultMutationEnu.Delete => Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

}