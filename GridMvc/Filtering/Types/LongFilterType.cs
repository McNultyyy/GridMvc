﻿using System;

namespace GridMvc.Filtering.Types
{
    /// <summary>
    /// Object contains some logic for filtering Int64 columns
    /// </summary>
    internal sealed class LongFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (Int64); }
        }

        public override GridFilterType GetValidType(GridFilterType type)
        {
            switch (type)
            {
                case GridFilterType.Equals:
                case GridFilterType.GreaterThan:
                case GridFilterType.LessThan:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }

        public override object GetTypedValue(string value)
        {
            long i;
            if (!long.TryParse(value, out i))
                return null;
            return i;
        }
    }
}