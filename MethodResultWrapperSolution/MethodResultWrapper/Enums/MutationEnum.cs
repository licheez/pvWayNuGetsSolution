using System;

namespace pvWay.MethodResultWrapper.Enums
{
    public enum MutationEnum
    {
        None,
        Create,
        Update,
        Deactivate
    }

    public class EnumMutation
    {
        private const string None = "N";
        private const string Create = "C";
        private const string Update = "U";
        private const string Deactivate = "D";

        public static string GetCode(MutationEnum value)
        {
            switch (value)
            {
                case MutationEnum.None:
                    return None;
                case MutationEnum.Create:
                    return Create;
                case MutationEnum.Update:
                    return Update;
                case MutationEnum.Deactivate:
                    return Deactivate;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public static MutationEnum GetValue(string code)
        {
            switch (code)
            {
                case None:
                    return MutationEnum.None;
                case Create:
                    return MutationEnum.Create;
                case Update:
                    return MutationEnum.Update;
                case Deactivate:
                    return MutationEnum.Deactivate;
                default:
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }

    }
}