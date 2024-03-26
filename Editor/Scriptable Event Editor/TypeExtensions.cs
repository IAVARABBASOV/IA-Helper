#if UNITY_EDITOR

public static class TypeExtensions
{
    public static bool IsSubclassOfGeneric(this System.Type type, System.Type genericType)
    {
        while (type != null && type != typeof(object))
        {
            System.Type currentType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if (genericType == currentType)
                return true;

            type = type.BaseType;
        }

        return false;
    }
}
#endif