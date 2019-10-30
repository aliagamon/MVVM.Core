using System;

namespace MVVM.Core.Utils
{
    public static class Utils
    {
        public static void AssertIsOfType(this object instance, Type type)
        {
            if (instance != null)
            {
                if (instance.GetType() != type)
                {
                    throw new Exception($"expected to be of type: {type} but was: {instance.GetType().Name}");
                }
            }
            else
            {
                throw new Exception($"expected to be of type: {type} but was <null>");
            }
        }
    }
}