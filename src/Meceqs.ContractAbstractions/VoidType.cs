using System;

namespace Meceqs
{
    /// <summary>
    /// Simulates a void type, because <c>void</c> can not be used as a type in Generics.
    /// </summary>
    /// <remarks>Based on https://github.com/jbogard/MediatR/blob/master/src/MediatR/Unit.cs</remarks>
    public struct VoidType : IEquatable<VoidType>, IComparable<VoidType>, IComparable
    {
        public static readonly VoidType Value = new VoidType();

        public int CompareTo(VoidType other)
        {
            return 0;
        }

        public int CompareTo(object obj)
        {
            return 0;
        }


        public bool Equals(VoidType other)
        {
            return true;
        }


        public override bool Equals(object obj)
        {
            return obj is VoidType;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "()";
        }

        public static bool operator ==(VoidType first, VoidType second)
        {
            return true;
        }

        public static bool operator !=(VoidType first, VoidType second)
        {
            return false;
        }
    }
}