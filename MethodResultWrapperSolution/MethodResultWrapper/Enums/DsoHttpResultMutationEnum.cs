using System;

namespace pvWay.MethodResultWrapper.Enums
{
    public enum DsoHttpResultMutationEnum
    {
        None,
        Create,
        Update,
        Deactivate
    }

    public class EnumDsoHttpResultMutation
    {
        private const string None = "N";
        private const string Create = "C";
        private const string Update = "U";
        private const string Deactivate = "D";

        public static string GetCode(DsoHttpResultMutationEnum value)
        {
            switch (value)
            {
                case DsoHttpResultMutationEnum.None:
                    return None;
                case DsoHttpResultMutationEnum.Create:
                    return Create;
                case DsoHttpResultMutationEnum.Update:
                    return Update;
                case DsoHttpResultMutationEnum.Deactivate:
                    return Deactivate;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

    }
}