using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Meceqs
{
    [DebuggerStepThrough]
    internal static class Check
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEmpty(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}